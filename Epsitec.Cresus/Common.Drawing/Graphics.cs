namespace Epsitec.Common.Drawing
{
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
			this.gradient_renderer = new Common.Drawing.Renderer.Gradient ();
		}

		~ Graphics()
		{
			this.Dispose (false);
		}
		
		
		public void RenderSolid()
		{
			this.rasterizer.Render (this.solid_renderer);
		}
		
		public void RenderGradient()
		{
			this.rasterizer.Render (this.gradient_renderer);
		}
		
		
		public Transform SaveTransform()
		{
			return new Transform (this.transform);
		}
		
		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			this.transform.MultiplyByPostfix (Drawing.Transform.FromScale (sx, sy, cx, cy));
			this.rasterizer.Transform = this.transform;
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
		
		public Renderer.Solid			Solid
		{
			get { return this.solid_renderer; }
		}
		
		public Renderer.Gradient		Gradient
		{
			get { return this.gradient_renderer; }
		}
		
		
		public void SetPixmapSize(int width, int height)
		{
			this.pixmap.Size = new System.Drawing.Size (width, height);
			
			this.solid_renderer.Pixmap    = null;
			this.gradient_renderer.Pixmap = null;
			
			this.solid_renderer.Pixmap    = this.pixmap;
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
		
		
		
		Pixmap						pixmap;
		Rasterizer					rasterizer;
		Transform					transform;
		
		Renderer.Solid				solid_renderer;
		Renderer.Gradient			gradient_renderer;
	}
}
