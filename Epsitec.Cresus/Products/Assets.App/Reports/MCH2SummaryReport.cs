//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MCH2SummaryReport : AbstractReport
	{
		public MCH2SummaryReport(DataAccessor accessor, NavigationTreeTableController treeTableController)
			: base (accessor, treeTableController)
		{
			var year = Timestamp.Now.Date.Year;

			this.reportParams = new MCH2SummaryParams
			(
				new Timestamp (new System.DateTime (year,  1,  1), 0),
				new Timestamp (new System.DateTime (year, 12, 31), 0),
				Guid.Empty
			);
		}


		public override void Initialize()
		{
			this.visibleSelectedRow = -1;

			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Assets), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			this.dataFiller = new MCH2SummaryTreeTableFiller (this.accessor, this.NodeGetter);
			TreeTableFiller<CumulNode>.FillColumns (this.treeTableController, this.dataFiller);

			base.Initialize ();
		}

		public override void ShowParamsPopup(Widget target)
		{
			var popup = new MCH2SummaryReportPopup (this.accessor)
			{
				InitialDate = this.Params.InitialTimestamp.Date,
				FinalDate   = this.Params.FinalTimestamp.Date,
				GroupGuid   = this.Params.RootGuid,
			};

			popup.Create (target);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.reportParams = new MCH2SummaryParams
					(
						new Timestamp (popup.InitialDate.GetValueOrDefault (), 0),
						new Timestamp (popup.FinalDate.GetValueOrDefault (), 0),
						popup.GroupGuid
					);

					this.UpdateParams ();
				}
			};
		}

		protected override void UpdateParams()
		{
			this.DataFiller.InitialTimestamp = this.Params.InitialTimestamp;
			this.DataFiller.FinalTimestamp   = this.Params.FinalTimestamp;

			var e = this.DataFiller.UsedExtractionInstructions.ToList ();
			this.NodeGetter.SetParams (this.Params.FinalTimestamp, this.Params.RootGuid, this.sortingInstructions, e);
			this.nodeGetter.SetLevel (1);

			this.UpdateTreeTable ();
		}


		protected override void UpdateTreeTable()
		{
			TreeTableFiller<CumulNode>.FillContent (this.treeTableController, this.dataFiller, this.visibleSelectedRow, crop: true);
		}


		private MCH2SummaryParams Params
		{
			get
			{
				return this.reportParams as MCH2SummaryParams;
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
		private AbstractTreeTableFiller<CumulNode>	dataFiller;
	}
}
