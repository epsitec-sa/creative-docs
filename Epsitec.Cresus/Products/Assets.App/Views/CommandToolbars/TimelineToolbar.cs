//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// Toolbar des événements des objets d'immobilisation, en bas de la fenêtre
	/// en mode ViewMode.Single.
	/// </summary>
	public class TimelineToolbar : AbstractCommandToolbar
	{
		public TimelineToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Timeline.Labels, 2);

			this.CreateSajex (5, 2);

			this.CreateButton (Res.Commands.Timeline.Compacted, 2);
			this.CreateButton (Res.Commands.Timeline.Expanded, 2);
			
			this.CreateSajex (5, 2);

			this.CreateButton (Res.Commands.Timeline.WeeksOfYear, 2);
			this.CreateButton (Res.Commands.Timeline.DaysOfWeek, 2);
			this.CreateButton (Res.Commands.Timeline.Graph, 2);
			
			this.CreateSajex (10, 2);
			
			this.CreateButton (Res.Commands.Timeline.First, 1);
			this.CreateButton (Res.Commands.Timeline.Prev, 1);
			this.CreateButton (Res.Commands.Timeline.Next, 1);
			this.CreateButton (Res.Commands.Timeline.Last, 1);
			this.CreateButton (Res.Commands.Timeline.Now, 1);
			this.CreateButton (Res.Commands.Timeline.Date, 1);
			
			this.CreateSeparator (1);
			
			this.CreateButton (Res.Commands.Timeline.New, 0);
			this.CreateButton (Res.Commands.Timeline.Delete, 0);
			this.CreateButton (Res.Commands.Timeline.Deselect, 4);
			
			this.CreateSeparator (3);
			
			this.CreateButton (Res.Commands.Timeline.Copy, 3);
			this.CreateButton (Res.Commands.Timeline.Paste, 3);
		}
	}
}
