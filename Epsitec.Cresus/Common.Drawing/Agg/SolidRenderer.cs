using System;

namespace Epsitec.Common.Drawing.Agg
{
	/// <summary>
	/// Implémentation de la classe Renderer.Solid basée sur AGG.
	/// </summary>
	public class SolidRenderer : Epsitec.Common.Drawing.Renderers.Solid
	{
		public SolidRenderer()
		{
		}
		
		public override Pixmap				Pixmap
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
		
		public override System.IntPtr		Handle
		{
			get { return this.agg_ren; }
		}
		
		
		
		public override void ClearARGB(double a, double r, double g, double b)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Solid.Clear (this.agg_ren, r, g, b, a);
		}
		
		public override void Clear4Colors(int x, int y, int dx, int dy, Color c1, Color c2, Color c3, Color c4)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Special.Fill4Colors (this.agg_ren, x, y, dx, dy, c1.R, c1.G, c1.B, c2.R, c2.G, c2.B, c3.R, c3.G, c3.B, c4.R, c4.G, c4.B);
		}
		
		public override void SetColorARGB(double a, double r, double g, double b)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Solid.Color (this.agg_ren, r, g, b, a);
		}
		
		
		public override void SetAlphaMask(Pixmap pixmap, MaskComponent component)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Solid.SetAlphaMask (this.agg_ren, (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle, (AntiGrain.Renderer.MaskComponent) component);
		}
		
		protected virtual void AssertAttached()
		{
			if (this.agg_ren == System.IntPtr.Zero)
			{
				throw new System.NullReferenceException ("SolidRenderer not attached");
			}
		}
		
		protected override void Dispose(bool disposing)
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
		
		
		
		private System.IntPtr			agg_ren;
		private Pixmap					pixmap;
	}
}
