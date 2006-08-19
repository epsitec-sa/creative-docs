//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		
		public override Bitmap					BitmapImage
		{
			get
			{
				return this;
			}
		}
		
		public System.Drawing.Bitmap			NativeBitmap
		{
			get
			{
				if (this.bitmap == null)
				{
					if ((this.IsLocked) &&
						(this.Stride != 0) &&
						(this.Scan0 != System.IntPtr.Zero) &&
						(this.bitmap_dx > 0) &&
						(this.bitmap_dy > 0))
					{
						//	Des pixels sont définis, mais pas d'objet bitmap natif. On peut donc créer ici le bitmap
						//	en se basant sur ces pixels :
						
						return new System.Drawing.Bitmap (this.bitmap_dx, this.bitmap_dy, this.Stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, this.Scan0);
					}
				}
				
				return this.bitmap;
			}
		}
		
		public bool								IsLocked
		{
			get
			{
				return this.bitmap_data != null;
			}
		}
		
		public System.IntPtr					Scan0
		{
			get
			{
				if (this.bitmap_data == null)
				{
					return System.IntPtr.Zero;
				}
				
				return this.bitmap_data.Scan0;
			}
		}
		
		public int								Stride
		{
			get
			{
				if (this.bitmap_data == null)
				{
					return 0;
				}
				
				return this.bitmap_data.Stride;
			}
		}
		
		
		public bool								IsValid
		{
			get
			{
				return this.bitmap != null;
			}
		}

		public int								PixelWidth
		{
			get
			{
				return this.bitmap_dx;
			}
		}
		
		public int								PixelHeight
		{
			get
			{
				return this.bitmap_dy;
			}
		}
		
		
		public static ICanvasFactory			CanvasFactory
		{
			get
			{
				return Bitmap.canvas_factory;
			}
			set
			{
				Bitmap.canvas_factory = value;
			}
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
		
		
		public bool LockBits()
		{
			lock (this)
			{
				if (this.bitmap_data == null)
				{
					System.Drawing.Imaging.ImageLockMode mode = System.Drawing.Imaging.ImageLockMode.ReadOnly;
					System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
					
					int width  = this.bitmap_dx;
					int height = this.bitmap_dy;
					
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
		
		
		public void FlipY()
		{
			try
			{
				this.LockBits ();
				
				int nx = this.bitmap_dx / 2;
				int ny = this.bitmap_dy / 2;
				int my = this.bitmap_dy - 1;
				
				unsafe
				{
					for (int y = 0; y < ny; y++)
					{
						int y1 = y;
						int y2 = my-y;
						int* s1 = (int*) (this.bitmap_data.Scan0.ToPointer ()) + y1 * this.bitmap_data.Stride / 4;
						int* s2 = (int*) (this.bitmap_data.Scan0.ToPointer ()) + y2 * this.bitmap_data.Stride / 4;
						
						for (int x = 0; x < this.bitmap_dx; x++)
						{
							int v1 = s1[x];
							int v2 = s2[x];
							s1[x] = v2;
							s2[x] = v1;
						}
					}
				}
			}
			finally
			{
				this.UnlockBits ();
			}
		}

		public byte[] Save(ImageFormat format)
		{
			return this.Save (format, 0);
		}

		public byte[] Save(ImageFormat format, int depth)
		{
			int quality = 0;
			ImageCompression compression = ImageCompression.None;
			
			switch (format)
			{
				case ImageFormat.Bmp:
				case ImageFormat.Png:
					if (depth == 0)
					{
						depth = 32;
					}
					break;

				case ImageFormat.Gif:
					break;
				
				case ImageFormat.Tiff:
					if (depth == 0)
					{
						depth = 32;
					}
					compression = ImageCompression.Lzw;
					break;
				
				case ImageFormat.Jpeg:
					depth = 24;
					quality = 75;
					break;

				case ImageFormat.WindowsIcon:
					break;
				
				default:
					throw new System.ArgumentException ("Invalid format specified", "format");
			}
			
			return this.Save (format, depth, quality, compression);
		}
		
		public byte[] Save(ImageFormat format, int depth, int quality, ImageCompression compression)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (format == ImageFormat.Jpeg)
			{
				System.Drawing.Imaging.Encoder          encoder   = System.Drawing.Imaging.Encoder.Quality;
				System.Drawing.Imaging.EncoderParameter parameter = new System.Drawing.Imaging.EncoderParameter (encoder, (long)(quality));
				
				list.Add (parameter);
			}
			
			if (format == ImageFormat.Tiff)
			{
				System.Drawing.Imaging.EncoderValue value = System.Drawing.Imaging.EncoderValue.CompressionNone;
				
				switch (compression)
				{
					case ImageCompression.Lzw:			value = System.Drawing.Imaging.EncoderValue.CompressionLZW;		break;
					case ImageCompression.FaxGroup3:	value = System.Drawing.Imaging.EncoderValue.CompressionCCITT3;	break;
					case ImageCompression.FaxGroup4:	value = System.Drawing.Imaging.EncoderValue.CompressionCCITT4;	break;
					case ImageCompression.Rle:			value = System.Drawing.Imaging.EncoderValue.CompressionRle;		break;
				}
				
				System.Drawing.Imaging.Encoder          encoder   = System.Drawing.Imaging.Encoder.Compression;
				System.Drawing.Imaging.EncoderParameter parameter = new System.Drawing.Imaging.EncoderParameter (encoder, (long)(value));
				
				list.Add (parameter);
			}
			
			if ((format == ImageFormat.Bmp) ||
				(format == ImageFormat.Png) ||
				(format == ImageFormat.Tiff))
			{
				System.Drawing.Imaging.Encoder          encoder   = System.Drawing.Imaging.Encoder.ColorDepth;
				System.Drawing.Imaging.EncoderParameter parameter = new System.Drawing.Imaging.EncoderParameter (encoder, (long)(depth));
				
				list.Add (parameter);
			}
			
			if (format == ImageFormat.WindowsIcon)
			{
				System.IO.MemoryStream stream = new System.IO.MemoryStream ();
				System.Drawing.Icon icon = System.Drawing.Icon.FromHandle (this.NativeBitmap.GetHicon ());
				icon.Save (stream);
				stream.Close ();
				icon.Dispose ();
				
				return stream.ToArray ();
			}
			
			System.Drawing.Imaging.ImageCodecInfo    encoder_info    = Bitmap.GetCodecInfo (format);
			System.Drawing.Imaging.EncoderParameters encoder_params  = new System.Drawing.Imaging.EncoderParameters (list.Count);
			
			for (int i = 0; i < list.Count; i++)
			{
				encoder_params.Param[i] = list[i] as System.Drawing.Imaging.EncoderParameter;
			}
			
			if (encoder_info != null)
			{
				System.IO.MemoryStream stream = new System.IO.MemoryStream ();
				this.NativeBitmap.Save (stream, encoder_info, encoder_params);
				stream.Close ();
				
				return stream.ToArray ();
			}
			
			return null;
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
		
		
		public static Image FromPixmap(Pixmap pixmap)
		{
			Bitmap bitmap = new Bitmap ();
			
			bitmap.pixmap    = pixmap;
			bitmap.bitmap    = null;
			bitmap.bitmap_dx = pixmap.Size.Width;
			bitmap.bitmap_dy = pixmap.Size.Height;
			bitmap.size      = new Size (bitmap.bitmap_dx, bitmap.bitmap_dy);
			bitmap.origin    = new Point (0, 0);
			
			//	Prétend que le bitmap est verrouillé, puisqu'on a de toute façons déjà accès aux
			//	pixels (c'est d'ailleurs bien la seule chose qu'on a) :
			
			bitmap.bitmap_lock_count = 1;
			bitmap.is_origin_defined = true;
			
			int dx, dy, stride;
			System.IntPtr pixels;
			System.Drawing.Imaging.PixelFormat format;
			
			pixmap.GetMemoryLayout (out dx, out dy, out stride, out format, out pixels);
			
			bitmap.bitmap_data = new BitmapData ();
			
			bitmap.bitmap_data.Width       = dx;
			bitmap.bitmap_data.Height      = dy;
			bitmap.bitmap_data.PixelFormat = format;
			bitmap.bitmap_data.Scan0       = pixels;
			bitmap.bitmap_data.Stride      = stride;
			
			return bitmap;
		}
		
		public static Image FromNativeBitmap(int dx, int dy)
		{
			return Bitmap.FromNativeBitmap (new System.Drawing.Bitmap (dx, dy));
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
			
			bitmap.bitmap    = native;
			bitmap.bitmap_dx = native.Width;
			bitmap.bitmap_dy = native.Height;
			bitmap.size      = size;
			bitmap.origin    = origin;
			
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
					(data[1] == (byte) '?'))
				{
					//	Il y a de très fortes chances que ce soit une image vectorielle définie
					//	au moyen du format interne propre à EPSITEC.
					
					if (Bitmap.canvas_factory != null)
					{
						return Bitmap.canvas_factory.CreateCanvas (data);
					}
					else
					{
						return null;
					}
				}
			}
			
			//	If the data we get is an EMF/WMF file (it should have the D7CDC69A header),
			//	then use the dedicated Metafile class to read and render it. This preserves
			//	the resolution.
			
			if ((data.Length > 4) &&
				(data[0] == 0xD7) &&
				(data[1] == 0xCD) &&
				(data[2] == 0xC6) &&
				(data[3] == 0x9A))
			{
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream (data, false))
				{
					System.Drawing.Imaging.Metafile metafile   = new System.Drawing.Imaging.Metafile (stream);
					System.Drawing.Bitmap           dst_bitmap = new System.Drawing.Bitmap (metafile.Width, metafile.Height);
					
					double dpi_x = metafile.HorizontalResolution;
					double dpi_y = metafile.VerticalResolution;
					
					using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dst_bitmap))
					{
						graphics.DrawImage (metafile, 0, 0, metafile.Width, metafile.Height);
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
			else
			{
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream (data, false))
				{
					System.Drawing.Bitmap src_bitmap = new System.Drawing.Bitmap (stream);
					System.Drawing.Bitmap dst_bitmap = new System.Drawing.Bitmap (src_bitmap.Width, src_bitmap.Height);
					
					double dpi_x = src_bitmap.HorizontalResolution;
					double dpi_y = src_bitmap.VerticalResolution;
					
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
		
		public static Image FromManifestResource(string namespace_name, string resource_name, System.Type type)
		{
			return Bitmap.FromManifestResource (string.Concat (namespace_name, ".", resource_name), type.Assembly);
		}
		
		public static Image FromManifestResource(string resource_name, System.Reflection.Assembly assembly)
		{
			Image bitmap = Bitmap.FromManifestResource (resource_name, assembly, new Point (0, 0));
			
			if (bitmap != null)
			{
				bitmap.is_origin_defined = false;
			}
			
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
				bitmap.bitmap_dx         = dst_bitmap.Width;
				bitmap.bitmap_dy         = dst_bitmap.Height;
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
			bitmap.bitmap_dx         = dst_bitmap.Width;
			bitmap.bitmap_dy         = dst_bitmap.Height;
			bitmap.size				 = image.Size;
			bitmap.origin			 = image.Origin;
			bitmap.is_origin_defined = image.IsOriginDefined;
			
			bitmap.dpi_x = dpi_x;
			bitmap.dpi_y = dpi_y;
			
			return bitmap;
		}
		
		public static Image FromLargerImage(Image image, Rectangle clip)
		{
			Image bitmap = Bitmap.FromLargerImage (image, clip, Point.Zero);
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
			bitmap.bitmap_dx         = dst_bitmap.Width;
			bitmap.bitmap_dy         = dst_bitmap.Height;
			bitmap.size				 = new Size (sx * dx, sy * dy);
			bitmap.origin			 = origin;
			bitmap.is_origin_defined = true;
			
			return bitmap;
		}
		
		
		public static ImageFormat MapFromMicrosoftImageFormat(System.Drawing.Imaging.ImageFormat format)
		{
			if (format == System.Drawing.Imaging.ImageFormat.Bmp)	return ImageFormat.Bmp;
			if (format == System.Drawing.Imaging.ImageFormat.Gif)	return ImageFormat.Gif;
			if (format == System.Drawing.Imaging.ImageFormat.Png)	return ImageFormat.Png;
			if (format == System.Drawing.Imaging.ImageFormat.Tiff)	return ImageFormat.Tiff;
			if (format == System.Drawing.Imaging.ImageFormat.Jpeg)	return ImageFormat.Jpeg;
			if (format == System.Drawing.Imaging.ImageFormat.Exif)	return ImageFormat.Exif;
			if (format == System.Drawing.Imaging.ImageFormat.Icon)	return ImageFormat.WindowsIcon;
			if (format == System.Drawing.Imaging.ImageFormat.Emf)	return ImageFormat.WindowsEmf;
			if (format == System.Drawing.Imaging.ImageFormat.Wmf)	return ImageFormat.WindowsWmf;
			
			return ImageFormat.Unknown;
		}
		
		public static System.Drawing.Imaging.ImageFormat MapToMicrosoftImageFormat(ImageFormat format)
		{
			switch (format)
			{
				case ImageFormat.Bmp:			return System.Drawing.Imaging.ImageFormat.Bmp;
				case ImageFormat.Gif:			return System.Drawing.Imaging.ImageFormat.Gif;
				case ImageFormat.Png:			return System.Drawing.Imaging.ImageFormat.Png;
				case ImageFormat.Tiff:			return System.Drawing.Imaging.ImageFormat.Tiff;
				case ImageFormat.Jpeg:			return System.Drawing.Imaging.ImageFormat.Jpeg;
				case ImageFormat.Exif:			return System.Drawing.Imaging.ImageFormat.Exif;
				case ImageFormat.WindowsIcon:	return System.Drawing.Imaging.ImageFormat.Icon;
				case ImageFormat.WindowsEmf:	return System.Drawing.Imaging.ImageFormat.Emf;
				case ImageFormat.WindowsWmf:	return System.Drawing.Imaging.ImageFormat.Wmf;
			}
			
			return null;
		}
		
		public static System.Drawing.Imaging.ImageCodecInfo GetCodecInfo(ImageFormat format)
		{
			string mime = null;
			
			switch (format)
			{
				case ImageFormat.Bmp:	mime = "image/bmp";		break;
				case ImageFormat.Gif:	mime = "image/gif";		break;
				case ImageFormat.Png:	mime = "image/png";		break;
				case ImageFormat.Tiff:	mime = "image/tiff";	break;
				case ImageFormat.Jpeg:	mime = "image/jpeg";	break;
			}
			
			if (mime == null)
			{
				return null;
			}
			
			System.Drawing.Imaging.ImageCodecInfo[] encoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders ();
			
			for (int i = 0; i < encoders.Length; i++)
			{
				if (encoders[i].MimeType == mime)
				{
					return encoders[i];
				}
			}
			
			return null;
		}
		
		public static string[] GetFilenameExtensions(ImageFormat format)
		{
			System.Drawing.Imaging.ImageCodecInfo info = Bitmap.GetCodecInfo (format);
			
			if (info != null)
			{
				return info.FilenameExtension.Split (';');
			}
			
			return null;
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
		
		
		#region ImageSeed Class
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
		#endregion
		
		protected System.Drawing.Bitmap			bitmap;
		protected int							bitmap_dx;
		protected int							bitmap_dy;
		protected BitmapData					bitmap_data;
		protected volatile int					bitmap_lock_count;
		protected Pixmap						pixmap;
		
		protected bool							is_disposed;
		
		static System.Collections.Hashtable		disabled_images = new System.Collections.Hashtable ();
		static ICanvasFactory					canvas_factory;
	}
}
