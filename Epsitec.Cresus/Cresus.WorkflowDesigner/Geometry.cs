//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner
{
	/// <summary>
	/// La classe Geometry contient quelques routines générales.
	/// </summary>
	public class Geometry
	{
		public static bool IsOver(Rectangle rect, Point a, Point b, double margin)
		{
			//	Retourne true si la droite ab passe sur le rectangle.
			//	Les 4 droites du rectangle sont étendues selon margin, pour éviter des liaisons
			//	presque parallèles:
			//	    o        o
			//	    |        |
			//	o---o--------o---o
			//	    |        |
			//	    |        | margin
			//	    |        <--->
			//	o---o--------o---o
			//	    |        |
			//	    o        o
			if (rect.Contains(a) && rect.Contains(b))
			{
				return true;
			}

			if (Geometry.IsIntersect(new Point(rect.Left-margin, rect.Top), new Point(rect.Right+margin, rect.Top), a, b))
			{
				return true;
			}

			if (Geometry.IsIntersect(new Point(rect.Left-margin, rect.Bottom), new Point(rect.Right+margin, rect.Bottom), a, b))
			{
				return true;
			}

			if (Geometry.IsIntersect(new Point(rect.Left, rect.Bottom-margin), new Point(rect.Left, rect.Top+margin), a, b))
			{
				return true;
			}

			if (Geometry.IsIntersect(new Point(rect.Right, rect.Bottom-margin), new Point(rect.Right, rect.Top+margin), a, b))
			{
				return true;
			}

			return false;
		}

		public static bool IsIntersect(Point a, Point b, Point c, Point d)
		{
			//	Retourne true si le point d'intersection de deux droites est sur l'une des deux.
			Point[] i = Geometry.Intersect(a, b, c, d);
			if (i == null)
			{
				return false;
			}

			return Geometry.IsInside(a, b, i[0]) && Geometry.IsInside(c, d, i[0]);
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


		public static Point Projection(Path path, Point pos)
		{
			int rank = 0;
			double distance = double.MaxValue;
			Point best = Point.Zero;

			while (true)
			{
				Point pp1, ss1, ss2, pp2, onCurve;
				if (!Geometry.PathExtract (path, rank, out pp1, out ss1, out ss2, out pp2))
				{
					break;
				}

				double d;
				if (Geometry.DetectBezier (pp1, ss1, ss2, pp2, pos, out onCurve, out d))
				{
					if (distance > d)
					{
						best = onCurve;
						distance = d;
					}
				}

				rank++;
			}

			return best;
		}

		private static bool DetectBezier(Point p1, Point s1, Point s2, Point p2, Point p, out Point onCurve, out double distance)
		{
			int maxStep = 100;		//	nombre d'étapes arbitraire fixé à 100
			onCurve = Point.Zero;
			distance = double.MaxValue;

			double t = 0;
			double dt = 1.0 / maxStep;

			for (int step = 1; step <= maxStep; step++)
			{
				t += dt;

				Point b = Point.FromBezier (p1, s1, s2, p2, t);
				double d = Point.Distance (b, p);

				if (distance > d)
				{
					onCurve = b;
					distance = d;
				}
			}

			return !onCurve.IsZero;
		}

		private static bool PathExtract(Path path, int rank, out Point pp1, out Point ss1, out Point ss2, out Point pp2)
		{
			//	Extrait un fragment de droite ou de courbe d'un chemin.
			//	Attention à utiliser un chemin obtenu avec GetShaperPath, et non GetMagnetPath !
			PathElement[] elements;
			Point[] points;
			path.GetElements (out elements, out points);

			pp1 = Point.Zero;
			ss1 = Point.Zero;
			ss2 = Point.Zero;
			pp2 = Point.Zero;

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
							pp1 = current;
							ss1 = current;
							ss2 = p1;
							pp2 = p1;
							return true;
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
							pp1 = current;
							ss1 = p1;
							ss2 = p2;
							pp2 = p3;
							return true;
						}
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if (--rank < 0)
						{
							pp1 = current;
							ss1 = p1;
							ss2 = p2;
							pp2 = p3;
							return true;
						}
						current = p3;
						break;

					default:
						if ((elements[i] & PathElement.FlagClose) != 0)
						{
							if (!Point.Equals (current, start) && --rank < 0)
							{
								pp1 = current;
								ss1 = current;
								ss2 = start;
								pp2 = start;
								return true;
							}
						}
						i++;
						break;
				}
			}
			return false;
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
	}
}
