namespace Epsitec.Common.Drawing.Renderer
{
	public class Solid : System.IDisposable
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
		
		
		public void Attach(Pixmap pixmap)
		{
			this.Detach ();
			
			this.agg_ren = Agg.Library.AggRendererSolidNew (pixmap.Handle);
			this.pixmap  = pixmap;
		}
		
		public void Detach()
		{
			if (this.agg_ren != System.IntPtr.Zero)
			{
				Agg.Library.AggRendererSolidDelete (this.agg_ren);
				this.agg_ren = System.IntPtr.Zero;
				this.pixmap  = null;
			}
		}
		
		
		public void Clear(System.Drawing.Color color)
		{
			Agg.Library.AggRendererSolidClear (this.agg_ren, color.R / 255.0, color.G / 255.0, color.B / 255.0, color.A / 255.0);
		}
		
		public void Clear(double r, double g, double b)
		{
			Agg.Library.AggRendererSolidClear (this.agg_ren, r, g, b, 1);
		}
		
		public void Clear(double r, double g, double b, double a)
		{
			Agg.Library.AggRendererSolidClear (this.agg_ren, r, g, b, a);
		}
		
		
		public void SetColor(System.Drawing.Color color)
		{
			Agg.Library.AggRendererSolidColor (this.agg_ren, color.R / 255.0, color.G / 255.0, color.B / 255.0, color.A / 255.0);
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
		
		
		
		private System.IntPtr			agg_ren;
		private Pixmap					pixmap;
	}
}
