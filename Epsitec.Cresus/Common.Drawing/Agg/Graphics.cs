namespace Epsitec.Common.Drawing.Agg
{
	/// <summary>
	/// Implémentation de la classe Graphics basée sur AGG.
	/// </summary>
	public class Graphics : Epsitec.Common.Drawing.Graphics
	{
		static Graphics()
		{
			Font.Initialise ();
		}
		
		public Graphics()
		{
			this.pixmap     = new Pixmap ();
			this.rasterizer = new Common.Drawing.Agg.Rasterizer ();
			this.transform  = new Transform ();
			
			this.solid_renderer    = new Common.Drawing.Agg.SolidRenderer ();
			this.image_renderer    = new Common.Drawing.Renderers.Image ();
			this.gradient_renderer = new Common.Drawing.Renderers.Gradient ();
			
			this.image_renderer.TransformUpdating    += new System.EventHandler (HandleTransformUpdating);
			this.gradient_renderer.TransformUpdating += new System.EventHandler (HandleTransformUpdating);
			
			this.rasterizer.Gamma = 1.2;
		}

		public override void RenderSolid()
		{
			this.rasterizer.Render (this.solid_renderer);
		}
		
		public override void RenderSolid(Color color)
		{
			this.solid_renderer.Color = color;
			this.rasterizer.Render (this.solid_renderer);
		}
		
		public override void RenderImage()
		{
			this.rasterizer.Render (this.image_renderer);
		}
		
		public override void RenderGradient()
		{
			this.rasterizer.Render (this.gradient_renderer);
		}
		
		

		public override double PaintText(double x, double y, string text, Font font, double size, Color color)
		{
			if (this.transform.OnlyTranslate && ! font.IsSynthetic)
			{
				x += this.transform.TX;
				y += this.transform.TY;
				
				return font.PaintPixelCache (this.pixmap, text, size, x, y, color);
			}
			else
			{
				double advance = this.AddText (x, y, text, font, size);
				this.RenderSolid (color);
				return advance;
			}
		}
		
		
		
		public override void AddLine(double x1, double y1, double x2, double y2)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x1, y1);
				path.LineTo (x2, y2);
				
				this.rasterizer.AddOutline (path, this.line_width, this.line_cap, this.line_join, this.line_miter_limit);
			}
		}
		
		public override void AddRectangle(double x, double y, double width, double height)
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
		
		public override double AddText(double x, double y, string text, Font font, double size)
		{
			return this.rasterizer.AddText (font, text, x, y, size);
		}
		
		public override void AddFilledRectangle(double x, double y, double width, double height)
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
		
		
		
		public override void Align(ref double x, ref double y)
		{
			this.transform.TransformDirect (ref x, ref y);
			x = System.Math.Floor (x + 0.5);
			y = System.Math.Floor (y + 0.5);
			this.transform.TransformInverse (ref x, ref y);
		}
		
		public override void Align(ref Drawing.Rectangle rect)
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
		
		
		public override Transform SaveTransform()
		{
			return new Transform (this.transform);
		}
		
		public override void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			this.transform.MultiplyByPostfix (Drawing.Transform.FromScale (sx, sy, cx, cy));
			this.UpdateTransform ();
		}
		
		public override void RotateTransform(double angle, double cx, double cy)
		{
			this.transform.MultiplyByPostfix (Drawing.Transform.FromRotation (angle, cx, cy));
			this.UpdateTransform ();
		}
		
		
		public override void SetClippingRectangle(double x, double y, double width, double height)
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
		
		public override void SetClippingRectangles(Drawing.Rectangle[] rectangles)
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
					this.pixmap.AddClipBox (rect.Left, rect.Bottom, rect.Right, rect.Top);
					bbox = Drawing.Rectangle.Union (bbox, rect);
				}
			}
			
			this.has_clip_rect = true;
			this.rasterizer.SetClipBox (bbox.Left, bbox.Bottom, bbox.Right, bbox.Top);
		}
		
		public override Drawing.Rectangle SaveClippingRectangle()
		{
			if (this.has_clip_rect)
			{
				return new Drawing.Rectangle (this.clip_x1, this.clip_y1, this.clip_x2-this.clip_x1, this.clip_y2-this.clip_y1);
			}
			
			return Drawing.Rectangle.Infinite;
		}
		
		public override void RestoreClippingRectangle(Drawing.Rectangle rect)
		{
			if (rect == Drawing.Rectangle.Infinite)
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
		
		public override void ResetClippingRectangle()
		{
			this.rasterizer.ResetClipBox ();
			this.pixmap.InfiniteClipping ();
			this.has_clip_rect = false;
		}
		
		public override bool TestForEmptyClippingRectangle()
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
		
		protected void HandleTransformUpdating(object sender, System.EventArgs e)
		{
			Renderers.ITransformProvider provider = sender as Renderers.ITransformProvider;
			
			if (provider != null)
			{
				provider.InternalTransform.MultiplyBy (this.transform);
			}
		}
		
		
		public override Epsitec.Common.Drawing.Rasterizer	Rasterizer
		{
			get { return this.rasterizer; }
		}
		
		public override Epsitec.Common.Drawing.Transform	Transform
		{
			set
			{
				this.transform.Reset (value);
				this.rasterizer.Transform = this.transform;
			}
		}
		
		public override Epsitec.Common.Drawing.Pixmap		Pixmap
		{
			get { return this.pixmap; }
		}
		
		public override Renderers.Solid						SolidRenderer
		{
			get { return this.solid_renderer; }
		}
		
		public override Renderers.Image						ImageRenderer
		{
			get { return this.image_renderer; }
		}
		
		public override Renderers.Gradient					GradientRenderer
		{
			get { return this.gradient_renderer; }
		}
		
		
		public override bool SetPixmapSize(int width, int height)
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
			
			this.solid_renderer.Pixmap    = this.pixmap;
			this.image_renderer.Pixmap    = this.pixmap;
			this.gradient_renderer.Pixmap = this.pixmap;
			
			return true;
		}
		
		
		protected override void Dispose(bool disposing)
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
		
		
		
		
		private Pixmap					pixmap;
		private Rasterizer				rasterizer;
		private Transform				transform;
		
		private Renderers.Solid			solid_renderer;
		private Renderers.Image			image_renderer;
		private Renderers.Gradient		gradient_renderer;
		
		private double					clip_x1, clip_y1, clip_x2, clip_y2;
		private bool					has_clip_rect;
	}
}
