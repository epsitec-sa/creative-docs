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
