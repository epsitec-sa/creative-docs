namespace Epsitec.Common.Drawing.Renderers
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
		
		
		public Pixmap						Pixmap
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
		
		public System.IntPtr				Handle
		{
			get { return this.agg_ren; }
		}
		
		public Color						Color
		{
			get
			{
				return this.color;
			}
			set
			{
				if (this.color != value)
				{
					this.color = value;
					this.SetColor (value);
				}
			}
		}
		
		
		public void Clear(Color color)
		{
			if (color.IsValid)
			{
				this.ClearARGB (color.A, color.R, color.G, color.B);
			}
		}
		
		public void Clear(double r, double g, double b)
		{
			this.ClearARGB (1, r, g, b);
		}
		
		public void ClearARGB(double a, double r, double g, double b)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Solid.Clear (this.agg_ren, r, g, b, a);
		}
		
		public void Clear4Colors(int x, int y, int dx, int dy, Color c1, Color c2, Color c3, Color c4)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Special.Fill4Colors (this.agg_ren, x, y, dx, dy, c1.R, c1.G, c1.B, c2.R, c2.G, c2.B, c3.R, c3.G, c3.B, c4.R, c4.G, c4.B);
		}
		
		
		public void SetColor(Color color)
		{
			if (color.IsEmpty)
			{
				this.SetColorARGB (0, 0, 0, 0);
			}
			else
			{
				this.SetColorARGB (color.A, color.R, color.G, color.B);
			}
		}
		
		public void SetColor(double r, double g, double b)
		{
			this.SetColorARGB (1, r, g, b);
		}
		
		public void SetColorARGB(double a, double r, double g, double b)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Solid.Color (this.agg_ren, r, g, b, a);
		}
		
		
		public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Solid.SetAlphaMask (this.agg_ren, (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle, (AntiGrain.Renderer.MaskComponent) component);
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected void AssertAttached()
		{
			if (this.agg_ren == System.IntPtr.Zero)
			{
				throw new System.NullReferenceException ("SolidRenderer not attached");
			}
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
			
			this.agg_ren = AntiGrain.Renderer.Solid.New (pixmap.Handle);
			this.pixmap  = pixmap;
		}
		
		protected void Detach()
		{
			if (this.agg_ren != System.IntPtr.Zero)
			{
				AntiGrain.Renderer.Solid.Delete (this.agg_ren);
				this.agg_ren = System.IntPtr.Zero;
				this.pixmap  = null;
			}
		}
		
		
		private Color						color;
		private System.IntPtr				agg_ren;
		private Pixmap						pixmap;
	}
}
