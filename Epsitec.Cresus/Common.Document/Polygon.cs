using System.Collections.Generic;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	public class Polygon
	{
		/// <summary>
		/// Un polygone est une forme ferm�e constitu�e de segments de droites, simplement d�finis
		/// par une liste de points. Un polygone est obligatoirement ferm�.
		/// Pour obtenir une forme plus complexe (par exemple une forme trou�e), il faut utiliser
		/// une liste de polygones.
		/// </summary>
		public Polygon()
		{
			this.points = new List<Point> ();
		}

		public int Count
		{
			get
			{
				return this.points.Count;
			}
		}

		public Point GetPoint(int index)
		{
			//	Retourne un point � partir d'un index donn�.
			if (index < 0 || index > this.points.Count-1)
			{
				return Point.Zero;
			}

			return this.points[index];
		}

		public Point GetCyclingPoint(int cyclingIndex)
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

		public void Add(Point p)
		{
			//	Ajoute un point au polygone, sauf s'il est identique au dernier ajout�.
			if (this.points.Count == 0 || this.points[this.points.Count-1] != p)
			{
				this.points.Add (p);
			}
		}

		public Point Center
		{
			//	Retourne le cdg d'un polygone.
			get
			{
				double px = 0;
				double py = 0;

				foreach (var point in this.points)
				{
					px += point.X;
					py += point.Y;
				}

				return new Point (px/this.points.Count, py/this.points.Count);
			}
		}


		#region Polygons to Path
		public static Path GetPolygonPathCorner(DrawingContext drawingContext, List<Polygon> polygons, Properties.Corner corner, bool simplify)
		{
			//	Cr�e le chemin de plusieurs polygones en injectant des coins quelconques.
			if (corner == null || corner.CornerType == Properties.CornerType.None)
			{
				return Polygon.GetPolygonPath (polygons);
			}
			else
			{
				Path path = new Path ();
				double min = double.MaxValue;

				foreach (var polygon in polygons)
				{
					for (int i = 0; i < polygon.points.Count; i++)
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

						for (int i = 0; i < polygon.points.Count; i++)
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

		public Path PolygonPath
		{
			//	Cr�e le chemin d'un polygone � coins droits.
			get
			{
				var path = new Path ();
				Polygon.AddPolygonPath (path, this);
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
			for (int i = 0; i < polygon.points.Count; i++)
			{
				if (i == 0)
				{
					path.MoveTo (polygon.points[i]);
				}
				else
				{
					path.LineTo (polygon.points[i]);
				}
			}

			path.Close ();
		}
		#endregion


		#region Polygons geometry
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

		public Polygon Move(double mx, double my)
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

				for (int i = 0; i < this.points.Count; i++)
				{
					Point p = this.points[i];

					pp.points.Add (p+move);
				}

				return pp;
			}
		}


		public Polygon Inflate(double inflate)
		{
			//	Engraisse/d�graisse un polygone.
			if (inflate == 0)
			{
				return this;
			}
			else
			{
				//	D�termine s'il faut mettre les points � l'int�rieur ou � l'ext�rieur.
				bool ccw = false;
				{
					Point a = this.GetCyclingPoint (-1);  // point pr�c�dent
					Point p = this.GetCyclingPoint (0);   // point courant
					Point b = this.GetCyclingPoint (1);   // point suivant

					Point c = Polygon.InflateCorner (a, p, b, inflate, ccw);  // calcule un point int�rieur/ext�rieur au hasard
					if (this.IsInside (c))  // point obtenu � l'int�rieur du polygone ?
					{
						ccw = true;  // on inverse la m�thode
					}
				}

				if (inflate < 0)  // d�graisse ?
				{
					ccw = !ccw;  // on inverse la m�thode
				}

				return this.Inflate (inflate, ccw);
			}
		}

		public static List<Polygon> Inflate(List<Polygon> polygons, double inflate)
		{
			//	Engraisse/d�graisse des polygones.
			//	Cette proc�dure ne fonctionne que dans des cas simples, sans d�g�n�rescence.
			//	Les polygones obtenus ont toujours le m�me nombre de sommets.
			//	D�s que l'engraissement produit des parties qui se touchent, le r�sultat est �trange.
			//	Idem d�s que le d�graissement produit des parties vides.
			//	TODO: Am�liorer...
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

						Point c = Polygon.InflateCorner (a, p, b, inflate, ccw);  // calcule un point int�rieur/ext�rieur au hasard
						if (Polygon.IsInside (polygons, c))  // point obtenu � l'int�rieur du polygone ?
						{
							ccw = true;  // on inverse la m�thode
						}
					}

					if (inflate < 0)  // d�graisse ?
					{
						ccw = !ccw;  // on inverse la m�thode
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

				for (int i = 0; i < this.points.Count; i++)
				{
					Point a = this.GetCyclingPoint (i-1);  // point pr�c�dent
					Point p = this.GetCyclingPoint (i);    // point courant
					Point b = this.GetCyclingPoint (i+1);  // point suivant

					Point c = Polygon.InflateCorner (a, p, b, inflate, ccw);
					pp.points.Add (c);
				}

				return pp;
			}
		}

		private static Point InflateCorner(Point a, Point p, Point b, double inflate, bool ccw)
		{
			//	Engraisse/d�graisse un coin 'a-p-b'.
			if (inflate == 0)
			{
				return p;
			}
			else
			{
				if (a == b)  // cas d�g�n�r� ?
				{
					return Point.Move (p, a, -inflate);
				}
				else
				{
					var pa = Point.Move (p, Polygon.RotateCW (p, a, !ccw), inflate);
					var aa = Point.Move (a, Polygon.RotateCW (a, p,  ccw), inflate);
					var pb = Point.Move (p, Polygon.RotateCW (p, b,  ccw), inflate);
					var bb = Point.Move (b, Polygon.RotateCW (b, p, !ccw), inflate);

					Point[] i = Geometry.Intersect (pa, aa, pb, bb);

					if (i != null && i.Length == 1)
					{
						var pp = i[0];  // p' <-- intersection

						//	Garde-fou (un peu comme MiterLimit en PostScript), si l'intersection gicle trop loin !
						//	Sauf qu'ici, on ne peut donner qu'un seul point pour l'extr�mit�.
						double d = Point.Distance (p, pp);
						if (d > System.Math.Abs (inflate)*Polygon.miterLimit)
						{
							aa = Point.Move (p, a, -inflate*Polygon.miterLimit);
							bb = Point.Move (p, b, -inflate*Polygon.miterLimit);
							pp = Point.Scale (aa, bb, 0.5);
						}

						return pp;
					}
					else
					{
						return p;  // sans intersection, on ne peut pas faire mieux que de redonner p !
					}
				}
			}
		}

		private static Point RotateCW(Point center, Point a, bool ccw)
		{
			//	Retourne le point 'a' tourn� de +/-90 degr�s autour de 'center'.
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


		private bool IsInside(Point p)
		{
			var surface = new InsideSurface (p, this.points.Count);

			for (int i = 0; i < this.points.Count; i++)
			{
				surface.AddLine (this.GetCyclingPoint (i), this.GetCyclingPoint (i+1));
			}

			return surface.IsInside ();
		}

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
				for (int i = 0; i < polygon.points.Count; i++)
				{
					surface.AddLine (polygon.GetCyclingPoint (i), polygon.GetCyclingPoint (i+1));
				}
			}

			return surface.IsInside ();
		}


		private static readonly double miterLimit = 10;

		private readonly List<Point>	points;
	}
}
