using System.Collections.Generic;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	public class Polygon
	{
		public Polygon()
		{
			this.points = new List<Point> ();
		}

		public List<Point> Points
		{
			get
			{
				return this.points;
			}
		}

		public Point GetPoint(int cyclingIndex)
		{
			while (cyclingIndex < 0)
			{
				cyclingIndex += this.points.Count;
			}

			while (cyclingIndex >= this.points.Count)
			{
				cyclingIndex -= this.points.Count;
			}

			return this.points[cyclingIndex];
		}


		#region Polygon to Path
		public static Path GetPolygonPathCorner(DrawingContext drawingContext, List<Polygon> polygons, Properties.Corner corner, bool simplify)
		{
			//	Crée le chemin d'un polygone à coins quelconques.
			if (corner == null)
			{
				return Polygon.GetPolygonPath (polygons);
			}
			else
			{
				Path path = new Path ();
				double min = double.MaxValue;

				foreach (var polygon in polygons)
				{
					for (int i = 0; i < polygon.Points.Count; i++)
					{
						Point p = polygon.GetPoint (i);    // point courant
						Point b = polygon.GetPoint (i+1);  // point suivant

						double d = Point.Distance (p, b);
						min = System.Math.Min (min, d);
					}

					double radius = simplify ? 0.0 : System.Math.Min (corner.Radius, min/2);

					if (corner.CornerType == Properties.CornerType.Right || radius == 0.0)
					{
						Polygon.AddPolygonPath (path, polygon);
					}
					else
					{
						path.DefaultZoom = Properties.Abstract.DefaultZoom (drawingContext);

						for (int i = 0; i < polygon.Points.Count; i++)
						{
							Point a = polygon.GetPoint (i-1);  // point précédent
							Point p = polygon.GetPoint (i);    // point courant
							Point b = polygon.GetPoint (i+1);  // point suivant

							Point c1 = Point.Move (p, a, radius);
							Point c2 = Point.Move (p, b, radius);

							if (i == 0)
							{
								path.MoveTo (c1);
							}
							else
							{
								path.LineTo (c1);
							}

							corner.PathCorner (path, c1, p, c2, radius);
						}

						path.Close ();
					}
				}

				return path;
			}
		}

		public static Path GetPolygonPath(List<Polygon> polygons)
		{
			//	Crée le chemin d'un polygone à coins droits.
			var path = new Path ();

			foreach (var polygon in polygons)
			{
				Polygon.AddPolygonPath (path, polygon);
			}

			return path;
		}

		private static void AddPolygonPath(Path path, Polygon polygon)
		{
			for (int i = 0; i < polygon.Points.Count; i++)
			{
				if (i == 0)
				{
					path.MoveTo (polygon.Points[i]);
				}
				else
				{
					path.LineTo (polygon.Points[i]);
				}
			}

			path.Close ();
		}
		#endregion


		#region Polygon geometry
		public static List<Polygon> Move(List<Polygon> polygons, double mx, double my)
		{
			if (mx == 0 && my == 0)
			{
				return polygons;
			}
			else
			{
				var pp = new List<Polygon> ();

				foreach (var polygon in polygons)
				{
					pp.Add (Polygon.Move (polygon, mx, my));
				}

				return pp;
			}
		}

		private static Polygon Move(Polygon polygon, double mx, double my)
		{
			//	Déplace une liste de points.
			if (mx == 0 && my == 0)
			{
				return polygon;
			}
			else
			{
				var pp = new Polygon ();
				var move = new Point (mx, my);

				for (int i = 0; i < polygon.Points.Count; i++)
				{
					Point p = polygon.Points[i];

					pp.Points.Add (p+move);
				}

				return pp;
			}
		}

		public static List<Polygon> Inflate(List<Polygon> polygons, double inflate)
		{
			if (inflate == 0)
			{
				return polygons;
			}
			else
			{
				var pp = new List<Polygon> ();

				foreach (var polygon in polygons)
				{
					pp.Add (Polygon.Inflate (polygon, inflate));
				}

				return pp;
			}
		}

		private static Polygon Inflate(Polygon polygon, double inflate)
		{
			//	Engraisse/dégraisse un polygone.
			if (inflate == 0)
			{
				return polygon;
			}
			else
			{
				var pp = new Polygon ();

				for (int i = 0; i < polygon.Points.Count; i++)
				{
					Point a = polygon.GetPoint (i-1);  // point précédent
					Point p = polygon.GetPoint (i);    // point courant
					Point b = polygon.GetPoint (i+1);  // point suivant

					Point c = Polygon.InflateCorner (a, p, b, inflate);

					if (!c.IsZero)
					{
						pp.Points.Add (c);
					}
				}

				return pp;
			}
		}

		private static Point InflateCorner(Point a, Point p, Point b, double inflate)
		{
			//	Engraisse/dégraisse un coin 'apb'.
			if (inflate == 0)
			{
				return p;
			}
			else
			{
#if false
				var aa = Point.Move (p, a, -inflate);
				return Point.Move(aa, b+aa-p, -inflate);
#else
				var pa = Point.Move (p, Polygon.RotateCW (p, a), inflate);
				var aa = Point.Move (a, Polygon.RotateCCW (a, p), inflate);
				var pb = Point.Move (p, Polygon.RotateCCW (p, b), inflate);
				var bb = Point.Move (b, Polygon.RotateCW (b, p), inflate);

				Point[] i = Geometry.Intersect (pa, aa, pb, bb);

				if (i != null && i.Length == 1)
				{
					return i[0];
				}
				else
				{
					return Point.Zero;
				}
#endif
			}
		}

		private static Point RotateCW(Point center, Point a)
		{
			double dx = a.X - center.X;
			double dy = a.Y - center.Y;

			return new Point (center.X + dy, center.Y - dx);
		}

		private static Point RotateCCW(Point center, Point a)
		{
			double dx = a.X - center.X;
			double dy = a.Y - center.Y;

			return new Point (center.X - dy, center.Y + dx);
		}
		#endregion


		private readonly List<Point>	points;
	}
}
