//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 21/03/2004

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
			
			this.ResetTransform ();
		}
		
		public PrintPort(System.Drawing.Graphics graphics, int dx, int dy)
		{
			this.graphics = graphics;
			this.settings = settings;
			this.brush    = System.Drawing.Brushes.Black;
			this.pen      = new System.Drawing.Pen (this.brush, 1.0f);
			
			this.offset_x = 0;
			this.offset_y = (float) (dy);
			this.scale    = 1.0f;
			
			this.transform = new Drawing.Transform ();
			
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
		
		public Drawing.Color					Color
		{
			get
			{
				return this.color;
			}
			set
			{
				if (this.color != value)
				{
					if (value.IsOpaque)
					{
						this.color = value;
						this.InvalidateBrush ();
						this.InvalidatePen ();
					}
					else
					{
						throw new System.FormatException (string.Format ("The color {0} is not compatible with the PrintPort.", value.ToString ()));
					}
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
			if (clip == Drawing.Rectangle.Infinite)
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
			this.clip = Drawing.Rectangle.Infinite;
			this.graphics.ResetClip ();
		}
		
		public bool TestForEmptyClippingRectangle()
		{
			return this.clip.IsSurfaceZero;
		}
		
		
		public void Align(ref double x, ref double y)
		{
		}
		
		
		public Drawing.Transform SaveTransform()
		{
			return new Drawing.Transform (this.transform);
		}
		
		public double GetTransformZoom()
		{
			//	Détermine le zoom approximatif en vigueur dans la transformation actuelle.
			//	Calcule la longueur d'un segment diagonal [1 1] après transformation pour
			//	connaître ce zoom.
			
			Drawing.Transform transform = this.SaveTransform ();
			
			double a = transform.XX + transform.XY;
			double b = transform.YX + transform.YY;
			
			return System.Math.Sqrt ((a*a + b*b) / 2);
		}
		
		public void RestoreTransform(Drawing.Transform transform)
		{
			this.ResetTransform ();
			
			this.transform = new Drawing.Transform (transform);
			
			float m11 = (float)(transform.XX);
			float m12 = (float)(transform.YX);
			float m21 = (float)(transform.XY);
			float m22 = (float)(transform.YY);
			float dx  = (float)(transform.TX);
			float dy  = (float)(transform.TY);
			
			System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix (m11, m12, m21, m22, dx, dy);
			
			this.graphics.MultiplyTransform (matrix, System.Drawing.Drawing2D.MatrixOrder.Prepend);
		}
		
		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			Drawing.Transform transform = new Drawing.Transform (this.transform);
			
			transform.MultiplyByPostfix (Drawing.Transform.FromScale (sx, sy, cx, cy));
			
			this.RestoreTransform (transform);
		}
		
		public void RotateTransform(double angle, double cx, double cy)
		{
			Drawing.Transform transform = new Drawing.Transform (this.transform);
			
			transform.MultiplyByPostfix (Drawing.Transform.FromRotation (angle, cx, cy));
			
			this.RestoreTransform (transform);
		}
		
		public void TranslateTransform(double ox, double oy)
		{
			Drawing.Transform transform = new Drawing.Transform (this.transform);
			
			transform.MultiplyByPostfix (Drawing.Transform.FromTranslation (ox, oy));
			
			this.RestoreTransform (transform);
		}
		
		public void MergeTransform(Drawing.Transform transform)
		{
			Drawing.Transform current_transform = new Drawing.Transform (this.transform);
			
			transform.MultiplyByPostfix (current_transform);
			
			this.RestoreTransform (current_transform);
		}
		
		
		public void PaintOutline(Drawing.Path path)
		{
			this.UpdatePen ();
			this.graphics.DrawPath (this.pen, path.CreateSystemPath ());
		}
		
		public void PaintSurface(Drawing.Path path)
		{
			this.UpdateBrush ();
			this.graphics.FillPath (this.brush, path.CreateSystemPath ());
		}
		
		
		public double PaintText(double x, double y, string text, Drawing.Font font, double size)
		{
			this.UpdateBrush ();
			
			System.Drawing.Font os_font = font.GetOsFont (size);
			
			double width = 0;
			
			if (os_font == null)
			{
				Drawing.Path path = new Drawing.Path ();
				double       ox   = 0;
				
				double[] glyph_x;
				int[]    glyph;
				byte[]   glyph_n;
				
				//	On n'a pas réussi à obtenir la fonte système pour représenter le texte, alors
				//	on va générer le chemin équivalent et le peindre ainsi. Ca va revenir exactement
				//	au même.
				
				font.GetGlyphsEndX (text, out glyph_x, out glyph, out glyph_n);
				
				int n = text.Length;
				
				System.Diagnostics.Debug.Assert (glyph_x.Length == n);
				System.Diagnostics.Debug.Assert (glyph.Length == n);
				System.Diagnostics.Debug.Assert (glyph_n.Length == n);
				
				for (int i = 0; i < n; i++)
				{
					path.Append (font, glyph[i], x, y, size);
					
					x += (glyph_x[i]-ox) * size;
					ox = glyph_x[i];
				}
				
				width = glyph_x[n-1] * size;
				
				this.PaintSurface (path);
			}
			else
			{
				double[] end_x;
				double   ox = 0;
				
				font.GetTextCharEndX (text, out end_x);
				
				y += font.Ascender * size;
				
				int n = text.Length;
				
				System.Diagnostics.Debug.Assert (end_x.Length == n);
				
				for (int i = 0; i < n; i++)
				{
					float tx = (float) (x);
					float ty = (float) (y);
					
					this.graphics.TranslateTransform (tx, ty);
					this.graphics.ScaleTransform (1, -1);
					this.graphics.DrawString (text.Substring (i, 1), os_font, this.brush, 0, 0);
					this.graphics.ScaleTransform (1, -1);
					this.graphics.TranslateTransform (-tx, -ty);
					
					x += (end_x[i]-ox) * size;
					ox = end_x[i];
				}
				
				
				width = end_x[n-1] * size;
			}
			
			return width;
		}
		
		public double PaintText(double x, double y, string text, Drawing.Font font, double size, Drawing.Font.ClassInfo[] infos)
		{
			//	TODO: déplacer ce code dans la librairie AGG; faire en sorte que ça marche aussi
			//	si ClassId != ClassId.Space...
			
			for (int i = 0; i < infos.Length; i++)
			{
				if ((infos[i].Scale != 1.00) &&
					(infos[i].ClassId == Drawing.Font.ClassId.Space))
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
		
		protected float							offset_x, offset_y;
		protected float							scale;
		
		protected Drawing.Color					color = Drawing.Color.FromRGB (0, 0, 0);
		protected Drawing.Rectangle				clip = Drawing.Rectangle.Infinite;
		protected Drawing.Transform				transform = new Drawing.Transform ();
	}
}
