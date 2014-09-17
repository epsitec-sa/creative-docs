//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class WarningsToolbar : AbstractTreeTableToolbar
	{
		public WarningsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, Res.Commands.Warnings.First);
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, Res.Commands.Warnings.Prev);
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, Res.Commands.Warnings.Next);
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, Res.Commands.Warnings.Last);

			this.separator1       = this.CreateSeparator     (DockStyle.None);
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			this.separator3       = this.CreateSeparator     (DockStyle.None);

			this.buttonDeselect   = this.CreateCommandButton (DockStyle.None, Res.Commands.Warnings.Deselect);

			this.separator4       = this.CreateSeparator     (DockStyle.None);

			this.buttonGoto       = this.CreateCommandButton (DockStyle.None, Res.Commands.Warnings.Goto);
		}
	}
}
