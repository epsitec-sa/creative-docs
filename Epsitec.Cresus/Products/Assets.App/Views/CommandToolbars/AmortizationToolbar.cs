//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// Toolbar de la vue des amortissements.
	/// </summary>
	public class AmortizationToolbar : AbstractCommandToolbar
	{
		public AmortizationToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}

		public override void CreateUI(Widget parent)
		{
			//	La valeur zéro pour level indique les commandes importantes.
			//	Les plus grandes valeurs correspondent à des commandes de moins
			//	en moins importantes, qui seront absentes si la place à
			//	disposition dans la toolbar vient à manquer.

			base.CreateUI (parent);

			this.CreateButton (Res.Commands.Timelines.Narrow, 4);
			this.CreateButton (Res.Commands.Timelines.Wide, 4);

			this.CreateSeparator (4);
			
			this.CreateButton (Res.Commands.Timelines.First, 2);
			this.CreateButton (Res.Commands.Timelines.Prev, 1);
			this.CreateButton (Res.Commands.Timelines.Next, 1);
			this.CreateButton (Res.Commands.Timelines.Last, 2);
			
			this.CreateSeparator (1);
			
			this.CreateButton (Res.Commands.Timelines.Amortizations.Preview, 0);
			this.CreateButton (Res.Commands.Timelines.Amortizations.Fix, 0);
			this.CreateButton (Res.Commands.Timelines.Amortizations.ToExtra, 0);
			this.CreateButton (Res.Commands.Timelines.Amortizations.Unpreview, 0);
			this.CreateButton (Res.Commands.Timelines.Amortizations.Delete, 0);

			this.CreateSeparator (5);

			this.CreateButton (Res.Commands.Timelines.Deselect, 5);
			
			this.CreateSeparator (3);
			
			this.CreateButton (Res.Commands.Timelines.Copy, 3);
			this.CreateButton (Res.Commands.Timelines.Paste, 3);
		}
	}
}
