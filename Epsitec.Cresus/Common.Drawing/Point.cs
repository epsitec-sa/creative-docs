namespace Epsitec.Common.Drawing
{
	public struct Point
	{
		public Point(double x, double y)
		{
			this.x = x;
			this.y = y;
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
			return System.String.Format ("{{X={0}, Y={1}}}", this.x, this.y);
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
