//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Renderers
{
	public class Smooth : IRenderer, System.IDisposable
	{
		public Smooth(Graphics graphics)
		{
			this.graphics = graphics;
		}
		
		~Smooth()
		{
			this.Dispose (false);
		}
		
		
		public Pixmap							Pixmap
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
		
		
		public System.IntPtr					Handle
		{
			get { return this.agg_ren; }
		}
		
		public Color							Color
		{
			set { this.SetColor (value); }
		}
		
		
		public void SetColor(Color color)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Smooth.Color (this.agg_ren, color.R, color.G, color.B, color.A);
		}
		
		public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Smooth.SetAlphaMask (this.agg_ren, (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle, (AntiGrain.Renderer.MaskComponent) component);
		}
		
		public void SetParameters(double r1, double r2)
		{
			if ((this.r1 != r1) ||
				(this.r2 != r2))
			{
				this.r1 = r1;
				this.r2 = r2;
				this.AssertAttached ();
			}
		}
		
		
		public void AddPath(Drawing.Path path)
		{
			this.SetTransform (graphics.Transform);
			
			AntiGrain.Renderer.Smooth.Setup (this.agg_ren, this.r1, this.r2, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
			AntiGrain.Renderer.Smooth.AddPath (this.agg_ren, path.Handle);
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
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
		
		protected void SetTransform(Transform value)
		{
			this.AssertAttached ();
			
			this.transform = new Transform (value);
			AntiGrain.Renderer.Smooth.Setup (this.agg_ren, this.r1, this.r2, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
		}
		
		protected virtual void AssertAttached()
		{
			if (this.agg_ren == System.IntPtr.Zero)
			{
				throw new System.NullReferenceException ("RendererSmooth not attached");
			}
		}
		
		
		protected void Attach(Pixmap pixmap)
		{
			this.Detach ();
			
			this.agg_ren = AntiGrain.Renderer.Smooth.New (pixmap.Handle);
			this.pixmap  = pixmap;
		}
		
		protected void Detach()
		{
			if (this.agg_ren != System.IntPtr.Zero)
			{
				AntiGrain.Renderer.Smooth.Delete (this.agg_ren);
				this.agg_ren = System.IntPtr.Zero;
				this.pixmap  = null;
				this.transform.Reset ();
			}
		}
		
		
		private Graphics						graphics;
		private System.IntPtr					agg_ren;
		private Pixmap							pixmap;
		private Transform						transform = new Transform ();
		private	double							r1;
		private double							r2;
	}
}
