//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// La classe PrintPort permet d'imprimer des éléments graphiques simples.
	/// </summary>
	public sealed class PrintPort : Drawing.IPaintPort
	{
		internal PrintPort(System.Drawing.Graphics graphics, PageSettings settings, System.Drawing.Printing.PrintPageEventArgs e)
		{
			this.graphics = graphics;
			this.settings = settings;
			this.brush    = System.Drawing.Brushes.Black;
			this.pen      = new System.Drawing.Pen (this.brush, 1.0f);
			
			if (this.settings != null)
			{
				System.Drawing.Rectangle bounds = PrintPort.GetRealMarginsBounds (e);

				//	1 display unit = 0.01 in = 0.254 mm
				
				this.offsetX = (float) (bounds.Left);
				this.offsetY = (float) (bounds.Top + bounds.Height);
				this.scale   = (float) (100.0 / 25.4);
			}

			this.transform = Drawing.Transform.Identity;
			this.stackColorModifier = new Stack<Drawing.ColorModifierCallback> ();
			
			this.graphics.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			
			this.ResetTransform ();
		}

		#region GetRealMarginsBounds Interop Code

		[System.Runtime.InteropServices.DllImport ("gdi32.dll")]
		private static extern int GetDeviceCaps(System.IntPtr hdc, DeviceCapsIndex index);

		private enum DeviceCapsIndex
		{
			PhysicalOffsetX = 112,
			PhysicalOffsetY = 113,
		}

		private static System.Drawing.Rectangle GetRealMarginsBounds(System.Drawing.Printing.PrintPageEventArgs e)
		{
			int cx = 0;
			int cy = 0;

			System.IntPtr hdc = e.Graphics.GetHdc ();

			try
			{
				cx = PrintPort.GetDeviceCaps (hdc, DeviceCapsIndex.PhysicalOffsetX);
				cy = PrintPort.GetDeviceCaps (hdc, DeviceCapsIndex.PhysicalOffsetY);
			}
			finally
			{
				e.Graphics.ReleaseHdc (hdc);
			}

			System.Drawing.Rectangle bounds = e.MarginBounds;

			int dpiX = (int) e.Graphics.DpiX;
			int dpiY = (int) e.Graphics.DpiY;

			bounds.Offset (-cx * 100 / dpiX, -cy * 100 / dpiY);
			
			return bounds;
		}

		#endregion

		internal PrintPort(System.Drawing.Graphics graphics, int dx, int dy)
		{
			this.graphics = graphics;
			this.brush    = System.Drawing.Brushes.Black;
			this.pen      = new System.Drawing.Pen (this.brush, 1.0f);
			
			this.offsetX = 0;
			this.offsetY = (float) (dy);
			this.scale   = 1.0f;

			this.transform = Drawing.Transform.Identity;

			this.stackColorModifier = new Stack<Drawing.ColorModifierCallback> ();
			
			this.graphics.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			
			this.ResetTransform ();
		}



		public double							LineWidth
		{
			get
			{
				return this.lineWidth;
			}
			set
			{
				if (this.lineWidth != value)
				{
					this.lineWidth = value;
					this.InvalidatePen ();
				}
			}
		}
		
		public Drawing.JoinStyle				LineJoin
		{
			get
			{
				return this.lineJoin;
			}
			set
			{
				if (this.lineJoin != value)
				{
					this.lineJoin = value;
					this.InvalidatePen ();
				}
			}
		}
		
		public Drawing.CapStyle					LineCap
		{
			get
			{
				return this.lineCap;
			}
			set
			{
				if (this.lineCap != value)
				{
					this.lineCap = value;
					this.InvalidatePen ();
				}
			}
		}
		
		public double							LineMiterLimit
		{
			get
			{
				return this.lineMiterLimit;
			}
			set
			{
				if (this.lineMiterLimit != value)
				{
					this.lineMiterLimit = value;
				}
			}
		}
		
		public Drawing.RichColor				RichColor
		{
			get
			{
				return Drawing.RichColor.FromColor(this.originalColor);
			}
			set
			{
				this.originalColor = value.Basic;
				this.FinalColor = this.GetFinalColor(value.Basic);
			}
		}
		
		public Drawing.Color					Color
		{
			get
			{
				return this.originalColor;
			}
			set
			{
				this.originalColor = value;
				this.FinalColor = this.GetFinalColor(value);
			}
		}
		
		public Drawing.RichColor				FinalRichColor
		{
			get
			{
				return Drawing.RichColor.FromColor(this.color);
			}
			set
			{
				Drawing.Color basic = value.Basic;
				if ( this.color != basic )
				{
					if ( basic.IsOpaque )
					{
						this.color = basic;
						this.InvalidateBrush();
						this.InvalidatePen();
					}
					else
					{
						throw new System.FormatException(string.Format("The color {0} is not compatible with the PrintPort.", value.ToString()));
					}
				}
			}
		}
		
		public Drawing.Color					FinalColor
		{
			get
			{
				return this.color;
			}
			set
			{
				if ( this.color != value )
				{
					if ( value.IsOpaque )
					{
						this.color = value;
						this.InvalidateBrush();
						this.InvalidatePen();
					}
					else
					{
						throw new System.FormatException(string.Format("The color {0} is not compatible with the PrintPort.", value.ToString()));
					}
				}
			}
		}
		
		public Drawing.ImageFilter				ImageFilter
		{
			get
			{
				return this.imageFilter;
			}
			set
			{
				if (this.imageFilter != value)
				{
					this.imageFilter = value;

					switch (this.imageFilter.Mode)
					{
						case Epsitec.Common.Drawing.ImageFilteringMode.None:
							this.graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
							break;

						case Epsitec.Common.Drawing.ImageFilteringMode.Bilinear:
							this.graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
							break;

						case Epsitec.Common.Drawing.ImageFilteringMode.Bessel:
						case Epsitec.Common.Drawing.ImageFilteringMode.Bicubic:
						case Epsitec.Common.Drawing.ImageFilteringMode.Blackman:
						case Epsitec.Common.Drawing.ImageFilteringMode.Catrom:
						case Epsitec.Common.Drawing.ImageFilteringMode.Gaussian:
						case Epsitec.Common.Drawing.ImageFilteringMode.Kaiser:
						case Epsitec.Common.Drawing.ImageFilteringMode.Lanczos:
						case Epsitec.Common.Drawing.ImageFilteringMode.Mitchell:
						case Epsitec.Common.Drawing.ImageFilteringMode.Quadric:
						case Epsitec.Common.Drawing.ImageFilteringMode.Sinc:
						case Epsitec.Common.Drawing.ImageFilteringMode.Spline16:
						case Epsitec.Common.Drawing.ImageFilteringMode.Spline36:
							this.graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
							break;

						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingBessel:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingBicubic:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingBlackman:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingCatrom:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingGaussian:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingKaiser:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingLanczos:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingMitchell:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingQuadric:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingSinc:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingSpline16:
						case Epsitec.Common.Drawing.ImageFilteringMode.ResamplingSpline36:
							this.graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
							break;
					}
				}
			}
		}


		public Drawing.Margins					ImageCrop
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

		public Drawing.Size						ImageFinalSize
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

		public void PushColorModifier(Drawing.ColorModifierCallback method)
		{
			this.stackColorModifier.Push(method);
		}

		public Drawing.ColorModifierCallback PopColorModifier()
		{
			return this.stackColorModifier.Pop();
		}

		public Drawing.RichColor GetFinalColor(Drawing.RichColor color)
		{
			foreach (Drawing.ColorModifierCallback method in this.stackColorModifier)
			{
				color = method (color);
			}
			return color;
		}

		public Drawing.Color GetFinalColor(Drawing.Color color)
		{
			if (this.stackColorModifier.Count == 0)
				return color;

			Drawing.RichColor rich = Drawing.RichColor.FromColor (color);
			foreach (Drawing.ColorModifierCallback method in this.stackColorModifier)
			{
				rich = method (rich);
			}
			return rich.Basic;
		}
		
		
		public Drawing.FillMode					FillMode
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
		
		public Drawing.Transform				Transform
		{
			get
			{
			   return this.transform;
			}
			set
			{
				if (this.transform != value)
				{
					this.ResetTransform ();
					
					this.transform = value;
					
					float m11 = (float)(transform.XX);
					float m12 = (float)(transform.YX);
					float m21 = (float)(transform.XY);
					float m22 = (float)(transform.YY);
					float dx  = (float)(transform.TX);
					float dy  = (float)(transform.TY);
					
					System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix (m11, m12, m21, m22, dx, dy);
					
					this.graphics.MultiplyTransform (matrix, System.Drawing.Drawing2D.MatrixOrder.Prepend);
				}
			}
		}

		public PageSettings						PageSettings
		{
			get
			{
				return this.settings;
			}
		}
		
		
		public void SetClippingRectangle(Drawing.Rectangle rect)
		{
			this.SetClippingRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		private void SetClippingRectangle(double x, double y, double width, double height)
		{
			//	Le rectangle de clipping est passé en coordonnées brutes (sans aucune
			//	transformation), c'est pourquoi on supprime temporairement la transformation
			//	avant d'appliquer le rectangle de clipping :
			
			System.Drawing.Drawing2D.Matrix matrix = this.graphics.Transform;
			System.Drawing.RectangleF       rect   = new System.Drawing.RectangleF ((float)(x), (float)(y), (float)(width), (float)(height));
			
			this.ResetTransform ();
			
			this.graphics.SetClip (rect, System.Drawing.Drawing2D.CombineMode.Intersect);
			this.graphics.Transform = matrix;
			
			this.clip = Drawing.Rectangle.Intersection (this.clip, new Drawing.Rectangle (x, y, width, height));
		}
		
		public Drawing.Rectangle SaveClippingRectangle()
		{
			return this.clip;
		}
		
		public void RestoreClippingRectangle(Drawing.Rectangle clip)
		{
			if (clip == Drawing.Rectangle.MaxValue)
			{
				this.ResetClippingRectangle ();
				return;
			}
			
			//	Le rectangle de clipping est passé en coordonnées brutes (sans aucune
			//	transformation), c'est pourquoi on supprime temporairement la transformation
			//	avant d'appliquer le rectangle de clipping :
			
			System.Drawing.Drawing2D.Matrix matrix = this.graphics.Transform;
			System.Drawing.RectangleF       rect   = new System.Drawing.RectangleF ((float)(clip.X), (float)(clip.Y), (float)(clip.Width), (float)(clip.Height));
			
			this.ResetTransform ();
			
			this.graphics.SetClip (rect, System.Drawing.Drawing2D.CombineMode.Replace);
			this.graphics.Transform = matrix;
			
			this.clip = clip;
		}
		
		public void ResetClippingRectangle()
		{
			this.clip = Drawing.Rectangle.MaxValue;
			this.graphics.ResetClip ();
		}
		
		public bool HasEmptyClippingRectangle
		{
			get
			{
				return this.clip.IsSurfaceZero;
			}
		}
		
		
		public void Align(ref double x, ref double y)
		{
		}
		
		
		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			Drawing.Transform scale = Drawing.Transform.CreateScaleTransform (sx, sy, cx, cy);
			this.Transform = scale.MultiplyBy (this.transform);
		}
		
		public void RotateTransformDeg(double angle, double cx, double cy)
		{
			Drawing.Transform rotation = Drawing.Transform.CreateRotationDegTransform (angle, cx, cy);
			this.Transform = rotation.MultiplyBy (this.transform);
		}
		
		public void RotateTransformRad(double angle, double cx, double cy)
		{
			Drawing.Transform rotation = Drawing.Transform.CreateRotationRadTransform (angle, cx, cy);
			this.Transform = rotation.MultiplyBy (this.transform);
		}
		
		public void TranslateTransform(double ox, double oy)
		{
			Drawing.Transform translation = Drawing.Transform.CreateTranslationTransform (ox, oy);
			this.Transform = translation.MultiplyBy (this.transform);
		}
		
		public void MergeTransform(Drawing.Transform transform)
		{
			this.Transform = transform.MultiplyBy (this.transform);
		}
		
		
		public void PaintOutline(Drawing.Path path)
		{
			if ((path != null) &&
				(this.lineWidth > 0))
			{
				this.UpdatePen ();
				
				System.Drawing.Drawing2D.GraphicsPath graPath = path.CreateSystemPath ();
				graPath.FillMode = this.fillMode == Drawing.FillMode.EvenOdd ? System.Drawing.Drawing2D.FillMode.Alternate : System.Drawing.Drawing2D.FillMode.Winding;
				this.graphics.DrawPath (this.pen, graPath);
			}
		}
		
		public void PaintSurface(Drawing.Path path)
		{
			if (path != null)
			{
				this.UpdateBrush ();
				System.Drawing.Drawing2D.GraphicsPath graPath = path.CreateSystemPath ();
				graPath.FillMode = this.fillMode == Drawing.FillMode.EvenOdd ? System.Drawing.Drawing2D.FillMode.Alternate : System.Drawing.Drawing2D.FillMode.Winding;
				this.graphics.FillPath (this.brush, graPath);
			}
		}
		
		
		public void PaintGlyphs(Drawing.Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
		{
			int n = glyphs.Length;
			
			if (n == 0)
			{
				return;
			}
			
			this.UpdateBrush ();
			
			//	Comme GDI+ ne sait pas accéder à une fonte par ses glyphes, on triche ici
			//	en peignant la surface des caractères, en attendant d'avoir le temps de
			//	faire appel à ExtTextOut avec ETO_GLYPH_INDEX.
			
			Drawing.Path path = new Drawing.Path ();
			Drawing.Transform ft = font.SyntheticTransform;
			
			ft = ft.Scale (size);
			
			for (int i = 0; i < n; i++)
			{
				double scaleX = sx == null ? 1 : sx[i];
				double scaleY = sy == null ? 1 : sy[i];
				path.Append (font, glyphs[i], ft.XX * scaleX, ft.XY * scaleX, ft.YX * scaleY, ft.YY * scaleY, ft.TX * scaleX + x[i], ft.TY * scaleY + y[i]);
			}
			
			this.PaintSurface (path);
			path.Dispose ();
		}
		
		
		public double PaintText(double x, double y, string text, Drawing.Font font, double size)
		{
			return this.PaintText (x, y, text, font, size, true);
		}
		
		public double PaintText(double x, double y, string text, Drawing.Font font, double size, bool usePlatformFont)
		{
			int n = text.Length;
			
			if (n == 0)
			{
				return 0.0;
			}
			
			this.UpdateBrush ();
			
			System.Drawing.Font osFont = null;
			
			try
			{
				if (usePlatformFont)
				{
#if false
					osFont = font.GetOsFont (size);
#endif
				}
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine ("Cannot access OS font " + font.FullName + ", emulating using Path.");
				osFont = null;
			}
			
			
			
			double width = 0;
			
			if ((osFont == null) ||
				(graphics.Transform.Elements[1] != 0) ||
				(graphics.Transform.Elements[2] != 0))
			{
				Drawing.Path path = new Drawing.Path ();
				
				double[] glyphX;
				ushort[] glyph;
				byte[]   glyphN;
				
				//	On n'a pas réussi à obtenir la fonte système pour représenter le texte, alors
				//	on va générer le chemin équivalent et le peindre ainsi. Ca va revenir exactement
				//	au même.
				
				font.GetGlyphsEndX (text, out glyphX, out glyph, out glyphN);
				
				System.Diagnostics.Debug.Assert (glyphX.Length == n);
				System.Diagnostics.Debug.Assert (glyph.Length == n);
				System.Diagnostics.Debug.Assert (glyphN.Length == n);
				
				Drawing.Transform ft = font.SyntheticTransform;
				
				ft = ft.Scale (size);
				
				for (int i = 0; i < n; i++)
				{
					double xx = x + glyphX[i]*size;
					path.Append(font, glyph[i], ft.XX, ft.XY, ft.YX, ft.YY, ft.TX + xx, ft.TY + y);
				}
				
				width = glyphX[n-1] * size;
				
				this.PaintSurface (path);
				path.Dispose ();
			}
			else
			{
				float adjust = 1f;
				
				if (true)
				{
					//	Algorithme méga-touille (brevet pas encore déposé) permettant de déterminer le facteur
					//	de correction à appliquer entre la taille de la fonte déterminée via les tables OpenType
					//	et AGG, et la taille que GDI+ utilise réellement :
					
					System.Drawing.StringFormat     format  = new System.Drawing.StringFormat (System.Drawing.StringFormat.GenericTypographic);
					System.Drawing.RectangleF       rect    = new System.Drawing.RectangleF (0f, 0f, 1000000f, 1000000f);
					System.Drawing.CharacterRange[] ranges  = { new System.Drawing.CharacterRange (10, 1) };
					System.Drawing.Region[]         regions = new System.Drawing.Region[1];
					
					format.SetMeasurableCharacterRanges (ranges);
					
					regions = graphics.MeasureCharacterRanges ("AAAAAAAAAAAA", osFont, rect, format);
					rect    = regions[0].GetBounds (graphics);
					
					double openTypeAdvance = font.GetCharAdvance ('A') * size;
					double gdiPlusAdvance  = rect.Width;
					
					adjust = (float) (openTypeAdvance / gdiPlusAdvance);
				}
				
				double[] endX;
				double   ox = 0;
				
				font.GetTextCharEndX (text, out endX);
				
				System.Drawing.FontFamily family = osFont.FontFamily;
				
				int ascent = family.GetCellAscent (System.Drawing.FontStyle.Regular);
				int descent= family.GetCellDescent (System.Drawing.FontStyle.Regular);
				int line   = family.GetLineSpacing (System.Drawing.FontStyle.Regular);
				int emH   = family.GetEmHeight (System.Drawing.FontStyle.Regular);
				
//-				y += font.Ascender * size;
				y += ascent * size / emH;
				
				System.Diagnostics.Debug.Assert (endX.Length == n);
				
				System.Drawing.Drawing2D.Matrix matrix = this.graphics.Transform;
				
				for (int i = 0; i < n; i++)
				{
					float tx = (float) (x);
					float ty = (float) (y);
					
					this.graphics.TranslateTransform (tx, ty);
					this.graphics.ScaleTransform (1.0f * adjust, -1.0f * adjust);
					this.graphics.DrawString (text.Substring (i, 1), osFont, this.brush, 0, 0, System.Drawing.StringFormat.GenericTypographic);
					this.graphics.Transform = matrix;
					
					x += (endX[i]-ox) * size;
					ox = endX[i];
				}
				
				
				width = endX[n-1] * size;
			}
			
			return width;
		}
		
		public double PaintText(double x, double y, string text, Drawing.Font font, double size, Drawing.FontClassInfo[] infos)
		{
			//	TODO: déplacer ce code dans la librairie AGG; faire en sorte que ça marche aussi
			//	si GlyphClass != GlyphClass.Space...
			
			for (int i = 0; i < infos.Length; i++)
			{
				if ((infos[i].Scale != 1.00) &&
					(infos[i].GlyphClass == Drawing.GlyphClass.Space))
				{
					string[] texts = text.Split (new char[] { ' ', (char) 160 });
					double spaceW = font.GetCharAdvance (' ') * size * infos[i].Scale;
					double totalW = 0;
					
					for (int j = 0; j < texts.Length; j++)
					{
						this.PaintText (x, y, texts[j], font, size);

						//	La largeur rendue par this.PaintText est fausse. Il faut la recalculer !
						double w = font.GetTextAdvance (texts[j]) * size + spaceW;
						
						totalW += w;
						x      += w;
					}
					
					return totalW - spaceW;
				}
			}
			
			return this.PaintText (x, y, text, font, size);
		}
		
		
		public void PaintImage(Drawing.Image bitmap, Drawing.Rectangle fill)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, 0, 0, bitmap.Width, bitmap.Height);
		}
		
		public void PaintImage(Drawing.Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight)
		{
			this.PaintImage (bitmap, fillX, fillY, fillWidth, fillHeight, 0, 0, bitmap.Width, bitmap.Height);
		}
		
		public void PaintImage(Drawing.Image bitmap, Drawing.Rectangle fill, Drawing.Point imageOrigin)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, imageOrigin.X, imageOrigin.Y, fill.Width, fill.Height);
		}
		
		public void PaintImage(Drawing.Image bitmap, Drawing.Rectangle fill, Drawing.Rectangle imageRect)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, imageRect.Left, imageRect.Bottom, imageRect.Width, imageRect.Height);
		}
		
		public void PaintImage(Drawing.Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight, double imageOriginX, double imageOriginY)
		{
			this.PaintImage (bitmap, fillX, fillY, fillWidth, fillHeight, imageOriginX, imageOriginY, fillWidth, fillHeight);
		}
		
		public void PaintImage(Drawing.Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight, double imageOriginX, double imageOriginY, double imageWidth, double imageHeight)
		{
			double ix1 = imageOriginX;
			double iy1 = imageOriginY;
			double ix2 = imageOriginX + imageWidth;
			double iy2 = imageOriginY + imageHeight;
			
			double idx = bitmap.Width;
			double idy = bitmap.Height;
			
			if (ix1 >= idx) return;
			if (iy1 >= idy) return;
			
			if (ix1 < 0)			//	Clipping à gauche.
			{
				fillX     -= ix1;
				fillWidth += ix1;
				ix1 = 0;
			}
			
			if (iy1 < 0)			//	Clipping en bas
			{
				fillY      -= iy1;
				fillHeight += iy1;
				iy1 = 0;
			}
			
			if (ix2 > idx)			//	Clipping à droite
			{
				fillWidth -= ix2 - idx;
				ix2 = idx;
			}
			
			if (iy2 > idy)			//	Clipping en haut
			{
				fillHeight -= iy2 - idy;
				iy2 = idy;
			}
			
			System.Drawing.RectangleF src = new System.Drawing.RectangleF ((float)(ix1), (float)(iy1), (float)(ix2-ix1), (float)(iy2-iy1));
			System.Drawing.RectangleF dst = new System.Drawing.RectangleF (0, 0, (float)(fillWidth), (float)(fillHeight));
			
			float tx = (float)(fillX);
			float ty = (float)(fillY + fillHeight);
			
			this.graphics.TranslateTransform (tx, ty);
			this.graphics.ScaleTransform (1, -1);
			this.graphics.DrawImage (bitmap.BitmapImage.NativeBitmap, dst, src, System.Drawing.GraphicsUnit.Pixel);
			this.graphics.ScaleTransform (1, -1);
			this.graphics.TranslateTransform (-tx, -ty);
		}


		public static void PrintToClipboardMetafile(System.Action<Drawing.IPaintPort> painter, int dx, int dy)
		{
			using (Metafile metafile = new Metafile (dx, dy))
			{
				PrintPort port = new PrintPort (metafile.Graphics, dx, dy);
				painter (port);
			}
		}

		public static void PrintToMetafile(System.Action<Drawing.IPaintPort> painter, string path, int dx, int dy)
		{
			using (Metafile metafile = new Metafile (path, dx, dy))
			{
				PrintPort port = new PrintPort (metafile.Graphics, dx, dy);
				painter (port);
			}
		}

		public static void PrintToBitmap(System.Action<Drawing.IPaintPort> painter, string path, int dx, int dy)
		{
			using (Bitmap bitmap = new Bitmap (path, dx, dy))
			{
				PrintPort port = new PrintPort (bitmap.Graphics, dx, dy);
				painter (port);
			}
		}

		public static void PrintFittedSinglePage(System.Action<Drawing.IPaintPort> painter, PrintDocument document, double dx, double dy)
		{
			var bounds = document.DefaultPageSettings.Bounds;
			var margins = document.DefaultPageSettings.Margins;

			double mx = System.Math.Max (margins.Left, margins.Right);
			double my = System.Math.Max (margins.Top, margins.Bottom);

			double pageDx = bounds.Width - 2 * mx;
			double pageDy = bounds.Height - 2 * my;

			double ratioX = dx / pageDx;
			double ratioY = dy / pageDy;

			double scale = 1 / System.Math.Max (ratioX, ratioY);

			var transform = Drawing.Transform.CreateScaleTransform (scale, scale);
			transform = transform.Translate ((bounds.Width - scale*dx) / 2, (bounds.Height - scale*dy) / 2);

			document.Print (new SinglePagePrintEngine (painter, transform));
		}

		public static void PrintSinglePage(System.Action<Drawing.IPaintPort> painter, PrintDocument document)
		{
			document.Print (new SinglePagePrintEngine (painter, Drawing.Transform.Identity));
		}

		public static void PrintSinglePage(System.Action<Drawing.IPaintPort> painter, PrintDocument document, Drawing.Transform transform)
		{
			document.Print(new SinglePagePrintEngine(painter, transform));
		}

		private void ResetTransform()
		{
			this.graphics.ResetTransform ();
			this.graphics.TranslateTransform (this.offsetX, this.offsetY);
			this.graphics.ScaleTransform (this.scale, -this.scale);
		}

		private void InvalidatePen()
		{
			this.isPenDirty = true;
		}

		private void InvalidateBrush()
		{
			this.isBrushDirty = true;
		}

		private void UpdatePen()
		{
			if (this.isPenDirty)
			{
				this.UpdateBrush ();
				
				this.pen.Brush      = this.brush;
				this.pen.Width      = (float) this.lineWidth;
				this.pen.MiterLimit = (float) this.lineMiterLimit;
				
				switch (this.lineCap)
				{
					case Drawing.CapStyle.Butt:
						this.pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
						this.pen.EndCap   = System.Drawing.Drawing2D.LineCap.Flat;
						break;
					case Drawing.CapStyle.Square:
						this.pen.StartCap = System.Drawing.Drawing2D.LineCap.Square;
						this.pen.EndCap   = System.Drawing.Drawing2D.LineCap.Square;
						break;
					case Drawing.CapStyle.Round:
						this.pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
						this.pen.EndCap   = System.Drawing.Drawing2D.LineCap.Round;
						break;
				}
				
				switch (this.lineJoin)
				{
					case Drawing.JoinStyle.Bevel:
						this.pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
						break;
					case Drawing.JoinStyle.Miter:
					case Drawing.JoinStyle.MiterRevert:
						this.pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
						break;
					case Drawing.JoinStyle.Round:
						this.pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
						break;
				}
				
				this.isPenDirty = false;
			}
		}

		private void UpdateBrush()
		{
			if (this.isBrushDirty)
			{
				int a = (int)(this.color.A * 255.5);
				int r = (int)(this.color.R * 255.5);
				int g = (int)(this.color.G * 255.5);
				int b = (int)(this.color.B * 255.5);
				
				System.Drawing.Color color = System.Drawing.Color.FromArgb (a, r, g, b);
				
				this.brush = new System.Drawing.SolidBrush (color);
				
				this.isBrushDirty = false;
			}
		}


		#region Metafile Class

		class Metafile : System.IDisposable
		{
			public Metafile(int dx, int dy)
				: this (Metafile.GetTempFilePath (), dx, dy)
			{
				this.copyToClipboard = true;
			}

			public Metafile(string path, int dx, int dy)
			{
				this.bitmap = new System.Drawing.Bitmap (1, 1, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				this.bitmapGraphics = System.Drawing.Graphics.FromImage (this.bitmap);
				this.units = System.Drawing.GraphicsUnit.Pixel;

				this.bitmapGraphics.FillRectangle (System.Drawing.Brushes.White, bitmap.GetBounds (ref units));

				this.hdc = bitmapGraphics.GetHdc ();
				this.metafile = new System.Drawing.Imaging.Metafile (path, this.hdc, new System.Drawing.Rectangle (0, 0, dx, dy), System.Drawing.Imaging.MetafileFrameUnit.Pixel);
				this.graphics = System.Drawing.Graphics.FromImage (this.metafile);
			}

			public System.Drawing.Graphics Graphics
			{
				get
				{
					return this.graphics;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.graphics.Dispose ();

				if (this.copyToClipboard)
				{
					Epsitec.Common.Drawing.Platform.ClipboardMetafileHelper.PutMetafileOnClipboardAndDispose (this.metafile);
				}
				else
				{
					this.metafile.Dispose ();
				}

				this.bitmapGraphics.ReleaseHdc (hdc);
				this.bitmapGraphics.Dispose ();
				this.bitmap.Dispose ();
			}

			#endregion

			private static string GetTempFilePath()
			{
				return System.IO.Path.GetTempFileName ();
			}


			readonly System.Drawing.Bitmap bitmap;
			readonly System.Drawing.Graphics bitmapGraphics;
			readonly System.Drawing.GraphicsUnit units;

			readonly System.IntPtr hdc;
			readonly System.Drawing.Imaging.Metafile metafile;
			readonly System.Drawing.Graphics graphics;
			readonly bool copyToClipboard;
		}

		#endregion

		#region Bitmap Class

		class Bitmap : System.IDisposable
		{
			public Bitmap(string path, int dx, int dy)
			{
				string ext = System.IO.Path.GetExtension (path).ToLowerInvariant ();

				switch (ext)
				{
					case ".bmp": this.format = System.Drawing.Imaging.ImageFormat.Bmp; break;
					case ".png": this.format = System.Drawing.Imaging.ImageFormat.Png; break;
					case ".gif": this.format = System.Drawing.Imaging.ImageFormat.Gif; break;

					default:
						throw new System.FormatException ("Unsupported image format : " + ext);
				}

				this.path = path;
				this.bitmap = new System.Drawing.Bitmap (dx, dy, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				this.bitmapGraphics = System.Drawing.Graphics.FromImage (this.bitmap);
				this.units = System.Drawing.GraphicsUnit.Pixel;

				this.bitmapGraphics.FillRectangle (System.Drawing.Brushes.White, bitmap.GetBounds (ref units));
			}

			public System.Drawing.Graphics Graphics
			{
				get
				{
					return this.bitmapGraphics;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.bitmapGraphics.Dispose ();
				this.bitmap.Save (this.path, this.format);
				this.bitmap.Dispose ();
			}

			#endregion

			readonly string path;
			readonly System.Drawing.Imaging.ImageFormat format;
			readonly System.Drawing.Bitmap bitmap;
			readonly System.Drawing.Graphics bitmapGraphics;
			readonly System.Drawing.GraphicsUnit units;
		}

		#endregion

		#region PrintEngine Class

		class SinglePagePrintEngine : IPrintEngine
		{
			public SinglePagePrintEngine(System.Action<Drawing.IPaintPort> print, Drawing.Transform transform)
			{
				this.print = print;
				this.transform = transform;
			}

			#region IPrintEngine Members

			public void PrepareNewPage(PageSettings settings)
			{
				settings.Margins = new Drawing.Margins (0, 0, 0, 0);
			}

			public void FinishingPrintJob()
			{
			}

			public void StartingPrintJob()
			{
			}

			public PrintEngineStatus PrintPage(PrintPort port)
			{
				port.Transform = transform;

				this.print (port);

				return PrintEngineStatus.FinishJob;
			}

			#endregion

			readonly System.Action<Drawing.IPaintPort> print;
			readonly Drawing.Transform transform;
		}

		#endregion



		private PageSettings					settings;

		private System.Drawing.Graphics			graphics;
		private System.Drawing.Brush			brush;
		private System.Drawing.Pen				pen;

		private bool							isPenDirty;
		private bool							isBrushDirty;

		private double							lineWidth       = 1.0;
		private Drawing.JoinStyle				lineJoin        = Drawing.JoinStyle.Miter;
		private Drawing.CapStyle				lineCap         = Drawing.CapStyle.Square;
		private double							lineMiterLimit = 4.0;
		private Drawing.ImageFilter				imageFilter;
		private Drawing.Margins					imageCrop;
		private Drawing.Size					imageFinalSize;

		private float							offsetX, offsetY;
		private float							scale;

		private Drawing.Color					originalColor = Drawing.Color.FromRgb (0, 0, 0);
		private Drawing.Color					color = Drawing.Color.FromRgb (0, 0, 0);
		private readonly Stack<Drawing.ColorModifierCallback>		stackColorModifier;
		private Drawing.Rectangle				clip = Drawing.Rectangle.MaxValue;
		private Drawing.Transform				transform = Drawing.Transform.Identity;
		private Drawing.FillMode				fillMode = Drawing.FillMode.NonZero;
	}
}
