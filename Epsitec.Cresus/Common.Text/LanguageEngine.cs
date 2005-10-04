//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for LanguageEngine.
	/// </summary>
	public sealed class LanguageEngine
	{
		private LanguageEngine()
		{
		}
		
		public static void GenerateHyphens(TextContext context, ulong[] text, int offset, int length, Unicode.BreakInfo[] breaks)
		{
			int run_start  = 0;
			int run_length = 0;
			
			string run_locale = null;
			
			for (int i = 0; i < length; i++)
			{
				Properties.LanguageProperty property;
				
				ulong  code   = text[offset + i];
				string locale = null;
				
				context.GetLanguage (code, out property);
				
				if (property != null)
				{
					double hyphenation = property.Hyphenation;
					
					if (hyphenation > 0)
					{
						locale = property.Locale;
					}
				}
				
				if ((locale != run_locale) ||
					((i > 1) && (breaks[i-1] != Unicode.BreakInfo.No) && (breaks[i-1] != Unicode.BreakInfo.NoAlpha)))
				{
					if ((run_length > 0) &&
						(run_locale != null))
					{
						//	Traite la tranche qui vient de se terminer.
						
						LanguageEngine.GenerateHyphensForRun (context, text, offset + run_start, run_length, run_locale, run_start, breaks);
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
				
				LanguageEngine.GenerateHyphensForRun (context, text, offset + run_start, run_length, run_locale, run_start, breaks);
			}
		}
		
		private static void GenerateHyphensForRun(TextContext context, ulong[] text, int text_offset, int length, string locale, int break_offset, Unicode.BreakInfo[] breaks)
		{
			if (length < 1)
			{
				return;
			}
			
			if ((locale.StartsWith ("fr")) ||
				(locale.StartsWith ("FR")))
			{
				System.Text.StringBuilder word = new System.Text.StringBuilder (length);
				
				for (int i = 0; i < length; i++)
				{
					int code = Unicode.Bits.GetCode (text[text_offset + i]);
					
					if (code > 0xffff)
					{
						code = 0xffff;
					}
					
					word.Append ((char) code);
				}
				
				short[] break_pos = Drawing.TextBreak.GetHyphenationPositions (word.ToString ());
				
				foreach (short pos in break_pos)
				{
					breaks[break_offset + pos - 1] = Unicode.BreakInfo.HyphenateGoodChoice;
				}
			}
		}
	}
}
