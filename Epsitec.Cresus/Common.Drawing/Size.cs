namespace Epsitec.Common.Drawing
{
	public struct Size
	{
		public Size(double width, double height)
		{
			this.width  = width;
			this.height = height;
		}
		
		public Size(System.Drawing.SizeF size)
		{
			this.width  = size.Width;
			this.height = size.Height;
		}
		
		public Size(System.Drawing.Size size)
		{
			this.width  = size.Width;
			this.height = size.Height;
		}
		
		public double					Width
		{
			get { return this.width; }
			set { this.width = value; }
		}
		
		public double					Height
		{
			get { return this.height; }
			set { this.height = value; }
		}
		
		public bool						IsEmpty
		{
			get { return this.width == 0 && this.height == 0; }
		}
		
		public static readonly Size		Empty;
		
		public Point ToPoint()
		{
			return new Point (this.width, this.height);
		}
		
		public override bool Equals(object obj)
		{
			if ((obj == null) &&
				(obj.GetType () != typeof (Size)))
			{
				return false;
			}
			
			Size s = (Size) obj;
			
			return (s.width == this.width) && (s.height == this.height);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		public override string ToString()
		{
			return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1}", this.width, this.height);
		}
		
		
		public static Size Parse(string value)
		{
			if (value == null)
			{
				return Size.Empty;
			}
			
			string[] args = value.Split (';', ':');
			
			if (args.Length != 2)
			{
				throw new System.ArgumentException (string.Format ("Invalid size specification ({0})", value));
			}
			
			string arg_x = args[0].Trim ();
			string arg_y = args[1].Trim ();
			
			double x = System.Double.Parse (arg_x, System.Globalization.CultureInfo.InvariantCulture);
			double y = System.Double.Parse (arg_y, System.Globalization.CultureInfo.InvariantCulture);
			
			return new Size (x, y);
		}
		
		public static Size Parse(string value, Size default_value)
		{
			string[] args = value.Split (new char[] { ';', ':' });
			
			if (args.Length != 2)
			{
				throw new System.ArgumentException (string.Format ("Invalid size specification ({0})", value));
			}
			
			string arg_x = args[0].Trim ();
			string arg_y = args[1].Trim ();
			
			if (arg_x != "*") default_value.Width  = System.Double.Parse (arg_x, System.Globalization.CultureInfo.InvariantCulture);
			if (arg_y != "*") default_value.Height = System.Double.Parse (arg_y, System.Globalization.CultureInfo.InvariantCulture);
			
			return default_value;
		}
		
		
		public static Size operator +(Size a, Size b)
		{
			return new Size (a.Width + b.Width, a.Height + b.Height);
		}
		
		public static Size operator -(Size a, Size b)
		{
			return new Size (a.Width - b.Width, a.Height - b.Height);
		}
		
		public static bool operator ==(Size a, Size b)
		{
			return (a.width == b.width) && (a.height == b.height);
		}
		
		public static bool operator !=(Size a, Size b)
		{
			return (a.width != b.width) || (a.height != b.height);
		}
		
		private double					width, height;
	}
}
