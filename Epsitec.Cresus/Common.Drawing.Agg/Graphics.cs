//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Graphics encapsule le contexte graphique utilisé pour peindre.
	/// </summary>
	public class Graphics : System.IDisposable, IPaintPort
	{
		static Graphics()
		{
			Agg.Library.Initialize ();
		}
		
		
		public Graphics()
		{
			this.ResetLineStyle ();
			
			this.pixmap     = new Pixmap ();
			this.rasterizer = new Common.Drawing.Rasterizer ();
			this.transform  = new Transform ();
			
			this.color_modifier_stack = new System.Collections.Stack();
			
			this.solid_renderer    = new Common.Drawing.Renderers.Solid ();
			this.image_renderer    = new Common.Drawing.Renderers.Image ();
			this.gradient_renderer = new Common.Drawing.Renderers.Gradient ();
			this.smooth_renderer   = new Common.Drawing.Renderers.Smooth (this);
			
			this.image_renderer.TransformUpdating    += new Support.EventHandler (this.HandleTransformUpdating);
			this.gradient_renderer.TransformUpdating += new Support.EventHandler (this.HandleTransformUpdating);
			
			this.rasterizer.Gamma = 1.2;
		}

		public double							LineWidth
		{
			get { return this.line_width; }
			set { this.line_width = value; }
		}
		
		public JoinStyle						LineJoin
		{
			get { return this.line_join; }
			set { this.line_join = value; }
		}
		
		public CapStyle							LineCap
		{
			get { return this.line_cap; }
			set { this.line_cap = value; }
		}
		
		public double							LineMiterLimit
		{
			get { return this.line_miter_limit; }
			set { this.line_miter_limit = value; }
		}

		public Rectangle						ClipBounds
		{
			get
			{
				return this.SaveClippingRectangle ();
			}
		}

		public Point							ClipOffset
		{
			get
			{
				return new Point (this.clip_ox, this.clip_oy);
			}
			set
			{
				this.clip_ox = value.X;
				this.clip_oy = value.Y;
			}
		}
		
		public RichColor						RichColor
		{
			get
			{
				return RichColor.FromColor (this.original_color);
			}
			set
			{
				this.original_color = value.Basic;
				this.FinalColor = this.GetFinalColor (value.Basic);
			}
		}
		
		public Color							Color
		{
			get
			{
				return this.original_color;
			}
			set
			{
				this.original_color = value;
				this.FinalColor = this.GetFinalColor (value);
			}
		}

		public RichColor						FinalRichColor
		{
			get
			{
				return RichColor.FromColor(this.FinalColor);
			}
			set
			{
				this.FinalColor = value.Basic;
			}
		}
		
		public Color							FinalColor
		{
			get
			{
				return this.SolidRenderer.Color;
			}
			set
			{
				this.SolidRenderer.Color = value;
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
		
		public Margins							ImageCrop
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
		
		
		public FillMode							FillMode
		{
			get
			{
				return this.Rasterizer.FillMode;
			}
			set
			{
				this.Rasterizer.FillMode = value;
			}
		}

		public Drawing.AbstractRasterizer		Rasterizer
		{
			get
			{
				return this.rasterizer;
			}
		}
		
		public Drawing.Transform				Transform
		{
			get
			{
				return new Transform (this.transform);
			}
			set
			{
				this.transform.Reset (value);
				this.rasterizer.Transform = this.transform;
			}
		}
		
		public Drawing.Pixmap					Pixmap
		{
			get { return this.pixmap; }
		}
		
		public Renderers.Solid					SolidRenderer
		{
			get { return this.solid_renderer; }
		}
		
		public Renderers.Image					ImageRenderer
		{
			get { return this.image_renderer; }
		}
		
		public Renderers.Gradient				GradientRenderer
		{
			get { return this.gradient_renderer; }
		}
		
		public Renderers.Smooth					SmoothRenderer
		{
			get { return this.smooth_renderer; }
		}
		
		
		public void ReplaceRasterizer(AbstractRasterizer rasterizer)
		{
			this.rasterizer = rasterizer;
			this.rasterizer.Transform = this.transform;
		}
		
		
		public void RenderSolid()
		{
			this.rasterizer.Render (this.solid_renderer);
		}
		
		public void RenderSolid(Color color)
		{
			this.Color = color;
			this.rasterizer.Render (this.solid_renderer);
		}
		
		public void FinalRenderSolid(Color color)
		{
			this.FinalColor = color;
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
		
		
		public void AllocatePixmap()
		{
			this.pixmap.AllocatePixmap (new System.Drawing.Size (8, 8));
			
			this.solid_renderer.Pixmap    = this.pixmap;
			this.image_renderer.Pixmap    = this.pixmap;
			this.gradient_renderer.Pixmap = this.pixmap;
			this.smooth_renderer.Pixmap   = this.pixmap;
		}
		
		public Epsitec.Common.Drawing.Graphics CreateAlphaMask()
		{
			Graphics mask = new Graphics ();
			
			mask.SetPixmapSize (this.pixmap.Size.Width, this.pixmap.Size.Height);
			mask.SolidRenderer.ClearAlphaRgb (0, 0, 0, 0);
			mask.Transform = this.Transform;
			
			return mask;
		}
		
		
		public void PushColorModifier(ColorModifierCallback method)
		{
			this.color_modifier_stack.Push (method);
		}

		public ColorModifierCallback PopColorModifier()
		{
			return this.color_modifier_stack.Pop () as ColorModifierCallback;
		}

		
		public RichColor GetFinalColor(RichColor color)
		{
			foreach (ColorModifierCallback method in this.color_modifier_stack)
			{
				method (ref color);
			}
			
			return color;
		}
		
		public Color GetFinalColor(Color color)
		{
			if (this.color_modifier_stack.Count == 0)
			{
				return color;
			}
			
			RichColor rich = RichColor.FromColor (color);
			
			foreach (ColorModifierCallback method in this.color_modifier_stack)
			{
				method (ref rich);
			}
			
			return rich.Basic;
		}

		
		public void PaintOutline(Path path)
		{
			this.Rasterizer.AddOutline (path, this.line_width, this.line_cap, this.line_join, this.line_miter_limit);
			this.RenderSolid ();
		}
		
		public void PaintSurface(Path path)
		{
			this.Rasterizer.AddSurface (path);
			this.RenderSolid ();
		}
		
		
		public void PaintGlyphs(Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
		{
			for (int i = 0; i < glyphs.Length; i++)
			{
				this.Rasterizer.AddGlyph (font, glyphs[i], x[i], y[i], size, sx == null ? 1 : sx[i], sy == null ? 1 : sy[i]);
			}
		}
		
		
		public void   PaintText(double x, double y, double width, double height, string text, Font font, double size, ContentAlignment align)
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
			
			this.PaintText (x, y, text, font, size);
		}
		
		public double PaintText(double x, double y, string text, Font font, double size)
		{
			if (this.transform.OnlyTranslate && ! font.IsSynthetic)
			{
				x += this.transform.TX;
				y += this.transform.TY;
				
				return font.PaintPixelCache (this.pixmap, text, size, x, y, this.SolidRenderer.Color);
			}
			else
			{
				double advance = this.AddText (x, y, text, font, size);
				this.RenderSolid ();
				return advance;
			}
		}
		
		public double PaintText(double x, double y, string text, Font font, double size, FontClassInfo[] infos)
		{
			//	TODO: déplacer ce code dans la librairie AGG; faire en sorte que ça marche aussi
			//	si ClassId != ClassId.Space...
			
			for (int i = 0; i < infos.Length; i++)
			{
				if ((infos[i].Scale != 1.00) &&
					(infos[i].GlyphClass == GlyphClass.Space))
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
			
#if true
			Point vector_oo = new Point (0, 0); vector_oo = this.ApplyTransformDirect (vector_oo);
			Point vector_ox = new Point (1, 0); vector_ox = this.ApplyTransformDirect (vector_ox) - vector_oo;
			Point vector_oy = new Point (0, 1); vector_oy = this.ApplyTransformDirect (vector_oy) - vector_oo;
			
			double fix_x = System.Math.Sqrt (vector_ox.X * vector_ox.X + vector_ox.Y * vector_ox.Y);
			double fix_y = System.Math.Sqrt (vector_oy.X * vector_oy.X + vector_oy.Y * vector_oy.Y);
			
			double sx = (ix2-ix1 <= 1) ? (Graphics.AlmostInfinite) : ((fill_width  > 1/fix_x) ? (fill_width-1/fix_x)  / (ix2-ix1-1) : 1.0);
			double sy = (iy2-iy1 <= 1) ? (Graphics.AlmostInfinite) : ((fill_height > 1/fix_y) ? (fill_height-1/fix_y) / (iy2-iy1-1) : 1.0);
#else
			double sx = (ix2-ix1 < 1) ? (Graphics.AlmostInfinite) : ((fill_width > 1)  ? (fill_width)  / (ix2-ix1) : 1.0);
			double sy = (iy2-iy1 < 1) ? (Graphics.AlmostInfinite) : ((fill_height > 1) ? (fill_height) / (iy2-iy1) : 1.0);
#endif
			
			Drawing.Bitmap bmi = bitmap.BitmapImage;
			
			sx *= bitmap.Width  / bmi.PixelWidth;
			sy *= bitmap.Height / bmi.PixelHeight;
			
			transform.Translate (-ix1, -iy1);
			transform.Scale (sx, sy);
			transform.Translate (fill_x, fill_y);
			
			this.AddFilledRectangle (fill_x, fill_y, fill_width, fill_height);
			this.ImageRenderer.BitmapImage = bitmap;
			this.ImageRenderer.Transform = transform;

			if (this.image_filter)
			{
				this.ImageRenderer.SelectBilinearFilter ();
			}
			else
			{
				this.ImageRenderer.SelectNoFilter ();
			}
			
			this.RenderImage ();
			this.ImageRenderer.BitmapImage = null;
		}
		
		
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
		
		public void AddCircle(Point p, double r)
		{
			this.AddCircle (p.X, p.Y, r, r);
		}
		
		public void AddCircle(Point p, double rx, double ry)
		{
			this.AddCircle (p.X, p.Y, rx, ry);
		}
		
		public void AddCircle(double x, double y, double r)
		{
			this.AddCircle (x, y, r, r);
		}
		
		public void AddFilledRectangle(Point p, Size s)
		{
			this.AddFilledRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void AddFilledRectangle(Rectangle rect)
		{
			this.AddFilledRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public void AddFilledCircle(Point p, double r)
		{
			this.AddFilledCircle (p.X, p.Y, r, r);
		}
		
		public void AddFilledCircle(Point p, double rx, double ry)
		{
			this.AddFilledCircle (p.X, p.Y, rx, ry);
		}
		
		public void AddFilledCircle(double x, double y, double r)
		{
			this.AddFilledCircle (x, y, r, r);
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
		
		
		public void   AddLine(double x1, double y1, double x2, double y2)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x1, y1);
				path.LineTo (x2, y2);
				
				this.rasterizer.AddOutline (path, this.line_width, this.line_cap, this.line_join, this.line_miter_limit);
			}
		}
		
		public void   AddRectangle(double x, double y, double width, double height)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x, y);
				path.LineTo (x+width, y);
				path.LineTo (x+width, y+height);
				path.LineTo (x, y+height);
				path.Close ();
				
				this.rasterizer.AddOutline (path, this.line_width, this.line_cap, this.line_join, this.line_miter_limit);
			}
		}
		
		public void   AddCircle(double cx, double cy, double rx, double ry)
		{
			using (Path path = Path.CreateCircle (cx, cy, rx, ry))
			{
				this.rasterizer.AddOutline (path, this.line_width, this.line_cap, this.line_join, this.line_miter_limit);
			}
		}
		
		public double AddText(double x, double y, string text, Font font, double size)
		{
			return this.rasterizer.AddText (font, text, x, y, size);
		}
		
		public void   AddFilledRectangle(double x, double y, double width, double height)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x, y);
				path.LineTo (x+width, y);
				path.LineTo (x+width, y+height);
				path.LineTo (x, y+height);
				path.Close ();
				
				this.rasterizer.AddSurface (path);
			}
		}
		
		public void   AddFilledCircle(double cx, double cy, double rx, double ry)
		{
			using (Path path = Path.CreateCircle (cx, cy, rx, ry))
			{
				this.rasterizer.AddSurface (path);
			}
		}
		
		public void Align(ref double x, ref double y)
		{
			this.transform.TransformDirect (ref x, ref y);
			x = System.Math.Floor (x + 0.5);
			y = System.Math.Floor (y + 0.5);
			this.transform.TransformInverse (ref x, ref y);
		}
		
		public void Align(ref Drawing.Rectangle rect)
		{
			double x1 = rect.Left;
			double y1 = rect.Bottom;
			double x2 = rect.Right;
			double y2 = rect.Top;
			
			this.transform.TransformDirect (ref x1, ref y1);
			this.transform.TransformDirect (ref x2, ref y2);
			
			x1 = System.Math.Floor (x1 + 0.5);
			y1 = System.Math.Floor (y1 + 0.5);
			x2 = System.Math.Floor (x2 + 0.5);
			y2 = System.Math.Floor (y2 + 0.5);
			
			this.transform.TransformInverse (ref x1, ref y1);
			this.transform.TransformInverse (ref x2, ref y2);
			
			rect = new Rectangle (x1, y1, x2-x1, y2-y1);
		}
		
		
		public void Align(ref Drawing.Point p)
		{
			double x = p.X;
			double y = p.Y;
			
			this.Align (ref x, ref y);
			
			p.X = x;
			p.Y = y;
		}
		
		
		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			this.transform.MultiplyByPostfix (Drawing.Transform.FromScale (sx, sy, cx, cy));
			this.UpdateTransform ();
		}
		
		public void RotateTransformDeg(double angle, double cx, double cy)
		{
			this.transform.MultiplyByPostfix (Drawing.Transform.FromRotationDeg (angle, cx, cy));
			this.UpdateTransform ();
		}
		
		public void RotateTransformRad(double angle, double cx, double cy)
		{
			this.transform.MultiplyByPostfix (Drawing.Transform.FromRotationRad (angle, cx, cy));
			this.UpdateTransform ();
		}
		
		public void TranslateTransform(double ox, double oy)
		{
			this.transform.MultiplyByPostfix (Drawing.Transform.FromTranslation (ox, oy));
			this.UpdateTransform ();
		}
		
		public void MergeTransform(Transform transform)
		{
			this.transform.MultiplyByPostfix (transform);
			this.UpdateTransform ();
		}

		public Point ApplyTransformDirect(Point pt)
		{
			return this.transform.TransformDirect (pt);
		}
		
		public Point ApplyTransformInverse(Point pt)
		{
			return this.transform.TransformInverse (pt);
		}
		
		protected void UpdateTransform()
		{
			this.rasterizer.Transform = this.transform;
			
			//	Lorsque la matrice de transformation change, il faut aussi mettre à jour les
			//	transformations des renderers qui en ont...
			
			Transform t_image    = this.image_renderer.Transform;
			Transform t_gradient = this.gradient_renderer.Transform;
			
			this.image_renderer.Transform    = t_image;
			this.gradient_renderer.Transform = t_gradient;
		}
		
		protected void HandleTransformUpdating(object sender)
		{
			Renderers.ITransformProvider provider = sender as Renderers.ITransformProvider;
			
			if (provider != null)
			{
				provider.InternalTransform.MultiplyBy (this.transform);
			}
		}
		
		
		public void SetClippingRectangle(double x, double y, double width, double height)
		{
			double x1 = x + this.clip_ox;
			double y1 = y + this.clip_oy;
			double x2 = x + width + this.clip_ox;
			double y2 = y + height + this.clip_oy;
			
			if (this.has_clip_rect)
			{
				x1 = System.Math.Max (x1, this.clip_x1);
				x2 = System.Math.Min (x2, this.clip_x2);
				y1 = System.Math.Max (y1, this.clip_y1);
				y2 = System.Math.Min (y2, this.clip_y2);
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
			this.pixmap.EmptyClipping ();
			this.pixmap.AddClipBox (x1, y1, x2, y2);
		}
		
		public void SetClippingRectangles(Drawing.Rectangle[] rectangles)
		{
			if (rectangles.Length == 0)
			{
				return;
			}
			
			Drawing.Rectangle clip = this.SaveClippingRectangle ();
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			
			this.pixmap.EmptyClipping ();
			
			for (int i = 0; i < rectangles.Length; i++)
			{
				Drawing.Rectangle rect = Drawing.Rectangle.Intersection (rectangles[i], clip);
				
				if (!rect.IsEmpty)
				{
					this.pixmap.AddClipBox (rect.Left + this.clip_ox, rect.Bottom + this.clip_oy, rect.Right + this.clip_ox, rect.Top + this.clip_oy);
					bbox = Drawing.Rectangle.Union (bbox, rect);
				}
			}
			
			this.has_clip_rect = true;
			this.rasterizer.SetClipBox (bbox.Left, bbox.Bottom, bbox.Right, bbox.Top);
		}
		
		public void SetClippingRectangles(System.Collections.ICollection rectangles)
		{
			if ((rectangles == null) ||
				(rectangles.Count == 0))
			{
				return;
			}
			
			Drawing.Rectangle clip = this.SaveClippingRectangle ();
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			
			this.pixmap.EmptyClipping ();
			
			foreach (Drawing.Rectangle rectangle in rectangles)
			{
				Drawing.Rectangle rect = Drawing.Rectangle.Intersection (rectangle, clip);
				
				if (!rect.IsEmpty)
				{
					this.pixmap.AddClipBox (rect.Left + this.clip_ox, rect.Bottom + this.clip_oy, rect.Right + this.clip_ox, rect.Top + this.clip_oy);
					bbox = Drawing.Rectangle.Union (bbox, rect);
				}
			}
			
			this.has_clip_rect = true;
			this.rasterizer.SetClipBox (bbox.Left, bbox.Bottom, bbox.Right, bbox.Top);
		}
		
		
		public Drawing.Rectangle SaveClippingRectangle()
		{
			if (this.has_clip_rect)
			{
				return new Drawing.Rectangle (this.clip_x1, this.clip_y1, this.clip_x2-this.clip_x1, this.clip_y2-this.clip_y1);
			}
			
			return Drawing.Rectangle.MaxValue;
		}
		
		public void RestoreClippingRectangle(Drawing.Rectangle rect)
		{
			if (rect == Drawing.Rectangle.MaxValue)
			{
				this.ResetClippingRectangle ();
			}
			else
			{
				this.clip_x1 = rect.Left;
				this.clip_y1 = rect.Bottom;
				this.clip_x2 = rect.Right;
				this.clip_y2 = rect.Top;
				
				this.has_clip_rect = true;
				
				this.rasterizer.SetClipBox (this.clip_x1, this.clip_y1, this.clip_x2, this.clip_y2);
				this.pixmap.EmptyClipping ();
				this.pixmap.AddClipBox (this.clip_x1, this.clip_y1, this.clip_x2, this.clip_y2);
			}
		}
		
		public void ResetClippingRectangle()
		{
			this.rasterizer.ResetClipBox ();
			this.pixmap.InfiniteClipping ();
			this.has_clip_rect = false;
		}
		
		
		public bool TestForEmptyClippingRectangle()
		{
			if (this.has_clip_rect)
			{
				if ((this.clip_x1 >= this.clip_x2) ||
					(this.clip_y1 >= this.clip_y2))
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		public void SetClippingRectangle(Point p, Size s)
		{
			this.SetClippingRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void SetClippingRectangle(Rectangle rect)
		{
			this.SetClippingRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		
		public bool SetPixmapSize(int width, int height)
		{
			if ((this.pixmap.Size.Width == width) &&
				(this.pixmap.Size.Height == height))
			{
				return false;
			}
			
			this.pixmap.Size = new System.Drawing.Size (width, height);
			
			this.solid_renderer.Pixmap    = null;
			this.image_renderer.Pixmap	  = null;
			this.gradient_renderer.Pixmap = null;
			this.smooth_renderer.Pixmap   = null;
			
			this.solid_renderer.Pixmap    = this.pixmap;
			this.image_renderer.Pixmap    = this.pixmap;
			this.gradient_renderer.Pixmap = this.pixmap;
			this.smooth_renderer.Pixmap   = this.pixmap;
			
			return true;
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}
		#endregion
		
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
					this.solid_renderer.Pixmap = null;
					this.solid_renderer.Dispose ();
				}
				if (this.image_renderer != null)
				{
					this.image_renderer.Pixmap = null;
					this.image_renderer.Dispose ();
				}
				if (this.gradient_renderer != null)
				{
					this.gradient_renderer.Pixmap = null;
					this.gradient_renderer.Dispose ();
				}
				if (this.smooth_renderer != null)
				{
					this.smooth_renderer.Pixmap = null;
					this.smooth_renderer.Dispose ();
				}
				
				this.pixmap            = null;
				this.rasterizer        = null;
				this.solid_renderer    = null;
				this.image_renderer    = null;
				this.gradient_renderer = null;
				this.smooth_renderer   = null;
			}
		}

		
		private const double				AlmostInfinite = 1000000000.0;
		private bool						image_filter = true;
		private Margins						image_crop;
		
		private double						line_width;
		private JoinStyle					line_join;
		private CapStyle					line_cap;
		private double						line_miter_limit;
		
		private Pixmap						pixmap;
		private AbstractRasterizer			rasterizer;
		private Transform					transform;
		
		private Renderers.Solid				solid_renderer;
		private Renderers.Image				image_renderer;
		private Renderers.Gradient			gradient_renderer;
		private Renderers.Smooth			smooth_renderer;
		
		private double						clip_x1, clip_y1, clip_x2, clip_y2;
		private double						clip_ox, clip_oy;
		private bool						has_clip_rect;

		private Color						original_color;
		private System.Collections.Stack	color_modifier_stack;
	}
}
