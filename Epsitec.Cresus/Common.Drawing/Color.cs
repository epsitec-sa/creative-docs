//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;
	
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (Color.Converter))]
	
	public struct Color
	{
		public Color(System.Drawing.Color color)
		{
			this.r = color.R / 255.0;
			this.g = color.G / 255.0;
			this.b = color.B / 255.0;
			this.a = color.A / 255.0;
			this.is_empty = color.IsEmpty;
		}
		
		public Color(double a, double r, double g, double b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
			this.is_empty = false;
		}
		
		public Color(double r, double g, double b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 1.0;
			this.is_empty = false;
		}
		
		public Color(double brightness)
		{
			this.r = brightness;
			this.g = brightness;
			this.b = brightness;
			this.a = 1.0;
			this.is_empty = false;
		}
		
		
		[XmlAttribute] public double	R
		{
			get { return this.r; }
			set { this.r = value; }
		}
		
		[XmlAttribute] public double	G
		{
			get { return this.g; }
			set { this.g = value; }
		}
		
		[XmlAttribute] public double	B
		{
			get { return this.b; }
			set { this.b = value; }
		}
		
		[XmlAttribute] public double	A
		{
			get { return this.a; }
			set { this.a = value; }
		}
		
		
		public bool						IsEmpty
		{
			get { return this.is_empty; }
		}
		
		public bool						IsValid
		{
			get { return !this.is_empty; }
		}
		
		public bool						IsTransparent
		{
			get { return !this.is_empty && (this.a == 0.0); }
		}
		
		public bool						IsOpaque
		{
			get { return !this.is_empty && (this.a == 1.0); }
		}
		
		public bool						IsVisible
		{
			get { return !this.is_empty && (this.a != 0.0); }
		}
		
		public bool						IsInRange
		{
			get
			{
				return (this.r >= 0.0) && (this.r <= 1.0)
					&& (this.g >= 0.0) && (this.g <= 1.0)
					&& (this.b >= 0.0) && (this.b <= 1.0)
					&& (this.a >= 0.0) && (this.a <= 1.0);
			}
		}
		
		public static Color				Empty
		{
			get
			{
				Color c = new Color ();
				c.is_empty = true;
				return c;
			}
		}
		
		public static Color				Transparent
		{
			get
			{
				Color c = new Color (0.0, 0.0, 0.0, 0.0);
				return c;
			}
		}
		
		
		public Color ClipToRange()
		{
			Color color = new Color ();
			
			color.r = Epsitec.Common.Math.Clip (this.r);
			color.g = Epsitec.Common.Math.Clip (this.g);
			color.b = Epsitec.Common.Math.Clip (this.b);
			color.a = Epsitec.Common.Math.Clip (this.a);
			
			return color;
		}
		
		public double GetBrightness()
		{
			//	Calcule la luminosité de la couleur.
			
			return this.r*0.30 + this.g*0.59 + this.b*0.11;
		}
		
		public void GetHSV(out double h, out double s, out double v)
		{
			Color.ConvertRGBtoHSV (this.r, this.g, this.b, out h, out s, out v);
		}

		public void GetARGB(out double a, out double r, out double g, out double b)
		{
			a = this.A;
			r = this.R;
			g = this.G;
			b = this.B;
		}
		
		
		public static Color FromColor(Color color, double alpha)
		{
			return new Color (color.a * alpha, color.r, color.g, color.b);
		}
		
		public static Color FromARGB(double a, double r, double g, double b)
		{
			return new Color (a, r, g, b);
		}
		
		public static Color FromRGB(double r, double g, double b)
		{
			return new Color (r, g, b);
		}
		
		public static Color FromBrightness(double brightness)
		{
			return new Color (brightness);
		}
		
		public static Color FromHSV(double h, double s, double v)
		{
			double r,g,b;
			Color.ConvertHSVtoRGB(h,s,v, out r, out g, out b);
			return new Color(r, g, b);
		}

		public static Color FromAHSV(double a, double h, double s, double v)
		{
			Color color = Color.FromHSV (h, s, v);
			color.A = a;
			return color;
		}
		
		public static Color FromName(string name)
		{
			if ((name.Length > 1) &&
				(name[0] == '#'))
			{
				return Color.FromHexa (name.Remove (0, 1));
			}
			
			System.Drawing.Color color = System.Drawing.Color.FromName (name);
			
			if (color.IsEmpty)
			{
				return Color.Empty;
			}
			
			return new Color (color);
		}

		public static Color FromHexa(string hexa)
		{
			// Conversion d'une chaîne "FF3300" en une couleur.
			
			if (hexa.Length != 6)
			{
				return Color.Empty;
			}
			
			try
			{
				byte r = System.Convert.ToByte (hexa.Substring (0, 2), 16);
				byte g = System.Convert.ToByte (hexa.Substring (2, 2), 16);
				byte b = System.Convert.ToByte (hexa.Substring (4, 2), 16);
			
				return new Color (r/255.0, g/255.0, b/255.0);
			}
			catch
			{
				return Color.Empty;
			}
		}

		public static string ToHexa(Color color)
		{
			// Conversion d'une couleur en chaîne "FF3300".
			
			int r = (int)(color.R*255.0+0.5);
			int g = (int)(color.G*255.0+0.5);
			int b = (int)(color.B*255.0+0.5);
			return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
		}

		
		public override bool Equals(object obj)
		{
			if ((obj == null) ||
				(obj.GetType () != typeof (Color)))
			{
				return false;
			}
			
			Color color = (Color) obj;
			
			return this == color;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		public override string ToString()
		{
			return System.String.Format ("{{R={0:0.00},G={1:0.00},B={2:0.00},A={3:0.00}}}", this.r, this.g, this.b, this.a);
		}
		
		
		public static Color Parse(string value, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return Color.Empty;
			}
			
			string[] args = value.Split (';', ':');
			
			if (args.Length == 3)
			{
				string arg_r = args[0].Trim ();
				string arg_g = args[1].Trim ();
				string arg_b = args[2].Trim ();
			
				double r = System.Double.Parse (arg_r, culture);
				double g = System.Double.Parse (arg_g, culture);
				double b = System.Double.Parse (arg_b, culture);
			
				return new Color (r, g, b);
			}
			
			if (args.Length == 4)
			{
				string arg_a = args[0].Trim ();
				string arg_r = args[1].Trim ();
				string arg_g = args[2].Trim ();
				string arg_b = args[3].Trim ();
				
				double a = System.Double.Parse (arg_a, culture);
				double r = System.Double.Parse (arg_r, culture);
				double g = System.Double.Parse (arg_g, culture);
				double b = System.Double.Parse (arg_b, culture);
				
				return new Color (a, r, g, b);
			}
			
			throw new System.ArgumentException (string.Format ("Invalid color specification ({0}).", value));
		}
		
		public static string ToString(Color color, System.Globalization.CultureInfo culture)
		{
			if (color.A == 1.0)
			{
				return string.Format ("{0};{1};{2}", color.R, color.G, color.B);
			}
			else
			{
				return string.Format ("{0};{1};{2};{3}", color.A, color.R, color.G, color.B);
			}
		}
		
		public static bool operator==(Color a, Color b)
		{
			if (a.is_empty && b.is_empty)
			{
				return true;
			}
			if (a.is_empty || b.is_empty)
			{
				return false;
			}
			
			return (a.r == b.r)
				&& (a.g == b.g)
				&& (a.b == b.b)
				&& (a.a == b.a);
		}
		
		public static bool operator!=(Color a, Color b)
		{
			if (a.is_empty && b.is_empty)
			{
				return false;
			}
			if (a.is_empty || b.is_empty)
			{
				return true;
			}
			
			return (a.r != b.r)
				|| (a.g != b.g)
				|| (a.b != b.b)
				|| (a.a != b.a);
		}
		
		
		public static void ConvertRGBtoHSV(double r, double g, double b, out double h, out double s, out double v)
		{
			// H = [0..360]
			// S = [0..1]
			// V = [0..1]
			
			double min = System.Math.Min(r,System.Math.Min(g,b));
			v = System.Math.Max(r,System.Math.Max(g,b));
			double delta = v-min;

			if ( v == 0 )  s = 0;
			else           s = delta/v;

			if ( s == 0 )  // achromatic ?
			{
				h = 0;
			}
			else	// chromatic ?
			{
				if ( r == v )  // between yellow and magenta ?
				{
					h = 60*(g-b)/delta;
				}
				else if ( g == v )  // between cyan and yellow ?
				{
					h = 120+60*(b-r)/delta;
				}
				else	// between magenta and cyan ?
				{
					h = 240+60*(r-g)/delta;
				}
				if ( h < 0 )  h += 360;
			}
		}

		public static void ConvertHSVtoRGB(double h, double s, double v, out double r, out double g, out double b)
		{
			r = g = b = v;
			if ( s == 0 )  return;  // noir ?

			while ( h <   0 )  h += 360;
			while ( h > 360 )  h -= 360;
			h /= 60;  // 0..6
			double f = h-System.Math.Floor(h);
			double p = v*(1-s);
			double q = v*(1-s*f);
			double t = v*(1-s*(1-f));

			int i = (int)h;
			if ( i == 6 )  i = 0;
			switch ( i )
			{
				case 0:  r=v;  g=t;  b=p;  break;
				case 1:  r=q;  g=v;  b=p;  break;
				case 2:  r=p;  g=v;  b=t;  break;
				case 3:  r=p;  g=q;  b=v;  break;
				case 4:  r=t;  g=p;  b=v;  break;
				case 5:  r=v;  g=p;  b=q;  break;
			}
		}

		
		
		public class Converter : Epsitec.Common.Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return Color.Parse (value, culture);
			}
			
			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				return Color.ToString ((Color) value, culture);
			}
		}
		
		
		private double					r, g, b, a;
		private bool					is_empty;
	}
}
