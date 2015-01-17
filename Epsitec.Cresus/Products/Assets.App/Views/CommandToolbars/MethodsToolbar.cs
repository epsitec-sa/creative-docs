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
	/// Toolbar de la vue des expressions d'amortissements.
	/// </summary>
	public class MethodsToolbar : AbstractCommandToolbar
	{
		public MethodsToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Methods.First, 3);
			this.CreateButton (Res.Commands.Methods.Prev, 2);
			this.CreateButton (Res.Commands.Methods.Next, 2);
			this.CreateButton (Res.Commands.Methods.Last, 3);

			this.CreateSeparator (2);

			this.helplineTargetButton =
			this.CreateButton (Res.Commands.Methods.New, 0);
			this.CreateButton (Res.Commands.Methods.Delete, 0);
			this.CreateButton (Res.Commands.Methods.Deselect, 5);

			this.CreateSeparator (4);

			this.CreateButton (Res.Commands.Methods.Copy, 4);
			this.CreateButton (Res.Commands.Methods.Paste, 4);
			this.CreateButton (Res.Commands.Methods.Export, 4);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Methods.Library, 1);
			this.CreateButton (Res.Commands.Methods.Compile, 1);
			this.CreateButton (Res.Commands.Methods.Show, 1);
			this.CreateButton (Res.Commands.Methods.Simulation, 1);

			this.CreateSearchController (SearchKind.Expressions, 6);
		}
	}
}
