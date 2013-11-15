//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce contrôleur pilote un NavigationTreeTableController d'après un filler.
	/// </summary>
	public class SimpleTreeTableController
	{
		public SimpleTreeTableController(AbstractTreeTableFiller<GuidNode> dataFiller, int rowsCount)
		{
			this.dataFiller = dataFiller;

			this.treeTable = new NavigationTreeTableController ();
			this.treeTable.RowsCount = rowsCount;

			this.selectedRow = -1;
		}


		public void CreateUI(Widget parent, int rowHeight = 18, int headerHeight = 24, int footerHeight = 24)
		{
			this.treeTable.CreateUI (parent, rowHeight, headerHeight, footerHeight);
			TreeTableFiller<GuidNode>.FillColumns (dataFiller, this.treeTable);

			this.treeTable.RowClicked += delegate (object sender, int row)
			{
				this.selectedRow = this.treeTable.TopVisibleRow + row;
				this.UpdateTreeTableController ();
				this.OnRowClicked (this.selectedRow);
			};

			this.treeTable.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateTreeTableController (crop);
			};
		}


		public bool AllowsMovement
		{
			get
			{
				return this.treeTable.AllowsMovement;
			}
			set
			{
				this.treeTable.AllowsMovement = value;
			}
		}

		public int SelectedRow
		{
			get
			{
				return this.selectedRow;
			}
			set
			{
				if (this.selectedRow != value)
				{
					this.selectedRow = value;
					this.UpdateTreeTableController ();
				}
			}
		}

		private void UpdateTreeTableController(bool crop = true)
		{
			int visibleCount = this.treeTable.VisibleRowsCount;
			int rowsCount    = this.treeTable.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.treeTable.TopVisibleRow;
			int selection    = this.selectedRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = this.treeTable.GetTopVisibleRow (selection);
				}

				if (this.treeTable.TopVisibleRow != firstRow)
				{
					this.treeTable.TopVisibleRow = firstRow;
				}

				selection -= this.treeTable.TopVisibleRow;
			}

			TreeTableFiller<GuidNode>.FillContent (this.dataFiller, this.treeTable, firstRow, count, selection);
		}


		#region Events handler
		private void OnRowClicked(int row)
		{
			if (this.RowClicked != null)
			{
				this.RowClicked (this, row);
			}
		}

		public delegate void RowClickedEventHandler(object sender, int row);
		public event RowClickedEventHandler RowClicked;
		#endregion


		private readonly AbstractTreeTableFiller<GuidNode>	dataFiller;
		private readonly NavigationTreeTableController		treeTable;

		private int											selectedRow;
	}
}