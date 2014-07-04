//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportsToolbar : AbstractCommandToolbar
	{
		protected override void CreateCommands()
		{
			this.SetCommandDescription (ToolbarCommand.ReportSelect, "Report.Select", "Choix d'un rapport");
			this.SetCommandDescription (ToolbarCommand.ReportParams, "Report.Params", "Paramètres du rapport");
			this.SetCommandDescription (ToolbarCommand.ReportExport, "Report.Export", "Exporter le rapport");
		}


		public override FrameBox CreateUI(Widget parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ReportSelect);
			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ReportParams);
			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ReportExport);

			return this.toolbar;
		}
	}
}
