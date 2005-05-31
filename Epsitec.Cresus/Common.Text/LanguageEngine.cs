//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for LanguageEngine.
	/// </summary>
	public class LanguageEngine
	{
		public LanguageEngine()
		{
		}
		
		public static void GenerateHyphens(Context context, ulong[] text, int offset, int length, Unicode.BreakInfo[] breaks)
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
						
						LanguageEngine.GenerateHyphensForRun (context, text, offset + run_start, run_length, run_locale, breaks);
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
				
				LanguageEngine.GenerateHyphensForRun (context, text, offset + run_start, run_length, run_locale, breaks);
			}
		}
		
		private static void GenerateHyphensForRun(Context context, ulong[] text, int offset, int length, string locale, Unicode.BreakInfo[] breaks)
		{
			if (length < 1)
			{
				return;
			}
			
			System.Text.StringBuilder word = new System.Text.StringBuilder (length);
			
			for (int i = 0; i < length; i++)
			{
				int code = Unicode.Bits.GetCode (text[offset + i]);
				
				if (code > 0xffff)
				{
					code = 0xffff;
				}
				
				word.Append ((char) code);
			}
			
			System.Diagnostics.Debug.WriteLine ("<" + word.ToString () + ">");
		}
	}
}
