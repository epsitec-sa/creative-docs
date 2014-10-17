//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ligne de Timeline affichant les éventuelles pastilles (glyphs).
	/// </summary>
	public class TimelineRowGlyphs : AbstractTimelineRow, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public void SetCells(TimelineCellGlyph[] cells)
		{
			this.cells = cells;
			this.Invalidate ();

			ToolTip.Default.RegisterDynamicToolTipHost (this);  // pour voir les tooltips dynamiques
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.cells == null || this.VisibleCellCount == 0)
			{
				return;
			}

			this.Paint (graphics);
		}

		private void Paint(Graphics graphics)
		{
			int x = 0;
			int index = 0;
			var lastCell = new TimelineCellGlyph ();  // cellule invalide

			for (int rank = 0; rank <= this.VisibleCellCount; rank++)
			{
				var cell = this.GetCell (rank);
				if (!TimelineRowGlyphs.IsSame (lastCell, cell) && x != rank)
				{
					var rect = this.GetCellsRect (x, rank);
					bool isHover = (this.detectedHoverRank >= x && this.detectedHoverRank < rank);

					this.PaintCellBackground (graphics, rect, lastCell, isHover, index, lastCell.Flags);

					//	Ajuste le rectangle pour avoir une hauteur constante.
					double h = this.CellWidth / this.RelativeWidth;
					var glyphRect = rect;
					glyphRect.Deflate ((rect.Height - h)/2);

					this.PaintCellForeground (graphics, glyphRect, lastCell, isHover, index);

					this.PaintGrid (graphics, rect, index, this.hilitedHoverRank, (lastCell.Flags & DataCellFlags.Group) != 0);

					index++;
					x = rank;
				}

				lastCell = cell;
			}
		}

		private void PaintCellBackground(Graphics graphics, Rectangle rect, TimelineCellGlyph cell, bool isHover, int index, DataCellFlags flags)
		{
			//	Dessine le fond.
			var color = TimelineRowGlyphs.GetCellColor (cell, isHover, index);

			if (!color.IsBackground ())
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (color);
			}

			//	S'il s'agit d'une date bloquée (avant un événement d'entrée ou après un
			//	événement de sortie), on dessine des hachures.
			//	Idem si les cellules sont antérieures à l'événement Locked (cadenas).
			if ((flags & DataCellFlags.OutOfBounds) != 0 ||
				(flags & DataCellFlags.Locked     ) != 0)
			{
				var reference = this.MapParentToClient (Point.Zero);
				PaintHatch.Paint (graphics, rect, reference, 0.3);
			}
		}

		private void PaintCellForeground(Graphics graphics, Rectangle rect, TimelineCellGlyph cell, bool isHover, int index)
		{
			if (cell.Glyphs != null)
			{
				if (cell.Glyphs.Count == 1)
				{
					PaintEventGlyph.Paint (graphics, rect, cell.Glyphs[0]);
				}
				else if (cell.Glyphs.Count == 2)
				{
					//	S'il y a 2 événements dans la cellule, on les affiche côte à côte.
					var suppl = rect.Width - rect.Height;
					var dx = suppl / (cell.Glyphs.Count-1);

					for (int i=0; i<cell.Glyphs.Count; i++)
					{
						var r = new Rectangle (rect.Left+dx*i, rect.Bottom, rect.Height, rect.Height);
						PaintEventGlyph.Paint (graphics, r, cell.Glyphs[i]);
					}
				}
				else if (cell.Glyphs.Count == 3)
				{
					//	S'il y a 3 événements dans la cellule, on les affiche côte à côte.
					var suppl = rect.Width - rect.Height;
					var dx = suppl / (cell.Glyphs.Count-1);

					for (int i=0; i<cell.Glyphs.Count; i++)
					{
						int mx = 0;
						if (i == 0)  mx = -3;  // un poil plus à gauche
						if (i == 2)  mx =  3;  // un poil plus à droite
						var r = new Rectangle (rect.Left+dx*i+mx, rect.Bottom, rect.Height, rect.Height);
						PaintEventGlyph.Paint (graphics, r, cell.Glyphs[i]);
					}
				}
				else if (cell.Glyphs.Count > 3)
				{
					//	S'il y a plus de 2 événements dans la cellule, on affiche juste le
					//	nombre d'événements.
					var text = string.Format ("... ({0})", cell.Glyphs.Count);
					graphics.Color = ColorManager.TextColor;
					graphics.PaintText (rect, text, Font.DefaultFont, this.FontSize, ContentAlignment.MiddleCenter);
				}
			}
		}


		private static bool IsSame(TimelineCellGlyph c1, TimelineCellGlyph c2)
		{
			return TimelineCellGlyph.IsSameGlyphs (c1, c2);
		}

		private static Color GetCellColor(TimelineCellGlyph cell, bool isHover, int index)
		{
			if (cell.IsValid)
			{
				if (cell.IsSelected)
				{
					return ColorManager.SelectionColor;
				}
				else if (isHover)
				{
					return ColorManager.HoverColor;
				}
				//-else if (cell.Glyph != TimelineGlyph.Empty)
				//-{
				//-	return ColorManager.EditSinglePropertyColor;
				//-}
				else
				{
					return ColorManager.GetBackgroundColor (isHover);
				}
			}
			else
			{
				return Color.Empty;
			}
		}


		private TimelineCellGlyph GetCell(int rank)
		{
			if (rank < this.VisibleCellCount)
			{
				int index = this.GetListIndex (rank);

				if (index >= 0 && index < this.cells.Length)
				{
					return this.cells[index];
				}
			}

			return new TimelineCellGlyph ();  // retourne une cellule invalide
		}

		private int GetListIndex(int rank)
		{
			if (rank >= 0 && rank < this.cells.Length)
			{
				int offset = (int) ((double) (this.cells.Length - this.VisibleCellCount) * this.pivot);
				return rank + offset;
			}
			else
			{
				return -1;
			}
		}


		#region IToolTipHost Members
		public object GetToolTipCaption(Point pos)
		{
			if (this.detectedHoverRank >= 0 && this.detectedHoverRank < this.cells.Length)
			{
				return this.cells[this.detectedHoverRank].Tooltip;
			}
			else
			{
				return null;
			}
		}
		#endregion


		private TimelineCellGlyph[] cells;
	}
}
