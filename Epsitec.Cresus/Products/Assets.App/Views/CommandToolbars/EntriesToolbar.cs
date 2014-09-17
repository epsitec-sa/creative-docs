//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class EntriesToolbar : AbstractTreeTableToolbar
	{
		public EntriesToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.buttonGraphic    = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Graphic);

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.First);
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Prev);
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Next);
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Last);

			this.separator1       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonCompactAll = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.CompactAll);
			this.buttonCompactOne = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.CompactOne);
			this.buttonExpandOne  = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.ExpandOne);
			this.buttonExpandAll  = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.ExpandAll);
			
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			this.separator3       = this.CreateSeparator     (DockStyle.None);

			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, Res.Commands.Entries.Deselect);

			this.separator4       = this.CreateSeparator     (DockStyle.None);

			this.buttonExport     = this.CreateCommandButton (DockStyle.None, Res.Commands.Entries.Export);
		}
	}
}
