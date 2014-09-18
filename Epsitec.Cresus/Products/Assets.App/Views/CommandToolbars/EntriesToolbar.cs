//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class EntriesToolbar : AbstractCommandToolbar
	{
		public EntriesToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.CreateButton (Res.Commands.Entries.First, 3);
			this.CreateButton (Res.Commands.Entries.Prev, 2);
			this.CreateButton (Res.Commands.Entries.Next, 2);
			this.CreateButton (Res.Commands.Entries.Last, 3);

			this.CreateSeparator (2);

			this.CreateButton (Res.Commands.Entries.CompactAll, 0);
			this.CreateButton (Res.Commands.Entries.ExpandAll, 0);

			this.CreateSeparator (4);

			this.CreateButton (Res.Commands.Entries.Deselect, 4);

			this.CreateSeparator (1);

			this.CreateButton (Res.Commands.Entries.Export, 1);
		}
	}
}
