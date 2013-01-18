//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.Pdf.Engine
{
	/// <summary>
	/// La classe Port permet d'exporter en PDF des éléments graphiques simples.
	/// L'unité est le dixième de millimètre (2100 = 21cm).
	/// L'origine graphique est en bas à gauche.
	/// [*] = documentation PDF Reference, version 1.6, fifth edition, 1236 pages
	/// </summary>
	public class Port : IPaintPort
	{
		public Port()
		{
			this.fontHash             = new FontHash ();
			this.characterHash        = new CharacterHash ();
			this.complexSurfaces      = new List<ComplexSurface> ();
			this.imageSurfaces        = new List<ImageSurface> ();
			this.stackColorModifier   = new Stack<ColorModifierCallback> ();
			this.nextComplexSurfaceId = 1;
			this.nextImageSurfaceId   = 1;

			this.Reset();
		}

		public void Reset()
		{
			//	Réinitialise le port, mais surtout pas le stack des modificateurs de couleurs.
			this.colorForce       = ColorForce.Default;
			this.defaultDecimals  = 2;
			this.LineWidth        = 1.0;
			this.lineJoin         = JoinStyle.MiterRevert;
			this.lineCap          = CapStyle.Square;
			this.lineDash         = false;
			this.lineDashPen1     = 0.0;
			this.lineDashGap1     = 0.0;
			this.lineDashPen2     = 0.0;
			this.lineDashGap2     = 0.0;
			this.lineDashPen3     = 0.0;
			this.lineDashGap3     = 0.0;
			this.lineMiterLimit   = 5.0;
			this.originalColor    = RichColor.FromBrightness(0.0);
			this.color            = RichColor.FromBrightness(0.0);
			this.complexSurfaceId = -1;
			this.complexType      = PdfComplexSurfaceType.ExtGState;
			this.transform        = Transform.Identity;
			this.fillMode         = FillMode.NonZero;

			this.Init();
		}

		public void Dispose()
		{
			this.fontHash.Clear ();
			this.characterHash.Clear ();

			//	Libère toutes les images.
			foreach (ImageSurface image in this.imageSurfaces)
			{
				image.Dispose ();
			}

			this.imageSurfaces.Clear ();
		}


		public bool TextToCurve
		{
			get;
			set;
		}

		public bool IsPreProcess
		{
			get;
			set;
		}

		public int CurrentPage
		{
			get;
			set;
		}


		public FontHash FontHash
		{
			get
			{
				return this.fontHash;
			}
		}

		public CharacterHash CharacterHash
		{
			get
			{
				return this.characterHash;
			}
		}

		public List<ComplexSurface> ComplexSurfaces
		{
			get
			{
				return this.complexSurfaces;
			}
		}

		public List<ImageSurface> ImageSurfaces
		{
			get
			{
				return this.imageSurfaces;
			}
		}


		public ColorForce ColorForce
		{
			//	Force un espace de couleur.
			get
			{
				return this.colorForce;
			}
			set
			{
				this.colorForce = value;
			}
		}

		public int DefaultDecimals
		{
			//	Indique le nombre de décimales par défaut.
			get
			{
				return this.defaultDecimals;
			}
			set
			{
				this.defaultDecimals = value;
			}
		}

		public double LineWidth
		{
			//	Spécifie un trait continu.
			get
			{
				return this.lineWidth;
			}
			set
			{
				this.lineWidth = value;
				this.lineDash = false;
			}
		}

		public void SetLineDash(double width, double pen1, double gap1, double pen2, double gap2, double pen3, double gap3)
		{
			//	Spécifie un traitillé.
			this.lineWidth    = width;
			this.lineDash     = true;
			this.lineDashPen1 = pen1;
			this.lineDashGap1 = gap1;
			this.lineDashPen2 = pen2;
			this.lineDashGap2 = gap2;
			this.lineDashPen3 = pen3;
			this.lineDashGap3 = gap3;
		}
		
		public bool LineDash
		{
			get
			{
				return this.lineDash;
			}
		}

		public double LineDashPen1
		{
			get
			{
				return this.lineDashPen1;
			}
		}

		public double LineDashGap1
		{
			get
			{
				return this.lineDashGap1;
			}
		}

		public double LineDashPen2
		{
			get
			{
				return this.lineDashPen2;
			}
		}

		public double LineDashGap2
		{
			get
			{
				return this.lineDashGap2;
			}
		}

		public double LineDashPen3
		{
			get
			{
				return this.lineDashPen3;
			}
		}

		public double LineDashGap3
		{
			get
			{
				return this.lineDashGap3;
			}
		}

		public JoinStyle LineJoin
		{
			get
			{
				return this.lineJoin;
			}
			set
			{
				this.lineJoin = value;
			}
		}

		public CapStyle LineCap
		{
			get
			{
				return this.lineCap;
			}
			set
			{
				this.lineCap = value;
			}
		}

		public double LineMiterLimit
		{
			get
			{
				return this.lineMiterLimit;
			}
			set
			{
				this.lineMiterLimit = value;
			}
		}

		public RichColor RichColor
		{
			get
			{
				return this.originalColor;
			}
			set
			{
				this.originalColor = value;
				this.FinalRichColor = this.GetFinalColor (value);
			}
		}

		public Color Color
		{
			get
			{
				return this.originalColor.Basic;
			}
			set
			{
				this.originalColor.Basic = value;
				this.FinalColor = this.GetFinalColor (value);
			}
		}

		public RichColor FinalRichColor
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.UpdateComplexSurfaceColor ();
			}
		}

		public Color FinalColor
		{
			get
			{
				return this.color.Basic;
			}
			set
			{
				this.color.Basic = value;
				this.UpdateComplexSurfaceColor ();
			}
		}

		public ImageFilter ImageFilter
		{
			get
			{
				return this.imageFilter;
			}
			set
			{
				this.imageFilter = value;
			}
		}

		public Margins ImageCrop
		{
			get
			{
				return this.imageCrop;
			}
			set
			{
				this.imageCrop = value;
			}
		}

		public Size ImageFinalSize
		{
			get
			{
				return this.imageFinalSize;
			}
			set
			{
				this.imageFinalSize = value;
			}
		}

		public void PushColorModifier(ColorModifierCallback method)
		{
			this.stackColorModifier.Push(method);
		}

		public ColorModifierCallback PopColorModifier()
		{
			return this.stackColorModifier.Pop() as ColorModifierCallback;
		}

		public RichColor GetFinalColor(RichColor color)
		{
			foreach ( ColorModifierCallback method in this.stackColorModifier )
			{
				color = method(color);
			}
			return color;
		}
		
		public Color GetFinalColor(Color color)
		{
			if ( this.stackColorModifier.Count == 0 )  return color;

			RichColor rich = RichColor.FromColor(color);
			foreach ( ColorModifierCallback method in this.stackColorModifier )
			{
				rich = method(rich);
			}
			return rich.Basic;
		}

		public FillMode FillMode
		{
			get
			{
				return this.fillMode;
			}
			set
			{
				this.fillMode = value;
			}
		}
		
		public Transform Transform
		{
			get
			{
			   return this.transform;
			}
			set
			{
				this.transform = value;
			}
		}


		#region Clipping
		public void SetClippingRectangle(Rectangle rect)
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
		
		public bool HasEmptyClippingRectangle
		{
			get
			{
				return false;
			}
		}
		#endregion
		
		
		public void Align(ref double x, ref double y)
		{
		}
		
		
		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			this.transform = this.transform.MultiplyByPostfix (Drawing.Transform.CreateScaleTransform (sx, sy, cx, cy));
		}
		
		public void RotateTransformDeg(double angle, double cx, double cy)
		{
			this.transform = this.transform.MultiplyByPostfix (Drawing.Transform.CreateRotationDegTransform (angle, cx, cy));
		}
		
		public void RotateTransformRad(double angle, double cx, double cy)
		{
			this.transform = this.transform.MultiplyByPostfix (Drawing.Transform.CreateRotationRadTransform (angle, cx, cy));
		}
		
		public void TranslateTransform(double ox, double oy)
		{
			this.transform = this.transform.MultiplyByPostfix (Drawing.Transform.CreateTranslationTransform (ox, oy));
		}
		
		public void MergeTransform(Transform transform)
		{
			this.transform = this.transform.MultiplyByPostfix (transform);
		}
		
		
		public void PaintOutline(Path path)
		{
			if (this.IsPreProcess)
			{
				this.AddComplexColor (this.color);
			}
			else
			{
				this.SetTransform (this.transform);
				this.SetWidth (this.lineWidth);
				this.SetCap (this.lineCap);
				this.SetJoin (this.lineJoin);
				this.SetDash (this.lineDash, this.lineDashPen1, this.lineDashGap1, this.lineDashPen2, this.lineDashGap2, this.lineDashPen3, this.lineDashGap3);
				this.SetLimit (this.lineMiterLimit);
				this.SetStrokeColor (this.color);
				this.PutPath (path);
				this.PutCommand ("S ");  // stroke, voir [*] page 200
				this.PutEOL ();
			}
		}
		
		public void PaintSurface(Path path)
		{
			if (this.IsPreProcess)
			{
				this.AddComplexColor (this.color);
			}
			else
			{
				this.SetTransform (this.transform);
				this.SetFillColor (this.color);
				this.DoFill (path);
				this.PutEOL ();
			}
		}

		private void AddComplexColor(RichColor color)
		{
			if (color.A == 1.0)  // couleur opaque ?
			{
				return;
			}

			//	S'il s'agit d'une couleur transparente, on ajoute une surface complexe
			//	qui s'y réfère.
			foreach (var x in this.complexSurfaces)
			{
				if (x.Page == this.CurrentPage &&
					x.Type == ComplexSurfaceType.TransparencyRegular &&
					x.Color == color)
				{
					return;
				}
			}

			var cs = new ComplexSurface (this.CurrentPage, color, this.nextComplexSurfaceId++);
			this.complexSurfaces.Add (cs);
		}
		
		
		public void PaintGlyphs(Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
		{
			if (this.IsPreProcess)
			{
				return;
			}

			int n = glyphs.Length;
			if ( n == 0 )  return;
			if ( n == 1 && glyphs[0] >= 0xffff )  return;
			
			if (this.TextToCurve)  // textes en courbes ?
			{
				Path path = new Path();
				Transform ft = font.SyntheticTransform;
				ft = ft.Scale(size);
			
				for ( int i=0 ; i<n ; i++ )
				{
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
			else	// textes en fontes ?
			{
				FontList fl = FontList.Search(this.fontHash, font);

				this.SetTransform(this.transform);
				this.SetFillColor(this.color);

				this.PutCommand("BT ");  // voir [*] page 375

				int lastFontPage = -1;
				double lastX = 0.0;
				double lastY = 0.0;
				double lastSizeX = 1.0;
				double lastSizeY = 1.0;
				for ( int i=0 ; i<n ; i++ )
				{
					int glyph = (int) glyphs[i];
					if ( glyph >= 0xffff )  continue;

					int code = fl.GetGlyphIndex((ushort)glyph);
					int fontPage = Export.GetFontPage (code);

					double posx = x[i];
					double posy = y[i];

					double sizeX = (sx == null) ? 1.0 : sx[i];
					double sizeY = (sy == null) ? 1.0 : sy[i];
					
					if ( fontPage != lastFontPage )
					{
						this.PutCommand(Export.GetFontShortName(fl.Id, fontPage));
						this.PutValue(size);
						this.PutCommand("Tf ");
						lastFontPage = fontPage;
					}

					if ( sizeX != lastSizeX || sizeY != lastSizeY )
					{
						this.PutValue(sizeX, -1);
						this.PutCommand("0 0 ");
						this.PutValue(sizeY, -1);
						this.PutPoint(new Point(posx, posy));
						this.PutCommand("Tm ");  // voir [*] page 376
						lastSizeX = sizeX;
						lastSizeY = sizeY;
					}
					else
					{
						this.PutPoint(new Point(posx-lastX, posy-lastY));
						this.PutCommand("Td ");  // voir [*] page 376
					}

					this.PutCommand("<");
					this.PutCommand (Export.GetFontIndex (code));
					this.PutCommand("> Tj ");  // voir [*] page 377

					lastX = posx;
					lastY = posy;
				}

				this.PutCommand("ET ");
				this.PutEOL();
			}
		}


		public static Size GetTextSingleLineSize(FormattedText formattedText, TextStyle style)
		{
			if (formattedText.IsNullOrEmpty ())
			{
				return Size.Empty;
			}
			else
			{
				var textLayout = Port.GetTextLayout (new Size (10000, 10000), formattedText, style);
				var size = textLayout.GetSingleLineSize ();
				size.Width += 1.0;  // il faut ajouter un chouia (0.1mm) pour éviter des plantées dans TextLayout !
				return size;
			}
		}

		public static double GetTextHeight(double width, FormattedText formattedText, TextStyle style)
		{
			if (formattedText.IsNullOrEmpty ())
			{
				return 0;
			}
			else
			{
				var textLayout = Port.GetTextLayout (new Size (width, 10000), formattedText, style);
				return textLayout.FindTextHeight ();
			}
		}

		public static double[] GetTextLineHeights(double width, FormattedText formattedText, TextStyle style)
		{
			if (formattedText.IsNullOrEmpty ())
			{
				return null;
			}
			else
			{
				var textLayout = Port.GetTextLayout (new Size (width, 10000), formattedText, style);

				var heights = new double[textLayout.TotalLineCount];
				for (int i=0; i<heights.Length; i++)
				{
					heights[i] = textLayout.GetLineHeight (i);
				}

				return heights;
			}
		}

		public void PaintText(Rectangle box, FormattedText formattedText, TextStyle style)
		{
			this.PaintText (box, Rectangle.MaxValue, formattedText, style);
		}

		public void PaintText(Rectangle box, Rectangle clipRect, FormattedText formattedText, TextStyle style)
		{
			if (this.IsPreProcess)
			{
				this.PreProcessText (box.Size, formattedText, style);
			}
			else
			{
				var textLayout = Port.GetTextLayout (box.Size, formattedText, style);
				textLayout.Paint (box.BottomLeft, this, clipRect, Color.Empty, GlyphPaintStyle.Normal);
			}
		}

		private void PreProcessText(Size boxSize, FormattedText formattedText, TextStyle style)
		{
			//	Préprocessing d'un texte. On construit la Hashtable de tous les
			//	caractères utilisés, et on collectionne les images utilisées.
			var textLayout = Port.GetTextLayout (boxSize, formattedText, style);

			//	On s'occupe des caractères.
			TextLayout.OneCharStructure[] fix = textLayout.ComputeStructure ();

			foreach (TextLayout.OneCharStructure oneChar in fix)
			{
				if (oneChar.Character != 0 && oneChar.Font != null)  // garde-fou
				{
					var cl = new CharacterList (oneChar);
					if (!this.characterHash.ContainsKey (cl))
					{
						this.characterHash.Add (cl, null);
					}
				}
			}

			//	On s'occupe des images.
			Dictionary<string, string> parameters;
			int offset = 0;
			while (offset < textLayout.Text.Length)
			{
				var tag = TextLayout.ParseTag (textLayout.Text, ref offset, out parameters);

				if (tag == TextLayout.Tag.Image)
				{
					if (parameters.ContainsKey ("src"))
					{
						var filename = parameters["src"];

						Image image = null;
						try
						{
							image = Bitmap.FromFile (filename);
						}
						catch
						{
						}

						if (image != null)
						{
							//	Le préprocessing d'une image n'a pas besoin des informations géométriques !
							this.PaintImage (image, 0, 0, 0, 0);
						}
					}
				}
			}
		}

		private static TextLayout GetTextLayout(Size boxSize, FormattedText formattedText, TextStyle style)
		{
			System.Diagnostics.Debug.Assert (style != null);

			return new TextLayout (style)
			{
				FormattedText = formattedText,
				LayoutSize    = boxSize,
			};
		}


		public double PaintText(double x, double y, string text, Font font, double fontSize)
		{
			if (this.IsPreProcess)
			{
				return 0;
			}

			if (this.TextToCurve)  // textes en courbes ?
			{
				int n = text.Length;
				if ( n == 0 )  return 0.0;

				var path = new Path();
				double width = 0.0;
				double ox;
				double[] glyphX;
				ushort[] glyph;
				byte[]   glyphN;

				font.GetGlyphsEndX(text, out glyphX, out glyph, out glyphN);
				System.Diagnostics.Debug.Assert(glyphX.Length == n);
				System.Diagnostics.Debug.Assert(glyph.Length == n);
				System.Diagnostics.Debug.Assert(glyphN.Length == n);

				Transform ft = font.SyntheticTransform;
				ft = ft.Scale(fontSize);

				for ( int i=0 ; i<n ; i++ )
				{
					ox = x + glyphX[i] * fontSize;
					path.Append(font, glyph[i], ft.XX, ft.XY, ft.YX, ft.YY, ft.TX+ox, ft.TY+y);
				}

				width = glyphX[n-1] * fontSize;

				this.SetTransform(this.transform);
				this.SetFillColor(this.color);
				this.DoFill(path);
				this.PutEOL();

				path.Dispose();
				return width;
			}
			else	// textes en fontes ?
			{
				FontList fl = FontList.Search(this.fontHash, font);

				int n = text.Length;
				if ( n == 0 )  return 0.0;

				double[] glyphX;
				ushort[] glyph;
				byte[]   glyphN;

				font.GetGlyphsEndX(text, out glyphX, out glyph, out glyphN);
				System.Diagnostics.Debug.Assert(glyphX.Length == n);
				System.Diagnostics.Debug.Assert(glyph.Length == n);
				System.Diagnostics.Debug.Assert(glyphN.Length == n);

				this.SetTransform(this.transform);
				this.SetFillColor(this.color);

				this.PutCommand("BT ");  // voir [*] page 375

				int lastFontPage = -1;
				double ox;

				for ( int i=0 ; i<n ; i++ )
				{
					ox = x + glyphX[i] * fontSize;

					int unicode = (int) text[i];
					int code = fl.GetUnicodeIndex(unicode);
					System.Diagnostics.Debug.Assert(code != -1);
					int fontPage = Export.GetFontPage (code);
					
					if ( fontPage != lastFontPage )
					{
						if ( lastFontPage != -1 )
						{
							this.PutCommand("> Tj ");  // voir [*] page 377
						}
						this.PutCommand(Export.GetFontShortName(fl.Id, fontPage));
						this.PutValue(fontSize);
						this.PutCommand("Tf ");
						this.PutPoint(new Point(ox, y));
						this.PutCommand("Td <");  // voir [*] page 376

						lastFontPage = fontPage;
					}

					this.PutCommand(Export.GetFontIndex (code));
				}

				this.PutCommand("> Tj ET ");
				this.PutEOL();

				return glyphX[n-1] * fontSize;
			}
		}
		
		public double PaintText(double x, double y, string text, Font font, double fontSize, FontClassInfo[] infos)
		{
#if false
			// Ce code ne fonctionne pas. Et je ne sais plus à quoi il sert !
			for ( int i=0 ; i<infos.Length ; i++ )
			{
				if ( infos[i].Scale != 1.00 &&
					 infos[i].GlyphClass == GlyphClass.Space )
				{
					string[] texts = text.Split(new char[] { ' ', (char) 160 });
					double spaceW = font.GetCharAdvance(' ') * fontSize * infos[i].Scale;
					double totalW = 0.0;
					
					for ( int j=0 ; j<texts.Length ; j++ )
					{
						double w = this.PaintText(x, y, texts[j], font, fontSize) + spaceW;
						
						totalW += w;
						x      += w;
					}
					
					return totalW-spaceW;
				}
			}
#endif
			
			return this.PaintText(x, y, text, font, fontSize);
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
			//	Tous les modes ne sont pas supportés. imageOriginX/imageOriginY doivent être nuls,
			//	et imageWidth/imageHeight doivent correspondre aux dimensions de l'image.
			//	Autrement dit, il faut dessiner toute l'image.
			if (this.IsPreProcess)
			{
				long uniqueId = ImageSurface.GetUniqueId (bitmap);

				var imageSurface = ImageSurface.Search (this.imageSurfaces, uniqueId);

				if (imageSurface == null)
				{
					imageSurface = new ImageSurface (uniqueId, this.nextImageSurfaceId++, bitmap);
					this.imageSurfaces.Add (imageSurface);
				}
			}
			else
			{
				System.Diagnostics.Debug.Assert (imageOriginX == 0);
				System.Diagnostics.Debug.Assert (imageOriginY == 0);
				System.Diagnostics.Debug.Assert (imageWidth == bitmap.Width);
				System.Diagnostics.Debug.Assert (imageHeight == bitmap.Height);

				long uniqueId = ImageSurface.GetUniqueId (bitmap);
				var imageSurface = ImageSurface.Search (this.imageSurfaces, uniqueId);

				if (imageSurface == null)
				{
					return;
				}

				var ot = this.Transform;

				this.TranslateTransform (fillX, fillY);
				this.TranslateTransform (fillWidth/2, fillHeight/2);
				this.ScaleTransform (fillWidth, -fillHeight, 0.0, 0.0);
				this.TranslateTransform (-0.5, -0.5);
				this.SetTransform (this.transform);

				this.PutCommand (Export.GetComplexSurfaceShortName (imageSurface.Id, PdfComplexSurfaceType.XObject));
				this.PutCommand ("Do ");  // external object, voir [*] page 302
				this.PutEOL ();

				this.Transform = ot;
			}
		}
		#endregion
		
		
		public StringBuffer GetPDF()
		{
			//	Donne tout le texte PDF généré.
			try
			{
				return this.stringBuilder;
			}
			finally
			{
				this.Reset ();
			}
		}

		private void Init()
		{
			//	Initialise tous les paramètres graphiques à des valeurs différentes
			//	des valeurs utilisées par la suite, ou aux valeurs par défaut.
			this.stringBuilder           = new StringBuffer ();
			this.currentWidth            = -1.0;
			this.currentCap              = (CapStyle) 999;
			this.currentJoin             = (JoinStyle) 999;
			this.currentDash             = false;
			this.currentPen1             = 0.0;
			this.currentGap1             = 0.0;
			this.currentPen2             = 0.0;
			this.currentGap2             = 0.0;
			this.currentLimit            = -1.0;
			this.currentStrokeColor      = RichColor.Empty;
			this.currentFillColor        = RichColor.Empty;
			this.currentComplexSurfaceId = -1;
			this.currentTransform        = Transform.Identity;
		}

		private void SetStrokeColor(RichColor color)
		{
			//	Spécifie la couleur de trait.
			if (this.currentStrokeColor != color || this.currentComplexSurfaceId != this.complexSurfaceId)
			{
				if (this.currentComplexSurfaceId != -1)
				{
					this.PutCommand (Export.GetComplexSurfaceShortName (0, PdfComplexSurfaceType.ExtGState));
					this.PutCommand ("gs ");  // graphic state, voir [*] page 189
				}

				if (this.currentStrokeColor.ColorSpace != color.ColorSpace)
				{
					this.PutStrokingColorSpace (color);
				}

				this.currentStrokeColor = color;
				this.currentComplexSurfaceId = this.complexSurfaceId;

				this.PutStrokingColor (color);

				if (this.complexSurfaceId != -1)
				{
					ComplexSurface cs = this.GetComplexSurface (this.complexSurfaceId);

					if (cs.Type == ComplexSurfaceType.TransparencyRegular  ||
						cs.Type == ComplexSurfaceType.TransparencyGradient)
					{
						this.PutCommand (Export.GetComplexSurfaceShortName (this.complexSurfaceId, this.complexType));
						this.PutCommand ("gs ");  // graphic state, voir [*] page 189
					}
				}
			}
		}

		private void SetFillColor(RichColor color)
		{
			//	Spécifie la couleur de surface.
			if (this.currentFillColor != color || this.currentComplexSurfaceId != this.complexSurfaceId)
			{
				if (this.currentComplexSurfaceId != -1)
				{
					this.PutCommand (Export.GetComplexSurfaceShortName (0, PdfComplexSurfaceType.ExtGState));
					this.PutCommand ("gs ");  // graphic state, voir [*] page 189
				}

				if (this.currentFillColor.ColorSpace != color.ColorSpace)
				{
					this.PutFillingColorSpace (color);
				}

				this.currentFillColor = color;
				this.currentComplexSurfaceId = this.complexSurfaceId;

				this.PutFillingColor (color);

				if (this.complexSurfaceId != -1)
				{
					ComplexSurface cs = this.GetComplexSurface (this.complexSurfaceId);

					if (cs.Type == ComplexSurfaceType.TransparencyRegular)
					{
						this.PutCommand (Export.GetComplexSurfaceShortName (this.complexSurfaceId, this.complexType));
						this.PutCommand ("gs ");  // graphic state, voir [*] page 189
					}
					else if (cs.Type == ComplexSurfaceType.TransparencyPattern &&
							 this.complexType != PdfComplexSurfaceType.ExtGState)
					{
						this.PutCommand (Export.GetComplexSurfaceShortName (this.complexSurfaceId, this.complexType));
						this.PutCommand ("gs ");  // graphic state, voir [*] page 189
					}
					else if (cs.Type == ComplexSurfaceType.TransparencyGradient)
					{
						this.PutCommand ("q ");
						this.PutCommand (Export.GetComplexSurfaceShortName (this.complexSurfaceId, this.complexType));
						this.PutCommand ("gs ");  // graphic state, voir [*] page 189
					}
				}
			}
		}

		private void DoFill(Path path)
		{
			//	Rempli la surface du chemin défini.
			if (this.complexSurfaceId == -1 || this.complexType != PdfComplexSurfaceType.ExtGState)
			{
				this.PutPath (path);
				this.PutCommandFill ();
			}
			else
			{
				ComplexSurface cs = this.GetComplexSurface (this.complexSurfaceId);

				if (cs.Type == ComplexSurfaceType.OpaqueGradient)
				{
					this.PutPath (path);
					this.PutCommand ("q ");  // save, voir [*] page 189
					this.PutCommandStroke ();
					this.PutCommand ("n ");
					//?this.PutTransform (cs.Matrix);  // current clipping, voir [*] page 205
					this.PutCommand (Export.GetComplexSurfaceShortName (this.complexSurfaceId, PdfComplexSurfaceType.ShadingColor));
					this.PutCommand ("sh Q ");  // shading, voir [*] page 273
				}
				else if (cs.Type == ComplexSurfaceType.TransparencyGradient)
				{
					this.PutPath (path);
					this.PutCommandStroke ();
					this.PutCommand ("n ");
					//?this.PutTransform (cs.Matrix);  // current clipping, voir [*] page 205
					this.PutCommand (Export.GetComplexSurfaceShortName (this.complexSurfaceId, PdfComplexSurfaceType.ShadingColor));
					this.PutCommand ("sh Q ");  // shading, voir [*] page 273
				}
				else if (cs.Type == ComplexSurfaceType.OpaquePattern       ||
						 cs.Type == ComplexSurfaceType.TransparencyPattern)
				{
					this.PutPath (path);
					this.PutCommand ("q ");  // save, voir [*] page 189
					this.PutCommandStroke ();
					this.PutCommand ("n ");
					this.PutCommand (Export.GetComplexSurfaceShortName (this.complexSurfaceId, PdfComplexSurfaceType.XObject));
					this.PutCommand ("Do Q ");  // external object, voir [*] page 302
				}
				else
				{
					this.PutPath (path);
					this.PutCommandFill ();
				}
			}
		}

		private void UpdateComplexSurfaceColor()
		{
			int id = -1;

			if (this.color.A < 1.0)  // couleur unie transparente ?
			{
				id = this.SearchComplexColor (this.CurrentPage, this.color);
			}

			this.complexSurfaceId = id;
			this.complexType = PdfComplexSurfaceType.ExtGState;
		}

		private void SetWidth(double width)
		{
			//	Spécifie l'épaisseur des traits.
			if ( this.currentWidth != width )
			{
				this.currentWidth = width;

				this.PutValue(width);
				this.PutCommand("w ");  // line width, voir [*] page 189
			}
		}

		private void SetCap(CapStyle cap)
		{
			//	Spécifie l'extrémité des traits.
			if ( this.currentCap != cap )
			{
				this.currentCap = cap;

				switch ( cap )
				{
					//	Line cap style, voir [*] page 186 et 189
					case CapStyle.Butt:    this.PutCommand("0 J ");  break;
					case CapStyle.Round:   this.PutCommand("1 J ");  break;
					case CapStyle.Square:  this.PutCommand("2 J ");  break;
				}
			}
		}

		private void SetJoin(JoinStyle join)
		{
			//	Spécifie les coins des traits.
			if ( this.currentJoin != join )
			{
				this.currentJoin = join;

				switch ( join )
				{
					//	Line join style, voir [*] page 186 et 189
					case JoinStyle.MiterRevert:
					case JoinStyle.Miter:  this.PutCommand("0 j ");  break;
					case JoinStyle.Round:  this.PutCommand("1 j ");  break;
					case JoinStyle.Bevel:  this.PutCommand("2 j ");  break;
				}
			}
		}

		private void SetDash(bool dash, double pen1, double gap1, double pen2, double gap2, double pen3, double gap3)
		{
			//	Spécifie le mode de traitillé.
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
					//	Line dash pattern, voir [*] page 188 et 189
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

		private void SetLimit(double limit)
		{
			//	Spécifie la limite pour JoinMiter.
			if ( this.currentLimit != limit )
			{
				this.currentLimit = limit;

				this.PutValue(limit, 1);
				this.PutCommand("M ");  // miter limit, voir [*] page 187 et 189
			}
		}

		private void SetTransform(Transform transform)
		{
			//	Spécifie la matrice de transformation.
			if ( this.currentTransform != transform )
			{
				Transform t = Transform.Inverse(this.currentTransform);
				t = t.MultiplyByPostfix(transform);

				//	Attention: inversion de XY et YX !
				this.PutValue(t.XX, -1);
				this.PutValue(t.YX, -1);  // xy
				this.PutValue(t.XY, -1);  // yx
				this.PutValue(t.YY, -1);
				this.PutValue(t.TX, -1);
				this.PutValue(t.TY, -1);
				this.PutCommand("cm ");  // current transform, voir [*] page 189

				this.currentTransform = transform;
			}
		}

		private void PutPath(Path path)
		{
			//	Met un chemin quelconque.
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
						Port.BezierS1ToS2(current, ref p1, ref p2, p3);
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

		private void PutTransform(Transform matrix)
		{
			//	Met une matrice de transformation.
			if ( matrix.XX != 1.0 ||
				 matrix.XY != 0.0 ||
				 matrix.YX != 0.0 ||
				 matrix.YY != 1.0 ||
				 matrix.TX != 0.0 ||
				 matrix.TY != 0.0 )  // autre que matrice identité ?
			{
				//	Attention: inversion de XY et YX !
				this.PutValue(matrix.XX, -1);
				this.PutValue(matrix.YX, -1);  // xy
				this.PutValue(matrix.XY, -1);  // yx
				this.PutValue(matrix.YY, -1);
				this.PutValue(matrix.TX, -1);
				this.PutValue(matrix.TY, -1);
				this.PutCommand("cm ");  // current transform, voir [*] page 189
			}
		}

		private void PutStrokingColorSpace(RichColor color)
		{
			//	Met un espace de couleur de trait.
			//	Stroking color, voir [*] page 257
			if ( this.colorForce == ColorForce.Nothing )  return;
			if ( this.colorForce != ColorForce.Default )  return;

			if ( color.ColorSpace == ColorSpace.Rgb  )  this.PutCommand("/DeviceRGB CS ");
			if ( color.ColorSpace == ColorSpace.Cmyk )  this.PutCommand("/DeviceCMYK CS ");
			if ( color.ColorSpace == ColorSpace.Gray )  this.PutCommand("/DeviceGray CS ");
		}

		private void PutFillingColorSpace(RichColor color)
		{
			//	Met un espace de couleur de trait.
			//	Stroking color, voir [*] page 257
			if ( this.colorForce == ColorForce.Nothing )  return;
			if ( this.colorForce != ColorForce.Default )  return;

			if ( color.ColorSpace == ColorSpace.Rgb  )  this.PutCommand("/DeviceRGB cs ");
			if ( color.ColorSpace == ColorSpace.Cmyk )  this.PutCommand("/DeviceCMYK cs ");
			if ( color.ColorSpace == ColorSpace.Gray )  this.PutCommand("/DeviceGray cs ");
		}

		private void PutStrokingColor(RichColor color)
		{
			//	Met une couleur de trait.
			//	Stroking color, voir [*] page 258
			if ( this.colorForce == ColorForce.Nothing )  return;

			this.PutColor(color);
			if ( this.colorForce == ColorForce.Default )  this.PutCommand("SC ");
			if ( this.colorForce == ColorForce.Rgb     )  this.PutCommand("RG ");
			if ( this.colorForce == ColorForce.Cmyk    )  this.PutCommand("K ");
			if ( this.colorForce == ColorForce.Gray    )  this.PutCommand("G ");
		}

		private void PutFillingColor(RichColor color)
		{
			//	Met une couleur de surface.
			//	Fill color, voir [*] page 258
			if ( this.colorForce == ColorForce.Nothing )  return;

			this.PutColor(color);
			if ( this.colorForce == ColorForce.Default )  this.PutCommand("sc ");
			if ( this.colorForce == ColorForce.Rgb     )  this.PutCommand("rg ");
			if ( this.colorForce == ColorForce.Cmyk    )  this.PutCommand("k ");
			if ( this.colorForce == ColorForce.Gray    )  this.PutCommand("g ");
		}

		private void PutColor(RichColor color)
		{
			//	Met une couleur (sans alpha).
			ColorSpace cs = color.ColorSpace;
			if ( this.colorForce == ColorForce.Rgb  )  cs = ColorSpace.Rgb;
			if ( this.colorForce == ColorForce.Cmyk )  cs = ColorSpace.Cmyk;
			if ( this.colorForce == ColorForce.Gray )  cs = ColorSpace.Gray;

			if ( cs == ColorSpace.Rgb )
			{
				this.PutValue(color.R, 3);
				this.PutValue(color.G, 3);
				this.PutValue(color.B, 3);
			}

			if ( cs == ColorSpace.Cmyk )
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

		private void PutPoint(Point pos)
		{
			//	Met un point.
			this.PutValue(pos.X);
			this.PutValue(pos.Y);
		}

		public void PutValue(double num)
		{
			//	Met une valeur avec 2 décimales.
			this.PutValue(num, this.defaultDecimals);
		}

		public void PutValue(double num, int decimals)
		{
			//	Met une valeur.
			this.stringBuilder.Append(Port.StringValue(num, decimals));
			this.stringBuilder.Append(" ");
		}

		private void PutInt(int num)
		{
			//	Met un entier.
			this.stringBuilder.Append(num.ToString(System.Globalization.CultureInfo.InvariantCulture));
			this.stringBuilder.Append(" ");
		}

		public void PutASCII85(byte[] data)
		{
			//	Met un buffer binaire en codage ASCII85 (voir [*] page 45).
			IO.Ascii85.Engine converter = new IO.Ascii85.Engine();
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			converter.EnforceMarks = false;
			converter.Encode(data, buffer);
			this.stringBuilder.Append (buffer.ToString ());
		}

		public void PutEOL()
		{
			this.stringBuilder.AppendNewLine ();
		}

		private void PutCommandFill()
		{
			//	Met une commande de remplissage.
			if ( this.fillMode == FillMode.NonZero )
			{
				this.PutCommand("f ");  // nonzero fill, voir [*] page 202
			}
			else
			{
				this.PutCommand("f* ");  // even-odd fill
			}
		}

		private void PutCommandStroke()
		{
			//	Met une commande de tracé.
			if (this.fillMode == FillMode.NonZero)
			{
				this.PutCommand ("W ");  // nonzero stroke, voir [*] page 205
			}
			else
			{
				this.PutCommand ("W* ");  // even-odd stroke
			}
		}

		public void PutCommand(string cmd)
		{
			//	Met une commande quelconque.
			this.stringBuilder.Append(cmd);
		}


		private static string StringBBox(Rectangle bbox)
		{
			//	Met une commande "/BBox [x0 y0 x1 y1]".
			return Port.StringBBox("/BBox", bbox);
		}

		public static string StringBBox(string cmd, Rectangle bbox)
		{
			//	Met une commande "cmd [x0 y0 x1 y1]".
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} [{1} {2} {3} {4}] ", cmd, Port.StringValue(bbox.Left), Port.StringValue(bbox.Bottom), Port.StringValue(bbox.Right), Port.StringValue(bbox.Top));
		}

		public static string StringLength(int length)
		{
			//	Met une commande "/Length x".
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "/Length {0}", length);
		}

		private static string StringValue(double num)
		{
			//	Met une valeur avec 2 décimales.
			return Port.StringValue(num, 2);
		}

		public static string StringValue(double num, int decimals)
		{
			//	Met une valeur.
			if ( decimals == -1 )  // précision automatique ?
			{
				double log = System.Math.Log10(System.Math.Abs(num));
				log = (log >= 0) ? System.Math.Ceiling(log) : -System.Math.Ceiling(-log);
				decimals = 4 - (int)log;  // 4 digits de précision
				decimals = System.Math.Max(decimals, 1);
			}

			decimals = System.Math.Max(decimals, 0);
			decimals = System.Math.Min(decimals, 9);

			switch ( decimals )
			{
				case 1:   return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.#}", num);
				case 2:   return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.##}", num);
				case 3:   return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.###}", num);
				case 4:   return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.####}", num);
				case 5:   return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.#####}", num);
				case 6:   return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.######}", num);
				case 7:   return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.#######}", num);
				case 8:   return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.########}", num);
				case 9:   return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.#########}", num);
				default:  return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.##}", num);
			}
		}


		private ComplexSurface GetComplexSurface(int id)
		{
			//	Cherche la surface complexe d'après son identificateur.
			foreach (ComplexSurface cs in this.complexSurfaces)
			{
				if (cs.Id == id)
				{
					return cs;
				}
			}
			return null;
		}

		private int SearchComplexColor(int page, RichColor color)
		{
			//	Cherche la surface complexe à utiliser pour une couleur transparente.
			foreach (ComplexSurface cs in this.complexSurfaces)
			{
				if (cs.Page == page && cs.Color == color)
				{
					return cs.Id;
				}
			}
			return -1;
		}


		private static void BezierS1ToS2(Point p1, ref Point s1, ref Point s2, Point p2)
		{
			//	Convertit une courbe de Bézier définie par un seul point secondaire en
			//	une courbe "traditionnelle" définie par deux points secondaires.
			//	Il s'agit ici d'une approximation empyrique !
			s1 = Point.Scale (p1, s1, 2.0/3.0);
			s2 = Point.Scale (p2, s2, 2.0/3.0);
		}


		private readonly FontHash						fontHash;
		private readonly CharacterHash					characterHash;
		private readonly List<ComplexSurface>			complexSurfaces;
		private readonly List<ImageSurface>				imageSurfaces;
		private readonly Stack<ColorModifierCallback>	stackColorModifier;

		private ColorForce						colorForce;
		private int								defaultDecimals;
		private double							lineWidth;
		private JoinStyle						lineJoin;
		private CapStyle						lineCap;
		private bool							lineDash;
		private double							lineDashPen1;
		private double							lineDashGap1;
		private double							lineDashPen2;
		private double							lineDashGap2;
		private double							lineDashPen3;
		private double							lineDashGap3;
		private double							lineMiterLimit;
		private RichColor						originalColor;
		private RichColor						color;
		private int								nextComplexSurfaceId;
		private int								nextImageSurfaceId;
		private int								complexSurfaceId;
		private PdfComplexSurfaceType			complexType;
		private Transform						transform;
		private FillMode						fillMode;
		private ImageFilter						imageFilter;
		private Margins							imageCrop;
		private Size							imageFinalSize;

		private StringBuffer					stringBuilder;
		private RichColor						currentStrokeColor;
		private RichColor						currentFillColor;
		private int								currentComplexSurfaceId;
		private double							currentWidth;
		private CapStyle						currentCap;
		private JoinStyle						currentJoin;
		private bool							currentDash;
		private double							currentPen1;
		private double							currentGap1;
		private double							currentPen2;
		private double							currentGap2;
		private double							currentPen3;
		private double							currentGap3;
		private double							currentLimit;
		private Transform						currentTransform;
	}
}