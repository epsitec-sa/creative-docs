//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe LanguageEngine offre des services dépendants de la langue, tels
	/// que la césure, par exemple.
	/// </summary>
	public sealed class LanguageEngine
	{
		private LanguageEngine()
		{
		}
		
		public static void GenerateHyphens(ILanguageRecognizer recognizer, ulong[] text, int offset, int length, Unicode.BreakInfo[] breaks)
		{
			int run_start  = 0;
			int run_length = 0;
			
			string run_locale = null;
			
			for (int i = 0; i < length; i++)
			{
				double hyphenation = 0;
				string locale      = null;

				recognizer.GetLanguage (text, offset+i, out hyphenation, out locale);

				if (hyphenation <= 0)
				{
					locale = null;
				}
				
				//	Détermine un "run" de caractères appartenant à la même langue
				//	et constituant un mot complet.
				
				if ((locale != run_locale) ||
					((i > 1) && (breaks[i-1] != Unicode.BreakInfo.No) && (breaks[i-1] != Unicode.BreakInfo.NoAlpha)))
				{
					if ((run_length > 0) &&
						(run_locale != null))
					{
						//	Traite la tranche qui vient de se terminer.
						
						LanguageEngine.GenerateHyphensForRun (text, offset + run_start, run_length, run_locale, run_start, breaks);
					}
					
					run_start  = i;
					run_length = 0;
					run_locale = locale;
				}
				
				run_length++;
			}
			
			if ((run_length > 0) &&
				(run_locale != null))
			{
				//	Traite la tranche finale.
				
				LanguageEngine.GenerateHyphensForRun (text, offset + run_start, run_length, run_locale, run_start, breaks);
			}
		}
		
		private static void GenerateHyphensForRun(ulong[] text, int text_offset, int length, string locale, int break_offset, Unicode.BreakInfo[] breaks)
		{
			if (length < 1)
			{
				return;
			}
			if ((locale == null) ||
				(locale.Length < 2))
			{
				return;
			}
			
			string two_letter_code = locale.Substring (0, 2);
			
			if (two_letter_code == "fr")
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (length);
				
				for (int i = 0; i < length; i++)
				{
					int code = Unicode.Bits.GetCode (text[text_offset + i]);
					
					if (code > 0xffff)
					{
						code = 0xffff;
					}
					else if (Unicode.DefaultBreakAnalyzer.IsSpace (code))
					{
						code = ' ';
					}
					
					buffer.Append ((char) code);
				}
				
				foreach (string word in buffer.ToString ().Split (' '))
				{
					if (word.Length > 3)
					{
						short[] break_pos = LanguageEngine.GetHyphenationPositions (word);
					
						foreach (short pos in break_pos)
						{
							breaks[break_offset + pos - 1] = Unicode.BreakInfo.HyphenateGoodChoice;
						}
					}
					
					//	Avance de la longueur du mot et de l'espace qui suit.
					
					break_offset += word.Length + 1;
				}
			}
		}
		
		private static short[] GetHyphenationPositions(string text)
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
	}
}
