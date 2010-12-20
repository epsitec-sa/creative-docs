//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			this.isEmpty = color.IsEmpty;
		}
		
		public Color(double a, double r, double g, double b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
			this.isEmpty = false;
		}
		
		public Color(double r, double g, double b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 1.0;
			this.isEmpty = false;
		}
		
		public Color(double brightness)
		{
			this.r = brightness;
			this.g = brightness;
			this.b = brightness;
			this.a = 1.0;
			this.isEmpty = false;
		}
		
		
		//[XmlAttribute]
		public double			R
		{
			get { return this.r; }
		//	set { this.r = value; }
		}
		
		//[XmlAttribute]
		public double			G
		{
			get { return this.g; }
		//	set { this.g = value; }
		}
		
		//[XmlAttribute]
		public double			B
		{
			get { return this.b; }
		//	set { this.b = value; }
		}
		
		//[XmlAttribute]
		public double			A
		{
			get { return this.a; }
		//	set { this.a = value; }
		}
		
		
		public bool								IsEmpty
		{
			get { return this.isEmpty; }
		}
		
		public bool								IsValid
		{
			get { return !this.isEmpty; }
		}
		
		public bool								IsTransparent
		{
			get { return !this.isEmpty && (this.a == 0.0); }
		}
		
		public bool								IsOpaque
		{
			get { return !this.isEmpty && (this.a == 1.0); }
		}
		
		public bool								IsVisible
		{
			get { return !this.isEmpty && (this.a != 0.0); }
		}
		
		public bool								IsInRange
		{
			get
			{
				return (this.r >= 0.0) && (this.r <= 1.0)
					&& (this.g >= 0.0) && (this.g <= 1.0)
					&& (this.b >= 0.0) && (this.b <= 1.0)
					&& (this.a >= 0.0) && (this.a <= 1.0);
			}
		}

		public static Color						Empty
		{
			get
			{
				Color c = new Color ();
				c.isEmpty = true;
				return c;
			}
		}
		
		public static Color						Transparent
		{
			get
			{
				Color c = new Color (0.0, 0.0, 0.0, 0.0);
				return c;
			}
		}

		public double							Hue
		{
			get
			{
				double h, s, v;
				Color.ConvertRgbToHsv (this.r, this.g, this.b, out h, out s, out v);
				return h;
			}
		}

		public double							Saturation
		{
			get
			{
				double h, s, v;
				Color.ConvertRgbToHsv (this.r, this.g, this.b, out h, out s, out v);
				return s;
			}
		}

		public double							Value
		{
			get
			{
				double h, s, v;
				Color.ConvertRgbToHsv (this.r, this.g, this.b, out h, out s, out v);
				return v;
			}
		}


		public Color ColorOrDefault(Color defaultValue)
		{
			if (this.IsEmpty)
			{
				return defaultValue;
			}
			else
			{
				return this;
			}
		}


		/// <summary>
		/// Mixes the specified color A with a color B; the <c>mix</c> parameter specifies
		/// how much of color B should be used.
		/// </summary>
		/// <param name="colorA">The color A.</param>
		/// <param name="colorB">The color B.</param>
		/// <param name="mix">The mix (<c>0</c> means 0 only, <c>1</c> means B only).</param>
		/// <returns>The result of the mix.</returns>
		public static Color Mix(Color colorA, Color colorB, double mix)
		{
			double r = colorA.R * (1 - mix) + colorB.R * mix;
			double g = colorA.G * (1 - mix) + colorB.G * mix;
			double b = colorA.B * (1 - mix) + colorB.B * mix;
			double a = colorA.A * (1 - mix) + colorB.A * mix;

			return new Color (a, r, g, b);
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
			return Color.GetBrightness (this.r, this.g, this.b);
		}
		
		public void GetHsv(out double h, out double s, out double v)
		{
			Color.ConvertRgbToHsv (this.r, this.g, this.b, out h, out s, out v);
		}

		public void GetAlphaRgb(out double a, out double r, out double g, out double b)
		{
			a = this.A;
			r = this.R;
			g = this.G;
			b = this.B;
		}
		
		
		public static double GetBrightness(double r, double g, double b)
		{
			//	Calcule la luminosité de la couleur.
			
			return r*0.30 + g*0.59 + b*0.11;
		}
		
		
		public static Color FromColor(Color color, double alpha)
		{
			return new Color (color.a * alpha, color.r, color.g, color.b);
		}

		public static Color FromAlphaColor(double a, Color color)
		{
			return new Color (a, color.R, color.G, color.B);
		}

		public static Color FromAlphaRgb(double a, double r, double g, double b)
		{
			return new Color (a, r, g, b);
		}
		
		public static Color FromRgb(double r, double g, double b)
		{
			return new Color (r, g, b);
		}
		
		public static Color FromBrightness(double brightness)
		{
			return new Color (brightness);
		}
		
		public static Color FromHsv(double h, double s, double v)
		{
			double r,g,b;
			Color.ConvertHsvToRgb(h,s,v, out r, out g, out b);
			return new Color(r, g, b);
		}

		public static Color FromAlphaHsv(double a, double h, double s, double v)
		{
			return Color.FromAlphaColor (a, Color.FromHsv (h, s, v));
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
			//	Conversion d'une chaîne "FF3300" en une couleur.
			
			if (hexa.Length == 8)
			{
				try
				{
					byte a = System.Convert.ToByte (hexa.Substring (0, 2), 16);
					byte r = System.Convert.ToByte (hexa.Substring (2, 2), 16);
					byte g = System.Convert.ToByte (hexa.Substring (4, 2), 16);
					byte b = System.Convert.ToByte (hexa.Substring (6, 2), 16);
				
					return new Color (a/255.0, r/255.0, g/255.0, b/255.0);
				}
				catch
				{
					return Color.Empty;
				}
			}
			
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
			//	Conversion d'une couleur en chaîne "FF3300" ou "80FF3300" s'il y a un
			//	canal alpha.

			int a = System.Math.Max (0, System.Math.Min (255, (int)(color.A*255.0+0.5)));
			int r = System.Math.Max (0, System.Math.Min (255, (int)(color.R*255.0+0.5)));
			int g = System.Math.Max (0, System.Math.Min (255, (int)(color.G*255.0+0.5)));
			int b = System.Math.Max (0, System.Math.Min (255, (int)(color.B*255.0+0.5)));

			if (a == 255)
			{
				return r.ToString ("X2") + g.ToString ("X2") + b.ToString ("X2");
			}
			else
			{
				return a.ToString ("X2") + r.ToString ("X2") + g.ToString ("X2") + b.ToString ("X2");
			}
		}

		
		public override string ToString()
		{
			return Color.ToString (this);
//-			return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "{{R={0:0.00},G={1:0.00},B={2:0.00},A={3:0.00}}}", this.r, this.g, this.b, this.a);
		}
		
		
		public override bool Equals(object obj)
		{
			return (obj is Color) && (this == (Color) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		
		public static Color Parse(string value)
		{
			if (value == null)
			{
				return Color.Empty;
			}
			
			string[] args = value.Split (';');
			
			if (args.Length == 3)
			{
				double r = Color.ColorComponentToDouble (args[0]);
				double g = Color.ColorComponentToDouble (args[1]);
				double b = Color.ColorComponentToDouble (args[2]);
			
				return new Color (r, g, b);
			}
			
			if (args.Length == 4)
			{
				double a = Color.ColorComponentToDouble (args[0]);
				double r = Color.ColorComponentToDouble (args[1]);
				double g = Color.ColorComponentToDouble (args[2]);
				double b = Color.ColorComponentToDouble (args[3]);
				
				return new Color (a, r, g, b);
			}
			
			throw new System.ArgumentException (string.Format ("Invalid color specification ({0}).", value));
		}
		
		public static string ToString(Color color)
		{
			if (color.A == 1.0)
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2}",
					/**/			  Color.DoubleToColorComponent (color.R),
					/**/			  Color.DoubleToColorComponent (color.G),
					/**/			  Color.DoubleToColorComponent (color.B));
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2};{3}",
					/**/			  Color.DoubleToColorComponent (color.A),
					/**/			  Color.DoubleToColorComponent (color.R),
					/**/			  Color.DoubleToColorComponent (color.G),
					/**/			  Color.DoubleToColorComponent (color.B));
			}
		}
		
		
		public static double ColorComponentToDouble(string value)
		{
			if (value.Length > 0)
			{
				if (value[0] == '#')
				{
					//	Convertit une chaîne du type #01F en une valeur numérique,
					//	à savoir 31/4095 dans ce cas (code hexa sur 12 bits).
					//
					//	#0 -----> 0.0
					//	#f -----> 1.0
					//	#ff ----> 1.0
					//	#ffff --> 1.0
					
					int n = System.Convert.ToInt32 (value.Substring (1), 16);
					double div = System.Math.Pow (16, value.Length-1) - 1;
					return n / div;
				}
				else
				{
					return System.Double.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
				}
			}
			
			return 0;
		}
		
		public static string DoubleToColorComponent(double value)
		{
			//	Utilise la représentation qui utilise le moins de digits possible
			//	pour representer exactement (sur 16 bits) la valeur spécifiée.
			
			double mul = 16;
			
			for (int i = 1; i < Color.ColorComponentDigits; i++)
			{
				int n = (int) (value * (mul - 1) + 0.5);
				
				if (Math.Equal (value, n / (mul - 1), Color.ColorComponentDelta))
				{
					return Color.DoubleToColorComponent (value, i);
				}
				
				mul *= 16;
			}
			
			return Color.DoubleToColorComponent (value, Color.ColorComponentDigits);
		}
		
		public static string DoubleToColorComponent(double value, int digits)
		{
			double mul = System.Math.Pow (16, digits) - 1;
			int    n   = (int) (value * mul + 0.5);
			
			string format = string.Format (System.Globalization.CultureInfo.InvariantCulture, "X{0}", digits);
			
			return string.Concat ("#", n.ToString (format));
		}
		
		
		public static bool operator==(Color a, Color b)
		{
			if (a.isEmpty && b.isEmpty)
			{
				return true;
			}
			if (a.isEmpty || b.isEmpty)
			{
				return false;
			}
			
			return Math.Equal (a.r, b.r, Color.ColorComponentDelta)
				&& Math.Equal (a.g, b.g, Color.ColorComponentDelta)
				&& Math.Equal (a.b, b.b, Color.ColorComponentDelta)
				&& Math.Equal (a.a, b.a, Color.ColorComponentDelta);
		}
		
		public static bool operator!=(Color a, Color b)
		{
			if (a.isEmpty && b.isEmpty)
			{
				return false;
			}
			if (a.isEmpty || b.isEmpty)
			{
				return true;
			}
			
			return ! Math.Equal (a.r, b.r, Color.ColorComponentDelta)
				|| ! Math.Equal (a.g, b.g, Color.ColorComponentDelta)
				|| ! Math.Equal (a.b, b.b, Color.ColorComponentDelta)
				|| ! Math.Equal (a.a, b.a, Color.ColorComponentDelta);
		}
		
		
		public static void ConvertRgbToHsv(double r, double g, double b, out double h, out double s, out double v)
		{
			//	H = [0..360]
			//	S = [0..1]
			//	V = [0..1]
			
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

		public static void ConvertHsvToRgb(double h, double s, double v, out double r, out double g, out double b)
		{
			r = g = b = v;
			
			if (s == 0)
			{
				//	Unsaturated color: this is a gray color.
				return;
			}

			while (h <    0) h += 360;
			while (h >= 360) h -= 360;
			
			h /= 60;  // 0..5
			
			double f = h-System.Math.Floor(h);
			double p = v*(1-s);
			double q = v*(1-s*f);
			double t = v*(1-s*(1-f));

			switch ((int)h)
			{
				case 0:  r=v; g=t; b=p; break;
				case 1:  r=q; g=v; b=p; break;
				case 2:  r=p; g=v; b=t; break;
				case 3:  r=p; g=q; b=v; break;
				case 4:  r=t; g=p; b=v; break;
				case 5:  r=v; g=p; b=q; break;
			}
		}
		
		#region Converter Class
		public class Converter : Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return Color.Parse (value);
			}

			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				return Color.ToString ((Color) value);
			}
		}
		#endregion
		
		public static readonly int				ColorComponentDigits = 4;
		public static readonly double			ColorComponentDelta  = (1.0 / 65535.0) / 2.0;

		private double							r, g, b;
		private double							a;
		private bool isEmpty
		{
			get
			{
				return double.IsNaN (this.a);
			}
			set
			{
				if (value)
				{
					this.a = double.NaN;
				}
				else
				{
					System.Diagnostics.Debug.Assert (this.isEmpty == false);
				}
			}
		}
	}
}
