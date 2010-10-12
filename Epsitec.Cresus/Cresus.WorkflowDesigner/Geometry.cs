//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner
{
	/// <summary>
	/// La classe Geometry contient quelques routines g�n�rales.
	/// </summary>
	public class Geometry
	{
		public static bool IsOver(Rectangle rect, Point a, Point b, double margin)
		{
			//	Retourne true si la droite ab passe sur le rectangle.
			//	Les 4 droites du rectangle sont �tendues selon margin, pour �viter des liaisons
			//	presque parall�les:
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
			//	Utilise l'algorithme de Gauss-Jordan utilis� pour le calcul
			//	matriciel. Les calculs ont �t� sp�cialis�s au cas simple de
			//	l'intersection de segments pour des questions de rapidit�.
			//
			//		Q=BX-AX : T=BY-AY
			//		R=CX-DX : U=CY-DY   matrice  [ Q R | S ]
			//		S=CX-AX : V=CY-AY            [ T U | V ]
			//
			//	Cette matrice repr�sente les coefficients de l'�quation
			//	vectorielle suivante :
			//		AB*a + CD*b = AC
			//
			//	La coordonn�e du point "P" s'obtient par :
			//		P = OA + a*AB
			//
			//	ou encore :
			//		P = OC + b*CD
			//
			//	Traite les cas particuliers des segments confondus ou parall�les.
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


		public static bool DetectOutline(Path path, double width, Point pos)
		{
			//	D�tecte si la souris est sur le trait d'un chemin.
			return (Geometry.DetectOutlineRank (path, width, pos) != -1);
		}

		public static int DetectOutlineRank(Path path, double width, Point pos)
		{
			//	D�tecte sur quel trait d'un chemin est la souris.
			//	Retourne le rang du trait (0..1), ou -1.
			//	Attention � utiliser un chemin obtenu avec GetShaperPath, et non GetMagnetPath !
			Point p1,s1,s2,p2;
			return Geometry.DetectOutlineRank (path, width, pos, out p1, out s1, out s2, out p2);
		}

		public static int DetectOutlineRank(Path path, double width, Point pos,
											out Point bp1, out Point bs1, out Point bs2, out Point bp2)
		{
			//	D�tecte sur quel trait d'un chemin est la souris.
			//	Retourne le rang du trait (0..1), ou -1.
			//	Attention � utiliser un chemin obtenu avec GetShaperPath, et non GetMagnetPath !
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
			//	Convertit une courbe de B�zier d�finie par un seul point secondaire en
			//	une courbe "traditionnelle" d�finie par deux points secondaires.
			//	Il s'agit ici d'une approximation empyrique !
			s1 = Point.Scale (p1, s1, 2.0/3.0);
			s2 = Point.Scale (p2, s2, 2.0/3.0);
		}
	}
}
