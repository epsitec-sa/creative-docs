namespace Epsitec.Common.Drawing
{
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
		
		public double					X
		{
			get { return this.x; }
			set { this.x = value; }
		}
		
		public double					Y
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
			return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1}", this.x, this.y);
		}
		
		
		public static Point Parse(string value)
		{
			if (value == null)
			{
				return Point.Empty;
			}
			
			string[] args = value.Split (';', ':');
			
			if (args.Length != 2)
			{
				throw new System.ArgumentException (string.Format ("Invalid point specification ({0})", value));
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
				throw new System.ArgumentException (string.Format ("Invalid point specification ({0})", value));
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
		
		public static Point operator -(Point a, Size b)
		{
			return new Point (a.x - b.Width, a.y - b.Height);
		}
		
		
		public static bool operator ==(Point a, Point b)
		{
			return (a.x == b.x) && (a.y == b.y);
		}
		
		public static bool operator !=(Point a, Point b)
		{
			return (a.x != b.x) || (a.y != b.y);
		}
		
		
		private double					x, y;
	}
}
