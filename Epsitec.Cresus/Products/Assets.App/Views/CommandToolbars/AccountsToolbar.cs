//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class AccountsToolbar : AbstractTreeTableToolbar
	{
		public AccountsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.buttonDateRange  = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.DateRange);
			this.buttonGraphic    = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.Graphic);

			this.buttonFirst      = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.First);
			this.buttonPrev       = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.Prev);
			this.buttonNext       = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.Next);
			this.buttonLast       = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.Last);

			this.separator1       = this.CreateSeparator     (DockStyle.None);
			
			this.buttonCompactAll = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.CompactAll);
			this.buttonCompactOne = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.CompactOne);
			this.buttonExpandOne  = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.ExpandOne);
			this.buttonExpandAll  = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.ExpandAll);
			
			this.separator2       = this.CreateSeparator     (DockStyle.None);
			this.separator3       = this.CreateSeparator     (DockStyle.None);
			this.separator4       = this.CreateSeparator     (DockStyle.None);

			this.buttonImport     = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.Import);
			this.buttonExport     = this.CreateCommandButton (DockStyle.None, Res.Commands.Accounts.Export);
		}
	}
}
