//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (RichColor.Converter))]
	
	/// <summary>
	/// The <c>RichColor</c> structure represents an RGB or CMYK color, or
	/// a grayscale value.
	/// </summary>
	public struct RichColor : System.Runtime.Serialization.ISerializable
	{
		public RichColor(Color color)
		{
			//	Constructeur donnant une couleur RGB quelconque.
			if (color.IsEmpty)
			{
				this.colorSpace = ColorSpace.Rgb;
				this.alpha  = 1.0;
				this.value1 = 0.0;
				this.value2 = 0.0;
				this.value3 = 0.0;
				this.value4 = 0.0;
				this.isValid = false;
				this.name = null;
			}
			else
			{
				this.colorSpace = ColorSpace.Rgb;
				this.alpha  = color.A;
				this.value1 = color.R;
				this.value2 = color.G;
				this.value3 = color.B;
				this.value4 = 0.0;
				this.isValid = true;
				this.name = null;
			}
		}

		public RichColor(double a, double r, double g, double b)
		{
			//	Constructeur donnant une couleur RGB quelconque.
			this.colorSpace = ColorSpace.Rgb;
			this.alpha  = a;
			this.value1 = r;
			this.value2 = g;
			this.value3 = b;
			this.value4 = 0.0;
			this.isValid = true;
			this.name = null;
		}

		public RichColor(double r, double g, double b)
		{
			//	Constructeur donnant une couleur RGB quelconque.
			this.colorSpace = ColorSpace.Rgb;
			this.alpha  = 1.0;
			this.value1 = r;
			this.value2 = g;
			this.value3 = b;
			this.value4 = 0.0;
			this.isValid = true;
			this.name = null;
		}

		public RichColor(double brightness)
		{
			//	Constructeur donnant une couleur RGB grise.
			this.colorSpace = ColorSpace.Rgb;
			this.alpha  = 1.0;
			this.value1 = brightness;
			this.value2 = brightness;
			this.value3 = brightness;
			this.value4 = 0.0;
			this.isValid = true;
			this.name = null;
		}

		public RichColor(double a, double c, double m, double y, double k)
		{
			//	Constructeur donnant une couleur CMYK quelconque.
			this.colorSpace = ColorSpace.Cmyk;
			this.alpha  = a;
			this.value1 = c;
			this.value2 = m;
			this.value3 = y;
			this.value4 = k;
			this.isValid = true;
			this.name = null;
		}

		
		public bool								IsEmpty
		{
			get
			{
				return !this.isValid;
			}
		}
		
		public bool								IsValid
		{
			get
			{
				return this.isValid;
			}
		}
		
		public bool								IsTransparent
		{
			get
			{
				return this.isValid && this.alpha == 0.0;
			}
		}
		
		public bool								IsOpaque
		{
			get
			{
				return this.isValid && this.alpha == 1.0;
			}
		}
		
		public bool								IsVisible
		{
			get
			{
				return this.isValid && this.alpha != 0.0;
			}
		}
		
		public ColorSpace						ColorSpace
		{
			//	Espace de couleur.
			get
			{
				return this.colorSpace;
			}

			set
			{
				if (this.colorSpace != value)
				{
					if (this.colorSpace == ColorSpace.Rgb)
					{
						if (value == ColorSpace.Cmyk)  this.RgbToCmyk();
						if (value == ColorSpace.Gray)  this.RgbToGray();
					}

					if (this.colorSpace == ColorSpace.Cmyk)
					{
						if (value == ColorSpace.Rgb )  this.CmykToRgb();
						if (value == ColorSpace.Gray)  this.CmykToGray();
					}

					if (this.colorSpace == ColorSpace.Gray)
					{
						if (value == ColorSpace.Rgb )  this.GrayToRgb();
						if (value == ColorSpace.Cmyk)  this.GrayToCmyk();
					}

					this.colorSpace = value;
				}
			}
		}

		public Color							Basic
		{
			//	Retourne la couleur de base Color.
			get
			{
				double r, g, b;
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						return Color.FromAlphaRgb(this.alpha, this.value1, this.value2, this.value3);

					case ColorSpace.Cmyk:
						RichColor.CmykToRgb(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						return Color.FromAlphaRgb(this.alpha, r, g, b);

					case ColorSpace.Gray:
						RichColor.GrayToRgb(this.value1, out r, out g, out b);
						return Color.FromAlphaRgb(this.alpha, r, g, b);
				}

				return Color.Empty;
			}

			set
			{
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						this.alpha  = value.A;
						this.value1 = value.R;
						this.value2 = value.G;
						this.value3 = value.B;
						this.value4 = 0.0;
						break;

					case ColorSpace.Cmyk:
						double c, m, y, k;
						RichColor.RgbToCmyk(value.R, value.G, value.B, out c, out m, out y, out k);
						this.alpha  = value.A;
						this.value1 = c;
						this.value2 = m;
						this.value3 = y;
						this.value4 = k;
						break;

					case ColorSpace.Gray:
						double gray;
						RichColor.RgbToGray(value.R, value.G, value.B, out gray);
						this.alpha  = value.A;
						this.value1 = gray;
						this.value2 = 0.0;
						this.value3 = 0.0;
						this.value4 = 0.0;
						break;
				}
			}
		}


		public double							A
		{
			//	Alpha channel (0=transparent, 1=opaque).
			get
			{
				return this.alpha;
			}
			
			set
			{
				this.alpha = value;
			}
		}
		
		public double							R
		{
			//	Color red (0=nothing, 1=red).
			get
			{
				double r, g, b;
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						return this.value1;

					case ColorSpace.Cmyk:
						RichColor.CmykToRgb(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						return r;

					case ColorSpace.Gray:
						RichColor.GrayToRgb(this.value1, out r, out g, out b);
						return r;
				}
				return 0.0;
			}
			
			set
			{
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						this.value1 = value;
						break;

					case ColorSpace.Cmyk:
						double r, g, b;
						RichColor.CmykToRgb(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						r = value;
						RichColor.RgbToCmyk(r, g, b, out this.value1, out this.value2, out this.value3, out this.value4);
						break;

					case ColorSpace.Gray:
						this.value1 = value;
						break;
				}
			}
		}
		
		public double							G
		{
			//	Color green (0=nothing, 1=green).
			get
			{
				double r, g, b;
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						return this.value2;

					case ColorSpace.Cmyk:
						RichColor.CmykToRgb(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						return g;

					case ColorSpace.Gray:
						RichColor.GrayToRgb(this.value1, out r, out g, out b);
						return g;
				}
				return 0.0;
			}
			
			set
			{
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						this.value2 = value;
						break;

					case ColorSpace.Cmyk:
						double r, g, b;
						RichColor.CmykToRgb(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						g = value;
						RichColor.RgbToCmyk(r, g, b, out this.value1, out this.value2, out this.value3, out this.value4);
						break;

					case ColorSpace.Gray:
						this.value1 = value;
						break;
				}
			}
		}
		
		public double							B
		{
			//	Color blue (0=nothing, 1=blue).
			get
			{
				double r, g, b;
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						return this.value3;

					case ColorSpace.Cmyk:
						RichColor.CmykToRgb(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						return b;

					case ColorSpace.Gray:
						RichColor.GrayToRgb(this.value1, out r, out g, out b);
						return b;
				}
				return 0.0;
			}
			
			set
			{
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						this.value3 = value;
						break;

					case ColorSpace.Cmyk:
						double r, g, b;
						RichColor.CmykToRgb(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
						b = value;
						RichColor.RgbToCmyk(r, g, b, out this.value1, out this.value2, out this.value3, out this.value4);
						break;

					case ColorSpace.Gray:
						this.value1 = value;
						break;
				}
			}
		}
		
		public double							C
		{
			//	Color cyan (0=nothing, 1=cyan).
			get
			{
				double c, m, y, k;
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						RichColor.RgbToCmyk(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						return c;

					case ColorSpace.Cmyk:
						return this.value1;

					case ColorSpace.Gray:
						RichColor.GrayToCmyk(this.value1, out c, out m, out y, out k);
						return c;
				}
				return 0.0;
			}
			
			set
			{
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						double c, m, y, k;
						RichColor.RgbToCmyk(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						c = value;
						RichColor.CmykToRgb(c, m, y, k, out this.value1, out this.value2, out this.value3);
						break;

					case ColorSpace.Cmyk:
						this.value1 = value;
						break;

					case ColorSpace.Gray:
						this.value1 = 1.0-value;
						break;
				}
			}
		}
		
		public double							M
		{
			//	Color magenta (0=nothing, 1=magenta).
			get
			{
				double c, m, y, k;
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						RichColor.RgbToCmyk(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						return m;

					case ColorSpace.Cmyk:
						return this.value2;

					case ColorSpace.Gray:
						RichColor.GrayToCmyk(this.value1, out c, out m, out y, out k);
						return m;
				}
				return 0.0;
			}
			
			set
			{
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						double c, m, y, k;
						RichColor.RgbToCmyk(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						m = value;
						RichColor.CmykToRgb(c, m, y, k, out this.value1, out this.value2, out this.value3);
						break;

					case ColorSpace.Cmyk:
						this.value2 = value;
						break;

					case ColorSpace.Gray:
						this.value1 = 1.0-value;
						break;
				}
			}
		}
		
		public double							Y
		{
			//	Color yellow (0=nothing, 1=yellow).
			get
			{
				double c, m, y, k;
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						RichColor.RgbToCmyk(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						return y;

					case ColorSpace.Cmyk:
						return this.value3;

					case ColorSpace.Gray:
						RichColor.GrayToCmyk(this.value1, out c, out m, out y, out k);
						return y;
				}
				return 0.0;
			}
			
			set
			{
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						double c, m, y, k;
						RichColor.RgbToCmyk(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						y = value;
						RichColor.CmykToRgb(c, m, y, k, out this.value1, out this.value2, out this.value3);
						break;

					case ColorSpace.Cmyk:
						this.value3 = value;
						break;

					case ColorSpace.Gray:
						this.value1 = 1.0-value;
						break;
				}
			}
		}
		
		public double							K
		{
			//	Color black (0=nothing, 1=black).
			get
			{
				double c, m, y, k;
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						RichColor.RgbToCmyk(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						return k;

					case ColorSpace.Cmyk:
						return this.value4;

					case ColorSpace.Gray:
						RichColor.GrayToCmyk(this.value1, out c, out m, out y, out k);
						return k;
				}
				return 0.0;
			}
			
			set
			{
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						double c, m, y, k;
						RichColor.RgbToCmyk(this.value1, this.value2, this.value3, out c, out m, out y, out k);
						k = value;
						RichColor.CmykToRgb(c, m, y, k, out this.value1, out this.value2, out this.value3);
						break;

					case ColorSpace.Cmyk:
						this.value4 = value;
						break;

					case ColorSpace.Gray:
						this.value1 = 1.0-value;
						break;
				}
			}
		}
		
		public double							Gray
		{
			//	Color gray (0=k, 1=white).
			get
			{
				double gray;
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						RichColor.RgbToGray(this.value1, this.value2, this.value3, out gray);
						return gray;

					case ColorSpace.Cmyk:
						RichColor.CmykToGray(this.value1, this.value2, this.value3, this.value4, out gray);
						return gray;

					case ColorSpace.Gray:
						return this.value1;
				}
				return 0.0;
			}
			
			set
			{
				switch (this.colorSpace)
				{
					case ColorSpace.Rgb:
						this.value1 = value;
						this.value2 = value;
						this.value3 = value;
						break;

					case ColorSpace.Cmyk:
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

		
		public string							Name
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

		
		public static readonly RichColor		Empty = new RichColor ();
		
		
		public void ChangeBrightness(double adjust)
		{
			//	Change la luminosité d'une couleur.
			switch (this.colorSpace)
			{
				case ColorSpace.Rgb:
					this.value1 = Epsitec.Common.Math.Clip(this.value1+adjust);
					this.value2 = Epsitec.Common.Math.Clip(this.value2+adjust);
					this.value3 = Epsitec.Common.Math.Clip(this.value3+adjust);
					break;

				case ColorSpace.Cmyk:
					this.CmykToRgb();
					this.value1 = Epsitec.Common.Math.Clip(this.value1+adjust);
					this.value2 = Epsitec.Common.Math.Clip(this.value2+adjust);
					this.value3 = Epsitec.Common.Math.Clip(this.value3+adjust);
					this.RgbToCmyk();
					break;

				case ColorSpace.Gray:
					this.value1 = Epsitec.Common.Math.Clip(this.value1+adjust);
					break;
			}
		}
		

		private void RgbToCmyk()
		{
			//	Conversion RGB -> CMYK.
			double c, m, y, k;
			RichColor.RgbToCmyk(this.value1, this.value2, this.value3, out c, out m, out y, out k);
			this.value1 = c;
			this.value2 = m;
			this.value3 = y;
			this.value4 = k;
		}

		private void CmykToRgb()
		{
			//	Conversion CMYK -> RGB.
			double r, g, b;
			RichColor.CmykToRgb(this.value1, this.value2, this.value3, this.value4, out r, out g, out b);
			this.value1 = r;
			this.value2 = g;
			this.value3 = b;
			this.value4 = 0.0;
		}

		private void RgbToGray()
		{
			//	Conversion RGB -> Gray.
			double gray;
			RichColor.RgbToGray(this.value1, this.value2, this.value3, out gray);
			this.value1 = gray;
			this.value2 = 0.0;
			this.value3 = 0.0;
			this.value4 = 0.0;
		}

		private void GrayToRgb()
		{
			//	Conversion Gray -> RGB.
			double r, g, b;
			RichColor.GrayToRgb(this.value1, out r, out g, out b);
			this.value1 = r;
			this.value2 = g;
			this.value3 = b;
			this.value4 = 0.0;
		}

		private void CmykToGray()
		{
			//	Conversion CMYK -> Gray.
			double gray;
			RichColor.CmykToGray(this.value1, this.value2, this.value3, this.value4, out gray);
			this.value1 = gray;
			this.value2 = 0.0;
			this.value3 = 0.0;
			this.value4 = 0.0;
		}

		private void GrayToCmyk()
		{
			//	Conversion Gray -> CMYK.
			double c, m, y, k;
			RichColor.GrayToCmyk(this.value1, out c, out m, out y, out k);
			this.value1 = c;
			this.value2 = m;
			this.value3 = y;
			this.value4 = k;
		}

		
		static public void RgbToCmyk(double r, double g, double b, out double c, out double m, out double y, out double k)
		{
			c = Epsitec.Common.Math.Clip(1.0-r);
			m = Epsitec.Common.Math.Clip(1.0-g);
			y = Epsitec.Common.Math.Clip(1.0-b);

			k = System.Math.Min(c, System.Math.Min(m, y));

			c -= k;
			m -= k;
			y -= k;
		}

		static public void CmykToRgb(double c, double m, double y, double k, out double r, out double g, out double b)
		{
			c += k;
			m += k;
			y += k;

			r = Epsitec.Common.Math.Clip(1.0-c);
			g = Epsitec.Common.Math.Clip(1.0-m);
			b = Epsitec.Common.Math.Clip(1.0-y);
		}

		
		static public void RgbToGray(double r, double g, double b, out double gray)
		{
			gray = Color.GetBrightness(r, g, b);
		}

		static public void GrayToRgb(double gray, out double r, out double g, out double b)
		{
			r = gray;
			g = gray;
			b = gray;
		}

		static public void CmykToGray(double c, double m, double y, double k, out double gray)
		{
			double r, g, b;
			RichColor.CmykToRgb(c, m, y, k, out r, out g, out b);
			gray = Color.GetBrightness(r, g, b);
		}

		static public void GrayToCmyk(double gray, out double c, out double m, out double y, out double k)
		{
			c = 0.0;
			m = 0.0;
			y = 0.0;
			k = 1.0-gray;
		}


		public static RichColor FromColor(Color color)
		{
			return new RichColor(color.A, color.R, color.G, color.B);
		}
		
		public static RichColor FromColor(Color color, double alpha)
		{
			return new RichColor(color.A*alpha, color.R, color.G, color.B);
		}
		
		public static RichColor FromAlphaRgb(double a, double r, double g, double b)
		{
			return new RichColor(a, r, g, b);
		}
		
		public static RichColor FromRgb(double r, double g, double b)
		{
			return new RichColor(r, g, b);
		}
		
		public static RichColor FromBrightness(double brightness)
		{
			return new RichColor(brightness);
		}
		
		public static RichColor FromHsv(double h, double s, double v)
		{
			double r,g,b;
			Color.ConvertHsvToRgb(h,s,v, out r, out g, out b);
			return new RichColor(r, g, b);
		}

		public static RichColor FromAlphaHsv(double a, double h, double s, double v)
		{
			RichColor color = RichColor.FromHsv(h, s, v);
			color.A = a;
			return color;
		}
		
		public static RichColor FromAlphaCmyk(double a, double c, double m, double y, double k)
		{
			return new RichColor(a, c, m, y, k);
		}
		
		public static RichColor FromCmyk(double c, double m, double y, double k)
		{
			RichColor color = new RichColor();

			color.colorSpace = ColorSpace.Cmyk;
			color.alpha  = 1.0;
			color.value1 = c;
			color.value2 = m;
			color.value3 = y;
			color.value4 = k;
			color.isValid = true;

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
			color.isValid = true;

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
			color.isValid = true;

			return color;
		}
		
		public static RichColor FromName(string name)
		{
			//	Conversion d'un nom système ou hexa en une couleur.
			if (name.Length > 1 && name[0] == '#')
			{
				return RichColor.FromHexa(name.Remove(0, 1));
			}
			
			System.Drawing.Color color = System.Drawing.Color.FromName(name);
			
			if (color.IsEmpty)
			{
				return RichColor.Empty;
			}
			
			return RichColor.FromColor(new Color(color));
		}

		public static RichColor FromHexa(string hexa)
		{
			//	Conversion d'une chaîne "FF3300" ou "003300FF" en une couleur.
			try
			{
				if (hexa.Length == 6)  // rgb ?
				{
					byte r = System.Convert.ToByte(hexa.Substring(0, 2), 16);
					byte g = System.Convert.ToByte(hexa.Substring(2, 2), 16);
					byte b = System.Convert.ToByte(hexa.Substring(4, 2), 16);
					return RichColor.FromRgb(r/255.0, g/255.0, b/255.0);
				}

				if (hexa.Length == 8)  // cmyk ?
				{
					byte c = System.Convert.ToByte(hexa.Substring(0, 2), 16);
					byte m = System.Convert.ToByte(hexa.Substring(2, 2), 16);
					byte y = System.Convert.ToByte(hexa.Substring(4, 2), 16);
					byte k = System.Convert.ToByte(hexa.Substring(6, 2), 16);
					return RichColor.FromCmyk(c/255.0, m/255.0, y/255.0, k/255.0);
				}

				if (hexa.Length == 2)  // gray ?
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

		
		public static string ToHexa(RichColor color)
		{
			//	Conversion d'une couleur en chaîne "FF3300" ou "003300FF".
			if (color.colorSpace == ColorSpace.Rgb)
			{
				int r = (int)(color.value1*255.0+0.5);
				int g = (int)(color.value2*255.0+0.5);
				int b = (int)(color.value3*255.0+0.5);
				return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
			}

			if (color.colorSpace == ColorSpace.Cmyk)
			{
				int c = (int)(color.value1*255.0+0.5);
				int m = (int)(color.value2*255.0+0.5);
				int y = (int)(color.value3*255.0+0.5);
				int k = (int)(color.value4*255.0+0.5);
				return c.ToString("X2") + m.ToString("X2") + y.ToString("X2") + k.ToString("X2");
			}

			if (color.colorSpace == ColorSpace.Gray)
			{
				int g = (int)(color.value1*255.0+0.5);
				return g.ToString("X2");
			}

			return "??";
		}

		
		public static bool IsRichColor(string value)
		{
			//	Détermine si la string code une couleur riche qui peut être lue au
			//	moyen de la méthode RichColor.Parse.
			
			if ((value == null) ||
				(value.Length == 0))
			{
				return true;
			}
			
			string[] args = value.Split (';');
			
			switch (args[0])
			{
				case "G":		return (args.Length == 2);
				case "αG":		return (args.Length == 3);
				case "RGB":		return (args.Length == 4);
				case "αRGB":	return (args.Length == 5);
				case "CMYK":	return (args.Length == 5);
				case "αCMYK":	return (args.Length == 6);
			}
			
			return false;
		}
		
		public static RichColor Parse(string value)
		{
			if ((value == null) ||
				(value.Length == 0))
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
						return RichColor.FromAlphaRgb (1.0, Color.ColorComponentToDouble (args[1]), Color.ColorComponentToDouble (args[2]), Color.ColorComponentToDouble (args[3]));
					}
					break;
				
				case "αRGB":
					if (args.Length == 5)
					{
						return RichColor.FromAlphaRgb (Color.ColorComponentToDouble (args[1]), Color.ColorComponentToDouble (args[2]), Color.ColorComponentToDouble (args[3]), Color.ColorComponentToDouble (args[4]));
					}
					break;
				
				case "CMYK":
					if (args.Length == 5)
					{
						return RichColor.FromAlphaCmyk (1.0, Color.ColorComponentToDouble (args[1]), Color.ColorComponentToDouble (args[2]), Color.ColorComponentToDouble (args[3]), Color.ColorComponentToDouble (args[4]));
					}
					break;
				
				case "αCMYK":
					if (args.Length == 6)
					{
						return RichColor.FromAlphaCmyk (Color.ColorComponentToDouble (args[1]), Color.ColorComponentToDouble (args[2]), Color.ColorComponentToDouble (args[3]), Color.ColorComponentToDouble (args[4]), Color.ColorComponentToDouble (args[5]));
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
				
				case ColorSpace.Rgb:
					if (color.IsOpaque)
					{
						return string.Format (System.Globalization.CultureInfo.InvariantCulture, "RGB;{0};{1};{2}", Color.DoubleToColorComponent (color.R), Color.DoubleToColorComponent (color.G), Color.DoubleToColorComponent (color.B));
					}
					else
					{
						return string.Format (System.Globalization.CultureInfo.InvariantCulture, "αRGB;{0};{1};{2};{3}", Color.DoubleToColorComponent (color.A), Color.DoubleToColorComponent (color.R), Color.DoubleToColorComponent (color.G), Color.DoubleToColorComponent (color.B));
					}
				
				case ColorSpace.Cmyk:
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
			if (!a.isValid && !b.isValid)
			{
				return true;
			}
			if (!a.isValid || !b.isValid)
			{
				return false;
			}

			if (a.colorSpace != b.colorSpace)
			{
				return false;
			}

			switch (a.colorSpace)
			{
				case ColorSpace.Rgb:
					return Math.Equal (a.alpha,  b.alpha,  Color.ColorComponentDelta)
						&& Math.Equal (a.value1, b.value1, Color.ColorComponentDelta)
						&& Math.Equal (a.value2, b.value2, Color.ColorComponentDelta)
						&& Math.Equal (a.value3, b.value3, Color.ColorComponentDelta);

				case ColorSpace.Cmyk:
					return Math.Equal (a.alpha,  b.alpha,  Color.ColorComponentDelta)
						&& Math.Equal (a.value1, b.value1, Color.ColorComponentDelta)
						&& Math.Equal (a.value2, b.value2, Color.ColorComponentDelta)
						&& Math.Equal (a.value3, b.value3, Color.ColorComponentDelta)
						&& Math.Equal (a.value4, b.value4, Color.ColorComponentDelta);

				case ColorSpace.Gray:
					return Math.Equal (a.alpha,  b.alpha,  Color.ColorComponentDelta)
						&& Math.Equal (a.value1, b.value1, Color.ColorComponentDelta);
			}
			return true;
		}
		
		public static bool operator != (RichColor a, RichColor b)
		{
			return ! (a == b);
		}
		

		public override string ToString()
		{
			switch (this.colorSpace)
			{
				case ColorSpace.Rgb:
					return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "{{R={0:0.00},G={1:0.00},B={2:0.00},A={3:0.00}}}", this.value1, this.value2, this.value3, this.alpha);

				case ColorSpace.Cmyk:
					return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "{{C={0:0.00},M={1:0.00},Y={2:0.00},K={3:0.00},A={4:0.00}}}", this.value1, this.value2, this.value3, this.value4, this.alpha);

				case ColorSpace.Gray:
					return System.String.Format (System.Globalization.CultureInfo.InvariantCulture, "{{G={0:0.00},A={1:0.00}}}", this.value1, this.alpha);
			}

			return "?";
		}
		
		
		public override bool Equals(object obj)
		{
			return (obj is RichColor) && (this == (RichColor) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		

		#region Converter Class
		public class Converter : Types.AbstractStringConverter
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
			this.colorSpace = (ColorSpace) info.GetValue("ColorSpace", typeof(ColorSpace));
			this.alpha  = info.GetDouble("Alpha");
			this.value1 = info.GetDouble("V1");
			this.value2 = info.GetDouble("V2");
			this.value3 = info.GetDouble("V3");
			this.value4 = info.GetDouble("V4");
			this.isValid = !info.GetBoolean ("IsEmpty");
			this.name = info.GetString("Name");
		}
		
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("ColorSpace", this.colorSpace, typeof(ColorSpace));
			info.AddValue("Alpha", this.alpha);
			info.AddValue("V1", this.value1);
			info.AddValue("V2", this.value2);
			info.AddValue("V3", this.value3);
			info.AddValue("V4", this.value4);
			info.AddValue ("IsEmpty", !this.isValid);
			info.AddValue("Name", this.name);
		}
		#endregion

		private double						alpha;
		private double						value1;  // red or cyan or gray
		private double						value2;  // green or magenta
		private double						value3;  // blue or yellow
		private double						value4;  // black
		private string						name;
		private ColorSpace					colorSpace;
		private bool						isValid;
	}
}
