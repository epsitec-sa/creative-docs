//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class PersonsReport : AbstractReport
	{
		public PersonsReport(DataAccessor accessor, AbstractReportParams reportParams)
			: base (accessor, reportParams)
		{
		}


		public override void Initialize(NavigationTreeTableController treeTableController)
		{
			this.treeTableController = treeTableController;
			this.visibleSelectedRow = -1;

			var primary = this.accessor.GetNodeGetter (BaseType.Persons);
			this.secondaryNodeGetter = new SortableNodeGetter (primary, this.accessor, BaseType.Persons);
			this.primaryNodeGetter = new SorterNodeGetter (this.secondaryNodeGetter);

			this.dataFiller = new PersonsTreeTableFiller (this.accessor, this.primaryNodeGetter);
			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Report.Persons");

			this.sortingInstructions = TreeTableFiller<SortableNode>.GetSortingInstructions (this.treeTableController);

			base.Initialize (treeTableController);
		}

		public override void UpdateParams()
		{
			this.dataFiller.Title = this.Title;

			this.secondaryNodeGetter.SetParams (null, this.sortingInstructions);
			this.primaryNodeGetter.SetParams (this.sortingInstructions);

			this.UpdateTreeTable ();

			this.OnParamsChanged ();
			this.OnUpdateCommands ();
		}

		public override void ShowExportPopup(Widget target)
		{
			ExportHelpers<SortableNode>.StartExportProcess (target, this.accessor, this.dataFiller, this.treeTableController.ColumnsState);
		}


		protected override void HandleSortingChanged(object sender)
		{
			this.sortingInstructions = TreeTableFiller<SortableNode>.GetSortingInstructions (this.treeTableController);
			this.UpdateParams ();
		}


		protected override void UpdateTreeTable()
		{
			TreeTableFiller<SortableNode>.FillContent (this.treeTableController, this.dataFiller, this.visibleSelectedRow, crop: true);
		}


		private SortingInstructions				sortingInstructions;
		private SortableNodeGetter				secondaryNodeGetter;
		private SorterNodeGetter				primaryNodeGetter;
		private PersonsTreeTableFiller			dataFiller;
	}
}
