//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TreeTableToolbar : AbstractCommandToolbar
	{
		public override void CreateUI(Widget parent)
		{
			this.CreateToolbar (parent, 24+6);  // les icônes actuelles font 24
			this.UpdateCommandButtons ();
		}


		protected override void UpdateCommandButtons()
		{
			this.UpdateCommandButton (this.buttonNew,      ToolbarCommand.New);
			this.UpdateCommandButton (this.buttonDelete,   ToolbarCommand.Delete);
			this.UpdateCommandButton (this.buttonEdit,     ToolbarCommand.Edit);
			this.UpdateCommandButton (this.buttonDeselect, ToolbarCommand.Deselect);
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

			this.buttonNew      = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.New ,     "TreeTable.New",      "Nouvel objet");
			this.buttonDelete   = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Delete ,  "TreeTable.Delete",   "Supprimer l'objet");
			this.buttonEdit     = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Edit,     "TreeTable.Edit",     "Modifier l'objet");
			this.buttonDeselect = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Deselect, "TreeTable.Deselect", "Désélectionne l'objet");
		}


		private IconButton buttonNew;
		private IconButton buttonDelete;
		private IconButton buttonEdit;
		private IconButton buttonDeselect;
	}
}
