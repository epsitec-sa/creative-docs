namespace Epsitec.Common.Drawing.Renderer
{
	public class Solid : IRenderer, System.IDisposable
	{
		public Solid()
		{
		}
		
		~Solid()
		{
			this.Dispose (false);
		}
		
		
		public Pixmap					Pixmap
		{
			get
			{
				return this.pixmap;
			}
			set
			{
				if (this.pixmap != value)
				{
					if (value == null)
					{
						this.Detach ();
					}
					else
					{
						this.Attach (value);
					}
				}
			}
		}
		
		public System.IntPtr			Handle
		{
			get { return this.agg_ren; }
		}
		
		public Color					Color
		{
			set { this.SetColor (value); }
		}
		
		
		public void Clear(Color color)
		{
			Agg.Library.AggRendererSolidClear (this.agg_ren, color.R, color.G, color.B, color.A);
		}
		
		public void Clear(double r, double g, double b)
		{
			Agg.Library.AggRendererSolidClear (this.agg_ren, r, g, b, 1);
		}
		
		public void Clear(double r, double g, double b, double a)
		{
			Agg.Library.AggRendererSolidClear (this.agg_ren, r, g, b, a);
		}
		
		
		public void SetColor(Color color)
		{
			Agg.Library.AggRendererSolidColor (this.agg_ren, color.R, color.G, color.B, color.A);
		}
		
		public void SetColor(double r, double g, double b)
		{
			Agg.Library.AggRendererSolidColor (this.agg_ren, r, g, b, 1);
		}
		
		public void SetColor(double r, double g, double b, double a)
		{
			Agg.Library.AggRendererSolidColor (this.agg_ren, r, g, b, a);
		}
		
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.pixmap != null)
				{
					this.pixmap.Dispose ();
					this.pixmap = null;
				}
			}
			
			this.Detach ();
		}
		
		
		protected void Attach(Pixmap pixmap)
		{
			this.Detach ();
			
			this.agg_ren = Agg.Library.AggRendererSolidNew (pixmap.Handle);
			this.pixmap  = pixmap;
		}
		
		protected void Detach()
		{
			if (this.agg_ren != System.IntPtr.Zero)
			{
				Agg.Library.AggRendererSolidDelete (this.agg_ren);
				this.agg_ren = System.IntPtr.Zero;
				this.pixmap  = null;
			}
		}
		
		
		
		private System.IntPtr			agg_ren;
		private Pixmap					pixmap;
	}
}
