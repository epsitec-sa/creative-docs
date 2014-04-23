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
	public class MCH2SummaryReport : AbstractReport
	{
		public MCH2SummaryReport(DataAccessor accessor, NavigationTreeTableController treeTableController)
			: base (accessor, treeTableController)
		{
		}

		public override void Dispose()
		{
			this.treeTableController.RowClicked        -= this.HandleRowClicked;
			this.treeTableController.ContentChanged    -= this.HandleContentChanged;
			this.treeTableController.TreeButtonClicked -= this.HandleTreeButtonClicked;
		}


		public override void Initialize()
		{
			this.visibleSelectedRow = -1;

			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Assets), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			this.dataFiller = new MCH2SummaryTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<CumulNode>.FillColumns (this.treeTableController, this.dataFiller);

			this.UpdateTreeTable ();

			//	Connexion des événements.
			this.treeTableController.RowClicked        += this.HandleRowClicked;
			this.treeTableController.ContentChanged    += this.HandleContentChanged;
			this.treeTableController.TreeButtonClicked += this.HandleTreeButtonClicked;
		}

		protected override void UpdateParams()
		{
			this.DataFiller.InitialTimestamp = this.Params.InitialTimestamp;
			this.DataFiller.FinalTimestamp   = this.Params.FinalTimestamp;

			var e = this.DataFiller.UsedExtractionInstructions.ToList ();
			this.nodeGetter.SetParams (this.Params.FinalTimestamp, this.Params.RootGuid, this.sortingInstructions, e);
			this.nodeGetter.SetLevel (1);

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

		private void HandleTreeButtonClicked(object sender, int row, NodeType type)
		{
			this.OnCompactOrExpand (this.treeTableController.TopVisibleRow + row);
		}


		protected void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			this.nodeGetter.CompactOrExpand (row);
			this.UpdateTreeTable ();
		}

		private void OnCompactAll()
		{
			//	Compacte toutes les lignes.
			this.nodeGetter.CompactAll ();
			this.UpdateTreeTable ();
		}

		private void OnCompactOne()
		{
			//	Compacte une ligne.
			this.nodeGetter.CompactOne ();
			this.UpdateTreeTable ();
		}

		private void OnExpandOne()
		{
			//	Etend une ligne.
			this.nodeGetter.ExpandOne ();
			this.UpdateTreeTable ();
		}

		private void OnExpandAll()
		{
			//	Etend toutes les lignes.
			this.nodeGetter.ExpandAll ();
			this.UpdateTreeTable ();
		}


		private void UpdateTreeTable()
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


		private SortingInstructions					sortingInstructions;
		private ObjectsNodeGetter					nodeGetter;
		private AbstractTreeTableFiller<CumulNode>	dataFiller;
	}
}
