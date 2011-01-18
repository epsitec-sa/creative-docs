//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Widgets
{
	public class TileArrow
	{
		public TileArrow()
		{
			this.surfaceColors   = new List<Color> ();
			this.outlineColors   = new List<Color> ();
			this.thicknessColors = new List<Color> ();
		}


		public void SetSurfaceColors(IEnumerable<Color> colors)
		{
			this.surfaceColors.Clear ();

			if (colors != null)
			{
				this.surfaceColors.AddRange (colors);
			}
		}

		public void SetOutlineColors(IEnumerable<Color> colors)
		{
			this.outlineColors.Clear ();

			if (colors != null)
			{
				this.outlineColors.AddRange (colors);
			}
		}

		public void SetThicknessColors(IEnumerable<Color> colors)
		{
			this.thicknessColors.Clear ();

			if (colors != null)
			{
				this.thicknessColors.AddRange (colors);
			}
		}


		public bool MouseHilite
		{
			get;
			set;
		}


		/// <summary>
		/// Marge supplémentaire nécessaire pour la flèche. Le côté dépend de ArrowLocation.
		/// </summary>
		/// <value>Epaisseur de la flèche.</value>
		public static double Breadth
		{
			get
			{
				return 8;
			}
		}

		/// <summary>
		/// Epaisseur de la flèche.
		/// </summary>
		/// <value>Epaisseur de la flèche.</value>
		private static double Thickness
		{
			get
			{
				return 8;
			}
		}
		
		
		public void Paint(Graphics graphics, Rectangle bounds, TileArrowMode mode, Direction direction)
		{
			switch (mode)
			{
				case TileArrowMode.None:
					this.PaintArrow (graphics, bounds, deflate => TileArrow.GetInactiveArrowPath (bounds, direction, deflate));
					break;

				case TileArrowMode.Visible:
					this.PaintArrow (graphics, bounds, deflate => TileArrow.GetArrowPath (bounds, direction, deflate));
					break;

				case TileArrowMode.Hilited:
					this.PaintArrow (graphics, bounds, deflate => TileArrow.GetHilitePath (bounds, direction, deflate));
					break;

				default:
					throw new System.NotImplementedException (string.Format ("Support for TileArrowMode.{0} not implemented", mode));
			}
		}

		private void PaintArrow(Graphics graphics, Rectangle bounds, System.Func<double, Path> pathProvider)
		{
			if (this.surfaceColors.Count > 0 && this.surfaceColors[0].IsValid)
			{
				using (Path path = pathProvider (0.5))
				{
					graphics.Rasterizer.AddSurface (path);

					if (this.surfaceColors.Count == 1)
					{
						graphics.RenderSolid (this.surfaceColors[0]);
					}
					else
					{
						if (this.MouseHilite)
						{
							Painter.PaintCircleGradient (graphics, bounds, this.surfaceColors[1], this.surfaceColors[0]);
						}
						else
						{
							Painter.PaintLeftToRightGradient (graphics, bounds, this.surfaceColors[0], this.surfaceColors[1]);
						}
					}
				}
			}

			if (this.thicknessColors.Count > 0 && this.thicknessColors[0].IsValid)
			{
				using (Path path = pathProvider (2.0))
				{
					graphics.Rasterizer.AddOutline (path, 3.0);
					graphics.RenderSolid (this.thicknessColors[0]);
				}
			}

			if (this.outlineColors.Count > 0 && this.outlineColors[0].IsValid)
			{
				using (Path path = pathProvider (0.0))
				{
					graphics.Rasterizer.AddOutline (path, 1.0);
					graphics.RenderSolid (this.outlineColors[0]);
				}
			}
		}

		
		private static Path GetArrowPath(Rectangle bounds, Direction arrowLocation, double deflate)
		{
			bounds.Deflate (deflate);

			return TileArrow.GetArrowPath (bounds, arrowLocation);
		}

		private static Path GetInactiveArrowPath(Rectangle bounds, Direction arrowLocation, double deflate)
		{
			Rectangle box;
			Point pick1, pick2, pick3;
			TileArrow.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick1, out pick2, out pick3);

			Path path = new Path ();
			path.AppendRectangle (box);
			return path;
		}

		private static Path GetArrowPath(Rectangle bounds, Direction arrowLocation)
		{
			Path path = new Path ();

			Rectangle box;
			Point pick1, pick2, pick3;
			TileArrow.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick1, out pick2, out pick3);

			switch (arrowLocation)
			{
				case Direction.Left:
					path.MoveTo (pick1);
					path.LineTo (pick2);
					path.LineTo (pick3);
					path.LineTo (box.BottomLeft);
					path.LineTo (box.BottomRight);
					path.LineTo (box.TopRight);
					path.LineTo (box.TopLeft);
					path.Close ();
					break;

				case Direction.Right:
					path.MoveTo (pick1);
					path.LineTo (pick2);
					path.LineTo (pick3);
					path.LineTo (box.TopRight);
					path.LineTo (box.TopLeft);
					path.LineTo (box.BottomLeft);
					path.LineTo (box.BottomRight);
					path.Close ();
					break;

				case Direction.Up:
					path.MoveTo (pick1);
					path.LineTo (pick2);
					path.LineTo (pick3);
					path.LineTo (box.TopLeft);
					path.LineTo (box.BottomLeft);
					path.LineTo (box.BottomRight);
					path.LineTo (box.TopRight);
					path.Close ();
					break;

				case Direction.Down:
					path.MoveTo (pick1);
					path.LineTo (pick2);
					path.LineTo (pick3);
					path.LineTo (box.BottomRight);
					path.LineTo (box.TopRight);
					path.LineTo (box.TopLeft);
					path.LineTo (box.BottomLeft);
					path.Close ();
					break;
			}

			return path;
		}

		private static Path GetHilitePath(Rectangle bounds, Direction arrowLocation, double deflate)
		{
			Rectangle box;
			Point pick1, pick2, pick3;
			TileArrow.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick1, out pick2, out pick3);

			Path path = new Path ();
			path.AppendRoundedRectangle (box, 2.0);
			return path;
		}

		private static void ComputeArrowGeometry(Rectangle bounds, Direction arrowLocation, out Rectangle box, out Point pick1, out Point pick2, out Point pick3)
		{
			//	Les points pick1..3 sont dans le sens CCW.
			Point pick;
			double thickness;

			bounds.Deflate (0.5);

			switch (arrowLocation)
			{
				case Direction.Left:
					box = new Rectangle (bounds.Left+TileArrow.Breadth, bounds.Bottom, bounds.Width-TileArrow.Breadth, bounds.Height);
					pick2 = Point.Scale (bounds.TopLeft, bounds.BottomLeft, 0.5);

					pick = Point.Scale (box.TopLeft, box.BottomLeft, 0.5);
					thickness = System.Math.Min (TileArrow.Thickness, box.Height/2);
					pick1 = Point.Move (pick, box.TopLeft, thickness);
					pick3 = Point.Move (pick, box.BottomLeft, thickness);
					break;

				default:
				case Direction.Right:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width-TileArrow.Breadth, bounds.Height);
					pick2 = Point.Scale (bounds.TopRight, bounds.BottomRight, 0.5);
					
					pick = Point.Scale (box.TopRight, box.BottomRight, 0.5);
					thickness = System.Math.Min (TileArrow.Thickness, box.Height/2);
					pick1 = Point.Move (pick, box.BottomRight, thickness);
					pick3 = Point.Move (pick, box.TopRight, thickness);
					break;

				case Direction.Up:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-TileArrow.Breadth);
					pick2 = Point.Scale (bounds.TopLeft, bounds.TopRight, 0.5);
					
					pick = Point.Scale (box.TopLeft, box.TopRight, 0.5);
					thickness = System.Math.Min (TileArrow.Thickness, box.Width/2);
					pick1 = Point.Move (pick, box.TopRight, thickness);
					pick3 = Point.Move (pick, box.TopLeft, thickness);
					break;

				case Direction.Down:
					box = new Rectangle (bounds.Left, bounds.Bottom+TileArrow.Breadth, bounds.Width, bounds.Height-TileArrow.Breadth);
					pick2 = Point.Scale (bounds.BottomLeft, bounds.BottomRight, 0.5);
					
					pick = Point.Scale (box.BottomLeft, box.BottomRight, 0.5);
					thickness = System.Math.Min (TileArrow.Thickness, box.Width/2);
					pick1 = Point.Move (pick, box.BottomLeft, thickness);
					pick3 = Point.Move (pick, box.BottomRight, thickness);
					break;
			}
		}


		private List<Color> surfaceColors;
		private List<Color> outlineColors;
		private List<Color> thicknessColors;
	}
}
