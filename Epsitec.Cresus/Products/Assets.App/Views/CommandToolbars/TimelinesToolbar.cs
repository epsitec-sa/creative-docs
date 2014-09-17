//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class TimelinesToolbar : AbstractCommandToolbar
	{
		public TimelinesToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}

		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Narrow);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Wide);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.First);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Prev);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Next);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Last);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.New);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Delete);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Amortizations.Preview);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Amortizations.Fix);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Amortizations.ToExtra);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Amortizations.Unpreview);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Amortizations.Delete);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Deselect);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Copy);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Timelines.Paste);
		}
	}
}
