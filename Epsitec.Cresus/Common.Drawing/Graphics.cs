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
	public class Graphics : System.IDisposable
	{
		public Graphics()
		{
			this.pixmap     = new Pixmap ();
			this.rasterizer = new Rasterizer ();
			this.transform  = new Transform ();
			
			this.solid_renderer    = new Common.Drawing.Renderer.Solid ();
			this.image_renderer    = new Common.Drawing.Renderer.Image ();
			this.gradient_renderer = new Common.Drawing.Renderer.Gradient ();
			
			this.image_renderer.TransformUpdating    += new System.EventHandler (HandleTransformUpdating);
			this.gradient_renderer.TransformUpdating += new System.EventHandler (HandleTransformUpdating);
			
			this.ResetLineStyle ();
			
			this.rasterizer.Gamma = 1.2;
		}

		~ Graphics()
		{
			this.Dispose (false);
		}
		
		
		public void RenderSolid()
		{
			this.rasterizer.Render (this.solid_renderer);
		}
		
		public void RenderSolid(Color color)
		{
			this.solid_renderer.Color = color;
			this.rasterizer.Render (this.solid_renderer);
		}
		
		public void RenderImage()
		{
			this.rasterizer.Render (this.image_renderer);
		}
		
		public void RenderGradient()
		{
			this.rasterizer.Render (this.gradient_renderer);
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

		
		
		public void AddLine(double x1, double y1, double x2, double y2)
		{
			Path path = new Path ();
			path.MoveTo (x1, y1);
			path.LineTo (x2, y2);
			
			this.rasterizer.AddOutline (path, this.line_width, this.line_cap, this.line_join, this.line_miter_limit);
		}
		
		public void AddLine(Point p1, Point p2)
		{
			this.AddLine (p1.X, p1.Y, p2.X, p2.Y);
		}
		
		public void AddRectangle(double x, double y, double width, double height)
		{
			Path path = new Path ();
			path.MoveTo (x, y);
			path.LineTo (x+width, y);
			path.LineTo (x+width, y+height);
			path.LineTo (x, y+height);
			path.Close ();
			
			this.rasterizer.AddOutline (path, this.line_width, this.line_cap, this.line_join, this.line_miter_limit);
		}
		
		public void AddRectangle(Point p, Size s)
		{
			this.AddRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void AddRectangle(Rectangle rect)
		{
			this.AddRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public void AddText(double x, double y, string text, Font font, double size)
		{
			foreach (char c in text)
			{
				int    glyph = font.GetGlyphIndex (c);
				double width = font.GetGlyphAdvance (glyph);
				
				this.rasterizer.AddGlyph (font, glyph, x, y, size);
				
				x += width * size;
			}
		}
		
		public void AddFilledRectangle(double x, double y, double width, double height)
		{
			Path path = new Path ();
			path.MoveTo (x, y);
			path.LineTo (x+width, y);
			path.LineTo (x+width, y+height);
			path.LineTo (x, y+height);
			path.Close ();
			
			this.rasterizer.AddSurface (path);
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
		
		
		public Transform SaveTransform()
		{
			return new Transform (this.transform);
		}
		
		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			this.transform.MultiplyByPostfix (Drawing.Transform.FromScale (sx, sy, cx, cy));
			this.UpdateTransform ();
		}
		
		public void RotateTransform(double angle, double cx, double cy)
		{
			this.transform.MultiplyByPostfix (Drawing.Transform.FromRotation (angle, cx, cy));
			this.UpdateTransform ();
		}
		
		
		public void SetClippingRectangle(double x, double y, double width, double height)
		{
			double x1 = x;
			double y1 = y;
			double x2 = x + width;
			double y2 = y + height;
			
			if (this.has_clip_rect)
			{
				x1 = System.Math.Max (x1, this.clip_x1);
				x2 = System.Math.Min (x2, this.clip_x2);
				y1 = System.Math.Max (y1, this.clip_y1);
				y2 = System.Math.Max (y2, this.clip_y2);
			}
			else
			{
				this.has_clip_rect = true;
			}
			
			this.clip_x1 = x1;
			this.clip_y1 = y1;
			this.clip_x2 = x2;
			this.clip_y2 = y2;
			
			this.rasterizer.SetClipBox (x1, y1, x2, y2);
		}
		
		public void SetClippingRectangle(Point p, Size s)
		{
			this.SetClippingRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void SetClippingRectangle(Rectangle rect)
		{
			this.SetClippingRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public void ResetClippingRectangle()
		{
			this.rasterizer.ResetClipBox ();
			this.has_clip_rect = false;
		}
		
		
		protected virtual void UpdateTransform()
		{
			this.rasterizer.Transform = this.transform;
			
			//	Lorsque la matrice de transformation change, il faut aussi mettre à jour les
			//	transformations des renderers qui en ont...
			
			Transform t_image    = this.image_renderer.Transform;
			Transform t_gradient = this.gradient_renderer.Transform;
			
			this.image_renderer.Transform    = t_image;
			this.gradient_renderer.Transform = t_gradient;
		}
		
		
		
		public Rasterizer				Rasterizer
		{
			get { return this.rasterizer; }
		}
		
		public Transform				Transform
		{
			set
			{
				this.transform.Reset (value);
				this.rasterizer.Transform = this.transform;
			}
		}
		
		public Pixmap					Pixmap
		{
			get { return this.pixmap; }
		}
		
		public Renderer.Solid			SolidRenderer
		{
			get { return this.solid_renderer; }
		}
		
		public Renderer.Image			ImageRenderer
		{
			get { return this.image_renderer; }
		}
		
		public Renderer.Gradient		GradientRenderer
		{
			get { return this.gradient_renderer; }
		}
		
		
		public void SetPixmapSize(int width, int height)
		{
			this.pixmap.Size = new System.Drawing.Size (width, height);
			
			this.solid_renderer.Pixmap    = null;
			this.image_renderer.Pixmap	  = null;
			this.gradient_renderer.Pixmap = null;
			
			this.solid_renderer.Pixmap    = this.pixmap;
			this.image_renderer.Pixmap    = this.pixmap;
			this.gradient_renderer.Pixmap = this.pixmap;
		}
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}

		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.pixmap != null)
				{
					this.pixmap.Dispose ();
				}
				if (this.rasterizer != null)
				{
					this.rasterizer.Dispose ();
				}
				if (this.solid_renderer != null)
				{
					this.solid_renderer.Dispose ();
				}
				if (this.gradient_renderer != null)
				{
					this.gradient_renderer.Dispose ();
				}
				
				this.pixmap            = null;
				this.rasterizer        = null;
				this.solid_renderer    = null;
				this.gradient_renderer = null;
			}
		}
		
		
		protected virtual void HandleTransformUpdating(object sender, System.EventArgs e)
		{
			Renderer.ITransformProvider provider = sender as Renderer.ITransformProvider;
			
			if (provider != null)
			{
				provider.InternalTransform.MultiplyBy (this.transform);
			}
		}
		
		
		
		private Pixmap					pixmap;
		private Rasterizer				rasterizer;
		private Transform				transform;
		
		private Renderer.Solid			solid_renderer;
		private Renderer.Image			image_renderer;
		private Renderer.Gradient		gradient_renderer;
		
		private double					line_width;
		private JoinStyle				line_join;
		private CapStyle				line_cap;
		private double					line_miter_limit;
		
		private double					clip_x1, clip_y1, clip_x2, clip_y2;
		private bool					has_clip_rect;
	}
}
