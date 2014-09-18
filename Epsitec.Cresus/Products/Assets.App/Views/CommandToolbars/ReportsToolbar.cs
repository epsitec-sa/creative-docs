//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// Toolbar de la vue des rapports, visible uniquement lorsqu'un rapport
	/// est choisi.
	/// </summary>
	public class ReportsToolbar : AbstractCommandToolbar
	{
		public ReportsToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Reports.Params, 0);
			this.CreateButton (Res.Commands.Reports.AddFavorite, 0);
			this.CreateButton (Res.Commands.Reports.RemoveFavorite, 0);

			this.CreateSeparator (3);
			
			this.CreateButton (Res.Commands.Reports.CompactAll, 3);
			this.CreateButton (Res.Commands.Reports.CompactOne, 4);
			this.CreateButton (Res.Commands.Reports.ExpandOne, 4);
			this.CreateButton (Res.Commands.Reports.ExpandAll, 3);
			
			this.CreateSeparator (2);
			
			this.CreateButton (Res.Commands.Reports.Period.Prev, 2);
			this.CreateButton (Res.Commands.Reports.Period.Next, 2);
			
			this.CreateSeparator (1);
			
			this.CreateButton (Res.Commands.Reports.Export, 1);

			this.CreateButton (DockStyle.Right, Res.Commands.Reports.Close);
		}
	}
}
