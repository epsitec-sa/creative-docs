//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Reports;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportsView : AbstractView
	{
		public ReportsView(DataAccessor accessor, MainToolbar toolbar, ViewType viewType, List<AbstractViewState> historyViewStates)
			: base (accessor, toolbar, viewType)
		{
			this.historyViewStates = historyViewStates;
		}


		public NavigationTreeTableController	TreeTableController
		{
			get
			{
				return this.treeTableController;
			}
		}

		public AbstractParams					ReportParams
		{
			get
			{
				return this.reportParams;
			}
			set
			{
				this.reportParams = value;

				if (this.report != null)
				{
					this.report.UpdateParams ();
				}
			}
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

			this.mainFrame = new FrameBox
			{
				Parent              = parent,
				Dock                = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			this.choiceFrame = new FrameBox
			{
				Parent              = parent,
				Dock                = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			this.reportChoiceController = new ReportChoiceController (this.accessor);
			this.reportChoiceController.CreateUI (this.choiceFrame);

			this.reportChoiceController.ReportSelected += delegate (object sender, ReportType reportType)
			{
				this.selectedReportType = reportType;
				this.UpdateUI ();
			};

			this.UpdateReport ();
			this.UpdateToolbars ();
		}

		public override void UpdateUI()
		{
			this.UpdateReport ();

			this.OnViewStateChanged (this.ViewState);
		}


		private void CreateToolbar(Widget parent)
		{
			this.toolbar = new ReportsToolbar (this.accessor);
			this.toolbar.CreateUI (parent);

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

					case ToolbarCommand.ReportPrevPeriod:
						this.OnChangePeriod (-1);
						break;

					case ToolbarCommand.ReportNextPeriod:
						this.OnChangePeriod (1);
						break;

					case ToolbarCommand.ReportClose:
						this.OnClose ();
						break;
				}
			};
		}


		private void OnSelect()
		{
			//	Affiche le Popup pour choisir un rapport.
			var target = this.toolbar.GetTarget (ToolbarCommand.ReportSelect);
			this.ShowReportPopup (target);
		}

		private void OnParams()
		{
			//	Affiche le Popup pour choisir les paramètres d'un rapport.
			var target = this.toolbar.GetTarget (ToolbarCommand.ReportParams);
			this.report.ShowParamsPopup (target);
		}

		private void OnExport()
		{
			//	Affiche le Popup pour choisir comment exporter le rapport.
			var target = this.toolbar.GetTarget (ToolbarCommand.ReportExport);
			this.report.ShowExportPopup (target);
		}

		private void OnChangePeriod(int direction)
		{
			this.reportParams = this.reportParams.ChangePeriod (direction);
			this.report.UpdateParams ();
		}

		private void OnClose()
		{
			//	Ferme le rapport.
			this.selectedReportType = ReportType.Unknown;
			this.UpdateUI ();
		}


		private void ShowReportPopup(Widget target)
		{
			//	Affiche le Popup pour choisir un rapport.
			var popup = new ReportPopup()
			{
				ReportType = this.selectedReportType,
			};

			popup.Create (target, leftOrRight: true);

			popup.ItemClicked += delegate (object sender, int rank)
			{
				this.selectedReportType = ReportsList.GetReportType (rank);
				this.UpdateUI ();
			};
		}


		private void UpdateReport()
		{
			if (this.report != null)
			{
				this.DeleteTreeTable ();

				this.report.ParamsChanged -= this.HandleParamsChanged;
				this.report.Dispose ();
				this.report = null;
			}

			switch (this.selectedReportType)
			{
				case ReportType.MCH2Summary:
					this.report = new MCH2SummaryReport (this.accessor, this);
					break;

				case ReportType.AssetsList:
					this.report = new AssetsReport (this.accessor, this);
					break;

				case ReportType.PersonsList:
					this.report = new PersonsReport (this.accessor, this);
					break;
			}

			if (this.report != null)
			{
				this.CreateTreeTable ();

				this.ReportParams = this.GetHistoryParams (this.selectedReportType);
				this.report.ParamsChanged += this.HandleParamsChanged;
			}

			this.mainFrame  .Visibility = (this.report != null);
			this.choiceFrame.Visibility = (this.report == null);

			this.reportChoiceController.ClearSelection ();

			this.UpdateToolbars ();
		}

		private void CreateTreeTable()
		{
			this.treeTableController = new NavigationTreeTableController ();
			this.treeTableController.CreateUI (this.mainFrame, footerHeight: 0);

			this.report.Initialize (this.treeTableController);
		}

		private void DeleteTreeTable()
		{
			this.treeTableController = null;
			this.mainFrame.Children.Clear ();
		}

		private void HandleParamsChanged(object sender)
		{
			this.OnViewStateChanged (this.ViewState);
		}


		private AbstractParams GetHistoryParams(ReportType reportType)
		{
			//	Retourne le dernier AbstractParams utilisé pour un type de rapport donné.
			var vs = this.historyViewStates
				.Where (x => x is ReportsViewState && (x as ReportsViewState).ReportType == reportType)
				.LastOrDefault () as ReportsViewState;

			if (vs == null)
			{
				//	Si on n'a pas trouvé, retourne les paramètres par défaut.
				return this.report.DefaultParams;
			}
			else
			{
				return vs.ReportParams;
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
					ReportParams = this.reportParams,
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


		private void UpdateToolbars()
		{
			bool changePeriodEnable = this.ChangePeriodEnable;

			this.toolbar.SetCommandEnable (ToolbarCommand.ReportSelect,     true);
			this.toolbar.SetCommandEnable (ToolbarCommand.ReportParams,     this.HasParams);
			this.toolbar.SetCommandEnable (ToolbarCommand.ReportExport,     this.report != null);
			this.toolbar.SetCommandEnable (ToolbarCommand.ReportPrevPeriod, changePeriodEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.ReportNextPeriod, changePeriodEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.ReportClose,      this.report != null);
		}

		private bool ChangePeriodEnable
		{
			get
			{
				return this.report != null
					&& this.reportParams != null
					&& this.reportParams.ChangePeriod (1) != null;
			}
		}

		private bool HasParams
		{
			get
			{
				return this.report != null
					&& this.reportParams != null;
			}
		}


		private readonly List<AbstractViewState> historyViewStates;

		private FrameBox						choiceFrame;
		private FrameBox						mainFrame;
		private ReportChoiceController			reportChoiceController;
		private ReportsToolbar					toolbar;
		private NavigationTreeTableController	treeTableController;
		private AbstractReport					report;
		private AbstractParams					reportParams;
		private ReportType						selectedReportType;
	}
}
