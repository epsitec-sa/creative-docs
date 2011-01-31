using System.Collections.Generic;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	public enum PointType
	{
		None,
		Primary,
		Secondary,
	}

	public class Polygon
	{
		/// <summary>
		/// Un polygone est une forme fermée constituée de segments de droites, simplement définis
		/// par une liste de points. Un polygone est obligatoirement fermé.
		/// Pour obtenir une forme plus complexe (par exemple une forme trouée), il faut utiliser
		/// une liste de polygones.
		/// </summary>
		public Polygon()
		{
			this.typedPoints = new List<TypedPoint> ();
		}

		public int Count
		{
			get
			{
				return this.typedPoints.Count;
			}
		}

		public Point GetPoint(int index)
		{
			//	Retourne un point à partir d'un index donné.
			if (index < 0 || index > this.typedPoints.Count-1)
			{
				return Point.Zero;
			}

			return this.typedPoints[index].Point;
		}

		public PointType GetPointType(int index)
		{
			//	Retourne un point à partir d'un index donné.
			if (index < 0 || index > this.typedPoints.Count-1)
			{
				return PointType.None;
			}

			return this.typedPoints[index].PointType;
		}

		public Point GetCyclingPoint(int cyclingIndex)
		{
			//	Retourne un point à partir d'un index donné dans un 'torre'.
			cyclingIndex = this.GetCyclingIndex (cyclingIndex);
			return this.typedPoints[cyclingIndex].Point;
		}

		public PointType GetCyclingPointType(int cyclingIndex)
		{
			//	Retourne un type de point à partir d'un index donné dans un 'torre'.
			cyclingIndex = this.GetCyclingIndex (cyclingIndex);
			return this.typedPoints[cyclingIndex].PointType;
		}

		private TypedPoint GetCyclingTypedPoint(int cyclingIndex)
		{
			//	Retourne un point à partir d'un index donné dans un 'torre'.
			cyclingIndex = this.GetCyclingIndex (cyclingIndex);
			return this.typedPoints[cyclingIndex];
		}

		public int GetCyclingIndex(int cyclingIndex)
		{
			//	Retourne un index à partir d'un index donné dans un 'torre'.
			while (cyclingIndex < 0)
			{
				cyclingIndex += this.typedPoints.Count;
			}

			while (cyclingIndex >= this.typedPoints.Count)
			{
				cyclingIndex -= this.typedPoints.Count;
			}

			return cyclingIndex;
		}

		public void Add(Point p, PointType pointType = PointType.Primary)
		{
			//	Ajoute un point au polygone, sauf s'il est identique au dernier ajouté.
			if (this.typedPoints.Count > 0 && pointType == PointType.Primary)
			{
				var last = this.typedPoints[this.typedPoints.Count-1];

				if (last.PointType == PointType.Primary && p == last.Point)
				{
					return;
				}
			}

			this.typedPoints.Add (new TypedPoint (p, pointType));
		}

		public Point Center
		{
			//	Retourne le cdg d'un polygone.
			get
			{
				double px = 0;
				double py = 0;

				foreach (var typedPoint in this.typedPoints)
				{
					px += typedPoint.Point.X;
					py += typedPoint.Point.Y;
				}

				return new Point (px/this.typedPoints.Count, py/this.typedPoints.Count);
			}
		}


		public static void Simplify(List<Polygon> polygons)
		{
			//	Simplifie une liste de polygones.
			foreach (var polygon in polygons)
			{
				polygon.Simplify ();
			}
		}

		public void Simplify()
		{
			//	Simplifie un polygone.
			//	Lorsqu'un polygone est généré à partir d'un chemin Path (Geometry.PathToPolygons),
			//	beaucoup d'incohérences et de redondances peuvent être rencontrées !
			int i = 0;
			while (i < this.typedPoints.Count)
			{
				if (i < this.typedPoints.Count-1)
				{
					var p0 = this.typedPoints[i];
					var p1 = this.typedPoints[i+1];

					if (p0.PointType == PointType.Primary &&
						p1.PointType == PointType.Primary &&
						Geometry.Compare (p0.Point, p1.Point))
					{
						this.typedPoints.RemoveAt (i+1);
						continue;
					}
				}

				if (i < this.typedPoints.Count-2)
				{
					var p0 = this.typedPoints[i];
					var p1 = this.typedPoints[i+1];
					var p2 = this.typedPoints[i+2];

					if (p0.PointType == PointType.Primary &&
						p1.PointType == PointType.Secondary &&
						p2.PointType == PointType.Secondary &&
						Geometry.Compare (p0.Point, p1.Point) &&
						Geometry.Compare (p0.Point, p2.Point))
					{
						this.typedPoints.RemoveAt (i+1);
						this.typedPoints.RemoveAt (i+1);
						continue;
					}
				}

				i++;
			}

			//	Si le dernier point d'un polygone est identique au premier, supprime-le.
			if (this.typedPoints.Count >= 2)
			{
				var p0 = this.typedPoints[0];
				var p1 = this.typedPoints[this.typedPoints.Count-1];

				if (p0.PointType == PointType.Primary &&
					p1.PointType == PointType.Primary &&
					Geometry.Compare (p0.Point, p1.Point))
				{
					this.typedPoints.RemoveAt (this.typedPoints.Count-1);
				}
			}
		}


		#region Polygons to Path
		public static Path GetPolygonPathCorner(DrawingContext drawingContext, List<Polygon> polygons, Properties.Corner corner, bool simplify)
		{
			//	Crée le chemin de plusieurs polygones en injectant des coins quelconques.
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
					for (int i = 0; i < polygon.typedPoints.Count; i++)
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

						if (polygon.typedPoints.Count >= 3 &&
							polygon.typedPoints.Count%3 == 0 &&
							polygon.typedPoints[1].PointType == PointType.Secondary &&
							polygon.typedPoints[2].PointType == PointType.Secondary)
						{
							for (int i = 0; i < polygon.typedPoints.Count; i+=3)
							{
								Point a = polygon.GetCyclingPoint (i-1);  // point précédent (secondaire)
								Point p = polygon.GetCyclingPoint (i);    // point courant (principal)
								Point b = polygon.GetCyclingPoint (i+1);  // point suivant (secondaire)
								Point c = polygon.GetCyclingPoint (i+2);  // point suivant (secondaire)
								Point d = polygon.GetCyclingPoint (i+3);  // point suivant (principal)

								Point c1 = Point.Move (p, a, radius);
								Point c2 = Point.Move (p, b, radius);
								Point c3 = Point.Move (d, c, radius);

								if (i == 0)
								{
									path.MoveTo (c1);
								}

								corner.PathCorner (path, c1, p, c2, radius);
								path.CurveTo (b, c, c3);
							}
						}
						else
						{
							for (int i = 0; i < polygon.typedPoints.Count; i++)
							{
								Point a = polygon.GetCyclingPoint (i-1);  // point précédent
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
						}

						path.Close ();
					}
				}

				return path;
			}
		}

		public Path PolygonPath
		{
			//	Crée le chemin d'un polygone à coins droits.
			get
			{
				var path = new Path ();
				Polygon.AddPolygonPath (path, this);
				return path;
			}
		}

		public static Path GetPolygonPath(List<Polygon> polygons)
		{
			//	Crée le chemin de plusieurs polygones à coins droits.
			var path = new Path ();

			foreach (var polygon in polygons)
			{
				Polygon.AddPolygonPath (path, polygon);
			}

			return path;
		}

		private static void AddPolygonPath(Path path, Polygon polygon)
		{
			//	Ajoute à un chemin un polygone à coins droits.
			int i = 0;
			while (i < polygon.typedPoints.Count)
			{
				var p1 = polygon.typedPoints[i++];

				if (i == 1)  // premier point ?
				{
					path.MoveTo (p1.Point);
				}
				else
				{
					if (p1.PointType == PointType.Secondary)
					{
						System.Diagnostics.Debug.Assert (i < polygon.typedPoints.Count);
						var p2 = polygon.typedPoints[i++];
						var p3 = polygon.typedPoints[(i < polygon.typedPoints.Count) ? i++ : 0];
						System.Diagnostics.Debug.Assert (p2.PointType == PointType.Secondary);
						System.Diagnostics.Debug.Assert (p3.PointType == PointType.Primary);

						path.CurveTo (p1.Point, p2.Point, p3.Point);
					}
					else
					{
						path.LineTo (p1.Point);
					}
				}
			}

			path.Close ();
		}
		#endregion


		#region Polygons geometry
		public static List<Polygon> Move(List<Polygon> polygons, double mx, double my)
		{
			//	Déplace des polygones.
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
			//	Déplace un polygone.
			if (mx == 0 && my == 0)
			{
				return this;
			}
			else
			{
				var pp = new Polygon ();
				var move = new Point (mx, my);

				for (int i = 0; i < this.typedPoints.Count; i++)
				{
					var p = this.typedPoints[i];

					pp.typedPoints.Add (new TypedPoint (p.Point+move, p.PointType));
				}

				return pp;
			}
		}


		public Polygon InflateAndConcave(double inflate, double concave)
		{
			//	Engraisse/dégraisse un polygone.
			if (inflate == 0 && concave == 0)
			{
				return this;
			}
			else
			{
				//	Détermine s'il faut mettre les points à l'intérieur ou à l'extérieur.
				bool ccw = false;
				{
					Point a = this.GetCyclingPoint (-1);  // point précédent
					Point p = this.GetCyclingPoint (0);   // point courant
					Point b = this.GetCyclingPoint (1);   // point suivant

					Point c = Polygon.InflateCorner (a, p, b, inflate, ccw);  // calcule un point intérieur/extérieur au hasard
					if (this.IsInside (c))  // point obtenu à l'intérieur du polygone ?
					{
						ccw = true;  // on inverse la méthode
					}
				}

				if (inflate < 0)  // dégraisse ?
				{
					ccw = !ccw;  // on inverse la méthode
				}

				return this.InflateAndConcave (inflate, concave, ccw);
			}
		}

		public static List<Polygon> InflateAndConcave(List<Polygon> polygons, double inflate, double concave)
		{
			//	Engraisse/dégraisse des polygones.
			//	Cette procédure ne fonctionne que dans des cas simples, sans dégénérescence.
			//	Les polygones obtenus ont toujours le même nombre de sommets.
			//	Dès que l'engraissement produit des parties qui se touchent, le résultat est étrange.
			//	Idem dès que le dégraissement produit des parties vides.
			//	TODO: Améliorer...
			if (inflate == 0 && concave == 0)
			{
				return polygons;
			}
			else
			{
				var pp = new List<Polygon> ();

				foreach (var polygon in polygons)
				{
					//	Détermine s'il faut mettre les points à l'intérieur ou à l'extérieur.
					bool ccw = false;
					{
						Point a = polygon.GetCyclingPoint (-1);  // point précédent
						Point p = polygon.GetCyclingPoint (0);   // point courant
						Point b = polygon.GetCyclingPoint (1);   // point suivant

						Point c = Polygon.InflateCorner (a, p, b, inflate, ccw);  // calcule un point intérieur/extérieur au hasard
						if (Polygon.IsInside (polygons, c))  // point obtenu à l'intérieur du polygone ?
						{
							ccw = true;  // on inverse la méthode
						}
					}

					if (inflate < 0)  // dégraisse ?
					{
						ccw = !ccw;  // on inverse la méthode
					}

					pp.Add (polygon.InflateAndConcave (inflate, concave, ccw));
				}

				return pp;
			}
		}

		private Polygon InflateAndConcave(double inflate, double concave, bool ccw)
		{
			var pp = this.Inflate (inflate, ccw);
			return pp.Concave (concave, ccw);
		}

		private Polygon Inflate(double inflate, bool ccw)
		{
			//	Engraisse/dégraisse un polygone.
			if (inflate == 0)
			{
				return this;
			}
			else
			{
				var pp = new Polygon ();

				for (int i = 0; i < this.typedPoints.Count; i++)
				{
					Point a = this.GetCyclingPoint (i-1);  // point précédent
					Point p = this.GetCyclingPoint (i);    // point courant
					Point b = this.GetCyclingPoint (i+1);  // point suivant

					Point c = Polygon.InflateCorner (a, p, b, inflate, ccw);
					pp.typedPoints.Add (new TypedPoint (c, this.typedPoints[i].PointType));
				}

				return pp;
			}
		}

		private static Point InflateCorner(Point a, Point p, Point b, double inflate, bool ccw)
		{
			//	Engraisse/dégraisse un coin 'a-p-b'.
			if (inflate == 0)
			{
				return p;
			}
			else
			{
				if (a == b)  // cas dégénéré ?
				{
					return Point.Move (p, a, -inflate);
				}
				else
				{
					var pa = Point.Move (p, Polygon.RotateCW (p, a, !ccw), inflate);
					var aa = Point.Move (a, Polygon.RotateCW (a, p,  ccw), inflate);
					var pb = Point.Move (p, Polygon.RotateCW (p, b,  ccw), inflate);
					var bb = Point.Move (b, Polygon.RotateCW (b, p, !ccw), inflate);

					if (Geometry.Compare (pa, pb))
					{
						return pa;
					}
					else
					{
						Point[] i = Geometry.Intersect (pa, aa, pb, bb);

						if (i != null && i.Length == 1)
						{
							var pp = i[0];  // p' <-- intersection

							//	Garde-fou (un peu comme MiterLimit en PostScript), si l'intersection gicle trop loin !
							//	Sauf qu'ici, on ne peut donner qu'un seul point pour l'extrémité.
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
		}

		private Polygon Concave(double concave, bool ccw, bool curve = true)
		{
			//	Rend un polygone concave ou convexe.
			if (concave == 0)
			{
				return this;
			}
			else
			{
				var pp = new Polygon ();

				for (int i = 0; i < this.typedPoints.Count; i++)
				{
					TypedPoint a = this.GetCyclingTypedPoint (i);    // point courant
					TypedPoint b = this.GetCyclingTypedPoint (i+1);  // point suivant

					if (a.PointType == PointType.Primary && b.PointType == PointType.Primary)
					{
						if (curve)
						{
#if true
							var a1 = Point.Scale (a.Point, b.Point, 1.0/3.0);
							var b1 = Point.Scale (b.Point, a.Point, 1.0/3.0);

							var a2 = Point.Move (a1, b.Point, concave);
							var b2 = Point.Move (b1, a.Point, concave);

							var a3 = Polygon.RotateCW (a1, a2,  ccw);
							var b3 = Polygon.RotateCW (b1, b2, !ccw);

							pp.typedPoints.Add (a);
							pp.typedPoints.Add (new TypedPoint (a3, PointType.Secondary));
							pp.typedPoints.Add (new TypedPoint (b3, PointType.Secondary));
#else
							var p = Polygon.ConcaveSegment (a.Point, b.Point, concave, ccw);

							pp.typedPoints.Add (a);
							pp.typedPoints.Add (new TypedPoint (p, PointType.Secondary));
							pp.typedPoints.Add (new TypedPoint (p, PointType.Secondary));
#endif
						}
						else
						{
							var p = Polygon.ConcaveSegment (a.Point, b.Point, concave, ccw);

							pp.typedPoints.Add (a);
							pp.typedPoints.Add (new TypedPoint (p, PointType.Primary));
						}
					}
					else
					{
						pp.typedPoints.Add (a);
					}
				}

				return pp;
			}
		}

		private static Point ConcaveSegment(Point a, Point b, double concave, bool ccw)
		{
			var c = Point.Scale (a, b, 0.5);
			var p = Point.Move (c, b, concave);

			return Polygon.RotateCW (c, p, ccw);
		}

		private static Point RotateCW(Point center, Point a, bool ccw)
		{
			//	Retourne le point 'a' tourné de +/-90 degrés autour de 'center'.
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
			var surface = new InsideSurface (p, this.typedPoints.Count);

			for (int i = 0; i < this.typedPoints.Count; i++)
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
				count += polygon.typedPoints.Count;
			}

			var surface = new InsideSurface (p, count);

			foreach (var polygon in polygons)
			{
				for (int i = 0; i < polygon.typedPoints.Count; i++)
				{
					surface.AddLine (polygon.GetCyclingPoint (i), polygon.GetCyclingPoint (i+1));
				}
			}

			return surface.IsInside ();
		}


		private static readonly double miterLimit = 10;

		private readonly List<TypedPoint>	typedPoints;


		private struct TypedPoint
		{
			public TypedPoint(Point point, PointType pointType)
			{
				this.Point     = point;
				this.PointType = pointType;
			}

			public Point			Point;
			public PointType		PointType;

			public override string ToString()
			{
				return string.Format ("x:{0} y:{1} type:{2}", Point.X, Point.Y, PointType);
			}
		}
	}
}
