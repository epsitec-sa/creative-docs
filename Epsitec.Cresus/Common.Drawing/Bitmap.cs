namespace Epsitec.Common.Drawing
{
	using BitmapData = System.Drawing.Imaging.BitmapData;
	
	/// <summary>
	/// La classe Bitmap encapsule une image de type bitmap.
	/// </summary>
	public class Bitmap : Image, System.IDisposable
	{
		public Bitmap()
		{
		}
		
		
		public override Bitmap			BitmapImage
		{
			get { return this; }
		}
		
		public System.Drawing.Bitmap	NativeBitmap
		{
			get { return this.bitmap; }
		}
		
		public System.IntPtr			Scan0
		{
			get { return this.bitmap_data.Scan0; }
		}
		
		public int						Stride
		{
			get { return this.bitmap_data.Stride; }
		}
		
		
		public int						PixelWidth
		{
			get { return this.bitmap.Width; }
		}
		public int						PixelHeight
		{
			get { return this.bitmap.Height; }
		}
		
		public bool LockBits()
		{
			lock (this)
			{
				if (this.bitmap_data == null)
				{
					System.Drawing.Imaging.ImageLockMode mode = System.Drawing.Imaging.ImageLockMode.ReadOnly;
					System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
					
					int width  = this.bitmap.Width;
					int height = this.bitmap.Height;
					
					this.bitmap_data = this.bitmap.LockBits (new System.Drawing.Rectangle (0, 0, width, height), mode, format);
				}
				
				if (this.bitmap_data != null)
				{
					this.bitmap_lock_count++;
					return true;
				}
			}
			
			return false;
		}
		
		public void UnlockBits()
		{
			lock (this)
			{
				if (this.bitmap_lock_count > 0)
				{
					this.bitmap_lock_count--;
					
					if (this.bitmap_lock_count == 0)
					{
						System.Diagnostics.Debug.Assert (this.bitmap != null);
						System.Diagnostics.Debug.Assert (this.bitmap_data != null);
						
						this.bitmap.UnlockBits (this.bitmap_data);
						this.bitmap_data = null;
					}
				}
			}
		}
		
		
		public static Bitmap FromNativeBitmap(System.Drawing.Bitmap native)
		{
			Bitmap bitmap = Bitmap.FromNativeBitmap (native, new Point (0, 0));
			bitmap.is_origin_defined = false;
			return bitmap;
		}
		
		public static Bitmap FromNativeBitmap(System.Drawing.Bitmap native, Point origin)
		{
			return Bitmap.FromNativeBitmap (native, origin, Size.Empty);
		}
		
		public static Bitmap FromNativeBitmap(System.Drawing.Bitmap native, Point origin, Size size)
		{
			if (size == Size.Empty)
			{
				size = new Size (native.Width, native.Height);
			}
			
			Bitmap bitmap = new Bitmap ();
			bitmap.bitmap = native;
			bitmap.size   = size;
			bitmap.origin = origin;
			
			bitmap.is_origin_defined = true;
			
			return bitmap;
		}
		
		public static Bitmap FromData(byte[] data)
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream (data, false))
			{
				System.Drawing.Bitmap native = new System.Drawing.Bitmap (stream);
				Bitmap bitmap = Bitmap.FromNativeBitmap (native);
				bitmap.is_origin_defined = false;
				return bitmap;
			}
		}
		
		public static Bitmap FromData(byte[] data, Point origin)
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream (data, false))
			{
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (stream);
				return Bitmap.FromNativeBitmap (bitmap, origin);
			}
		}
		
		public static Bitmap FromData(byte[] data, Point origin, Size size)
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream (data, false))
			{
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (stream);
				return Bitmap.FromNativeBitmap (bitmap, origin, size);
			}
		}
		
		public static Bitmap FromFile(string file_name)
		{
			Bitmap bitmap = Bitmap.FromFile (file_name, new Point (0, 0));
			bitmap.is_origin_defined = false;
			return bitmap;
		}
		
		public static Bitmap FromFile(string file_name, Point origin)
		{
			return Bitmap.FromFile (file_name, origin, Size.Empty);
		}
		
		public static Bitmap FromFile(string file_name, Point origin, Size size)
		{
			Bitmap bitmap = new Bitmap ();
			
			using (System.Drawing.Image src_image = System.Drawing.Image.FromFile (file_name))
			{
				if (size == Size.Empty)
				{
					size = new Size (src_image.Width, src_image.Height);
				}
				
				bitmap.bitmap = new System.Drawing.Bitmap (src_image);
				bitmap.size   = size;
				bitmap.origin = origin;
				
				bitmap.is_origin_defined = true;
			}
			
			return bitmap;
		}
		
		public static Bitmap FromManifestResource(string resource_name, System.Reflection.Assembly assembly)
		{
			using (System.IO.Stream stream = assembly.GetManifestResourceStream (resource_name))
			{
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (stream);
				return Bitmap.FromNativeBitmap (bitmap);
			}
		}
		
		public static Bitmap FromManifestResource(string resource_name, System.Reflection.Assembly assembly, Point origin)
		{
			using (System.IO.Stream stream = assembly.GetManifestResourceStream (resource_name))
			{
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (stream);
				return Bitmap.FromNativeBitmap (bitmap, origin);
			}
		}
		
		public static Bitmap FromManifestResource(string resource_name, System.Reflection.Assembly assembly, Point origin, Size size)
		{
			using (System.IO.Stream stream = assembly.GetManifestResourceStream (resource_name))
			{
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (stream);
				return Bitmap.FromNativeBitmap (bitmap, origin, size);
			}
		}
		
		public static Bitmap FromImageDisabled(Image image, Color background)
		{
			if (image == null)
			{
				return null;
			}
			
			int r = (int)(background.R * 255.5);
			int g = (int)(background.G * 255.5);
			int b = (int)(background.B * 255.5);
			
			ImageSeed seed = new ImageSeed (r, g, b, image.UniqueId);
			
			lock (Bitmap.disabled_images)
			{
				if (Bitmap.disabled_images.Contains (seed))
				{
					return Bitmap.disabled_images[seed] as Bitmap;
				}
				
				System.Drawing.Color  color = System.Drawing.Color.FromArgb (r, g, b);
				
				System.Drawing.Bitmap src_bitmap = image.BitmapImage.bitmap;
				System.Drawing.Bitmap dst_bitmap = new System.Drawing.Bitmap (src_bitmap.Width, src_bitmap.Height);
				
				Platform.ImageDisabler.Paint (src_bitmap, dst_bitmap, color);
				
				Bitmap bitmap = new Bitmap ();
				
				bitmap.bitmap			 = dst_bitmap;
				bitmap.size				 = image.Size;
				bitmap.origin			 = image.Origin;
				bitmap.is_origin_defined = image.IsOriginDefined;
				
				Bitmap.disabled_images[seed] = bitmap;
				
				return bitmap;
			}
		}
		
		public static Bitmap FromLargerImage(Image image, Rectangle clip)
		{
			Bitmap bitmap = Bitmap.FromLargerImage (image, clip, Point.Empty);
			bitmap.is_origin_defined = false;
			return bitmap;
		}
		
		public static Bitmap FromLargerImage(Image image, Rectangle clip, Point origin)
		{
			if (image == null)
			{
				return null;
			}
			
			System.Drawing.Bitmap src_bitmap = image.BitmapImage.bitmap;
			
			int dx = (int)(clip.Width  + 0.5);
			int dy = (int)(clip.Height + 0.5);
			int x  = (int)(clip.Left);
			int y  = (int)(clip.Bottom);
			int yy = src_bitmap.Height - dy - y;
			
			System.Drawing.Bitmap dst_bitmap = new System.Drawing.Bitmap (dx, dy);
			
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dst_bitmap))
			{
				graphics.DrawImage (src_bitmap, 0, 0, new System.Drawing.Rectangle (x, yy, dx, dy), System.Drawing.GraphicsUnit.Pixel);
			}
			
			Bitmap bitmap = new Bitmap ();
			
			double sx = image.Width  / src_bitmap.Width;
			double sy = image.Height / src_bitmap.Height;
			
			bitmap.bitmap			 = dst_bitmap;
			bitmap.size				 = new Size (sx * dx, sy * dy);
			bitmap.origin			 = origin;
			bitmap.is_origin_defined = true;
			
			return bitmap;
		}
		
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.bitmap != null)
				{
					this.bitmap.Dispose ();
				}
				
				this.bitmap = null;
				this.bitmap_data = null;
				this.bitmap_lock_count = 0;
			}
		}
		
		
		
		private class ImageSeed
		{
			public ImageSeed(int r, int g, int b, long id)
			{
				this.r  = r;
				this.g  = g;
				this.b  = b;
				this.id = id;
			}
			
			public override int GetHashCode()
			{
				return this.r ^ this.g ^ this.b ^ this.id.GetHashCode ();
			}
			
			public override bool Equals(object obj)
			{
				ImageSeed that = obj as ImageSeed;
				
				if (that != null)
				{
					if ((this.r == that.r) &&
						(this.g == that.g) &&
						(this.b == that.b) &&
						(this.id == that.id))
					{
						return true;
					}
				}
				
				return false;
			}
			
			private int							r, g, b;
			private long						id;
		}
		
		
		protected System.Drawing.Bitmap			bitmap;
		protected BitmapData					bitmap_data;
		protected volatile int					bitmap_lock_count;
		
		static System.Collections.Hashtable		disabled_images = new System.Collections.Hashtable ();
	}
}
