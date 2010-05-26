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
		public Color SurfaceColor
		{
			get;
			set;
		}

		public Color OutlineColor
		{
			get;
			set;
		}

		public Color ThicknessColor
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
		
		
		public void Paint(Graphics graphics, Rectangle bounds, TileArrowMode mode, Direction direction)
		{
			switch (mode)
			{
				case TileArrowMode.None:
					this.PaintArrow (graphics, deflate => TileArrow.GetInactiveArrowPath (bounds, direction, deflate));
					break;

				case TileArrowMode.VisibleDirect:
					this.PaintArrow (graphics, deflate => TileArrow.GetDirectArrowPath (bounds, direction, deflate));
					break;

				case TileArrowMode.VisibleReverse:
					this.PaintArrow (graphics, deflate => TileArrow.GetReverseArrowPath (bounds, direction, deflate));
					break;

				case TileArrowMode.Hilite:
					this.PaintArrow (graphics, deflate => TileArrow.GetHilitePath (bounds, direction, deflate));
					break;

				default:
					throw new System.NotImplementedException (string.Format ("Support for TileArrowMode.{0} not implemented", mode));
			}
		}

		private void PaintArrow(Graphics graphics, System.Func<double, Path> pathProvider)
		{
			if (this.SurfaceColor.IsValid)
			{
				using (Path path = pathProvider (0.5))
				{
					graphics.Rasterizer.AddSurface (path);
					graphics.RenderSolid (this.SurfaceColor);
				}
			}
			
			if (this.ThicknessColor.IsValid)
			{
				using (Path path = pathProvider (2.0))
				{
					graphics.Rasterizer.AddOutline (path, 3.0);
					graphics.RenderSolid (this.ThicknessColor);
				}
			}
			
			if (this.OutlineColor.IsValid)
			{
				using (Path path = pathProvider (0.0))
				{
					graphics.Rasterizer.AddOutline (path, 1.0);
					graphics.RenderSolid (this.OutlineColor);
				}
			}
		}

		
		private static Path GetDirectArrowPath(Rectangle bounds, Direction arrowLocation, double deflate)
		{
			bounds.Deflate (deflate);

			return TileArrow.GetArrowPath (bounds, arrowLocation);
		}

		private static Path GetInactiveArrowPath(Rectangle bounds, Direction arrowLocation, double deflate)
		{
			Rectangle box;
			Point pick;
			TileArrow.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick);

			Path path = new Path ();
			path.AppendRectangle (box);
			return path;
		}

		private static Path GetReverseArrowPath(Rectangle bounds, Direction arrowLocation, double deflate)
		{
			double revertedarrowBody;

			switch (arrowLocation)
			{
				case Direction.Left:
					arrowLocation = Direction.Right;
					revertedarrowBody = System.Math.Floor (bounds.Width*0.25);
					bounds = new Rectangle (bounds.Left, bounds.Bottom, revertedarrowBody, bounds.Height);
					break;

				case Direction.Right:
					arrowLocation = Direction.Left;
					revertedarrowBody = System.Math.Floor (bounds.Width*0.25);
					bounds = new Rectangle (bounds.Right-revertedarrowBody, bounds.Bottom, revertedarrowBody, bounds.Height);
					break;

				case Direction.Up:
					arrowLocation = Direction.Down;
					revertedarrowBody = System.Math.Floor (bounds.Height*0.25);
					bounds = new Rectangle (bounds.Left, bounds.Top-revertedarrowBody, bounds.Width, revertedarrowBody);
					break;

				case Direction.Down:
					arrowLocation = Direction.Up;
					revertedarrowBody = System.Math.Floor (bounds.Height*0.25);
					bounds = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, revertedarrowBody);
					break;

				default:
					throw new System.NotImplementedException (string.Format ("Support for Direction.{0} not implemented", arrowLocation));
			}

			bounds.Deflate (deflate);

			return TileArrow.GetArrowPath (bounds, arrowLocation);
		}
		
		private static Path GetArrowPath(Rectangle bounds, Direction arrowLocation)
		{
			Path path = new Path ();

			Rectangle box;
			Point pick;
			TileArrow.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick);

			switch (arrowLocation)
			{
				case Direction.Left:
					path.MoveTo (pick);
					path.LineTo (box.BottomLeft);
					path.LineTo (box.BottomRight);
					path.LineTo (box.TopRight);
					path.LineTo (box.TopLeft);
					path.Close ();
					break;

				case Direction.Right:
					path.MoveTo (pick);
					path.LineTo (box.TopRight);
					path.LineTo (box.TopLeft);
					path.LineTo (box.BottomLeft);
					path.LineTo (box.BottomRight);
					path.Close ();
					break;

				case Direction.Up:
					path.MoveTo (pick);
					path.LineTo (box.TopLeft);
					path.LineTo (box.BottomLeft);
					path.LineTo (box.BottomRight);
					path.LineTo (box.TopRight);
					path.Close ();
					break;

				case Direction.Down:
					path.MoveTo (pick);
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
			Point pick;
			TileArrow.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick);

			Path path = new Path ();
			path.AppendRoundedRectangle (box, 2.0);
			return path;
		}

		private static void ComputeArrowGeometry(Rectangle bounds, Direction arrowLocation, out Rectangle box, out Point pick)
		{
			bounds.Deflate (0.5);

			switch (arrowLocation)
			{
				default:
				case Direction.Left:
					box = new Rectangle (bounds.Left+TileArrow.Breadth, bounds.Bottom, bounds.Width-TileArrow.Breadth, bounds.Height);
					pick = Point.Scale (bounds.TopLeft, bounds.BottomLeft, 0.5);
					break;

				case Direction.Right:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width-TileArrow.Breadth, bounds.Height);
					pick = Point.Scale (bounds.TopRight, bounds.BottomRight, 0.5);
					break;

				case Direction.Up:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-TileArrow.Breadth);
					pick = Point.Scale (bounds.TopLeft, bounds.TopRight, 0.5);
					break;

				case Direction.Down:
					box = new Rectangle (bounds.Left, bounds.Bottom+TileArrow.Breadth, bounds.Width, bounds.Height-TileArrow.Breadth);
					pick = Point.Scale (bounds.BottomLeft, bounds.BottomRight, 0.5);
					break;
			}
		}

	}
}
