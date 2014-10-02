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
	/// Toolbar de la vue des groupes.
	/// </summary>
	public class GroupsToolbar : AbstractCommandToolbar
	{
		public GroupsToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Groups.First, 2);
			this.CreateButton (Res.Commands.Groups.Prev, 1);
			this.CreateButton (Res.Commands.Groups.Next, 1);
			this.CreateButton (Res.Commands.Groups.Last, 2);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Groups.CompactAll, 3);
			this.CreateButton (Res.Commands.Groups.CompactOne, 4);
			this.CreateButton (Res.Commands.Groups.ExpandOne, 4);
			this.CreateButton (Res.Commands.Groups.ExpandAll, 3);

			this.CreateSeparator (3);

			this.CreateButton (Res.Commands.Groups.New, 0);
			this.CreateButton (Res.Commands.Groups.Delete, 0);
			this.CreateButton (Res.Commands.Groups.Deselect, 6);

			this.CreateSeparator (5);

			this.CreateButton (Res.Commands.Groups.Copy, 5);
			this.CreateButton (Res.Commands.Groups.Paste, 5);
			this.CreateButton (Res.Commands.Groups.Export, 5);

			this.CreateSearchController (SearchKind.Groups, 7);
		}
	}
}
