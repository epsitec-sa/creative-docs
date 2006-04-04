//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;
	
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (Size.Converter))]
	
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
		
		
		[XmlAttribute] public double			Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}
		
		[XmlAttribute] public double			Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}
		
		
		public bool								IsEmpty
		{
			get
			{
				return (this.width <= 0) && (this.height <= 0);
			}
		}
		
		
		public static readonly Size				Empty;
		public static readonly Size				MaxValue = new Drawing.Size (2000000000, 2000000000);
		public static readonly Size				Zero = new Drawing.Size (0, 0);
		public static readonly Size				PositiveInfinity = new Drawing.Size (double.PositiveInfinity, double.PositiveInfinity);
		
		public Point ToPoint()
		{
			return new Point (this.width, this.height);
		}
		
		
		public override string ToString()
		{
			return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "[{0};{1}]", this.width, this.height);
		}
		
		
		public override bool Equals(object obj)
		{
			return (obj is Size) && (this == (Size) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
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
				throw new System.ArgumentException (string.Format ("Invalid size specification ({0}).", value));
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
				throw new System.ArgumentException (string.Format ("Invalid size specification ({0}).", value));
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
		
		public static Size operator *(Size a, double value)
		{
			return new Size (a.width * value, a.height * value);
		}
		
		public static Size operator /(Size a, double value)
		{
			return new Size (a.width / value, a.height / value);
		}
		
		public static bool operator ==(Size a, Size b)
		{
			return (a.width == b.width) && (a.height == b.height);
		}
		
		public static bool operator !=(Size a, Size b)
		{
			return (a.width != b.width) || (a.height != b.height);
		}
		
		
		public static Size operator +(Size a, Margins b)
		{
			return new Size (a.Width + b.Width, a.Height + b.Height);
		}
		
		public static Size operator -(Size a, Margins b)
		{
			return new Size (System.Math.Max (0, a.Width - b.Width), System.Math.Max (0, a.Height - b.Height));
		}
		
		
		#region Converter Class
		public class Converter : AbstractStringConverter
		{
			public override object ParseString(string value)
			{
				return Size.Parse (value);
			}
			
			public override string ToString(object value)
			{
				Size size = (Size) value;
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1}", size.Width, size.Height);
			}
			
			public static string ToString(object value, bool suppress_width, bool suppress_height)
			{
				Size size = (Size) value;
				
				string arg1 = suppress_width  ? "*" : size.Width.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string arg2 = suppress_height ? "*" : size.Height.ToString (System.Globalization.CultureInfo.InvariantCulture);
				
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1}", arg1, arg2);
			}
		}
		#endregion
		
		
		private double							width;
		private double							height;
	}
}
