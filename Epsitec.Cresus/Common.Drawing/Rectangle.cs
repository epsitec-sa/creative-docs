namespace Epsitec.Common.Drawing
{
	using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;
	using XmlIgnore    = System.Xml.Serialization.XmlIgnoreAttribute;
	
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (Rectangle.Converter))]
	
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
			this.y2 = y + height;
		}
		
		public Rectangle(System.Drawing.RectangleF r)
		{
			this.x1 = r.Left;
			this.y1 = r.Bottom;
			this.x2 = r.Right;
			this.y2 = r.Top;
		}
		
		public Rectangle(System.Drawing.Rectangle r)
		{
			this.x1 = r.Left;
			this.y1 = r.Bottom;
			this.x2 = r.Right;
			this.y2 = r.Top;
		}
		
		
		public static readonly Rectangle Empty;
		public static readonly Rectangle Infinite = new Rectangle (- 1000000000, -1000000000, 2000000000, 2000000000);
		
		public bool						IsEmpty
		{
			get { return (this.x2 <= this.x1) || (this.y2 <= this.y1); }
		}
		
		public bool						IsValid
		{
			get { return (this.x2 > this.x1) && (this.y2 > this.y1); }
		}
		
		
		[XmlIgnore] public Point		Location
		{
			get { return new Point (this.x1, this.y1); }
			set { double dx = this.Width; double dy = this.Height; this.x1 = value.X; this.y1 = value.Y; this.x2 = this.x1 + dx; this.y2 = this.y1 + dy; }
		}

		[XmlIgnore] public Size			Size
		{
			get { return new Size (this.x2 - this.x1, this.y2 - this.y1); }
			set { this.x2 = this.x1 + value.Width; this.y2 = this.y1 + value.Height; }
		}
		
		
		[XmlAttribute] public double	X
		{
			get { return this.x1; }
			set { double dx = this.Width; this.x1 = value; this.x2 = value + dx; }
		}
		
		[XmlAttribute] public double	Y
		{
			get { return this.y1; }
			set { double dy = this.Height; this.y1 = value; this.y2 = value + dy; }
		}
		
		[XmlAttribute] public double	Width
		{
			get { return this.x2 - this.x1; }
			set { this.x2 = this.x1 + value; }
		}
		
		[XmlAttribute] public double	Height
		{
			get { return this.y2 - this.y1; }
			set { this.y2 = this.y1 + value; }
		}
		
		
		[XmlIgnore] public double		Left
		{
			get { return this.x1; }
			set { this.x1 = value; }
		}
		
		[XmlIgnore] public double		Right
		{
			get { return this.x2; }
			set { this.x2 = value; }
		}
		
		[XmlIgnore] public double		Bottom
		{
			get { return this.y1; }
			set { this.y1 = value; }
		}
		
		[XmlIgnore] public double		Top
		{
			get { return this.y2; }
			set { this.y2 = value; }
		}
		
		
		public Point					Center
		{
			get { return new Point ((this.x1 + this.x2) / 2, (this.y1 + this.y2) / 2); }
		}
		
		
		public bool Contains(Point p)
		{
			return this.Contains (p.X, p.Y);
		}
		
		public bool Contains(double x, double y)
		{
			return (!this.IsEmpty) && (this.x1 <= x) && (this.x2 > x) && (this.y1 <= y) && (this.y2 > y);
		}
		
		public bool Contains(Rectangle r)
		{
			return (!r.IsEmpty) && (!this.IsEmpty) && (r.x1 >= this.x1) && (r.x2 <= this.x2) && (r.y1 >= this.y1) && (r.y2 <= this.y2);
		}
		
		
		public bool IntersectsWith(Rectangle r)
		{
			return (!r.IsEmpty) && (!this.IsEmpty) && (this.x1 < r.x2) && (this.x2 > r.x1) && (this.y1 < r.y2) && (this.y2 > r.y1);
		}
		
		public bool IntersectsWithAligned(Rectangle r)
		{
			return (!r.IsEmpty)
				&& (!this.IsEmpty)
				&& (System.Math.Floor (this.x1) < System.Math.Ceiling (r.x2))
				&& (System.Math.Ceiling (this.x2) > System.Math.Floor (r.x1))
				&& (System.Math.Floor (this.y1) < System.Math.Ceiling (r.y2))
				&& (System.Math.Ceiling (this.y2) > System.Math.Floor (r.y1));
		}
		
		
		public void Inflate(Size s)
		{
			this.Inflate (s.Width, s.Height);
		}
		
		public void Inflate(double x, double y)
		{
			if (! this.IsEmpty)
			{
				this.x1 -= x;
				this.x2 += x;
				this.y1 -= y;
				this.y2 += y;
			}
		}
		
		public void Inflate(Margins margins)
		{
			if (! this.IsEmpty)
			{
				this.x1 -= margins.Left;
				this.x2 += margins.Right;
				this.y1 -= margins.Bottom;
				this.y2 += margins.Top;
			}
		}
		
		public void Offset(Point p)
		{
			this.Offset (p.X, p.Y);
		}
		
		public void Offset(double x, double y)
		{
			if (! this.IsEmpty)
			{
				this.x1 += x;
				this.y1 += y;
				this.x2 += x;
				this.y2 += y;
			}
		}
		
		public void Scale(double s)
		{
			this.x1 *= s;
			this.y1 *= s;
			this.x2 *= s;
			this.y2 *= s;
		}
		
		public void Scale(double sx, double sy)
		{
			this.x1 *= sx;
			this.y1 *= sy;
			this.x2 *= sx;
			this.y2 *= sy;
		}
		
		public void MergeWith(Rectangle r)
		{
			if (this.IsEmpty)
			{
				this.x1 = r.x1;
				this.y1 = r.y1;
				this.x2 = r.x2;
				this.y2 = r.y2;
			}
			else
			{
				double x1 = System.Math.Min (this.x1, r.x1);
				double y1 = System.Math.Min (this.y1, r.y1);
				double x2 = System.Math.Max (this.x2, r.x2);
				double y2 = System.Math.Max (this.y2, r.y2);
			
				this.x1 = x1;
				this.y1 = y1;
				this.x2 = x2;
				this.y2 = y2;
			}
		}
		
		public void MergeWith(Point p)
		{
			if (this.IsEmpty)
			{
				this.x1 = p.X;
				this.y1 = p.Y;
				this.x2 = p.X + 0.000001;
				this.y2 = p.Y + 0.000001;
			}
			else
			{
				this.x1 = System.Math.Min (this.x1, p.X);
				this.y1 = System.Math.Min (this.y1, p.Y);
				this.x2 = System.Math.Max (this.x2, p.X);
				this.y2 = System.Math.Max (this.y2, p.Y);
			}
		}
		
		public void Normalise()
		{
			double x1 = System.Math.Min (this.x1, this.x2);
			double x2 = System.Math.Max (this.x1, this.x2);
			double y1 = System.Math.Min (this.y1, this.y2);
			double y2 = System.Math.Max (this.y1, this.y2);
			
			this.x1 = x1;
			this.x2 = x2;
			this.y1 = y1;
			this.y2 = y2;
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
			if (r.IsEmpty)
			{
				return Rectangle.Empty;
			}
			
			r.Inflate (x, y);
			return r;
		}
		
		public static Rectangle Inflate(Rectangle r, Margins margins)
		{
			if (r.IsEmpty)
			{
				return Rectangle.Empty;
			}
			
			r.Inflate (margins);
			return r;
		}
		
		public static Rectangle Offset(Rectangle r, Point p)
		{
			if (r.IsEmpty)
			{
				return Rectangle.Empty;
			}
			
			r.Offset (p);
			return r;
		}
		
		public static Rectangle Offset(Rectangle r, double x, double y)
		{
			if (r.IsEmpty)
			{
				return Rectangle.Empty;
			}
			
			r.Offset (x, y);
			return r;
		}
		
		public static Rectangle Union(Rectangle a, Rectangle b)
		{
			if (a.IsEmpty)
			{
				return b;
			}
			if (b.IsEmpty)
			{
				return a;
			}
			
			double x1 = System.Math.Min (a.x1, b.x1);
			double x2 = System.Math.Max (a.x2, b.x2);
			double y1 = System.Math.Min (a.y1, b.y1);
			double y2 = System.Math.Max (a.y2, b.y2);
			
			return ((x1 >= x2) || (y1 >= y2)) ? Rectangle.Empty : Rectangle.FromCorners (x1, y1, x2, y2);
		}
		
		public static Rectangle Intersection(Rectangle a, Rectangle b)
		{
			if (a.IsEmpty || b.IsEmpty)
			{
				return Rectangle.Empty;
			}
			
			double x1 = System.Math.Max (a.x1, b.x1);
			double x2 = System.Math.Min (a.x2, b.x2);
			double y1 = System.Math.Max (a.y1, b.y1);
			double y2 = System.Math.Min (a.y2, b.y2);
			
			return ((x1 >= x2) || (y1 >= y2)) ? Rectangle.Empty : Rectangle.FromCorners (x1, y1, x2, y2);
		}

		
		public static Rectangle Parse(string value, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return Rectangle.Empty;
			}
			
			string[] args = value.Split (';', ':');
			
			if (args.Length != 4)
			{
				throw new System.ArgumentException (string.Format ("Invalid rectangle specification ({0}).", value));
			}
			
			string arg_x  = args[0].Trim ();
			string arg_y  = args[1].Trim ();
			string arg_dx = args[2].Trim ();
			string arg_dy = args[3].Trim ();
			
			double x  = System.Double.Parse (arg_x, culture);
			double y  = System.Double.Parse (arg_y, culture);
			double dx = System.Double.Parse (arg_dx, culture);
			double dy = System.Double.Parse (arg_dy, culture);
			
			return new Rectangle (x, y, dx, dy);
		}
		
		public static Rectangle Parse(string value, System.Globalization.CultureInfo culture, Rectangle default_value)
		{
			string[] args = value.Split (new char[] { ';', ':' });
			
			if (args.Length != 4)
			{
				throw new System.ArgumentException (string.Format ("Invalid rectangle specification ({0}).", value));
			}
			
			string arg_x  = args[0].Trim ();
			string arg_y  = args[1].Trim ();
			string arg_dx = args[2].Trim ();
			string arg_dy = args[3].Trim ();
			
			if (arg_x  != "*") default_value.X      = System.Double.Parse (arg_x, culture);
			if (arg_y  != "*") default_value.Y      = System.Double.Parse (arg_y, culture);
			if (arg_dx != "*") default_value.Width  = System.Double.Parse (arg_dx, culture);
			if (arg_dy != "*") default_value.Height = System.Double.Parse (arg_dy, culture);
			
			return default_value;
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
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "[{0};{1};{2};{3}]",
								  this.X, this.Y, this.Width, this.Height);
		}

		
		public class Converter : Epsitec.Common.Converters.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return Rectangle.Parse (value, culture);
			}
			
			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				Rectangle rect = (Rectangle) value;
				return string.Format ("{0};{1};{2};{3}", rect.X, rect.Y, rect.Width, rect.Height);
			}
		}
		
		
		private double					x1, y1;
		private double					x2, y2;
	}
}
