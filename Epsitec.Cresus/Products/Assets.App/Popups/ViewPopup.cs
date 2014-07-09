//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class ViewPopup : AbstractPopup
	{
		public List<ViewTypeKind>				ViewTypeKinds;
		public ViewTypeKind						SelectedViewType;

		protected override Size					DialogSize
		{
			get
			{
				return new Size (ViewPopup.margins*2 + ViewPopup.buttonSize*this.ViewTypeKinds.Count, ViewPopup.margins*2 + ViewPopup.buttonSize);
			}
		}

		public override void CreateUI()
		{
			var frame = this.CreateFrame (ViewPopup.margins, ViewPopup.margins, ViewPopup.buttonSize*this.ViewTypeKinds.Count, ViewPopup.buttonSize);

			foreach (var kind in this.ViewTypeKinds)
			{
				this.CreateButton (frame, kind);
			}
		}

		protected IconButton CreateButton(Widget parent, ViewTypeKind kind)
		{
			var icon = StaticDescriptions.GetViewTypeIcon (kind);
			var text = StaticDescriptions.GetViewTypeDescription (kind);

			var button = new IconButton
			{
				Parent        = parent,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				IconUri       = Misc.GetResourceIconUri (icon),
				ButtonStyle   = ButtonStyle.ActivableIcon,
				ActiveState   = (kind == this.SelectedViewType) ? ActiveState.Yes : ActiveState.No,
				PreferredSize = new Size (ViewPopup.buttonSize, ViewPopup.buttonSize),
			};

			ToolTip.Default.SetToolTip (button, text);

			button.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnViewTypeClicked (kind);
			};

			return button;
		}



		#region Events handler
		private void OnViewTypeClicked(ViewTypeKind kind)
		{
			this.ViewTypeClicked.Raise (this, kind);
		}

		public event EventHandler<ViewTypeKind> ViewTypeClicked;
		#endregion


		private const int margins    = 5;
		private const int buttonSize = AbstractCommandToolbar.primaryToolbarHeight;

	}
}