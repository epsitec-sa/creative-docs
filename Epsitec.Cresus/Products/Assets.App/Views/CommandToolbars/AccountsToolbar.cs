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

			this.CreateButton (Res.Commands.Accounts.DateRange, 0);
			this.CreateButton (Res.Commands.Accounts.Graphic, 1);

			this.CreateButton (Res.Commands.Accounts.First, 5);
			this.CreateButton (Res.Commands.Accounts.Prev, 4);
			this.CreateButton (Res.Commands.Accounts.Next, 4);
			this.CreateButton (Res.Commands.Accounts.Last, 5);

			this.CreateSeparator (2);
			
			this.CreateButton (Res.Commands.Accounts.CompactAll, 2);
			this.CreateButton (Res.Commands.Accounts.CompactOne, 3);
			this.CreateButton (Res.Commands.Accounts.ExpandOne, 3);
			this.CreateButton (Res.Commands.Accounts.ExpandAll, 2);
			
			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Accounts.Import, 0);
			this.CreateButton (Res.Commands.Accounts.Export, 0);
		}
	}
}
