namespace Epsitec.Common.Drawing
{
	public enum GradientFill
	{
		None,
		X, Y, XY,
		Circle,
		Diamond,
		SqrtXY,
		Conic
	}
}

namespace Epsitec.Common.Drawing.Renderers
{
	public class Gradient : IRenderer, System.IDisposable, ITransformProvider
	{
		public Gradient()
		{
		}
		
		~Gradient()
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
		
		public GradientFill				Fill
		{
			get
			{
				return this.fill;
			}
			set
			{
				if (this.fill != value)
				{
					this.AssertAttached ();
					this.fill = value;
					AntiGrain.Renderer.Gradient.Select (this.agg_ren, (int) this.fill);
				}
			}
		}
		
		public System.IntPtr			Handle
		{
			get { return this.agg_ren; }
		}
		
		public Transform				Transform
		{
			get
			{
				return this.transform;
			}
			set
			{
				if (value == null)
				{
					throw new System.NullReferenceException ("Rasterizer.Transform");
				}
				
				//	Note: on recalcule la transformation à tous les coups, parce que l'appelant peut être
				//	Graphics.UpdateTransform...
				
				this.AssertAttached ();
				this.transform     = new Transform (value);
				this.int_transform = new Transform (value);
				this.OnTransformUpdating ();
				Transform inverse = Transform.Inverse (this.int_transform);
				AntiGrain.Renderer.Gradient.Matrix (this.agg_ren, inverse.XX, inverse.XY, inverse.YX, inverse.YY, inverse.TX, inverse.TY);
			}
		}
		
		public Transform				InternalTransform
		{
			get { return this.int_transform; }
		}
		
		public event Support.EventHandler		TransformUpdating;
		
		public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Gradient.SetAlphaMask (this.agg_ren, (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle, (AntiGrain.Renderer.MaskComponent) component);
		}
		
		
		public void SetColors(Color a, Color b)
		{
			this.SetColors (a.R, a.G, a.B, a.A, b.R, b.G, b.B, b.A);
		}
		
		public void SetColors(double ar, double ag, double ab, double br, double bg, double bb)
		{
			this.SetColors (ar, ag, ab, 1.0, br, bg, bb, 1.0);
		}
		
		public void SetColors(double ar, double ag, double ab, double aa, double br, double bg, double bb, double ba)
		{
			double[] r = new double[256];
			double[] g = new double[256];
			double[] b = new double[256];
			double[] a = new double[256];
			
			double delta_r = (br - ar) / 255.0;
			double delta_g = (bg - ag) / 255.0;
			double delta_b = (bb - ab) / 255.0;
			double delta_a = (ba - aa) / 255.0;
			
			for (int i = 0; i < 256; i++)
			{
				r[i] = ar + i * delta_r;
				g[i] = ag + i * delta_g;
				b[i] = ab + i * delta_b;
				a[i] = aa + i * delta_a;
			}
			
			this.SetColors (r, g, b, a);
		}
		
		public void SetColors(double[] r, double[] g, double[] b, double[] a)
		{
			if ((r.Length != 256) ||
				(g.Length != 256) ||
				(b.Length != 256) ||
				(a.Length != 256))
			{
				throw new System.ArgumentOutOfRangeException ("Color arrays missized");
			}
			
			this.AssertAttached ();
			AntiGrain.Renderer.Gradient.Color1 (this.agg_ren, r, g, b, a);
		}

		
		public void SetParameters(double r1, double r2)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Gradient.Range (this.agg_ren, r1, r2);
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
		
		protected virtual void AssertAttached()
		{
			if (this.agg_ren == System.IntPtr.Zero)
			{
				throw new System.NullReferenceException ("RendererGradient not attached");
			}
		}
		
		
		protected void Attach(Pixmap pixmap)
		{
			this.Detach ();
			
			this.agg_ren = AntiGrain.Renderer.Gradient.New (pixmap.Handle);
			this.pixmap  = pixmap;
		}
		
		protected void Detach()
		{
			if (this.agg_ren != System.IntPtr.Zero)
			{
				AntiGrain.Renderer.Gradient.Delete (this.agg_ren);
				this.agg_ren = System.IntPtr.Zero;
				this.pixmap  = null;
				this.fill    = GradientFill.None;
				this.transform.Reset ();
				this.int_transform.Reset ();
			}
		}
		
		protected virtual void OnTransformUpdating()
		{
			if (this.TransformUpdating != null)
			{
				this.TransformUpdating (this);
			}
		}
		
		
		
		private System.IntPtr			agg_ren;
		private Pixmap					pixmap;
		private GradientFill			fill			= GradientFill.None;
		private Transform				transform		= new Transform ();
		private Transform				int_transform	= new Transform ();
	}
}
