namespace Epsitec.Common.Drawing
{
	using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;
	
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (Point.Converter))]
	
	public struct Point
	{
		public Point(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
		
		public Point(System.Drawing.Point pt)
		{
			this.x = pt.X;
			this.y = pt.Y;
		}
		
		public Point(System.Drawing.PointF pt)
		{
			this.x = pt.X;
			this.y = pt.Y;
		}
		
		
		[XmlAttribute] public double	X
		{
			get { return this.x; }
			set { this.x = value; }
		}
		
		[XmlAttribute] public double	Y
		{
			get { return this.y; }
			set { this.y = value; }
		}
		
		
		public bool						IsEmpty
		{
			get { return this.x == 0 && this.y == 0; }
		}
		
		
		public static readonly Point 	Empty;
		
		public Size ToSize()
		{
			return new Size (this.x, this.y);
		}
		
		public Point GridAlign(double offset, double step)
		{
			return new Point (Point.GridAlign (this.X, offset, step), Point.GridAlign (this.Y, offset, step));
		}
		
		public Point GridAlign(Point offset, Point step)
		{
			return new Point (Point.GridAlign (this.X, offset.X, step.X), Point.GridAlign (this.Y, offset.Y, step.Y));
		}
		
		
		internal static double GridAlign(double value, double offset, double step)
		{
			// Met une valeur sur la grille la plus proche.
			if ( value < 0.0 )
			{
				return (double)((int)((value+offset-step/2.0)/step)*step)-offset;
			}
			else
			{
				return (double)((int)((value+offset+step/2.0)/step)*step)-offset;
			}
		}

		
		public override bool Equals(object obj)
		{
			if ((obj == null) &&
				(obj.GetType () != typeof (Point)))
			{
				return false;
			}
			
			Point p = (Point) obj;
			
			return (p.x == this.x) && (p.y == this.y);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		public override string ToString()
		{
			return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "[{0};{1}]", this.x, this.y);
		}
		
		
		public static Point Parse(string value, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return Point.Empty;
			}
			
			string[] args = value.Split (';', ':');
			
			if (args.Length != 2)
			{
				throw new System.ArgumentException (string.Format ("Invalid point specification ({0}).", value));
			}
			
			string arg_x = args[0].Trim ();
			string arg_y = args[1].Trim ();
			
			double x = System.Double.Parse (arg_x, culture);
			double y = System.Double.Parse (arg_y, culture);
			
			return new Point (x, y);
		}
		
		public static Point Parse(string value, System.Globalization.CultureInfo culture, Point default_value)
		{
			string[] args = value.Split (new char[] { ';', ':' });
			
			if (args.Length != 2)
			{
				throw new System.ArgumentException (string.Format ("Invalid point specification ({0}).", value));
			}
			
			string arg_x = args[0].Trim ();
			string arg_y = args[1].Trim ();
			
			if (arg_x != "*") default_value.X = System.Double.Parse (arg_x, culture);
			if (arg_y != "*") default_value.Y = System.Double.Parse (arg_y, culture);
			
			return default_value;
		}
		
		
		public static Point operator +(Point a, Point b)
		{
			return new Point (a.x + b.x, a.y + b.y);
		}
		
		public static Point operator +(Point a, Size b)
		{
			return new Point (a.x + b.Width, a.y + b.Height);
		}
		
		public static Point operator -(Point a, Point b)
		{
			return new Point (a.x - b.x, a.y - b.y);
		}
		
		public static Point operator -(Point a, Size b)
		{
			return new Point (a.x - b.Width, a.y - b.Height);
		}
		
		public static Point operator *(Point a, double value)
		{
			return new Point (a.x * value, a.y * value);
		}
		
		public static Point operator /(Point a, double value)
		{
			return new Point (a.x / value, a.y / value);
		}
		
		
		public static bool operator ==(Point a, Point b)
		{
			return (a.x == b.x) && (a.y == b.y);
		}
		
		public static bool operator !=(Point a, Point b)
		{
			return (a.x != b.x) || (a.y != b.y);
		}
		
		
		public static double ComputeAngle(Point c, Point a)
		{
			// Calcule l'angle d'un triangle rectangle.
			// L'angle est anti-horaire (CCW), compris entre 0 et 2*PI.
			// Pour obtenir un angle horaire (CW), il suffit de passer -y.
			//
			//      ^
			//      |
			//    y o----o
			//      |  / |
			//      |/)a |
			//  ----o----o-->
			//      |    x 
			//      |
			
			return Point.ComputeAngle(a.X-c.X, a.Y-c.Y);
		}

		public static double ComputeAngle(double x, double y)
		{
			if ( x == 0.0 && y == 0.0 )  return 0.0;
#if true
			return System.Math.Atan2 (y, x);
#else
			if ( x >= 0.0 )
			{
				if ( y >= 0.0 )
				{
					if ( x > y )  return System.Math.PI*0.0 + System.Math.Atan(y/x);
					else          return System.Math.PI*0.5 - System.Math.Atan(x/y);
				}
				else
				{
					if ( x > -y )  return System.Math.PI*2.0 + System.Math.Atan(y/x);
					else           return System.Math.PI*1.5 - System.Math.Atan(x/y);
				}
			}
			else
			{
				if ( y >= 0.0 )
				{
					if ( -x > y )  return System.Math.PI*1.0 + System.Math.Atan(y/x);
					else           return System.Math.PI*0.5 - System.Math.Atan(x/y);
				}
				else
				{
					if ( -x > -y )  return System.Math.PI*1.0 + System.Math.Atan(y/x);
					else            return System.Math.PI*1.5 - System.Math.Atan(x/y);
				}
			}
#endif
		}

		
		public static double Distance(Point a, Point b)
		{
			double dx = a.X - b.X;
			double dy = a.Y - b.Y;
			
			return System.Math.Sqrt (dx*dx + dy*dy);
		}
		
		public static Point Projection(Point a, Point b, Point p)
		{
			//	Calcule la projection d'un point P sur une droite AB.

			if ((a.X == b.X) && (a.Y == b.Y))
			{
				return a;
			}
			
			double k;
			
			k  = (b.X-a.X)*(p.X-a.X) + (b.Y-a.Y)*(p.Y-a.Y);
			k /= (b.X-a.X)*(b.X-a.X) + (b.Y-a.Y)*(b.Y-a.Y);
			
			return new Point (a.X + k*(b.X-a.X), a.Y + k*(b.Y-a.Y));
		}
		
		public static Point Scale(Point a, Point b, double scale)
		{
			//	Multiplie le vecteur AB par le facteur d'échelle.
			//	Retourne la nouvelle extrémité B.
			
			return new Point (a.X + (b.X-a.X)*scale, a.Y + (b.Y-a.Y)*scale);
		}
		
		public static Point Move(Point a, Point b, double distance)
		{
			//	Avance d'une certaine distance le long d'une droite AB.
			
			double length = Point.Distance (a, b);
			double scale  = (length == 0) ? 0 : distance / length;
			
			return Point.Scale (a, b, scale);
		}

		public static Point Symmetry(Point c, Point a)
		{
			//	Calcule le point A' symétrique de A par rapport au centre C.
			
			return new Point (c.X-(a.X-c.X), c.Y-(a.Y-c.Y));
		}


		public static bool Detect(Point a, Point b, Point p, double width)
		{
			//	Détecte si le point P est sur un segment AB d'épaisseur 'width'.
			
			Rectangle rect = new Rectangle ();
			
			rect.Left   = System.Math.Min (a.X, b.X) - width;
			rect.Right  = System.Math.Max (a.X, b.X) + width;
			rect.Bottom = System.Math.Min (a.Y, b.Y) - width;
			rect.Top    = System.Math.Max (a.Y, b.Y) + width;
			
			if (rect.Contains (p))
			{
				Point  proj = Point.Projection (a, b, p);
				double dist = Point.Distance (p, proj);
				
				return dist <= width;
			}
			
			return false;
		}

		public static bool Detect(Point p1, Point s1, Point s2, Point p2, Point p, double width)
		{
			//	Détecte si le point P est sur un segment de Bezier d'épaisseur 'width'.
			
			int max_step = 10;		//	nombre d'étapes arbitraire fixé à 10
			
			Point  a = p1;
			double t = 0;
			double dt = 1.0 / max_step;
			
			for (int step = 1; step <= max_step; step++)
			{
				t += dt;
				
				Point b = Point.Bezier (p1, s1, s2, p2, t);
				
				if (Point.Detect (a, b, p, width))
				{
					return true;
				}
				
				a = b;
			}
			
			return false;
		}

		public static Point Bezier(Point p1, Point s1, Point s2, Point p2, double t)
		{
			//	Calcule un point sur une courbe de Bézier, en fonction du paramètre t (0..1).
			//	Si t=0, on est sur p1.
			//	Si t=1, on est sur p2.
			
			double t1 = (1-t)*(1-t)*(1-t);
			double t2 = (1-t)*(1-t)*t*3;
			double t3 = (1-t)*t*t*3;
			double t4 = t*t*t;
			
			return new Point (p1.X*t1 + s1.X*t2 + s2.X*t3 + p2.X*t4,
							  p1.Y*t1 + s1.Y*t2 + s2.Y*t3 + p2.Y*t4);
		}

		public static double Bezier(Point p1, Point s1, Point s2, Point p2, Point p)
		{
			//	Cherche la valeur de t (0..1) correspondant le mieux possible au nouveau point.
			//	Il n'est pas obligatoire que le nouveau point soit sur la courbe
			//	(algorithme = distance la plus courte).
			
			int max_step = 1000;	//	nombre d'étapes arbitraire fixé à 1000
			
			double t = 0;
			double dt = 1.0 / max_step;
			
			double best_t = 0;
			double min    = 1000000;
			
			for (int step = 1; step < max_step; step++)
			{
				t += dt;
				
				Point  b = Point.Bezier (p1, s1, s2, p2, t);
				double d = Point.Distance (b, p);  // d <- distance jusqu'au point
				
				if (d < min)
				{
					min    = d;		// min <- distance minimale
					best_t = t;		// t   <- valeur correspondante
				}
			}
			
			return best_t;
		}

		public static bool Intersect(Point a, Point b, double y, out Point i)
		{
			//	Calcule l'intersection I d'une droite AB avec une horizontale Y.
			
			i = a;
			
			if ((y < System.Math.Min (a.Y, b.Y)) ||
				(y >= System.Math.Max (a.Y, b.Y)))
			{
				return false;
			}
			
			if (a.Y == b.Y)
			{
				return true;
			}
			
			i.X = a.X + (y-a.Y)*((b.X-a.X)/(b.Y-a.Y));
			i.Y = y;
			
			return true;
		}
		
		
		
		public class Converter : Epsitec.Common.Converters.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return Point.Parse (value, culture);
			}
			
			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				Point point = (Point) value;
				return string.Format ("{0};{1}", point.X, point.Y);
			}
		}
		
		
		private double					x, y;
	}
}
