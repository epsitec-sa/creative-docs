namespace Epsitec.Common.Drawing
{
	public class TextBreak : System.IDisposable
	{
		public TextBreak(Font font, string text, double size)
		{
			this.handle = Agg.Library.AggFontFaceBreakNew (font.Handle, text, 0);
			this.size   = size;
		}
		
		~TextBreak()
		{
			this.Dispose (false);
		}
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		public System.IntPtr			Handle
		{
			get { return this.handle; }
		}
		
		public bool GetNextBreak(double max_width, out string text, out double width)
		{
			if (this.handle == System.IntPtr.Zero)
			{
				text  = "";
				width = 0;
				
				return false;
			}
			
			width = max_width / this.size;
			text  = Agg.Library.AggFontFaceBreakIter (this.handle, ref width);
			
			width *= this.size;
			
			if (width <= 0)
			{
				text  = "";
				width = 0;
				
				return false;
			}
			
			return true;
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	Rien de spécial à libérer...
			}
			
			if (this.handle != System.IntPtr.Zero)
			{
				Agg.Library.AggFontFaceBreakDelete (this.handle);
				this.handle = System.IntPtr.Zero;
			}
		}
		
		
		private System.IntPtr			handle;
		private double					size;
	}
}
