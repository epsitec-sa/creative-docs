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
	/// Toolbar de la vue des catégories d'immobilisations.
	/// </summary>
	public class CategoriesToolbar : AbstractCommandToolbar
	{
		public CategoriesToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Categories.First, 2);
			this.CreateButton (Res.Commands.Categories.Prev, 1);
			this.CreateButton (Res.Commands.Categories.Next, 1);
			this.CreateButton (Res.Commands.Categories.Last, 2);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Categories.New, 0);
			this.CreateButton (Res.Commands.Categories.Delete, 0);
			this.CreateButton (Res.Commands.Categories.Deselect, 4);

			this.CreateSeparator (3);

			this.CreateButton (Res.Commands.Categories.Copy, 3);
			this.CreateButton (Res.Commands.Categories.Paste, 3);
			this.CreateButton (Res.Commands.Categories.Export, 3);

			this.CreateSeparator (5);

			this.CreateSearchController (SearchKind.Categories, 5);
		}
	}
}
