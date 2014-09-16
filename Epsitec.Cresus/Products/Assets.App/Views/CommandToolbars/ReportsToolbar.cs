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
		public ReportsToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}

		protected override void CreateCommands()
		{
			this.SetCommandDescription (ToolbarCommand.ReportParams,         "Report.Params",         Res.Strings.Toolbar.Reports.Params.ToString ());
			this.SetCommandDescription (ToolbarCommand.ReportAddFavorite,    "Report.AddFavorite",    Res.Strings.Toolbar.Reports.AddFavorite.ToString ());
			this.SetCommandDescription (ToolbarCommand.ReportRemoveFavorite, "Report.RemoveFavorite", Res.Strings.Toolbar.Reports.RemoveFavorite.ToString ());
			this.SetCommandDescription (ToolbarCommand.CompactAll,           "TreeTable.CompactAll",  Res.Strings.Toolbar.Reports.CompactAll.ToString ());
			this.SetCommandDescription (ToolbarCommand.CompactOne,           "TreeTable.CompactOne",  Res.Strings.Toolbar.Reports.CompactOne.ToString ());
			this.SetCommandDescription (ToolbarCommand.ExpandOne,            "TreeTable.ExpandOne",   Res.Strings.Toolbar.Reports.ExpandOne.ToString ());
			this.SetCommandDescription (ToolbarCommand.ExpandAll,            "TreeTable.ExpandAll",   Res.Strings.Toolbar.Reports.ExpandAll.ToString ());
			this.SetCommandDescription (ToolbarCommand.ReportPrevPeriod,     "Report.PrevPeriod",     Res.Strings.Toolbar.Reports.PrevPeriod.ToString ());
			this.SetCommandDescription (ToolbarCommand.ReportNextPeriod,     "Report.NextPeriod",     Res.Strings.Toolbar.Reports.NextPeriod.ToString ());
			this.SetCommandDescription (ToolbarCommand.ReportExport,         "Report.Export",         Res.Strings.Toolbar.Reports.Export.ToString ());
			this.SetCommandDescription (ToolbarCommand.ReportClose,          "Report.Close",          Res.Strings.Toolbar.Reports.Close.ToString (), new Shortcut (KeyCode.AlphaW | KeyCode.ModifierControl));
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			CommandDispatcher.SetDispatcher (this.toolbar, this.commandDispatcher);

			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.ReportParams);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ReportAddFavorite);
			this.CreateCommandButton (DockStyle.Left,  ToolbarCommand.ReportRemoveFavorite);
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

			this.AttachShortcuts ();
		}
	}
}
