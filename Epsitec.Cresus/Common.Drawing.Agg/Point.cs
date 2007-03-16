//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		
		[XmlAttribute] public double			X
		{
			get { return this.x; }
			set { this.x = value; }
		}
		
		[XmlAttribute] public double			Y
		{
			get { return this.y; }
			set { this.y = value; }
		}
		
		
		public bool								IsZero
		{
			get { return this.x == 0 && this.y == 0; }
		}
		
		
		public static readonly Point 			Zero;
		
		public Size ToSize()
		{
			return new Size (this.x, this.y);
		}
		
		
		public static Point GridAlign(Point point, double offset, double step)
		{
			return new Point (Point.GridAlign (point.X, offset, step), Point.GridAlign (point.Y, offset, step));
		}
		
		public static Point GridAlign(Point point, Point offset, Point step)
		{
			return new Point (Point.GridAlign (point.X, offset.X, step.X), Point.GridAlign (point.Y, offset.Y, step.Y));
		}
		
		
		internal static double GridAlign(double value, double offset, double step)
		{
			//	Met une valeur sur la grille la plus proche.
			if (value+offset < 0.0)
			{
				return (double)((int)((value+offset-step/2.0)/step)*step)-offset;
			}
			else
			{
				return (double)((int)((value+offset+step/2.0)/step)*step)-offset;
			}
		}

		
		public override string ToString()
		{
			return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1}", this.x, this.y);
		}
		
		
		public override bool Equals(object obj)
		{
			return (obj is Point) && (this == (Point) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		
		public static Point Parse(string value)
		{
			if (value == null)
			{
				return Point.Zero;
			}
			
			string[] args = value.Split (';', ':');
			
			if (args.Length != 2)
			{
				throw new System.ArgumentException (string.Format ("Invalid point specification ({0}).", value));
			}
			
			string arg_x = args[0].Trim ();
			string arg_y = args[1].Trim ();
			
			double x = System.Double.Parse (arg_x, System.Globalization.CultureInfo.InvariantCulture);
			double y = System.Double.Parse (arg_y, System.Globalization.CultureInfo.InvariantCulture);
			
			return new Point (x, y);
		}
		
		public static Point Parse(string value, Point default_value)
		{
			string[] args = value.Split (new char[] { ';', ':' });
			
			if (args.Length != 2)
			{
				throw new System.ArgumentException (string.Format ("Invalid point specification ({0}).", value));
			}
			
			string arg_x = args[0].Trim ();
			string arg_y = args[1].Trim ();
			
			if (arg_x != "*") default_value.X = System.Double.Parse (arg_x, System.Globalization.CultureInfo.InvariantCulture);
			if (arg_y != "*") default_value.Y = System.Double.Parse (arg_y, System.Globalization.CultureInfo.InvariantCulture);
			
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
		
		public static Point operator -(Point a)
		{
			return new Point (-a.x, -a.y);
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

		public static Point ScaleMul(Point a, Point b)
		{
			return new Point (a.x * b.x, a.y * b.y);
		}
		
		public static Point ScaleDiv(Point a, Point b)
		{
			return new Point (a.x / b.x, a.y / b.y);
		}
		
		
		public static double ComputeAngleRad(Point c, Point a)
		{
			return Point.ComputeAngleRad(a.X-c.X, a.Y-c.Y);
		}

		public static double ComputeAngleRad(double x, double y)
		{
			//	Calcule l'angle d'un triangle rectangle.
			//	L'angle est anti-horaire (CCW), compris entre 0 et 2*PI.
			//	Pour obtenir un angle horaire (CW), il suffit de passer -y.
			//
			//	    ^
			//	    |
			//	  y o----o
			//	    |  / |
			//	    |/)a |
			//	----o----o-->
			//	    |    x 
			//	    |
			
			if ((x == 0.0) && (y == 0.0))
			{
				return 0.0;
			}
			
			return System.Math.Atan2 (y, x);
		}
		
		public static double ComputeAngleDeg(Point c, Point a)
		{
			return Math.RadToDeg (Point.ComputeAngleRad (c, a));
		}

		public static double ComputeAngleDeg(double x, double y)
		{
			return Math.RadToDeg (Point.ComputeAngleRad (x, y));
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
			//	Multiplie le vecteur AB par le facteur d'�chelle.
			//	Retourne la nouvelle extr�mit� B.
			
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
			//	Calcule le point A' sym�trique de A par rapport au centre C.
			
			return new Point (c.X-(a.X-c.X), c.Y-(a.Y-c.Y));
		}


		public static bool DetectSegment(Point a, Point b, Point p, double width)
		{
			//	D�tecte si le point P est sur un segment AB d'�paisseur 'width'.
			
			double x_left   = System.Math.Min (a.X, b.X) - width;
			double x_right  = System.Math.Max (a.X, b.X) + width;
			double y_bottom = System.Math.Min (a.Y, b.Y) - width;
			double y_top    = System.Math.Max (a.Y, b.Y) + width;
			
			Rectangle rect = new Rectangle (x_left, y_bottom, x_right - x_left, y_top - y_bottom);
			
			if (rect.Contains (p))
			{
				Point  proj = Point.Projection (a, b, p);
				double dist = Point.Distance (p, proj);
				
				return dist <= width;
			}
			
			return false;
		}

		public static bool DetectBezier(Point p1, Point s1, Point s2, Point p2, Point p, double width)
		{
			//	D�tecte si le point P est sur un segment de Bezier d'�paisseur 'width'.
			
			int max_step = 10;		//	nombre d'�tapes arbitraire fix� � 10
			
			Point  a = p1;
			double t = 0;
			double dt = 1.0 / max_step;
			
			for (int step = 1; step <= max_step; step++)
			{
				t += dt;
				
				Point b = Point.FromBezier (p1, s1, s2, p2, t);
				
				if (Point.DetectSegment (a, b, p, width))
				{
					return true;
				}
				
				a = b;
			}
			
			return false;
		}

		public static Point FromBezier(Point p1, Point s1, Point s2, Point p2, double t)
		{
			//	Calcule un point sur une courbe de B�zier, en fonction du param�tre t (0..1).
			//	Si t=0, on est sur p1.
			//	Si t=1, on est sur p2.
			
			double t1 = (1-t)*(1-t)*(1-t);
			double t2 = (1-t)*(1-t)*t*3;
			double t3 = (1-t)*t*t*3;
			double t4 = t*t*t;
			
			return new Point (p1.X*t1 + s1.X*t2 + s2.X*t3 + p2.X*t4,
							  p1.Y*t1 + s1.Y*t2 + s2.Y*t3 + p2.Y*t4);
		}

		public static double FindBezierParameter(Point p1, Point s1, Point s2, Point p2, Point p)
		{
			//	Cherche la valeur de t (0..1) correspondant le mieux possible au nouveau point.
			//	Il n'est pas obligatoire que le nouveau point soit sur la courbe
			//	(algorithme = distance la plus courte).
			
			int max_step = 1000;	//	nombre d'�tapes arbitraire fix� � 1000
			
			double t = 0;
			double dt = 1.0 / max_step;
			
			double best_t = 0;
			double min    = 1000000;
			
			for (int step = 1; step < max_step; step++)
			{
				t += dt;
				
				Point  b = Point.FromBezier (p1, s1, s2, p2, t);
				double d = Point.Distance (b, p);  // d <- distance jusqu'au point
				
				if (d < min)
				{
					min    = d;		// min <- distance minimale
					best_t = t;		// t   <- valeur correspondante
				}
			}
			
			return best_t;
		}

		
		public static bool IntersectsWithHorizontal(Point a, Point b, double y, out Point i)
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
		
		public static bool IntersectsWithVertical(Point a, Point b, double x, out Point i)
		{
			//	Calcule l'intersection I d'une droite AB avec une verticale X.
			
			i = a;
			
			if ((x < System.Math.Min (a.X, b.X)) ||
				(x >= System.Math.Max (a.X, b.X)))
			{
				return false;
			}
			
			if (a.X == b.X)
			{
				return true;
			}
			
			i.X = x;
			i.Y = a.Y + (x-a.X)*((b.Y-a.Y)/(b.X-a.X));
			
			return true;
		}
		
		
		#region Converter Class
		public class Converter : AbstractStringConverter
		{
			public override object ParseString(string value)
			{
				return Point.Parse (value);
			}
			
			public override string ToString(object value)
			{
				Point point = (Point) value;
				return point.ToString ();
			}
			
			public static string ToString(object value, bool suppress_x, bool suppress_y)
			{
				Point point = (Point) value;
				
				string arg1 = suppress_x ? "*" : point.X.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string arg2 = suppress_y ? "*" : point.Y.ToString (System.Globalization.CultureInfo.InvariantCulture);
				
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1}", arg1, arg2);
			}
		}
		#endregion
		
		private double							x;
		private double							y;
	}
}
