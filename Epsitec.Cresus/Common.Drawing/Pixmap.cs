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
						this.agg_buffer = Agg.Library.AggBufferNew (value.Width, value.Height, 24);
					}
					else
					{
						Agg.Library.AggBufferResize (this.agg_buffer, value.Width, value.Height, 24);
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
			Agg.Library.AggBufferClear (this.agg_buffer);
		}
		
		public void Paint(System.Drawing.Graphics graphics)
		{
			System.IntPtr hdc = graphics.GetHdc ();
			
			try
			{
				this.Paint (hdc);
			}
			finally
			{
				graphics.ReleaseHdc (hdc);
			}
		}

		public void Paint(System.IntPtr hdc)
		{
			Agg.Library.AggBufferPaint (this.agg_buffer, hdc);
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
				Agg.Library.AggBufferDelete (this.agg_buffer);
				this.agg_buffer = System.IntPtr.Zero;
			}
		}
		
		
		
		protected System.IntPtr			agg_buffer;
		protected System.Drawing.Size	size;
	}
}
