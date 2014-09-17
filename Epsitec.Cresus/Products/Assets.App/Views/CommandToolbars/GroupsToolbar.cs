//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class GroupsToolbar : AbstractTreeTableToolbar
	{
		public GroupsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.First);
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Prev);
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Next);
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, Res.Commands.TreeTable.Last);

			this.separator1       = this.CreateSeparator     (DockStyle.None);
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			this.separator3       = this.CreateSeparator     (DockStyle.None);

			this.buttonNew        = this.CreateCommandButton (DockStyle.None, Res.Commands.Groups.New);
			this.buttonDelete     = this.CreateCommandButton (DockStyle.None, Res.Commands.Groups.Delete);
			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, Res.Commands.Groups.Deselect);

			this.separator4       = this.CreateSeparator     (DockStyle.None);

			this.buttonCopy       = this.CreateCommandButton (DockStyle.None, Res.Commands.Groups.Copy);
			this.buttonPaste      = this.CreateCommandButton (DockStyle.None, Res.Commands.Groups.Paste);
			this.buttonExport     = this.CreateCommandButton (DockStyle.None, Res.Commands.Groups.Export);
		}
	}
}
