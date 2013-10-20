//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TreeTableToolbar : AbstractCommandToolbar
	{
		public override void CreateUI(Widget parent)
		{
			this.CreateToolbar (parent, AbstractCommandToolbar.SecondaryToolbarHeight);
			this.UpdateCommandButtons ();
		}


		protected override void UpdateCommandButtons()
		{
			this.UpdateCommandButton (this.buttonFirst,    ToolbarCommand.First);
			this.UpdateCommandButton (this.buttonPrev,     ToolbarCommand.Prev);
			this.UpdateCommandButton (this.buttonNext,     ToolbarCommand.Next);
			this.UpdateCommandButton (this.buttonLast,     ToolbarCommand.Last);
			this.UpdateCommandButton (this.buttonNew,      ToolbarCommand.New);
			this.UpdateCommandButton (this.buttonDelete,   ToolbarCommand.Delete);
			this.UpdateCommandButton (this.buttonDeselect, ToolbarCommand.Deselect);
		}


		protected override void CreateToolbar(Widget parent, int size)
		{
			var toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = size,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonFirst    = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.First,    "TreeTable.First",    "Retour sur la première ligne");
			this.buttonPrev     = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Prev,     "TreeTable.Prev",     "Recule sur la ligne précédente");
			this.buttonNext     = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Next,     "TreeTable.Next",     "Avance sur la ligne suivante");
			this.buttonLast     = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Last,     "TreeTable.Last",     "Avance sur la dernière ligne");
			this.buttonNew      = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.New ,     "TreeTable.New",      "Nouvel ligne");
			this.buttonDelete   = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Delete ,  "TreeTable.Delete",   "Supprimer la ligne");
			this.buttonDeselect = this.CreateCommandButton (toolbar, DockStyle.Left, ToolbarCommand.Deselect, "TreeTable.Deselect", "Désélectionne la ligne");

			this.buttonNew.Margins = new Margins (20, 0, 0, 0);
		}


		private IconButton buttonFirst;
		private IconButton buttonPrev;
		private IconButton buttonNext;
		private IconButton buttonLast;
		private IconButton buttonNew;
		private IconButton buttonDelete;
		private IconButton buttonDeselect;
	}
}
