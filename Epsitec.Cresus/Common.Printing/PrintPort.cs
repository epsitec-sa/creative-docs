//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 21/03/2004

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// La classe PrintPort permet d'imprimer des �l�ments graphiques simples.
	/// </summary>
	public class PrintPort
	{
		internal PrintPort(System.Drawing.Graphics graphics, PageSettings settings)
		{
			this.graphics = graphics;
			this.settings = settings;
			this.brush    = System.Drawing.Brushes.Black;
			this.pen      = new System.Drawing.Pen (this.brush, 1.0f);
			
			if (this.settings != null)
			{
				double dx = this.settings.Bounds.Width;
				double dy = this.settings.Bounds.Height;
				
				this.graphics.ResetTransform ();
				this.graphics.TranslateTransform (0, (float) dy);
				this.graphics.ScaleTransform (0, -1);
			}
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
					this.color = value;
					this.InvalidateBrush ();
					this.InvalidatePen ();
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
			System.Drawing.Font os_font = font.GetOsFont (size);
			
			double width = 0;
			
			if (os_font == null)
			{
				Drawing.Path path = new Drawing.Path ();
				double       ox   = 0;
				
				double[] glyph_x;
				int[]    glyph;
				byte[]   glyph_n;
				
				//	On n'a pas r�ussi � obtenir la fonte syst�me pour repr�senter le texte, alors
				//	on va g�n�rer le chemin �quivalent et le peindre ainsi. Ca va revenir exactement
				//	au m�me.
				
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
				
				width = glyph_x[n-1];
				
				this.PaintSurface (path);
			}
			else
			{
				double[] end_x;
				double   ox = 0;
				
				font.GetTextCharEndX (text, out end_x);
			
				int n = text.Length;
				
				System.Diagnostics.Debug.Assert (end_x.Length == n);
				
				for (int i = 0; i < n; i++)
				{
					this.graphics.DrawString (text.Substring (i, 1), os_font, this.brush, (float) x, (float) y);
					
					x += (end_x[i]-ox) * size;
					ox = end_x[i];
				}
				
				width = end_x[n-1];
			}
			
			return width;
		}
		
		public double PaintText(double x, double y, string text, Drawing.Font font, double size, Drawing.Font.ClassInfo[] infos)
		{
			//	TODO: d�placer ce code dans la librairie AGG; faire en sorte que �a marche aussi
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
			
			if (ix1 < 0)			//	Clipping � gauche.
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
			
			if (ix2 > idx)			//	Clipping � droite
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
			System.Drawing.RectangleF dst = new System.Drawing.RectangleF ((float)(fill_x), (float)(fill_y), (float)(fill_width), (float)(fill_height));
			
			this.graphics.DrawImage (bitmap.BitmapImage.NativeBitmap, src, dst, System.Drawing.GraphicsUnit.Pixel);
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
				int a = 0;
				int r = 0;
				int g = 0;
				int b = 0;
				
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
		
		protected Drawing.Color					color = Drawing.Color.FromRGB (0, 0, 0);
	}
}
