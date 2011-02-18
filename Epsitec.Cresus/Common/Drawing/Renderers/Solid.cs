//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Renderers
{
	public sealed class Solid : IRenderer, System.IDisposable
	{
		public Solid()
		{
			this.handle = new Agg.SafeSolidRendererHandle ();
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
			get { return this.handle; }
		}
		
		public Color							Color
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
				this.ClearAlphaRgb (color.A, color.R, color.G, color.B);
			}
		}
		
		public void Clear(double r, double g, double b)
		{
			this.ClearAlphaRgb (1, r, g, b);
		}
		
		public void ClearAlphaRgb(double a, double r, double g, double b)
		{
			if (this.handle.IsInvalid)
			{
				return;
			}
			
			AntiGrain.Renderer.Solid.Clear (this.handle, r, g, b, a);
		}
		
		public void Clear4Colors(int x, int y, int dx, int dy, Color c1, Color c2, Color c3, Color c4)
		{
			if (this.handle.IsInvalid)
			{
				return;
			}

			AntiGrain.Renderer.Special.Fill4Colors (this.handle, x, y, dx, dy, c1.R, c1.G, c1.B, c2.R, c2.G, c2.B, c3.R, c3.G, c3.B, c4.R, c4.G, c4.B);
		}
		
		
		public void SetColor(Color color)
		{
			if (color.IsEmpty)
			{
				this.SetColorAlphaRgb (0, 0, 0, 0);
			}
			else
			{
				this.SetColorAlphaRgb (color.A, color.R, color.G, color.B);
			}
		}
		
		public void SetColor(double r, double g, double b)
		{
			this.SetColorAlphaRgb (1, r, g, b);
		}
		
		public void SetColorAlphaRgb(double a, double r, double g, double b)
		{
			if (this.handle.IsInvalid)
			{
				return;
			}

			AntiGrain.Renderer.Solid.Color (this.handle, r, g, b, a);
		}
		
		
		public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
		{
			if (this.handle.IsInvalid)
			{
				return;
			}

			AntiGrain.Renderer.Solid.SetAlphaMask (this.handle, (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle, (AntiGrain.Renderer.MaskComponent) component);
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Detach ();
		}
		#endregion
		
		private void AssertAttached()
		{
			if (this.handle.IsInvalid)
			{
				throw new System.NullReferenceException ("SolidRenderer not attached");
			}
		}


		private void Attach(Pixmap pixmap)
		{
			this.Detach ();
			
			this.handle.Create (pixmap.Handle);
			this.pixmap = pixmap;
			this.color  = new Color ();
		}

		private void Detach()
		{
			if (this.pixmap != null)
			{
				this.handle.Delete ();
				this.pixmap = null;
			}
		}
		
		
		private Color							color;
		private readonly Agg.SafeSolidRendererHandle handle;
		private Pixmap							pixmap;
	}
}
