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
//-			this.SetCommandDescription (ToolbarCommand.ReportSelect,     "Report.Select",        "Choix d'un rapport");
			this.SetCommandDescription (ToolbarCommand.ReportParams,     "Report.Params",        "Paramètres du rapport");
			this.SetCommandDescription (ToolbarCommand.CompactAll,       "TreeTable.CompactAll", "Compacter tout");
			this.SetCommandDescription (ToolbarCommand.CompactOne,       "TreeTable.CompactOne", "Compacter un niveau");
			this.SetCommandDescription (ToolbarCommand.ExpandOne,        "TreeTable.ExpandOne",  "Etendre un niveau");
			this.SetCommandDescription (ToolbarCommand.ExpandAll,        "TreeTable.ExpandAll",  "Etendre tout");
			this.SetCommandDescription (ToolbarCommand.ReportPrevPeriod, "Report.PrevPeriod",    "Période précédente");
			this.SetCommandDescription (ToolbarCommand.ReportNextPeriod, "Report.NextPeriod",    "Période suivante");
			this.SetCommandDescription (ToolbarCommand.ReportExport,     "Report.Export",        "Exporter le rapport");
			this.SetCommandDescription (ToolbarCommand.ReportClose,      "Report.Close",         "Fermer le rapport");
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
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.CompactAll);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.CompactOne);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ExpandOne);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ExpandAll);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ReportPrevPeriod);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ReportNextPeriod);
			this.CreateSeparator     (DockStyle.Left);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ReportExport);
			this.CreateCommandButton (DockStyle.Right, ToolbarCommand.ReportClose);

			return this.toolbar;
		}
	}
}
