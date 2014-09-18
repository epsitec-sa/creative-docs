//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// Toolbar utilisée pour les timelines des objets d'immobilisations en mode "plusieurs
	/// timelines" (ViewMode.Multiple), dans la partie droite.
	/// </summary>
	public class TimelinesToolbar : AbstractCommandToolbar
	{
		public TimelinesToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Timelines.Narrow, 4);
			this.CreateButton (Res.Commands.Timelines.Wide, 4);

			this.CreateSeparator (4);
			
			this.CreateButton (Res.Commands.Timelines.First, 2);
			this.CreateButton (Res.Commands.Timelines.Prev, 1);
			this.CreateButton (Res.Commands.Timelines.Next, 1);
			this.CreateButton (Res.Commands.Timelines.Last, 2);
			
			this.CreateSeparator (1);
			
			this.CreateButton (Res.Commands.Timelines.New, 0);
			this.CreateButton (Res.Commands.Timelines.Delete, 0);
			this.CreateButton (Res.Commands.Timelines.Deselect, 5);
			
			this.CreateSeparator (3);
			
			this.CreateButton (Res.Commands.Timelines.Copy, 3);
			this.CreateButton (Res.Commands.Timelines.Paste, 3);
		}
	}
}
