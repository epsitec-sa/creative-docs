//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 21/03/2004

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// La classe PrintPort permet d'imprimer des éléments graphiques simples.
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
		
		public void PaintText(double x, double y, string text, Drawing.Font font, double size)
		{
			System.Drawing.Font os_font = font.GetOsFont (size);
			
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
				
				font.GetGlyphAndTextCharEndX (text, out glyph_x, out glyph, out glyph_n);
				
				for (int i = 0; i < text.Length; i++)
				{
					path.Append (font, glyph[i], x, y, size);
					
					x += (glyph_x[i]-ox) * size;
					ox = glyph_x[i];
				}
				
				this.PaintSurface (path);
			}
			else
			{
				double[] end_x = font.GetTextCharEndX (text);
				double   ox    = 0;
			
				for (int i = 0; i < text.Length; i++)
				{
					this.graphics.DrawString (text.Substring (i, 1), os_font, this.brush, (float) x, (float) y);
					
					x += (end_x[i]-ox) * size;
					ox = end_x[i];
				}
			}
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
