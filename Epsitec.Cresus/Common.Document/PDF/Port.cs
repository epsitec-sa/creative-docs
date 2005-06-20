using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe Port permet d'exporter en PDF des éléments graphiques simples.
	/// </summary>
	public class Port : IPaintPort
	{
		public Port(System.Collections.ArrayList patternList)
		{
			this.patternList = patternList;
			this.Init();
		}
		
		
		// Spécifie un trait continu.
		public double LineWidth
		{
			get { return this.lineWidth; }

			set
			{
				this.lineWidth = value;
				this.lineDash = false;
			}
		}

		// Spécifie un traitillé.
		public void SetLineDash(double width, double pen1, double gap1, double pen2, double gap2)
		{
			this.lineWidth = width;
			this.lineDash = true;
			this.lineDashPen1 = pen1;
			this.lineDashGap1 = gap1;
			this.lineDashPen2 = pen2;
			this.lineDashGap2 = gap2;
		}
		
		public bool LineDash
		{
			get { return this.lineDash; }
		}
		
		public double LineDashPen1
		{
			get { return this.lineDashPen1; }
		}
		
		public double LineDashGap1
		{
			get { return this.lineDashGap1; }
		}
		
		public double LineDashPen2
		{
			get { return this.lineDashPen2; }
		}
		
		public double LineDashGap2
		{
			get { return this.lineDashGap2; }
		}
		
		public JoinStyle LineJoin
		{
			get { return this.lineJoin; }
			set { this.lineJoin = value; }
		}
		
		public CapStyle LineCap
		{
			get { return this.lineCap; }
			set { this.lineCap = value; }
		}
		
		public double LineMiterLimit
		{
			get { return this.lineMiterLimit; }
			set { this.lineMiterLimit = value; }
		}
		
		public Color Color
		{
			get { return this.color; }

			set
			{
				this.color = value;
				this.pattern = -1;
			}
		}
		
		public void SetColoredPattern(Color color, int pattern)
		{
			this.color = color;
			this.pattern = pattern;
		}
		
		public FillMode FillMode
		{
			get { return this.fillMode; }
			set { this.fillMode = value; }
		}
		
		public Transform Transform
		{
			get
			{
			   return new Transform(this.transform);
			}

			set
			{
				if ( this.transform != value )
				{
					this.transform.Reset(value);
				}
			}
		}

		#region Clipping
		public void SetClippingRectangle(Point p, Size s)
		{
		}
		
		public void SetClippingRectangle(Rectangle rect)
		{
		}

		public void SetClippingRectangle(double x, double y, double width, double height)
		{
		}
		
		public Rectangle SaveClippingRectangle()
		{
			return Rectangle.Empty;
		}
		
		public void RestoreClippingRectangle(Rectangle clip)
		{
		}
		
		public void ResetClippingRectangle()
		{
		}
		
		public bool TestForEmptyClippingRectangle()
		{
			return true;
		}
		#endregion
		
		
		public void Align(ref double x, ref double y)
		{
		}
		
		
		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			this.transform.MultiplyByPostfix(Drawing.Transform.FromScale(sx, sy, cx, cy));
		}
		
		public void RotateTransformDeg(double angle, double cx, double cy)
		{
			this.transform.MultiplyByPostfix(Drawing.Transform.FromRotationDeg(angle, cx, cy));
		}
		
		public void RotateTransformRad(double angle, double cx, double cy)
		{
			this.transform.MultiplyByPostfix(Drawing.Transform.FromRotationRad(angle, cx, cy));
		}
		
		public void TranslateTransform(double ox, double oy)
		{
			this.transform.MultiplyByPostfix(Drawing.Transform.FromTranslation(ox, oy));
		}
		
		public void MergeTransform(Transform transform)
		{
			this.transform.MultiplyByPostfix(transform);
		}
		
		
		public void PaintOutline(Path path)
		{
			this.SetTransform(this.transform);
			this.SetWidth(this.lineWidth);
			this.SetCap(this.lineCap);
			this.SetJoin(this.lineJoin);
			this.SetDash(this.lineDash, this.lineDashPen1, this.lineDashGap1, this.lineDashPen2, this.lineDashGap2);
			this.SetLimit(this.lineMiterLimit);
			this.SetStrokeColor(this.color);
			this.PutPath(path);
			this.PutCommand("S ");  // stroke
			this.PutEOL();
		}
		
		public void PaintSurface(Path path)
		{
			this.SetTransform(this.transform);
			this.SetFillColor(this.color);
			this.PutPath(path);
			this.PutCommand("f ");  // fill
			this.PutEOL();
		}
		
		
		public double PaintText(double x, double y, string text, Font font, double size)
		{
			int n = text.Length;
			if ( n == 0 )  return 0.0;

			Drawing.Path path = new Drawing.Path();
			double width = 0.0;
			double ox = 0.0;
			double[] glyphX;
			int[]    glyph;
			byte[]   glyphN;

			font.GetGlyphsEndX(text, out glyphX, out glyph, out glyphN);
			System.Diagnostics.Debug.Assert(glyphX.Length == n);
			System.Diagnostics.Debug.Assert(glyph.Length == n);
			System.Diagnostics.Debug.Assert(glyphN.Length == n);

			Drawing.Transform ft = font.SyntheticTransform;
			ft.Scale(size);

			for ( int i=0 ; i<n ; i++ )
			{
				path.Append(font, glyph[i], ft.XX, ft.XY, ft.YX, ft.YY, ft.TX+x, ft.TY+y);
					
				x += (glyphX[i]-ox) * size;
				ox = glyphX[i];
			}

			width = glyphX[n-1] * size;

			this.SetTransform(this.transform);
			this.SetFillColor(this.color);
			this.PutPath(path);
			this.PutCommand("f ");  // fill
			this.PutEOL();

			path.Dispose();
			return width;
		}
		
		public double PaintText(double x, double y, string text, Font font, double size, Font.ClassInfo[] infos)
		{
			for ( int i=0 ; i<infos.Length ; i++ )
			{
				if ( infos[i].Scale != 1.00 &&
					 infos[i].ClassId == Drawing.Font.ClassId.Space )
				{
					string[] texts = text.Split(new char[] { ' ', (char) 160 });
					double spaceW = font.GetCharAdvance(' ') * size * infos[i].Scale;
					double totalW = 0.0;
					
					for ( int j=0 ; j<texts.Length ; j++ )
					{
						double w = this.PaintText(x, y, texts[j], font, size) + spaceW;
						
						totalW += w;
						x      += w;
					}
					
					return totalW-spaceW;
				}
			}
			
			return this.PaintText(x, y, text, font, size);
		}
		
		
		#region PaintImage
		public void PaintImage(Image bitmap, Rectangle fill)
		{
			this.PaintImage(bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, 0, 0, bitmap.Width, bitmap.Height);
		}
		
		public void PaintImage(Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight)
		{
			this.PaintImage(bitmap, fillX, fillY, fillWidth, fillHeight, 0, 0, bitmap.Width, bitmap.Height);
		}
		
		public void PaintImage(Image bitmap, Rectangle fill, Point imageOrigin)
		{
			this.PaintImage(bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, imageOrigin.X, imageOrigin.Y, fill.Width, fill.Height);
		}
		
		public void PaintImage(Image bitmap, Rectangle fill, Rectangle imageRect)
		{
			this.PaintImage(bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, imageRect.Left, imageRect.Bottom, imageRect.Width, imageRect.Height);
		}
		
		public void PaintImage(Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight, double imageOriginX, double imageOriginY)
		{
			this.PaintImage(bitmap, fillX, fillY, fillWidth, fillHeight, imageOriginX, imageOriginY, fillWidth, fillHeight);
		}
		
		public void PaintImage(Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight, double imageOriginX, double imageOriginY, double imageWidth, double imageHeight)
		{
			System.Diagnostics.Debug.WriteLine("PDF.Port.PaintImage not supported");
		}
		#endregion
		
		
		// Spécifie les dimensions d'une page.
		public void SetPageSize(Size size)
		{
			this.PutCommand("[0 0 ");
			this.PutDistance(size.Width);
			this.PutDistance(size.Height);
			this.PutCommand("] endobj");
		}

		// Donne tout le texte PDF généré.
		public string GetPDF()
		{
			return this.stringBuilder.ToString();
		}

		// Initialise tous les paramètres graphiques à des valeurs différentes
		// des valeurs utilisées par la suite, ou aux valeurs par défaut.
		protected void Init()
		{
			this.stringBuilder = new System.Text.StringBuilder();
			this.currentWidth = -1.0;
			this.currentCap = (CapStyle) 999;
			this.currentJoin = (JoinStyle) 999;
			this.currentDash = false;
			this.currentPen1 = 0.0;
			this.currentGap1 = 0.0;
			this.currentPen2 = 0.0;
			this.currentGap2 = 0.0;
			this.currentLimit = -1.0;
			this.currentStrokeColor = Color.Empty;
			this.currentFillColor = Color.Empty;
			this.currentPattern = -1;
			this.currentTransform = new Transform();
		}

		// Spécifie la couleur de trait.
		protected void SetStrokeColor(Color color)
		{
			if ( this.currentStrokeColor != color || this.currentPattern != pattern )
			{
				this.currentStrokeColor = color;
				this.currentPattern = pattern;

				if ( pattern == -1 )
				{
					this.PutColor(color);
					this.PutCommand("RG ");
				}
				else
				{
					this.PutCommand("/Cs cs ");
					this.PutColor(color);
					this.PutCommand(Export.ShortNamePattern(pattern));
					this.PutCommand("SCN ");
				}
			}
		}

		// Spécifie la couleur de surface.
		protected void SetFillColor(Color color)
		{
			if ( this.currentFillColor != color || this.currentPattern != pattern )
			{
				this.currentFillColor = color;
				this.currentPattern = pattern;

				if ( pattern == -1 )
				{
					this.PutColor(color);
					this.PutCommand("rg ");
				}
				else
				{
					this.PutCommand("/Cs cs ");
					this.PutColor(color);
					this.PutCommand(Export.ShortNamePattern(pattern));
					this.PutCommand("scn ");
				}
			}
		}

		// Spécifie l'épaisseur des traits.
		protected void SetWidth(double width)
		{
			if ( this.currentWidth != width )
			{
				this.currentWidth = width;

				this.PutDistance(width);
				this.PutCommand("w ");
			}
		}

		// Spécifie l'extrémité des traits.
		protected void SetCap(CapStyle cap)
		{
			if ( this.currentCap != cap )
			{
				this.currentCap = cap;

				switch ( cap )
				{
					case CapStyle.Butt:    this.PutCommand("0 J ");  break;
					case CapStyle.Round:   this.PutCommand("1 J ");  break;
					case CapStyle.Square:  this.PutCommand("2 J ");  break;
				}
			}
		}

		// Spécifie les coins des traits.
		protected void SetJoin(JoinStyle join)
		{
			if ( this.currentJoin != join )
			{
				this.currentJoin = join;

				switch ( join )
				{
					case JoinStyle.MiterRevert:
					case JoinStyle.Miter:  this.PutCommand("0 j ");  break;
					case JoinStyle.Round:  this.PutCommand("1 j ");  break;
					case JoinStyle.Bevel:  this.PutCommand("2 j ");  break;
				}
			}
		}

		// Spécifie le mode de traitillé.
		protected void SetDash(bool dash, double pen1, double gap1, double pen2, double gap2)
		{
			if ( this.currentDash != dash ||
				 this.currentPen1 != pen1 ||
				 this.currentGap1 != gap1 ||
				 this.currentPen2 != pen2 ||
				 this.currentGap2 != gap2 )
			{
				this.currentDash = dash;
				this.currentPen1 = pen1;
				this.currentGap1 = gap1;
				this.currentPen2 = pen2;
				this.currentGap2 = gap2;

				if ( dash )
				{
					this.PutCommand("[");
					this.PutDistance(pen1);
					this.PutDistance(gap1);
					if ( pen2 != 0.0 || gap2 != 0.0 )
					{
						this.PutDistance(pen2);
						this.PutDistance(gap2);
					}
					this.PutCommand("] 0 d ");
				}
				else
				{
					this.PutCommand("[] 0 d ");
				}
			}
		}

		// Spécifie la limite pour JoinMiter.
		protected void SetLimit(double limit)
		{
			if ( this.currentLimit != limit )
			{
				this.currentLimit = limit;

				this.PutValue(limit, 1);
				this.PutCommand("M ");
			}
		}

		// Spécifie la matrice de transformation.
		protected void SetTransform(Transform transform)
		{
			if ( this.currentTransform != transform )
			{
				Transform t = Transform.Inverse(this.currentTransform);
				t.MultiplyByPostfix(transform);

				// Attention: l'angle est inversé en PDF !
				this.PutValue(t.XX, 4);
				this.PutValue(-t.XY, 4);
				this.PutValue(-t.YX, 4);
				this.PutValue(t.YY, 4);
				this.PutDistance(t.TX);
				this.PutDistance(t.TY);
				this.PutCommand("cm ");

				this.currentTransform.Reset(transform);
			}
		}

		// Met un chemin quelconque.
		protected void PutPath(Path path)
		{
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);

			Point start = new Point(0, 0);
			Point current = new Point(0, 0);
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			Point p3 = new Point(0, 0);
			int i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & PathElement.MaskCommand )
				{
					case PathElement.MoveTo:
						current = points[i++];
						start = current;
						this.PutPoint(current);
						this.PutCommand("m ");  // moveto
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						current = p1;
						this.PutPoint(current);
						this.PutCommand("l ");  // lineto
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						p1 = Point.Scale(current, p1, 2.0/3.0);
						p2 = Point.Scale(p3,      p2, 2.0/3.0);
						current = p3;
						this.PutPoint(p1);
						this.PutPoint(p2);
						this.PutPoint(p3);
						this.PutCommand("c ");  // curveto
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						current = p3;
						this.PutPoint(p1);
						this.PutPoint(p2);
						this.PutPoint(p3);
						this.PutCommand("c ");  // curveto
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							this.PutCommand("h ");  // close
						}
						i ++;
						break;
				}
			}
		}

		// Met une couleur (sans alpha).
		protected void PutColor(Color color)
		{
			this.PutValue(color.R, 3);
			this.PutValue(color.G, 3);
			this.PutValue(color.B, 3);
		}

		// Met un point.
		protected void PutPoint(Point pos)
		{
			this.PutDistance(pos.X);
			this.PutDistance(pos.Y);
		}

		// Met une distance.
		protected void PutDistance(double num)
		{
			this.stringBuilder.Append(Port.StringDistance(num));
			this.stringBuilder.Append(" ");
		}

		// Met une valeur.
		protected void PutValue(double num, int decimals)
		{
			this.stringBuilder.Append(Port.StringValue(num, decimals));
			this.stringBuilder.Append(" ");
		}

		// Met un entier.
		protected void PutInt(int num)
		{
			this.stringBuilder.Append(num.ToString(System.Globalization.CultureInfo.InvariantCulture));
			this.stringBuilder.Append(" ");
		}

		// Met une fin de ligne.
		protected void PutEOL()
		{
			int len = this.stringBuilder.Length;
			if ( len == 0 )  return;

			if ( this.stringBuilder[len-1] == ' ' )
			{
				this.stringBuilder.Remove(len-1, 1);  // enlève l'espace à la fin
			}

			this.stringBuilder.Append("\r\n");
		}

		// Met une commande quelconque.
		protected void PutCommand(string cmd)
		{
			this.stringBuilder.Append(cmd);
		}


		// Met une distance.
		public static string StringDistance(double num)
		{
			return Port.StringValue(num*7.2/25.4, 2);  // dixièmes de millimètres -> 72ème de pouce
		}

		// Met une valeur.
		public static string StringValue(double num, int decimals)
		{
			if ( decimals == 1 )
			{
				return string.Format("{0:0.#}", num);
			}
			else if ( decimals == 2 )
			{
				return string.Format("{0:0.##}", num);
			}
			else if ( decimals == 3 )
			{
				return string.Format("{0:0.###}", num);
			}
			else
			{
				return string.Format("{0:0.####}", num);
			}
		}


		// Cherche le pattern à utiliser pour un objet et une propriété.
		public int SearchPattern(Objects.Abstract obj, Properties.Abstract property)
		{
			if ( this.patternList == null )  return -1;

			foreach ( Pattern pattern in this.patternList )
			{
				if ( pattern.Object == obj && pattern.Property == property )
				{
					return pattern.Id;
				}
			}
			return 1;
		}


		protected System.Collections.ArrayList	patternList;

		protected double						lineWidth = 1.0;
		protected JoinStyle						lineJoin = JoinStyle.MiterRevert;
		protected CapStyle						lineCap = CapStyle.Square;
		protected bool							lineDash = false;
		protected double						lineDashPen1 = 0.0;
		protected double						lineDashGap1 = 0.0;
		protected double						lineDashPen2 = 0.0;
		protected double						lineDashGap2 = 0.0;
		protected double						lineMiterLimit = 5.0;
		protected Color							color = Color.FromRGB(0, 0, 0);
		protected int							pattern = -1;
		protected Transform						transform = new Transform();
		protected FillMode						fillMode = FillMode.NonZero;

		protected System.Text.StringBuilder		stringBuilder;
		protected Color							currentStrokeColor;
		protected Color							currentFillColor;
		protected int							currentPattern;
		protected double						currentWidth;
		protected CapStyle						currentCap;
		protected JoinStyle						currentJoin;
		protected bool							currentDash;
		protected double						currentPen1;
		protected double						currentGap1;
		protected double						currentPen2;
		protected double						currentGap2;
		protected double						currentLimit;
		protected Transform						currentTransform;
	}
}
