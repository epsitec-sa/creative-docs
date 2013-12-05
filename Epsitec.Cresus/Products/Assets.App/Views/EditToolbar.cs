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
		public override FrameBox CreateUI(Widget parent)
		{
			var toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
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
