//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// TreeTable de base, constituée de lignes AbstractTreeTableColumn créées créées avec SetColumns.
	/// La première colonne de gauche est spéciale; elle contient les informations sur l'arborescence
	/// et elle ne fait pas partie des colonnes scrollables horizontalement.
	/// On ne gère ici aucun déplacement vertical.
	/// On se contente d'afficher les AbstractTreeTableColumn passées avec SetColumns.
	/// Un seul événement RowClicked permet de connaître la colonne et la ligne cliquée.
	/// </summary>
	public class TreeTable : Widget
	{
		public TreeTable()
		{
			this.firstWidth   = 200;
			this.headerHeight = 24;
			this.footerHeight = 24;
			this.rowHeight    = 18;

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

			this.columnFirst.CellHovered += delegate (object sender, int row)
			{
				this.SetHilitedHoverRow (row);
			};

			this.columnFirst.CellClicked += delegate (object sender, int row)
			{
				this.OnRowClicked (-1, row);
			};

			this.columnFirst.TreeButtonClicked += delegate (object sender, int row, TreeTableFirstType type)
			{
				this.OnTreeButtonClicked (row, type);
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

				column.CellHovered += delegate (object sender, int row)
				{
					this.SetHilitedHoverRow (row);
				};

				column.CellClicked += delegate (object sender, int row)
				{
					this.OnRowClicked (index, row);
				};
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

		public int								HeaderHeight
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

		public int								FooterHeight
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

		public int								RowHeight
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


		private void SetHilitedHoverRow(int row)
		{
			this.columnFirst.HilitedHoverRow = row;

			foreach (var column in this.Columns)
			{
				column.HilitedHoverRow = row;
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
		private void OnRowClicked(int column, int row)
		{
			if (this.RowClicked != null)
			{
				this.RowClicked (this, column, row);
			}
		}

		public delegate void RowClickedEventHandler(object sender, int column, int row);
		public event RowClickedEventHandler RowClicked;


		private void OnTreeButtonClicked(int row, TreeTableFirstType type)
		{
			if (this.TreeButtonClicked != null)
			{
				this.TreeButtonClicked (this, row, type);
			}
		}

		public delegate void TreeButtonClickedEventHandler(object sender, int row, TreeTableFirstType type);
		public event TreeButtonClickedEventHandler TreeButtonClicked;
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