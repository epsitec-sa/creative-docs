//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditToolbar : AbstractCommandToolbar
	{
		public override FrameBox CreateUI(Widget parent)
		{
			var toolbar = this.CreateToolbar (parent, AbstractCommandToolbar.secondaryToolbarHeight);
			this.UpdateCommandButtons ();

			return toolbar;
		}


		protected override void UpdateCommandButtons()
		{
			this.UpdateCommandButton (this.buttonAccept, ToolbarCommand.Accept);
			this.UpdateCommandButton (this.buttonCancel, ToolbarCommand.Cancel);
		}


		protected override FrameBox CreateToolbar(Widget parent, int size)
		{
			var toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = size,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonAccept = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Accept, "Edit.Accept", "Accepter les modifications");
			this.buttonCancel = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Cancel, "Edit.Cancel", "Annuler les modifications");

			return toolbar;
		}


		private IconButton buttonAccept;
		private IconButton buttonCancel;
	}
}
