namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Canvas permet de représenter une image vectorielle.
	/// </summary>
	public class Canvas : Image
	{
		internal Canvas(byte[] data)
		{
			this.data = new byte[data.Length];
			data.CopyTo (this.data, 0);
			
			this.disabled = new Canvas (this);
		}
		
		protected Canvas(Canvas original)
		{
			//	Version "disabled" du même dessin; on partage les données avec le modèle
			//	original...
			
			this.data     = original.data;
			this.disabled = this;
		}
		
		public override void DefineZoom(double zoom)
		{
			this.zoom = zoom;
			this.InvalidateCache ();
		}
		
		public override Image GetDisabled()
		{
			return this.disabled;
		}
		
		
		public static ICanvasEngine		Engine
		{
			get { return Canvas.engine; }
			set { Canvas.engine = value; }
		}
		
		public override Bitmap			BitmapImage
		{
			get
			{
				this.ValidateCache ();
				return this.cache;
			}
		}
		
		public override Size			Size
		{
			get
			{
				this.ValidateGeometry ();
				return base.Size;
			}
		}

		public override Point			Origin
		{
			get
			{
				this.ValidateGeometry ();
				return base.Origin;
			}
		}

		public override bool			IsOriginDefined
		{
			get { return true; }
		}
		
		public override bool			IsDisabledDefined
		{
			get { return true; }
		}
		
		
		protected void ValidateCache()
		{
			if (this.cache == null)
			{
				int dx = (int) (this.Width * this.zoom);
				int dy = (int) (this.Height * this.zoom);
				
				Size size = new Size (dx, dy);
				
				System.Diagnostics.Debug.Assert (Canvas.Engine != null);
				
				using (Drawing.Agg.Graphics graphics = new Agg.Graphics ())
				{
					graphics.SetPixmapSize (dx, dy);
					Drawing.Pixmap pixmap = graphics.Pixmap;
					pixmap.Clear ();
					
					bool is_disabled = (this == this.disabled);
					
					Canvas.Engine.Paint (graphics, size, this.data, is_disabled);
					
					int width, height, stride;
					System.Drawing.Imaging.PixelFormat format;
					System.IntPtr scan0;
					
					pixmap.GetMemoryLayout (out width, out height, out stride, out format, out scan0);
					
					System.Diagnostics.Debug.Assert (width == dx);
					System.Diagnostics.Debug.Assert (height == dy);
					

					System.Drawing.Bitmap                bitmap = new System.Drawing.Bitmap (dx, dy, format);
					System.Drawing.Rectangle             bbox   = new System.Drawing.Rectangle (0, 0, dx, dy);
					System.Drawing.Imaging.ImageLockMode mode   = System.Drawing.Imaging.ImageLockMode.WriteOnly;
					System.Drawing.Imaging.BitmapData    data   = bitmap.LockBits (bbox, mode, format);
					
					try
					{
						unsafe
						{
							int   num = stride / 4;
							uint* src = (uint*) scan0.ToPointer ();
							uint* buf = (uint*) data.Scan0.ToPointer () + dy * num;
						
							for (int line = 0; line < dy; line++)
							{
								buf -= num;
							
								uint* dst = buf;
							
								for (int i = 0; i < num; i++)
								{
									*dst++ = *src++;
								}
							}
						}
					}
					finally
					{
						bitmap.UnlockBits (data);
					}
					
					this.cache = Bitmap.FromNativeBitmap (bitmap, this.Origin, this.Size).BitmapImage;
				}
			}
		}
		
		protected void ValidateGeometry()
		{
			if (this.is_geom_ok == false)
			{
				System.Diagnostics.Debug.Assert (Canvas.Engine != null);
				
				Canvas.Engine.GetSizeAndOrigin (this.data, out this.size, out this.origin);
				
				this.is_geom_ok = true;
			}
		}
		
		protected void InvalidateCache()
		{
			if (this.cache != null)
			{
				this.cache.Dispose ();
				this.cache = null;
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.Assert (this.is_disposed == false);
			
			this.is_disposed = true;
			
			if (disposing)
			{
				this.InvalidateCache ();
			}
			
			base.Dispose (disposing);
		}

		
		protected bool					is_disposed;
		protected bool					is_geom_ok;
		
		protected Canvas				disabled;
		protected byte[]				data;
		protected double				zoom = 1.0;
		protected Bitmap				cache;
		
		protected static ICanvasEngine	engine;
	}
}
