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
		
		
		public static Bitmap FromFile(string file_name)
		{
			Bitmap bitmap = Bitmap.FromFile (file_name, new Point (0, 0));
			
			bitmap.is_origin_defined = false;
			
			return bitmap;
		}
		
		public static Bitmap FromFile(string file_name, Point origin)
		{
			Bitmap bitmap = new Bitmap ();
			
			bitmap.bitmap = System.Drawing.Image.FromFile (file_name) as System.Drawing.Bitmap;
			bitmap.size   = new Size (bitmap.bitmap.Width, bitmap.bitmap.Height);
			bitmap.origin = origin;
			
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
		
		
		protected System.Drawing.Bitmap	bitmap;
		protected BitmapData			bitmap_data;
		protected volatile int			bitmap_lock_count;
	}
}
