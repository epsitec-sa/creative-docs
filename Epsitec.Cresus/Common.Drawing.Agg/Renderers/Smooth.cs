//	Copyright © 2003-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Renderers
{
	public sealed class Smooth : IRenderer, System.IDisposable
	{
		public Smooth(Graphics graphics)
		{
			this.graphics = graphics;
			this.handle = new Agg.SafeSmoothRendererHandle ();
		}
		
		
		public Pixmap							Pixmap
		{
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
			get
			{
				return this.handle;
			}
		}

		public Color							Color
		{
			set
			{
				if (this.handle.IsInvalid)
				{
					return;
				}

				AntiGrain.Renderer.Smooth.Color (this.handle, value.R, value.G, value.B, value.A);
			}
		}


		public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Smooth.SetAlphaMask (this.handle, (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle, (AntiGrain.Renderer.MaskComponent) component);
		}
		
		public void SetParameters(double r1, double r2)
		{
			if ((this.r1 != r1) ||
				(this.r2 != r2))
			{
				this.r1 = r1;
				this.r2 = r2;
				
				if (this.handle.IsInvalid)
				{
					return;
				}
			}
		}
		
		
		public void AddPath(Drawing.Path path)
		{
			this.SetTransform (this.graphics.Transform);
			
			AntiGrain.Renderer.Smooth.Setup (this.handle, this.r1, this.r2, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
			AntiGrain.Renderer.Smooth.AddPath (this.handle, path.Handle);
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Detach ();
		}
		#endregion


		private void SetTransform(Transform value)
		{
			if (this.handle.IsInvalid)
			{
				return;
			}
			
			this.transform = value;
			AntiGrain.Renderer.Smooth.Setup (this.handle, this.r1, this.r2, this.transform.XX, this.transform.XY, this.transform.YX, this.transform.YY, this.transform.TX, this.transform.TY);
		}

		private void AssertAttached()
		{
			if (this.handle.IsInvalid)
			{
				throw new System.NullReferenceException ("RendererSmooth not attached");
			}
		}


		private void Attach(Pixmap pixmap)
		{
			this.Detach ();
			
			this.handle.Create (pixmap.Handle);
			this.pixmap = pixmap;
		}

		private void Detach()
		{
			if (this.pixmap != null)
			{
				this.handle.Delete ();
				this.pixmap = null;
				this.transform = Transform.Identity;
			}
		}
		
		
		readonly Graphics						graphics;
		private readonly Agg.SafeSmoothRendererHandle handle;
		private Pixmap							pixmap;
		private Transform						transform = Transform.Identity;
		private	double							r1;
		private double							r2;
	}
}
