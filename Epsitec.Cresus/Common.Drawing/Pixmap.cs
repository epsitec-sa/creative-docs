namespace Epsitec.Common.Drawing
{
	public class Pixmap : System.IDisposable
	{
		public Pixmap()
		{
		}
		
		~Pixmap()
		{
			this.Dispose (false);
		}
		
		
		public System.Drawing.Size		Size
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
		
		public System.IntPtr			Handle
		{
			get { return this.agg_buffer; }
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
		
		
		public void Erase(System.Drawing.Rectangle clip)
		{
			AntiGrain.Buffer.ClearRect (this.agg_buffer, clip.Left, clip.Top, clip.Right, clip.Bottom);
		}
		
		public void GetMemoryLayout(out int width, out int height, out int stride, out System.Drawing.Imaging.PixelFormat format, out System.IntPtr scan0)
		{
			format = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
			AntiGrain.Buffer.GetMemoryLayout (this.agg_buffer, out width, out height, out stride, out scan0);
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
			AntiGrain.Buffer.AddClipBox (this.agg_buffer, cx1, cy1, cx2, cy2);
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
				//	Nothing 'managed' to dispose here
			}
			
			if (this.agg_buffer != System.IntPtr.Zero)
			{
				AntiGrain.Buffer.Delete (this.agg_buffer);
				this.agg_buffer = System.IntPtr.Zero;
			}
		}
		
		
		
		protected System.IntPtr			agg_buffer;
		protected System.Drawing.Size	size;
	}
}
