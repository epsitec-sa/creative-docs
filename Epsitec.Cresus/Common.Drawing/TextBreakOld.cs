namespace Epsitec.Common.Drawing
{
	public class TextBreakOld : System.IDisposable
	{
		static TextBreakOld()
		{
			AntiGrain.Interface.Initialise ();
		}
		
		public TextBreakOld(Font font, string text, double size, TextBreakMode mode)
		{
			this.handle = AntiGrain.Font.Break.New (font.Handle, text, (int) mode);
			this.size   = size;
		}
		
		
		~TextBreakOld()
		{
			this.Dispose (false);
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
				//	Rien de spécial à libérer...
			}
			
			if (this.handle != System.IntPtr.Zero)
			{
				AntiGrain.Font.Break.Delete (this.handle);
				this.handle = System.IntPtr.Zero;
			}
		}
		
		
		public System.IntPtr			Handle
		{
			get { return this.handle; }
		}
		
		public bool						MoreText
		{
			get { return AntiGrain.Font.Break.HasMore (this.handle); }
		}
		
		
		public bool GetNextBreak(double max_width, out string text, out double width, out int n_char)
		{
			if (this.handle == System.IntPtr.Zero)
			{
				text   = "";
				width  = 0;
				n_char = 0;
				
				return false;
			}
			
			width = max_width / this.size;
			text  = AntiGrain.Font.Break.Iter (this.handle, ref width, out n_char);
			
			width *= this.size;
			
			if (text == null)
			{
				System.Diagnostics.Debug.Assert (width == 0.0);
				System.Diagnostics.Debug.Assert (n_char == 0);
				
				text   = "";
				
				return false;
			}
			
			return true;
		}
		
		
		private System.IntPtr			handle;
		private double					size;
	}
}
