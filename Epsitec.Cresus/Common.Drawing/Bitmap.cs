namespace Epsitec.Common.Drawing
{
	using BitmapData = System.Drawing.Imaging.BitmapData;
	
	/// <summary>
	/// La classe Bitmap encapsule une image de type bitmap.
	/// </summary>
	public class Bitmap : Image
	{
		public Bitmap()
		{
		}
		
		
		public override void DefineZoom(double zoom)
		{
			//	Le zoom de l'appelant ne joue aucun rôle... La définition de
			//	l'image est fixe.
		}
		
		public override void DefineColor(Drawing.Color color)
		{
		}
		
		public override void DefineAdorner(object adorner)
		{
		}
		
		public override void MergeTransform(Transform transform)
		{
			//	Fusionne la transformation spécifiée avec la transformation propre à l'image
			//	(changement d'échelle pour que la taille logique soit respectée).
			
			if (this.bitmap != null)
			{
				//	Il se peut que le bitmap définisse une échelle interne (la taille logique ne
				//	correspond pas à la taille exprimée en pixels). Dans ce cas, il faut modifier
				//	la matrice de transformation pour que le dessin ait la taille logique, et pas
				//	la taille physique :
					
				double sx = this.PixelWidth / this.Width;
				double sy = this.PixelHeight / this.Height;
					
				if ((sx != 1) &&
					(sy != 1))
				{
					transform.Scale (1/sx, 1/sy);
				}
			}
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
		
		
		public bool						IsValid
		{
			get { return this.bitmap != null; }
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
		
		
		public override Image GetImageForPaintStyle(GlyphPaintStyle style)
		{
			switch (style)
			{
				case GlyphPaintStyle.Normal:
					return this;
				case GlyphPaintStyle.Disabled:
					return Bitmap.FromImageDisabled (this, Color.FromName ("Control"));
			}
			
			return null;
		}
		
		public static Image FromNativeBitmap(System.Drawing.Bitmap native)
		{
			Image bitmap = Bitmap.FromNativeBitmap (native, new Point (0, 0));
			bitmap.is_origin_defined = false;
			return bitmap;
		}
		
		public static Image FromNativeBitmap(System.Drawing.Bitmap native, Point origin)
		{
			return Bitmap.FromNativeBitmap (native, origin, Size.Empty);
		}
		
		public static Image FromNativeBitmap(System.Drawing.Bitmap native, Point origin, Size size)
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
		
		public static Image FromData(byte[] data)
		{
			Image bitmap = Bitmap.FromData (data, new Point (0, 0));
			bitmap.is_origin_defined = false;
			return bitmap;
		}
		
		public static Image FromData(byte[] data, Point origin)
		{
			return Bitmap.FromData (data, origin, Size.Empty);
		}
		
		public static Image FromData(byte[] data, Point origin, Size size)
		{
			//	Avant de passer les données brutes à .NET pour en extraire l'image de
			//	format PNG/TIFF/JPEG, on regarde s'il ne s'agit pas d'un format "maison".
			
			if (data.Length > 40)
			{
				if ((data[0] == (byte) '<') &&
					(data[1] == (byte) '?') &&
					(data[2] == (byte) 'x'))
				{
					//	Il y a de très fortes chances que ce soit une image vectorielle définie
					//	au moyen du format interne propre à EPSITEC.
					
					return new Canvas (data);
				}
			}
			
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream (data, false))
			{
				System.Drawing.Bitmap src_bitmap = new System.Drawing.Bitmap (stream);
				System.Drawing.Bitmap dst_bitmap = new System.Drawing.Bitmap (src_bitmap.Width, src_bitmap.Height);
				
				double dpi_x = src_bitmap.HorizontalResolution;
				double dpi_y = dst_bitmap.VerticalResolution;
				
				src_bitmap.SetResolution (dst_bitmap.HorizontalResolution, dst_bitmap.VerticalResolution);
				
				using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dst_bitmap))
				{
					graphics.DrawImageUnscaled (src_bitmap, 0, 0, src_bitmap.Width, src_bitmap.Height);
				}
				
				Image image = Bitmap.FromNativeBitmap (dst_bitmap, origin, size);
				
				if (image != null)
				{
					image.dpi_x = dpi_x;
					image.dpi_y = dpi_y;
				}
				
				return image;
			}
		}
		
		public static Image FromFile(string file_name)
		{
			Image bitmap = Bitmap.FromFile (file_name, new Point (0, 0));
			bitmap.is_origin_defined = false;
			return bitmap;
		}
		
		public static Image FromFile(string file_name, Point origin)
		{
			return Bitmap.FromFile (file_name, origin, Size.Empty);
		}
		
		public static Image FromFile(string file_name, Point origin, Size size)
		{
			using (System.IO.FileStream file = new System.IO.FileStream (file_name, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				long   length = file.Length;
				byte[] buffer = new byte[length];
				file.Read (buffer, 0, (int) length);
				
				return Bitmap.FromData (buffer, origin, size);
			}
		}
		
		public static Image FromManifestResource(string resource_name, System.Reflection.Assembly assembly)
		{
			Image bitmap = Bitmap.FromManifestResource (resource_name, assembly, new Point (0, 0));
			bitmap.is_origin_defined = false;
			return bitmap;
		}
		
		public static Image FromManifestResource(string resource_name, System.Reflection.Assembly assembly, Point origin)
		{
			return Bitmap.FromManifestResource (resource_name, assembly, origin, Size.Empty);
		}
		
		public static Image FromManifestResource(string resource_name, System.Reflection.Assembly assembly, Point origin, Size size)
		{
			using (System.IO.Stream stream = assembly.GetManifestResourceStream (resource_name))
			{
				long   length = stream.Length;
				byte[] buffer = new byte[length];
				stream.Read (buffer, 0, (int) length);
				
				return Bitmap.FromData (buffer, origin, size);
			}
		}
		
		public static Image FromImageDisabled(Image image, Color background)
		{
			System.Diagnostics.Debug.Assert (image != null);
			
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
		
		public static Image CopyImage(Image image)
		{
			if (image == null)
			{
				return null;
			}
			
			System.Drawing.Bitmap src_bitmap = image.BitmapImage.bitmap;
			System.Drawing.Bitmap dst_bitmap = new System.Drawing.Bitmap (src_bitmap.Width, src_bitmap.Height);
			
			double dpi_x = src_bitmap.HorizontalResolution;
			double dpi_y = dst_bitmap.VerticalResolution;
				
			src_bitmap.SetResolution (dst_bitmap.HorizontalResolution, dst_bitmap.VerticalResolution);
				
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dst_bitmap))
			{
				graphics.DrawImageUnscaled (src_bitmap, 0, 0, src_bitmap.Width, src_bitmap.Height);
			}
			
			Bitmap bitmap = new Bitmap ();
			
			bitmap.bitmap			 = dst_bitmap;
			bitmap.size				 = image.Size;
			bitmap.origin			 = image.Origin;
			bitmap.is_origin_defined = image.IsOriginDefined;
			
			bitmap.dpi_x = dpi_x;
			bitmap.dpi_y = dpi_y;
			
			return bitmap;
		}
		
		public static Image FromLargerImage(Image image, Rectangle clip)
		{
			Image bitmap = Bitmap.FromLargerImage (image, clip, Point.Empty);
			bitmap.is_origin_defined = false;
			return bitmap;
		}
		
		public static Image FromLargerImage(Image image, Rectangle clip, Point origin)
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
		
		
		protected override void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.Assert (this.is_disposed == false);
			
			this.is_disposed = true;
			
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
			
			base.Dispose (disposing);
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
		
		protected bool							is_disposed;
		
		static System.Collections.Hashtable		disabled_images = new System.Collections.Hashtable ();
	}
}
