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
		
		
		public abstract void RenderSolid();
		public abstract void RenderSolid(Color color);
		public abstract void RenderImage();
		public abstract void RenderGradient();
		
		public void ResetLineStyle()
		{
			this.LineWidth      = 1.0;
			this.LineJoin       = JoinStyle.Miter;
			this.LineCap        = CapStyle.Square;
			this.LineMiterLimit = 4.0;
		}
		
		
		public double						LineWidth
		{
			get { return this.line_width; }
			set { this.line_width = value; }
		}
		
		public JoinStyle					LineJoin
		{
			get { return this.line_join; }
			set { this.line_join = value; }
		}
		
		public CapStyle						LineCap
		{
			get { return this.line_cap; }
			set { this.line_cap = value; }
		}
		
		public double						LineMiterLimit
		{
			get { return this.line_miter_limit; }
			set { this.line_miter_limit = value; }
		}

		public Rectangle					ClipBounds
		{
			get
			{
				return this.SaveClippingRectangle ();
			}
		}
		
		
		internal Transform					InternalTransform
		{
			get { return this.SaveTransform (); }
		}
		
		public abstract Rasterizer			Rasterizer		{ get; }
		public abstract Transform			Transform		{ set; }
		public abstract Pixmap				Pixmap			{ get; }
		public abstract Renderers.Solid		SolidRenderer	{ get; }
		public abstract Renderers.Image		ImageRenderer	{ get; }
		public abstract Renderers.Gradient	GradientRenderer{ get; }
		public abstract Renderers.Smooth	SmoothRenderer	{ get; }
		
		public abstract double PaintText(double x, double y, string text, Font font, double size, Color color);
		
		public abstract Graphics CreateAlphaMask();
		
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
		
		public void PaintImage(Image bitmap, Rectangle fill)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, 0, 0, bitmap.Width, bitmap.Height);
		}
		
		public void PaintImage(Image bitmap, double fill_x, double fill_y, double fill_width, double fill_height)
		{
			this.PaintImage (bitmap, fill_x, fill_y, fill_width, fill_height, 0, 0, bitmap.Width, bitmap.Height);
		}
		
		public void PaintImage(Image bitmap, Rectangle fill, Point image_origin)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, image_origin.X, image_origin.Y, fill.Width, fill.Height);
		}
		
		public void PaintImage(Image bitmap, Rectangle fill, Rectangle image_rect)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, image_rect.Left, image_rect.Bottom, image_rect.Width, image_rect.Height);
		}
		
		public void PaintImage(Image bitmap, double fill_x, double fill_y, double fill_width, double fill_height, double image_origin_x, double image_origin_y)
		{
			this.PaintImage (bitmap, fill_x, fill_y, fill_width, fill_height, image_origin_x, image_origin_y, fill_width, fill_height);
		}
		
		public void PaintImage(Image bitmap, double fill_x, double fill_y, double fill_width, double fill_height, double image_origin_x, double image_origin_y, double image_width, double image_height)
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
			
			Transform transform = new Transform ();
			
			Point vector_oo = new Point (0, 0); vector_oo = this.ApplyTransformDirect (vector_oo);
			Point vector_ox = new Point (1, 0); vector_ox = this.ApplyTransformDirect (vector_ox) - vector_oo;
			Point vector_oy = new Point (0, 1); vector_oy = this.ApplyTransformDirect (vector_oy) - vector_oo;
			
			double fix_x = System.Math.Sqrt (vector_ox.X * vector_ox.X + vector_ox.Y * vector_ox.Y);
			double fix_y = System.Math.Sqrt (vector_oy.X * vector_oy.X + vector_oy.Y * vector_oy.Y);
			
			double sx = (fill_width > 1)  ? (fill_width-1/fix_x)  / (ix2-ix1-1) : 1.0;
			double sy = (fill_height > 1) ? (fill_height-1/fix_x) / (iy2-iy1-1) : 1.0;
			
			transform.Translate (-ix1, -iy1);
			transform.Scale (sx, sy);
			transform.Translate (fill_x, fill_y);
			
			this.AddFilledRectangle (fill_x, fill_y, fill_width, fill_height);
			this.ImageRenderer.BitmapImage = bitmap;
			this.ImageRenderer.Transform = transform;
			this.RenderImage ();
		}
		
		public abstract Point ApplyTransformDirect(Point pt);
		public abstract Point ApplyTransformInverse(Point pt);
		
		
		public void AddLine(Point p1, Point p2)
		{
			this.AddLine (p1.X, p1.Y, p2.X, p2.Y);
		}
		
		public void AddRectangle(Point p, Size s)
		{
			this.AddRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void AddRectangle(Rectangle rect)
		{
			this.AddRectangle (rect.X, rect.Y, rect.Width, rect.Height);
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
		
		public double GetTransformZoom()
		{
			//	Détermine le zoom approximatif en vigueur dans la transformation actuelle.
			//	Calcule la longueur d'un segment diagonal [1 1] après transformation pour
			//	connaître ce zoom.
			
			Transform transform = this.SaveTransform ();
			
			double a = transform.XX + transform.XY;
			double b = transform.YX + transform.YY;
			
			return System.Math.Sqrt ((a*a + b*b) / 2);
		}
		
		
		public abstract void AddLine(double x1, double y1, double x2, double y2);
		public abstract void AddRectangle(double x, double y, double width, double height);
		public abstract double AddText(double x, double y, string text, Font font, double size);
		public abstract void AddFilledRectangle(double x, double y, double width, double height);
		public abstract void Align(ref double x, ref double y);
		public abstract void Align(ref Drawing.Rectangle rect);
		
		public void Align(ref Drawing.Point p)
		{
			double x = p.X;
			double y = p.Y;
			
			this.Align (ref x, ref y);
			
			p.X = x;
			p.Y = y;
		}
		
		
		public abstract Transform SaveTransform();
		public abstract void RestoreTransform(Transform transform);
		public abstract void ScaleTransform(double sx, double sy, double cx, double cy);
		public abstract void RotateTransform(double angle, double cx, double cy);
		public abstract void TranslateTransform(double ox, double oy);
		public abstract void SetClippingRectangle(double x, double y, double width, double height);
		public abstract void SetClippingRectangles(Drawing.Rectangle[] rectangles);
		public abstract Drawing.Rectangle SaveClippingRectangle();
		public abstract void RestoreClippingRectangle(Drawing.Rectangle rect);
		public abstract void ResetClippingRectangle();
		public abstract bool TestForEmptyClippingRectangle();
		
		public void SetClippingRectangle(Point p, Size s)
		{
			this.SetClippingRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void SetClippingRectangle(Rectangle rect)
		{
			this.SetClippingRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		
		public abstract bool SetPixmapSize(int width, int height);
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}
		#endregion
		
		protected abstract void Dispose(bool disposing);
		
		
		protected double					line_width;
		protected JoinStyle					line_join;
		protected CapStyle					line_cap;
		protected double					line_miter_limit;
	}
}
