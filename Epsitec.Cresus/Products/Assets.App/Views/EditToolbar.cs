//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditToolbar : AbstractCommandToolbar
	{
		protected override void CreateCommands()
		{
			this.SetCommand (ToolbarCommand.Accept, "Edit.Accept", "Accepter les modifications");
			this.SetCommand (ToolbarCommand.Cancel, "Edit.Cancel", "Annuler les modifications");
		}


		public override FrameBox CreateUI(Widget parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonAccept = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Accept);
			this.buttonCancel = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.Cancel);

			return this.toolbar;
		}


		private IconButton buttonAccept;
		private IconButton buttonCancel;
	}
}
