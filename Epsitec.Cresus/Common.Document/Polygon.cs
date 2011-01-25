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


		#region Polygon to Path
		public static Path GetPolygonPathCorner(DrawingContext drawingContext, List<Polygon> polygons, Properties.Corner corner, bool simplify)
		{
			//	Cr�e le chemin de plusieurs polygones en injectant des coins quelconques.
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
						Point p = polygon.GetCyclingPoint (i);    // point courant
						Point b = polygon.GetCyclingPoint (i+1);  // point suivant

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
							Point a = polygon.GetCyclingPoint (i-1);  // point pr�c�dent
							Point p = polygon.GetCyclingPoint (i);    // point courant
							Point b = polygon.GetCyclingPoint (i+1);  // point suivant

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
			//	Cr�e le chemin de plusieurs polygones � coins droits.
			var path = new Path ();

			foreach (var polygon in polygons)
			{
				Polygon.AddPolygonPath (path, polygon);
			}

			return path;
		}

		private static void AddPolygonPath(Path path, Polygon polygon)
		{
			//	Ajoute � un chemin un polygone � coins droits.
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
			//	D�place des polygones.
			if (mx == 0 && my == 0)
			{
				return polygons;
			}
			else
			{
				var pp = new List<Polygon> ();

				foreach (var polygon in polygons)
				{
					pp.Add (polygon.Move (mx, my));
				}

				return pp;
			}
		}

		private Polygon Move(double mx, double my)
		{
			//	D�place un polygone.
			if (mx == 0 && my == 0)
			{
				return this;
			}
			else
			{
				var pp = new Polygon ();
				var move = new Point (mx, my);

				for (int i = 0; i < this.Points.Count; i++)
				{
					Point p = this.Points[i];

					pp.Points.Add (p+move);
				}

				return pp;
			}
		}


		public static List<Polygon> Inflate(List<Polygon> polygons, double inflate)
		{
			//	Engraisse/d�graisse des polygones.
			//	Cette proc�dure ne fonctionne que dans des cas simples, sans d�g�n�rescence.
			//	D�s que l'engraissement produit des parties qui se touchent, le r�sultat est �trange.
			//	Idem d�s que le d�graissement produit des parties vides.
			if (inflate == 0)
			{
				return polygons;
			}
			else
			{
				var pp = new List<Polygon> ();

				foreach (var polygon in polygons)
				{
					//	D�termine s'il faut mettre les points � l'int�rieur ou � l'ext�rieur.
					bool ccw = false;
					{
						Point a = polygon.GetCyclingPoint (-1);  // point pr�c�dent
						Point p = polygon.GetCyclingPoint (0);   // point courant
						Point b = polygon.GetCyclingPoint (1);   // point suivant

						Point c = Polygon.InflateCorner (a, p, b, inflate, ccw);
						if (Polygon.IsInside (polygons, c))
						{
							ccw = true;
						}
					}

					if (inflate < 0)
					{
						ccw = !ccw;
					}

					pp.Add (polygon.Inflate (inflate, ccw));
				}

				return pp;
			}
		}

		private Polygon Inflate(double inflate, bool ccw)
		{
			//	Engraisse/d�graisse un polygone.
			if (inflate == 0)
			{
				return this;
			}
			else
			{
				var pp = new Polygon ();

				for (int i = 0; i < this.Points.Count; i++)
				{
					Point a = this.GetCyclingPoint (i-1);  // point pr�c�dent
					Point p = this.GetCyclingPoint (i);    // point courant
					Point b = this.GetCyclingPoint (i+1);  // point suivant

					Point c = Polygon.InflateCorner (a, p, b, inflate, ccw);

					if (!c.IsZero)
					{
						pp.Points.Add (c);
					}
				}

				return pp;
			}
		}

		private static Point InflateCorner(Point a, Point p, Point b, double inflate, bool ccw, bool exact = true)
		{
			//	Engraisse/d�graisse un coin 'apb'.
			if (inflate == 0)
			{
				return p;
			}
			else
			{
				if (exact)
				{
					var pa = Point.Move (p, Polygon.RotateCW (p, a, !ccw), inflate);
					var aa = Point.Move (a, Polygon.RotateCW (a, p, ccw), inflate);
					var pb = Point.Move (p, Polygon.RotateCW (p, b, ccw), inflate);
					var bb = Point.Move (b, Polygon.RotateCW (b, p, !ccw), inflate);

					Point[] i = Geometry.Intersect (pa, aa, pb, bb);

					if (i != null && i.Length == 1)
					{
						return i[0];
					}
					else
					{
						return Point.Zero;
					}
				}
				else
				{
					var aa = Point.Move (p, a, -inflate);
					return Point.Move (aa, b+aa-p, -inflate);
				}
			}
		}

		private static Point RotateCW(Point center, Point a, bool ccw)
		{
			double dx = a.X - center.X;
			double dy = a.Y - center.Y;

			if (ccw)
			{
				return new Point (center.X - dy, center.Y + dx);
			}
			else
			{
				return new Point (center.X + dy, center.Y - dx);
			}
		}
		#endregion


		private static bool IsInside(List<Polygon> polygons, Point p)
		{
			//	Il faut faire le test sur l'ensemble des polygones, car il peut y
			//	avoir des trous.
			int count = 0;
			foreach (var polygon in polygons)
			{
				count += polygon.points.Count;
			}

			var surface = new InsideSurface (p, count);

			foreach (var polygon in polygons)
			{
				for (int i = 0; i < polygon.points.Count-1; i++)
				{
					surface.AddLine (polygon.points[i], polygon.points[i+1]);
				}
			}

			return surface.IsInside ();
		}

		private Point GetCyclingPoint(int cyclingIndex)
		{
			//	Retourne un point � partir d'un index donn� dans un 'torre'.
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


		private readonly List<Point>	points;
	}
}
