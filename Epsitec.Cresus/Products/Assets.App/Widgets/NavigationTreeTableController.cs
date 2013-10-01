//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class NavigationTreeTableController
	{
		public void CreateUI(Widget parent)
		{
			this.treeTable = new TreeTable ()
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
		}


		#region TreeTable Facade
		public int								VisibleRowsCount
		{
			get
			{
				return this.treeTable.VisibleRowsCount;
			}
		}

		public TreeTableColumnFirst				ColumnFirst
		{
			get
			{
				return this.treeTable.ColumnFirst;
			}
		}

		public IEnumerable<AbstractTreeTableColumn> Columns
		{
			get
			{
				return this.treeTable.Columns;
			}
		}

		public void SetColumns(List<AbstractTreeTableColumn> columns)
		{
			this.treeTable.SetColumns (columns);
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

			if (visibleRows < 0)
			{
				return;
			}

			this.scroller.Resolution = 1.0m;
			this.scroller.VisibleRangeRatio = System.Math.Min (visibleRows/totalRows, 1.0m);

			this.scroller.MinValue = 0.0m;
			this.scroller.MaxValue = System.Math.Max ((decimal) this.rowsCount - visibleRows, 0.0m);

			this.scroller.SmallChange = 1.0m;
			this.scroller.LargeChange = visibleRows;

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
		#endregion


		private TreeTable treeTable;
		private VScroller scroller;
		private int rowsCount;
	}
}