//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner
{
	/// <summary>
	/// La classe Geometry contient quelques routines générales.
	/// </summary>
	public class Geometry
	{
		public static double AngleAvg(IList<double> angles)
		{
			//	Retourne la moyenne de plusieurs angles donnés en degrés.
			//	Pour effectuer la moyenne des angles, on procède à une addition de vecteurs unitaires.
			double x = 0;
			double y = 0;

			foreach (var angle in angles)
			{
				double rad = Geometry.DegToRad (angle);

				x += System.Math.Cos (rad);  // ajoute le vecteur unitaire
				y += System.Math.Sin (rad);
			}

			return Point.ComputeAngleDeg (x, y);
		}


		private static double DegToRad(double angle)
		{
			return angle * System.Math.PI / 180.0;
		}

		private static double RadToDeg(double angle)
		{
			return angle * 180.0 / System.Math.PI;
		}


		public static Point IsIntersect(Point a, Point b, Point c, Point d)
		{
			Point[] points = Geometry.Intersect (a, b, c, d);

			if (points == null)
			{
				return Point.Zero;
			}

			Point i = points[0];
			if (Geometry.IsInside (a, b, i) && Geometry.IsInside (c, d, i))
			{
				return i;
			}

			return Point.Zero;
		}

		public static Point[] Intersect(Point a, Point b, Point c, Point d)
		{
			//	Calcule le point d'intersection entre deux droites ab et cd.
			//	Utilise l'algorithme de Gauss-Jordan utilisé pour le calcul
			//	matriciel. Les calculs ont été spécialisés au cas simple de
			//	l'intersection de segments pour des questions de rapidité.
			//
			//		Q=BX-AX : T=BY-AY
			//		R=CX-DX : U=CY-DY   matrice  [ Q R | S ]
			//		S=CX-AX : V=CY-AY            [ T U | V ]
			//
			//	Cette matrice représente les coefficients de l'équation
			//	vectorielle suivante :
			//		AB*a + CD*b = AC
			//
			//	La coordonnée du point "P" s'obtient par :
			//		P = OA + a*AB
			//
			//	ou encore :
			//		P = OC + b*CD
			//
			//	Traite les cas particuliers des segments confondus ou parallèles.
			double	q,r,s,t,u,v;

			q = b.X-a.X;
			r = c.X-d.X;
			s = c.X-a.X;
			t = b.Y-a.Y;
			u = c.Y-d.Y;
			v = c.Y-a.Y;

			Point p = new Point();
			Point[] table = new Point[1];

			if ( q == 0.0 )  // ab vertical ?
			{
				if ( r == 0.0 )  // cd vertical ?
				{
					return null;
				}
				else
				{
					p.X = ((d.X-c.X)*s/r)+c.X;
					p.Y = ((d.Y-c.Y)*s/r)+c.Y;
					table[0] = p;
					return table;
				}
			}

			if ( t != 0.0 )  // ab pas horizontal ?
			{
				u = u-(t*r)/q;
				v = v-(t*s)/q;
			}

			if ( u == 0.0 )
			{
				return null;
			}

			p.X = ((d.X-c.X)*v/u)+c.X;
			p.Y = ((d.Y-c.Y)*v/u)+c.Y;
			table[0] = p;
			return table;
		}

		public static bool IsInside(Point a, Point b, Point p)
		{
			//	Teste si un point p est entre les points a et b du segment ab.
			if ( p == a || p == b )  return true;
			double length = Point.Distance(a,b);
			return ( Point.Distance(a,p) <= length && Point.Distance(b,p) <= length );
		}


		public static Point PointOnPath(Path path, double offset)
		{
			//	Calcule la position d'un point sur un chemin, à partir d'un offset compris entre 0 et 1.
			//	0 -> début du chemin
			//	1 -> fin du chemin
			var list = Geometry.PathExtract (path);

			if (list.Count == 0)
			{
				return Point.Zero;
			}

			int i = (int) (offset*list.Count);
			i = System.Math.Max (i, 0);
			i = System.Math.Min (i, list.Count-1);

			double t = (i+offset) / list.Count;

			return Point.FromBezier (list[i].p1, list[i].s1, list[i].s2, list[i].p2, t);
		}

		public static double OffsetOnPath(Path path, Point pos)
		{
			//	Calcule l'offset d'un point sur un chemin.
			//	0 -> début du chemin
			//	1 -> fin du chemin
			var list = Geometry.PathExtract (path);

			if (list.Count == 0)
			{
				return 0;
			}

			double distance = double.MaxValue;
			double offset = 0;

			foreach (var b in list)
			{
				double d, t;
				Geometry.DetectBezier (b, pos, out d, out t);

				if (distance > d)
				{
					distance = d;
					offset = t;
				}
			}

			return offset;
		}

		private static void DetectBezier(SingleBezier b, Point pos, out double distance, out double offset)
		{
			//	Détecte si le point P est sur un segment de Bezier d'épaisseur 'width'.

			int maxStep = 100;		//	nombre d'étapes arbitraire fixé à 100

			Point  a = b.p1;
			double t = 0;
			double dt = 1.0 / maxStep;
			distance = double.MaxValue;
			offset = 0;

			for (int step = 1; step <= maxStep; step++)
			{
				t += dt;

				Point p = Point.FromBezier (b.p1, b.s1, b.s2, b.p2, t);
				double d = Point.Distance (p, pos);

				if (distance > d)
				{
					distance = d;
					offset = t;
				}
			}
		}


		private static List<SingleBezier> PathExtract(Path path)
		{
			//	Extrait tous les fragments de droite ou de courbe d'un chemin.
			var list = new List<SingleBezier> ();
			int rank = 0;

			while (true)
			{
				SingleBezier b = Geometry.PathExtract (path, rank++);

				if (b.IsZero)
				{
					break;
				}

				list.Add (b);
			}

			return list;
		}

		private static SingleBezier PathExtract(Path path, int rank)
		{
			//	Extrait un fragment de droite ou de courbe d'un chemin.
			PathElement[] elements;
			Point[] points;
			path.GetElements (out elements, out points);

			var result = new SingleBezier ();

			Point start = Point.Zero;
			Point current = Point.Zero;
			Point p1 = Point.Zero;
			Point p2 = Point.Zero;
			Point p3 = Point.Zero;
			int i = 0;
			while (i < elements.Length)
			{
				switch (elements[i] & PathElement.MaskCommand)
				{
					case PathElement.MoveTo:
						current = points[i++];
						start = current;
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						if (--rank < 0)
						{
							result.p1 = current;
							result.s1 = current;
							result.s2 = p1;
							result.p2 = p1;
							return result;
						}
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						Geometry.BezierS1ToS2 (current, ref p1, ref p2, p3);
						if (--rank < 0)
						{
							result.p1 = current;
							result.s1 = p1;
							result.s2 = p2;
							result.p2 = p3;
							return result;
						}
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if (--rank < 0)
						{
							result.p1 = current;
							result.s1 = p1;
							result.s2 = p2;
							result.p2 = p3;
							return result;
						}
						current = p3;
						break;

					default:
						if ((elements[i] & PathElement.FlagClose) != 0)
						{
							if (!Point.Equals (current, start) && --rank < 0)
							{
								result.p1 = current;
								result.s1 = current;
								result.s2 = start;
								result.p2 = start;
								return result;
							}
						}
						i++;
						break;
				}
			}

			result.p1 = Point.Zero;
			result.s1 = Point.Zero;
			result.s2 = Point.Zero;
			result.p2 = Point.Zero;
			return result;
		}


		public static bool DetectOutline(Path path, double width, Point pos)
		{
			//	Détecte si la souris est sur le trait d'un chemin.
			return (Geometry.DetectOutlineRank (path, width, pos) != -1);
		}

		public static int DetectOutlineRank(Path path, double width, Point pos)
		{
			//	Détecte sur quel trait d'un chemin est la souris.
			//	Retourne le rang du trait (0..1), ou -1.
			//	Attention à utiliser un chemin obtenu avec GetShaperPath, et non GetMagnetPath !
			Point p1,s1,s2,p2;
			return Geometry.DetectOutlineRank (path, width, pos, out p1, out s1, out s2, out p2);
		}

		public static int DetectOutlineRank(Path path, double width, Point pos,
											out Point bp1, out Point bs1, out Point bs2, out Point bp2)
		{
			//	Détecte sur quel trait d'un chemin est la souris.
			//	Retourne le rang du trait (0..1), ou -1.
			//	Attention à utiliser un chemin obtenu avec GetShaperPath, et non GetMagnetPath !
			bp1 = Point.Zero;
			bs1 = Point.Zero;
			bs2 = Point.Zero;
			bp2 = Point.Zero;

			if (path == null)
			{
				return -1;
			}

			PathElement[] elements;
			Point[] points;
			path.GetElements (out elements, out points);

			Point start = Point.Zero;
			Point current = Point.Zero;
			Point p1 = Point.Zero;
			Point p2 = Point.Zero;
			Point p3 = Point.Zero;
			int i = 0;
			int rank = 0;
			while (i < elements.Length)
			{
				switch (elements[i] & PathElement.MaskCommand)
				{
					case PathElement.MoveTo:
						current = points[i++];
						start = current;
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						if (Point.DetectSegment (current, p1, pos, width))
						{
							bp1 = current;
							bs1 = current;
							bs2 = p1;
							bp2 = p1;
							return rank;
						}
						rank++;
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						Geometry.BezierS1ToS2 (current, ref p1, ref p2, p3);
						if (Point.DetectBezier (current, p1, p2, p3, pos, width))
						{
							bp1 = current;
							bs1 = p1;
							bs2 = p2;
							bp2 = p3;
							return rank;
						}
						rank++;
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if (Point.DetectBezier (current, p1, p2, p3, pos, width))
						{
							bp1 = current;
							bs1 = p1;
							bs2 = p2;
							bp2 = p3;
							return rank;
						}
						rank++;
						current = p3;
						break;

					default:
						if ((elements[i] & PathElement.FlagClose) != 0)
						{
							if (current != start)
							{
								if (Point.DetectSegment (current, start, pos, width))
								{
									bp1 = current;
									bs1 = current;
									bs2 = start;
									bp2 = start;
									return rank;
								}
								rank++;
							}
						}
						i++;
						break;
				}
			}
			return -1;
		}

		public static void BezierS1ToS2(Point p1, ref Point s1, ref Point s2, Point p2)
		{
			//	Convertit une courbe de Bézier définie par un seul point secondaire en
			//	une courbe "traditionnelle" définie par deux points secondaires.
			//	Il s'agit ici d'une approximation empyrique !
			s1 = Point.Scale (p1, s1, 2.0/3.0);
			s2 = Point.Scale (p2, s2, 2.0/3.0);
		}


		private struct SingleBezier
		{
			public Point p1;
			public Point s1;
			public Point s2;
			public Point p2;

			public bool IsZero
			{
				get
				{
					return this.p1.IsZero || this.s1.IsZero || this.s2.IsZero || this.p2.IsZero;
				}
			}
		}
	}
}
