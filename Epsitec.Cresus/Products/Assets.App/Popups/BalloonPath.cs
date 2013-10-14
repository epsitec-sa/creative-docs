//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Cette classe, fortement inspirée de Common.Designer.EntitiesEditor.ObjectComment,
	/// calcule le chemin d'une 'bulle de bd', c'est-à-dire un rectangle avec une queue
	/// partant de l'un des 4 côtés.
	/// </summary>
	public static class BalloonPath
	{
		public static Path GetPath(Rectangle mainRect, Rectangle targetRect)
		{
			//	Retourne le chemin d'un rectangle correspondant à mainRect, avec une
			//	petite queue en direction de targetRect.
			//
			//	  mainRect    targetRect
			//	+----------+
			//	|          |   +---+
			//	|          |-->|   |
			//	|          |   +---+
			//	+----------+

			BalloonPath.mainRect   = mainRect;
			BalloonPath.targetRect = targetRect;

			var path = new Path ();

			var mode    = BalloonPath.GetAttachMode ();
			var himself = BalloonPath.GetAttachHimself (mode);
			var other   = BalloonPath.GetAttachOther (mode);

			double d = Point.Distance (himself, other);

			Rectangle bounds = BalloonPath.mainRect;

			if (mode == AttachMode.None || himself.IsZero || other.IsZero || d <= 0)
			{
				path.AppendRectangle (bounds);
			}
			else if (mode == AttachMode.Left)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y -= BalloonPath.queueThickness;
				h2.Y += BalloonPath.queueThickness;

				if (h1.Y < bounds.Bottom)
				{
					h2.Y += bounds.Bottom-h1.Y;
					h1.Y = bounds.Bottom;
				}

				if (h2.Y > bounds.Top)
				{
					h1.Y -= h2.Y-bounds.Top;
					h2.Y = bounds.Top;
				}

				path.MoveTo (other);
				path.LineTo (h1);
				path.LineTo (bounds.BottomLeft);
				path.LineTo (bounds.BottomRight);
				path.LineTo (bounds.TopRight);
				path.LineTo (bounds.TopLeft);
				path.LineTo (h2);
				path.Close ();
			}
			else if (mode == AttachMode.Right)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y -= BalloonPath.queueThickness;
				h2.Y += BalloonPath.queueThickness;

				if (h1.Y < bounds.Bottom)
				{
					h2.Y += bounds.Bottom-h1.Y;
					h1.Y = bounds.Bottom;
				}

				if (h2.Y > bounds.Top)
				{
					h1.Y -= h2.Y-bounds.Top;
					h2.Y = bounds.Top;
				}

				path.MoveTo (other);
				path.LineTo (h1);
				path.LineTo (bounds.BottomRight);
				path.LineTo (bounds.BottomLeft);
				path.LineTo (bounds.TopLeft);
				path.LineTo (bounds.TopRight);
				path.LineTo (h2);
				path.Close ();
			}
			else if (mode == AttachMode.Bottom)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.X -= BalloonPath.queueThickness;
				h2.X += BalloonPath.queueThickness;

				if (h1.X < bounds.Left)
				{
					h2.X += bounds.Left-h1.X;
					h1.X = bounds.Left;
				}

				if (h2.X > bounds.Right)
				{
					h1.X -= h2.X-bounds.Right;
					h2.X = bounds.Right;
				}

				path.MoveTo (other);
				path.LineTo (h1);
				path.LineTo (bounds.BottomLeft);
				path.LineTo (bounds.TopLeft);
				path.LineTo (bounds.TopRight);
				path.LineTo (bounds.BottomRight);
				path.LineTo (h2);
				path.Close ();
			}
			else if (mode == AttachMode.Top)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.X -= BalloonPath.queueThickness;
				h2.X += BalloonPath.queueThickness;

				if (h1.X < bounds.Left)
				{
					h2.X += bounds.Left-h1.X;
					h1.X = bounds.Left;
				}

				if (h2.X > bounds.Right)
				{
					h1.X -= h2.X-bounds.Right;
					h2.X = bounds.Right;
				}

				path.MoveTo (other);
				path.LineTo (h1);
				path.LineTo (bounds.TopLeft);
				path.LineTo (bounds.BottomLeft);
				path.LineTo (bounds.BottomRight);
				path.LineTo (bounds.TopRight);
				path.LineTo (h2);
				path.Close ();
			}
			else if (mode == AttachMode.BottomLeft)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y += BalloonPath.queueThickness*System.Math.Sqrt (2);
				h2.X += BalloonPath.queueThickness*System.Math.Sqrt (2);

				path.MoveTo (other);
				path.LineTo (h1);
				path.LineTo (bounds.TopLeft);
				path.LineTo (bounds.TopRight);
				path.LineTo (bounds.BottomRight);
				path.LineTo (h2);
				path.Close ();
			}
			else if (mode == AttachMode.BottomRight)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y += BalloonPath.queueThickness*System.Math.Sqrt (2);
				h2.X -= BalloonPath.queueThickness*System.Math.Sqrt (2);

				path.MoveTo (other);
				path.LineTo (h1);
				path.LineTo (bounds.TopRight);
				path.LineTo (bounds.TopLeft);
				path.LineTo (bounds.BottomLeft);
				path.LineTo (h2);
				path.Close ();
			}
			else if (mode == AttachMode.TopLeft)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y -= BalloonPath.queueThickness*System.Math.Sqrt (2);
				h2.X += BalloonPath.queueThickness*System.Math.Sqrt (2);

				path.MoveTo (other);
				path.LineTo (h1);
				path.LineTo (bounds.BottomLeft);
				path.LineTo (bounds.BottomRight);
				path.LineTo (bounds.TopRight);
				path.LineTo (h2);
				path.Close ();
			}
			else if (mode == AttachMode.TopRight)
			{
				Point h1 = himself;
				Point h2 = himself;

				h1.Y -= BalloonPath.queueThickness*System.Math.Sqrt (2);
				h2.X -= BalloonPath.queueThickness*System.Math.Sqrt (2);

				path.MoveTo (other);
				path.LineTo (h1);
				path.LineTo (bounds.BottomRight);
				path.LineTo (bounds.BottomLeft);
				path.LineTo (bounds.TopLeft);
				path.LineTo (h2);
				path.Close ();
			}

			return path;
		}

		private static Point GetAttachHimself(AttachMode mode)
		{
			//	Retourne le point d'attache sur le rectangle cible.
			Point pos = Point.Zero;

			if (mode != AttachMode.None)
			{
				Rectangle bounds = BalloonPath.mainRect;
				bounds.Inflate (0.5);

				if (mode == AttachMode.BottomLeft)
				{
					pos = bounds.BottomLeft;
				}

				if (mode == AttachMode.BottomRight)
				{
					pos = bounds.BottomRight;
				}

				if (mode == AttachMode.TopLeft)
				{
					pos = bounds.TopLeft;
				}

				if (mode == AttachMode.TopRight)
				{
					pos = bounds.TopRight;
				}

				if (mode == AttachMode.Left || mode == AttachMode.Right)
				{
					pos.X = (mode == AttachMode.Left) ? bounds.Left : bounds.Right;

					double miny = System.Math.Max (BalloonPath.targetRect.Bottom, bounds.Bottom);
					double maxy = System.Math.Min (BalloonPath.targetRect.Top, bounds.Top);

					if (miny <= maxy)
					{
						pos.Y = (miny+maxy)/2;
					}
					else
					{
						pos.Y = (bounds.Top < BalloonPath.targetRect.Top) ? bounds.Top : bounds.Bottom;
					}
				}

				if (mode == AttachMode.Bottom || mode == AttachMode.Top)
				{
					pos.Y = (mode == AttachMode.Bottom) ? bounds.Bottom : bounds.Top;

					double minx = System.Math.Max (BalloonPath.targetRect.Left, bounds.Left);
					double maxx = System.Math.Min (BalloonPath.targetRect.Right, bounds.Right);

					if (minx <= maxx)
					{
						pos.X = (minx+maxx)/2;
					}
					else
					{
						pos.X = (bounds.Right < BalloonPath.targetRect.Right) ? bounds.Right : bounds.Left;
					}
				}
			}

			return pos;
		}

		private static Point GetAttachOther(AttachMode mode)
		{
			//	Retourne le point d'attache sur le rectangle cible.
			var pos = Point.Zero;

			if (mode != AttachMode.None)
			{
				Rectangle bounds = BalloonPath.mainRect;
				bounds.Inflate (0.5);

				if (mode == AttachMode.BottomLeft)
				{
					return BalloonPath.targetRect.TopRight;
				}

				if (mode == AttachMode.BottomRight)
				{
					return BalloonPath.targetRect.TopLeft;
				}

				if (mode == AttachMode.TopLeft)
				{
					return BalloonPath.targetRect.BottomRight;
				}

				if (mode == AttachMode.TopRight)
				{
					return BalloonPath.targetRect.BottomLeft;
				}

				Point himself = BalloonPath.GetAttachHimself (mode);

				if (mode == AttachMode.Left || mode == AttachMode.Right)
				{
					pos.X = (mode == AttachMode.Left) ? BalloonPath.targetRect.Right : BalloonPath.targetRect.Left;

					if (himself.Y < BalloonPath.targetRect.Bottom)
					{
						pos.Y = BalloonPath.targetRect.Bottom;
					}
					else if (himself.Y > BalloonPath.targetRect.Top)
					{
						pos.Y = BalloonPath.targetRect.Top;
					}
					else
					{
						pos.Y = himself.Y;
					}
				}

				if (mode == AttachMode.Bottom || mode == AttachMode.Top)
				{
					pos.Y = (mode == AttachMode.Bottom) ? BalloonPath.targetRect.Top : BalloonPath.targetRect.Bottom;

					if (himself.X < BalloonPath.targetRect.Left)
					{
						pos.X = BalloonPath.targetRect.Left;
					}
					else if (himself.X > BalloonPath.targetRect.Right)
					{
						pos.X = BalloonPath.targetRect.Right;
					}
					else
					{
						pos.X = himself.X;
					}
				}
			}

			return pos;
		}

		private static AttachMode GetAttachMode()
		{
			//	Cherche d'où doit partir la queue du commentaire (de quel côté).
			var mainRect = BalloonPath.mainRect;

			if (!mainRect.IntersectsWith (BalloonPath.targetRect))
			{
				if (mainRect.Bottom >= BalloonPath.targetRect.Top && mainRect.Right <= BalloonPath.targetRect.Left)
				{
					return AttachMode.BottomRight;
				}

				if (mainRect.Top <= BalloonPath.targetRect.Bottom && mainRect.Right <= BalloonPath.targetRect.Left)
				{
					return AttachMode.TopRight;
				}

				if (mainRect.Bottom >= BalloonPath.targetRect.Top && mainRect.Left >= BalloonPath.targetRect.Right)
				{
					return AttachMode.BottomLeft;
				}

				if (mainRect.Top <= BalloonPath.targetRect.Bottom && mainRect.Left >= BalloonPath.targetRect.Right)
				{
					return AttachMode.TopLeft;
				}

				if (mainRect.Bottom >= BalloonPath.targetRect.Top)  // commentaire en dessus ?
				{
					return AttachMode.Bottom;
				}

				if (mainRect.Top <= BalloonPath.targetRect.Bottom)  // commentaire en dessous ?
				{
					return AttachMode.Top;
				}

				if (mainRect.Left >= BalloonPath.targetRect.Right)  // commentaire à droite ?
				{
					return AttachMode.Left;
				}

				if (mainRect.Right <= BalloonPath.targetRect.Left)  // commentaire à gauche ?
				{
					return AttachMode.Right;
				}
			}

			return AttachMode.None;
		}


		private enum AttachMode
		{
			None,
			Left,
			Right,
			Bottom,
			Top,
			BottomLeft,
			BottomRight,
			TopLeft,
			TopRight,
		}


		private static readonly double queueThickness = 10;

		private static Rectangle mainRect;
		private static Rectangle targetRect;
	}
}