//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// La classe PrintPort permet d'imprimer des éléments graphiques simples.
	/// </summary>
	public class PrintPort : Drawing.IPaintPort
	{
		public PrintPort(System.Drawing.Graphics graphics, PageSettings settings)
		{
			this.graphics = graphics;
			this.settings = settings;
			this.brush    = System.Drawing.Brushes.Black;
			this.pen      = new System.Drawing.Pen (this.brush, 1.0f);
			
			if (this.settings != null)
			{
				this.offset_x = (float) (- graphics.VisibleClipBounds.Left);
				this.offset_y = (float) (graphics.VisibleClipBounds.Top + graphics.VisibleClipBounds.Height);
				this.scale  = (float) (100.0 / 25.4);
			}
			
			this.transform = new Drawing.Transform ();
			this.stackColorModifier = new System.Collections.Stack();
			
			this.graphics.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			
			this.ResetTransform ();
		}
		
		public PrintPort(System.Drawing.Graphics graphics, int dx, int dy)
		{
			this.graphics = graphics;
			this.brush    = System.Drawing.Brushes.Black;
			this.pen      = new System.Drawing.Pen (this.brush, 1.0f);
			
			this.offset_x = 0;
			this.offset_y = (float) (dy);
			this.scale    = 1.0f;
			
			this.transform = new Drawing.Transform ();
			
			this.graphics.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			
			this.ResetTransform ();
		}
		
		
		public double							LineWidth
		{
			get
			{
				return this.line_width;
			}
			set
			{
				if (this.line_width != value)
				{
					this.line_width = value;
					this.InvalidatePen ();
				}
			}
		}
		
		public Drawing.JoinStyle				LineJoin
		{
			get
			{
				return this.line_join;
			}
			set
			{
				if (this.line_join != value)
				{
					this.line_join = value;
					this.InvalidatePen ();
				}
			}
		}
		
		public Drawing.CapStyle					LineCap
		{
			get
			{
				return this.line_cap;
			}
			set
			{
				if (this.line_cap != value)
				{
					this.line_cap = value;
					this.InvalidatePen ();
				}
			}
		}
		
		public double							LineMiterLimit
		{
			get
			{
				return this.line_miter_limit;
			}
			set
			{
				if (this.line_miter_limit != value)
				{
					this.line_miter_limit = value;
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
		
		public bool								ImageFilter
		{
			get
			{
				return this.image_filter;
			}
			set
			{
				this.image_filter = value;
			}
		}


		public Drawing.Margins					ImageCrop
		{
			get
			{
				return this.image_crop;
			}
			set
			{
				this.image_crop = value;
			}
		}

		public Drawing.Size						ImageFinalSize
		{
			get
			{
				return this.image_final_size;
			}
			set
			{
				this.image_final_size = value;
			}
		}

		public void PushColorModifier(Drawing.ColorModifierCallback method)
		{
			this.stackColorModifier.Push(method);
		}

		public Drawing.ColorModifierCallback PopColorModifier()
		{
			return this.stackColorModifier.Pop() as Drawing.ColorModifierCallback;
		}

		public System.Collections.Stack			StackColorModifier
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

		public Drawing.RichColor GetFinalColor(Drawing.RichColor color)
		{
			foreach ( Drawing.ColorModifierCallback method in this.stackColorModifier )
			{
				method(ref color);
			}
			return color;
		}
		
		public Drawing.Color GetFinalColor(Drawing.Color color)
		{
			if ( this.stackColorModifier.Count == 0 )  return color;

			Drawing.RichColor rich = Drawing.RichColor.FromColor(color);
			foreach ( Drawing.ColorModifierCallback method in this.stackColorModifier )
			{
				method(ref rich);
			}
			return rich.Basic;
		}
		
		
		public Drawing.FillMode					FillMode
		{
			get
			{
				return this.fill_mode;
			}
			set
			{
				this.fill_mode = value;
			}
		}
		
		public Drawing.Transform				Transform
		{
			get
			{
			   return new Drawing.Transform (this.transform);
			}
			set
			{
				if (this.transform != value)
				{
					this.ResetTransform ();
					
					this.transform = new Drawing.Transform (value);
					
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
		
		
		public void SetClippingRectangle(Drawing.Point p, Drawing.Size s)
		{
			this.SetClippingRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void SetClippingRectangle(Drawing.Rectangle rect)
		{
			this.SetClippingRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void SetClippingRectangle(double x, double y, double width, double height)
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
		
		public bool TestForEmptyClippingRectangle()
		{
			return this.clip.IsSurfaceZero;
		}
		
		
		public void Align(ref double x, ref double y)
		{
		}
		
		
		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			Drawing.Transform current = new Drawing.Transform (this.transform);
			Drawing.Transform scale   = Drawing.Transform.FromScale (sx, sy, cx, cy);
			
			scale.MultiplyBy (current);
			
			this.Transform = scale;
		}
		
		public void RotateTransformDeg(double angle, double cx, double cy)
		{
			Drawing.Transform current  = new Drawing.Transform (this.transform);
			Drawing.Transform rotation = Drawing.Transform.FromRotationDeg (angle, cx, cy);
			
			rotation.MultiplyBy (current);
			
			this.Transform = rotation;
		}
		
		public void RotateTransformRad(double angle, double cx, double cy)
		{
			Drawing.Transform current  = new Drawing.Transform (this.transform);
			Drawing.Transform rotation = Drawing.Transform.FromRotationRad (angle, cx, cy);
			
			rotation.MultiplyBy (current);
			
			this.Transform = rotation;
		}
		
		public void TranslateTransform(double ox, double oy)
		{
			Drawing.Transform current     = new Drawing.Transform (this.transform);
			Drawing.Transform translation = Drawing.Transform.FromTranslation (ox, oy);
			
			translation.MultiplyBy (current);
			
			this.Transform = translation;
		}
		
		public void MergeTransform(Drawing.Transform transform)
		{
			Drawing.Transform current = new Drawing.Transform (this.transform);
			
			transform.MultiplyBy (current);
			
			this.Transform = transform;
		}
		
		
		public void PaintOutline(Drawing.Path path)
		{
			if ((path != null) &&
				(this.line_width > 0))
			{
				this.UpdatePen ();
				
				System.Drawing.Drawing2D.GraphicsPath gra_path = path.CreateSystemPath ();
				gra_path.FillMode = this.fill_mode == Drawing.FillMode.EvenOdd ? System.Drawing.Drawing2D.FillMode.Alternate : System.Drawing.Drawing2D.FillMode.Winding;
				this.graphics.DrawPath (this.pen, gra_path);
			}
		}
		
		public void PaintSurface(Drawing.Path path)
		{
			if (path != null)
			{
				this.UpdateBrush ();
				System.Drawing.Drawing2D.GraphicsPath gra_path = path.CreateSystemPath ();
				gra_path.FillMode = this.fill_mode == Drawing.FillMode.EvenOdd ? System.Drawing.Drawing2D.FillMode.Alternate : System.Drawing.Drawing2D.FillMode.Winding;
				this.graphics.FillPath (this.brush, gra_path);
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
			
			ft.Scale (size);
			
			for (int i = 0; i < n; i++)
			{
				double scale_x = sx == null ? 1 : sx[i];
				double scale_y = sy == null ? 1 : sy[i];
				path.Append (font, glyphs[i], ft.XX * scale_x, ft.XY * scale_x, ft.YX * scale_y, ft.YY * scale_y, ft.TX * scale_x + x[i], ft.TY * scale_y + y[i]);
			}
			
			this.PaintSurface (path);
			path.Dispose ();
		}
		
		
		public double PaintText(double x, double y, string text, Drawing.Font font, double size)
		{
			return this.PaintText (x, y, text, font, size, true);
		}
		
		public double PaintText(double x, double y, string text, Drawing.Font font, double size, bool use_platform_font)
		{
			int n = text.Length;
			
			if (n == 0)
			{
				return 0.0;
			}
			
			this.UpdateBrush ();
			
			System.Drawing.Font os_font = null;
			
			try
			{
				if (use_platform_font)
				{
#if false
					os_font = font.GetOsFont (size);
#endif
				}
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine ("Cannot access OS font " + font.FullName + ", emulating using Path.");
				os_font = null;
			}
			
			
			
			double width = 0;
			
			if ((os_font == null) ||
				(graphics.Transform.Elements[1] != 0) ||
				(graphics.Transform.Elements[2] != 0))
			{
				Drawing.Path path = new Drawing.Path ();
				double       ox   = 0;
				
				double[] glyph_x;
				ushort[] glyph;
				byte[]   glyph_n;
				
				//	On n'a pas réussi à obtenir la fonte système pour représenter le texte, alors
				//	on va générer le chemin équivalent et le peindre ainsi. Ca va revenir exactement
				//	au même.
				
				font.GetGlyphsEndX (text, out glyph_x, out glyph, out glyph_n);
				
				System.Diagnostics.Debug.Assert (glyph_x.Length == n);
				System.Diagnostics.Debug.Assert (glyph.Length == n);
				System.Diagnostics.Debug.Assert (glyph_n.Length == n);
				
				Drawing.Transform ft = font.SyntheticTransform;
				
				ft.Scale (size);
				
				for (int i = 0; i < n; i++)
				{
					path.Append (font, glyph[i], ft.XX, ft.XY, ft.YX, ft.YY, ft.TX + x, ft.TY + y);
					
					x += (glyph_x[i]-ox) * size;
					ox = glyph_x[i];
				}
				
				width = glyph_x[n-1] * size;
				
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
					
					regions = graphics.MeasureCharacterRanges ("AAAAAAAAAAAA", os_font, rect, format);
					rect    = regions[0].GetBounds (graphics);
					
					double open_type_advance = font.GetCharAdvance ('A') * size;
					double gdi_plus_advance  = rect.Width;
					
					adjust = (float) (open_type_advance / gdi_plus_advance);
				}
				
				double[] end_x;
				double   ox = 0;
				
				font.GetTextCharEndX (text, out end_x);
				
				System.Drawing.FontFamily family = os_font.FontFamily;
				
				int ascent = family.GetCellAscent (System.Drawing.FontStyle.Regular);
				int descent= family.GetCellDescent (System.Drawing.FontStyle.Regular);
				int line   = family.GetLineSpacing (System.Drawing.FontStyle.Regular);
				int em_h   = family.GetEmHeight (System.Drawing.FontStyle.Regular);
				
//-				y += font.Ascender * size;
				y += ascent * size / em_h;
				
				System.Diagnostics.Debug.Assert (end_x.Length == n);
				
				System.Drawing.Drawing2D.Matrix matrix = this.graphics.Transform;
				
				for (int i = 0; i < n; i++)
				{
					float tx = (float) (x);
					float ty = (float) (y);
					
					this.graphics.TranslateTransform (tx, ty);
					this.graphics.ScaleTransform (1.0f * adjust, -1.0f * adjust);
					this.graphics.DrawString (text.Substring (i, 1), os_font, this.brush, 0, 0, System.Drawing.StringFormat.GenericTypographic);
					this.graphics.Transform = matrix;
					
					x += (end_x[i]-ox) * size;
					ox = end_x[i];
				}
				
				
				width = end_x[n-1] * size;
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
					double space_w = font.GetCharAdvance (' ') * size * infos[i].Scale;
					double total_w = 0;
					
					for (int j = 0; j < texts.Length; j++)
					{
						double w = this.PaintText (x, y, texts[j], font, size) + space_w;
						
						total_w += w;
						x       += w;
					}
					
					return total_w - space_w;
				}
			}
			
			return this.PaintText (x, y, text, font, size);
		}
		
		
		public void PaintImage(Drawing.Image bitmap, Drawing.Rectangle fill)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, 0, 0, bitmap.Width, bitmap.Height);
		}
		
		public void PaintImage(Drawing.Image bitmap, double fill_x, double fill_y, double fill_width, double fill_height)
		{
			this.PaintImage (bitmap, fill_x, fill_y, fill_width, fill_height, 0, 0, bitmap.Width, bitmap.Height);
		}
		
		public void PaintImage(Drawing.Image bitmap, Drawing.Rectangle fill, Drawing.Point image_origin)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, image_origin.X, image_origin.Y, fill.Width, fill.Height);
		}
		
		public void PaintImage(Drawing.Image bitmap, Drawing.Rectangle fill, Drawing.Rectangle image_rect)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, image_rect.Left, image_rect.Bottom, image_rect.Width, image_rect.Height);
		}
		
		public void PaintImage(Drawing.Image bitmap, double fill_x, double fill_y, double fill_width, double fill_height, double image_origin_x, double image_origin_y)
		{
			this.PaintImage (bitmap, fill_x, fill_y, fill_width, fill_height, image_origin_x, image_origin_y, fill_width, fill_height);
		}
		
		public void PaintImage(Drawing.Image bitmap, double fill_x, double fill_y, double fill_width, double fill_height, double image_origin_x, double image_origin_y, double image_width, double image_height)
		{
			double ix1 = image_origin_x;
			double iy1 = image_origin_y;
			double ix2 = image_origin_x + image_width;
			double iy2 = image_origin_y + image_height;
			
			double idx = bitmap.Width;
			double idy = bitmap.Height;
			
			if (ix1 >= idx) return;
			if (iy1 >= idy) return;
			
			if (ix1 < 0)			//	Clipping à gauche.
			{
				fill_x     -= ix1;
				fill_width += ix1;
				ix1 = 0;
			}
			
			if (iy1 < 0)			//	Clipping en bas
			{
				fill_y      -= iy1;
				fill_height += iy1;
				iy1 = 0;
			}
			
			if (ix2 > idx)			//	Clipping à droite
			{
				fill_width -= ix2 - idx;
				ix2 = idx;
			}
			
			if (iy2 > idy)			//	Clipping en haut
			{
				fill_height -= iy2 - idy;
				iy2 = idy;
			}
			
			System.Drawing.RectangleF src = new System.Drawing.RectangleF ((float)(ix1), (float)(iy1), (float)(ix2-ix1), (float)(iy2-iy1));
			System.Drawing.RectangleF dst = new System.Drawing.RectangleF (0, 0, (float)(fill_width), (float)(fill_height));
			
			float tx = (float)(fill_x);
			float ty = (float)(fill_y + fill_height);
			
			this.graphics.TranslateTransform (tx, ty);
			this.graphics.ScaleTransform (1, -1);
			this.graphics.DrawImage (bitmap.BitmapImage.NativeBitmap, dst, src, System.Drawing.GraphicsUnit.Pixel);
			this.graphics.ScaleTransform (1, -1);
			this.graphics.TranslateTransform (-tx, -ty);
		}
		
		
		protected void ResetTransform()
		{
			this.graphics.ResetTransform ();
			this.graphics.TranslateTransform (this.offset_x, this.offset_y);
			this.graphics.ScaleTransform (this.scale, -this.scale);
		}
		
		
		protected void InvalidatePen()
		{
			this.is_pen_dirty = true;
		}
		
		protected void InvalidateBrush()
		{
			this.is_brush_dirty = true;
		}
		
		protected void UpdatePen()
		{
			if (this.is_pen_dirty)
			{
				this.UpdateBrush ();
				
				this.pen.Brush      = this.brush;
				this.pen.Width      = (float) this.line_width;
				this.pen.MiterLimit = (float) this.line_miter_limit;
				
				switch (this.line_cap)
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
				
				switch (this.line_join)
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
				
				this.is_pen_dirty = false;
			}
		}
		
		protected void UpdateBrush()
		{
			if (this.is_brush_dirty)
			{
				int a = (int)(this.color.A * 255.5);
				int r = (int)(this.color.R * 255.5);
				int g = (int)(this.color.G * 255.5);
				int b = (int)(this.color.B * 255.5);
				
				System.Drawing.Color color = System.Drawing.Color.FromArgb (a, r, g, b);
				
				this.brush = new System.Drawing.SolidBrush (color);
				
				this.is_brush_dirty = false;
			}
		}
		
		
		
		protected PageSettings					settings;
		
		protected System.Drawing.Graphics		graphics;
		protected System.Drawing.Brush			brush;
		protected System.Drawing.Pen			pen;
		
		protected bool							is_pen_dirty;
		protected bool							is_brush_dirty;
		
		protected double						line_width       = 1.0;
		protected Drawing.JoinStyle				line_join        = Drawing.JoinStyle.Miter;
		protected Drawing.CapStyle				line_cap         = Drawing.CapStyle.Square;
		protected double						line_miter_limit = 4.0;
		protected bool							image_filter;
		protected Drawing.Margins				image_crop;
		protected Drawing.Size					image_final_size;
		
		protected float							offset_x, offset_y;
		protected float							scale;
		
		protected Drawing.Color					originalColor = Drawing.Color.FromRgb (0, 0, 0);
		protected Drawing.Color					color = Drawing.Color.FromRgb (0, 0, 0);
		protected System.Collections.Stack		stackColorModifier;
		protected Drawing.Rectangle				clip = Drawing.Rectangle.MaxValue;
		protected Drawing.Transform				transform = new Drawing.Transform ();
		protected Drawing.FillMode				fill_mode = Drawing.FillMode.NonZero;
	}
}
