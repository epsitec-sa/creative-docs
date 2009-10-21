//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin
{
	/// <summary>
	/// La classe Geometry contient quelques routines générales.
	/// </summary>
	public class Geometry
	{
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

		public static void BezierS1ToS2(Point p1, ref Point s1, ref Point s2, Point p2)
		{
			//	Convertit une courbe de Bézier définie par un seul point secondaire en
			//	une courbe "traditionnelle" définie par deux points secondaires.
			//	Il s'agit ici d'une approximation empyrique !
			s1 = Point.Scale(p1, s1, 2.0/3.0);
			s2 = Point.Scale(p2, s2, 2.0/3.0);
		}

		
		public static void RenderHorizontalGradient(Graphics graphics, Rectangle rect, Color leftColor, Color rightColor)
		{
			//	Peint la surface avec un dégradé horizontal.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.X;
			graphics.GradientRenderer.SetColors(leftColor, rightColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale(rect.Width/100/2, rect.Height/100/2);
			t = t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		public static void RenderVerticalGradient(Graphics graphics, Rectangle rect, Color bottomColor, Color topColor)
		{
			//	Peint la surface avec un dégradé vertical.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors(bottomColor, topColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale(rect.Width/100/2, rect.Height/100/2);
			t = t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		public static void RenderVerticalGradient(Graphics graphics, Rectangle rect, Color bottomColor, Color topColor, int repeat, double middle)
		{
			//	Peint la surface avec un dégradé vertical.
			Color c1 = topColor;
			Color c2 = bottomColor;

			double[] r = new double[256];
			double[] g = new double[256];
			double[] b = new double[256];
			double[] a = new double[256];

			for ( int i=0 ; i<256 ; i++ )
			{
				double factor = Geometry.GradientRepeatFactor(i/255.0, repeat, middle);
				r[i] = c1.R + (c2.R-c1.R)*factor;
				g[i] = c1.G + (c2.G-c1.G)*factor;
				b[i] = c1.B + (c2.B-c1.B)*factor;
				a[i] = c1.A + (c2.A-c1.A)*factor;
			}

			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors(r, g, b, a);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale(rect.Width/100/2, rect.Height/100/2);
			t = t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected static double GradientRepeatFactor(double progress, int repeat, double middle)
		{
			//	Calcule le facteur de progression dans la couleur [0..1].
			//	Si M>0:  P=1-(1-P)^(1+M)
			//	Si M<0:  P=P^(1-M)
			if (repeat > 1)
			{
				int i = (int) (progress*repeat);
				progress = (progress*repeat)%1.0;
				if (i%2 != 0)  progress = 1.0-progress;
			}

			if (middle != 0)
			{
				if (middle > 0.0)
				{
					progress = 1.0-System.Math.Pow(1.0-progress, 1.0+middle);
				}
				else
				{
					progress = System.Math.Pow(progress, 1.0-middle);
				}
			}

			return progress;
		}

		public static void RenderCircularGradient(Graphics graphics, Point center, double radius, Color extColor, Color intColor)
		{
			//	Peint la surface avec un dégradé circulaire.
			Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);

			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Circle;
			graphics.GradientRenderer.SetColors(intColor, extColor);
			graphics.GradientRenderer.SetParameters(0, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = Transform.Identity;
			t = t.Scale(rect.Width/100/2, rect.Height/100/2);
			t = t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}
	}
}
