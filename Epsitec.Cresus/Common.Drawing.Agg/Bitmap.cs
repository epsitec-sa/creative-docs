//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.InteropServices;

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
						//	Des pixels sont d�finis, mais pas d'objet bitmap natif. On peut donc cr�er ici le bitmap
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

		public byte[]							GetRawBitmapBytes()
		{
			this.LockBits ();

			try
			{
				if (this.bitmap_data != null)
				{
					System.IntPtr memory = this.Scan0;
					int size = this.bitmap_data.Height * this.bitmap_data.Stride;

					if ((memory != System.IntPtr.Zero) &&
						(size > 0))
					{
						byte[] data = new byte[size];
						Marshal.Copy (memory, data, 0, size);
						return data;
					}
				}
			}
			finally
			{
				this.UnlockBits ();
			}

			return null;
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
			//	Le zoom de l'appelant ne joue aucun r�le... La d�finition de
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
			//	Fusionne la transformation sp�cifi�e avec la transformation propre � l'image
			//	(changement d'�chelle pour que la taille logique soit respect�e).
			
			if (this.bitmap != null)
			{
				//	Il se peut que le bitmap d�finisse une �chelle interne (la taille logique ne
				//	correspond pas � la taille exprim�e en pixels). Dans ce cas, il faut modifier
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

		static System.Collections.Generic.Dictionary<System.Drawing.Bitmap, System.Drawing.Imaging.BitmapData> lockedBitmapDataCache = new System.Collections.Generic.Dictionary<System.Drawing.Bitmap, System.Drawing.Imaging.BitmapData> ();
		
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
					
					lock (lockedBitmapDataCache)
					{
						System.Diagnostics.Debug.Assert (!Bitmap.lockedBitmapDataCache.ContainsKey (this.bitmap));
					}

					lock (Bitmap.lockedBitmapDataCache)
					{
						int  attempt = 0;
						bool success = false;

						while ((success == false) && (attempt++ < 10))
						{
							try
							{
								this.bitmap_data = this.bitmap.LockBits (new System.Drawing.Rectangle (0, 0, width, height), mode, format);
								success = true;
							}
							catch (System.Exception)
							{
								System.Diagnostics.Debug.WriteLine ("Attempted to lock bitmap and failed: " + attempt);
								System.Threading.Thread.Sleep (1);
							}
						}

						Bitmap.lockedBitmapDataCache[this.bitmap] = this.bitmap_data;
					}
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
						
						lock (Bitmap.lockedBitmapDataCache)
						{
							Bitmap.lockedBitmapDataCache.Remove (this.bitmap);
						}
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
			return this.Save (format, depth, quality, compression, 72.0);
		}
		
		public byte[] Save(ImageFormat format, int depth, int quality, ImageCompression compression, double dpi)
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
				using (Opac.FreeImage.Image image = Opac.FreeImage.NativeBitmap.ConvertToImage (this.NativeBitmap))
				{
					return Opac.FreeImage.NativeIcon.CreateIcon (image);
				}
			}
			
			if (format == ImageFormat.WindowsVistaIcon)
			{
				using (Opac.FreeImage.Image image = Opac.FreeImage.NativeBitmap.ConvertToImage (this.NativeBitmap))
				{
					return Opac.FreeImage.NativeIcon.CreateHighResolutionIcon (image);
				}
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
				System.Drawing.Bitmap  bitmap = this.NativeBitmap;
				
				bitmap.SetResolution ((float) dpi, (float) dpi);
				bitmap.Save (stream, encoder_info, encoder_params);
				stream.Close ();
				
				byte[] data = stream.ToArray ();

				if (data == null)
				{
					return null;
				}

				if (format == ImageFormat.Png)
				{
					//	FreeImage does a better job than Windows for the PNG compression.

					using (Opac.FreeImage.Image fi = Opac.FreeImage.Image.Load (data))
					{
						switch (depth)
						{
							case 24:
								using (Opac.FreeImage.Image fi24 = fi.ConvertTo24Bits ())
								{
									fi24.SetDotsPerInch (dpi, dpi);
									data = fi24.SaveToMemory (Opac.FreeImage.FileFormat.Png);
								}
								break;
							case 32:
								using (Opac.FreeImage.Image fi32 = fi.ConvertTo32Bits ())
								{
									fi32.SetDotsPerInch (dpi, dpi);
									data = fi32.SaveToMemory (Opac.FreeImage.FileFormat.Png);
								}
								break;
						}
					}
				}
				
				return data;
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

		public static Image FromImage(Opac.FreeImage.Image image)
		{
			if (image == null)
			{
				return null;
			}

			Pixmap pixmap = new Pixmap ();
			pixmap.AllocatePixmap (image, true);
			
			return Bitmap.FromPixmap (pixmap);
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
			
			//	Pr�tend que le bitmap est verrouill�, puisqu'on a de toute fa�ons d�j� acc�s aux
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

		public static Image FromNativeIcon(PlatformSystemIcon systemIcon)
		{
			System.Drawing.Icon icon = null;

			switch (systemIcon)
			{
				case PlatformSystemIcon.Application:
					icon = System.Drawing.SystemIcons.Application;
					break;

				case PlatformSystemIcon.Asterisk:
					icon = System.Drawing.SystemIcons.Asterisk;
					break;

				case PlatformSystemIcon.Error:
					icon = System.Drawing.SystemIcons.Error;
					break;

				case PlatformSystemIcon.Exclamation:
					icon = System.Drawing.SystemIcons.Exclamation;
					break;

				case PlatformSystemIcon.Hand:
					icon = System.Drawing.SystemIcons.Hand;
					break;

				case PlatformSystemIcon.Information:
					icon = System.Drawing.SystemIcons.Information;
					break;

				case PlatformSystemIcon.Question:
					icon = System.Drawing.SystemIcons.Question;
					break;

				case PlatformSystemIcon.Shield:
					icon = System.Drawing.SystemIcons.Shield;
					break;

				case PlatformSystemIcon.Warning:
					icon = System.Drawing.SystemIcons.Warning;
					break;

				case PlatformSystemIcon.WinLogo:
					icon = System.Drawing.SystemIcons.WinLogo;
					break;
			}

			if (icon != null)
			{
				return Bitmap.FromNativeIcon (icon);
			}
			else
			{
				return null;
			}
		}
		
		public static Image FromNativeIcon(System.Drawing.Icon native)
		{
			System.Drawing.Bitmap src_bitmap = native.ToBitmap ();
			System.Drawing.Bitmap dst_bitmap;

			double dpi_x = src_bitmap.HorizontalResolution;
			double dpi_y = src_bitmap.VerticalResolution;
			
			int dx = src_bitmap.Width;
			int dy = src_bitmap.Height;

			dst_bitmap = Bitmap.ConvertIcon (native.Handle, dx, dy);

			Bitmap bitmap = new Bitmap ();

			bitmap.bitmap			 = dst_bitmap;
			bitmap.bitmap_dx         = dx;
			bitmap.bitmap_dy         = dy;
			bitmap.size				 = new Size (dx, dy);
			bitmap.origin			 = new Point (0, 0);
			bitmap.is_origin_defined = false;

			bitmap.dpi_x = dpi_x;
			bitmap.dpi_y = dpi_y;

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
			//	Avant de passer les donn�es brutes � .NET pour en extraire l'image de
			//	format PNG/TIFF/JPEG, on regarde s'il ne s'agit pas d'un format "maison".
			
			if (data.Length > 40)
			{
				if ((data[0] == (byte) '<') &&
					(data[1] == (byte) '?'))
				{
					//	Il y a de tr�s fortes chances que ce soit une image vectorielle d�finie
					//	au moyen du format interne propre � EPSITEC.
					
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

					double dpi_x = metafile.HorizontalResolution;
					double dpi_y = metafile.VerticalResolution;
					
					int dx = metafile.Width;
					int dy = metafile.Height;

					int bitmapWidth = dx;
					int bitmapHeight = dy;

					//	Don't allocate a pixmap with more than 100 Mpixels... which is
					//	already really huge.
					
					while ((bitmapWidth*bitmapHeight > 100*1000*1000)
						&& (dpi_x * dpi_y > 100*100))
					{
						bitmapWidth  /= 2;
						bitmapHeight /= 2;
						dpi_x /= 2;
						dpi_y /= 2;
					}

					if ((bitmapWidth != dx) ||
						(bitmapHeight != dy))
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Reduced WMF size from {0}x{1} to {2}x{3} pixels", dx, dy, bitmapWidth, bitmapHeight));
					}
					
					System.Drawing.Bitmap dst_bitmap;

					try
					{
						dst_bitmap = new System.Drawing.Bitmap (bitmapWidth, bitmapHeight);
					}
					catch
					{
						dst_bitmap = null;
					}
					
					
					using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dst_bitmap))
					{
						graphics.DrawImage (metafile, 0, 0, bitmapWidth, bitmapHeight);
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
				for (int attempt = 0; attempt < 10; attempt++)
				{
					try
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
					catch (System.OutOfMemoryException)
					{
						System.Diagnostics.Debug.WriteLine ("Out of memory in GDI - attempt " + attempt);
						System.GC.Collect ();
						System.Threading.Thread.Sleep (1);
					}
				}

				return null;
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
				if (stream == null)
				{
					return null;
				}

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
			double dpi_y = src_bitmap.VerticalResolution;
				
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

		private static System.Drawing.Bitmap ConvertIcon(System.IntPtr hIcon, int dx, int dy)
		{
			BITMAPV5HEADER bmpInfo32 = new BITMAPV5HEADER ();
			const int BI_BITFIELDS = 3;

			bmpInfo32.bV5Size     = Marshal.SizeOf (bmpInfo32);
			bmpInfo32.bV5Width    = dx;
			bmpInfo32.bV5Height   = dy;
			bmpInfo32.bV5Planes   = 1;
			bmpInfo32.bV5BitCount = 32;
			bmpInfo32.bV5Compression = BI_BITFIELDS;
			bmpInfo32.bV5RedMask   = 0x00ff0000;
			bmpInfo32.bV5GreenMask = 0x0000ff00;
			bmpInfo32.bV5BlueMask  = 0x000000ff;
			bmpInfo32.bV5AlphaMask = 0xff000000;

			System.IntPtr bits;
			System.IntPtr dc = Bitmap.CreateCompatibleDC (System.IntPtr.Zero);
			System.IntPtr bmp = Bitmap.CreateDIBSection (dc, ref bmpInfo32, 0, out bits, System.IntPtr.Zero, 0);
			System.IntPtr hbOld = Bitmap.SelectObject (dc, bmp);
			System.IntPtr brush = Bitmap.CreateSolidBrush (0xffffff);

			Bitmap.DrawIconEx (dc, 0, 0, hIcon, 0, 0, 0, brush, DI_NORMAL);
			Bitmap.DeleteObject (brush);

			System.Drawing.Bitmap bitmap = System.Drawing.Bitmap.FromHicon (hIcon);

			System.Drawing.Imaging.ImageLockMode mode = System.Drawing.Imaging.ImageLockMode.ReadOnly;
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
			System.Drawing.Imaging.BitmapData data = bitmap.LockBits (new System.Drawing.Rectangle (0, 0, dx, dy), mode, format);

			unsafe
			{
				byte* rawBytes = (byte*) bits.ToPointer ();
				byte* gdiBytes = (byte*) data.Scan0.ToPointer ();

				gdiBytes += data.Stride * dy;

				for (int y = 0; y < dy; y++)
				{
					gdiBytes -= data.Stride;

					for (int x = 0; x < dx; x++)
					{
						byte bB = gdiBytes[x*4+0];
						byte bG = gdiBytes[x*4+1];
						byte bR = gdiBytes[x*4+2];

						byte cB = *rawBytes++;
						byte cG = *rawBytes++;
						byte cR = *rawBytes++;
						byte cA = *rawBytes++;

						if (cA != 0)
						{
							gdiBytes[x*4+0] = cB;
							gdiBytes[x*4+1] = cG;
							gdiBytes[x*4+2] = cR;
							gdiBytes[x*4+3] = cA;
						}
					}
				}
			}

			bitmap.UnlockBits (data);

			Bitmap.SelectObject (dc, hbOld);
			Bitmap.DeleteObject (bmp);
			Bitmap.DeleteDC (dc);

			return bitmap;
		}

		#region Private Interop Definitions

		[StructLayout (LayoutKind.Sequential)]
		private struct ICONINFO
		{
			public bool fIcon;
			public System.Int32 xHotspot;
			public System.Int32 yHotspot;
			public System.IntPtr hbmMask;
			public System.IntPtr hbmColor;
		}

		[StructLayout (LayoutKind.Sequential)]
		private struct CIEXYZ
		{
			public uint x, y, z;
		}

		[StructLayout (LayoutKind.Sequential)]
		private struct CIEXYZTRIPLE
		{
			public CIEXYZ red, green, blue;
		}

		[StructLayout (LayoutKind.Sequential)]
		private struct BITMAPV5HEADER
		{
			public int bV5Size;
			public int bV5Width;
			public int bV5Height;
			public ushort bV5Planes;
			public ushort bV5BitCount;
			public uint bV5Compression;
			public uint bV5SizeImage;
			public int bV5XPelsPerMeter;
			public int bV5YPelsPerMeter;
			public uint bV5ClrUsed;
			public uint bV5ClrImportant;
			public uint bV5RedMask;
			public uint bV5GreenMask;
			public uint bV5BlueMask;
			public uint bV5AlphaMask;
			public CIEXYZTRIPLE bV5Endpoints;
			public uint bV5CSType;
			public uint bV5GammaRed;
			public uint bV5GammaGreen;
			public uint bV5GammaBlue;
			public uint bV5Intent;
			public uint bV5ProfileData;
			public uint bV5ProfileSize;
			public uint bV5Reserved;
		}

		[DllImport ("shell32.dll", CharSet=CharSet.Auto)]
		static extern uint ExtractIconEx(string szFileName, int nIconIndex, out System.IntPtr phiconLarge, out System.IntPtr phiconSmall, uint nIcons);

		[DllImport ("gdi32.dll")]
		static extern System.IntPtr CreateCompatibleDC(System.IntPtr hdc);

		[DllImport ("gdi32.dll")]
		static extern System.IntPtr CreateSolidBrush(int color);

		[DllImport ("gdi32.dll")]
		static extern System.IntPtr SelectObject(System.IntPtr hdc, System.IntPtr obj);

		[DllImport ("gdi32.dll")]
		static extern void DeleteObject(System.IntPtr obj);

		[DllImport ("gdi32.dll")]
		static extern int GetPixel(System.IntPtr dc, int x, int y);

		[DllImport ("gdi32.dll")]
		static extern void DeleteDC(System.IntPtr dc);

		[DllImport ("gdi32.dll")]
		static extern System.IntPtr CreateDIBSection(System.IntPtr hdc, [In] ref BITMAPV5HEADER pbmi, uint iUsage, out System.IntPtr ppvBits, System.IntPtr hSection, uint dwOffset);

		[DllImport ("user32.dll")]
		private static extern bool GetIconInfo(System.IntPtr hIcon, out ICONINFO iconinfo);

		[DllImport ("user32.dll", SetLastError=true)]
		private static extern bool DrawIconEx(System.IntPtr hDc, int x, int y, System.IntPtr hIcon, int dx, int dy, int imageIndex, System.IntPtr filckerFreeBrush, int flags);

		private const int DI_MASK			= 0x01;
		private const int DI_IMAGE			= 0x02;
		private const int DI_NORMAL			= 0x03;
		private const int DI_COMPAT			= 0x04;
		private const int DI_DEFAULTSIZE	= 0x08;

		#endregion
		
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
