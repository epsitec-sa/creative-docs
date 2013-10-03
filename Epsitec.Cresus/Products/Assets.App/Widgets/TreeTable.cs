﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public TreeTable(int rowHeight, int headerHeight, int footerHeight)
		{
			this.rowHeight    = rowHeight;
			this.headerHeight = headerHeight;
			this.footerHeight = footerHeight;

			this.lastColumnSeparatorRank = -1;

			this.treeTableColumns = new List<AbstractTreeTableColumn> ();

			//	Crée le conteneur de gauche, qui contiendra toutes les colonnes
			//	en mode DockToLeft (habituellement la seule TreeTableColumnGlyph).
			//	Ce conteneur n'est pas scrollable horizontalement; sa largeur
			//	s'adapte en fonctions du total des colonnes contenues.
			this.leftContainer = new FrameBox
			{
				Parent         = this,
				Dock           = DockStyle.Left,
				PreferredWidth = 0,
				Margins        = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
			};

			//	Crée le conteneur de droite, qui contiendra toutes les colonnes
			//	qui n'ont pas le mode DockToLeft. Ce conteneur est scrollable
			//	horizontalement.
			this.columnsContainer = new Scrollable
			{
				Parent                 = this,
				HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.HideAlways,
				Dock                   = DockStyle.Fill,
			};

			this.columnsContainer.Viewport.IsAutoFitting = true;

			//	Crée la surcouche.
			this.foreground = new Foreground
			{
				Parent       = this,
				Anchor       = AnchorStyles.All,
				Margins      = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
				HilitedColor = ColorManager.MoveColumnColor,
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

		public int								VisibleRowsCount
		{
			get
			{
				return (int) ((this.ActualHeight - this.headerHeight - this.footerHeight - AbstractScroller.DefaultBreadth) / this.rowHeight);
			}
		}


		public void SetColumns(TreeTableColumnDescription[] descriptions)
		{
			this.treeTableColumns.Clear ();
			this.leftContainer.Children.Clear ();
			this.columnsContainer.Viewport.Children.Clear ();

			int index = 0;

			foreach (var description in descriptions)
			{
				var column = TreeTableColumnDescription.Create (description);
				column.ColumnIndex = index++;

				this.treeTableColumns.Add (column);

				if (description.DockToLeft)
				{
					column.Dock = DockStyle.Left;
					this.leftContainer.Children.Add (column);
				}
				else
				{
					column.Dock = DockStyle.Left;
					this.columnsContainer.Viewport.Children.Add (column);
				}

				column.CellHovered += delegate (object sender, int row)
				{
					this.SetHilitedHoverRow (row);
				};

				column.CellClicked += delegate (object sender, int row)
				{
					this.OnRowClicked (index, row);
				};

				if (column is TreeTableColumnGlyph)
				{
					var glyph = column as TreeTableColumnGlyph;

					glyph.TreeButtonClicked += delegate (object sender, int row, TreeTableGlyphType type)
					{
						this.OnTreeButtonClicked (row, type);
					};
				}
			}

			this.UpdateChildrensGeometry ();
		}

		public void SetColumnCells(int rank, TreeTableCellGlyph[] cells)
		{
			var column = this.GetColumn (rank) as TreeTableColumnGlyph;
			System.Diagnostics.Debug.Assert (column != null);

			column.SetCells (cells);
		}

		public void SetColumnCells(int rank, TreeTableCellString[] cells)
		{
			var column = this.GetColumn (rank) as TreeTableColumnString;
			System.Diagnostics.Debug.Assert (column != null);

			column.SetCells (cells);
		}

		public void SetColumnCells(int rank, TreeTableCellDecimal[] cells)
		{
			var column = this.GetColumn (rank) as TreeTableColumnDecimal;
			System.Diagnostics.Debug.Assert (column != null);

			column.SetCells (cells);
		}


		protected override void OnEntered(MessageEventArgs e)
		{
			this.ShowGrid = true;
			base.OnEntered (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			this.ShowGrid = false;
			base.OnExited (e);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.IsMouseType)
			{
				if (message.MessageType == MessageType.MouseDown)
				{
					if (this.lastColumnSeparatorRank != -1)
					{
						this.BeginDragColumnWidth (this.lastColumnSeparatorRank, pos);
					}
				}
				else if (message.MessageType == MessageType.MouseUp)
				{
					if (this.isDragColumnWidth)
					{
						this.EndDragColumnWidth (this.lastColumnSeparatorRank, pos);
					}
				}
			}

			base.ProcessMessage (message, pos);
		}

		protected override void OnMouseMove(MessageEventArgs e)
		{
			if (this.isDragColumnWidth)
			{
				this.MoveDragColumnWidth (this.lastColumnSeparatorRank, e.Point);
			}
			else
			{
				int rank = this.DetectColumnSeparator (e.Point);

				if (this.lastColumnSeparatorRank != rank)
				{
					this.lastColumnSeparatorRank = rank;

					var x = this.GetColumnSeparatorX (this.lastColumnSeparatorRank);
					this.ColumnSeparatorUpdateForeground (x);
				}
			}

			base.OnMouseMove (e);
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateChildrensGeometry ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (new Rectangle (Point.Zero, this.ActualSize));
			graphics.RenderSolid (ColorManager.TreeTableBackgroundColor);
		}


		private bool ShowGrid
		{
			get
			{
				return this.showGrid;
			}
			set
			{
				if (this.showGrid != value)
				{
					this.showGrid = value;

					foreach (var column in this.treeTableColumns)
					{
						column.ShowGrid = this.showGrid;
					}
				}
			}
		}

		private void SetHilitedHoverRow(int row)
		{
			foreach (var column in this.treeTableColumns)
			{
				column.HilitedHoverRow = row;
			}
		}

		private void UpdateChildrensGeometry()
		{
			foreach (var column in this.treeTableColumns)
			{
				column.HeaderHeight = this.headerHeight;
				column.FooterHeight = this.footerHeight;
				column.RowHeight    = this.rowHeight;
			}
		}

		private AbstractTreeTableColumn GetColumn(int rank)
		{
			System.Diagnostics.Debug.Assert (rank >= 0 && rank < this.treeTableColumns.Count);
			return this.treeTableColumns[rank];
		}


		#region Drag column width
		private void BeginDragColumnWidth(int rank, Point pos)
		{
			this.isDragColumnWidth = true;
			this.dragColumnWidthInitialMouse = pos.X;
			this.dragColumnWidthInitialLeft = this.GetColumnSeparatorX (rank).Value - this.treeTableColumns[rank].ActualWidth;
			this.dragColumnWidthInitialWidth = this.treeTableColumns[rank].ActualWidth;

		}

		private void MoveDragColumnWidth(int rank, Point pos)
		{
			var delta = pos.X - this.dragColumnWidthInitialMouse;
			var width = System.Math.Max (this.dragColumnWidthInitialWidth + delta, 0.0);
			this.treeTableColumns[rank].PreferredWidth = width;

			var x = this.dragColumnWidthInitialLeft + width;
			this.ColumnSeparatorUpdateForeground (x);
		}

		private void EndDragColumnWidth(int rank, Point pos)
		{
			this.isDragColumnWidth = false;
		}
		#endregion


		#region Column separator
		private void ColumnSeparatorUpdateForeground(double? x)
		{
			if (x.HasValue)
			{
				this.foreground.HilitedZone = this.GetColumnSeparatorRect (x.Value);
			}
			else
			{
				this.foreground.HilitedZone = Rectangle.Empty;
			}
		}

		private Rectangle GetColumnSeparatorRect(double x)
		{
			x = System.Math.Floor (x);
			return new Rectangle (x-1, 0, 3, this.ActualHeight);
		}

		private int DetectColumnSeparator(Point pos)
		{
			if (pos.Y > AbstractScroller.DefaultBreadth)
			{
				//	A l'envers, pour pouvoir déployer une colonne de largeur nulle.
				for (int i=this.treeTableColumns.Count-1; i>=0; i--)
				{
					double? x = this.GetColumnSeparatorX (i);

					if (x.HasValue &&
						pos.X >= x.Value - TreeTable.colomnSeparatorWidth &&
						pos.X <= x.Value + TreeTable.colomnSeparatorWidth)
					{
						return i;
					}
				}
			}

			return -1;
		}

		private double? GetColumnSeparatorX(int rank)
		{
			if (rank != -1)
			{
				var column = this.treeTableColumns[rank];

				if (column.DockToLeft)
				{
					return column.ActualBounds.Right;
				}
				else
				{
					double offset = this.columnsContainer.ViewportOffsetX;
					double position = column.ActualBounds.Right;

					if (position > offset)
					{
						var x = this.columnsContainer.ActualBounds.Left - offset + position;

						if (rank == this.treeTableColumns.Count-1)  // dernière colonne ?
						{
							x -= 2;  // pour ne pas être sous l'ascenseur vertical
						}

						return x;
					}
				}
			}

			return null;
		}
		#endregion


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


		private void OnTreeButtonClicked(int row, TreeTableGlyphType type)
		{
			if (this.TreeButtonClicked != null)
			{
				this.TreeButtonClicked (this, row, type);
			}
		}

		public delegate void TreeButtonClickedEventHandler(object sender, int row, TreeTableGlyphType type);
		public event TreeButtonClickedEventHandler TreeButtonClicked;
		#endregion


		private static readonly double colomnSeparatorWidth = 6;

		private readonly List<AbstractTreeTableColumn> treeTableColumns;
		private readonly FrameBox				leftContainer;
		private readonly Scrollable				columnsContainer;
		private readonly Foreground				foreground;

		private int								headerHeight;
		private int								footerHeight;
		private int								rowHeight;
		private bool							showGrid;
		private int								lastColumnSeparatorRank;
		private bool							isDragColumnWidth;
		private double							dragColumnWidthInitialMouse;
		private double							dragColumnWidthInitialLeft;
		private double							dragColumnWidthInitialWidth;
	}
}