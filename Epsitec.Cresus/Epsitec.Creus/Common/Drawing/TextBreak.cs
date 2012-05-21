//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	public class TextBreak
	{
		static TextBreak()
		{
			TextBreak.factory = new Epsitec.Common.Text.TextBreakEngineFactory ();
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
		
		public bool GetNextBreak(double maxWidth, out string text, out double width, out int n_char)
		{
			return this.engine.GetNextBreak (maxWidth, out text, out width, out n_char);
		}


		public TextBreak.Line[] GetLines(double maxWidth)
		{
			return this.GetLines (maxWidth, maxWidth, maxWidth);
		}

		public TextBreak.Line[] GetLines(double initialWidth, double followingWidth, double fullWidth)
		{
			//	Effectue le découpage des "runs" en lignes.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			this.Rewind ();
			
			double width = initialWidth;
			string lineText;
			double lineWidth;
			int    lineSkip;
			
			while (this.GetNextBreak (width, out lineText, out lineWidth, out lineSkip))
			{
				if ((lineText.Length == 0) &&
					(lineSkip == 0))
				{
					//	Panique: il n'est pas possible de couper cette ligne, quel que soit
					//	le moyen utilisé. On abandonne !
					
					return null;
				}

				list.Add (new TextBreak.Line (lineText, lineWidth, lineSkip));

				if (lineSkip == 0)
				{
					break;
				}
				
				if ((lineText.Length > 0) &&
					(lineText[lineText.Length-1] == TextBreak.CodeLineBreak))
				{
					//	Le texte se termine par une fin de ligne forcée (<br/>) et il faut donc
					//	continuer à disposer les lignes avec toute la largeur.
					
					followingWidth = fullWidth;
				}

				width = followingWidth;
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
			public string FontFace    = "";
			public Font Font        = null;
			public double FontSize    = 0.0;
			public double FontScale   = 0.0;
			public RichColor FontColor   = RichColor.Empty;
			public bool Bold        = false;
			public bool Italic      = false;
			public bool Underline   = false;
			public bool Anchor      = false;
			public bool Wave        = false;
			public Color WaveColor   = Color.Empty;
			public Color BackColor   = Color.Empty;
			public Image Image       = null;
			public double VerticalOffset = 0.0;
			public string Replacement = null;
			public string Locale      = null;
		}

		public class Run
		{
			public Run()
			{
			}

			public Run(int length, int fontId, double fontScale)
			{
				this.Length    = length;
				this.FontId    = fontId;
				this.FontScale = fontScale;
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
