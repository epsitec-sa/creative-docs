namespace Epsitec.Common.Drawing
{
	public struct Rectangle
	{
		public Rectangle(Point p, Size s)
		{
			this.x1 = p.X;
			this.y1 = p.Y;
			this.x2 = this.x1 + s.Width;
			this.y2 = this.y1 + s.Height;
		}
		
		public Rectangle(double x, double y, double width, double height)
		{
			this.x1 = x;
			this.y1 = y;
			this.x2 = x + width;
			this.y2 = y + width;
		}
		
		public Rectangle(System.Drawing.RectangleF r)
		{
			this.x1 = r.Left;
			this.y1 = r.Top;
			this.x2 = r.Right;
			this.y2 = r.Bottom;
		}
		
		public Rectangle(System.Drawing.Rectangle r)
		{
			this.x1 = r.Left;
			this.y1 = r.Top;
			this.x2 = r.Right;
			this.y2 = r.Bottom;
		}
		
		
		public static readonly Rectangle Empty;
		
		public bool						IsEmpty
		{
			get { return (this.x2 <= this.x1) || (this.y2 <= this.y1); }
		}
		
		public Point					Location
		{
			get { return new Point (this.x1, this.y1); }
			set { double dx = this.Width; double dy = this.Height; this.x1 = value.X; this.y1 = value.Y; this.x2 = this.x1 + dx; this.y2 = this.y1 + dy; }
		}

		public Size						Size
		{
			get { return new Size (this.x2 - this.x1, this.y2 - this.y1); }
			set { this.x2 = this.x1 + value.Width; this.y2 = this.y1 + value.Height; }
		}
		
		public double					X
		{
			get { return this.x1; }
			set { double dx = this.Width; this.x1 = value; this.x2 = value + dx; }
		}
		
		public double					Y
		{
			get { return this.y1; }
			set { double dy = this.Height; this.y1 = value; this.y2 = value + dy; }
		}
		
		public double					Width
		{
			get { return this.x2 - this.x1; }
			set { this.x2 = this.x1 + value; }
		}
		
		public double					Height
		{
			get { return this.y2 - this.y1; }
			set { this.y2 = this.y1 + value; }
		}
		
		public double					Left
		{
			get { return this.x1; }
			set { this.x1 = value; }
		}
		
		public double					Right
		{
			get { return this.x2; }
			set { this.x2 = value; }
		}
		
		public double					Top
		{
			get { return this.y1; }
			set { this.y1 = value; }
		}
		
		public double					Bottom
		{
			get { return this.y2; }
			set { this.y2 = value; }
		}

		
		
		public bool Contains(Point p)
		{
			return this.Contains (p.X, p.Y);
		}
		
		public bool Contains(double x, double y)
		{
			return (this.x1 <= x) && (this.x2 > x) && (this.y1 <= y) && (this.y2 > y);
		}
		
		public bool Contains(Rectangle r)
		{
			return (r.x1 >= this.x1) && (r.x2 <= this.x2) && (r.y1 >= this.y1) && (r.y2 <= this.y2);
		}
		
		
		public bool IntersectsWith(Rectangle r)
		{
			return (this.x1 < r.x2) && (this.x2 > r.x1) && (this.y1 < r.y2) && (this.y2 > r.y1);
		}
		
		
		public void Inflate(Size s)
		{
			this.Inflate (s.Width, s.Height);
		}
		
		public void Inflate(double x, double y)
		{
			this.x1 -= x;
			this.x2 += x;
			this.y1 -= y;
			this.y2 += y;
		}
		
		
		public void Offset(Point p)
		{
			this.Offset (p.X, p.Y);
		}
		
		public void Offset(double x, double y)
		{
			this.x1 += x;
			this.y1 += y;
			this.x2 += x;
			this.y2 += y;
		}
		
		
		public static Rectangle FromCorners(double x1, double y1, double x2, double y2)
		{
			Rectangle r;
			
			r.x1 = x1;
			r.y1 = y1;
			r.x2 = x2;
			r.y2 = y2;
			
			return r;
		}
		
		
		public static Rectangle Inflate(Rectangle r, double x, double y)
		{
			r.Inflate (x, y);
			return r;
		}
		
		public static Rectangle Union(Rectangle a, Rectangle b)
		{
			double x1 = System.Math.Min (a.x1, b.x1);
			double x2 = System.Math.Max (a.x2, b.x2);
			double y1 = System.Math.Min (a.y1, b.y1);
			double y2 = System.Math.Max (a.y2, b.y2);
			
			return ((x1 >= x2) || (y1 >= y2)) ? Rectangle.Empty : Rectangle.FromCorners (x1, y1, x2, y2);
		}
		
		public static Rectangle Intersection(Rectangle a, Rectangle b)
		{
			double x1 = System.Math.Max (a.x1, b.x1);
			double x2 = System.Math.Min (a.x2, b.x2);
			double y1 = System.Math.Max (a.y1, b.y1);
			double y2 = System.Math.Min (a.y2, b.y2);
			
			return ((x1 >= x2) || (y1 >= y2)) ? Rectangle.Empty : Rectangle.FromCorners (x1, y1, x2, y2);
		}

		
		public static bool operator ==(Rectangle a, Rectangle b)
		{
			return (a.x1 == b.x1) && (a.x2 == b.x2) && (a.y1 == b.y1) && (a.y2 == b.y2);
		}
		
		public static bool operator !=(Rectangle a, Rectangle b)
		{
			return (a.x1 != b.x1) || (a.x2 != b.x2) || (a.y1 != b.y1) || (a.y2 != b.y2);
		}
		
		
		public override bool Equals(object obj)
		{
			if ((obj == null) &&
				(obj.GetType () != typeof (Rectangle)))
			{
				return false;
			}
			
			Rectangle r = (Rectangle) obj;
			
			return (r.x1 == this.x1) && (r.x2 == this.x2) && (r.y1 == this.y1) && (r.y2 == this.y2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("{X=");
			buffer.Append (this.x1.ToString ());
			buffer.Append (",Y=");
			buffer.Append (this.y1.ToString ());
			buffer.Append (",Width=");
			buffer.Append (this.Width.ToString ());
			buffer.Append (",Height=");
			buffer.Append (this.Height.ToString ());
			buffer.Append ("}");
			
			return buffer.ToString ();
		}

		
		
		private double					x1, y1;
		private double					x2, y2;
	}
}
