using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Geometry contient quelques routines générales.
	/// </summary>
	public class Geometry
	{
		// Détecte si la souris est sur le trait d'un chemin.
		public static bool DetectOutline(Path path, double width, Point pos)
		{
			return (Geometry.DetectOutlineRank(path, width, pos) != -1 );
		}

		// Détecte sur quel trait d'un chemin est la souris.
		// Retourne le rang du trait (0..1), ou -1.
		public static int DetectOutlineRank(Path path, double width, Point pos)
		{
			Point p1,s1,s2,p2;
			return Geometry.DetectOutlineRank(path, width, pos, out p1, out s1, out s2, out p2);
		}

		// Détecte sur quel trait d'un chemin est la souris.
		// Retourne le rang du trait (0..1), ou -1.
		public static int DetectOutlineRank(Path path, double width, Point pos,
											out Point bp1, out Point bs1, out Point bs2, out Point bp2)
		{
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);

			Point start = new Point(0, 0);
			Point current = new Point(0, 0);
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			Point p3 = new Point(0, 0);
			bp1 = new Point(0, 0);
			bs1 = new Point(0, 0);
			bs2 = new Point(0, 0);
			bp2 = new Point(0, 0);
			int i = 0;
			int rank = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & PathElement.MaskCommand )
				{
					case PathElement.MoveTo:
						current = points[i++];
						start = current;
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						if ( Point.DetectSegment(current,p1, pos, width) )
						{
							bp1 = current;
							bs1 = current;
							bs2 = p1;
							bp2 = p1;
							return rank;
						}
						rank ++;
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						p1 = Point.Scale(current, p1, 2.0/3.0);
						p2 = Point.Scale(p3,      p2, 2.0/3.0);
						if ( Point.DetectBezier(current,p1,p2,p3, pos, width) )
						{
							bp1 = current;
							bs1 = p1;
							bs2 = p2;
							bp2 = p3;
							return rank;
						}
						rank ++;
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if ( Point.DetectBezier(current,p1,p2,p3, pos, width) )
						{
							bp1 = current;
							bs1 = p1;
							bs2 = p2;
							bp2 = p3;
							return rank;
						}
						rank ++;
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							if ( current != start )
							{
								if ( Point.DetectSegment(current,start, pos, width) )
								{
									bp1 = current;
									bs1 = current;
									bs2 = start;
									bp2 = start;
									return rank;
								}
								rank ++;
							}
						}
						i ++;
						break;
				}
			}
			return -1;
		}

		// Détecte si la souris est dans un chemin.
		public static bool DetectSurface(Path path, Point pos)
		{
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);

			int total = 0;
			int i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & PathElement.MaskCommand )
				{
					case PathElement.MoveTo:
						i += 1;
						break;

					case PathElement.LineTo:
						i += 1;
						total += 1;
						break;

					case PathElement.Curve3:
						i += 2;
						total += InsideSurface.bezierStep;
						break;

					case PathElement.Curve4:
						i += 3;
						total += InsideSurface.bezierStep;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							total += 1;
						}
						i ++;
						break;
				}
			}
			InsideSurface surf = new InsideSurface(pos, total+10);

			Point start = new Point(0, 0);
			Point current = new Point(0, 0);
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			Point p3 = new Point(0, 0);
			i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & PathElement.MaskCommand )
				{
					case PathElement.MoveTo:
						current = points[i++];
						start = current;
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						surf.AddLine(current, p1);
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						p1 = Point.Scale(current, p1, 2.0/3.0);
						p2 = Point.Scale(p3,      p2, 2.0/3.0);
						surf.AddBezier(current, p1, p2, p3);
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						surf.AddBezier(current, p1, p2, p3);
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							surf.AddLine(current, start);
						}
						i ++;
						break;
				}
			}
			return surf.IsInside();
		}

		// Calcule la bbox qui englobe exactement un chemin quelconque.
		public static Rectangle ComputeBoundingBox(Path path)
		{
			Rectangle bbox = Rectangle.Empty;
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);

			Point start = new Point(0, 0);
			Point current = new Point(0, 0);
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			Point p3 = new Point(0, 0);
			int i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & PathElement.MaskCommand )
				{
					case PathElement.MoveTo:
						current = points[i++];
						start = current;
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						bbox.MergeWith(current);
						bbox.MergeWith(p1);
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						p1 = Point.Scale(current, p1, 2.0/3.0);
						p2 = Point.Scale(p3,      p2, 2.0/3.0);
						Geometry.BoundingBoxAddBezier(ref bbox, current,p1,p2,p3);
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						Geometry.BoundingBoxAddBezier(ref bbox, current,p1,p2,p3);
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							bbox.MergeWith(current);
							bbox.MergeWith(start);
						}
						i ++;
						break;
				}
			}
			return bbox;
		}

		// Ajoute une courbe de Bézier dans la bbox.
		protected static void BoundingBoxAddBezier(ref Rectangle bbox, Point p1, Point s1, Point s2, Point p2)
		{
			double step = 1.0/10.0;  // nombre arbitraire de 10 subdivisions
			for ( double t=0 ; t<=1.0 ; t+=step )
			{
				bbox.MergeWith(Point.FromBezier(p1, s1, s2, p2, t));
			}
		}


		#region PathToCurve
		// Convertit un chemin composé de segments de droites en un chemin composé
		// de courbes de Bézier.
		public static Path PathToCurve(Path path)
		{
			Path newPath = new Path();

			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);

			int i = 0;
			int ii = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & PathElement.MaskCommand )
				{
					case PathElement.MoveTo:
						ii = i;
						newPath.MoveTo(points[i++]);
						break;

					case PathElement.LineTo:
						if ( !Geometry.PathToCurveContinues(points, ii, i) )
						{
							Geometry.PathToCurveMagic(newPath, points, ii, i-1);
							ii = i-1;
						}
						i++;
						break;

					case PathElement.Curve3:
						Geometry.PathToCurveMagic(newPath, points, ii, i-1);
						newPath.CurveTo(points[i++], points[i++]);
						ii = i;
						break;

					case PathElement.Curve4:
						Geometry.PathToCurveMagic(newPath, points, ii, i-1);
						newPath.CurveTo(points[i++], points[i++], points[i++]);
						ii = i;
						break;

					default:
						Geometry.PathToCurveMagic(newPath, points, ii, i-1);
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							newPath.Close();
						}
						i++;
						ii = i;
						break;
				}
			}

			return newPath;
		}

		protected static bool PathToCurveContinues(Point[] points, int start, int current)
		{
			if ( current-start <= 1 )  return true;

			double r;
			if ( current-start <= 2 )
			{
				r = Geometry.PathToCurveRapport(points[current-2], points[current-1], points[current]);
				return r < 0.1;
			}
			else
			{
				r = Geometry.PathToCurveRapport(points[current-2], points[current-1], points[current]);
				if ( r >= 0.1 )  return false;

				r = Geometry.PathToCurveRapport(points[current-1]+points[start]-points[start+1], points[start+1], points[current]);
				return r < 0.5;
			}
		}

		protected static void PathToCurveMagic(Path newPath, Point[] points, int start, int end)
		{
			if ( end <= start )  return;

			if ( end-start <= 1 )
			{
				newPath.LineTo(points[end]);
			}
			else
			{
				Point p1 = points[start];
				Point s1 = points[start+1];
				Point s2 = points[end-1];
				Point p2 = points[end];

				double len = Point.Distance(p1, p2);
				s1 = Point.Move(p1, s1, len*0.4);
				s2 = Point.Move(p2, s2, len*0.4);

				newPath.CurveTo(s1, s2, p2);
			}
		}

		protected static double PathToCurveRapport(Point p1, Point p2, Point p3)
		{
			Point p = Point.Projection(p1,p3, p2);
			return Point.Distance(p2,p) / Point.Distance(p1,p3);
		}
		#endregion


		// Calcule le point d'intersection entre deux droites ab et cd.
		// Utilise l'algorithme de Gauss-Jordan utilisé pour le calcul
		// matriciel. Les calculs ont été spécialisés au cas simple de
		// l'intersection de segments pour des questions de rapidité.
		//
		//		Q=BX-AX : T=BY-AY
		//		R=CX-DX : U=CY-DY   matrice  [ Q R | S ]
		//		S=CX-AX : V=CY-AY            [ T U | V ]
		//
		// Cette matrice représente les coefficients de l'équation
		// vectorielle suivante :
		//		AB*a + CD*b = AC
		//
		// La coordonnée du point "P" s'obtient par :
		//		P = OA + a*AB
		//
		// ou encore :
		//		P = OC + b*CD
		//
		// Traite les cas particuliers des segments confondus ou parallèles.
		public static bool Intersect(Point a, Point b, Point c, Point d, out Point p)
		{
			double	q,r,s,t,u,v;

			q = b.X-a.X;
			r = c.X-d.X;
			s = c.X-a.X;
			t = b.Y-a.Y;
			u = c.Y-d.Y;
			v = c.Y-a.Y;

			p = new Point(0,0);

			if ( q == 0.0 )  // ab vertical ?
			{
				if ( r == 0.0 )  // cd vertical ?
				{
					return false;
				}
				else
				{
					p.X = ((d.X-c.X)*s/r)+c.X;
					p.Y = ((d.Y-c.Y)*s/r)+c.Y;
					return true;
				}
			}

			if ( t != 0.0 )  // ab pas horizontal ?
			{
				u = u-(t*r)/q;
				v = v-(t*s)/q;
			}

			if ( u == 0.0 )
			{
				return false;
			}

			p.X = ((d.X-c.X)*v/u)+c.X;
			p.Y = ((d.Y-c.Y)*v/u)+c.Y;
			return true;
		}

		// Teste si un point p est entre les points a et b du segment ab.
		public static bool IsInside(Point a, Point b, Point p)
		{
			if ( p == a || p == b )  return true;
			double length = Point.Distance(a,b);
			return ( Point.Distance(a,p) <= length && Point.Distance(b,p) <= length );
		}

		
		// Teste si l'objet est rectangulaire.
		public static bool IsRectangular(Point p0, Point p1, Point p2, Point p3)
		{
			if ( !Geometry.IsRight(p3, p0, p2) )  return false;
			if ( !Geometry.IsRight(p0, p2, p1) )  return false;
			if ( !Geometry.IsRight(p2, p1, p3) )  return false;
			if ( !Geometry.IsRight(p1, p3, p0) )  return false;
			return true;
		}

		// Teste si 3 points forment un angle droit.
		protected static bool IsRight(Point p1, Point corner, Point p2)
		{
			Point p = Point.Projection(p1, corner, p2);
			return Point.Distance(p, corner) < 0.00001;
		}


		// Teste si deux points sont (presque) identiques.
		public static bool Compare(Point a, Point b)
		{
			double dx = System.Math.Abs(a.X-b.X);
			double dy = System.Math.Abs(a.Y-b.Y);
			return ( dx < 0.0000001 && dy < 0.0000001 );
		}
	}
}
