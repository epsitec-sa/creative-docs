//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class PersonsReport : AbstractReport
	{
		public PersonsReport(DataAccessor accessor, NavigationTreeTableController treeTableController)
			: base (accessor, treeTableController)
		{
		}

		public override void Dispose()
		{
			this.treeTableController.RowClicked     -= this.HandleRowClicked;
			this.treeTableController.ContentChanged -= this.HandleContentChanged;
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

			this.UpdateTreeTable ();

			//	Connexion des événements.
			this.treeTableController.RowClicked     += this.HandleRowClicked;
			this.treeTableController.ContentChanged += this.HandleContentChanged;
		}

		protected override void UpdateParams()
		{
			this.secondaryNodeGetter.SetParams (null, this.sortingInstructions);
			this.primaryNodeGetter.SetParams (this.sortingInstructions);

			this.UpdateTreeTable ();
		}


		private void HandleRowClicked(object sender, int row, int column)
		{
			this.visibleSelectedRow = this.treeTableController.TopVisibleRow + row;
			this.UpdateTreeTable ();
		}

		private void HandleContentChanged(object sender, bool row)
		{
			this.UpdateTreeTable ();
		}

		private void UpdateTreeTable()
		{
			TreeTableFiller<SortableNode>.FillContent (this.treeTableController, this.dataFiller, this.visibleSelectedRow, crop: true);
		}


		private SortingInstructions				sortingInstructions;
		private SortableNodeGetter				secondaryNodeGetter;
		private SorterNodeGetter				primaryNodeGetter;
		private PersonsTreeTableFiller			dataFiller;
	}
}
