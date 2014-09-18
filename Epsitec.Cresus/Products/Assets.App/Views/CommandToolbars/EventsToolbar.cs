//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// Toolbar de la vue des événements d'un objet d'immobilisation, en mode ViewMode.Event
	/// lorsqu'un objet est double-cliqué.
	/// </summary>
	public class EventsToolbar : AbstractCommandToolbar
	{
		public EventsToolbar(DataAccessor accessor, CommandContext commandContext)
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

			this.CreateButton (Res.Commands.Events.First, 2);
			this.CreateButton (Res.Commands.Events.Prev, 1);
			this.CreateButton (Res.Commands.Events.Next, 1);
			this.CreateButton (Res.Commands.Events.Last, 2);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Events.New, 0);
			this.CreateButton (Res.Commands.Events.Delete, 0);
			this.CreateButton (Res.Commands.Events.Deselect, 4);

			this.CreateSeparator (3);

			this.CreateButton (Res.Commands.Events.Copy, 3);
			this.CreateButton (Res.Commands.Events.Paste, 3);
			this.CreateButton (Res.Commands.Events.Export, 3);
		}
	}
}
