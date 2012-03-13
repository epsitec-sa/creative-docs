//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		
		public Rectangle(Point p1, Point p2)
		{
			this.x1 = System.Math.Min (p1.X, p2.X);
			this.y1 = System.Math.Min (p1.Y, p2.Y);
			this.x2 = System.Math.Max (p1.X, p2.X);
			this.y2 = System.Math.Max (p1.Y, p2.Y);
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

		public static readonly Rectangle		Zero     = new Rectangle (0, 0, 0, 0);
		public static readonly Rectangle		Empty    = new Rectangle (0, 0, -1, -1);
		public static readonly Rectangle		MaxValue = new Rectangle (-1000000000, -1000000000, 2000000000, 2000000000);
		
		public bool								IsEmpty
		{
			get { return (this.x2 < this.x1) || (this.y2 < this.y1); }
		}
		
		public bool								IsInfinite
		{
			get
			{
				return (this.x1 <= -1000000000) && (this.x2 >= 1000000000) && (this.y1 <= -1000000000) && (this.y2 >= 1000000000);
			}
		}
		
		public bool								IsSurfaceZero
		{
			get { return (this.x2 <= this.x1) || (this.y2 <= this.y1); }
		}
		
		public bool								IsValid
		{
			get { return (this.x2 >= this.x1) && (this.y2 >= this.y1); }
		}
		
		
		[XmlIgnore] public Point				Location
		{
			get { return new Point (this.x1, this.y1); }
			set { double dx = this.Width; double dy = this.Height; this.x1 = value.X; this.y1 = value.Y; this.x2 = this.x1 + dx; this.y2 = this.y1 + dy; }
		}

		[XmlIgnore] public Size					Size
		{
			get { return new Size (this.x2 - this.x1, this.y2 - this.y1); }
			set { this.x2 = this.x1 + value.Width; this.y2 = this.y1 + value.Height; }
		}
		
		
		[XmlAttribute] public double			X
		{
			get { return this.x1; }
			set { double dx = this.Width; this.x1 = value; this.x2 = value + dx; }
		}
		
		[XmlAttribute] public double			Y
		{
			get { return this.y1; }
			set { double dy = this.Height; this.y1 = value; this.y2 = value + dy; }
		}
		
		[XmlAttribute] public double			Width
		{
			get { return this.x2 - this.x1; }
			set { this.x2 = this.x1 + value; }
		}
		
		[XmlAttribute] public double			Height
		{
			get { return this.y2 - this.y1; }
			set { this.y2 = this.y1 + value; }
		}
		
		
		[XmlIgnore] public double				Left
		{
			get { return this.x1; }
			set { this.x1 = value; }
		}
		
		[XmlIgnore] public double				Right
		{
			get { return this.x2; }
			set { this.x2 = value; }
		}
		
		[XmlIgnore] public double				Bottom
		{
			get { return this.y1; }
			set { this.y1 = value; }
		}
		
		[XmlIgnore] public double				Top
		{
			get { return this.y2; }
			set { this.y2 = value; }
		}
		
		
		[XmlIgnore] public Point				BottomLeft
		{
			get { return new Point (this.x1, this.y1); }
			set
			{
				this.x1 = value.X;
				this.y1 = value.Y;
			}
		}
		
		[XmlIgnore] public Point				BottomRight
		{
			get { return new Point (this.x2, this.y1); }
			set
			{
				this.x2 = value.X;
				this.y1 = value.Y;
			}
		}
		
		[XmlIgnore] public Point				TopRight
		{
			get { return new Point (this.x2, this.y2); }
			set
			{
				this.x2 = value.X;
				this.y2 = value.Y;
			}
		}
		
		[XmlIgnore] public Point				TopLeft
		{
			get { return new Point (this.x1, this.y2); }
			set
			{
				this.x1 = value.X;
				this.y2 = value.Y;
			}
		}

		[XmlIgnore]
		public Point							Center
		{
			get
			{
				return new Point ((this.x1 + this.x2) / 2, (this.y1 + this.y2) / 2);
			}
			set
			{
				Point center = this.Center;

				if (center != value)
				{
					this.Offset (value - center);
				}
			}
		}
		
		[XmlIgnore]
		public Point							MiddleLeft
		{
			get { return new Point (this.x1, (this.y2 - this.y1) / 2); }
		}

		[XmlIgnore]
		public Point							MiddleRight
		{
			get { return new Point (this.x2, (this.y2 - this.y1) / 2); }
		}

		[XmlIgnore]
		public Point							MiddleTop
		{
			get { return new Point ((this.x2 - this.x1) / 2, this.y2); }
		}

		[XmlIgnore]
		public Point							MiddleBottom
		{
			get { return new Point ((this.x2 - this.x1) / 2, this.y1); }
		}
		
		[XmlIgnore]
		public double							Surface
		{
			get
			{
				if (this.IsSurfaceZero)
				{
					return 0;
				}
				else if (this.IsInfinite)
				{
					return 1000000000.0*1000000000.0;
				}
				else
				{
					return this.Width * this.Height;
				}
			}
		}
		
		
		public Point GetGrip(GripId grip)
		{
			switch (grip)
			{
				case GripId.VertexBottomLeft:	return this.BottomLeft;
				case GripId.VertexBottomRight:	return this.BottomRight;
				case GripId.VertexTopLeft:		return this.TopLeft;
				case GripId.VertexTopRight:		return this.TopRight;
				case GripId.EdgeBottom:			return this.MiddleBottom;
				case GripId.EdgeTop:			return this.MiddleTop;
				case GripId.EdgeLeft:			return this.MiddleLeft;
				case GripId.EdgeRight:			return this.MiddleRight;
				case GripId.Body:				return this.Center;
			}
			
			throw new System.ArgumentOutOfRangeException ("GripId", grip, "GripId is out of range.");
		}
		
		public void SetGrip(GripId grip, Point pos)
		{
			switch (grip)
			{
				case GripId.VertexBottomLeft:	this.BottomLeft  = pos; break;
				case GripId.VertexBottomRight:	this.BottomRight = pos; break;
				case GripId.VertexTopLeft:		this.TopLeft     = pos; break;;
				case GripId.VertexTopRight:		this.TopRight    = pos; break;
				
				case GripId.EdgeBottom:			this.Bottom = pos.Y; break;
				case GripId.EdgeTop:			this.Top    = pos.Y; break;
				case GripId.EdgeLeft:			this.Left   = pos.X; break;
				case GripId.EdgeRight:			this.Right  = pos.X; break;
				
				case GripId.Body:
					this.Location = this.Location + pos - this.Center;
					break;
					
				default:
					throw new System.ArgumentOutOfRangeException ("GripId", grip, "GripId is out of range.");
			}
		}
		
		public void OffsetGrip(GripId grip, Point offset)
		{
			this.SetGrip (grip, this.GetGrip (grip) + offset);
		}
		
		public void FlipX()
		{
			double x = this.x1;
			this.x1  = this.x2;
			this.x2  = x;
		}
		
		public void FlipY()
		{
			double y = this.y1;
			this.y1  = this.y2;
			this.y2  = y;
		}
		
		
		public static GripId ConvertToGrip(EdgeId edges)
		{
			switch (edges)
			{
				case EdgeId.None:					return GripId.None;
				
				case EdgeId.Bottom:					return GripId.EdgeBottom;
				case EdgeId.Top:					return GripId.EdgeTop;
				case EdgeId.Left:					return GripId.EdgeLeft;
				case EdgeId.Right:					return GripId.EdgeRight;
				
				case EdgeId.Bottom | EdgeId.Left:	return GripId.VertexBottomLeft;
				case EdgeId.Bottom | EdgeId.Right:	return GripId.VertexBottomRight;
				case EdgeId.Top    | EdgeId.Left:	return GripId.VertexTopLeft;
				case EdgeId.Top    | EdgeId.Right:	return GripId.VertexTopRight;
			}
			
			throw new System.ArgumentException (string.Format ("EdgeId {0} cannot be mapped to GripId.", edges), "edges");
		}
		
		public static EdgeId ConvertToEdges(GripId grip)
		{
			switch (grip)
			{
				case GripId.None:				return EdgeId.None;
				
				case GripId.VertexBottomLeft:	return EdgeId.Bottom | EdgeId.Left;
				case GripId.VertexBottomRight:	return EdgeId.Bottom | EdgeId.Right;
				case GripId.VertexTopLeft:		return EdgeId.Top    | EdgeId.Left;
				case GripId.VertexTopRight:		return EdgeId.Top    | EdgeId.Right;
				
				case GripId.EdgeBottom:			return EdgeId.Bottom;
				case GripId.EdgeTop:			return EdgeId.Top;
				case GripId.EdgeLeft:			return EdgeId.Left;
				case GripId.EdgeRight:			return EdgeId.Right;
			}
			
			throw new System.ArgumentOutOfRangeException ("GripId", grip, "GripId is out of range.");
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
			if (r.IsEmpty)
			{
				return true;
			}
			
			return (!this.IsEmpty) && (r.x1 >= this.x1) && (r.x2 <= this.x2) && (r.y1 >= this.y1) && (r.y2 <= this.y2);
		}
		
		public Point Constrain(Point p)
		{
			if (this.IsValid)
			{
				p.X = System.Math.Max (this.x1, p.X);
				p.X = System.Math.Min (this.x2, p.X);
				p.Y = System.Math.Max (this.y1, p.Y);
				p.Y = System.Math.Min (this.y2, p.Y);
			}
			
			return p;
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
		
		public void Inflate(double s)
		{
			this.Inflate (s, s);
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
		
		public void Inflate(double left, double right, double top, double bottom)
		{
			if (! this.IsEmpty)
			{
				this.x1 -= left;
				this.x2 += right;
				this.y1 -= bottom;
				this.y2 += top;
			}
		}
		
		public void Deflate(Size s)
		{
			this.Deflate (s.Width, s.Height);
		}
		
		public void Deflate(double s)
		{
			this.Deflate (s, s);
		}
		
		public void Deflate(double x, double y)
		{
			if (! this.IsEmpty)
			{
				this.x1 += x;
				this.x2 -= x;
				this.y1 += y;
				this.y2 -= y;
			}
		}
		
		public void Deflate(Margins margins)
		{
			if (! this.IsEmpty)
			{
				this.x1 += margins.Left;
				this.x2 -= margins.Right;
				this.y1 += margins.Bottom;
				this.y2 -= margins.Top;
			}
		}
		
		public void Deflate(double left, double right, double top, double bottom)
		{
			if (! this.IsEmpty)
			{
				this.x1 += left;
				this.x2 -= right;
				this.y1 += bottom;
				this.y2 -= top;
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
			if (r.IsEmpty)
			{
				//	Rien à faire...
			}
			else if (this.IsEmpty)
			{
				this.x1 = r.x1;
				this.y1 = r.y1;
				this.x2 = r.x2;
				this.y2 = r.y2;
			}
			else
			{
				this.x1 = System.Math.Min (this.x1, r.x1);
				this.y1 = System.Math.Min (this.y1, r.y1);
				this.x2 = System.Math.Max (this.x2, r.x2);
				this.y2 = System.Math.Max (this.y2, r.y2);
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
		
		public void RoundInflate()
		{
			this.x1 = System.Math.Floor (this.x1);
			this.x2 = System.Math.Ceiling (this.x2);
			this.y1 = System.Math.Floor (this.y1);
			this.y2 = System.Math.Ceiling (this.y2);
		}
		
		public void RoundDeflate()
		{
			this.x1 = System.Math.Ceiling (this.x1);
			this.x2 = System.Math.Floor (this.x2);
			this.y1 = System.Math.Ceiling (this.y1);
			this.y2 = System.Math.Floor (this.y2);
		}
		
		public void RoundCeiling()
		{
			this.x1 = System.Math.Ceiling (this.x1);
			this.x2 = System.Math.Ceiling (this.x2);
			this.y1 = System.Math.Ceiling (this.y1);
			this.y2 = System.Math.Ceiling (this.y2);
		}
		
		public void RoundFloor()
		{
			this.x1 = System.Math.Floor (this.x1);
			this.x2 = System.Math.Floor (this.x2);
			this.y1 = System.Math.Floor (this.y1);
			this.y2 = System.Math.Floor (this.y2);
		}


		public static Rectangle FromOppositeCorners(double x1, double y1, double x2, double y2)
		{
			Rectangle r;

			r.x1 = x1;
			r.y1 = y1;
			r.x2 = x2;
			r.y2 = y2;

			return r;
		}
		
		public static Rectangle FromPoints(double x1, double y1, double x2, double y2)
		{
			Rectangle r;
			
			r.x1 = System.Math.Min (x1, x2);
			r.y1 = System.Math.Min (y1, y2);
			r.x2 = System.Math.Max (x1, x2);
			r.y2 = System.Math.Max (y1, y2);
			
			return r;
		}
		
		public static Rectangle FromPoints(Point p1, Point p2)
		{
			Rectangle r;
			
			r.x1 = System.Math.Min (p1.X, p2.X);
			r.y1 = System.Math.Min (p1.Y, p2.Y);
			r.x2 = System.Math.Max (p1.X, p2.X);
			r.y2 = System.Math.Max (p1.Y, p2.Y);
			
			return r;
		}

		public static Rectangle Scale(Rectangle r, double s)
		{
			return new Rectangle (r.x1*s, r.y1*s, r.Width*s, r.Height*s);
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
		
		public static Rectangle Deflate(Rectangle r, double x, double y)
		{
			if (r.IsEmpty)
			{
				return Rectangle.Empty;
			}
			
			r.Deflate (x, y);
			return r;
		}
		
		public static Rectangle Deflate(Rectangle r, Margins margins)
		{
			if (r.IsEmpty)
			{
				return Rectangle.Empty;
			}
			
			r.Deflate (margins);
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
			
			return ((x1 > x2) || (y1 > y2)) ? Rectangle.Empty : Rectangle.FromPoints (x1, y1, x2, y2);
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
			
			return ((x1 > x2) || (y1 > y2)) ? Rectangle.Empty : Rectangle.FromPoints (x1, y1, x2, y2);
		}

		public static Rectangle Constrain(Rectangle r, Rectangle bounds)
		{
			double dx = System.Math.Min (r.Width, bounds.Width);
			double dy = System.Math.Min (r.Height, bounds.Height);

			if (r.x1 < bounds.x1)
			{
				r.x1 = bounds.x1;
				r.x2 = bounds.x1 + dx;
			}
			else if (r.x2 > bounds.x2)
			{
				r.x1 = bounds.x2 - dx;
				r.x2 = bounds.x2;
			}

			if (r.y1 < bounds.y1)
			{
				r.y1 = bounds.y1;
				r.y2 = bounds.y1 + dy;
			}
			else if (r.y2 > bounds.y2)
			{
				r.y1 = bounds.y2 - dy;
				r.y2 = bounds.y2;
			}

			return r;
		}
		
		public static Rectangle Parse(string value)
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
			
			string argX  = args[0].Trim ();
			string argY  = args[1].Trim ();
			string argDx = args[2].Trim ();
			string argDy = args[3].Trim ();
			
			double x  = System.Double.Parse (argX, System.Globalization.CultureInfo.InvariantCulture);
			double y  = System.Double.Parse (argY, System.Globalization.CultureInfo.InvariantCulture);
			double dx = System.Double.Parse (argDx, System.Globalization.CultureInfo.InvariantCulture);
			double dy = System.Double.Parse (argDy, System.Globalization.CultureInfo.InvariantCulture);
			
			return new Rectangle (x, y, dx, dy);
		}
		
		public static Rectangle Parse(string value, Rectangle defaultValue)
		{
			string[] args = value.Split (new char[] { ';', ':' });
			
			if (args.Length != 4)
			{
				throw new System.ArgumentException (string.Format ("Invalid rectangle specification ({0}).", value));
			}
			
			string argX  = args[0].Trim ();
			string argY  = args[1].Trim ();
			string argDx = args[2].Trim ();
			string argDy = args[3].Trim ();
			
			if (argX  != "*") defaultValue.X      = System.Double.Parse (argX, System.Globalization.CultureInfo.InvariantCulture);
			if (argY  != "*") defaultValue.Y      = System.Double.Parse (argY, System.Globalization.CultureInfo.InvariantCulture);
			if (argDx != "*") defaultValue.Width  = System.Double.Parse (argDx, System.Globalization.CultureInfo.InvariantCulture);
			if (argDy != "*") defaultValue.Height = System.Double.Parse (argDy, System.Globalization.CultureInfo.InvariantCulture);
			
			return defaultValue;
		}
		
		public static bool operator ==(Rectangle a, Rectangle b)
		{
			return (a.x1 == b.x1) && (a.x2 == b.x2) && (a.y1 == b.y1) && (a.y2 == b.y2);
		}
		
		public static bool operator !=(Rectangle a, Rectangle b)
		{
			return (a.x1 != b.x1) || (a.x2 != b.x2) || (a.y1 != b.y1) || (a.y2 != b.y2);
		}
		
		
		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2};{3}",
				this.X, this.Y, this.Width, this.Height);
		}


		public static bool Equal(Margins a, Margins b, double δ)
		{
			return Math.Equal (a.Left, b.Left, δ)
				&& Math.Equal (a.Right, b.Right, δ)
				&& Math.Equal (a.Top, b.Top, δ)
				&& Math.Equal (a.Bottom, b.Bottom, δ);
		}

		public override bool Equals(object obj)
		{
			return (obj is Rectangle) && (this == (Rectangle) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		
		#region Converter Class
		public class Converter : Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return Rectangle.Parse (value);
			}

			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				Rectangle rect = (Rectangle) value;
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2};{3}", rect.X, rect.Y, rect.Width, rect.Height);
			}
			
			public static string ToString(object value, bool suppressX, bool suppressY, bool suppressWidth, bool suppressHeight)
			{
				Rectangle rect = (Rectangle) value;
				
				string arg1 = suppressX      ? "*" : rect.X.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string arg2 = suppressY      ? "*" : rect.Y.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string arg3 = suppressWidth  ? "*" : rect.Width.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string arg4 = suppressHeight ? "*" : rect.Height.ToString (System.Globalization.CultureInfo.InvariantCulture);
				
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2};{3}", arg1, arg2, arg3, arg4);
			}
		}
		#endregion
		
		private double					x1, y1;
		private double					x2, y2;
	}
}
