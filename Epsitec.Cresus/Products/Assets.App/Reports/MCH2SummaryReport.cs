//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MCH2SummaryReport : AbstractReport
	{
		public MCH2SummaryReport(DataAccessor accessor, ReportsView reportView, ReportType reportType)
			: base (accessor, reportView, reportType)
		{
		}


		public override AbstractParams			DefaultParams
		{
			get
			{
				return new MCH2SummaryParams ();  // paramètres par défaut
			}
		}

		public override string					Title
		{
			get
			{
				return string.Concat (base.Title, " ", this.Params.DateRange.ToNiceString ());
			}
		}


		public override void Initialize(NavigationTreeTableController treeTableController)
		{
			this.treeTableController = treeTableController;
			this.visibleSelectedRow = -1;

			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.dataFiller = new MCH2SummaryTreeTableFiller (this.accessor, this.NodeGetter);
			TreeTableFiller<SortableCumulNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Report.MCH2Summary");

			this.sortingInstructions = TreeTableFiller<SortableCumulNode>.GetSortingInstructions (this.treeTableController);

			base.Initialize (treeTableController);
		}

		public override void ShowParamsPopup(Widget target)
		{
			//	Affiche le Popup pour choisir les paramètres d'un rapport.
			var popup = new MCH2SummaryReportPopup (this.accessor)
			{
				DateRange   = this.Params.DateRange,
				GroupGuid   = this.Params.RootGuid,
				Level       = this.Params.Level
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.reportView.ReportParams = new MCH2SummaryParams (popup.DateRange, popup.GroupGuid, popup.Level);
				}
			};
		}

		public override void UpdateParams()
		{
			if (this.Params == null)
			{
				return;
			}

			this.dataFiller.Title = this.Title;
			this.DataFiller.DateRange = this.Params.DateRange;

			//	On réinitialise ici les colonnes, car les dates InitialTimestamp et FinalTimestamp
			//	peuvent avoir changé, et les colonnes doivent afficher "Etat 01.01.2014" et
			//	"Etat 31.12.2014" (par exemple).
			TreeTableFiller<SortableCumulNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Report.MCH2Summary");

			//	Il faut utiliser ToTimestamp.JustBefore pour afficher les noms des objets tels
			//	qu'ils sont définis le 31.12.xx à 23h59.
			var ei = this.DataFiller.UsedExtractionInstructions.ToList ();
			this.NodeGetter.SetParams (this.Params.DateRange.ToTimestamp.JustBefore, this.Params.RootGuid, this.sortingInstructions, ei);
			this.dataFiller.Timestamp = this.Params.DateRange.ToTimestamp.JustBefore;

			if (this.Params.Level.HasValue)
			{
				this.NodeGetter.SetLevel (this.Params.Level.Value);
			}

			this.UpdateTreeTable ();

			this.OnParamsChanged ();
			this.OnUpdateCommands ();
		}

		public override void ShowExportPopup(Widget target)
		{
			ExportHelpers<SortableCumulNode>.StartExportProcess (target, this.accessor, this.dataFiller, this.treeTableController.ColumnsState);
		}


		protected override void HandleSortingChanged(object sender)
		{
			this.sortingInstructions = TreeTableFiller<SortableCumulNode>.GetSortingInstructions (this.treeTableController);
			this.UpdateParams ();
		}


		protected override void UpdateTreeTable()
		{
			TreeTableFiller<SortableCumulNode>.FillContent (this.treeTableController, this.dataFiller, this.visibleSelectedRow, crop: true);
		}


		private MCH2SummaryParams Params
		{
			get
			{
				return this.reportView.ReportParams as MCH2SummaryParams;
			}
		}

		private MCH2SummaryTreeTableFiller DataFiller
		{
			get
			{
				return this.dataFiller as MCH2SummaryTreeTableFiller;
			}
		}

		private ObjectsNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as ObjectsNodeGetter;
			}
		}


		private SortingInstructions					sortingInstructions;
		private AbstractTreeTableFiller<SortableCumulNode>	dataFiller;
	}
}
