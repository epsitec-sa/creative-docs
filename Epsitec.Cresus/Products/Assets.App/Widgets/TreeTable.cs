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
		public TreeTable(int rowHeight, int headerHeight, int footerHeight)
		{
			this.rowHeight    = rowHeight;
			this.headerHeight = headerHeight;
			this.footerHeight = footerHeight;

			this.lastColumnSeparatorRank = -1;
			this.lastColumnOrderRank = -1;

			this.treeTableColumns = new List<AbstractTreeTableColumn> ();

			//	Crée le conteneur de gauche, qui contiendra toutes les colonnes
			//	en mode DockToLeft (habituellement la seule TreeTableColumnTree).
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
				Parent  = this,
				Anchor  = AnchorStyles.All,
				Margins = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
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
				column.Index = index++;

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
					if (!this.isDragColumnOrder)
					{
						this.SetHilitedHoverRow (row);
					}
				};

				column.CellClicked += delegate (object sender, int row)
				{
					if (!this.isDragColumnOrder)
					{
						this.OnRowClicked (column.Index, row);
					}
				};

				if (column is TreeTableColumnTree)
				{
					var tree = column as TreeTableColumnTree;

					tree.TreeButtonClicked += delegate (object sender, int row, TreeTableTreeType type)
					{
						this.OnTreeButtonClicked (row, type);
					};
				}
			}

			this.UpdateChildrensGeometry ();
		}

		public void SetColumnCells(int rank, TreeTableCellTree[] cells)
		{
			var column = this.GetColumn (rank) as TreeTableColumnTree;
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


		protected override void OnExited(MessageEventArgs e)
		{
			foreach (var column in this.treeTableColumns)
			{
				column.ClearDetectedHoverRow ();
			}

			base.OnExited (e);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (message.IsMouseType)
			{
				if (message.MessageType == MessageType.MouseDown)
				{
					if (this.lastColumnSeparatorRank != -1)
					{
						this.BeginDragColumnWidth (this.lastColumnSeparatorRank, pos);
					}
					else if (this.lastColumnOrderRank != -1)
					{
						this.BeginDragColumnOrder (this.lastColumnOrderRank, pos);
					}
				}
				else if (message.MessageType == MessageType.MouseUp)
				{
					if (this.isDragColumnWidth)
					{
						this.EndDragColumnWidth (this.lastColumnSeparatorRank, pos);
					}
					if (this.isDragColumnOrder)
					{
						this.EndDragColumnOrder (this.lastColumnOrderRank, pos);
					}
				}
			}

			base.ProcessMessage (message, pos);
		}

		protected override void OnMouseMove(MessageEventArgs e)
		{
			var pos = this.foreground.MapParentToClient (e.Point);

			if (this.isDragColumnWidth)
			{
				this.ProcessDragColumnWidth (this.lastColumnSeparatorRank, pos);
			}
			else if (this.isDragColumnOrder)
			{
				this.ProcessDragColumnOrder (this.lastColumnOrderRank, pos);
			}
			else
			{
				int sepRank = this.DetectColumnSeparator (pos);
				int ordRank = this.DetectColumnOrder     (pos);

				if (sepRank != this.lastColumnSeparatorRank ||
					ordRank != this.lastColumnOrderRank)
				{
					this.lastColumnSeparatorRank = sepRank;
					this.lastColumnOrderRank     = ordRank;

					if (this.lastColumnSeparatorRank != -1)
					{
						var x = this.GetColumnSeparatorX (this.lastColumnSeparatorRank);
						this.ColumnSeparatorUpdateForeground (x);
					}
					else if (this.lastColumnOrderRank != -1)
					{
						var rect = this.GetColumnOrderRect (this.lastColumnOrderRank);
						this.ColumnOrderUpdateForeground (rect);
					}
					else
					{
						this.ColumnSeparatorUpdateForeground (null);
					}
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
			this.dragColumnWidthInitialLeft = this.GetColumnSeparatorX (rank).Value - this.treeTableColumns[rank-1].ActualWidth;
			this.dragColumnWidthInitialWidth = this.treeTableColumns[rank-1].ActualWidth;

		}

		private void ProcessDragColumnWidth(int rank, Point pos)
		{
			var delta = pos.X - this.dragColumnWidthInitialMouse;
			var width = System.Math.Max (this.dragColumnWidthInitialWidth + delta, 0.0);
			this.treeTableColumns[rank-1].PreferredWidth = width;

			var x = this.dragColumnWidthInitialLeft + width;
			this.ColumnSeparatorUpdateForeground (x);
		}

		private void EndDragColumnWidth(int rank, Point pos)
		{
			this.isDragColumnWidth = false;
			this.ColumnSeparatorUpdateForeground (null);
		}
		#endregion


		#region Column separator
		private void ColumnSeparatorUpdateForeground(double? x)
		{
			this.foreground.ClearZones ();

			if (x.HasValue)
			{
				var rect = this.GetColumnSeparatorRect (x.Value, 1);
				this.foreground.AddZone (rect, ColorManager.MoveColumnColor);
			}

			this.foreground.Invalidate ();
		}

		private Rectangle GetColumnSeparatorRect(double x, int thickness)
		{
			x = System.Math.Floor (x);
			return new Rectangle (x-thickness, 0, thickness*2+1, this.foreground.ActualHeight);
		}

		private int DetectColumnSeparator(Point pos)
		{
			if (pos.Y >= 0 && pos.Y < this.foreground.ActualHeight)
			{
				//	A l'envers, pour pouvoir déployer une colonne de largeur nulle.
				//	On saute la colonne 0 qui est tout à gauche.
				for (int i=this.treeTableColumns.Count; i>0; i--)
				{
					double? x = this.GetColumnSeparatorX (i);

					if (x.HasValue &&
						pos.X >= x.Value - 4 &&
						pos.X <= x.Value + 4)
					{
						return i;
					}
				}
			}

			return -1;
		}

		private double? GetColumnSeparatorX(int rank)
		{
			//	Retourne la position d'un frontière. S'il existe n colonnes, on peut
			//	obtenir les positions 0..n (0 = tout à gauche, n = tout à droite).
			if (rank != -1)
			{
				if (rank == 0)  // tout à gauche ?
				{
					var column = this.treeTableColumns[0];
					return column.ActualBounds.Left;
				}
				else  // cherche une frontière droite ?
				{
					rank--;  // 0..n-1
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
			}

			return null;
		}
		#endregion


		#region Drag column order
		private void BeginDragColumnOrder(int rank, Point pos)
		{
			this.isDragColumnOrder = true;
			this.dragColumnOrderInitialMouse = pos.X;
			this.dragColumnOrderInitialRect = this.GetColumnOrderRect (rank);
			this.dragColumnOrderDstRank = -1;
		}

		private void ProcessDragColumnOrder(int rank, Point pos)
		{
			var delta = pos.X - this.dragColumnOrderInitialMouse;
			delta += this.dragColumnOrderInitialMouse - this.dragColumnOrderInitialRect.Center.X;
			var rect = Rectangle.Offset (this.dragColumnOrderInitialRect, delta, 0);

			this.dragColumnOrderDstRank = this.DetectColumnOrderDst (pos);

			if (this.dragColumnOrderDstRank == rank ||  // aucun sens si drag juste avant
				this.dragColumnOrderDstRank == rank+1)  // aucun sens si drag juste après
			{
				this.dragColumnOrderDstRank = -1;
			}

			var x = this.GetColumnSeparatorX (this.dragColumnOrderDstRank);
			this.ColumnOrderUpdateForeground (this.dragColumnOrderInitialRect, rect, x, this.dragColumnOrderInitialRect.Width);
		}

		private void EndDragColumnOrder(int rank, Point pos)
		{
			this.isDragColumnOrder = false;
			this.ColumnSeparatorUpdateForeground (null);
		}
		#endregion


		#region Column order
		private void ColumnOrderUpdateForeground(Rectangle src, Rectangle dst, double? dstX, double dstWidth)
		{
			this.foreground.ClearZones ();

			if (src.IsValid)
			{
				//	La colonne source est fortement estompée, pour donner l'illusion
				//	qu'elle a disparu.
				src = new Rectangle (src.Left, 0, src.Width, this.foreground.ActualHeight);
				this.foreground.AddZone (src, Color.FromAlphaRgb (0.8, 1.0, 1.0, 1.0));
			}

			if (dst.IsValid)
			{
				//	L'en-tête destination est dessinée pour ressembler au maximum
				//	à une en-tête normale.
				var color = Color.FromAlphaColor (0.8, ColorManager.TreeTableBackgroundColor);
				this.foreground.AddZone (dst, color);

				//	On dessine un rectangle plus foncé autour.
				var tr = new Rectangle (dst.Left,    dst.Top-1,  dst.Width, 1         );
				var br = new Rectangle (dst.Left,    dst.Bottom, dst.Width, 1         );
				var lr = new Rectangle (dst.Left,    dst.Bottom, 1,         dst.Height);
				var rr = new Rectangle (dst.Right-1, dst.Bottom, 1,         dst.Height);

				color = ColorManager.TreeTableBackgroundColor.Delta (-0.3);
				
				this.foreground.AddZone (tr, color);
				this.foreground.AddZone (br, color);
				this.foreground.AddZone (lr, color);
				this.foreground.AddZone (rr, color);
			}

			if (dstX.HasValue)
			{
				var rect = this.GetColumnSeparatorRect (dstX.Value, (int) (dstWidth/2));
				rect.Deflate (0, 0, this.headerHeight, 0);
				this.foreground.AddZone (rect, Color.FromAlphaRgb (0.9, 0.9, 0.9, 0.9));

				var lr = new Rectangle (rect.Left,    rect.Bottom, 1, rect.Height);
				var mr = new Rectangle (dstX.Value-1, rect.Bottom, 3, rect.Height);
				var rr = new Rectangle (rect.Right-1, rect.Bottom, 1, rect.Height);

				this.foreground.AddZone (lr, Color.FromAlphaRgb (0.2, 0.0, 0.0, 0.0));
				this.foreground.AddZone (mr, ColorManager.HoverColor);
				this.foreground.AddZone (rr, Color.FromAlphaRgb (0.2, 0.0, 0.0, 0.0));
			}

			this.foreground.Invalidate ();
		}

		private void ColumnOrderUpdateForeground(Rectangle rect)
		{
			this.foreground.ClearZones ();

			if (rect.IsValid)
			{
				var color = Color.FromAlphaColor (0.4, ColorManager.MoveColumnColor);
				this.foreground.AddZone (rect, color);
			}

			this.foreground.Invalidate ();
		}

		private int DetectColumnOrderDst(Point pos)
		{
			if (pos.Y >= 0 && pos.Y < this.foreground.ActualHeight)
			{
				double x1 = 0;

				for (int i=0; i<this.treeTableColumns.Count; i++)
				{
					double? x = this.GetColumnSeparatorX (i+1);  // une frontière droite

					if (x.HasValue)
					{
						var x2 = x.Value;

						if (pos.X < (x1+x2)/2)
						{
							return i;
						}

						x1 = x2;
					}
				}
			}

			return -1;
		}

		private int DetectColumnOrder(Point pos)
		{
			for (int i=0; i<this.treeTableColumns.Count; i++)
			{
				var rect = this.GetColumnOrderRect (i);

				if (rect.Contains (pos))
				{
					return i;
				}
			}

			return -1;
		}

		private Rectangle GetColumnOrderRect(int rank)
		{
			double x1 = 0;
			double x2 = 0;

			if (rank != -1)
			{
				var column = this.treeTableColumns[rank];
				
				if (column.DockToLeft)
				{
					x1 = column.ActualBounds.Left;
					x2 = column.ActualBounds.Right;
				}
				else
				{
					double start  = this.columnsContainer.ActualBounds.Left;
					double offset = this.columnsContainer.ViewportOffsetX;

					x1 = start - offset + column.ActualBounds.Left;
					x2 = start - offset + column.ActualBounds.Right;

					x1 = System.Math.Max (x1, start);
					x2 = System.Math.Max (x2, start);
				}
			}

			if (x1 < x2)
			{
				return new Rectangle (x1, this.foreground.ActualHeight-this.headerHeight, x2-x1, this.headerHeight);
			}
			else
			{
				return Rectangle.Empty;
			}
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


		private readonly List<AbstractTreeTableColumn> treeTableColumns;
		private readonly FrameBox				leftContainer;
		private readonly Scrollable				columnsContainer;
		private readonly Foreground				foreground;

		private int								headerHeight;
		private int								footerHeight;
		private int								rowHeight;

		private int								lastColumnSeparatorRank;
		private bool							isDragColumnWidth;
		private double							dragColumnWidthInitialMouse;
		private double							dragColumnWidthInitialLeft;
		private double							dragColumnWidthInitialWidth;

		private int								lastColumnOrderRank;
		private bool							isDragColumnOrder;
		private double							dragColumnOrderInitialMouse;
		private Rectangle						dragColumnOrderInitialRect;
		private int								dragColumnOrderDstRank;
	}
}