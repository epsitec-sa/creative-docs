namespace Epsitec.Common.Drawing
{
	public enum ColorSpace
	{
		None = 0,
		RGB  = 1,
		CMYK = 2,
		Gray = 3,
	}

	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (RichColor.Converter))]
	
	/// <summary>
	/// Cette classe représente une couleur RGB ou CMYK.
	/// </summary>
	public struct RichColor : System.Runtime.Serialization.ISerializable
	{
		// Constructeur donnant une couleur RGB quelconque.
		public RichColor(Color color)
		{
			if ( color.IsEmpty )
			{
				this.colorSpace = ColorSpace.RGB;
				this.alpha  = 1.0;
				this.value1 = 0.0;
				this.value2 = 0.0;
				this.value3 = 0.0;
				this.value4 = 0.0;
				this.isEmpty = true;
				this.name = null;
			}
			else
			{
				this.colorSpace = ColorSpace.RGB;
				this.alpha  = color.A;
				this.value1 = color.R;
				this.value2 = color.G;
				this.value3 = color.B;
				this.value4 = 0.0;
				this.isEmpty = false;
				this.name = null;
			}
		}

		// Constructeur donnant une couleur RGB quelconque.
		public RichColor(double a, double r, double g, double b)
		{
			this.colorSpace = ColorSpace.RGB;
			this.alpha  = a;
			this.value1 = r;
			this.value2 = g;
			this.value3 = b;
			this.value4 = 0.0;
			this.isEmpty = false;
			this.name = null;
		}

		// Constructeur donnant une couleur RGB quelconque.
		public RichColor(double r, double g, double b)
		{
			this.colorSpace = ColorSpace.RGB;
			this.alpha  = 1.0;
			this.value1 = r;
			this.value2 = g;
			this.value3 = b;
			this.value4 = 0.0;
			this.isEmpty = false;
			this.name = null;
		}

		// Constructeur donnant une couleur RGB grise.
		public RichColor(double brightness)
		{
			this.colorSpace = ColorSpace.RGB;
			this.alpha  = 1.0;
			this.value1 = brightness;
			this.value2 = brightness;
			this.value3 = brightness;
			this.value4 = 0.0;
			this.isEmpty = false;
			this.name = null;
		}

		// Constructeur donnant une couleur CMYK quelconque.
		public RichColor(double a, double c, double m, double y, double k)
		{
			this.colorSpace = ColorSpace.CMYK;
			this.alpha  = a;
			this.value1 = c;
			this.value2 = m;
			this.value3 = y;
			this.value4 = k;
			this.isEmpty = false;
			this.name = null;
		}

		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
		}
		
		public bool IsValid
		{
			get
			{
				return !this.isEmpty;
			}
		}
		
		public bool IsTransparent
		{
			get
			{
				return !this.isEmpty && this.alpha == 0.0;
			}
		}
		
		public bool IsOpaque
		{
			get
			{
				return !this.isEmpty && this.alpha == 1.0;
			}
		}
		
		public bool IsVisible
		{
			get
			{
				return !this.isEmpty && this.alpha != 0.0;
			}
		}
		
		// Espace de couleur.
		public ColorSpace ColorSpace
		{
			get
			{
				return this.colorSpace;
			}

			set
			{
				if ( this.colorSpace != value )
				{
					if ( this.colorSpace == ColorSpace.RGB )
					{
						if ( value == ColorSpace.CMYK )  this.RGB2CMYK();
						if ( value == ColorSpace.Gray )  this.RGB2Gray();
					}

					if ( this.colorSpace == ColorSpace.CMYK )
					{
						if ( value == ColorSpace.RGB  )  this.CMYK2RGB();
						if ( value == ColorSpace.Gray )  this.CMYK2Gray();
					}

					if ( this.colorSpace == ColorSpace.Gray )
					{
						if ( value == ColorSpace.RGB  )  this.Gray2RGB();
						if ( value == ColorSpace.CMYK )  this.Gray2CMYK();
					}

					this.colorSpace = value;
				}
			}
		}

		// Retourne la couleur de base Color.
		public Color Basic
		{
			get
			{
				double r, g, b;
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						return Color.FromARGB(this.alpha, this.value1, this.value2, this.value3);

					case ColorSpace.CMYK:
						RichColor.CMYK2RGB(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						return Color.FromARGB(this.alpha, r, g, b);

					case ColorSpace.Gray:
						RichColor.Gray2RGB(this.value1, out r, out g, out b);
						return Color.FromARGB(this.alpha, r, g, b);
				}

				return Color.Empty;
			}

			set
			{
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						this.alpha  = value.A;
						this.value1 = value.R;
						this.value2 = value.G;
						this.value3 = value.B;
						this.value4 = 0.0;
						break;

					case ColorSpace.CMYK:
						double c, m, y, k;
						RichColor.RGB2CMYK(value.R, value.G, value.B, out c, out m, out y, out k);
						this.alpha  = value.A;
						this.value1 = c;
						this.value2 = m;
						this.value3 = y;
						this.value4 = k;
						break;

					case ColorSpace.Gray:
						double gray;
						RichColor.RGB2Gray(value.R, value.G, value.B, out gray);
						this.alpha  = value.A;
						this.value1 = gray;
						this.value2 = 0.0;
						this.value3 = 0.0;
						this.value4 = 0.0;
						break;
				}
			}
		}


		// Alpha channel (0=transparent, 1=opaque).
		public double A
		{
			get
			{
				return this.alpha;
			}
			
			set
			{
				this.alpha = value;
			}
		}
		
		// Color red (0=nothing, 1=red).
		public double R
		{
			get
			{
				double r, g, b;
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						return this.value1;

					case ColorSpace.CMYK:
						RichColor.CMYK2RGB(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						return r;

					case ColorSpace.Gray:
						RichColor.Gray2RGB(this.value1, out r, out g, out b);
						return r;
				}
				return 0.0;
			}
			
			set
			{
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						this.value1 = value;
						break;

					case ColorSpace.CMYK:
						double r, g, b;
						RichColor.CMYK2RGB(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						r = value;
						RichColor.RGB2CMYK(r, g, b, out this.value1, out this.value2, out this.value3, out this.value4);
						break;

					case ColorSpace.Gray:
						this.value1 = value;
						break;
				}
			}
		}
		
		// Color green (0=nothing, 1=green).
		public double G
		{
			get
			{
				double r, g, b;
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						return this.value2;

					case ColorSpace.CMYK:
						RichColor.CMYK2RGB(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						return g;

					case ColorSpace.Gray:
						RichColor.Gray2RGB(this.value1, out r, out g, out b);
						return g;
				}
				return 0.0;
			}
			
			set
			{
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						this.value2 = value;
						break;

					case ColorSpace.CMYK:
						double r, g, b;
						RichColor.CMYK2RGB(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						g = value;
						RichColor.RGB2CMYK(r, g, b, out this.value1, out this.value2, out this.value3, out this.value4);
						break;

					case ColorSpace.Gray:
						this.value1 = value;
						break;
				}
			}
		}
		
		// Color blue (0=nothing, 1=blue).
		public double B
		{
			get
			{
				double r, g, b;
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						return this.value3;

					case ColorSpace.CMYK:
						RichColor.CMYK2RGB(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						return b;

					case ColorSpace.Gray:
						RichColor.Gray2RGB(this.value1, out r, out g, out b);
						return b;
				}
				return 0.0;
			}
			
			set
			{
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						this.value3 = value;
						break;

					case ColorSpace.CMYK:
						double r, g, b;
						RichColor.CMYK2RGB(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						b = value;
						RichColor.RGB2CMYK(r, g, b, out this.value1, out this.value2, out this.value3, out this.value4);
						break;

					case ColorSpace.Gray:
						this.value1 = value;
						break;
				}
			}
		}
		
		// Color cyan (0=nothing, 1=cyan).
		public double C
		{
			get
			{
				double c, m, y, k;
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						RichColor.RGB2CMYK(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						return c;

					case ColorSpace.CMYK:
						return this.value1;

					case ColorSpace.Gray:
						RichColor.Gray2CMYK(this.value1, out c, out m, out y, out k);
						return c;
				}
				return 0.0;
			}
			
			set
			{
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						double c, m, y, k;
						RichColor.RGB2CMYK(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						c = value;
						RichColor.CMYK2RGB(c, m, y, k, out this.value1, out this.value2, out this.value3);
						break;

					case ColorSpace.CMYK:
						this.value1 = value;
						break;

					case ColorSpace.Gray:
						this.value1 = 1.0-value;
						break;
				}
			}
		}
		
		// Color magenta (0=nothing, 1=magenta).
		public double M
		{
			get
			{
				double c, m, y, k;
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						RichColor.RGB2CMYK(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						return m;

					case ColorSpace.CMYK:
						return this.value2;

					case ColorSpace.Gray:
						RichColor.Gray2CMYK(this.value1, out c, out m, out y, out k);
						return m;
				}
				return 0.0;
			}
			
			set
			{
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						double c, m, y, k;
						RichColor.RGB2CMYK(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						m = value;
						RichColor.CMYK2RGB(c, m, y, k, out this.value1, out this.value2, out this.value3);
						break;

					case ColorSpace.CMYK:
						this.value2 = value;
						break;

					case ColorSpace.Gray:
						this.value1 = 1.0-value;
						break;
				}
			}
		}
		
		// Color yellow (0=nothing, 1=yellow).
		public double Y
		{
			get
			{
				double c, m, y, k;
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						RichColor.RGB2CMYK(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						return y;

					case ColorSpace.CMYK:
						return this.value3;

					case ColorSpace.Gray:
						RichColor.Gray2CMYK(this.value1, out c, out m, out y, out k);
						return y;
				}
				return 0.0;
			}
			
			set
			{
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						double c, m, y, k;
						RichColor.RGB2CMYK(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						y = value;
						RichColor.CMYK2RGB(c, m, y, k, out this.value1, out this.value2, out this.value3);
						break;

					case ColorSpace.CMYK:
						this.value3 = value;
						break;

					case ColorSpace.Gray:
						this.value1 = 1.0-value;
						break;
				}
			}
		}
		
		// Color black (0=nothing, 1=black).
		public double K
		{
			get
			{
				double c, m, y, k;
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						RichColor.RGB2CMYK(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						return k;

					case ColorSpace.CMYK:
						return this.value4;

					case ColorSpace.Gray:
						RichColor.Gray2CMYK(this.value1, out c, out m, out y, out k);
						return k;
				}
				return 0.0;
			}
			
			set
			{
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						double c, m, y, k;
						RichColor.RGB2CMYK(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						k = value;
						RichColor.CMYK2RGB(c, m, y, k, out this.value1, out this.value2, out this.value3);
						break;

					case ColorSpace.CMYK:
						this.value4 = value;
						break;

					case ColorSpace.Gray:
						this.value1 = 1.0-value;
						break;
				}
			}
		}
		
		// Color gray (0=k, 1=white).
		public double Gray
		{
			get
			{
				double gray;
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						RichColor.RGB2Gray(this.value1, this.value2, this.value3, out gray);
						return gray;

					case ColorSpace.CMYK:
						RichColor.CMYK2Gray(this.value1, this.value2, this.value3, this.value4, out gray);
						return gray;

					case ColorSpace.Gray:
						return this.value1;
				}
				return 0.0;
			}
			
			set
			{
				switch ( this.colorSpace )
				{
					case ColorSpace.RGB:
						this.value1 = value;
						this.value2 = value;
						this.value3 = value;
						break;

					case ColorSpace.CMYK:
						this.value1 = 0.0;
						this.value2 = 0.0;
						this.value3 = 0.0;
						this.value4 = 1.0-value;
						break;

					case ColorSpace.Gray:
						this.value1 = value;
						break;
				}
			}
		}

		
		public string Name
		{
			get
			{
				return this.name;
			}

			set
			{
				this.name = value;
			}
		}

		// Change la luminosité d'une couleur.
		public void ChangeBrightness(double adjust)
		{
			switch ( this.colorSpace )
			{
				case ColorSpace.RGB:
					this.value1 = Epsitec.Common.Math.Clip(this.value1+adjust);
					this.value2 = Epsitec.Common.Math.Clip(this.value2+adjust);
					this.value3 = Epsitec.Common.Math.Clip(this.value3+adjust);
					break;

				case ColorSpace.CMYK:
					this.CMYK2RGB();
					this.value1 = Epsitec.Common.Math.Clip(this.value1+adjust);
					this.value2 = Epsitec.Common.Math.Clip(this.value2+adjust);
					this.value3 = Epsitec.Common.Math.Clip(this.value3+adjust);
					this.RGB2CMYK();
					break;

				case ColorSpace.Gray:
					this.value1 = Epsitec.Common.Math.Clip(this.value1+adjust);
					break;
			}
		}
		

		// Conversion RGB -> CMYK.
		private void RGB2CMYK()
		{
			double c, m, y, k;
			RichColor.RGB2CMYK(this.value1, this.value2, this.value3, out c, out m, out y, out k);
			this.value1 = c;
			this.value2 = m;
			this.value3 = y;
			this.value4 = k;
		}

		// Conversion CMYK -> RGB.
		private void CMYK2RGB()
		{
			double r, g, b;
			RichColor.CMYK2RGB(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
			this.value1 = r;
			this.value2 = g;
			this.value3 = b;
			this.value4 = 0.0;
		}

		// Conversion RGB -> Gray.
		private void RGB2Gray()
		{
			double gray;
			RichColor.RGB2Gray(this.value1, this.value2, this.value3, out gray);
			this.value1 = gray;
			this.value2 = 0.0;
			this.value3 = 0.0;
			this.value4 = 0.0;
		}

		// Conversion Gray -> RGB.
		private void Gray2RGB()
		{
			double r, g, b;
			RichColor.Gray2RGB(this.value1, out r, out g, out b);
			this.value1 = r;
			this.value2 = g;
			this.value3 = b;
			this.value4 = 0.0;
		}

		// Conversion CMYK -> Gray.
		private void CMYK2Gray()
		{
			double gray;
			RichColor.CMYK2Gray(this.value1, this.value2, this.value3, this.value4, out gray);
			this.value1 = gray;
			this.value2 = 0.0;
			this.value3 = 0.0;
			this.value4 = 0.0;
		}

		// Conversion Gray -> CMYK.
		private void Gray2CMYK()
		{
			double c, m, y, k;
			RichColor.Gray2CMYK(this.value1, out c, out m, out y, out k);
			this.value1 = c;
			this.value2 = m;
			this.value3 = y;
			this.value4 = k;
		}

		static public void RGB2CMYK(double r, double g, double b,
									out double c, out double m, out double y, out double k)
		{
			c = Epsitec.Common.Math.Clip(1.0-r);
			m = Epsitec.Common.Math.Clip(1.0-g);
			y = Epsitec.Common.Math.Clip(1.0-b);

			k = System.Math.Min(c, System.Math.Min(m, y));

			c -= k;
			m -= k;
			y -= k;
		}

		static public void CMYK2RGB(double c, double m, double y, double k,
									out double r, out double g, out double b)
		{
			c += k;
			m += k;
			y += k;

			r = Epsitec.Common.Math.Clip(1.0-c);
			g = Epsitec.Common.Math.Clip(1.0-m);
			b = Epsitec.Common.Math.Clip(1.0-y);
		}

		static private void RGB2Gray(double r, double g, double b,
									 out double gray)
		{
			gray = Color.GetBrightness(r, g, b);
		}

		static private void Gray2RGB(double gray,
									 out double r, out double g, out double b)
		{
			r = gray;
			g = gray;
			b = gray;
		}

		static private void CMYK2Gray(double c, double m, double y, double k,
									  out double gray)
		{
			double r, g, b;
			RichColor.CMYK2RGB(c, m, y, k, out r, out g, out b);
			gray = Color.GetBrightness(r, g, b);
		}

		static private void Gray2CMYK(double gray,
									  out double c, out double m, out double y, out double k)
		{
			c = 0.0;
			m = 0.0;
			y = 0.0;
			k = 1.0-gray;
		}


		public static RichColor Empty
		{
			get
			{
				RichColor c = new RichColor();
				c.isEmpty = true;
				return c;
			}
		}
		
		public static RichColor FromColor(Color color)
		{
			return new RichColor(color.A, color.R, color.G, color.B);
		}
		
		public static RichColor FromColor(Color color, double alpha)
		{
			return new RichColor(color.A*alpha, color.R, color.G, color.B);
		}
		
		public static RichColor FromARGB(double a, double r, double g, double b)
		{
			return new RichColor(a, r, g, b);
		}
		
		public static RichColor FromRGB(double r, double g, double b)
		{
			return new RichColor(r, g, b);
		}
		
		public static RichColor FromBrightness(double brightness)
		{
			return new RichColor(brightness);
		}
		
		public static RichColor FromHSV(double h, double s, double v)
		{
			double r,g,b;
			Color.ConvertHSVtoRGB(h,s,v, out r, out g, out b);
			return new RichColor(r, g, b);
		}

		public static RichColor FromAHSV(double a, double h, double s, double v)
		{
			RichColor color = RichColor.FromHSV(h, s, v);
			color.A = a;
			return color;
		}
		
		public static RichColor FromACMYK(double a, double c, double m, double y, double k)
		{
			return new RichColor(a, c, m, y, k);
		}
		
		public static RichColor FromCMYK(double c, double m, double y, double k)
		{
			RichColor color = new RichColor();

			color.colorSpace = ColorSpace.CMYK;
			color.alpha  = 1.0;
			color.value1 = c;
			color.value2 = m;
			color.value3 = y;
			color.value4 = k;
			color.isEmpty = false;

			return color;
		}
		
		public static RichColor FromAGray(double a, double gray)
		{
			RichColor color = new RichColor();

			color.colorSpace = ColorSpace.Gray;
			color.alpha  = a;
			color.value1 = gray;
			color.value2 = 0.0;
			color.value3 = 0.0;
			color.value4 = 0.0;
			color.isEmpty = false;

			return color;
		}
		
		public static RichColor FromGray(double gray)
		{
			RichColor color = new RichColor();

			color.colorSpace = ColorSpace.Gray;
			color.alpha  = 1.0;
			color.value1 = gray;
			color.value2 = 0.0;
			color.value3 = 0.0;
			color.value4 = 0.0;
			color.isEmpty = false;

			return color;
		}
		
		// Conversion d'un nom système ou hexa en une couleur.
		public static RichColor FromName(string name)
		{
			if ( name.Length > 1 && name[0] == '#' )
			{
				return RichColor.FromHexa(name.Remove(0, 1));
			}
			
			System.Drawing.Color color = System.Drawing.Color.FromName(name);
			
			if ( color.IsEmpty )
			{
				return RichColor.Empty;
			}
			
			return RichColor.FromColor(new Color(color));
		}

		// Conversion d'une chaîne "FF3300" ou "003300FF" en une couleur.
		public static RichColor FromHexa(string hexa)
		{
			try
			{
				if ( hexa.Length == 6 )  // rgb ?
				{
					byte r = System.Convert.ToByte(hexa.Substring(0, 2), 16);
					byte g = System.Convert.ToByte(hexa.Substring(2, 2), 16);
					byte b = System.Convert.ToByte(hexa.Substring(4, 2), 16);
					return RichColor.FromRGB(r/255.0, g/255.0, b/255.0);
				}

				if ( hexa.Length == 8 )  // cmyk ?
				{
					byte c = System.Convert.ToByte(hexa.Substring(0, 2), 16);
					byte m = System.Convert.ToByte(hexa.Substring(2, 2), 16);
					byte y = System.Convert.ToByte(hexa.Substring(4, 2), 16);
					byte k = System.Convert.ToByte(hexa.Substring(6, 2), 16);
					return RichColor.FromCMYK(c/255.0, m/255.0, y/255.0, k/255.0);
				}

				if ( hexa.Length == 2 )  // gray ?
				{
					byte g = System.Convert.ToByte(hexa.Substring(0, 2), 16);
					return RichColor.FromGray(g/255.0);
				}

				return RichColor.Empty;
			}
			catch
			{
				return RichColor.Empty;
			}
		}

		// Conversion d'une couleur en chaîne "FF3300" ou "003300FF".
		public static string ToHexa(RichColor color)
		{
			if ( color.colorSpace == ColorSpace.RGB )
			{
				int r = (int)(color.value1*255.0+0.5);
				int g = (int)(color.value2*255.0+0.5);
				int b = (int)(color.value3*255.0+0.5);
				return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
			}

			if ( color.colorSpace == ColorSpace.CMYK )
			{
				int c = (int)(color.value1*255.0+0.5);
				int m = (int)(color.value2*255.0+0.5);
				int y = (int)(color.value3*255.0+0.5);
				int k = (int)(color.value4*255.0+0.5);
				return c.ToString("X2") + m.ToString("X2") + y.ToString("X2") + k.ToString("X2");
			}

			if ( color.colorSpace == ColorSpace.Gray )
			{
				int g = (int)(color.value1*255.0+0.5);
				return g.ToString("X2");
			}

			return "??";
		}


		public static RichColor Parse(string value)
		{
			if (value == null)
			{
				return RichColor.Empty;
			}
			
			string[] args = value.Split (';');
			
			switch (args[0])
			{
				case "G":
					if (args.Length == 2)
					{
						return RichColor.FromAGray (1.0, Color.ColorComponentToDouble (args[1]));
					}
					break;
				
				case "αG":
					if (args.Length == 3)
					{
						return RichColor.FromAGray (Color.ColorComponentToDouble (args[1]), Color.ColorComponentToDouble (args[2]));
					}
					break;
				
				case "RGB":
					if (args.Length == 4)
					{
						return RichColor.FromARGB (1.0, Color.ColorComponentToDouble (args[1]), Color.ColorComponentToDouble (args[2]), Color.ColorComponentToDouble (args[3]));
					}
					break;
				
				case "αRGB":
					if (args.Length == 5)
					{
						return RichColor.FromARGB (Color.ColorComponentToDouble (args[1]), Color.ColorComponentToDouble (args[2]), Color.ColorComponentToDouble (args[3]), Color.ColorComponentToDouble (args[4]));
					}
					break;
				
				case "CMYK":
					if (args.Length == 5)
					{
						return RichColor.FromACMYK (1.0, Color.ColorComponentToDouble (args[1]), Color.ColorComponentToDouble (args[2]), Color.ColorComponentToDouble (args[3]), Color.ColorComponentToDouble (args[4]));
					}
					break;
				
				case "αCMYK":
					if (args.Length == 6)
					{
						return RichColor.FromACMYK (Color.ColorComponentToDouble (args[1]), Color.ColorComponentToDouble (args[2]), Color.ColorComponentToDouble (args[3]), Color.ColorComponentToDouble (args[4]), Color.ColorComponentToDouble (args[5]));
					}
					break;
			}
			
			throw new System.ArgumentException (string.Format ("Invalid color specification ({0}).", value));
		}
		
		public static string ToString(RichColor color)
		{
			switch (color.ColorSpace)
			{
				case ColorSpace.Gray:
					if (color.IsOpaque)
					{
						return string.Format (System.Globalization.CultureInfo.InvariantCulture, "G;{0}", Color.DoubleToColorComponent (color.Gray));
					}
					else
					{
						return string.Format (System.Globalization.CultureInfo.InvariantCulture, "αG;{0};{1}", Color.DoubleToColorComponent (color.A), Color.DoubleToColorComponent (color.Gray));
					}
				
				case ColorSpace.RGB:
					if (color.IsOpaque)
					{
						return string.Format (System.Globalization.CultureInfo.InvariantCulture, "RGB;{0};{1};{2}", Color.DoubleToColorComponent (color.R), Color.DoubleToColorComponent (color.G), Color.DoubleToColorComponent (color.B));
					}
					else
					{
						return string.Format (System.Globalization.CultureInfo.InvariantCulture, "αRGB;{0};{1};{2};{3}", Color.DoubleToColorComponent (color.A), Color.DoubleToColorComponent (color.R), Color.DoubleToColorComponent (color.G), Color.DoubleToColorComponent (color.B));
					}
				
				case ColorSpace.CMYK:
					if (color.IsOpaque)
					{
						return string.Format (System.Globalization.CultureInfo.InvariantCulture, "CMYK;{0};{1};{2};{3}", Color.DoubleToColorComponent (color.C), Color.DoubleToColorComponent (color.M), Color.DoubleToColorComponent (color.Y), Color.DoubleToColorComponent (color.K));
					}
					else
					{
						return string.Format (System.Globalization.CultureInfo.InvariantCulture, "αCMYK;{0};{1};{2};{3};{4}", Color.DoubleToColorComponent (color.A), Color.DoubleToColorComponent (color.C), Color.DoubleToColorComponent (color.M), Color.DoubleToColorComponent (color.Y), Color.DoubleToColorComponent (color.K));
					}
				
				default:
					throw new System.NotSupportedException (string.Format ("ColorSpace.{0} not supported", color.ColorSpace));
			}
		}
		
		
		public static bool operator == (RichColor a, RichColor b)
		{
			if ( a.isEmpty && b.isEmpty )  return true;
			if ( a.isEmpty || b.isEmpty )  return false;
			
			if ( a.colorSpace != b.colorSpace )  return false;

			switch ( a.colorSpace )
			{
				case ColorSpace.RGB:
					return Types.Comparer.Equal (a.alpha,  b.alpha,  Color.ColorComponentDelta)
						&& Types.Comparer.Equal (a.value1, b.value1, Color.ColorComponentDelta)
						&& Types.Comparer.Equal (a.value2, b.value2, Color.ColorComponentDelta)
						&& Types.Comparer.Equal (a.value3, b.value3, Color.ColorComponentDelta);

				case ColorSpace.CMYK:
					return Types.Comparer.Equal (a.alpha,  b.alpha,  Color.ColorComponentDelta)
						&& Types.Comparer.Equal (a.value1, b.value1, Color.ColorComponentDelta)
						&& Types.Comparer.Equal (a.value2, b.value2, Color.ColorComponentDelta)
						&& Types.Comparer.Equal (a.value3, b.value3, Color.ColorComponentDelta)
						&& Types.Comparer.Equal (a.value4, b.value4, Color.ColorComponentDelta);

				case ColorSpace.Gray:
					return Types.Comparer.Equal (a.alpha,  b.alpha,  Color.ColorComponentDelta)
						&& Types.Comparer.Equal (a.value1, b.value1, Color.ColorComponentDelta);
			}
			return true;
		}
		
		public static bool operator != (RichColor a, RichColor b)
		{
			return ! (a == b);
		}
		

		public override bool Equals(object obj)
		{
			if ( obj == null || obj.GetType() != typeof(RichColor) )
			{
				return false;
			}
			
			RichColor color = (RichColor) obj;
			return this == color;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			switch ( this.colorSpace )
			{
				case ColorSpace.RGB:
					return System.String.Format ("{{R={0:0.00},G={1:0.00},B={2:0.00},A={3:0.00}}}", this.value1, this.value2, this.value3, this.alpha);

				case ColorSpace.CMYK:
					return System.String.Format ("{{C={0:0.00},M={1:0.00},Y={2:0.00},K={3:0.00},A={4:0.00}}}", this.value1, this.value2, this.value3, this.value4, this.alpha);

				case ColorSpace.Gray:
					return System.String.Format ("{{G={0:0.00},A={1:0.00}}}", this.value1, this.alpha);
			}

			return "?";
		}
		

		#region Converter Class
		public class Converter : Epsitec.Common.Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return RichColor.Parse (value);
			}
			
			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				return RichColor.ToString ((RichColor) value);
			}
		}
		#endregion
		
		#region ISerializable Members
		public RichColor(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
#if false
			try
			{
				object o1 = info.GetValue ("ColorSpace", typeof (object));
			
				this.colorSpace = (ColorSpace) info.GetValue ("ColorSpace", typeof (ColorSpace));
			
				this.alpha  = info.GetDouble ("Alpha");
				this.value1 = info.GetDouble ("V1");
				this.value2 = info.GetDouble ("V2");
				this.value3 = info.GetDouble ("V3");
				this.value4 = info.GetDouble ("V4");
				this.isEmpty = info.GetBoolean ("IsEmpty");
				this.name = info.GetString ("Name");
			}
			catch (System.Runtime.Serialization.SerializationException)
			{
				object o1 = info.GetValue ("colorSpace", typeof (object));
			
				if (o1.GetType () == typeof (string))
				{
					this.colorSpace = (ColorSpace) System.Enum.Parse (typeof (ColorSpace), info.GetString ("colorSpace"), false);
				
					this.alpha  = System.Double.Parse (info.GetString ("alpha"), System.Globalization.CultureInfo.InvariantCulture);
					this.value1 = System.Double.Parse (info.GetString ("value1"), System.Globalization.CultureInfo.InvariantCulture);
					this.value2 = System.Double.Parse (info.GetString ("value2"), System.Globalization.CultureInfo.InvariantCulture);
					this.value3 = System.Double.Parse (info.GetString ("value3"), System.Globalization.CultureInfo.InvariantCulture);
					this.value4 = System.Double.Parse (info.GetString ("value4"), System.Globalization.CultureInfo.InvariantCulture);
					this.isEmpty = info.GetString ("isEmpty") == "true";
					this.name = null;
				}
				else
				{
					this.colorSpace = (ColorSpace) info.GetValue ("colorSpace", typeof (ColorSpace));
				
					this.alpha  = info.GetDouble ("alpha");
					this.value1 = info.GetDouble ("value1");
					this.value2 = info.GetDouble ("value2");
					this.value3 = info.GetDouble ("value3");
					this.value4 = info.GetDouble ("value4");
					this.isEmpty = info.GetBoolean ("isEmpty");
					this.name = null;
				}
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Deserialized old-style RichColor: {0} - {1}/{2}/{3}/{4}", this.colorSpace, this.value1, this.value2, this.value3, this.value4));
			}
#else
			this.colorSpace = (ColorSpace) info.GetValue("ColorSpace", typeof(ColorSpace));
			this.alpha  = info.GetDouble("Alpha");
			this.value1 = info.GetDouble("V1");
			this.value2 = info.GetDouble("V2");
			this.value3 = info.GetDouble("V3");
			this.value4 = info.GetDouble("V4");
			this.isEmpty = info.GetBoolean("IsEmpty");
			this.name = info.GetString("Name");
#endif
		}
		
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("ColorSpace", this.colorSpace, typeof(ColorSpace));
			info.AddValue("Alpha", this.alpha);
			info.AddValue("V1", this.value1);
			info.AddValue("V2", this.value2);
			info.AddValue("V3", this.value3);
			info.AddValue("V4", this.value4);
			info.AddValue("IsEmpty", this.isEmpty);
			info.AddValue("Name", this.name);
		}
		#endregion

		
		private ColorSpace					colorSpace;
		private double						alpha;
		private double						value1;  // red or cyan or gray
		private double						value2;  // green or magenta
		private double						value3;  // blue or yellow
		private double						value4;  // black
		private bool						isEmpty;
		private string						name;
		
		
	}
}
