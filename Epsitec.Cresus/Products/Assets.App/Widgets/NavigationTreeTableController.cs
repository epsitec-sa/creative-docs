//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;

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
			parent.BackColor = ColorManager.TreeTableOutColor;

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

			this.treeTable.RowClicked += delegate (object sender, int row, int column)
			{
				this.OnRowClicked (row, column);
			};

			this.treeTable.RowDoubleClicked += delegate (object sender, int row)
			{
				this.OnRowDoubleClicked (row);
			};

			this.treeTable.ContentChanged += delegate (object sender, bool crop)
			{
				this.OnContentChanged (crop);
			};

			this.treeTable.SortingChanged += delegate (object sender)
			{
				this.OnSortingChanged ();
			};

			this.treeTable.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnTreeButtonClicked (row, type);
			};

			this.treeTable.DokeySelect += delegate (object sender, KeyCode key)
			{
				this.OnDokeySelect (key);
			};

			this.scroller.ValueChanged += delegate
			{
				this.OnContentChanged (false);
			};
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

		public int GetTopVisibleRow(int sel)
		{
			//?System.Diagnostics.Debug.Assert (this.treeTable.VisibleRowsCount > 0);
			int count = System.Math.Min (this.treeTable.VisibleRowsCount, this.rowsCount);

			sel = System.Math.Min (sel + count/2, this.rowsCount-1);
			sel = System.Math.Max (sel - count+1, 0);

			return sel;
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

		public bool								AllowsSorting
		{
			get
			{
				return this.treeTable.AllowsSorting;
			}
			set
			{
				this.treeTable.AllowsSorting = value;
			}
		}

		public int								VisibleRowsCount
		{
			get
			{
				return this.treeTable.VisibleRowsCount;
			}
		}

		public ColumnsState						ColumnsState
		{
			get
			{
				return this.treeTable.ColumnsState;
			}
		}

		public void SetColumns(TreeTableColumnDescription[] descriptions, SortingInstructions defaultSorting, int defaultDockToLeftCount, string treeTableName)
		{
			this.treeTable.SetColumns (descriptions, defaultSorting, defaultDockToLeftCount, treeTableName);
		}

		public void SetColumnCells(int rank, TreeTableColumnItem columnItem)
		{
			this.treeTable.SetColumnCells (rank, columnItem);
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
				this.scroller.MaxValue = System.Math.Max (totalRows - visibleRows, 0.0m);

				this.scroller.SmallChange = 1.0m;
				this.scroller.LargeChange = visibleRows;
			}

			this.OnContentChanged (true);  // on demande de mettre à jour le contenu
		}


		#region Events handler
		private void OnRowClicked(int row, int column)
		{
			this.RowClicked.Raise (this, row, column);
		}

		public event EventHandler<int, int> RowClicked;


		private void OnRowDoubleClicked(int row)
		{
			this.RowDoubleClicked.Raise (this, row);
		}

		public event EventHandler<int> RowDoubleClicked;


		private void OnContentChanged(bool crop)
		{
			this.ContentChanged.Raise (this, crop);
		}

		public event EventHandler<bool> ContentChanged;


		private void OnSortingChanged()
		{
			this.SortingChanged.Raise (this);
		}

		public event EventHandler SortingChanged;


		private void OnTreeButtonClicked(int row, NodeType type)
		{
			this.TreeButtonClicked.Raise (this, row, type);
		}

		public event EventHandler<int, NodeType> TreeButtonClicked;


		private void OnDokeySelect(KeyCode key)
		{
			this.DokeySelect.Raise (this, key);
		}

		public event EventHandler<KeyCode> DokeySelect;
		#endregion


		private TreeTable						treeTable;
		private VScroller						scroller;
		private int								rowsCount;
	}
}