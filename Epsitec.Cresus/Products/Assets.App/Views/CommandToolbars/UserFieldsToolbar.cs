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
	/// Toolbar de la vue des définitions de champs, pour les objets d'immobilisations
	/// et les contacts.
	/// </summary>
	public class UserFieldsToolbar : AbstractCommandToolbar
	{
		public UserFieldsToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.UserFields.First, 4);
			this.CreateButton (Res.Commands.UserFields.Prev, 3);
			this.CreateButton (Res.Commands.UserFields.Next, 3);
			this.CreateButton (Res.Commands.UserFields.Last, 4);

			this.CreateSeparator (3);

			this.CreateButton (Res.Commands.UserFields.MoveTop, 2);
			this.CreateButton (Res.Commands.UserFields.MoveUp, 1);
			this.CreateButton (Res.Commands.UserFields.MoveDown, 1);
			this.CreateButton (Res.Commands.UserFields.MoveBottom, 2);

			this.CreateSeparator (1);

			this.helplineTargetButton =
			this.CreateButton (Res.Commands.UserFields.New, 0);
			this.CreateButton (Res.Commands.UserFields.Delete, 0);
			this.CreateButton (Res.Commands.UserFields.Deselect, 6);

			this.CreateSeparator (5);

			this.CreateButton (Res.Commands.UserFields.Copy, 5);
			this.CreateButton (Res.Commands.UserFields.Paste, 5);
			this.CreateButton (Res.Commands.UserFields.Export, 5);

			this.CreateSearchController (SearchKind.UserFields, 7);
		}
	}
}
