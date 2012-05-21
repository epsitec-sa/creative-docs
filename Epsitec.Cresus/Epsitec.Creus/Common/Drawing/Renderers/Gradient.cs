//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Renderers
{
	/// <summary>
	/// The <c>Gradient</c> class implements a renderer which can paint various styles
	/// of gradients, as defined by the <see cref="GradientFill"/> enumeration.
	/// </summary>
	public sealed class Gradient : IRenderer, System.IDisposable
	{
		public Gradient(Graphics graphics)
		{
			this.graphics = graphics;
			this.handle   = new Agg.SafeGradientRendererHandle ();
			this.AlphaMutiplier = 1.0;
		}


		public double							AlphaMutiplier
		{
			get;
			set;
		}

	
		public Transform						Transform
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
				
				if (this.handle.IsInvalid)
				{
					return;
				}
				
				this.transform         = value;
				this.internalTransform = value.MultiplyBy (this.graphics.Transform);

				Transform inverse = Transform.Inverse (this.internalTransform);
				AntiGrain.Renderer.Gradient.Matrix (this.handle, inverse.XX, inverse.XY, inverse.YX, inverse.YY, inverse.TX, inverse.TY);
			}
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
		
		public GradientFill						Fill
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
					AntiGrain.Renderer.Gradient.Select (this.handle, (int) this.fill);
				}
			}
		}
		
		public System.IntPtr					Handle
		{
			get { return this.handle; }
		}

		public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Gradient.SetAlphaMask (this.handle, (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle, (AntiGrain.Renderer.MaskComponent) component);
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
			
			double deltaR = (br - ar) / 255.0;
			double deltaG = (bg - ag) / 255.0;
			double deltaB = (bb - ab) / 255.0;
			double deltaA = (ba - aa) / 255.0;
			
			for (int i = 0; i < 256; i++)
			{
				r[i] = ar + i * deltaR;
				g[i] = ag + i * deltaG;
				b[i] = ab + i * deltaB;
				a[i] = aa + i * deltaA;
			}
			
			this.SetColors (r, g, b, a);
		}
		
		public void SetColors(double[] r, double[] g, double[] b, double[] a)
		{
			if (r.Length != 256 ||
				g.Length != 256 ||
				b.Length != 256 ||
				a.Length != 256)
			{
				throw new System.ArgumentOutOfRangeException ("Color arrays missized");
			}
			
			this.AssertAttached ();

			if (this.AlphaMutiplier == 1.0)
			{
				AntiGrain.Renderer.Gradient.Color1 (this.handle, r, g, b, a);
			}
			else
			{
				double[] rr = new double[r.Length];
				double[] gg = new double[r.Length];
				double[] bb = new double[r.Length];
				double[] aa = new double[r.Length];

				for (int i = 0; i < r.Length; i++)
				{
					rr[i] = r[i]*this.AlphaMutiplier;
					gg[i] = g[i]*this.AlphaMutiplier;
					bb[i] = b[i]*this.AlphaMutiplier;
					aa[i] = a[i]*this.AlphaMutiplier;
				}

				AntiGrain.Renderer.Gradient.Color1 (this.handle, rr, gg, bb, aa);
			}
		}

		
		public void SetParameters(double r1, double r2)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Gradient.Range (this.handle, r1, r2);
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
				throw new System.NullReferenceException ("RendererGradient not attached");
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
				this.pixmap  = null;
				this.fill    = GradientFill.None;
				this.transform = Transform.Identity;
				this.internalTransform = Transform.Identity;
			}
		}
		

		readonly Graphics						graphics;
		readonly Agg.SafeGradientRendererHandle	handle;
		private Pixmap							pixmap;
		private GradientFill					fill;
		private Transform						transform		  = Transform.Identity;
		private Transform						internalTransform = Transform.Identity;
	}
}
