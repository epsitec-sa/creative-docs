namespace Epsitec.Common.Drawing
{
	public enum ContentAlignment
	{
		BottomLeft,
		BottomCenter,
		BottomRight,
		
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		
		TopLeft,
		TopCenter,
		TopRight
	}
	
	/// <summary>
	/// La classe Graphics encapsule le contexte graphique utilisé pour peindre.
	/// </summary>
	public abstract class Graphics : System.IDisposable
	{
		protected Graphics()
		{
			this.ResetLineStyle ();
		}

		~ Graphics()
		{
			this.Dispose (false);
		}
		
		
		public virtual void RenderSolid()
		{
		}
		
		public virtual void RenderSolid(Color color)
		{
		}
		
		public virtual void RenderImage()
		{
		}
		
		public virtual void RenderGradient()
		{
		}
		
		
		public void ResetLineStyle()
		{
			this.LineWidth      = 1.0;
			this.LineJoin       = JoinStyle.Miter;
			this.LineCap        = CapStyle.Square;
			this.LineMiterLimit = 4.0;
		}
		
		
		public double					LineWidth
		{
			get { return this.line_width; }
			set { this.line_width = value; }
		}
		
		public JoinStyle				LineJoin
		{
			get { return this.line_join; }
			set { this.line_join = value; }
		}
		
		public CapStyle					LineCap
		{
			get { return this.line_cap; }
			set { this.line_cap = value; }
		}
		
		public double					LineMiterLimit
		{
			get { return this.line_miter_limit; }
			set { this.line_miter_limit = value; }
		}

		
		
		public virtual double PaintText(double x, double y, string text, Font font, double size, Color color)
		{
			return font.GetTextAdvance (text) * size;
		}
		
		public void PaintText(double x, double y, double width, double height, string text, Font font, double size, ContentAlignment align, Color color)
		{
			double text_width  = font.GetTextAdvance (text) * size;
			double text_height = (font.Ascender - font.Descender) * size;
			
			switch (align)
			{
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					y = y - font.Descender * size;
					break;
				
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					y = y + (height - text_height) / 2 - font.Descender * size;
					break;
				
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopRight:
					y = y + height - text_height - font.Descender * size;
					break;
			}
			
			switch (align)
			{
				case ContentAlignment.BottomLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.TopLeft:
					break;
				
				case ContentAlignment.BottomCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
					x = x + (width - text_width) / 2;
					break;
				
				case ContentAlignment.BottomRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.TopRight:
					x = x + width - text_width;
					break;
			}
			
			this.PaintText (x, y, text, font, size, color);
		}
		
		public virtual void AddLine(double x1, double y1, double x2, double y2)
		{
		}
		
		public void AddLine(Point p1, Point p2)
		{
			this.AddLine (p1.X, p1.Y, p2.X, p2.Y);
		}
		
		public virtual void AddRectangle(double x, double y, double width, double height)
		{
		}
		
		public void AddRectangle(Point p, Size s)
		{
			this.AddRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void AddRectangle(Rectangle rect)
		{
			this.AddRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public virtual double AddText(double x, double y, string text, Font font, double size)
		{
			return font.GetTextAdvance (text) * size;
		}
		
		public virtual void AddFilledRectangle(double x, double y, double width, double height)
		{
		}
		
		public void AddFilledRectangle(Point p, Size s)
		{
			this.AddFilledRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void AddFilledRectangle(Rectangle rect)
		{
			this.AddFilledRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		
		
		public void AddText(double x, double y, double width, double height, string text, Font font, double size, ContentAlignment align)
		{
			double text_width  = font.GetTextAdvance (text) * size;
			double text_height = (font.Ascender - font.Descender) * size;
			
			switch (align)
			{
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					y = y - font.Descender * size;
					break;
				
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					y = y + (height - text_height) / 2 - font.Descender * size;
					break;
				
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopRight:
					y = y + height - text_height - font.Descender * size;
					break;
			}
			
			switch (align)
			{
				case ContentAlignment.BottomLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.TopLeft:
					break;
				
				case ContentAlignment.BottomCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
					x = x + (width - text_width) / 2;
					break;
				
				case ContentAlignment.BottomRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.TopRight:
					x = x + width - text_width;
					break;
			}
			
			this.AddText (x, y, text, font, size);
		}
		
		
		public virtual void Align(ref double x, ref double y)
		{
		}
		
		public virtual void Align(ref Drawing.Rectangle rect)
		{
		}
		
		
		public virtual Transform SaveTransform()
		{
			return null;
		}
		
		public virtual void ScaleTransform(double sx, double sy, double cx, double cy)
		{
		}
		
		public virtual void RotateTransform(double angle, double cx, double cy)
		{
		}
		
		
		public virtual void SetClippingRectangle(double x, double y, double width, double height)
		{
		}
		
		public void SetClippingRectangle(Point p, Size s)
		{
			this.SetClippingRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void SetClippingRectangle(Rectangle rect)
		{
			this.SetClippingRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public virtual Drawing.Rectangle SaveClippingRectangle()
		{
			return Drawing.Rectangle.Infinite;
		}
		
		public virtual void RestoreClippingRectangle(Drawing.Rectangle rect)
		{
		}
		
		public virtual void ResetClippingRectangle()
		{
		}
		
		public virtual bool TestForEmptyClippingRectangle()
		{
			return false;
		}
		
		
		
		
		public virtual Rasterizer			Rasterizer
		{
			get { return null; }
		}
		
		public virtual Transform			Transform
		{
			set { }
		}
		
		public virtual Pixmap				Pixmap
		{
			get { return null; }
		}
		
		public virtual Renderer.Solid		SolidRenderer
		{
			get { return null; }
		}
		
		public virtual Renderer.Image		ImageRenderer
		{
			get { return null; }
		}
		
		public virtual Renderer.Gradient	GradientRenderer
		{
			get { return null; }
		}
		
		
		public virtual bool SetPixmapSize(int width, int height)
		{
			return false;
		}
		
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}
		
		protected virtual void Dispose(bool disposing)
		{
		}
		
		
		
		protected double				line_width;
		protected JoinStyle				line_join;
		protected CapStyle				line_cap;
		protected double				line_miter_limit;
	}
}
