//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing.Platform;

using System.Runtime.InteropServices;

namespace Epsitec.Common.Drawing
{
	using BitmapData = System.Drawing.Imaging.BitmapData;
	
	/// <summary>
	/// La classe Bitmap encapsule une image de type bitmap.
	/// </summary>
	public class Bitmap : Image
	{
		static Bitmap()
		{
			Epsitec.Common.Drawing.Platform.NativeBitmap.SetOutOfMemoryHandler (Bitmap.NotifyMemoryExhauted);
		}


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
						(this.bitmapDx > 0) &&
						(this.bitmapDy > 0))
					{
						//	Des pixels sont définis, mais pas d'objet bitmap natif. On peut donc créer ici le bitmap
						//	en se basant sur ces pixels :
						
						return new System.Drawing.Bitmap (this.bitmapDx, this.bitmapDy, this.Stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, this.Scan0);
					}
				}
				
				return this.bitmap;
			}
		}
		
		public bool								IsLocked
		{
			get
			{
				return this.bitmapData != null;
			}
		}
		
		public System.IntPtr					Scan0
		{
			get
			{
				if (this.bitmapData == null)
				{
					return System.IntPtr.Zero;
				}
				
				return this.bitmapData.Scan0;
			}
		}
		
		public int								Stride
		{
			get
			{
				if (this.bitmapData == null)
				{
					return 0;
				}
				
				return this.bitmapData.Stride;
			}
		}

		public byte[]							GetRawBitmapBytes()
		{
			this.LockBits ();

			try
			{
				if (this.bitmapData != null)
				{
					System.IntPtr memory = this.Scan0;
					int size = this.bitmapData.Height * this.bitmapData.Stride;

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
				return this.bitmapDx;
			}
		}
		
		public int								PixelHeight
		{
			get
			{
				return this.bitmapDy;
			}
		}
		
		
		public static ICanvasFactory			CanvasFactory
		{
			get
			{
				return Bitmap.canvasFactory;
			}
			set
			{
				Bitmap.canvasFactory = value;
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
		
		static System.Collections.Generic.Dictionary<System.Drawing.Bitmap, System.Drawing.Imaging.BitmapData> lockedBitmapDataCache = new System.Collections.Generic.Dictionary<System.Drawing.Bitmap, System.Drawing.Imaging.BitmapData> ();
		
		public bool LockBits()
		{
			lock (this)
			{
				if (this.bitmapData == null)
				{
					System.Drawing.Imaging.ImageLockMode mode = System.Drawing.Imaging.ImageLockMode.ReadOnly;
					System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
					
					int width  = this.bitmapDx;
					int height = this.bitmapDy;
					
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
								this.bitmapData = this.bitmap.LockBits (new System.Drawing.Rectangle (0, 0, width, height), mode, format);
								success = true;
							}
							catch (System.Exception)
							{
								System.Diagnostics.Debug.WriteLine ("Attempted to lock bitmap and failed: " + attempt);
								System.Threading.Thread.Sleep (1);
							}
						}

						Bitmap.lockedBitmapDataCache[this.bitmap] = this.bitmapData;
					}
				}
				
				if (this.bitmapData != null)
				{
					this.bitmapLockCount++;
					return true;
				}
			}
			
			return false;
		}
		
		public void UnlockBits()
		{
			lock (this)
			{
				if (this.bitmapLockCount > 0)
				{
					this.bitmapLockCount--;
					
					if (this.bitmapLockCount == 0)
					{
						System.Diagnostics.Debug.Assert (this.bitmap != null);
						System.Diagnostics.Debug.Assert (this.bitmapData != null);
						
						this.bitmap.UnlockBits (this.bitmapData);
						this.bitmapData = null;
						
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
				
				int nx = this.bitmapDx / 2;
				int ny = this.bitmapDy / 2;
				int my = this.bitmapDy - 1;
				
				unsafe
				{
					for (int y = 0; y < ny; y++)
					{
						int y1 = y;
						int y2 = my-y;
						int* s1 = (int*) (this.bitmapData.Scan0.ToPointer ()) + y1 * this.bitmapData.Stride / 4;
						int* s2 = (int*) (this.bitmapData.Scan0.ToPointer ()) + y2 * this.bitmapData.Stride / 4;
						
						for (int x = 0; x < this.bitmapDx; x++)
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

			if ((format == ImageFormat.WindowsIcon) ||
				(format == ImageFormat.WindowsPngIcon))
			{
				return this.SaveIcon (format);
			}
			
			System.Drawing.Imaging.ImageCodecInfo    encoderInfo    = Bitmap.GetCodecInfo (format);
			System.Drawing.Imaging.EncoderParameters encoderParams  = new System.Drawing.Imaging.EncoderParameters (list.Count);
			
			for (int i = 0; i < list.Count; i++)
			{
				encoderParams.Param[i] = list[i] as System.Drawing.Imaging.EncoderParameter;
			}
			
			if (encoderInfo != null)
			{
				System.IO.MemoryStream stream = new System.IO.MemoryStream ();
				System.Drawing.Bitmap  bitmap = this.NativeBitmap;
				
				bitmap.SetResolution ((float) dpi, (float) dpi);
				bitmap.Save (stream, encoderInfo, encoderParams);
				stream.Close ();
				
				byte[] data = stream.ToArray ();

				if (data == null)
				{
					return null;
				}

				return data;
			}

			return null;
		}

		private byte[] SaveIcon(ImageFormat format)
		{
			byte[] imageBytes = this.GetRawBitmapBytes ();

			switch (format)
			{
				case ImageFormat.WindowsIcon:
					return NativeIcon.CreateIcon (imageBytes, this.Stride, this.PixelWidth, this.PixelHeight);
			
				case ImageFormat.WindowsPngIcon:
					return NativeIcon.CreatePngIcon (imageBytes, this.Stride, this.PixelWidth, this.PixelHeight);

				default:
					return null;
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

		public static Image FromImage(NativeBitmap image)
		{
			if (image == null)
			{
				return null;
			}

			try
			{
				Pixmap pixmap = new Pixmap ();
				pixmap.AllocatePixmap (image);

				return Bitmap.FromPixmap (pixmap);
			}
			catch (System.NullReferenceException)
			{
				return null;
			}
		}
		
		public static Image FromPixmap(Pixmap pixmap)
		{
			Bitmap bitmap = new Bitmap ();
			
			bitmap.pixmap    = pixmap;
			bitmap.bitmap    = null;
			bitmap.bitmapDx = pixmap.Size.Width;
			bitmap.bitmapDy = pixmap.Size.Height;
			bitmap.size      = new Size (bitmap.bitmapDx, bitmap.bitmapDy);
			bitmap.origin    = new Point (0, 0);
			
			//	Prétend que le bitmap est verrouillé, puisqu'on a de toute façons déjà accès aux
			//	pixels (c'est d'ailleurs bien la seule chose qu'on a) :
			
			bitmap.bitmapLockCount = 1;
			bitmap.isOriginDefined = true;
			
			int dx, dy, stride;
			System.IntPtr pixels;
			System.Drawing.Imaging.PixelFormat format;
			
			pixmap.GetMemoryLayout (out dx, out dy, out stride, out format, out pixels);
			
			bitmap.bitmapData = new BitmapData ();
			
			bitmap.bitmapData.Width       = dx;
			bitmap.bitmapData.Height      = dy;
			bitmap.bitmapData.PixelFormat = format;
			bitmap.bitmapData.Scan0       = pixels;
			bitmap.bitmapData.Stride      = stride;
			
			return bitmap;
		}
		
		public static Image FromNativeBitmap(int dx, int dy)
		{
			return Bitmap.FromNativeBitmap (new System.Drawing.Bitmap (dx, dy));
		}
		
		public static Image FromNativeBitmap(System.Drawing.Bitmap native)
		{
			Image bitmap = Bitmap.FromNativeBitmap (native, new Point (0, 0));
			bitmap.isOriginDefined = false;
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
			bitmap.bitmapDx = native.Width;
			bitmap.bitmapDy = native.Height;
			bitmap.size      = size;
			bitmap.origin    = origin;
			
			bitmap.isOriginDefined = true;
			
			return bitmap;
		}

		public static Image FromNativeIcon(string path, int dx, int dy)
		{
			return Bitmap.FromNativeIcon (IconLoader.LoadIcon (path, dx, dy));
		}

		public static System.Drawing.Icon LoadNativeIcon(string path, int dx, int dy)
		{
			return IconLoader.LoadIcon (path, dx, dy);
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
			if (native == null)
			{
				return null;
			}

			System.Drawing.Bitmap srcBitmap = native.ToBitmap ();
			System.Drawing.Bitmap dstBitmap;

			double dpiX = srcBitmap.HorizontalResolution;
			double dpiY = srcBitmap.VerticalResolution;
			
			int dx = srcBitmap.Width;
			int dy = srcBitmap.Height;

			dstBitmap = Bitmap.ConvertIcon (native.Handle, dx, dy);

			Bitmap bitmap = new Bitmap ();

			bitmap.bitmap			 = dstBitmap;
			bitmap.bitmapDx         = dx;
			bitmap.bitmapDy         = dy;
			bitmap.size				 = new Size (dx, dy);
			bitmap.origin			 = new Point (0, 0);
			bitmap.isOriginDefined = false;

			bitmap.dpiX = dpiX;
			bitmap.dpiY = dpiY;

			return bitmap;
			
		}
		
		public static Image FromData(byte[] data)
		{
			Image bitmap = Bitmap.FromData (data, new Point (0, 0));
			
			if (bitmap == null)
			{
				return null;
			}
			else
			{
				bitmap.isOriginDefined = false;
				return bitmap;
			}
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
					
					if (Bitmap.canvasFactory != null)
					{
						return Bitmap.canvasFactory.CreateCanvas (data);
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

					double dpiX = metafile.HorizontalResolution;
					double dpiY = metafile.VerticalResolution;
					
					int dx = metafile.Width;
					int dy = metafile.Height;

					int bitmapWidth = dx;
					int bitmapHeight = dy;

					//	Don't allocate a pixmap with more than 100 Mpixels... which is
					//	already really huge.
					
					while ((bitmapWidth*bitmapHeight > 100*1000*1000)
						&& (dpiX * dpiY > 100*100))
					{
						bitmapWidth  /= 2;
						bitmapHeight /= 2;
						dpiX /= 2;
						dpiY /= 2;
					}

					if ((bitmapWidth != dx) ||
						(bitmapHeight != dy))
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Reduced WMF size from {0}x{1} to {2}x{3} pixels", dx, dy, bitmapWidth, bitmapHeight));
					}
					
					System.Drawing.Bitmap dstBitmap;

					try
					{
						dstBitmap = new System.Drawing.Bitmap (bitmapWidth, bitmapHeight);
					}
					catch
					{
						dstBitmap = null;
					}
					
					
					using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dstBitmap))
					{
						graphics.DrawImage (metafile, 0, 0, bitmapWidth, bitmapHeight);
					}
					
					Image image = Bitmap.FromNativeBitmap (dstBitmap, origin, size);
					
					if (image != null)
					{
						image.dpiX = dpiX;
						image.dpiY = dpiY;
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
							System.Drawing.Bitmap srcBitmap = Bitmap.DecompressBitmap (stream, data);
							System.Drawing.Bitmap dstBitmap = new System.Drawing.Bitmap (8, 8);

							double dpiX = srcBitmap.HorizontalResolution;
							double dpiY = srcBitmap.VerticalResolution;

							srcBitmap.SetResolution (dstBitmap.HorizontalResolution, dstBitmap.VerticalResolution);

							using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dstBitmap))
							{
								try
								{
									graphics.DrawImageUnscaled (srcBitmap, 0, 0);
								}
								catch
								{
									graphics.DrawImageUnscaled (srcBitmap, 0, 0);
								}
							}

							dstBitmap = srcBitmap;
							Image image = Bitmap.FromNativeBitmap (dstBitmap, origin, size);

							if (image != null)
							{
								image.dpiX = dpiX;
								image.dpiY = dpiY;
							}

							return image;
						}
					}
					catch
					{
						System.Diagnostics.Debug.WriteLine ("Out of memory in GDI - attempt " + attempt);
						Bitmap.NotifyMemoryExhauted ();
					}
				}

				return null;
			}
		}

		private static System.Drawing.Bitmap DecompressBitmap(System.IO.MemoryStream stream, byte[] data)
		{
			try
			{
				return new System.Drawing.Bitmap (stream);
			}
			catch (ExternalException)
			{
				using (var native = Platform.NativeBitmap.Load (data))
				{
					var bytes  = native.SaveToMemory (Platform.BitmapFileType.Bmp);

					using (System.IO.MemoryStream stream2 = new System.IO.MemoryStream (bytes, false))
					{
						return new System.Drawing.Bitmap (stream2);
					}
				}
			}
		}

		private static void NotifyMemoryExhauted()
		{
			Bitmap.OnOutOfMemoryEncountered ();
			System.GC.Collect ();
			System.Threading.Thread.Sleep (1);
		}

		private static void OnOutOfMemoryEncountered()
		{
			var handler = Bitmap.OutOfMemoryEncountered;

			if (handler != null)
			{
				handler (null);
			}
		}

		public static event Support.EventHandler OutOfMemoryEncountered;
		
		public static Image FromFile(string fileName)
		{
			Image bitmap = Bitmap.FromFile (fileName, new Point (0, 0));
			bitmap.isOriginDefined = false;
			return bitmap;
		}
		
		public static Image FromFile(string fileName, Point origin)
		{
			return Bitmap.FromFile (fileName, origin, Size.Empty);
		}
		
		public static Image FromFile(string fileName, Point origin, Size size)
		{
			using (System.IO.FileStream file = new System.IO.FileStream (fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				long   length = file.Length;
				byte[] buffer = new byte[length];
				file.Read (buffer, 0, (int) length);
				
				return Bitmap.FromData (buffer, origin, size);
			}
		}
		
		public static Image FromManifestResource(string namespaceName, string resourceName, System.Type type)
		{
			if (resourceName == null)
			{
				return null;
			}

			return Bitmap.FromManifestResource (string.Concat (namespaceName, ".", resourceName), type.Assembly);
		}
		
		public static Image FromManifestResource(string resourceName, System.Reflection.Assembly assembly)
		{
			if (resourceName == null)
            {
				return null;
            }

			Image bitmap = Bitmap.FromManifestResource (resourceName, assembly, new Point (0, 0));
			
			if (bitmap != null)
			{
				bitmap.isOriginDefined = false;
			}
			
			return bitmap;
		}
		
		public static Image FromManifestResource(string resourceName, System.Reflection.Assembly assembly, Point origin)
		{
			return Bitmap.FromManifestResource (resourceName, assembly, origin, Size.Empty);
		}
		
		public static Image FromManifestResource(string resourceName, System.Reflection.Assembly assembly, Point origin, Size size)
		{
			using (System.IO.Stream stream = assembly.GetManifestResourceStream (resourceName))
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
			
			lock (Bitmap.disabledImages)
			{
				if (Bitmap.disabledImages.Contains (seed))
				{
					return Bitmap.disabledImages[seed] as Bitmap;
				}
				
				System.Drawing.Color  color = System.Drawing.Color.FromArgb (r, g, b);
				
				System.Drawing.Bitmap srcBitmap = image.BitmapImage.bitmap;
				System.Drawing.Bitmap dstBitmap = new System.Drawing.Bitmap (srcBitmap.Width, srcBitmap.Height);
				
				Platform.ImageDisabler.Paint (srcBitmap, dstBitmap, color);
				
				Bitmap bitmap = new Bitmap ();
				
				bitmap.bitmap			 = dstBitmap;
				bitmap.bitmapDx         = dstBitmap.Width;
				bitmap.bitmapDy         = dstBitmap.Height;
				bitmap.size				 = image.Size;
				bitmap.origin			 = image.Origin;
				bitmap.isOriginDefined = image.IsOriginDefined;
				
				Bitmap.disabledImages[seed] = bitmap;
				
				return bitmap;
			}
		}
		
		public static Image CopyImage(Image image)
		{
			if (image == null)
			{
				return null;
			}
			
			System.Drawing.Bitmap srcBitmap = image.BitmapImage.bitmap;
			System.Drawing.Bitmap dstBitmap = new System.Drawing.Bitmap (srcBitmap.Width, srcBitmap.Height);
			
			double dpiX = srcBitmap.HorizontalResolution;
			double dpiY = srcBitmap.VerticalResolution;
				
			srcBitmap.SetResolution (dstBitmap.HorizontalResolution, dstBitmap.VerticalResolution);
				
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dstBitmap))
			{
				graphics.DrawImageUnscaled (srcBitmap, 0, 0, srcBitmap.Width, srcBitmap.Height);
			}
			
			Bitmap bitmap = new Bitmap ();
			
			bitmap.bitmap			 = dstBitmap;
			bitmap.bitmapDx         = dstBitmap.Width;
			bitmap.bitmapDy         = dstBitmap.Height;
			bitmap.size				 = image.Size;
			bitmap.origin			 = image.Origin;
			bitmap.isOriginDefined = image.IsOriginDefined;
			
			bitmap.dpiX = dpiX;
			bitmap.dpiY = dpiY;
			
			return bitmap;
		}
		
		public static Image FromLargerImage(Image image, Rectangle clip)
		{
			Image bitmap = Bitmap.FromLargerImage (image, clip, Point.Zero);
			bitmap.isOriginDefined = false;
			return bitmap;
		}
		
		public static Image FromLargerImage(Image image, Rectangle clip, Point origin)
		{
			if (image == null)
			{
				return null;
			}
			
			System.Drawing.Bitmap srcBitmap = image.BitmapImage.bitmap;
			
			int dx = (int)(clip.Width  + 0.5);
			int dy = (int)(clip.Height + 0.5);
			int x  = (int)(clip.Left);
			int y  = (int)(clip.Bottom);
			int yy = srcBitmap.Height - dy - y;
			
			System.Drawing.Bitmap dstBitmap = new System.Drawing.Bitmap (dx, dy);
			
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dstBitmap))
			{
				graphics.DrawImage (srcBitmap, 0, 0, new System.Drawing.Rectangle (x, yy, dx, dy), System.Drawing.GraphicsUnit.Pixel);
			}
			
			Bitmap bitmap = new Bitmap ();
			
			double sx = image.Width  / srcBitmap.Width;
			double sy = image.Height / srcBitmap.Height;
			
			bitmap.bitmap			 = dstBitmap;
			bitmap.bitmapDx         = dstBitmap.Width;
			bitmap.bitmapDy         = dstBitmap.Height;
			bitmap.size				 = new Size (sx * dx, sy * dy);
			bitmap.origin			 = origin;
			bitmap.isOriginDefined = true;
			
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
			System.Diagnostics.Debug.Assert (this.isDisposed == false);
			
			this.isDisposed = true;
			
			if (disposing)
			{
				if (this.bitmap != null)
				{
					this.bitmap.Dispose ();
				}
				
				this.bitmap = null;
				this.bitmapData = null;
				this.bitmapLockCount = 0;
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

		private static class IconLoader
		{
			public static System.Drawing.Icon LoadIcon(string path, int dx, int dy)
			{
				uint mode = IconLoader.LR_LOADFROMFILE;

				if ((dx == 0) || (dy == 0))
				{
					dx = 0;
					dy = 0;
					mode |= IconLoader.LR_DEFAULTSIZE;
				}

				System.IntPtr hIcon = IconLoader.LoadImage (System.IntPtr.Zero, path, IconLoader.IMAGE_ICON, dx, dy, mode);
				string errorMessage = new System.ComponentModel.Win32Exception (Marshal.GetLastWin32Error ()).Message;
				return hIcon == System.IntPtr.Zero ? null : System.Drawing.Icon.FromHandle (hIcon);
			}

			const int IMAGE_ICON = 1;
			const int LR_LOADFROMFILE = 0x0010;
			const int LR_DEFAULTSIZE  = 0x0040;

			[DllImport ("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
			static extern System.IntPtr LoadImage(System.IntPtr hinst, string lpszName, uint uType,
			   int cxDesired, int cyDesired, uint fuLoad);
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
		protected int							bitmapDx;
		protected int							bitmapDy;
		protected BitmapData					bitmapData;
		protected volatile int					bitmapLockCount;
		protected Pixmap						pixmap;
		
		protected bool							isDisposed;
		
		static System.Collections.Hashtable		disabledImages = new System.Collections.Hashtable ();
		static ICanvasFactory					canvasFactory;
	}
}
