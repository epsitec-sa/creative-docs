namespace Epsitec.Common.Drawing
{
	using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;
	using XmlIgnore    = System.Xml.Serialization.XmlIgnoreAttribute;
	
	
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (Margins.Converter))]
	
	public struct Margins
	{
		public Margins(double left, double right, double top, double bottom)
		{
			this.left   = left;
			this.right  = right;
			this.top    = top;
			this.bottom = bottom;
		}
		
		
		[XmlAttribute] public double	Left
		{
			get { return this.left; }
			set { this.left = value; }
		}
		
		[XmlAttribute] public double	Right
		{
			get { return this.right; }
			set { this.right = value; }
		}
		
		[XmlAttribute] public double	Top
		{
			get { return this.top; }
			set { this.top = value; }
		}
		
		[XmlAttribute] public double	Bottom
		{
			get { return this.bottom; }
			set { this.bottom = value; }
		}
		
		
		public double					Width
		{
			get { return this.left + this.right; }
		}
		
		public double					Height
		{
			get { return this.top + this.bottom; }
		}
		
		public Size						Size
		{
			get { return new Size (this.Width, this.Height); }
		}
		
		
		public static readonly Margins	Zero = new Margins(0, 0, 0, 0);
		
		public override bool Equals(object obj)
		{
			if ((obj == null) &&
				(obj.GetType () != typeof (Margins)))
			{
				return false;
			}
			
			Margins m = (Margins) obj;
			
			return (m.left == this.left) && (m.right == this.right) && (m.top == this.top) && (m.bottom == this.bottom);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		public override string ToString()
		{
			return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2};{3}", this.left, this.right, this.top, this.bottom);
		}
		
		
		public static Margins Parse(string value, System.Globalization.CultureInfo culture)
		{
			string[] args = value.Split (new char[] { ';', ':' });
			
			if (args.Length != 4)
			{
				throw new System.ArgumentException (string.Format ("Invalid margins specification ({0})", value));
			}
			
			string arg_x1 = args[0].Trim ();
			string arg_x2 = args[1].Trim ();
			string arg_y1 = args[2].Trim ();
			string arg_y2 = args[3].Trim ();
			
			double x1 = System.Double.Parse (arg_x1, culture);
			double x2 = System.Double.Parse (arg_x2, culture);
			double y1 = System.Double.Parse (arg_y1, culture);
			double y2 = System.Double.Parse (arg_y2, culture);
			
			return new Margins (x1, x2, y1, y2);
		}
		
		public static Margins Parse(string value, System.Globalization.CultureInfo culture, Margins default_value)
		{
			string[] args = value.Split (new char[] { ';', ':' });
			
			if (args.Length != 4)
			{
				throw new System.ArgumentException (string.Format ("Invalid margins specification ({0})", value));
			}
			
			string arg_x1 = args[0].Trim ();
			string arg_x2 = args[1].Trim ();
			string arg_y1 = args[2].Trim ();
			string arg_y2 = args[3].Trim ();
			
			if (arg_x1 != "*") default_value.Left   = System.Double.Parse (arg_x1, culture);
			if (arg_x2 != "*") default_value.Right  = System.Double.Parse (arg_x2, culture);
			if (arg_y1 != "*") default_value.Top    = System.Double.Parse (arg_y1, culture);
			if (arg_y2 != "*") default_value.Bottom = System.Double.Parse (arg_y2, culture);
			
			return default_value;
		}
		
		public static bool operator ==(Margins a, Margins b)
		{
			return a.Equals (b);
		}
		
		public static bool operator !=(Margins a, Margins b)
		{
			return !a.Equals (b);
		}
		
		public static Margins operator -(Margins a)
		{
			return new Margins (-a.left, -a.right, -a.top, -a.bottom);
		}
		
		public class Converter : Epsitec.Common.Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return Margins.Parse (value, culture);
			}
			
			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				Margins margins = (Margins) value;
				return string.Format (culture, "{0};{1};{2};{3}", margins.Left, margins.Right, margins.Top, margins.Bottom);
			}
			
			public static string ToString(object value, System.Globalization.CultureInfo culture, bool suppress_left, bool suppress_right, bool suppress_top, bool suppress_bottom)
			{
				Margins margins = (Margins) value;
				
				string arg1 = suppress_left   ? "*" : margins.Left.ToString (culture);
				string arg2 = suppress_right  ? "*" : margins.Right.ToString (culture);
				string arg3 = suppress_top    ? "*" : margins.Top.ToString (culture);
				string arg4 = suppress_bottom ? "*" : margins.Bottom.ToString (culture);
				
				return string.Format (culture, "{0};{1};{2};{3}", arg1, arg2, arg3, arg4);
			}
		}
		
		
		
		private double					left;
		private double					right;
		private double					top;
		private double					bottom;
	}
}
