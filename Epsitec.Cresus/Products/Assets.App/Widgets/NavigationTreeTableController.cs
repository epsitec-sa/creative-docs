//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce contrôleur inclut une TreeTable et un ascenseur vertical permettant la
	/// navigation en fonction du nombre de lignes RowsCount.
	/// Il cache complètement le TreeTable sous-jacent à l'aide d'une Facade.
	/// </summary>
	public class NavigationTreeTableController
	{
		public void CreateUI(Widget parent, int rowHeight = 18, int headerHeight = 22, int footerHeight = 22)
		{
			parent.BackColor = ColorManager.TreeTableBackgroundColor;

			this.treeTable = new TreeTable (rowHeight, headerHeight, footerHeight)
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.scroller = new VScroller ()
			{
				Parent     = parent,
				Dock       = DockStyle.Right,
				Margins    = new Margins (0, 0, this.treeTable.VScrollerTopMargin, this.treeTable.VScrollerBottomMargin),
				IsInverted = true,  // le zéro est en haut
			};

			this.treeTable.SizeChanged += delegate
			{
				this.UpdateScroller ();
			};

			this.treeTable.RowClicked += delegate (object sender, int column, int row)
			{
				this.OnRowClicked (column, row);
			};

			this.treeTable.ContentChanged += delegate (object sender)
			{
				this.OnContentChanged ();
			};

			this.treeTable.TreeButtonClicked += delegate (object sender, int row, TreeTableTreeType type)
			{
				this.OnTreeButtonClicked (row, type);
			};

			this.scroller.ValueChanged += delegate
			{
				this.OnRowChanged ();
			};

			this.UpdateScroller ();
		}


		public int								RowsCount
		{
			get
			{
				return this.rowsCount;
			}
			set
			{
				if (this.rowsCount != value)
				{
					this.rowsCount = value;
					this.UpdateScroller ();
				}
			}
		}

		public int								TopVisibleRow
		{
			get
			{
				return (int) this.scroller.Value;
			}
			set
			{
				this.scroller.Value = value;
			}
		}


		#region TreeTable Facade
		public bool								AllowsMovement
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

		public int								VisibleRowsCount
		{
			get
			{
				return this.treeTable.VisibleRowsCount;
			}
		}

		public void SetColumns(TreeTableColumnDescription[] descriptions, int dockToLeftCount)
		{
			this.treeTable.SetColumns (descriptions, dockToLeftCount);
		}

		public void SetColumnCells(int rank, TreeTableCellTree[] cells)
		{
			this.treeTable.SetColumnCells (rank, cells);
		}

		public void SetColumnCells(int rank, TreeTableCellString[] cells)
		{
			this.treeTable.SetColumnCells (rank, cells);
		}

		public void SetColumnCells(int rank, TreeTableCellDecimal[] cells)
		{
			this.treeTable.SetColumnCells (rank, cells);
		}
		#endregion


		private void UpdateScroller()
		{
			if (this.treeTable == null || this.scroller == null)
			{
				return;
			}

			var totalRows   = (decimal) this.rowsCount;
			var visibleRows = (decimal) this.treeTable.VisibleRowsCount;

			if (visibleRows < 0 || totalRows == 0)
			{
				this.scroller.Resolution = 1.0m;
				this.scroller.VisibleRangeRatio = 1.0m;

				this.scroller.MinValue = 0.0m;
				this.scroller.MaxValue = 1.0m;

				this.scroller.SmallChange = 1.0m;
				this.scroller.LargeChange = 1.0m;
			}
			else
			{
				this.scroller.Resolution = 1.0m;
				this.scroller.VisibleRangeRatio = System.Math.Min (visibleRows/totalRows, 1.0m);

				this.scroller.MinValue = 0.0m;
				this.scroller.MaxValue = System.Math.Max ((decimal) this.rowsCount - visibleRows, 0.0m);

				this.scroller.SmallChange = 1.0m;
				this.scroller.LargeChange = visibleRows;
			}

			this.OnRowChanged ();  // met à jour le tableau
		}


		#region Events handler
		private void OnRowChanged()
		{
			if (this.RowChanged != null)
			{
				this.RowChanged (this);
			}
		}

		public delegate void RowChangedEventHandler(object sender);
		public event RowChangedEventHandler RowChanged;


		private void OnRowClicked(int column, int row)
		{
			if (this.RowClicked != null)
			{
				this.RowClicked (this, column, row);
			}
		}

		public delegate void RowClickedEventHandler(object sender, int column, int row);
		public event RowClickedEventHandler RowClicked;


		private void OnContentChanged()
		{
			if (this.ContentChanged != null)
			{
				this.ContentChanged (this);
			}
		}

		public delegate void ContentChangedEventHandler(object sender);
		public event ContentChangedEventHandler ContentChanged;


		private void OnTreeButtonClicked(int row, TreeTableTreeType type)
		{
			if (this.TreeButtonClicked != null)
			{
				this.TreeButtonClicked (this, row, type);
			}
		}

		public delegate void TreeButtonClickedEventHandler(object sender, int row, TreeTableTreeType type);
		public event TreeButtonClickedEventHandler TreeButtonClicked;
		#endregion


		private TreeTable treeTable;
		private VScroller scroller;
		private int rowsCount;
	}
}