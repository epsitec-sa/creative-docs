using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	public enum ColorForce
	{
		Default,		// color space selon RichColor.ColorSpace
		Nothing,		// aucune commande de couleur
		RGB,			// force le color space RGB
		CMYK,			// force le color space CMYK
		Gray,			// force le color space Gray
	}

	/// <summary>
	/// La classe Port permet d'exporter en PDF des �l�ments graphiques simples.
	/// [*] = documentation PDF Reference, version 1.6, fifth edition, 1236 pages
	/// </summary>
	public class Port : IPaintPort
	{
		public Port()
		{
			this.complexSurfaceList = null;
			this.imageSurfaceList   = null;
			this.fontList           = null;

			this.stackColorModifier = new System.Collections.Stack();
			this.Reset();
		}

		public Port(System.Collections.ArrayList complexSurfaceList,
					System.Collections.ArrayList imageSurfaceList,
					System.Collections.Hashtable fontList)
		{
			this.complexSurfaceList = complexSurfaceList;
			this.imageSurfaceList   = imageSurfaceList;
			this.fontList           = fontList;

			this.stackColorModifier = new System.Collections.Stack();
			this.Reset();
		}

		// R�initialise le port, mais surtout pas le stack des modificateurs
		// de couleurs.
		public void Reset()
		{
			this.colorForce = ColorForce.Default;
			this.defaultDecimals = 2;
			this.LineWidth = 1.0;
			this.lineJoin = JoinStyle.MiterRevert;
			this.lineCap = CapStyle.Square;
			this.lineDash = false;
			this.lineDashPen1 = 0.0;
			this.lineDashGap1 = 0.0;
			this.lineDashPen2 = 0.0;
			this.lineDashGap2 = 0.0;
			this.lineDashPen3 = 0.0;
			this.lineDashGap3 = 0.0;
			this.lineMiterLimit = 5.0;
			this.originalColor = RichColor.FromBrightness(0.0);
			this.color = RichColor.FromBrightness(0.0);
			this.complexSurfaceId = -1;
			this.complexType = TypeComplexSurface.ExtGState;
			this.transform = new Transform();
			this.fillMode = FillMode.NonZero;

			this.Init();
		}
		
		
		// Force un espace de couleur.
		public ColorForce ColorForce
		{
			get { return this.colorForce; }
			set { this.colorForce = value; }
		}

		// Indique le nombre de d�cimales par d�faut.
		public int DefaultDecimals
		{
			get { return this.defaultDecimals; }
			set { this.defaultDecimals = value; }
		}

		// Sp�cifie un trait continu.
		public double LineWidth
		{
			get { return this.lineWidth; }

			set
			{
				this.lineWidth = value;
				this.lineDash = false;
			}
		}

		// Sp�cifie un traitill�.
		public void SetLineDash(double width, double pen1, double gap1, double pen2, double gap2, double pen3, double gap3)
		{
			this.lineWidth = width;
			this.lineDash = true;
			this.lineDashPen1 = pen1;
			this.lineDashGap1 = gap1;
			this.lineDashPen2 = pen2;
			this.lineDashGap2 = gap2;
			this.lineDashPen3 = pen3;
			this.lineDashGap3 = gap3;
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
		
		public double LineDashPen3
		{
			get { return this.lineDashPen3; }
		}
		
		public double LineDashGap3
		{
			get { return this.lineDashGap3; }
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
		
		public RichColor RichColor
		{
			get { return this.originalColor; }

			set
			{
				this.originalColor = value;
				this.FinalRichColor = this.GetFinalColor(value);
			}
		}
		
		public Color Color
		{
			get { return this.originalColor.Basic; }

			set
			{
				this.originalColor.Basic = value;
				this.FinalColor = this.GetFinalColor(value);
			}
		}
		
		public RichColor FinalRichColor
		{
			get { return this.color; }

			set
			{
				this.color = value;
				this.complexSurfaceId = -1;
				this.complexType = TypeComplexSurface.ExtGState;
			}
		}
		
		public Color FinalColor
		{
			get { return this.color.Basic; }

			set
			{
				this.color.Basic = value;
				this.complexSurfaceId = -1;
				this.complexType = TypeComplexSurface.ExtGState;
			}
		}
		
		public void PushColorModifier(ColorModifier method)
		{
			this.stackColorModifier.Push(method);
		}

		public ColorModifier PopColorModifier()
		{
			return this.stackColorModifier.Pop() as ColorModifier;
		}

		public System.Collections.Stack StackColorModifier
		{
			get
			{
				return this.stackColorModifier;
			}
			set
			{
				this.stackColorModifier = value;
			}
		}

		public RichColor GetFinalColor(RichColor color)
		{
			foreach ( ColorModifier method in this.stackColorModifier )
			{
				method(ref color);
			}
			return color;
		}
		
		public Color GetFinalColor(Color color)
		{
			if ( this.stackColorModifier.Count == 0 )  return color;

			RichColor rich = RichColor.FromColor(color);
			foreach ( ColorModifier method in this.stackColorModifier )
			{
				method(ref rich);
			}
			return rich.Basic;
		}
		
		public void SetColoredComplexSurface(RichColor color, int id)
		{
			this.FinalRichColor = color;
			this.complexSurfaceId = id;
			this.complexType = TypeComplexSurface.ExtGState;
		}

		public void SetColoredComplexSurface(RichColor color, int id, TypeComplexSurface type)
		{
			this.FinalRichColor = color;
			this.complexSurfaceId = id;
			this.complexType = type;
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
				this.transform.Reset(value);
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
			this.SetDash(this.lineDash, this.lineDashPen1, this.lineDashGap1, this.lineDashPen2, this.lineDashGap2, this.lineDashPen3, this.lineDashGap3);
			this.SetLimit(this.lineMiterLimit);
			this.SetStrokeColor(this.color);
			this.PutPath(path);
			this.PutCommand("S ");  // stroke, voir [*] page 200
			this.PutEOL();
		}
		
		public void PaintSurface(Path path)
		{
			this.SetTransform(this.transform);
			this.SetFillColor(this.color);
			this.DoFill(path);
			this.PutEOL();
		}
		
		
		public void PaintGlyphs(Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
		{
			int n = glyphs.Length;
			if ( n == 0 )  return;
			
			//	TODO: g�n�rer ce qu'il faut pour mettre les glyphes dans le fichier PDF. Mais
			//	attention, il faudrait aussi g�n�rer l'information textuelle �quivalente pour
			//	permettre � Acrobat Reader d'utiliser les fonctions de recherche, par exemple.
			
			Drawing.Path path = new Drawing.Path();
			Drawing.Transform ft = font.SyntheticTransform;
			
			ft.Scale(size);
			
			for ( int i=0 ; i<n ; i++ )
			{
				if ( glyphs[i] == 65535 )  continue;  // TODO: bof !!!
				double scaleX = sx == null ? 1.0 : sx[i];
				double scaleY = sy == null ? 1.0 : sy[i];
				path.Append(font, glyphs[i], ft.XX*scaleX, ft.XY*scaleX, ft.YX*scaleY, ft.YY*scaleY, ft.TX*scaleX+x[i], ft.TY*scaleY+y[i]);
			}
			
			this.SetTransform(this.transform);
			this.SetFillColor(this.color);
			this.DoFill(path);
			this.PutEOL();
			
			path.Dispose();
		}
		
		public double PaintText(double x, double y, string text, Font font, double size)
		{
			if ( this.fontList == null )  // textes en courbes ?
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
				this.DoFill(path);
				this.PutEOL();

				path.Dispose();
				return width;
			}
			else	// textes en fontes ?
			{
				System.Diagnostics.Debug.Assert(this.fontList != null);
				FontList fl = FontList.Search(this.fontList, font);
				System.Diagnostics.Debug.Assert(fl != null);

				int n = text.Length;
				if ( n == 0 )  return 0.0;

				double[] glyphX;
				int[]    glyph;
				byte[]   glyphN;

				font.GetGlyphsEndX(text, out glyphX, out glyph, out glyphN);
				System.Diagnostics.Debug.Assert(glyphX.Length == n);
				System.Diagnostics.Debug.Assert(glyph.Length == n);
				System.Diagnostics.Debug.Assert(glyphN.Length == n);

				this.SetTransform(this.transform);
				this.SetFillColor(this.color);

				this.PutCommand("BT ");  // voir [*] page 375

				int lastFontPage = -1;
				double ox = 0.0;
				double lastX = 0.0;
				double lastY = 0.0;
				for ( int i=0 ; i<n ; i++ )
				{
					int unicode = (int) text[i];
					int code = fl.GetUnicodeIndex(unicode);
					System.Diagnostics.Debug.Assert(code != -1);
					int fontPage = code/Export.charPerFont;
					
					if ( fontPage != lastFontPage )
					{
						if ( lastFontPage != -1 )
						{
							this.PutCommand("> Tj ");  // voir [*] page 377
						}
						this.PutCommand(Export.ShortNameFont(fl.Id, fontPage));
						this.PutValue(size);
						this.PutCommand("Tf ");
						this.PutPoint(new Point(x-lastX, y-lastY));
						this.PutCommand("Td <");  // voir [*] page 376

						lastX = x;
						lastY = y;
						lastFontPage = fontPage;
					}

					this.PutCommand((code%Export.charPerFont).ToString("X2"));

					x += (glyphX[i]-ox) * size;
					ox = glyphX[i];
				}

				this.PutCommand("> Tj ET ");
				this.PutEOL();

				return glyphX[n-1] * size;
			}
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
		// Cette classe impl�mente une version restreinte de PaintImage.
		// La position et les dimensions de l'image doivent �tre d�finies par
		// une transformation, et le rectangle doit �tre �gal � (0,0,1,1).
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
			System.Diagnostics.Debug.Assert(fillX == 0.0);
			System.Diagnostics.Debug.Assert(fillY == 0.0);
			System.Diagnostics.Debug.Assert(System.Math.Abs(fillWidth-1.0) < 0.000001);
			System.Diagnostics.Debug.Assert(System.Math.Abs(fillHeight-1.0) < 0.000001);

			if ( this.imageSurfaceList == null )  return;

			ImageSurface image = ImageSurface.Search(this.imageSurfaceList, bitmap);
			if ( image == null )  return;

			this.SetTransform(this.transform);
			this.PutCommand(Export.ShortNameComplexSurface(image.Id, TypeComplexSurface.XObject));
			this.PutCommand("Do ");  // external object, voir [*] page 302
			this.PutEOL();
		}
		#endregion
		
		
		// Donne tout le texte PDF g�n�r�.
		public string GetPDF()
		{
			string s = this.stringBuilder.ToString();
			this.Reset();
			return s;
		}

		// Initialise tous les param�tres graphiques � des valeurs diff�rentes
		// des valeurs utilis�es par la suite, ou aux valeurs par d�faut.
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
			this.currentStrokeColor = RichColor.Empty;
			this.currentFillColor = RichColor.Empty;
			this.currentComplexSurfaceId = -1;
			this.currentTransform = new Transform();
		}

		// Sp�cifie la couleur de trait.
		protected void SetStrokeColor(RichColor color)
		{
			if ( this.currentStrokeColor != color || this.currentComplexSurfaceId != this.complexSurfaceId )
			{
				if ( this.currentComplexSurfaceId != -1 )
				{
					this.PutCommand(Export.ShortNameComplexSurface(0, TypeComplexSurface.ExtGState));
					this.PutCommand("gs ");  // graphic state, voir [*] page 189
				}

				if ( this.currentStrokeColor.ColorSpace != color.ColorSpace )
				{
					this.PutStrokingColorSpace(color);
				}

				this.currentStrokeColor = color;
				this.currentComplexSurfaceId = this.complexSurfaceId;

				this.PutStrokingColor(color);

				if ( this.complexSurfaceId != -1 )
				{
					ComplexSurface cs = this.GetComplexSurface(this.complexSurfaceId);

					if ( cs.Type == Type.TransparencyRegular  ||
						 cs.Type == Type.TransparencyGradient )
					{
						this.PutCommand(Export.ShortNameComplexSurface(this.complexSurfaceId, this.complexType));
						this.PutCommand("gs ");  // graphic state, voir [*] page 189
					}
				}
			}
		}

		// Sp�cifie la couleur de surface.
		protected void SetFillColor(RichColor color)
		{
			this.SearchComplexSurfaceByColor();

			if ( this.currentFillColor != color || this.currentComplexSurfaceId != this.complexSurfaceId )
			{
				if ( this.currentComplexSurfaceId != -1 )
				{
					this.PutCommand(Export.ShortNameComplexSurface(0, TypeComplexSurface.ExtGState));
					this.PutCommand("gs ");  // graphic state, voir [*] page 189
				}

				if ( this.currentFillColor.ColorSpace != color.ColorSpace )
				{
					this.PutFillingColorSpace(color);
				}

				this.currentFillColor = color;
				this.currentComplexSurfaceId = this.complexSurfaceId;

				this.PutFillingColor(color);

				if ( this.complexSurfaceId != -1 )
				{
					ComplexSurface cs = this.GetComplexSurface(this.complexSurfaceId);

					if ( cs.IsSmooth )
					{
						this.PutCommand("q ");
						this.PutCommand(Export.ShortNameComplexSurface(this.complexSurfaceId, this.complexType));
						this.PutCommand("gs ");  // graphic state, voir [*] page 189
					}
					else if ( cs.Type == Type.TransparencyRegular )
					{
						this.PutCommand(Export.ShortNameComplexSurface(this.complexSurfaceId, this.complexType));
						this.PutCommand("gs ");  // graphic state, voir [*] page 189
					}
					else if ( cs.Type == Type.TransparencyPattern &&
							  this.complexType != TypeComplexSurface.ExtGState )
					{
						this.PutCommand(Export.ShortNameComplexSurface(this.complexSurfaceId, this.complexType));
						this.PutCommand("gs ");  // graphic state, voir [*] page 189
					}
					else if ( cs.Type == Type.TransparencyGradient )
					{
						this.PutCommand("q ");
						this.PutCommand(Export.ShortNameComplexSurface(this.complexSurfaceId, this.complexType));
						this.PutCommand("gs ");  // graphic state, voir [*] page 189
					}
				}
			}
		}

		// Rempli la surface du chemin d�fini.
		protected void DoFill(Path path)
		{
			this.SearchComplexSurfaceByColor();

			if ( this.complexSurfaceId == -1 || this.complexType != TypeComplexSurface.ExtGState )
			{
				this.PutPath(path);
				this.PutCommandFill();
			}
			else
			{
				ComplexSurface cs = this.GetComplexSurface(this.complexSurfaceId);

				if ( cs.IsSmooth )
				{
					Rectangle bbox = Geometry.ComputeBoundingBox(path);
					Export.SurfaceInflate(cs, ref bbox);
					Path pathBBox = new Path();
					pathBBox.AppendRectangle(bbox);
					this.PutPath(pathBBox);

					if ( cs.Type == Type.OpaqueGradient       ||
						 cs.Type == Type.TransparencyGradient )
					{
						this.PutCommandStroke();
						this.PutCommand("n ");
						this.PutTransform(cs.Matrix);  // current clipping, voir [*] page 205
						this.PutCommand(Export.ShortNameComplexSurface(this.complexSurfaceId, TypeComplexSurface.ShadingColor));
						this.PutCommand("sh Q ");  // shading, voir [*] page 273
					}
					else
					{
						this.PutCommandFill();
						this.PutCommand("Q ");
					}
				}
				else if ( cs.Type == Type.OpaqueGradient )
				{
					this.PutPath(path);
					this.PutCommand("q ");  // save, voir [*] page 189
					this.PutCommandStroke();
					this.PutCommand("n ");
					this.PutTransform(cs.Matrix);  // current clipping, voir [*] page 205
					this.PutCommand(Export.ShortNameComplexSurface(this.complexSurfaceId, TypeComplexSurface.ShadingColor));
					this.PutCommand("sh Q ");  // shading, voir [*] page 273
				}
				else if ( cs.Type == Type.TransparencyGradient )
				{
					this.PutPath(path);
					this.PutCommandStroke();
					this.PutCommand("n ");
					this.PutTransform(cs.Matrix);  // current clipping, voir [*] page 205
					this.PutCommand(Export.ShortNameComplexSurface(this.complexSurfaceId, TypeComplexSurface.ShadingColor));
					this.PutCommand("sh Q ");  // shading, voir [*] page 273
				}
				else if ( cs.Type == Type.OpaquePattern       ||
						  cs.Type == Type.TransparencyPattern )
				{
					this.PutPath(path);
					this.PutCommand("q ");  // save, voir [*] page 189
					this.PutCommandStroke();
					this.PutCommand("n ");
					this.PutCommand(Export.ShortNameComplexSurface(this.complexSurfaceId, TypeComplexSurface.XObject));
					this.PutCommand("Do Q ");  // external object, voir [*] page 302
				}
				else
				{
					this.PutPath(path);
					this.PutCommandFill();
				}
			}
		}

		// S'il n'existe pas d'identificateur de surface complexe connu et que
		// la couleur est transparente, cherche un identificateur d'apr�s la couleur.
		// C'est le cas des textes transparents, qui d�finissent les couleurs avec
		// port.Color au lieu de port.SetColoredComplexSurface !
		protected void SearchComplexSurfaceByColor()
		{
			if ( this.complexSurfaceId != -1 )  return;
			if ( this.color.A == 1.0 )  return;

			int id = this.SearchComplexColor(this.color);
			if ( id == -1 )  return;
			this.complexSurfaceId = id;
		}

		// Sp�cifie l'�paisseur des traits.
		protected void SetWidth(double width)
		{
			if ( this.currentWidth != width )
			{
				this.currentWidth = width;

				this.PutValue(width);
				this.PutCommand("w ");  // line width, voir [*] page 189
			}
		}

		// Sp�cifie l'extr�mit� des traits.
		protected void SetCap(CapStyle cap)
		{
			if ( this.currentCap != cap )
			{
				this.currentCap = cap;

				switch ( cap )
				{
					// Line cap style, voir [*] page 186 et 189
					case CapStyle.Butt:    this.PutCommand("0 J ");  break;
					case CapStyle.Round:   this.PutCommand("1 J ");  break;
					case CapStyle.Square:  this.PutCommand("2 J ");  break;
				}
			}
		}

		// Sp�cifie les coins des traits.
		protected void SetJoin(JoinStyle join)
		{
			if ( this.currentJoin != join )
			{
				this.currentJoin = join;

				switch ( join )
				{
					// Line join style, voir [*] page 186 et 189
					case JoinStyle.MiterRevert:
					case JoinStyle.Miter:  this.PutCommand("0 j ");  break;
					case JoinStyle.Round:  this.PutCommand("1 j ");  break;
					case JoinStyle.Bevel:  this.PutCommand("2 j ");  break;
				}
			}
		}

		// Sp�cifie le mode de traitill�.
		protected void SetDash(bool dash, double pen1, double gap1, double pen2, double gap2, double pen3, double gap3)
		{
			if ( this.currentDash != dash ||
				 this.currentPen1 != pen1 ||
				 this.currentGap1 != gap1 ||
				 this.currentPen2 != pen2 ||
				 this.currentGap2 != gap2 ||
				 this.currentPen3 != pen3 ||
				 this.currentGap3 != gap3 )
			{
				this.currentDash = dash;
				this.currentPen1 = pen1;
				this.currentGap1 = gap1;
				this.currentPen2 = pen2;
				this.currentGap2 = gap2;
				this.currentPen3 = pen3;
				this.currentGap3 = gap3;

				if ( dash )
				{
					// Line dash pattern, voir [*] page 188 et 189
					this.PutCommand("[");
					this.PutValue(pen1);
					this.PutValue(gap1);
					if ( pen2 != 0.0 || gap2 != 0.0 )
					{
						this.PutValue(pen2);
						this.PutValue(gap2);
					}
					if ( pen3 != 0.0 || gap3 != 0.0 )
					{
						this.PutValue(pen3);
						this.PutValue(gap3);
					}
					this.PutCommand("] 0 d ");
				}
				else
				{
					this.PutCommand("[] 0 d ");
				}
			}
		}

		// Sp�cifie la limite pour JoinMiter.
		protected void SetLimit(double limit)
		{
			if ( this.currentLimit != limit )
			{
				this.currentLimit = limit;

				this.PutValue(limit, 1);
				this.PutCommand("M ");  // miter limit, voir [*] page 187 et 189
			}
		}

		// Sp�cifie la matrice de transformation.
		protected void SetTransform(Transform transform)
		{
			if ( this.currentTransform != transform )
			{
				Transform t = Transform.Inverse(this.currentTransform);
				t.MultiplyByPostfix(transform);

				// Attention: inversion de XY et YX !
				this.PutValue(t.XX, -1);
				this.PutValue(t.YX, -1);  // xy
				this.PutValue(t.XY, -1);  // yx
				this.PutValue(t.YY, -1);
				this.PutValue(t.TX, -1);
				this.PutValue(t.TY, -1);
				this.PutCommand("cm ");  // current transform, voir [*] page 189

				this.currentTransform.Reset(transform);
			}
		}

		// Met un chemin quelconque.
		public void PutPath(Path path)
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
						this.PutCommand("m ");  // moveto, voir [*] page 196
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						current = p1;
						this.PutPoint(current);
						this.PutCommand("l ");  // lineto, voir [*] page 196
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
						this.PutCommand("c ");  // curveto, voir [*] page 196
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						current = p3;
						this.PutPoint(p1);
						this.PutPoint(p2);
						this.PutPoint(p3);
						this.PutCommand("c ");  // curveto, voir [*] page 196
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							this.PutCommand("h ");  // close, voir [*] page 197
						}
						i ++;
						break;
				}
			}
		}

		// Met une matrice de transformation.
		public void PutTransform(Transform matrix)
		{
			if ( matrix.XX != 1.0 ||
				 matrix.XY != 0.0 ||
				 matrix.YX != 0.0 ||
				 matrix.YY != 1.0 ||
				 matrix.TX != 0.0 ||
				 matrix.TY != 0.0 )  // autre que matrice identit� ?
			{
				// Attention: inversion de XY et YX !
				this.PutValue(matrix.XX, -1);
				this.PutValue(matrix.YX, -1);  // xy
				this.PutValue(matrix.XY, -1);  // yx
				this.PutValue(matrix.YY, -1);
				this.PutValue(matrix.TX, -1);
				this.PutValue(matrix.TY, -1);
				this.PutCommand("cm ");  // current transform, voir [*] page 189
			}
		}

		// Met un espace de couleur de trait.
		// Stroking color, voir [*] page 257
		protected void PutStrokingColorSpace(RichColor color)
		{
			if ( this.colorForce == ColorForce.Nothing )  return;
			if ( this.colorForce != ColorForce.Default )  return;

			if ( color.ColorSpace == ColorSpace.RGB  )  this.PutCommand("/DeviceRGB CS ");
			if ( color.ColorSpace == ColorSpace.CMYK )  this.PutCommand("/DeviceCMYK CS ");
			if ( color.ColorSpace == ColorSpace.Gray )  this.PutCommand("/DeviceGray CS ");
		}

		// Met un espace de couleur de trait.
		// Stroking color, voir [*] page 257
		protected void PutFillingColorSpace(RichColor color)
		{
			if ( this.colorForce == ColorForce.Nothing )  return;
			if ( this.colorForce != ColorForce.Default )  return;

			if ( color.ColorSpace == ColorSpace.RGB  )  this.PutCommand("/DeviceRGB cs ");
			if ( color.ColorSpace == ColorSpace.CMYK )  this.PutCommand("/DeviceCMYK cs ");
			if ( color.ColorSpace == ColorSpace.Gray )  this.PutCommand("/DeviceGray cs ");
		}

		// Met une couleur de trait.
		// Stroking color, voir [*] page 258
		protected void PutStrokingColor(RichColor color)
		{
			if ( this.colorForce == ColorForce.Nothing )  return;

			this.PutColor(color);
			if ( this.colorForce == ColorForce.Default )  this.PutCommand("SC ");
			if ( this.colorForce == ColorForce.RGB     )  this.PutCommand("RG ");
			if ( this.colorForce == ColorForce.CMYK    )  this.PutCommand("K ");
			if ( this.colorForce == ColorForce.Gray    )  this.PutCommand("G ");
		}

		// Met une couleur de surface.
		// Fill color, voir [*] page 258
		protected void PutFillingColor(RichColor color)
		{
			if ( this.colorForce == ColorForce.Nothing )  return;

			this.PutColor(color);
			if ( this.colorForce == ColorForce.Default )  this.PutCommand("sc ");
			if ( this.colorForce == ColorForce.RGB     )  this.PutCommand("rg ");
			if ( this.colorForce == ColorForce.CMYK    )  this.PutCommand("k ");
			if ( this.colorForce == ColorForce.Gray    )  this.PutCommand("g ");
		}

		// Met une couleur (sans alpha).
		public void PutColor(RichColor color)
		{
			ColorSpace cs = color.ColorSpace;
			if ( this.colorForce == ColorForce.RGB  )  cs = ColorSpace.RGB;
			if ( this.colorForce == ColorForce.CMYK )  cs = ColorSpace.CMYK;
			if ( this.colorForce == ColorForce.Gray )  cs = ColorSpace.Gray;

			if ( cs == ColorSpace.RGB )
			{
				this.PutValue(color.R, 3);
				this.PutValue(color.G, 3);
				this.PutValue(color.B, 3);
			}

			if ( cs == ColorSpace.CMYK )
			{
				this.PutValue(color.C, 3);
				this.PutValue(color.M, 3);
				this.PutValue(color.Y, 3);
				this.PutValue(color.K, 3);
			}

			if ( cs == ColorSpace.Gray )
			{
				this.PutValue(color.Gray, 3);
			}
		}

		// Met un point.
		public void PutPoint(Point pos)
		{
			this.PutValue(pos.X);
			this.PutValue(pos.Y);
		}

		// Met une valeur avec 2 d�cimales.
		public void PutValue(double num)
		{
			this.PutValue(num, this.defaultDecimals);
		}

		// Met une valeur.
		public void PutValue(double num, int decimals)
		{
			this.stringBuilder.Append(Port.StringValue(num, decimals));
			this.stringBuilder.Append(" ");
		}

		// Met un entier.
		public void PutInt(int num)
		{
			this.stringBuilder.Append(num.ToString(System.Globalization.CultureInfo.InvariantCulture));
			this.stringBuilder.Append(" ");
		}

		// Met une fin de ligne.
		public void PutEOL()
		{
			int len = this.stringBuilder.Length;
			if ( len == 0 )  return;

			if ( this.stringBuilder[len-1] == ' ' )
			{
				this.stringBuilder.Remove(len-1, 1);  // enl�ve l'espace � la fin
			}

			this.stringBuilder.Append("\r\n");
		}

		// Met une commande de remplissage.
		protected void PutCommandFill()
		{
			if ( this.fillMode == FillMode.NonZero )
			{
				this.PutCommand("f ");  // nonzero fill, voir [*] page 202
			}
			else
			{
				this.PutCommand("f* ");  // even-odd fill
			}
		}

		// Met une commande de trac�.
		protected void PutCommandStroke()
		{
			if ( this.fillMode == FillMode.NonZero )
			{
				this.PutCommand("W ");  // nonzero stroke, voir [*] page 205
			}
			else
			{
				this.PutCommand("W* ");  // even-odd stroke
			}
		}

		// Met une commande quelconque.
		public void PutCommand(string cmd)
		{
			this.stringBuilder.Append(cmd);
		}


		// Met une commande "/BBox [x0 y0 x1 y1]".
		public static string StringBBox(Rectangle bbox)
		{
			return Port.StringBBox("/BBox", bbox);
		}

		// Met une commande "cmd [x0 y0 x1 y1]".
		public static string StringBBox(string cmd, Rectangle bbox)
		{
			return string.Format("{0} [{1} {2} {3} {4}] ", cmd, Port.StringValue(bbox.Left), Port.StringValue(bbox.Bottom), Port.StringValue(bbox.Right), Port.StringValue(bbox.Top));
		}

		// Met une commande "/Length x".
		public static string StringLength(int length)
		{
			return string.Format("/Length {0}", length);
		}

		// Met une valeur avec 2 d�cimales.
		public static string StringValue(double num)
		{
			return Port.StringValue(num, 2);
		}

		// Met une valeur.
		public static string StringValue(double num, int decimals)
		{
			if ( decimals == -1 )  // pr�cision automatique ?
			{
				double log = System.Math.Log10(System.Math.Abs(num));
				log = (log >= 0) ? System.Math.Ceiling(log) : -System.Math.Ceiling(-log);
				decimals = 4 - (int)log;  // 4 digits de pr�cision
				decimals = System.Math.Max(decimals, 1);
			}

			decimals = System.Math.Max(decimals, 0);
			decimals = System.Math.Min(decimals, 9);

			switch ( decimals )
			{
				case 1:   return string.Format("{0:0.#}", num);
				case 2:   return string.Format("{0:0.##}", num);
				case 3:   return string.Format("{0:0.###}", num);
				case 4:   return string.Format("{0:0.####}", num);
				case 5:   return string.Format("{0:0.#####}", num);
				case 6:   return string.Format("{0:0.######}", num);
				case 7:   return string.Format("{0:0.#######}", num);
				case 8:   return string.Format("{0:0.########}", num);
				case 9:   return string.Format("{0:0.#########}", num);
				default:  return string.Format("{0:0.##}", num);
			}
		}


		// Cherche la surface complexe � utiliser pour un objet et une propri�t�.
		public int SearchComplexSurface(Objects.Abstract obj, Properties.Abstract property)
		{
			if ( this.complexSurfaceList == null )  return -1;

			foreach ( ComplexSurface cs in this.complexSurfaceList )
			{
				if ( cs.Object == obj && cs.Fill == property )
				{
					return cs.Id;
				}
			}
			return -1;
		}

		// Cherche la surface complexe d'apr�s son identificateur.
		protected ComplexSurface GetComplexSurface(int id)
		{
			if ( this.complexSurfaceList == null )  return null;

			foreach ( ComplexSurface cs in this.complexSurfaceList )
			{
				if ( cs.Id == id )
				{
					return cs;
				}
			}
			return null;
		}

		// Cherche la surface complexe � utiliser pour une couleur transparente.
		protected int SearchComplexColor(RichColor color)
		{
			if ( this.complexSurfaceList == null )  return -1;

			foreach ( ComplexSurface cs in this.complexSurfaceList )
			{
				if ( cs.Fill is Properties.Font )
				{
					Properties.Font font = cs.Fill as Properties.Font;
					RichColor fcolor = this.GetFinalColor(font.FontColor);
					if ( fcolor == color )
					{
						return cs.Id;
					}
				}
			}
			return -1;
		}

		// Cherche l'image � utiliser.
		public ImageSurface SearchImageSurface(string filename)
		{
			if ( this.imageSurfaceList == null )  return null;
			return ImageSurface.Search(this.imageSurfaceList, filename);
		}


		protected System.Collections.ArrayList	complexSurfaceList;
		protected System.Collections.ArrayList	imageSurfaceList;
		protected System.Collections.Hashtable	fontList;

		protected ColorForce					colorForce;
		protected int							defaultDecimals;
		protected double						lineWidth;
		protected JoinStyle						lineJoin;
		protected CapStyle						lineCap;
		protected bool							lineDash;
		protected double						lineDashPen1;
		protected double						lineDashGap1;
		protected double						lineDashPen2;
		protected double						lineDashGap2;
		protected double						lineDashPen3;
		protected double						lineDashGap3;
		protected double						lineMiterLimit;
		protected RichColor						originalColor;
		protected RichColor						color;
		protected System.Collections.Stack		stackColorModifier;
		protected int							complexSurfaceId;
		protected TypeComplexSurface			complexType;
		protected Transform						transform;
		protected FillMode						fillMode;

		protected System.Text.StringBuilder		stringBuilder;
		protected RichColor						currentStrokeColor;
		protected RichColor						currentFillColor;
		protected int							currentComplexSurfaceId;
		protected double						currentWidth;
		protected CapStyle						currentCap;
		protected JoinStyle						currentJoin;
		protected bool							currentDash;
		protected double						currentPen1;
		protected double						currentGap1;
		protected double						currentPen2;
		protected double						currentGap2;
		protected double						currentPen3;
		protected double						currentGap3;
		protected double						currentLimit;
		protected Transform						currentTransform;
	}
}
