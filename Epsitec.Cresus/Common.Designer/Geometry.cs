using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
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

	}
}
