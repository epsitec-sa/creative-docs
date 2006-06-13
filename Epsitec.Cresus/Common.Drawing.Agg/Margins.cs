//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		
		[XmlAttribute] public double			Left
		{
			get { return this.left; }
			set { this.left = value; }
		}
		
		[XmlAttribute] public double			Right
		{
			get { return this.right; }
			set { this.right = value; }
		}
		
		[XmlAttribute] public double			Top
		{
			get { return this.top; }
			set { this.top = value; }
		}
		
		[XmlAttribute] public double			Bottom
		{
			get { return this.bottom; }
			set { this.bottom = value; }
		}
		
		
		public double							Width
		{
			get { return this.left + this.right; }
		}
		
		public double							Height
		{
			get { return this.top + this.bottom; }
		}
		
		public Size								Size
		{
			get { return new Size (this.Width, this.Height); }
		}

		
		public void ClipNegative()
		{
			if (this.left < 0)
			{
				this.left = 0;
			}
			if (this.right < 0)
			{
				this.right = 0;
			}
			if (this.top < 0)
			{
				this.top = 0;
			}
			if (this.bottom < 0)
			{
				this.bottom = 0;
			}
		}

		public static readonly Margins			Zero = new Margins(0, 0, 0, 0);
		
		public override string ToString()
		{
			return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2};{3}", this.left, this.right, this.top, this.bottom);
		}
		
		
		public override bool Equals(object obj)
		{
			return (obj is Margins) && (this == (Margins) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		
		public static Margins Parse(string value)
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
			
			double x1 = System.Double.Parse (arg_x1, System.Globalization.CultureInfo.InvariantCulture);
			double x2 = System.Double.Parse (arg_x2, System.Globalization.CultureInfo.InvariantCulture);
			double y1 = System.Double.Parse (arg_y1, System.Globalization.CultureInfo.InvariantCulture);
			double y2 = System.Double.Parse (arg_y2, System.Globalization.CultureInfo.InvariantCulture);
			
			return new Margins (x1, x2, y1, y2);
		}
		
		public static Margins Parse(string value, Margins default_value)
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
			
			if (arg_x1 != "*") default_value.Left   = System.Double.Parse (arg_x1, System.Globalization.CultureInfo.InvariantCulture);
			if (arg_x2 != "*") default_value.Right  = System.Double.Parse (arg_x2, System.Globalization.CultureInfo.InvariantCulture);
			if (arg_y1 != "*") default_value.Top    = System.Double.Parse (arg_y1, System.Globalization.CultureInfo.InvariantCulture);
			if (arg_y2 != "*") default_value.Bottom = System.Double.Parse (arg_y2, System.Globalization.CultureInfo.InvariantCulture);
			
			return default_value;
		}
		
		public static bool operator ==(Margins a, Margins b)
		{
			return (a.left == b.left) && (a.right == b.right) && (a.top == b.top) && (a.bottom == b.bottom);
		}
		
		public static bool operator !=(Margins a, Margins b)
		{
			return (a.left != b.left) || (a.right != b.right) || (a.top != b.top) || (a.bottom != b.bottom);
		}
		
		public static Margins operator -(Margins a)
		{
			return new Margins (-a.left, -a.right, -a.top, -a.bottom);
		}
		
		public static Margins operator +(Margins a, Margins b)
		{
			return new Margins (a.left+b.left, a.right+b.right, a.top+b.top, a.bottom+b.bottom);
		}
		
		
		#region Converter Class
		public class Converter : AbstractStringConverter
		{
			public override object ParseString(string value)
			{
				return Margins.Parse (value);
			}
			
			public override string ToString(object value)
			{
				Margins margins = (Margins) value;
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2};{3}", margins.Left, margins.Right, margins.Top, margins.Bottom);
			}
			
			public static string ToString(object value, bool suppress_left, bool suppress_right, bool suppress_top, bool suppress_bottom)
			{
				Margins margins = (Margins) value;
				
				string arg1 = suppress_left   ? "*" : margins.Left.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string arg2 = suppress_right  ? "*" : margins.Right.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string arg3 = suppress_top    ? "*" : margins.Top.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string arg4 = suppress_bottom ? "*" : margins.Bottom.ToString (System.Globalization.CultureInfo.InvariantCulture);
				
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2};{3}", arg1, arg2, arg3, arg4);
			}
		}
		#endregion
		
		private double							left;
		private double							right;
		private double							top;
		private double							bottom;
	}
}
