//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Surcouche interactive pour TreeTable s'occupant du déplacement d'une colonne.
	/// Ceci permet de modifier l'ordre des colonnes.
	/// </summary>
	public class InteractiveLayerOrder : AbstractInteractiveLayer
	{
		public InteractiveLayerOrder(TreeTable treeTable)
			: base (treeTable)
		{
			this.lastColumnRank = -1;
		}


		public override void BeginDrag(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.lastColumnRank != -1)
			{
				this.isDragging = true;

				this.dragInitialMouse = pos.X;
				this.dragInitialRect = this.GetColumnRect (this.lastColumnRank);
				this.dragDstRank = -1;
			}
		}

		public override void ProcessDrag(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.isDragging)
			{
				var delta = pos.X - this.dragInitialMouse;
				delta += this.dragInitialMouse - this.dragInitialRect.Center.X;
				var rect = Rectangle.Offset (this.dragInitialRect, delta, 0);

				this.dragDstRank = this.DetectColumnDst (pos);

				if (this.dragDstRank == this.lastColumnRank ||  // aucun sens si drag juste avant
					this.dragDstRank == this.lastColumnRank+1)  // aucun sens si drag juste après
				{
					this.dragDstRank = -1;
				}

				var x = this.GetSeparatorX (this.dragDstRank);
				this.UpdateForeground (this.dragInitialRect, rect, x, this.dragInitialRect.Width);
			}
			else
			{
				int rank = this.DetectColumnSrc (pos);

				if (rank != this.lastColumnRank)
				{
					this.lastColumnRank = rank;

					if (this.lastColumnRank != -1)
					{
						var rect = this.GetColumnRect (this.lastColumnRank);
						this.UpdateForeground (rect);
					}
					else
					{
						this.ClearForeground ();
					}
				}
			}
		}

		public override void EndDrag(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.isDragging)
			{
				this.isDragging = false;
				this.ClearForeground ();
			}
		}


		private int DetectColumnDst(Point pos)
		{
			if (pos.Y >= 0 && pos.Y < this.foreground.ActualHeight)
			{
				double x1 = 0;

				for (int i=0; i<this.ColumnCount; i++)
				{
					double? x = this.GetSeparatorX (i+1);  // une frontière droite

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

		private int DetectColumnSrc(Point pos)
		{
			for (int i=0; i<this.ColumnCount; i++)
			{
				var rect = this.GetColumnRect (i);

				if (rect.Contains (pos))
				{
					return i;
				}
			}

			return -1;
		}

		private Rectangle GetColumnRect(int rank)
		{
			double x1 = 0;
			double x2 = 0;

			if (rank != -1)
			{
				var column = this.GetColumn (rank);
				
				if (column.DockToLeft)
				{
					x1 = column.ActualBounds.Left;
					x2 = column.ActualBounds.Right;
				}
				else
				{
					double start  = this.ColumnsContainer.ActualBounds.Left;
					double offset = this.ColumnsContainer.ViewportOffsetX;

					x1 = start - offset + column.ActualBounds.Left;
					x2 = start - offset + column.ActualBounds.Right;

					x1 = System.Math.Max (x1, start);
					x2 = System.Math.Max (x2, start);
				}
			}

			if (x1 < x2)
			{
				return new Rectangle (x1, this.foreground.ActualHeight-this.HeaderHeight, x2-x1, this.HeaderHeight);
			}
			else
			{
				return Rectangle.Empty;
			}
		}


		private void UpdateForeground(Rectangle src, Rectangle dst, double? dstX, double dstWidth)
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
				var rect = this.GetSeparatorRect (dstX.Value, (int) (dstWidth/2));
				var mr = new Rectangle (dstX.Value, rect.Bottom, 1, rect.Height);

				rect.Deflate (0, 0, this.HeaderHeight, 0);
				this.foreground.AddZone (rect, Color.FromAlphaRgb (0.9, 0.9, 0.9, 0.9));
				this.foreground.AddZone (mr, ColorManager.HoverColor);
			}

			this.foreground.Invalidate ();
		}

		private void UpdateForeground(Rectangle rect)
		{
			this.foreground.ClearZones ();

			if (rect.IsValid)
			{
				var color = Color.FromAlphaColor (0.4, ColorManager.MoveColumnColor);
				this.foreground.AddZone (rect, color);
			}

			this.foreground.Invalidate ();
		}


		private int								lastColumnRank;
		private double							dragInitialMouse;
		private Rectangle						dragInitialRect;
		private int								dragDstRank;
	}
}