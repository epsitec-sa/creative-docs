//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// Toolbar de la vue des plans comptables.
	/// </summary>
	public class AccountsToolbar : AbstractCommandToolbar
	{
		public AccountsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			//	La valeur zéro pour superficiality indique les commandes importantes.
			//	Les plus grandes valeurs correspondent à des commandes de moins
			//	en moins importantes (de plus en plus superficielles), qui seront
			//	absentes si la place à disposition dans la toolbar vient à manquer.

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

			this.helplineButton =
			this.CreateButton (Res.Commands.Accounts.Import, 0);
			this.CreateButton (Res.Commands.Accounts.Export, 0);

			this.CreateSearchController (SearchKind.Accounts, 6);
		}
	}
}
