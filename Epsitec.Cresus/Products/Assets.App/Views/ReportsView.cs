//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
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

			var splitter = new VSplitter
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
				Parent = rightFrame,
				Dock   = DockStyle.Top,
			};

			this.CreateReport (rightFrame);

			this.InitializeScrollList ();
			this.UpdateButtons ();
		}

		private void CreateScrollList(Widget parent)
		{
			//?new StaticText
			//?{
			//?	Parent         = parent,
			//?	Text           = ReportsView.indentPrefix + "Liste des rapports disponibles",
			//?	Dock           = DockStyle.Top,
			//?	Margins        = new Margins (0, 0, 0, 10),
			//?};

			this.scrollList = new ScrollList
			{
				Parent         = parent,
				Dock           = DockStyle.Fill,
			};

			this.scrollList.SelectedItemChanged += delegate
			{
				if (this.scrollList.SelectedItemIndex != -1)
				{
					this.UpdateReport (this.scrollList.Items.Keys[this.scrollList.SelectedItemIndex]);
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


		private void UpdateReport(string id)
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

			switch (id)
			{
				case "MCH2Report":
					this.report = new MCH2Report (this.accessor, this.treeTableController);
					this.report.Initialize ();

					this.paramsPanel = new MCH2ParamsPanel (this.accessor);
					this.report.SetParams (this.paramsPanel.ReportParams);
					break;

				case "AssetsList":
					this.report = new AssetsReport (this.accessor, this.treeTableController);
					this.report.Initialize ();

					this.paramsPanel = new AssetsParamsPanel (this.accessor);
					this.report.SetParams (this.paramsPanel.ReportParams);
					break;

				case "PersonsList":
					this.report = new PersonsReport (this.accessor, this.treeTableController);
					this.report.Initialize ();

					this.report.SetParams (null);
					break;
			}

			if (this.paramsPanel != null)
			{
				this.paramsPanel.CreateUI (this.paramsFrame);
				this.paramsPanel.ParamsChanged += this.HandleParamsChanged;
			}
		}

		private void HandleParamsChanged(object sender)
		{
			this.report.SetParams (this.paramsPanel.ReportParams);
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

			foreach (var report in this.Reports)
			{
				this.scrollList.Items.Add (report.Id, ReportsView.indentPrefix + report.Name);
			}
		}

		private IEnumerable<Report> Reports
		{
			get
			{
				yield return new Report ("MCH2Report",     "Tableau des immobilisations MCH2");
				yield return new Report ("AssetsList",     "Liste des objets d'immobilisations");
				yield return new Report ("PersonsList",    "Liste des personnes");
			}
		}

		private struct Report
		{
			public Report(string id, string name)
			{
				this.Id   = id;
				this.Name = name;
			}

			public readonly string Id;
			public readonly string Name;
		}


		private const string indentPrefix = "  ";

		private ScrollList						scrollList;
		private FrameBox						paramsFrame;
		private Button							showButton;
		private Button							exportButton;
		private NavigationTreeTableController	treeTableController;
		private AbstractParamsPanel				paramsPanel;
		private AbstractReport					report;
	}
}
