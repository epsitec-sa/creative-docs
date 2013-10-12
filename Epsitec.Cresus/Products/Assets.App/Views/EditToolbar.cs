//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditToolbar : AbstractCommandToolbar
	{
		public override void CreateUI(Widget parent)
		{
			this.CreateToolbar (parent, AbstractCommandToolbar.SecondaryToolbarHeight);
			this.UpdateCommandButtons ();
		}


		protected override void UpdateCommandButtons()
		{
			this.UpdateCommandButton (this.buttonAccept, ToolbarCommand.Accept);
			this.UpdateCommandButton (this.buttonCancel, ToolbarCommand.Cancel);
		}


		protected override void CreateToolbar(Widget parent, int size)
		{
			var toolbar = new HToolBar
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = size,
				Padding         = new Margins (0),
			};

			this.buttonAccept = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Accept, "Edit.Accept", "Accepter les modifications");
			this.buttonCancel = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Cancel, "Edit.Cancel", "Annuler les modifications");
		}


		private IconButton buttonAccept;
		private IconButton buttonCancel;
	}
}
