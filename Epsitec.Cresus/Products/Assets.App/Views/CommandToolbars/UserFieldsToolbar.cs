//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class UserFieldsToolbar : AbstractTreeTableToolbar
	{
		public UserFieldsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.First);
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.Prev);
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.Next);
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.Last);

			this.separator1       = this.CreateSeparator     (DockStyle.None);
			this.separator2       = this.CreateSeparator     (DockStyle.None);

			this.buttonMoveTop    = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.MoveTop);
			this.buttonMoveUp     = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.MoveUp);
			this.buttonMoveDown   = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.MoveDown);
			this.buttonMoveBottom = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.MoveBottom);

			this.separator3       = this.CreateSeparator     (DockStyle.None);

			this.buttonNew        = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.New);
			this.buttonDelete     = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.Delete);
			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.Deselect);

			this.separator4       = this.CreateSeparator     (DockStyle.None);

			this.buttonCopy       = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.Copy);
			this.buttonPaste      = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.Paste);
			this.buttonExport     = this.CreateCommandButton (DockStyle.None, Res.Commands.UserFields.Export);
		}
	}
}
