//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

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
			int runStart  = 0;
			int runLength = 0;
			
			string runLocale = null;
			
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
				
				if ((locale != runLocale) ||
					((i > 1) && (breaks[i-1] != Unicode.BreakInfo.No) && (breaks[i-1] != Unicode.BreakInfo.NoAlpha)))
				{
					if ((runLength > 0) &&
						(runLocale != null))
					{
						//	Traite la tranche qui vient de se terminer.
						
						LanguageEngine.GenerateHyphensForRun (text, offset + runStart, runLength, runLocale, runStart, breaks);
					}
					
					runStart  = i;
					runLength = 0;
					runLocale = locale;
				}
				
				runLength++;
			}
			
			if ((runLength > 0) &&
				(runLocale != null))
			{
				//	Traite la tranche finale.
				
				LanguageEngine.GenerateHyphensForRun (text, offset + runStart, runLength, runLocale, runStart, breaks);
			}
		}
		
		private static void GenerateHyphensForRun(ulong[] text, int textOffset, int length, string locale, int breakOffset, Unicode.BreakInfo[] breaks)
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
			
			string twoLetterCode = locale.Substring (0, 2);
			
			if (twoLetterCode == "fr")
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (length);
				
				for (int i = 0; i < length; i++)
				{
					int code = Unicode.Bits.GetCode (text[textOffset + i]);
					
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
						foreach (int pos in BreakEngines.FrenchWordBreakEngine.Break (word))
						{
							breaks[breakOffset + pos - 1] = Unicode.BreakInfo.HyphenateGoodChoice;
						}
					}
					
					//	Avance de la longueur du mot et de l'espace qui suit.
					
					breakOffset += word.Length + 1;
				}
			}
		}
	}
}
