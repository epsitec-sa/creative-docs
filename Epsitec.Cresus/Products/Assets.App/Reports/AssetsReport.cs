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
			this.treeTableController.RowClicked -= this.HandleRowClicked;
			this.treeTableController.ContentChanged -= this.HandleContentChanged;
		}

		public void SetParams(Timestamp timestamp, Guid rootGuid)
		{
			this.timestamp = timestamp;
			this.rootGuid = rootGuid;
		}

		public override void Initialize()
		{
			this.visibleSelectedRow = -1;

			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			var nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			var sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Assets), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			nodeGetter.SetParams (this.timestamp, this.rootGuid, sortingInstructions);

			this.dataFiller = new AssetsTreeTableFiller (this.accessor, nodeGetter);
			this.dataFiller.Timestamp = this.timestamp;
			TreeTableFiller<CumulNode>.FillColumns (this.treeTableController, this.dataFiller);
			this.Update ();

			//	Connexion des événements.
			this.treeTableController.RowClicked += this.HandleRowClicked;
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
			TreeTableFiller<CumulNode>.FillContent (this.treeTableController, this.dataFiller, this.visibleSelectedRow, crop: true);
		}


		private AbstractTreeTableFiller<CumulNode> dataFiller;
		private Timestamp					timestamp;
		private Guid						rootGuid;
	}
}
