//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	using PixelFormat = System.Drawing.Imaging.PixelFormat;
	using BitmapData  = System.Drawing.Imaging.BitmapData;
	
	public class Pixmap : System.IDisposable
	{
		public Pixmap()
		{
		}
		
		~Pixmap()
		{
			this.Dispose (false);
		}
		
		
		public System.Drawing.Size				Size
		{
			get
			{
				return this.size;
			}
			set
			{
				if (this.size != value)
				{
					if (this.agg_buffer == System.IntPtr.Zero)
					{
						this.agg_buffer = AntiGrain.Buffer.New (value.Width, value.Height, 32);
					}
					else
					{
						AntiGrain.Buffer.Resize (this.agg_buffer, value.Width, value.Height, 32);
					}
					
					this.size = value;
				}
			}
		}
		
		public System.IntPtr					Handle
		{
			get
			{
				return this.agg_buffer;
			}
		}
		
		public bool								IsOSBitmap
		{
			get
			{
				return this.is_os_bitmap;
			}
		}
		
		
		public void CreateOSBitmap(System.Drawing.Size size, System.IntPtr display_context)
		{
			if ((this.size.IsEmpty) &&
				(this.agg_buffer == System.IntPtr.Zero))
			{
				this.agg_buffer   = AntiGrain.Buffer.New (display_context, size.Width, size.Height, 32);
				this.size         = size;
				this.is_os_bitmap = true;
				return;
			}
			
			throw new System.InvalidOperationException ("Cannot re-create bitmap.");
		}
		
		public void Clear()
		{
			AntiGrain.Buffer.Clear (this.agg_buffer);
		}
		
		public void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clip)
		{
			System.IntPtr hdc = graphics.GetHdc ();
			
			try
			{
				this.Paint (hdc, clip);
			}
			finally
			{
				graphics.ReleaseHdc (hdc);
			}
		}
		
		public void Paint(System.Drawing.Graphics graphics, System.Drawing.Point offset, System.Drawing.Rectangle clip)
		{
			System.IntPtr hdc = graphics.GetHdc ();
			
			try
			{
				this.Paint (hdc, offset, clip);
			}
			finally
			{
				graphics.ReleaseHdc (hdc);
			}
		}
		
		public void Blend(System.Drawing.Graphics graphics, System.Drawing.Point offset, System.Drawing.Rectangle clip)
		{
			System.IntPtr hdc = graphics.GetHdc ();
			
			try
			{
				this.Blend (hdc, offset, clip);
			}
			finally
			{
				graphics.ReleaseHdc (hdc);
			}
		}
		
		public void Paint(System.IntPtr hdc, System.Drawing.Rectangle clip)
		{
			AntiGrain.Buffer.Paint (this.agg_buffer, hdc, clip.Left, clip.Bottom, clip.Right, clip.Top);
		}
		
		public void Paint(System.IntPtr hdc, System.Drawing.Point offset, System.Drawing.Rectangle clip)
		{
			AntiGrain.Buffer.PaintOffset (this.agg_buffer, hdc, offset.X, offset.Y, clip.Left, clip.Bottom, clip.Right, clip.Top);
		}
		
		public void Blend(System.IntPtr hdc, System.Drawing.Point offset, System.Drawing.Rectangle clip)
		{
			AntiGrain.Buffer.BlendOffset (this.agg_buffer, hdc, offset.X, offset.Y, clip.Left, clip.Bottom, clip.Right, clip.Top);
		}
		
		
		public void Compose(int x, int y, Pixmap source, int x_source, int y_source, int width, int height)
		{
			AntiGrain.Buffer.ComposeBuffer (this.agg_buffer, x, y, source.agg_buffer, x_source, y_source, width, height);
		}
		
		public void Copy(int x, int y, Pixmap source, int x_source, int y_source, int width, int height)
		{
			AntiGrain.Buffer.BltBuffer (this.agg_buffer, x, y, source.agg_buffer, x_source, y_source, width, height);
		}
		
		
		public void Erase(System.Drawing.Rectangle clip)
		{
			AntiGrain.Buffer.ClearRect (this.agg_buffer, clip.Left, clip.Top, clip.Right, clip.Bottom);
		}
		
		public void GetMemoryLayout(out int width, out int height, out int stride, out System.Drawing.Imaging.PixelFormat format, out System.IntPtr scan0)
		{
			format = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
			scan0  = AntiGrain.Buffer.GetMemoryLayout (this.agg_buffer, out width, out height, out stride);
		}
		
		public System.IntPtr GetMemoryBitmapHandle()
		{
			return AntiGrain.Buffer.GetMemoryBitmapHandle (this.agg_buffer);
		}
		
		public void InfiniteClipping()
		{
			AntiGrain.Buffer.InfiniteClipping (this.agg_buffer);
		}
		
		public void EmptyClipping()
		{
			AntiGrain.Buffer.EmptyClipping (this.agg_buffer);
		}
		
		public void AddClipBox(double x1, double y1, double x2, double y2)
		{
			int cx1 = (int)(x1);
			int cy1 = (int)(y1);
			int cx2 = (int)(x2+0.9999);
			int cy2 = (int)(y2+0.9999);
			AntiGrain.Buffer.AddClipBox (this.agg_buffer, cx1, cy1, cx2-1, cy2-1);
		}
		
		
		public Color GetPixel(int x, int y)
		{
			if ((x < 0) || (x >= this.Size.Width) ||
				(y < 0) || (y >= this.Size.Height))
			{
				return Color.Empty;
			}
			
			using (Pixmap.RawData src = new Pixmap.RawData (this))
			{
				return src[x, y];
			}
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
				//	Nothing 'managed' to dispose here
			}
			
			if (this.agg_buffer != System.IntPtr.Zero)
			{
				AntiGrain.Buffer.Delete (this.agg_buffer);
				this.agg_buffer = System.IntPtr.Zero;
			}
		}
		
		
		#region RawData Class
		public class RawData : System.IDisposable
		{
			public RawData(Pixmap pixmap)
			{
				pixmap.GetMemoryLayout (out this.dx, out this.dy, out this.stride, out this.format, out this.pixels);
			}
			
			public RawData(System.Drawing.Bitmap bitmap) : this (bitmap, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
			{
			}
			
			public RawData(System.Drawing.Bitmap bitmap, System.Drawing.Imaging.PixelFormat format)
			{
				System.Drawing.Rectangle             clip   = new System.Drawing.Rectangle (0, 0, bitmap.Width, bitmap.Height);
				System.Drawing.Imaging.ImageLockMode mode   = System.Drawing.Imaging.ImageLockMode.ReadWrite;
				
				this.bm = bitmap;
				this.bm_data = bitmap.LockBits (clip, mode, format);
				
				this.stride = this.bm_data.Stride;
				this.pixels = this.bm_data.Scan0;
				this.dx     = this.bm_data.Width;
				this.dy     = this.bm_data.Height;
				this.format = format;
			}
			
			public RawData(Image image) : this (image.BitmapImage.NativeBitmap)
			{
			}
			
			
			public PixelFormat					PixelFormat
			{
				get { return this.format; }
			}
			
			public int							Stride
			{
				get { return this.stride; }
			}
			
			public System.IntPtr				Pixels
			{
				get { return this.pixels; }
			}
			
			public int							Width
			{
				get { return this.dx; }
			}
			
			public int							Height
			{
				get { return this.dy; }
			}
			
			public bool							IsBottomUp
			{
				get { return this.bm == null; }
			}
			
			
			public Color						this[int x, int y]
			{
				get
				{
					byte r, g, b, a;
					
					switch (this.format)
					{
						case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
						case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
						case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
							break;
						default:
							throw new System.InvalidOperationException (string.Format ("Cannot access pixel of format {0}.", this.format));
					}
					
					unsafe
					{
						byte* ptr = x * 4 + y * this.stride + (byte*) this.Pixels.ToPointer ();
						a = ptr[3];
						r = ptr[2];
						g = ptr[1];
						b = ptr[0];
					}
					
					return Color.FromAlphaRgb (a / 255.0, r / 255.0, g / 255.0, b / 255.0);
				}
				set
				{
					byte r = (byte) (value.R * 255.0 + .5);
					byte g = (byte) (value.G * 255.0 + .5);
					byte b = (byte) (value.B * 255.0 + .5);
					byte a = (byte) (value.A * 255.0 + .5);
					
					switch (this.format)
					{
						case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
						case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
						case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
							break;
						default:
							throw new System.InvalidOperationException (string.Format ("Cannot access pixel of format {0}.", this.format));
					}
					
					unsafe
					{
						byte* ptr = x * 4 + y * this.stride + (byte*) this.Pixels.ToPointer ();
						ptr[3] = a;
						ptr[2] = r;
						ptr[1] = g;
						ptr[0] = b;
					}
				}
			}
			
			
			#region IDisposable Members
			public void Dispose()
			{
				this.Dispose (true);
				System.GC.SuppressFinalize (this);
			}
			#endregion
			
			public void CopyFrom(RawData that)
			{
				if (this.format != that.format)
				{
					throw new System.InvalidOperationException (string.Format ("Cannot copy formats {0} to {1}.", this.format, that.format));
				}
				
				int bpp = 0;
				
				switch (this.format)
				{
					case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
					case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
					case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
						bpp = 4;
						break;
				}
				
				if (bpp == 0)
				{
					throw new System.InvalidOperationException (string.Format ("Copy does not support format {0}.", this.format));
				}
				
				unsafe
				{
					System.IntPtr srcscan0  = that.Pixels;
					System.IntPtr dstscan0  = this.Pixels;
					int           srcstride = that.Stride;
					int           dststride = this.Stride;
					
					int dx = System.Math.Min (this.Width,  that.Width);
					int dy = System.Math.Min (this.Height, that.Height);
					
					int* srcdata = (int*) srcscan0.ToPointer ();
					int* dstdata = (int*) dstscan0.ToPointer ();
					int  srcymul = srcstride / 4;
					int  dstymul = dststride / 4;
					
					if (that.IsBottomUp == false)
					{
						srcdata += that.Height * srcymul;
						srcdata -= srcymul;
						srcymul  = - srcymul;
					}
					if (this.IsBottomUp == false)
					{
						dstdata += this.Height * dstymul;
						dstdata -= dstymul;
						dstymul  = - dstymul;
					}
					
					for (int y = 0; y < dy; y++)
					{
						for (int x = 0; x < dx; x++)
						{
							dstdata[x] = srcdata[x];
						}
						
						srcdata += srcymul;
						dstdata += dstymul;
					}
				}
			}
			
			public void CopyTo(RawData raw)
			{
				raw.CopyFrom (this);
			}
			
			
			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (this.bm != null)
					{
						this.bm.UnlockBits (this.bm_data);
						
						this.bm      = null;
						this.bm_data = null;
					}
				}
			}
			
			
			protected int						stride;
			protected System.IntPtr				pixels;
			protected int						dx, dy;
			protected PixelFormat				format;
			
			protected System.Drawing.Bitmap		bm;
			protected BitmapData				bm_data;
		}
		#endregion
		
		protected System.IntPtr					agg_buffer;
		protected System.Drawing.Size			size;
		protected bool							is_os_bitmap;
	}
}
