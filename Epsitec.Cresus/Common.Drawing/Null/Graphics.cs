namespace Epsitec.Common.Drawing.Null
{
	using Epsitec.Common.Drawing;
	
	/// <summary>
	/// Implémentation de la classe Graphics, sans effet (utilisée pour des mesures
	/// de performance seulement).
	/// </summary>
	public class Graphics : Epsitec.Common.Drawing.Graphics
	{
		public Graphics()
		{
			this.solid_renderer = new Common.Drawing.Null.SolidRenderer ();
			this.rasterizer     = new Common.Drawing.Null.Rasterizer ();
		}
		
		
		public override void RenderSolid()
		{
		}
		
		public override void RenderSolid(Color color)
		{
		}
		
		public override void RenderImage()
		{
		}
		
		public override void RenderGradient()
		{
		}
		
		
		public override Epsitec.Common.Drawing.Graphics CreateAlphaMask()
		{
			return null;
		}
		
		
		public override double PaintText(double x, double y, string text, Font font, double size)
		{
			return font.GetTextAdvance (text) * size;
		}
		
		public override double PaintText(double x, double y, string text, Font font, double size, Font.ClassInfo[] infos)
		{
			return font.GetTextAdvance (text) * size;
		}
		
		public override void   AddLine(double x1, double y1, double x2, double y2)
		{
		}
		
		public override void   AddRectangle(double x, double y, double width, double height)
		{
		}
		
		public override void   AddCircle(double cx, double cy, double rx, double ry)
		{
		}

		
		public override double AddText(double x, double y, string text, Font font, double size)
		{
			return font.GetTextAdvance (text) * size;
		}
		
		public override void   AddFilledRectangle(double x, double y, double width, double height)
		{
		}
		
		public override void   AddFilledCircle(double cx, double cy, double rx, double ry)
		{
		}
		
		public override void Align(ref double x, ref double y)
		{
		}
		
		public override void Align(ref Drawing.Rectangle rect)
		{
		}
		
		
		public override Transform SaveTransform()
		{
			return null;
		}
		
		public override void RestoreTransform(Transform transform)
		{
			this.Transform = transform;
		}
		
		public override void ScaleTransform(double sx, double sy, double cx, double cy)
		{
		}
		
		public override void RotateTransformDeg(double angle, double cx, double cy)
		{
		}
		
		public override void RotateTransformRad(double angle, double cx, double cy)
		{
		}
		
		public override void TranslateTransform(double ox, double oy)
		{
		}
		
		public override void MergeTransform(Transform transform)
		{
		}

		public override Point ApplyTransformDirect(Point pt)
		{
			return pt;
		}
		
		public override Point ApplyTransformInverse(Point pt)
		{
			return pt;
		}
		
		public override void SetClippingRectangle(double x, double y, double width, double height)
		{
		}
		
		public override void SetClippingRectangles(Drawing.Rectangle[] rectangles)
		{
		}
		
		public override Drawing.Rectangle SaveClippingRectangle()
		{
			return Drawing.Rectangle.Infinite;
		}
		
		public override void RestoreClippingRectangle(Drawing.Rectangle rect)
		{
		}
		
		public override void ResetClippingRectangle()
		{
		}
		
		public override bool TestForEmptyClippingRectangle()
		{
			return false;
		}
		
		
		public override Drawing.Rasterizer	Rasterizer
		{
			get { return this.rasterizer; }
		}
		
		public override Transform			Transform
		{
			set { }
		}
		
		public override Pixmap				Pixmap
		{
			get { return null; }
		}
		
		public override Renderers.Image		ImageRenderer
		{
			get { return null; }
		}
		
		public override Renderers.Gradient	GradientRenderer
		{
			get { return null; }
		}
		
		public override Renderers.Solid		SolidRenderer
		{
			get { return this.solid_renderer; }
		}
		
		public override Renderers.Smooth	SmoothRenderer
		{
			get { return null; }
		}
		
		public override bool SetPixmapSize(int width, int height)
		{
			return false;
		}
		
		
		protected override void Dispose(bool disposing)
		{
		}
		
		
		private Renderers.Solid			solid_renderer;
		private Rasterizer				rasterizer;
	}
}
