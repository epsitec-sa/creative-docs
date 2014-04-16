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
			var secondary = new SortableNodeGetter (primary, this.accessor, BaseType.Persons);
			var nodeGetter = new SorterNodeGetter (secondary);

			var sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Persons), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			secondary.SetParams (null, sortingInstructions);
			nodeGetter.SetParams (sortingInstructions);

			this.dataFiller = new PersonsTreeTableFiller (this.accessor, nodeGetter);
			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller);
			this.Update ();

			//	Connexion des événements.
			this.treeTableController.RowClicked     += this.HandleRowClicked;
			this.treeTableController.ContentChanged += this.HandleContentChanged;
		}

		private void HandleRowClicked(object sender, int row, int column)
		{
			this.visibleSelectedRow = this.treeTableController.TopVisibleRow + row;
			this.Update ();
		}

		private void HandleContentChanged(object sender, bool val1)
		{
			this.Update ();
		}

		private void Update()
		{
			TreeTableFiller<SortableNode>.FillContent (this.treeTableController, this.dataFiller, this.visibleSelectedRow, crop: true);
		}


		private PersonsTreeTableFiller			dataFiller;
	}
}
