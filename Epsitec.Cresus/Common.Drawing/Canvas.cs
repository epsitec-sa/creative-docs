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
		}
		
		public override void DefineZoom(double zoom)
		{
			this.zoom = zoom;
			this.InvalidateCache ();
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
					
					Canvas.Engine.Paint (graphics, size, this.data);
					
					System.Drawing.Bitmap win32_bitmap = new System.Drawing.Bitmap (dx, dy, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					
					using (System.Drawing.Graphics win32_graphics = System.Drawing.Graphics.FromImage (win32_bitmap))
					{
						System.Drawing.Rectangle clip = new System.Drawing.Rectangle (0, 0, dx, dy);
						pixmap.Blend (win32_graphics, new System.Drawing.Point (0, 0), clip);
					}
					
					System.Diagnostics.Debug.WriteLine ("Cached bitmap size: " + this.Size.ToString ());
					
					int width;
					int height;
					int stride;
					System.Drawing.Imaging.PixelFormat format;
					System.IntPtr scan0;
					pixmap.GetMemoryLayout (out width, out height, out stride, out format, out scan0);
					
					System.Drawing.Imaging.BitmapData data = win32_bitmap.LockBits (new System.Drawing.Rectangle (0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, format);
					
					unsafe
					{
						uint* src = (uint*) scan0.ToPointer ();
						uint* dst = (uint*) data.Scan0.ToPointer () + height * stride / 4;
						
						for (int line = 0; line < height; line++)
						{
							dst -= stride / 4;
							uint* ptr = dst;
							
							for (int x = 0; x < stride / 4; x++)
							{
								*ptr++ = *src++;
							}
						}
					}
					win32_bitmap.UnlockBits (data);
					
					System.Diagnostics.Debug.WriteLine (width+"x"+height+", "+stride+", ptr="+scan0.ToString());
					
					this.cache = Bitmap.FromNativeBitmap (win32_bitmap, this.Origin, this.Size).BitmapImage;
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
		
		protected byte[]				data;
		protected double				zoom = 1.0;
		protected Bitmap				cache;
		
		protected static ICanvasEngine	engine;
	}
}
