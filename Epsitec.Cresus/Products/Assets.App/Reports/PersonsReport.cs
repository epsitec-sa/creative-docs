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

		public override void Initialize()
		{
			this.visibleSelectedRow = -1;

			var primary = this.accessor.GetNodeGetter (BaseType.Persons);
			var secondary = new SortableNodeGetter (primary, this.accessor, BaseType.Persons);
			var nodeGetter = new SorterNodeGetter (secondary);

			var sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Persons), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			secondary.SetParams (null, sortingInstructions);
			(nodeGetter as SorterNodeGetter).SetParams (sortingInstructions);

			var dataFiller = new PersonsTreeTableFiller (this.accessor, nodeGetter);
			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, dataFiller);
			TreeTableFiller<SortableNode>.FillContent (this.treeTableController, dataFiller, this.visibleSelectedRow, crop: true);

			//	Connexion des événements.
			this.treeTableController.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.treeTableController.TopVisibleRow + row;
				TreeTableFiller<SortableNode>.FillContent (this.treeTableController, dataFiller, this.visibleSelectedRow, crop: true);
			};

			this.treeTableController.ContentChanged += delegate (object sender, bool crop)
			{
				TreeTableFiller<SortableNode>.FillContent (this.treeTableController, dataFiller, this.visibleSelectedRow, crop: true);
			};
		}
	}
}
