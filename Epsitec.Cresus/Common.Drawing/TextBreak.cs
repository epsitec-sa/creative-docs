//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	public class TextBreak
	{
		static TextBreak()
		{
			System.Runtime.Remoting.ObjectHandle handle = System.Activator.CreateInstance ("Common.Text", "Epsitec.Common.Text.TextBreakEngineFactory");

			TextBreak.factory = handle.Unwrap () as ITextBreakEngineFactory;
		}

		static ITextBreakEngineFactory factory;
		
		public TextBreak()
		{
			this.engine = TextBreak.factory.Create ();
		}
		
		
		public void SetText(string text, TextBreakMode mode)
		{
			this.engine.SetText (text, mode);
		}

		public void SetRuns(ICollection<TextBreak.XRun> runs)
		{
			List<TextBreakRun> list = new List<TextBreakRun> ();

			foreach (TextBreak.XRun run in runs)
			{
				Font   font = run.Font;
				double size = run.FontSize;
				
				if (run.Image != null)
				{
					font = null;
					size = run.Image.Width;
				}

				list.Add (new TextBreakRun (font, run.Length, size, run.Locale));
			}
			
			this.engine.SetRuns (list);
		}
		
		public void Rewind()
		{
			this.engine.Rewind ();
		}
		
		public bool GetNextBreak(double max_width, out string text, out double width, out int n_char)
		{
			return this.engine.GetNextBreak (max_width, out text, out width, out n_char);
		}


		public TextBreak.Line[] GetLines(double max_width)
		{
			return this.GetLines (max_width, max_width, max_width);
		}

		public TextBreak.Line[] GetLines(double initial_width, double following_width, double full_width)
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

				list.Add (new TextBreak.Line (line_text, line_width, line_skip));
				
				if ((line_text.Length > 0) &&
					(line_text[line_text.Length-1] == TextBreak.CodeLineBreak))
				{
					//	Le texte se termine par une fin de ligne forcée (<br/>) et il faut donc
					//	continuer à disposer les lignes avec toute la largeur.
					
					following_width = full_width;
				}

				width = following_width;
			}

			TextBreak.Line[] lines = new TextBreak.Line[list.Count];
			list.CopyTo (lines);
			
			return lines;
		}
		
		
		public static string GetUnicodeName(int code)
		{
			return AntiGrain.Font.GetUnicodeName (code);
		}

		public class XRun
		{
			//	Décrit une portion de texte d'une longueur quelconque utilisant
			//	une seule et même typographie.
			public int Start       = 0;  // index début
			public int Length      = 0;  // index fin - index début
			public string FontName    = "";
			public Font Font        = null;
			public double FontSize    = 0.0;
			public double FontScale   = 0.0;
			public RichColor FontColor   = RichColor.Empty;
			public bool Bold        = false;
			public bool Italic      = false;
			public bool Underlined  = false;
			public bool Anchor      = false;
			public bool Wave        = false;
			public Color WaveColor   = Color.Empty;
			public Image Image       = null;
			public double VerticalOffset = 0.0;
			public string Locale      = null;
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

			public Run(Run run)
				: this (run.Length, run.FontId, run.FontScale)
			{
			}


			public void Reset()
			{
				this.Length    = 0;
				this.FontId    = -1;
				this.FontScale = 0;
			}


			public int Length    = 0;
			public int FontId    = -1;
			public double FontScale = 0;
		}

		public class Line
		{
			public Line(string text, double width, int skip)
			{
				//	Décrit une ligne physique pouvant contenir plusieurs typographies
				//	différentes.
				this.text  = text;
				this.width = width;
				this.skip  = skip;
			}


			public int Skip
			{
				get
				{
					return this.skip;
				}
			}

			public string Text
			{
				get
				{
					return this.text;
				}
			}

			public double Width
			{
				get
				{
					return this.width;
				}

			}


			private int skip;   // nb de caractères à sauter jusqu'au début de la ligne suivante
			private string text;   // texte de la ligne, terminée év. par les caractères de césure
			private double width;  // largeur occupée par la ligne
		}
		
		public const char CodeLineBreak	= '\u2028';
		private ITextBreakEngine		engine;
	}
}
