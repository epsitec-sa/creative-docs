using System.Collections.Generic;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Geometry contient quelques routines générales.
	/// </summary>
	public class Geometry
	{
		public static double GetSliderPosition(Point sliderStarting, Point sliderEnding, Point pos)
		{
			pos = Point.Projection (sliderStarting, sliderEnding, pos);

			double minX = System.Math.Min (sliderStarting.X, sliderEnding.X);
			double maxX = System.Math.Max (sliderStarting.X, sliderEnding.X);
			double minY = System.Math.Min (sliderStarting.Y, sliderEnding.Y);
			double maxY = System.Math.Max (sliderStarting.Y, sliderEnding.Y);

			if (pos.X >= minX && pos.X <= maxX && pos.Y >= minY && pos.Y <= maxY)
			{
				return Point.Distance (sliderStarting, pos) / Point.Distance (sliderStarting, sliderEnding);
			}
			else
			{
				if (Point.Distance (pos, sliderStarting) < Point.Distance (pos, sliderEnding))
				{
					return 0;
				}
				else
				{
					return 1;
				}
			}
		}


		public static Point ComputeArrowExtremity(Point p1, Point p2, double para, double perp, int rank)
		{
			//	Calcule l'extrémité gauche ou droite d'une flèche.
			double distPara = Point.Distance (p1, p2)*para;
			double distPerp = Point.Distance (p1, p2)*perp;

			Point c = Point.Move (p2, p1, distPara);
			Point p = Point.Move (c, Point.Symmetry (p2, p1), distPerp);

			double angle = (rank == 0) ? 90 : -90;
			return Transform.RotatePointDeg (c, angle, p);
		}

	
		public static List<Polygon> PathToPolygons(Path path)
		{
			//	Extrait les points d'un chemin quelconque.
			var polygons = new List<Polygon> ();
			Polygon polygon = null;

			PathElement[] elements;
			Point[] points;
			path.GetElements (out elements, out points);

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

						polygon = new Polygon ();
						polygons.Add (polygon);

						polygon.Add (current, PointType.Primary);
						start = current;
						break;

					case PathElement.LineTo:
						current = points[i++];

						if (polygon != null)
						{
							polygon.Add (current, PointType.Primary);
						}
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];

						if (polygon != null)
						{
							polygon.Add (p1, PointType.Secondary);
							polygon.Add (p2, PointType.Secondary);
							polygon.Add (p3, PointType.Primary);
						}
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];

						if (polygon != null)
						{
							polygon.Add (p1, PointType.Secondary);
							polygon.Add (p2, PointType.Secondary);
							polygon.Add (p3, PointType.Primary);
						}
						current = p3;
						break;

					default:
						if ((elements[i] & PathElement.FlagClose) != 0)
						{
							if (polygon != null && !Point.Equals(current, start))
							{
								polygon.Add (current, PointType.Primary);
							}
						}
						i ++;
						break;
				}
			}

			Polygon.Simplify (polygons);
			return polygons;
		}

		public static Path PathExtract(Path path, int rank)
		{
			//	Extrait un fragment d'un chemin.
			//	Attention à utiliser un chemin obtenu avec GetShaperPath, et non GetMagnetPath !
			Point p1, s1, s2, p2;
			Objects.SelectedSegment.Type type;
			type = Geometry.PathExtract(path, rank, out p1, out s1, out s2, out p2);

			if ( type == Objects.SelectedSegment.Type.Line )
			{
				Path subPath = new Path();
				subPath.MoveTo(p1);
				subPath.LineTo(p2);
				return subPath;
			}
			
			if ( type == Objects.SelectedSegment.Type.Curve )
			{
				Path subPath = new Path();
				subPath.MoveTo(p1);
				subPath.CurveTo(s1, s2, p2);
				return subPath;
			}

			return null;
		}

		public static Objects.SelectedSegment.Type PathExtract(Path path, int rank, out Point pp1, out Point ss1, out Point ss2, out Point pp2)
		{
			//	Extrait un fragment de droite ou de courbe d'un chemin.
			//	Attention à utiliser un chemin obtenu avec GetShaperPath, et non GetMagnetPath !
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);

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
						if ( --rank < 0 )
						{
							pp1 = current;
							ss1 = current;
							ss2 = p1;
							pp2 = p1;
							return Objects.SelectedSegment.Type.Line;
						}
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						Geometry.BezierS1ToS2(current, ref p1, ref p2, p3);
						if ( --rank < 0 )
						{
							pp1 = current;
							ss1 = p1;
							ss2 = p2;
							pp2 = p3;
							return Objects.SelectedSegment.Type.Curve;
						}
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if ( --rank < 0 )
						{
							pp1 = current;
							ss1 = p1;
							ss2 = p2;
							pp2 = p3;
							return Objects.SelectedSegment.Type.Curve;
						}
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							if ( !Point.Equals(current, start) && --rank < 0 )
							{
								pp1 = current;
								ss1 = current;
								ss2 = start;
								pp2 = start;
								return Objects.SelectedSegment.Type.Line;
							}
						}
						i ++;
						break;
				}
			}
			return Objects.SelectedSegment.Type.None;
		}

		public static bool DetectOutline(Path path, double width, Point pos)
		{
			//	Détecte si la souris est sur le trait d'un chemin.
			return (Geometry.DetectOutlineRank(path, width, pos) != -1);
		}

		public static int DetectOutlineRank(Path path, double width, Point pos)
		{
			//	Détecte sur quel trait d'un chemin est la souris.
			//	Retourne le rang du trait (0..1), ou -1.
			//	Attention à utiliser un chemin obtenu avec GetShaperPath, et non GetMagnetPath !
			Point p1,s1,s2,p2;
			return Geometry.DetectOutlineRank(path, width, pos, out p1, out s1, out s2, out p2);
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
			path.GetElements(out elements, out points);

			Point start = Point.Zero;
			Point current = Point.Zero;
			Point p1 = Point.Zero;
			Point p2 = Point.Zero;
			Point p3 = Point.Zero;
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
						Geometry.BezierS1ToS2(current, ref p1, ref p2, p3);
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

		public static bool DetectSurface(Path path, Point pos)
		{
			//	Détecte si la souris est dans un chemin.
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

			Point start = Point.Zero;
			Point current = Point.Zero;
			Point p1 = Point.Zero;
			Point p2 = Point.Zero;
			Point p3 = Point.Zero;
			bool closed = false;
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
						Geometry.BezierS1ToS2(current, ref p1, ref p2, p3);
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
							if (current != start)
							{
								surf.AddLine (current, start);
							}
							closed = true;
						}
						i ++;
						break;
				}
			}

			if ( !closed )
			{
				surf.AddLine(current, start);
			}

			return surf.IsInside();
		}

		public static Rectangle ComputeBoundingBox(Shape shape)
		{
			//	Calcule la bbox qui englobe exactement une forme quelconque.
			Properties.Line     stroke  = shape.PropertyStroke  as Properties.Line;
			Properties.Gradient surface = shape.PropertySurface as Properties.Gradient;

			if ( shape.Type == Type.Stroke && stroke != null && surface != null && shape.IsVisible )
			{
				double width   = stroke.Width + surface.Smooth*2;
				CapStyle cap   = stroke.Cap;
				JoinStyle join = stroke.EffectiveJoin;
				double limit   = stroke.Limit;

				Path realPath = new Path();
				realPath.Append(shape.Path, width, cap, join, limit, 0.5);  // (*)
				return Geometry.ComputeBoundingBox(realPath);

				// (*)	Si approximation_zoom vaut 0.1, certaines bboxGeom sont calculées de façon imprécises.
				//		Ceci est surtout visible lors de forts grossissements, ce qui est très marqué avec
				//		Pictogram, mais également visible avec CrDoc. Avec 0.5, le calcul semble plus précis,
				//		mais je ne comprends pas la signification de ce paramètre !
			}

			if ( shape.Type == Type.Surface && surface != null && shape.IsVisible )
			{
				Rectangle bbox = Geometry.ComputeBoundingBox(shape.Path);
				bbox.Inflate(surface.Smooth);
				return bbox;
			}

			return Geometry.ComputeBoundingBox(shape.Path);
		}

		public static Rectangle ComputeBoundingBox(Path path)
		{
			//	Calcule la bbox qui englobe exactement un chemin quelconque.
#if true
			return path.ComputeBounds();
#else
			Rectangle bbox = Rectangle.Empty;
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);

			Point start = Point.Zero;
			Point current = Point.Zero;
			Point p1 = Point.Zero;
			Point p2 = Point.Zero;
			Point p3 = Point.Zero;
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
						Geometry.BezierS1ToS2(current, ref p1, ref p2, p3);
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

		protected static void BoundingBoxAddBezier(ref Rectangle bbox, Point p1, Point s1, Point s2, Point p2)
		{
			//	Ajoute une courbe de Bézier dans la bbox.
			double step = 1.0/10.0;  // nombre arbitraire de 10 subdivisions
			for ( double t=0 ; t<=1.0 ; t+=step )
			{
				bbox.MergeWith(Point.FromBezier(p1, s1, s2, p2, t));
			}
#endif
		}

		public static void BezierS1ToS2(Point p1, ref Point s1, ref Point s2, Point p2)
		{
			//	Convertit une courbe de Bézier définie par un seul point secondaire en
			//	une courbe "traditionnelle" définie par deux points secondaires.
			//	Il s'agit ici d'une approximation empyrique !
			s1 = Point.Scale(p1, s1, 2.0/3.0);
			s2 = Point.Scale(p2, s2, 2.0/3.0);
		}


		#region PathToCurve
		public static Path PathToCurve(Path path)
		{
			//	Convertit un chemin composé de segments de droites en un chemin composé
			//	de courbes de Bézier.
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


		public static Point[] Intersect(Point a, Point b, Point c, double r)
		{
			//	Calcule le ou les deux points d'intersection entre une droite ab et
			//	un cercle de centre c et de rayon r.
			double angle = Point.ComputeAngleDeg(a, b);
			a = Transform.RotatePointDeg(-angle, a-c);
			if ( a.Y < -r || a.Y > r )  return null;

			if ( a.Y == r || a.Y == -r )
			{
				Point i = new Point(0, a.Y);
				i = Transform.RotatePointDeg(angle, i)+c;
				Point[] table = new Point[1];
				table[0] = i;
				return table;
			}
			else
			{
				double x = System.Math.Sqrt(r*r - a.Y*a.Y);
				Point i1 = new Point( x, a.Y);
				Point i2 = new Point(-x, a.Y);
				i1 = Transform.RotatePointDeg(angle, i1)+c;
				i2 = Transform.RotatePointDeg(angle, i2)+c;
				Point[] table = new Point[2];
				table[0] = i1;
				table[1] = i2;
				return table;
			}
		}

		public static Point[] Intersect(Point a, double ra, Point b, double rb)
		{
			//	Calcule le ou les deux points d'intersection entre deux cercles.
			double d = Point.Distance(a, b);

			if ( d > ra+rb )
			{
				return null;
			}

			if ( d == ra+rb )
			{
				Point[] table = new Point[1];
				table[0] = Point.Move(a, b, ra);
				return table;
			}

			if ( a.Y == b.Y )
			{
				double sqr = ((ra*ra-rb*rb+a.X*a.X+b.X*b.X-2*a.X*b.X)/(2*(b.X-a.X)));
				sqr = System.Math.Pow(sqr, 2);
				sqr = ra*ra - sqr;
				sqr = System.Math.Sqrt(sqr);

				Point p = new Point();
				Point q = new Point();
				p.X = q.X = (ra*ra-rb*rb-a.X*a.X+b.X*b.X)/(2*(b.X-a.X));
				p.Y = a.Y + sqr;
				q.Y = a.Y - sqr;

				Point[] table = new Point[2];
				table[0] = p;
				table[1] = q;
				return table;
			}

			double A = 2*(b.X-a.X);
			double B = 2*(b.Y-a.Y);
			double C = System.Math.Pow(b.X-a.X, 2) + System.Math.Pow(b.Y-a.Y, 2) - rb*rb + ra*ra;
			double delta = System.Math.Pow(2*A*C, 2) - 4*(A*A+B*B)*(C*C-B*B*ra*ra);

			if ( B == 0 )
			{
				Point p = new Point();
				Point q = new Point();
				double sqr = System.Math.Sqrt(delta);
				p.X = a.X + (2*A*C-sqr)/(2*(A*A+B*B));
				q.X = a.X + (2*A*C+sqr)/(2*(A*A+B*B));
				double sqr2 = System.Math.Sqrt(rb*rb - System.Math.Pow((2*C-A*A)/2*A, 2));
				p.Y = a.Y + B/2 + sqr2;
				q.Y = a.Y + B/2 - sqr2;

				Point[] table = new Point[2];
				table[0] = p;
				table[1] = q;
				return table;
			}
			else
			{
				Point p = new Point();
				Point q = new Point();
				double sqr = System.Math.Sqrt(delta);
				p.X = a.X + (2*A*C-sqr)/(2*(A*A+B*B));
				q.X = a.X + (2*A*C+sqr)/(2*(A*A+B*B));
				p.Y = a.Y + (C-A*(p.X-a.X))/B;
				q.Y = a.Y + (C-A*(q.X-a.X))/B;

				Point[] table = new Point[2];
				table[0] = p;
				table[1] = q;
				return table;
			}
		}

		
		public static bool IsRectangular(Point p0, Point p1, Point p2, Point p3)
		{
			//	Teste si l'objet est rectangulaire.
			if ( !Geometry.IsRight(p3, p0, p2) )  return false;
			if ( !Geometry.IsRight(p0, p2, p1) )  return false;
			if ( !Geometry.IsRight(p2, p1, p3) )  return false;
			if ( !Geometry.IsRight(p1, p3, p0) )  return false;
			return true;
		}

		public static bool IsRight(Point p1, Point corner, Point p2)
		{
			//	Teste si 3 points forment un angle droit.
			Point p = Point.Projection(p1, corner, p2);
			return Point.Distance(p, corner) < 0.00001;
		}


		public static bool Compare(Point a, Point b)
		{
			//	Teste si deux points sont (presque) identiques.
			double dx = System.Math.Abs(a.X-b.X);
			double dy = System.Math.Abs(a.Y-b.Y);
			return ( dx < 0.0000001 && dy < 0.0000001 );
		}


		public static double Hypothenus(double a, double b)
		{
			//	Calcule l'hypothénuse d'un triangle rectangle.
			return System.Math.Sqrt(a*a + b*b);
		}

	
		public static Point ArcBezierDeg(Path path, Stretcher stretcher, Point c, double rx, double ry, double a1, double a2, bool ccw, bool continuePath)
		{
			//	Génère un arc de cercle à l'aide d'un maximun de 4 courbes de Bézier.
			//	Retourne le point d'arrivée.
			Point r = new Point(rx, ry);
			Point p2 = new Point();
			
			a1 = Math.DegToRad(a1);
			a2 = Math.DegToRad(a2);
			a1 = Math.ClipAngleRad(a1);
			a2 = Math.ClipAngleRad(a2);
			
			if ( System.Math.Abs(a1-a2) < 0.0000001 )
			{
				a2 = a1;
			}
			
			if ( ccw )
			{
				if ( a2 <= a1 )
				{
					a2 += System.Math.PI * 2.0;
				}
				
				while ( a1 < a2 )
				{
					double aa = System.Math.Min(a1 + System.Math.PI/2.0, a2);
					double k  = Geometry.GetArcBezierKappaRad(aa - a1);
					
					Point p1, s1, s2;
					
					Geometry.ArcBezierPSRad(a1, k,  ccw, out p1, out s1);
					Geometry.ArcBezierPSRad(aa, k, !ccw, out p2, out s2);
					
					p1 = c + Point.ScaleMul(p1, r);
					s1 = c + Point.ScaleMul(s1, r);
					s2 = c + Point.ScaleMul(s2, r);
					p2 = c + Point.ScaleMul(p2, r);

					if ( !continuePath )
					{
						continuePath = true;
						path.MoveTo(stretcher.Transform(p1));
					}
					path.CurveTo(stretcher.Transform(s1), stretcher.Transform(s2), stretcher.Transform(p2));
					a1 = aa;
				}
			}
			else
			{
				if ( a2 >= a1 )
				{
					a2 -= System.Math.PI * 2.0;
				}
				
				while ( a1 > a2 )
				{
					double aa = System.Math.Max(a1 - System.Math.PI/2.0, a2);
					double k  = Geometry.GetArcBezierKappaRad(a1 - aa);
					
					Point p1, s1, s2;
					
					Geometry.ArcBezierPSRad(a1, k,  ccw, out p1, out s1);
					Geometry.ArcBezierPSRad(aa, k, !ccw, out p2, out s2);
					
					p1 = c + Point.ScaleMul(p1, r);
					s1 = c + Point.ScaleMul(s1, r);
					s2 = c + Point.ScaleMul(s2, r);
					p2 = c + Point.ScaleMul(p2, r);
					
					if ( !continuePath )
					{
						continuePath = true;
						path.MoveTo(stretcher.Transform(p1));
					}
					path.CurveTo(stretcher.Transform(s1), stretcher.Transform(s2), stretcher.Transform(p2));
					a1 = aa;
				}
			}
			
			return p2;
		}

		protected static void ArcBezierPSRad(double a, double k, bool ccw, out Point p, out Point s)
		{
			//	Calcule le point principal et le point secondaire d'un arc de Bézier
			//	de rayon 1 et de centre (0;0).
			p = new Point(System.Math.Cos(a), System.Math.Sin(a));
			s = (ccw) ? new Point(p.X - System.Math.Sin(a)*k, p.Y + System.Math.Cos(a)*k)
					  : new Point(p.X + System.Math.Sin(a)*k, p.Y - System.Math.Cos(a)*k);
		}

		protected static double GetArcBezierKappaRad(double a)
		{
			//	Détermine le facteur kappa en fonction de l'angle (0..PI/2).
			double sin = System.Math.Sin(a/2.0);
			double cos = System.Math.Cos(a/2.0);
			
			double dx = (4.0-4.0*cos)/3.0;
			double dy = sin+(1.0-cos)*(cos-3.0)/(3.0*sin);
			
			return System.Math.Sqrt(dx*dx + dy*dy);
		}
	}
}
