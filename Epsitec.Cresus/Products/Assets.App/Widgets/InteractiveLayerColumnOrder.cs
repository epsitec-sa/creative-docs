//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Surcouche interactive pour TreeTable s'occupant du déplacement d'une colonne.
	/// Ceci permet de modifier l'ordre des colonnes.
	/// </summary>
	public class InteractiveLayerColumnOrder : AbstractInteractiveLayer
	{
		public InteractiveLayerColumnOrder(TreeTable treeTable)
			: base (treeTable)
		{
			this.detectedColumnRank = -1;
		}


		public override void MouseDown(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.detectedColumnRank != -1)
			{
				this.isDragging = true;

				this.dragInitialMouse = pos.X;
				this.dragInitialRect = this.GetColumnRect (this.detectedColumnRank);
				this.dragDstRank = TreeTableColumnSeparator.Invalid;
			}
		}

		public override void MouseMove(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.isDragging)
			{
				var delta = pos.X - this.dragInitialMouse;
				delta += this.dragInitialMouse - this.dragInitialRect.Center.X;
				var rect = Rectangle.Offset (this.dragInitialRect, delta, 0);

				this.dragDstRank = this.DetectColumnDst (pos);

				bool dockToLeft = this.detectedColumnRank < this.DockToLeftCount;
				if (!this.dragDstRank.IsValidDrag (this.detectedColumnRank, dockToLeft))
				{
					this.dragDstRank = TreeTableColumnSeparator.Invalid;
				}

				this.UpdateForeground (this.dragInitialRect, rect, this.dragDstRank, this.dragInitialRect.Width);
			}
			else
			{
				int rank = this.DetectColumnSrc (pos);
				this.SetActiveHover (rank);
			}
		}

		public override void MouseUp(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.isDragging)
			{
				this.isDragging = false;
				this.ClearForeground ();

				if (this.dragDstRank.IsValid)
				{
					this.ChangeColumnOrder (this.detectedColumnRank, this.dragDstRank);
				}
			}
		}


		public override bool HasActiveHover
		{
			get
			{
				return this.detectedColumnRank != -1;
			}
		}

		public override void ClearActiveHover()
		{
			this.SetActiveHover (-1);
		}

		private void SetActiveHover(int rank)
		{
			if (rank != this.detectedColumnRank)
			{
				this.detectedColumnRank = rank;

				if (this.detectedColumnRank != -1)
				{
					var rect = this.GetColumnRect (this.detectedColumnRank);
					this.UpdateForeground (rect);
				}
				else
				{
					this.ClearForeground ();
				}
			}
		}


		private TreeTableColumnSeparator DetectColumnDst(Point pos)
		{
			if (pos.Y >= 0 && pos.Y < this.foreground.ActualHeight)
			{
				for (int i=0; i<this.ColumnCount; i++)
				{
					double? x1 = this.GetSeparatorX (i+0);  // frontière gauche
					double? x2 = this.GetSeparatorX (i+1);  // frontière droite

					if (x1.HasValue && x2.HasValue)
					{
						var xm = (x1+x2)/2;

						if (pos.X >= x1 && pos.X < xm)
						{
							if (i == 0 && this.DockToLeftCount == 0)
							{
								//	Cas particulier où le conteneur de gauche est vide et où on
								//	cherche à y revenir.
								if (pos.X < 10)
								{
									//	On va revenir dans le conteneur de gauche.
									return new TreeTableColumnSeparator (0, -1);
								}
								else
								{
									//	On va tout à gauche du conteneur scrollable.
									return new TreeTableColumnSeparator (0);
								}
							}

							if (i == this.DockToLeftCount)  // sur la frontière left|scrollable ?
							{
								//	On est à droite de la frontière left|scrollable.
								return new TreeTableColumnSeparator (i, 1);
							}
							else
							{
								return new TreeTableColumnSeparator (i);
							}
						}

						if (pos.X >= xm && pos.X < x2)
						{
							if (i+1 == this.DockToLeftCount)  // sur la frontière left|scrollable ?
							{
								//	On est à gauche de la frontière left|scrollable.
								return new TreeTableColumnSeparator (i+1, -1);
							}
							else
							{
								return new TreeTableColumnSeparator (i+1);
							}
						}
					}
				}

				return new TreeTableColumnSeparator (this.ColumnCount);
			}

			return TreeTableColumnSeparator.Invalid;
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


		private void UpdateForeground(Rectangle src, Rectangle headerDst, TreeTableColumnSeparator dst, double dstWidth)
		{
			this.foreground.ClearZones ();

			if (src.IsValid)
			{
				//	La colonne source est fortement estompée, pour donner l'illusion
				//	qu'elle a disparu.
				src = new Rectangle (src.Left, 0, src.Width, this.foreground.ActualHeight);
				this.foreground.AddSurface (src, Color.FromAlphaRgb (0.8, 1.0, 1.0, 1.0));
			}

			if (headerDst.IsValid)
			{
				//	L'en-tête destination est dessinée pour ressembler au maximum
				//	à une en-tête normale.
				var color = Color.FromAlphaColor (0.8, ColorManager.TreeTableBackgroundColor);
				this.foreground.AddSurface (headerDst, color);

				//	On dessine un rectangle plus foncé autour.
				headerDst.Deflate (0.5);
				color = ColorManager.TreeTableBackgroundColor.Delta (-0.3);
				this.foreground.AddOutline (headerDst, color);
			}

			if (dst.IsValid)
			{
				var x = this.GetSeparatorX (dst.Rank);

				if (x.HasValue)
				{
					if (dst.Rank == 0 && this.DockToLeftCount == 0)
					{
						//	Cas particulier où le conteneur de gauche est vide et où on
						//	cherche à y revenir.
						x += 5;
					}

					if (dst.Left)
					{
						x -= 5;
					}

					if (dst.Right)
					{
						x += 5;
					}

					//	Ligne traitillée centrale.
					var line = new DashedPath ();
					line.AddDash (2, 4);
					line.MoveTo (x.Value+0.5, this.foreground.ActualHeight - this.HeaderHeight - 20);
					line.LineTo (x.Value+0.5, 0);
					var dash = line.GenerateDashedPath ();

					this.foreground.AddOutline (dash, ColorManager.TextColor);

					//	Flèche vers le bas.
					var path = this.ArrowDown (x.Value);
					this.foreground.AddSurface (path, ColorManager.HoverColor);
				}
			}

			this.foreground.Invalidate ();
		}

		public Path ArrowDown(double x)
		{
			var path = new Path ();

			var y1 = this.foreground.ActualHeight;
			var y2 = this.foreground.ActualHeight - this.HeaderHeight;
			var y3 = this.foreground.ActualHeight - this.HeaderHeight - 20;

			path.MoveTo (x,    y3);
			path.LineTo (x-20, y2);
			path.LineTo (x- 8, y2);
			path.LineTo (x- 8, y1);
			path.LineTo (x+10, y1);
			path.LineTo (x+10, y2);
			path.LineTo (x+20, y2);
			path.Close ();

			return path;
		}

		private void UpdateForeground(Rectangle rect)
		{
			this.foreground.ClearZones ();

			if (rect.IsValid)
			{
				var color = Color.FromAlphaColor (0.4, ColorManager.MoveColumnColor);
				this.foreground.AddSurface (rect, color);
			}

			this.foreground.Invalidate ();
		}


		private int								detectedColumnRank;
		private double							dragInitialMouse;
		private Rectangle						dragInitialRect;
		private TreeTableColumnSeparator		dragDstRank;
	}
}