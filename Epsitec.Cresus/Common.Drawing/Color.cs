namespace Epsitec.Common.Drawing
{
	public struct Color
	{
		public Color(System.Drawing.Color color)
		{
			this.r = color.R / 255.0f;
			this.g = color.G / 255.0f;
			this.b = color.B / 255.0f;
			this.a = color.A / 255.0f;
			this.is_empty = color.IsEmpty;
		}
		
		public Color(double a, double r, double g, double b)
		{
			this.r = (float) r;
			this.g = (float) g;
			this.b = (float) b;
			this.a = (float) a;
			this.is_empty = false;
		}
		
		public Color(double r, double g, double b)
		{
			this.r = (float) r;
			this.g = (float) g;
			this.b = (float) b;
			this.a = 1.0f;
			this.is_empty = false;
		}
		
		public Color(double brightness)
		{
			this.r = (float) brightness;
			this.g = (float) brightness;
			this.b = (float) brightness;
			this.a = 1.0f;
			this.is_empty = false;
		}
		
		
		public double					R
		{
			get { return this.r; }
			set { this.r = (float) value; }
		}
		
		public double					G
		{
			get { return this.g; }
			set { this.g = (float) value; }
		}
		
		public double					B
		{
			get { return this.b; }
			set { this.b = (float) value; }
		}
		
		public double					A
		{
			get { return this.a; }
			set { this.a = (float) value; }
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
			get { return this.a == 0.0; }
		}
		
		public bool						IsInRange
		{
			get
			{
				return (this.r >= 0.0f) && (this.r <= 1.0f)
					&& (this.g >= 0.0f) && (this.g <= 1.0f)
					&& (this.b >= 0.0f) && (this.b <= 1.0f)
					&& (this.a >= 0.0f) && (this.a <= 1.0f);
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
		
		
		public void ClipToRange()
		{
			if (this.r < 0.0f) this.r = 0.0f;
			if (this.r > 1.0f) this.r = 1.0f;
			if (this.g < 0.0f) this.g = 0.0f;
			if (this.g > 1.0f) this.g = 1.0f;
			if (this.b < 0.0f) this.b = 0.0f;
			if (this.b > 1.0f) this.b = 1.0f;
			if (this.a < 0.0f) this.a = 0.0f;
			if (this.a > 1.0f) this.a = 1.0f;
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
			Color.HSVtoRGB(h,s,v, out r, out g, out b);
			return new Color(r, g, b);
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


		// Conversion d'une chaîne "FF3300" en une couleur.
		public static Color FromHexa(string hexa)
		{
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


		public static void ToHSV(Color color, out double h, out double s, out double v)
		{
			Color.RGBtoHSV(color.r, color.g, color.b, out h, out s, out v);
		}
				
		public void ToHSV(out double h, out double s, out double v)
		{
			Color.RGBtoHSV(this.r, this.g, this.b, out h, out s, out v);
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
		
		
		// H = [0..360]
		// S = [0..1]
		// V = [0..1]
		public static void RGBtoHSV(double r, double g, double b, out double h, out double s, out double v)
		{
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

		public static void HSVtoRGB(double h, double s, double v, out double r, out double g, out double b)
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

		
		private float					r, g, b, a;
		private bool					is_empty;
	}
}
