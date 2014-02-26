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
		public List<ViewType>					ViewTypes;
		public ViewType							SelectedViewType;

		protected override Size					DialogSize
		{
			get
			{
				return new Size (ViewPopup.margins*2 + ViewPopup.buttonSize*ViewTypes.Count, ViewPopup.margins*2 + ViewPopup.buttonSize);
			}
		}

		public override void CreateUI()
		{
			var frame = this.CreateFrame (ViewPopup.margins, ViewPopup.margins, ViewPopup.buttonSize*ViewTypes.Count, ViewPopup.buttonSize);

			foreach (var viewType in this.ViewTypes)
			{
				this.CreateButton (frame, viewType);
			}
		}

		protected IconButton CreateButton(Widget parent, ViewType viewType)
		{
			var icon = StaticDescriptions.GetViewTypeIcon (viewType);
			var text = StaticDescriptions.GetViewTypeDescription (viewType);

			var button = new IconButton
			{
				Parent        = parent,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				IconUri       = Misc.GetResourceIconUri (icon),
				ButtonStyle   = ButtonStyle.ActivableIcon,
				ActiveState   = (viewType == this.SelectedViewType) ? ActiveState.Yes : ActiveState.No,
				PreferredSize = new Size (ViewPopup.buttonSize, ViewPopup.buttonSize),
			};

			ToolTip.Default.SetToolTip (button, text);

			button.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnViewTypeClicked (viewType);
			};

			return button;
		}



		#region Events handler
		private void OnViewTypeClicked(ViewType viewType)
		{
			this.ViewTypeClicked.Raise (this, viewType);
		}

		public event EventHandler<ViewType> ViewTypeClicked;
		#endregion


		private const int margins  = 3;
		private const int buttonSize = AbstractCommandToolbar.primaryToolbarHeight;

	}
}