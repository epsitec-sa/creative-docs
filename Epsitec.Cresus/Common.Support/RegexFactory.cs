//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

using System.Text.RegularExpressions;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe RegexFactory permet de construire des objets "regex" � partir de
	/// textes simples avec des jokers "*"...
	/// </summary>
	public class RegexFactory
	{
		private RegexFactory()
		{
		}
		
		public static System.Text.RegularExpressions.Regex FromSimpleJoker(string pattern, Options options)
		{
			RegexOptions              regex_options = RegexOptions.ExplicitCapture;
			System.Text.StringBuilder regex_pattern = new System.Text.StringBuilder ();
			
			bool capture    = (options & Options.Capture) != 0;
			int  capture_id = 1;
			
			if ((options & Options.IgnoreCase) != 0)	regex_options |= RegexOptions.IgnoreCase;
			if ((options & Options.Compiled) != 0)		regex_options |= RegexOptions.Compiled;
			
			regex_pattern.Append (@"\A");						//	force ancrage au d�but
			
			for (int i = 0; i < pattern.Length; i++)
			{
				char c = pattern[i];
				if (c == '*')
				{
					if (capture)
					{
						regex_pattern.Append (@"(?<");			//	groupe nomm�..
						regex_pattern.Append (capture_id++);	//	..avec comme nom le range 'capture_id'..
						regex_pattern.Append (@">(.){0,}?)");	//	..et acceptant zero ou plus de caract�re (minimum possible)
					}
					else
					{
						regex_pattern.Append (@"(.){0,}?");		//	zero ou plus de caract�res (minimum possible)
					}
				}
				else if (c == '?')
				{
					if (capture)
					{
						regex_pattern.Append (@"(?<");			//	groupe nomm�..
						regex_pattern.Append (capture_id++);	//	..avec comme nom le range 'capture_id'..
						regex_pattern.Append (@">(.){1})");		//	..et acceptant exactement un caract�re
					}
					else
					{
						regex_pattern.Append (@"(.){1}");		//	exactement un caract�re
					}
				}
				else
				{
					regex_pattern.Append (Regex.Escape (pattern.Substring (i, 1)));
				}
			}
			
			regex_pattern.Append (@"\z");						//	force ancrage � la fin
			
			return new Regex (regex_pattern.ToString (), regex_options);
		}
		
		[System.Flags] public enum Options
		{
			None			= 0,
			IgnoreCase		= 0x0001,
			Compiled		= 0x0002,
			Capture			= 0x0004,
		}
	}
}
