namespace Epsitec.Common.Drawing
{
	public class TextBreak : System.IDisposable
	{
		static TextBreak()
		{
			AntiGrain.Interface.Initialise ();
		}
		
		public TextBreak(Font font, string text, double size, TextBreakMode mode)
		{
			this.handle = AntiGrain.Font.Break.New (font.Handle, text, (int) mode);
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
		
		
		private System.IntPtr			handle;
		private double					size;
	}
	
	[System.Flags]
	public enum TextBreakMode
	{
		None			= 0x0000,
		
		Hyphenate		= 0x0001,		//	césure des mots, si possible
		Ellipsis		= 0x0002,		//	ajoute une ellipse (...) si le dernier mot est tronqué
		Overhang		= 0x0004,		//	permet de dépasser la largeur si on ne peut pas faire autrement
		Split			= 0x0008,		//	coupe brutalement si on ne peut pas faire autrement
		
		SingleLine		= 0x0100,		//	force tout sur une ligne (utile avec Ellipsis, Overhang et Split)
	}
}
