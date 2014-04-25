//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportsView : AbstractView
	{
		public ReportsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
		}

		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			var topTitle = new TopTitle
			{
				Parent = parent,
			};
			topTitle.SetTitle (this.GetViewTitle (ViewType.Reports));

			this.CreateToolbar (parent);

			var mainFrame = new FrameBox
			{
				Parent              = parent,
				Dock                = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			var leftFrame = new FrameBox
			{
				Parent         = mainFrame,
				Dock           = DockStyle.Left,
				PreferredWidth = 300,
			};

			new VSplitter
			{
				Parent         = mainFrame,
				Dock           = DockStyle.Left,
				PreferredWidth = 10,
			};

			var rightFrame = new FrameBox
			{
				Parent         = mainFrame,
				Dock           = DockStyle.Fill,
			};

			this.CreateScrollList (leftFrame);
			this.CreateButtons    (leftFrame);

			this.paramsFrame = new FrameBox
			{
				Parent          = rightFrame,
				PreferredHeight = 24,
				Dock            = DockStyle.Top,
			};

			this.CreateReport (rightFrame);

			this.InitializeScrollList ();
			this.UpdateButtons ();
		}

		public override void UpdateUI()
		{
			this.UpdateReport ();
			this.OnViewStateChanged (this.ViewState);
		}


		private void CreateToolbar(Widget parent)
		{
			this.toolbar = new ReportsToolbar ();
			this.toolbar.CreateUI (parent);

			this.toolbar.SetCommandEnable (ToolbarCommand.ReportSelect, true);
			this.toolbar.SetCommandEnable (ToolbarCommand.ReportParams, true);
			this.toolbar.SetCommandEnable (ToolbarCommand.ReportExport, true);

			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.ReportSelect:
						this.OnSelect ();
						break;

					case ToolbarCommand.ReportParams:
						this.OnParams ();
						break;

					case ToolbarCommand.ReportExport:
						this.OnExport ();
						break;
				}
			};
		}


		private void CreateScrollList(Widget parent)
		{
			new StaticText
			{
				Parent          = parent,
				Text            = ReportsView.indentPrefix + "Liste des rapports disponibles",
				PreferredHeight = 24,
				Dock            = DockStyle.Top,
			};

			this.scrollList = new ScrollList
			{
				Parent         = parent,
				Dock           = DockStyle.Fill,
			};

			this.scrollList.SelectedItemChanged += delegate
			{
				if (this.scrollList.SelectedItemIndex != -1)
				{
					string key = this.scrollList.Items.Keys[this.scrollList.SelectedItemIndex];
					this.selectedReportType = ReportsView.ParseReportType (key);

					this.UpdateUI ();
				}

				this.UpdateButtons ();
			};
		}

		private void CreateButtons(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent              = parent,
				Dock                = DockStyle.Bottom,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredHeight     = 30,
				Margins             = new Margins (0, 0, 10, 0),
			};

			this.showButton = new Button
			{
				Parent      = frame,
				Text        = "Visualiser",
				ButtonStyle = ButtonStyle.Icon,
				AutoFocus   = false,
				Dock        = DockStyle.Fill,
				Margins     = new Margins (0, 5, 0, 0),
			};

			this.exportButton = new Button
			{
				Parent      = frame,
				Text        = "Exporter",
				ButtonStyle = ButtonStyle.Icon,
				AutoFocus   = false,
				Dock        = DockStyle.Fill,
				Margins     = new Margins (5, 0, 0, 0),
			};
		}

		private void CreateReport(Widget parent)
		{
			this.treeTableController = new NavigationTreeTableController ();
			this.treeTableController.CreateUI (parent, rowHeight: 18, headerHeight: 18, footerHeight: 0);
			this.treeTableController.AllowsMovement = false;
			this.treeTableController.AddSortedColumn (0);
		}


		private void OnSelect()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.ReportSelect);
			if (target != null)
			{
			}
		}

		private void OnParams()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.ReportParams);
			if (target != null)
			{
				this.report.ShowParamsPopup (target);
			}
		}

		private void OnExport()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.ReportExport);
			if (target != null)
			{
			}
		}


		private void UpdateReport()
		{
			if (this.paramsPanel != null)
			{
				this.paramsPanel.ParamsChanged -= this.HandleParamsChanged;
				this.paramsPanel = null;
			}

			this.paramsFrame.Children.Clear ();

			if (this.report != null)
			{
				this.report.Dispose ();
				this.report = null;
			}

			switch (this.selectedReportType)
			{
				case ReportType.MCH2Summary:
					this.report = new MCH2SummaryReport (this.accessor, this.treeTableController);
					this.report.Initialize ();

					this.paramsPanel = new MCH2SummaryParamsPanel (this.accessor);

					if (!(this.reportParams is MCH2SummaryParams))
					{
						this.reportParams = this.paramsPanel.ReportParams;
					}
					break;

				case ReportType.AssetsList:
					this.report = new AssetsReport (this.accessor, this.treeTableController);
					this.report.Initialize ();

					this.paramsPanel = new AssetsParamsPanel (this.accessor);

					if (!(this.reportParams is AssetsParams))
					{
						this.reportParams = this.paramsPanel.ReportParams;
					}
					break;

				case ReportType.PersonsList:
					this.report = new PersonsReport (this.accessor, this.treeTableController);
					this.report.Initialize ();

					this.reportParams = null;
					break;
			}

			this.report.SetParams (this.reportParams);

			if (this.paramsPanel != null)
			{
				this.paramsPanel.CreateUI (this.paramsFrame);
				this.paramsPanel.ReportParams = this.reportParams;
				this.paramsPanel.ParamsChanged += this.HandleParamsChanged;
			}

			int index = this.scrollList.Items.Keys.ToList ().FindIndex (x => ReportsView.ParseReportType (x) == this.selectedReportType);
			this.scrollList.SelectedItemIndex = index;
		}

		private void HandleParamsChanged(object sender)
		{
			this.reportParams = this.paramsPanel.ReportParams;
			this.report.SetParams (this.reportParams);
			this.OnViewStateChanged (this.ViewState);
		}


		private void UpdateButtons()
		{
			bool enable = this.scrollList.SelectedItemIndex != -1;

			this.showButton  .Enable = enable;
			this.exportButton.Enable = enable;
		}


		private void InitializeScrollList()
		{
			this.scrollList.Items.Clear ();

			foreach (var report in ReportsView.Reports)
			{
				this.scrollList.Items.Add (ReportsView.ReportTypeToString (report.Type), ReportsView.indentPrefix + report.Name);
			}
		}


		public override AbstractViewState ViewState
		{
			get
			{
				return new ReportsViewState
				{
					ViewType     = ViewType.Reports,
					ReportType   = this.selectedReportType,
					ReportParams = (this.paramsPanel == null) ? null : this.paramsPanel.ReportParams,
				};
			}
			set
			{
				var viewState = value as ReportsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.selectedReportType = viewState.ReportType;
				this.reportParams       = viewState.ReportParams;

				this.UpdateUI ();
			}
		}


		public static string GetReportName(ReportType type)
		{
			var report = ReportsView.Reports.Where (x => x.Type == type).FirstOrDefault ();
			return report.Name;
		}

		private static IEnumerable<Report> Reports
		{
			get
			{
				yield return new Report (ReportType.MCH2Summary, "Tableau des immobilisations MCH2");
				yield return new Report (ReportType.AssetsList,  "Liste des objets d'immobilisations");
				yield return new Report (ReportType.PersonsList, "Liste des personnes");
			}
		}


		private static string ReportTypeToString(ReportType type)
		{
			return type.ToString ();
		}

		private static ReportType ParseReportType(string text)
		{
			ReportType type;

			if (System.Enum.TryParse<ReportType> (text, out type))
			{
				return type;
			}
			else
			{
				return ReportType.Unknown;
			}
		}


		private struct Report
		{
			public Report(ReportType type, string name)
			{
				this.Type = type;
				this.Name = name;
			}

			public readonly ReportType			Type;
			public readonly string				Name;
		}


		private const string indentPrefix = "  ";

		private ReportsToolbar					toolbar;
		private ScrollList						scrollList;
		private FrameBox						paramsFrame;
		private Button							showButton;
		private Button							exportButton;
		private NavigationTreeTableController	treeTableController;
		private AbstractParamsPanel				paramsPanel;
		private AbstractReport					report;
		private ReportType						selectedReportType;
		private AbstractParams					reportParams;
	}
}
