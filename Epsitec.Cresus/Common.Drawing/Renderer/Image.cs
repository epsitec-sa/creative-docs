namespace Epsitec.Common.Drawing.Renderer
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

		public System.Drawing.Bitmap	Bitmap
		{
			get
			{
				return this.bitmap;
			}
			
			set
			{
				if (this.bitmap != value)
				{
					if (this.bitmap != null)
					{
						System.Diagnostics.Debug.Assert (this.bitmap != null);
						System.Diagnostics.Debug.Assert (this.bitmap_data != null);
						
						this.bitmap.UnlockBits (this.bitmap_data);
						this.bitmap_data = null;
						
						this.AssertAttached ();
						
						Agg.Library.AggRendererImageSource2 (this.agg_ren, System.IntPtr.Zero, 0, 0, 0);
					}
					
					this.bitmap = value;
					
					if (this.bitmap != null)
					{
						int width  = this.bitmap.Width;
						int height = this.bitmap.Height;
						
						System.Drawing.Imaging.ImageLockMode mode = System.Drawing.Imaging.ImageLockMode.ReadOnly;
						System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
						
						this.bitmap_data = this.bitmap.LockBits (new System.Drawing.Rectangle (0, 0, width, height), mode, format);
						
						this.AssertAttached ();
						
						Agg.Library.AggRendererImageSource2 (this.agg_ren, this.bitmap_data.Scan0, width, height, -this.bitmap_data.Stride);
					}
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
				this.OnTransformUpdating (System.EventArgs.Empty);
				Transform inverse = Transform.Inverse (this.int_transform);
				Agg.Library.AggRendererImageMatrix (this.agg_ren, inverse.XX, inverse.XY, inverse.YX, inverse.YY, inverse.TX, inverse.TY);
			}
		}
		
		public Transform				InternalTransform
		{
			get { return this.int_transform; }
		}
		
		public event System.EventHandler TransformUpdating;
		
		
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
				
				System.Drawing.Bitmap bitmap = this.bitmap;
				this.Bitmap = null;
				
				if (bitmap != null)
				{
					bitmap.Dispose ();
				}
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
			
			this.agg_ren = Agg.Library.AggRendererImageNew (pixmap.Handle);
			this.pixmap  = pixmap;
		}
		
		protected void Detach()
		{
			if (this.agg_ren != System.IntPtr.Zero)
			{
				Agg.Library.AggRendererImageDelete (this.agg_ren);
				this.agg_ren = System.IntPtr.Zero;
				this.pixmap  = null;
				this.transform.Reset ();
			}
		}
		
		protected virtual void OnTransformUpdating(System.EventArgs e)
		{
			if (this.TransformUpdating != null)
			{
				this.TransformUpdating (this, e);
			}
		}
		
		
		
		private System.IntPtr			agg_ren;
		private Pixmap					pixmap;
		private System.Drawing.Bitmap	bitmap;
		private BitmapData				bitmap_data;
		private Transform				transform		= new Transform ();
		private Transform				int_transform	= new Transform ();
	}
}
