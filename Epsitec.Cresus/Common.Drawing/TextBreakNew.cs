namespace Epsitec.Common.Drawing
{
	public class TextBreakNew : System.IDisposable
	{
		static TextBreakNew()
		{
			AntiGrain.Interface.Initialise ();
		}
		
		public TextBreakNew()
		{
			this.handle = AntiGrain.TextBreak.New ();
		}
		
		
		~TextBreakNew()
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
				AntiGrain.TextBreak.Delete (this.handle);
				this.handle = System.IntPtr.Zero;
			}
		}
		
		
		public System.IntPtr			Handle
		{
			get { return this.handle; }
		}
		
		public bool						MoreText
		{
			get { return false; /*return AntiGrain.TextBreak.HasMore (this.handle);*/ }
		}
		
		
		public void SetText(string text, TextBreakMode mode)
		{
			AntiGrain.TextBreak.SetText (this.handle, text, (int) mode);
			
			if ((mode & TextBreakMode.Hyphenate) != 0)
			{
				AntiGrain.TextBreak.Hyphenate (this.handle);
			}
		}
		
		public void SetFonts(Drawing.Font[] fonts)
		{
			AntiGrain.TextBreak.SetFontFaceCount (this.handle, fonts.Length);
			
			for (int i = 0; i < fonts.Length; i++)
			{
				AntiGrain.TextBreak.SetNthFontFace (this.handle, i, fonts[i].Handle);
			}
		}
		
		public void SetFonts(System.Collections.IList fonts)
		{
			AntiGrain.TextBreak.SetFontFaceCount (this.handle, fonts.Count);
			
			for (int i = 0; i < fonts.Count; i++)
			{
				Font font = fonts[i] as Font;
				AntiGrain.TextBreak.SetNthFontFace (this.handle, i, font.Handle);
			}
		}
		
		public void SetRuns(int[] run_lengths, int[] font_ids, double[] font_scales)
		{
			AntiGrain.TextBreak.SetRunCount (this.handle, run_lengths.Length);
			
			for (int i = 0; i < run_lengths.Length; i++)
			{
				AntiGrain.TextBreak.SetNthRun (this.handle, i, run_lengths[i], font_ids[i], font_scales[i]);
			}
		}
		
		public void SetRuns(System.Collections.IList runs)
		{
			AntiGrain.TextBreak.SetRunCount (this.handle, runs.Count);
			
			for (int i = 0; i < runs.Count; i++)
			{
				Run run = runs[i] as Run;
				
				AntiGrain.TextBreak.SetNthRun (this.handle, i, run.Length, run.FontId, run.FontScale);
			}
		}
		
		
		public void Rewind()
		{
			AntiGrain.TextBreak.Rewind (this.handle);
		}
		
		public bool GetNextBreak(double max_width, out string text, out double width, out int n_char)
		{
			width = max_width;
			text  = AntiGrain.TextBreak.FindNextBreak (this.handle, ref width, out n_char);
			
			if (text == null)
			{
				System.Diagnostics.Debug.Assert (width == 0.0);
				System.Diagnostics.Debug.Assert (n_char == 0);
				
				text   = "";
				
				return false;
			}
			
			return true;
		}
		
		
		public class Run
		{
			public Run()
			{
			}
			
			public Run(int length, int font_id, double font_scale)
			{
				this.Length    = length;
				this.FontId    = font_id;
				this.FontScale = font_scale;
			}
			
			public Run(Run run) : this (run.Length, run.FontId, run.FontScale)
			{
			}
			
			
			public void Reset()
			{
				this.Length    = 0;
				this.FontId    = -1;
				this.FontScale = 0;
			}
			
			
			public int			Length    = 0;
			public int			FontId    = -1;
			public double		FontScale = 0;
		}
		
		
		private System.IntPtr			handle;
	}
}
