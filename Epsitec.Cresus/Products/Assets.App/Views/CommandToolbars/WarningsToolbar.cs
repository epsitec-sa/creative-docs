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
	/// Toolbar de la vue des avertissements.
	/// </summary>
	public class WarningsToolbar : AbstractCommandToolbar
	{
		public WarningsToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Warnings.First, 2);
			this.CreateButton (Res.Commands.Warnings.Prev, 1);
			this.CreateButton (Res.Commands.Warnings.Next, 1);
			this.CreateButton (Res.Commands.Warnings.Last, 2);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Warnings.Deselect, 3);

			this.CreateSeparator (3);

			this.helplineTargetButton =
			this.CreateButton (Res.Commands.Warnings.Goto, 0);

			this.CreateSearchController (SearchKind.Warnings, 4);
		}
	}
}
