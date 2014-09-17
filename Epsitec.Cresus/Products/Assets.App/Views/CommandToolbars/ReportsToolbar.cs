//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class ReportsToolbar : AbstractCommandToolbar
	{
		public ReportsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}

		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.Params);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.AddFavorite);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.RemoveFavorite);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.CompactAll);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.CompactOne);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.ExpandOne);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.ExpandAll);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.Period.Prev);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.Period.Next);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left, Res.Commands.Reports.Export);

			this.CreateCommandButton (DockStyle.Right, Res.Commands.Reports.Close);
		}
	}
}
