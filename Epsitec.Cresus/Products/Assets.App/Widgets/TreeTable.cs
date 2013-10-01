//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class TreeTable : Widget
	{
		public TreeTable()
		{
			this.firstWidth   = 200;
			this.headerHeight = 30;
			this.footerHeight = 30;
			this.rowHeight    = 20;

			this.firstContainer = new FrameBox
			{
				Dock           = DockStyle.Left,
				PreferredWidth = this.firstWidth,
				Margins        = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
			};

			this.columnsContainer = new Scrollable
			{
				HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.HideAlways,
				Dock                   = DockStyle.Fill,
			};

			this.Children.Add (this.firstContainer);
			this.Children.Add (this.columnsContainer);

			this.columnFirst = new TreeTableColumnFirst
			{
				Parent         = this.firstContainer,
				Dock           = DockStyle.Fill,
				PreferredWidth = this.firstWidth,
			};
		}

		public int								VScrollerTopMargin
		{
			get
			{
				return this.headerHeight;
			}
		}

		public int								VScrollerBottomMargin
		{
			get
			{
				return this.footerHeight + (int) AbstractScroller.DefaultBreadth;
			}
		}

		public TreeTableColumnFirst				ColumnFirst
		{
			get
			{
				return this.columnFirst;
			}
		}

		public void SetColumns(List<AbstractTreeTableColumn> columns)
		{
			this.columnsContainer.Viewport.IsAutoFitting = true;
			this.columnsContainer.Viewport.Children.Clear ();

			int index = 0;

			foreach (var column in columns)
			{
				column.ColumnIndex = index++;
				this.columnsContainer.Viewport.Children.Add (column);
			}

			this.UpdateChildrensGeometry ();
		}

		public IEnumerable<AbstractTreeTableColumn> Columns
		{
			get
			{
				return this.columnsContainer.Viewport.Children.Cast<AbstractTreeTableColumn> ();
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (new Rectangle (Point.Zero, this.ActualSize));
			graphics.RenderSolid (ColorManager.TreeTableBackgroundColor);
		}


		private int								FirstWidth
		{
			get
			{
				return this.firstWidth;
			}
			set
			{
				if (this.firstWidth != value)
				{
					this.firstWidth = value;

					this.firstContainer.PreferredWidth = this.firstWidth;
					this.columnFirst.PreferredWidth = this.firstWidth;
				}
			}
		}

		private int								HeaderHeight
		{
			get
			{
				return this.headerHeight;
			}
			set
			{
				if (this.headerHeight != value)
				{
					this.headerHeight = value;
					this.Invalidate ();
				}
			}
		}

		private int								FooterHeight
		{
			get
			{
				return this.footerHeight;
			}
			set
			{
				if (this.footerHeight != value)
				{
					this.footerHeight = value;
					this.Invalidate ();
				}
			}
		}

		private int								RowHeight
		{
			get
			{
				return this.rowHeight;
			}
			set
			{
				if (this.rowHeight != value)
				{
					this.rowHeight = value;
					this.Invalidate ();
				}
			}
		}

		public int								VisibleRowsCount
		{
			get
			{
				return (int) ((this.ActualHeight - this.headerHeight - this.footerHeight - AbstractScroller.DefaultBreadth) / this.rowHeight);
			}
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateChildrensGeometry ();
		}

		private void UpdateChildrensGeometry()
		{
			this.columnFirst.HeaderHeight = this.headerHeight;
			this.columnFirst.FooterHeight = this.footerHeight;
			this.columnFirst.RowHeight    = this.rowHeight;

			foreach (var column in this.Columns)
			{
				column.Dock = DockStyle.Left;

				column.HeaderHeight = this.headerHeight;
				column.FooterHeight = this.footerHeight;
				column.RowHeight    = this.rowHeight;
			}
		}


		#region Events handler
		private void OnRowClicked(int row, int rank)
		{
			if (this.RowClicked != null)
			{
				this.RowClicked (this, row, rank);
			}
		}

		public delegate void RowClickedEventHandler(object sender, int row, int rank);
		public event RowClickedEventHandler RowClicked;
		#endregion


		private readonly FrameBox				firstContainer;
		private readonly Scrollable				columnsContainer;
		private readonly TreeTableColumnFirst	columnFirst;

		private int								firstWidth;
		private int								headerHeight;
		private int								footerHeight;
		private int								rowHeight;
	}
}