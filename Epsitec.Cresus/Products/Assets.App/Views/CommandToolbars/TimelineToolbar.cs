//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class TimelineToolbar : AbstractCommandToolbar
	{
		public TimelineToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}

		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Labels);
			this.CreateSajex (5);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Compacted);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Expanded);
			this.CreateSajex (5);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.WeeksOfYear);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.DaysOfWeek);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Graph);
			this.CreateSajex (10);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.First);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Prev);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Next);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Last);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Now);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Date);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.New);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Delete);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Deselect);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Copy);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timeline.Paste);
		}
	}
}
