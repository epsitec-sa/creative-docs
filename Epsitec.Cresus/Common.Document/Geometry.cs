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
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);

			Point start = new Point(0, 0);
			Point current = new Point(0, 0);
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			Point p3 = new Point(0, 0);
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
						if ( Point.DetectSegment(current,p1, pos, width) )  return rank;
						rank ++;
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i++];
						p2 = points[i++];
						if ( Point.DetectBezier(current,p1,p1,p2, pos, width) )  return rank;
						rank ++;
						current = p2;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if ( Point.DetectBezier(current,p1,p2,p3, pos, width) )  return rank;
						rank ++;
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							if ( Point.DetectSegment(current,start, pos, width) )  return rank;
							rank ++;
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
						p1 = points[i++];
						p2 = points[i++];
						surf.AddBezier(current, p1, p1, p2);
						current = p2;
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
						p1 = points[i++];
						p2 = points[i++];
						Geometry.BoundingBoxAddBezier(ref bbox, current,p1,p1,p2);
						current = p2;
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

		// Ajoute un courbe de Bézier dans la bbox.
		protected static void BoundingBoxAddBezier(ref Rectangle bbox, Point p1, Point s1, Point s2, Point p2)
		{
			double step = 1.0/10.0;  // nombre arbitraire de 10 subdivisions
			for ( double t=0 ; t<=1.0 ; t+=step )
			{
				bbox.MergeWith(Point.FromBezier(p1, s1, s2, p2, t));
			}
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
	}
}
