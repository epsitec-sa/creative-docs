//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// Toolbar de la vue des contacts.
	/// </summary>
	public class PersonsToolbar : AbstractCommandToolbar
	{
		public PersonsToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Persons.First, 2);
			this.CreateButton (Res.Commands.Persons.Prev, 1);
			this.CreateButton (Res.Commands.Persons.Next, 1);
			this.CreateButton (Res.Commands.Persons.Last, 2);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Persons.New, 0);
			this.CreateButton (Res.Commands.Persons.Delete, 0);
			this.CreateButton (Res.Commands.Persons.Deselect, 4);

			this.CreateSeparator (3);

			this.CreateButton (Res.Commands.Persons.Copy, 3);
			this.CreateButton (Res.Commands.Persons.Paste, 3);
			this.CreateButton (Res.Commands.Persons.Export, 3);

			this.CreateSeparator (5);

			this.CreateSearchController (5);
		}
	}
}
