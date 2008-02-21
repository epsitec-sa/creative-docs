//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Renderers
{
	public sealed class Image : IRenderer, System.IDisposable, ITransformProvider
	{
		public Image()
		{
			this.handle = new Agg.SafeImageRendererHandle ();
		}
		
		
		public Pixmap							Pixmap
		{
			set
			{
				if (this.pixmap != value)
				{
					if (value == null)
					{
						this.BitmapImage = null;
						this.Detach ();
						this.transform.Reset ();
					}
					else
					{
						this.Attach (value);
					}
				}
			}
		}

		public Drawing.Image					BitmapImage
		{
			get
			{
				return this.image;
			}
			set
			{
				if (this.image != value)
				{
					if (this.bitmap != null)
					{
						if (this.bitmapNeedsUnlock)
						{
							this.bitmap.UnlockBits ();
						}
						
						this.AssertAttached ();
						
						this.bitmap = null;
						this.bitmapNeedsUnlock = false;
						
						AntiGrain.Renderer.Image.Source2 (this.handle, System.IntPtr.Zero, 0, 0, 0);
					}
					
					this.image = value;
					
					if (this.image != null)
					{
						this.bitmap              = this.image.BitmapImage;
						this.bitmapNeedsUnlock = ! this.bitmap.IsLocked;
						
						int width  = this.bitmap.PixelWidth;
						int height = this.bitmap.PixelHeight;
						
						if (this.bitmapNeedsUnlock)
						{
							this.bitmap.LockBits ();
						}
						
						this.AssertAttached ();
						
						AntiGrain.Renderer.Image.Source2 (this.handle, this.bitmap.Scan0, width, height, -this.bitmap.Stride);
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
				
				this.transform     = new Transform (value);
				this.internalTransform = new Transform (value);
				this.OnTransformUpdating ();
				
				Transform inverse = Transform.Inverse (this.internalTransform);
				
				AntiGrain.Renderer.Image.Matrix (this.handle, inverse.XX, inverse.XY, inverse.YX, inverse.YY, inverse.TX, inverse.TY);
			}
		}
		
		public Transform						InternalTransform
		{
			get
			{
				return this.internalTransform;
			}
		}
		
		public event Support.EventHandler		TransformUpdating;
		
		public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Image.SetAlphaMask (this.handle, (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle, (AntiGrain.Renderer.MaskComponent) component);
		}
		
		public void SelectAdvancedFilter(ImageFilteringMode mode, double radius)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Image.SetStretchMode (this.handle, (int) mode, radius);
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Detach ();
			this.BitmapImage = null;
		}
		#endregion
		
		private void AssertAttached()
		{
			if (this.handle.IsInvalid)
			{
				throw new System.NullReferenceException ("RendererImage not attached");
			}
		}
		
		private void Attach(Pixmap pixmap)
		{
			this.Detach ();
			
			this.transform.Reset ();
			this.handle.Create (pixmap.Handle);
			this.pixmap = pixmap;
		}
		
		private void Detach()
		{
			if (this.pixmap != null)
			{
				this.handle.Delete ();
				this.pixmap = null;
			}
		}
		
		
		private void OnTransformUpdating()
		{
			if (this.TransformUpdating != null)
			{
				this.TransformUpdating (this);
			}
		}
		
		
		
		private readonly Agg.SafeImageRendererHandle handle;
		private Pixmap							pixmap;
		private Drawing.Image					image;
		private Drawing.Bitmap					bitmap;
		private bool							bitmapNeedsUnlock;
		
		private Transform						transform		  = new Transform ();
		private Transform						internalTransform = new Transform ();
	}
}
