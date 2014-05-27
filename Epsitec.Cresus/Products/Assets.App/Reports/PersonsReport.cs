//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class PersonsReport : AbstractReport
	{
		public PersonsReport(DataAccessor accessor, ReportsView reportView)
			: base (accessor, reportView)
		{
		}


		public override string					TreeTableName
		{
			get
			{
				return "View.Report.Persons";
			}
		}


		public override void Initialize()
		{
			this.visibleSelectedRow = -1;

			var primary = this.accessor.GetNodeGetter (BaseType.Persons);
			this.secondaryNodeGetter = new SortableNodeGetter (primary, this.accessor, BaseType.Persons);
			this.primaryNodeGetter = new SorterNodeGetter (this.secondaryNodeGetter);

			this.sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Persons), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			this.dataFiller = new PersonsTreeTableFiller (this.accessor, this.primaryNodeGetter);
			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller);

			base.Initialize ();
		}

		public override void UpdateParams()
		{
			this.secondaryNodeGetter.SetParams (null, this.sortingInstructions);
			this.primaryNodeGetter.SetParams (this.sortingInstructions);

			this.UpdateTreeTable ();
			this.OnParamsChanged ();
		}

		public override void ShowExportPopup(Widget target)
		{
			ExportHelpers<SortableNode>.StartExportProcess (target, this.accessor, this.dataFiller);
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
