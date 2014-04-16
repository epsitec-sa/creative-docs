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
	public class AssetsReport : AbstractReport
	{
		public AssetsReport(DataAccessor accessor, NavigationTreeTableController treeTableController)
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

			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Assets), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			this.dataFiller = new AssetsTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<CumulNode>.FillColumns (this.treeTableController, this.dataFiller);

			this.UpdateTreeTable ();

			//	Connexion des événements.
			this.treeTableController.RowClicked     += this.HandleRowClicked;
			this.treeTableController.ContentChanged += this.HandleContentChanged;
		}

		public override void Update()
		{
			this.nodeGetter.SetParams (this.Params.Timestamp, this.Params.RootGuid, this.sortingInstructions);
			this.dataFiller.Timestamp = this.Params.Timestamp;

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
			TreeTableFiller<CumulNode>.FillContent (this.treeTableController, this.dataFiller, this.visibleSelectedRow, crop: true);
		}


		private AssetsParams Params
		{
			get
			{
				return this.reportParams as AssetsParams;
			}
		}


		private SortingInstructions					sortingInstructions;
		private ObjectsNodeGetter					nodeGetter;
		private AbstractTreeTableFiller<CumulNode>	dataFiller;
	}
}
