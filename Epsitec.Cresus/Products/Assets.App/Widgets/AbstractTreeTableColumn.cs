//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public abstract class AbstractTreeTableColumn : Widget, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public AbstractTreeTableColumn()
		{
			this.textLayout = new TextLayout ();

			this.hilitedHoverRow = -1;
			this.sortedType = SortedType.None;

			//	Il est inutile d'enclencher les tooltips dynamiques, car les colonnes
			//	sont "sous" un widget AbstractInteractiveLayer, ce qui empêche
			//	l'appel automatique normal.
			//- ToolTip.Default.RegisterDynamicToolTipHost (this);  // pour voir les tooltips dynamiques
		}


		public ObjectField						Field
		{
			get;
			set;
		}

		public bool								DockToLeft
		{
			get;
			set;
		}

		public string							HeaderDescription
		{
			get;
			set;
		}

		public string							HeaderTooltip
		{
			get;
			set;
		}

		public string							FooterDescription
		{
			get;
			set;
		}

		public int								HeaderHeight
		{
			get;
			set;
		}

		public int								FooterHeight
		{
			get;
			set;
		}

		public int								RowHeight
		{
			get;
			set;
		}

		public int								VerticalAdjust
		{
			get;
			set;
		}

		public bool								AllowsMovement
		{
			get;
			set;
		}

		public bool								AllowsSorting
		{
			get;
			set;
		}

		public int								VisibleCellCount
		{
			get
			{
				return (int) ((this.ActualHeight - this.HeaderHeight - this.FooterHeight) / this.RowHeight);
			}
		}

		public TreeTableHoverMode				HoverMode
		{
			get
			{
				return this.hoverMode;
			}
			set
			{
				if (this.hoverMode != value)
				{
					this.hoverMode = value;
					this.Invalidate ();
				}
			}
		}

		public int								HilitedHoverRow
		{
			get
			{
				return this.hilitedHoverRow;
			}
			set
			{
				if (this.hilitedHoverRow != value)
				{
					this.hilitedHoverRow = value;
					this.Invalidate ();
				}
			}
		}


		public void SetSortedColumn(SortedType type, bool primary)
		{
			this.sortedType    = type;
			this.sortedPrimary = primary;
		}


		public virtual void SetCells(TreeTableColumnItem columnItem)
		{
			this.cells = columnItem.Cells;
			this.Invalidate ();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.PaintHeader (graphics);
			this.PaintFooter (graphics);

			if (this.cells != null)
			{
				int y = 0;

				foreach (var cell in this.cells)
				{
					//	Dessine le fond.
					var rect = this.GetCellsRect (y);

					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (this.GetCellColor (y == this.hilitedHoverRow, cell.CellState));

					if ((cell.CellState & CellState.Unavailable) != 0)
					{
						this.PaintUnavailable (graphics, rect, y, this.hilitedHoverRow);
					}

					//	Dessine la valeur.
					this.PaintCell (graphics, rect, y, cell);

					//	Dessine la grille.
					this.PaintGrid (graphics, rect, y, this.hilitedHoverRow);

					y++;
				}
			}
		}

		protected virtual void PaintCell(Graphics graphics, Rectangle rect, int y, AbstractTreeTableCell cell)
		{
		}
		

		private void PaintHeader(Graphics graphics)
		{
			if (this.HeaderHeight > 0)
			{
				var rect = this.HeaderRect;

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (ColorManager.TreeTableBackgroundColor);

				if (!string.IsNullOrEmpty (this.HeaderDescription))
				{
					rect = this.GetContentDeflateRectangle (rect, considerSorting: true);
					this.PaintText (graphics, rect, this.HeaderDescription);
				}

				if (this.sortedType != SortedType.None)
				{
					this.PaintSorted (graphics, this.HeaderRect, this.sortedType, this.sortedPrimary);
				}

				//	Si l'en-tête est haute, on dessine un trait de séparation en dessous.
				if (this.HeaderHeight >= this.RowHeight*2)
				{
					rect = this.HeaderRect;
					rect.Deflate (0, 0.5);

					graphics.AddLine (rect.BottomLeft, rect.BottomRight);
					graphics.RenderSolid (ColorManager.GridColor);
				}
			}
		}

		private void PaintFooter(Graphics graphics)
		{
			if (this.FooterHeight > 0)
			{
				var rect = this.FooterRect;

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (ColorManager.TreeTableBackgroundColor);

				if (!string.IsNullOrEmpty (this.FooterDescription))
				{
					rect = this.GetContentDeflateRectangle (rect);
					this.PaintText (graphics, rect, this.FooterDescription);
				}
			}
		}


		protected void PaintUnavailable(Graphics graphics, Rectangle rect, int currentRow, int hilitedRow)
		{
			//	Dessine une cellule inaccessible d'un objet de regroupement.
			if (currentRow == hilitedRow)
			{
				var reference = this.MapParentToClient (Point.Zero);

				PaintHatch.Paint (graphics, rect, reference, 0.3);
			}
		}

		protected void PaintGrid(Graphics graphics, Rectangle rect, int currentRow, int hilitedRow)
		{
			//	Dessine une portion de grille dans une cellule, selon le mode choisi.

			if (this.hoverMode == TreeTableHoverMode.VerticalGradient && hilitedRow != -1)
			{
				//	Dessine une portion de grille dans une cellule, sous forme de 2 traits,
				//	en bas et à droite. Plus la distance jusqu'à la cellule survolée est
				//	grande et plus l'effet est estompé.
				rect.Deflate (0.5);

				graphics.AddLine (rect.BottomLeft, rect.BottomRight);
				graphics.AddLine (rect.BottomRight, rect.TopRight);

				var delta = System.Math.Abs (currentRow - hilitedRow);
				var alpha = System.Math.Max (1.0 - delta * 0.1, 0.0);
				var color = Color.FromAlphaColor (alpha, ColorManager.GridColor);

				graphics.RenderSolid (color);
			}

			if (this.hoverMode == TreeTableHoverMode.OnlyVerticalSeparators)
			{
				//	Dessine une portion de séparteur vertical dans une cellule, sous la
				//	forme d'un trait à droite.
				rect.Deflate (0.5);

				graphics.AddLine (rect.BottomRight, rect.TopRight);
				graphics.RenderSolid (ColorManager.GridColor);
			}
		}

		private void PaintSorted(Graphics graphics, Rectangle rect, SortedType type, bool primary)
		{
			if (primary)
			{
				rect = new Rectangle (rect.Right-rect.Height, rect.Bottom, rect.Height, rect.Height);
				rect.Deflate ((int) (rect.Height*0.35));

				var path = AbstractTreeTableColumn.GetSortedPath (rect, type);

				graphics.AddFilledPath (path);
				graphics.RenderSolid (ColorManager.TextColor);
			}
			else
			{
				rect = new Rectangle (rect.Right-rect.Height, rect.Bottom, rect.Height, rect.Height);
				rect.Deflate ((int) (rect.Height*0.35) + 0.5);

				var path = AbstractTreeTableColumn.GetSortedPath (rect, type);

				graphics.AddPath (path);
				graphics.RenderSolid (ColorManager.TextColor);
			}
		}

		private static Path GetSortedPath(Rectangle rect, SortedType type)
		{
			var path = new Path ();

			if (type == SortedType.Ascending)
			{
				path.MoveTo (rect.Center.X, rect.Bottom);
				path.LineTo (rect.Left, rect.Top);
				path.LineTo (rect.Right, rect.Top);
				path.Close ();
			}

			if (type == SortedType.Descending)
			{
				path.MoveTo (rect.Center.X, rect.Top);
				path.LineTo (rect.Left, rect.Bottom);
				path.LineTo (rect.Right, rect.Bottom);
				path.Close ();
			}

			return path;
		}

		protected void PaintText(Graphics graphics, Rectangle rect, string text)
		{
			//	Dessine un texte inclu dans un rectangle, avec un effet visible si
			//	le texte déborde (par exemple "Abracadab...").
			var font      = Font.DefaultFont;
			var fontSize  = this.FontSize;
			var color     = this.Enable ? ColorManager.TextColor : ColorManager.DisableTextColor;
			var alignment = this.RowContentAlignment;

			this.textLayout.Text            = text;
			this.textLayout.DefaultFont     = font;
			this.textLayout.DefaultFontSize = fontSize;
			this.textLayout.LayoutSize      = rect.Size;
			this.textLayout.BreakMode       = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			this.textLayout.Alignment       = alignment;

			this.textLayout.Paint (rect.BottomLeft, graphics, rect, color, GlyphPaintStyle.Normal);
		}

		protected Rectangle GetContentDeflateRectangle(Rectangle rect, bool considerSorting = false)
		{
			var m = (!considerSorting || this.sortedType == SortedType.None) ? 0.0 : rect.Height*0.5;

			if (this.RowContentAlignment == ContentAlignment.TopLeft    ||
				this.RowContentAlignment == ContentAlignment.MiddleLeft ||
				this.RowContentAlignment == ContentAlignment.BottomLeft)
			{
				rect.Deflate (this.DescriptionMargin, m, 0, 0);
			}
			else
			{
				rect.Deflate (0, this.DescriptionMargin+m, 0, 0);
			}

			return rect;
		}

		protected virtual ContentAlignment RowContentAlignment
		{
			get
			{
				return ContentAlignment.MiddleLeft;
			}
		}


		private Rectangle HeaderRect
		{
			get
			{
				return new Rectangle (0, this.ActualHeight-this.HeaderHeight, this.ActualWidth, this.HeaderHeight);
			}
		}

		private Rectangle FooterRect
		{
			get
			{
				return new Rectangle (0, 0, this.ActualWidth, this.FooterHeight);
			}
		}

		protected int DescriptionMargin
		{
			get
			{
				return (int) (this.RowHeight * 0.5);
			}
		}

		protected double FontSize
		{
			get
			{
				return this.RowHeight * 0.65;
			}
		}

		protected Rectangle GetCellsRect(int y)
		{
			int p1 = this.GetVerticalPosition (y+1);
			int p2 = this.GetVerticalPosition (y);

			p1 += this.VerticalAdjust;

			if (y > 0)
			{
				p2 += this.VerticalAdjust;
			}

			return new Rectangle (0, p1, this.ActualWidth, p2-p1);
		}

		private int GetVerticalPosition(int rank)
		{
			//	Retourne la position verticale, avec une subile répartition du reste
			//	pour que la dernière cellule touche toujours le bas.
			double dim = (this.ActualHeight - this.HeaderHeight - this.FooterHeight) / this.VisibleCellCount;
			return (int) this.ActualHeight - this.HeaderHeight - (int) (rank * dim);
		}


		protected Color GetCellColor(bool isHover, CellState cellState)
		{
			if (this.Enable == false)
			{
				return ColorManager.TreeTableBackgroundColor;
			}
			else if ((cellState & CellState.Selected) != 0)
			{
				return ColorManager.SelectionColor;
			}
			else if ((cellState & CellState.Event) != 0)
			{
				return ColorManager.GetEditSinglePropertyColor (DataAccessor.Simulation);
			}
			else if ((cellState & CellState.Error) != 0)
			{
				return ColorManager.ErrorColor;
			}
			else
			{
				if (this.DockToLeft)
				{
					return ColorManager.GetTreeTableDockToLeftBackgroundColor (isHover);
				}
				else
				{
					return ColorManager.GetBackgroundColor (isHover);
				}
			}
		}


		#region IToolTipHost Members
		public object GetToolTipCaption(Point pos)
		{
			//	N'est pas appelé normalement, car les colonnes sont "sous" un widget
			//	AbstractInteractiveLayer. Cela empêche l'appel automatique normal.
			if (this.HeaderHeight > 0 && pos.Y > this.ActualHeight-this.HeaderHeight)
			{
				return this.FullHeaderTooltip;
			}

			return null;
		}
		#endregion

		private string FullHeaderTooltip
		{
			//	Retourne le texte complet pour le tooltip d'une colonne, expliquant:
			//		ligne 1 -> la signification de la colonne
			//		ligne 2 -> le clic pour trier
			//		ligne 3 -> le drag pour déplacer
			//	Selon le contexte, chaque ligne est succeptible de ne pas exister.
			get
			{
				var list = new List<string> ();

				if (string.IsNullOrEmpty (this.HeaderTooltip))
				{
					//	Affiche le nom de la colonne, s'il n'y a pas de tooltip.
					//	Cela est bien pratique lorsque la colonne est étroite et que
					//	le nom ne s'affiche pas intégralement.
					if (!string.IsNullOrEmpty (this.HeaderDescription))
					{
						list.Add (this.HeaderDescription);
					}
				}
				else
				{
					list.Add (this.HeaderTooltip);
				}

				if (this.AllowsSorting)
				{
					list.Add (Res.Strings.AbstractTreeTableColumn.Sort.Tooltip.ToString ());
				}

				if (this.AllowsMovement)
				{
					list.Add (Res.Strings.AbstractTreeTableColumn.Move.Tooltip.ToString ());
				}

				//	Termine les lignes par un point, mais seulement s'il y en a plusieurs.
				if (list.Count > 1)
				{
					for (int i=0; i<list.Count; i++)
					{
						list[i] += ".";
					}
				}

				return string.Join ("<br/>", list);
			}
		}


		#region Events handler
		protected void OnChildrenMouseMove(int row)
		{
			this.ChildrenMouseMove.Raise (this, row);
		}

		public event EventHandler<int> ChildrenMouseMove;
		#endregion


		private readonly TextLayout				textLayout;

		protected IEnumerable<AbstractTreeTableCell> cells;
		private TreeTableHoverMode				hoverMode;
		protected int							hilitedHoverRow;
		private SortedType						sortedType;
		private bool							sortedPrimary;
	}
}
