//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Renderers
{
	using BitmapData = System.Drawing.Imaging.BitmapData;
	
	public class Image : IRenderer, System.IDisposable, ITransformProvider
	{
		public Image()
		{
		}
		
		~Image()
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
						if (this.bitmap_needs_unlock)
						{
							this.bitmap.UnlockBits ();
						}
						
						this.AssertAttached ();
						
						this.bitmap = null;
						this.bitmap_needs_unlock = false;
						
						AntiGrain.Renderer.Image.Source2 (this.agg_ren, System.IntPtr.Zero, 0, 0, 0);
					}
					
					this.image = value;
					
					if (this.image != null)
					{
						this.bitmap              = this.image.BitmapImage;
						this.bitmap_needs_unlock = ! this.bitmap.IsLocked;
						
						int width  = this.bitmap.PixelWidth;
						int height = this.bitmap.PixelHeight;
						
						if (this.bitmap_needs_unlock)
						{
							this.bitmap.LockBits ();
						}
						
						this.AssertAttached ();
						
						AntiGrain.Renderer.Image.Source2 (this.agg_ren, this.bitmap.Scan0, width, height, -this.bitmap.Stride);
					}
				}
			}
		}
		
		public System.IntPtr					Handle
		{
			get
			{
				return this.agg_ren;
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
				
				if (this.agg_ren == System.IntPtr.Zero)
				{
					return;
				}
				
				this.transform     = new Transform (value);
				this.int_transform = new Transform (value);
				this.OnTransformUpdating ();
				
				Transform inverse = Transform.Inverse (this.int_transform);
				
				AntiGrain.Renderer.Image.Matrix (this.agg_ren, inverse.XX, inverse.XY, inverse.YX, inverse.YY, inverse.TX, inverse.TY);
			}
		}
		
		public Transform						InternalTransform
		{
			get
			{
				return this.int_transform;
			}
		}
		
		public event Support.EventHandler		TransformUpdating;
		
		public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Image.SetAlphaMask (this.agg_ren, (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle, (AntiGrain.Renderer.MaskComponent) component);
		}
		
		public void SelectAdvancedFilter(ImageFilteringMode mode, double radius)
		{
			this.AssertAttached ();
			AntiGrain.Renderer.Image.SetStretchMode (this.agg_ren, (int) mode, radius);
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
				
				this.BitmapImage = null;
			}
			
			this.Detach ();
		}
		
		
		protected virtual void AssertAttached()
		{
			if (this.agg_ren == System.IntPtr.Zero)
			{
				throw new System.NullReferenceException ("RendererImage not attached");
			}
		}
		
		protected void Attach(Pixmap pixmap)
		{
			this.Detach ();
			
			this.transform.Reset ();
			this.agg_ren = AntiGrain.Renderer.Image.New (pixmap.Handle);
			this.pixmap  = pixmap;
		}
		
		protected void Detach()
		{
			if (this.agg_ren != System.IntPtr.Zero)
			{
				AntiGrain.Renderer.Image.Delete (this.agg_ren);
				this.agg_ren = System.IntPtr.Zero;
				this.pixmap  = null;
			}
		}
		
		
		protected virtual void OnTransformUpdating()
		{
			if (this.TransformUpdating != null)
			{
				this.TransformUpdating (this);
			}
		}
		
		
		
		private System.IntPtr					agg_ren;
		private Pixmap							pixmap;
		private Drawing.Image					image;
		private Drawing.Bitmap					bitmap;
		private bool							bitmap_needs_unlock;
		
		private Transform						transform		= new Transform ();
		private Transform						int_transform	= new Transform ();
	}
}
