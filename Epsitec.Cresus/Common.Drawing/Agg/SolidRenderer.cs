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
		
		
		
		public override void Clear(double r, double g, double b, double a)
		{
			AntiGrain.Renderer.Solid.Clear (this.agg_ren, r, g, b, a);
		}
		
		public override void SetColor(double r, double g, double b, double a)
		{
			AntiGrain.Renderer.Solid.Color (this.agg_ren, r, g, b, a);
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
