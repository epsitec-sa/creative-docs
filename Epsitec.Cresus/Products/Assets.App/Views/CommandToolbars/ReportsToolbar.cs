//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public class ReportsToolbar : AbstractCommandToolbar
	{
		public ReportsToolbar(DataAccessor accessor)
			: base (accessor)
		{
		}

		protected override void CreateCommands()
		{
//-			this.SetCommandDescription (ToolbarCommand.ReportSelect, "Report.Select", "Choix d'un rapport");
			this.SetCommandDescription (ToolbarCommand.ReportParams, "Report.Params", "Paramètres du rapport");
			this.SetCommandDescription (ToolbarCommand.ReportExport, "Report.Export", "Exporter le rapport");
			this.SetCommandDescription (ToolbarCommand.ReportClose,  "Report.Close",  "Fermer le rapport");
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

//-			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ReportSelect);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ReportParams);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ReportExport);
			this.CreateCommandButton (DockStyle.Right, ToolbarCommand.ReportClose);

			return this.toolbar;
		}
	}
}
