using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe InsideSurface permet de calculer si un point est dans une surface
	/// fermée quelconque constituée de segments de droites ou de courbes de Bézier.
	/// </summary>
	public class InsideSurface
	{
		public static bool Contains(Path path, Point pos)
		{
			//	Détecte si la souris est dans un chemin.
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);

			int total = 0;
			int i = 0;
			while (i < elements.Length)
			{
				switch (elements[i] & PathElement.MaskCommand)
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
						if ((elements[i] & PathElement.FlagClose) != 0)
						{
							total += 1;
						}
						i++;
						break;
				}
			}
			InsideSurface surf = new InsideSurface(pos, total+10);

			Point start = new Point(0, 0);
			Point current = new Point(0, 0);
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			Point p3 = new Point(0, 0);
			bool closed = false;
			i = 0;
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
						surf.AddLine(current, p1);
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						InsideSurface.BezierS1ToS2(current, ref p1, ref p2, p3);
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
						if ((elements[i] & PathElement.FlagClose) != 0)
						{
							surf.AddLine(current, start);
							closed = true;
						}
						i++;
						break;
				}
			}

			if (!closed)
			{
				surf.AddLine(current, start);
			}

			return surf.IsInside();
		}

		protected static void BezierS1ToS2(Point p1, ref Point s1, ref Point s2, Point p2)
		{
			//	Convertit une courbe de Bézier définie par un seul point secondaire en
			//	une courbe "traditionnelle" définie par deux points secondaires.
			//	Il s'agit ici d'une approximation empyrique !
			s1 = Point.Scale(p1, s1, 2.0/3.0);
			s2 = Point.Scale(p2, s2, 2.0/3.0);
		}

		
		public InsideSurface(Point p, int max)
		{
			//	Constructeur. Il faut donner le point dont on désire savoir s'il est
			//	dans la surface, ainsi que le nombre maximum de lignes qui seront
			//	ajoutées. Une courbe de Bézier compte pour InsideSurface.bezierStep.
			this.p = p;
			this.total = 0;
			this.list = new double[max+10];
		}

		public void AddLine(Point a, Point b)
		{
			//	Ajoute un segment de droite.
			Point i;
			if ( Point.IntersectsWithHorizontal(a,b, this.p.Y, out i) )
			{
				if ( a.Y == b.Y )  return;  // ligne horizontale ?
				if ( this.total < this.list.Length )
				{
					this.list[this.total++] = i.X;
				}
			}
		}

		public void AddBezier(Point p1, Point s1, Point s2, Point p2)
		{
			//	Ajoute un segment de Bézier.
			Point a = p1;
			double step = 1.0/InsideSurface.bezierStep;
			for ( double t=step ; t<1.0 ; t+=step )
			{
				Point b = Point.FromBezier(p1, s1, s2, p2, t);
				this.AddLine(a, b);
				a = b;
			}
			this.AddLine(a, p2);
		}

		public bool IsInside()
		{
			//	Indique si le point donné dans le constructeur est à l'intérieur de la surface.
			int nb = 0;
			for ( int i=0 ; i<this.total ; i++ )
			{
				if ( this.p.X < this.list[i] )  nb ++;
			}
			return ( nb%2 != 0 );  // magiqne, non ?
		}


		protected Point					p;
		protected int					total;
		protected double[]				list;
		public static readonly int		bezierStep = 10;
	}
}
