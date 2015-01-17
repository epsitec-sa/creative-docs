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
	/// Toolbar de la vue des écritures comptables.
	/// </summary>
	public class EntriesToolbar : AbstractCommandToolbar
	{
		public EntriesToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Entries.First, 3);
			this.CreateButton (Res.Commands.Entries.Prev, 2);
			this.CreateButton (Res.Commands.Entries.Next, 2);
			this.CreateButton (Res.Commands.Entries.Last, 3);

			this.CreateSeparator (2);

			this.CreateButton (Res.Commands.Entries.CompactAll, 0);
			this.CreateButton (Res.Commands.Entries.ExpandAll, 0);

			this.CreateSeparator (4);

			this.CreateButton (Res.Commands.Entries.Deselect, 4);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Entries.Export, 1);

			this.CreateSearchController (SearchKind.Entries, 5);
		}
	}
}
