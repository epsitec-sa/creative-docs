//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	public class TextBreak : System.IDisposable
	{
		static TextBreak()
		{
			int    line_break_length;
			byte[] line_break_data;
			
			//	The compressed file is produced by Common.Drawing.Tests, CompressLineBreakTool, based on
			//	the UNICODE LineBreak-4.0.0.txt source file, applying maximum BZIP2 compression to it.
			//	This allows us to reduce the size from 558KB to 85KB of data.
			//	Switched to Deflate-9: 640KB to 110KB of data.
			
			using (System.IO.Stream stream = typeof (TextBreak).Assembly.GetManifestResourceStream ("Epsitec.Common.Drawing.Resources.LineBreak.compressed"))
			{
				line_break_data   = IO.Decompression.DecompressToArray (stream);
				line_break_length = line_break_data.Length;
			}
			
			AntiGrain.Interface.Initialise ();
			AntiGrain.TextBreak.InitialiseLineBreakInformation (line_break_data, line_break_length);
		}
		
		public TextBreak()
		{
			this.handle = AntiGrain.TextBreak.New ();
		}
		
		
		~TextBreak()
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
		
		public void SetRuns(XRun[] runs)
		{
			AntiGrain.TextBreak.SetRunCount (this.handle, runs.Length);
			
			for (int i = 0; i < runs.Length; i++)
			{
				int    id   = runs[i].FontId;
				double size = runs[i].FontSize;
				
				if (runs[i].Image != null)
				{
					//	Traite une image comme si c'était une fonte spéciale, et prend la largeur
					//	de l'image comme référence :
					
					id   = -1;
					size = runs[i].Image.Width;
				}

				AntiGrain.TextBreak.SetNthRun(this.handle, i, runs[i].Length, id, size);
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
		
		
		public Line[] GetLines(double max_width)
		{
			return this.GetLines (max_width, max_width, max_width);
		}
		
		public Line[] GetLines(double initial_width, double following_width, double full_width)
		{
			//	Effectue le découpage des "runs" en lignes.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			this.Rewind ();
			
			double width = initial_width;
			string line_text;
			double line_width;
			int    line_skip;
			
			while (this.GetNextBreak (width, out line_text, out line_width, out line_skip))
			{
				if ((line_text.Length == 0) &&
					(line_skip == 0))
				{
					//	Panique: il n'est pas possible de couper cette ligne, quel que soit
					//	le moyen utilisé. On abandonne !
					
					return null;
				}
				
				list.Add (new Line (line_text, line_width, line_skip));
				
				if ((line_text.Length > 0) &&
					(line_text[line_text.Length-1] == TextBreak.CodeLineBreak))
				{
					//	Le texte se termine par une fin de ligne forcée (<br/>) et il faut donc
					//	continuer à disposer les lignes avec toute la largeur.
					
					following_width = full_width;
				}

				width = following_width;
			}
			
			Line[] lines = new Line[list.Count];
			list.CopyTo (lines);
			
			return lines;
		}
		
		
		public static string GetUnicodeName(int code)
		{
			return AntiGrain.TextBreak.GetUnicodeName (code);
		}
		
		public static short[] GetHyphenationPositions(string text)
		{
			short[] breaks = new short[25];
			int     num    = AntiGrain.TextBreak.HyphenateWord (text, text.Length, breaks);
			
			short[] result = new short[num];
			
			for (int i = 0; i < num; i++)
			{
				result[i] = breaks[i];
			}
			
			return result;
		}
		
		public class XRun
		{
			// Décrit une portion de texte d'une longueur quelconque utilisant
			// une seule et même typographie.
			public int					Start       = 0;  // index début
			public int					Length      = 0;  // index fin - index début
			public string				FontName    = "";
			public int					FontId      = -1;
			public double				FontSize    = 0.0;
			public double				FontScale   = 0.0;
			public Color				FontColor   = Color.Empty;
			public bool					Bold        = false;
			public bool					Italic      = false;
			public bool					Underlined  = false;
			public bool					Anchor      = false;
			public bool					Wave        = false;
			public Color				WaveColor   = Color.Empty;
			public Image				Image       = null;
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
			
			
			public int					Length    = 0;
			public int					FontId    = -1;
			public double				FontScale = 0;
		}
		
		public class Line
		{
			// Décrit une ligne physique pouvant contenir plusieurs typographies
			// différentes.
			public Line(string text, double width, int skip)
			{
				this.text  = text;
				this.width = width;
				this.skip  = skip;
			}
			
			
			public int					Skip
			{
				get { return this.skip; }
			}
			
			public string				Text
			{
				get { return this.text; }
			}
			
			public double				Width
			{
				get { return this.width; }

			}
			
			
			private int					skip;   // nb de caractères à sauter jusqu'au début de la ligne suivante
			private string				text;   // texte de la ligne, terminée év. par les caractères de césure
			private double				width;  // largeur occupée par la ligne
		}
		
		private System.IntPtr			handle;
		
		public const char				CodeLineBreak	= '\u2028';
	}
	
	[System.Flags] public enum TextBreakMode
	{
		None			= 0x0000,
		
		Hyphenate		= 0x0001,		//	césure des mots, si possible
		Ellipsis		= 0x0002,		//	ajoute une ellipse (...) si le dernier mot est tronqué
		Overhang		= 0x0004,		//	permet de dépasser la largeur si on ne peut pas faire autrement
		Split			= 0x0008,		//	coupe brutalement si on ne peut pas faire autrement
		
		SingleLine		= 0x0100,		//	force tout sur une ligne (utile avec Ellipsis, Overhang et Split)
	}
}
