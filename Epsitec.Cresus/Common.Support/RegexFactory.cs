//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;

namespace Epsitec.Common.Support
{
	public enum PredefinedRegex
	{
		None,
		
		Alpha,
		AlphaNum,
		AlphaNumDot,
		
		FileName,
		PathName,
		
		ResourceFullName,				//	"abc123#x.y3.z"
		ResourceBundleName,				//	"abc"
		ResourceFieldName,				//	"x.y3.z"
		
		InvariantDecimalNum,
		LocalizedDecimalNum
	}
	
	/// <summary>
	/// La classe RegexFactory permet de construire des objets "regex" à partir de
	/// textes simples avec des jokers "*"...
	/// </summary>
	public class RegexFactory
	{
		private RegexFactory()
		{
		}
		
		static RegexFactory()
		{
			RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
			
			RegexFactory.alpha_name     = new Regex (@"^[a-zA-Z]+$", options);
			RegexFactory.alpha_num_name = new Regex (@"^[a-zA-Z_][a-zA-Z0-9_]*$", options);
			RegexFactory.alpha_dot_name = new Regex (@"^[a-zA-Z_]([a-zA-Z0-9_]*((?![\.]$)(?<X>[\.])(?!\k<X>))*)*$", options);
			RegexFactory.file_name      = new Regex (@"^[a-zA-Z0-9_\""\'\$\+\-\=\@\&\(\)\!]([a-zA-Z0-9_\""\'\$\+\-\=\@\&\(\)\!]*((?![\. ]$)(?<X>[\. ])(?!\k<X>))*)*$", options);
			RegexFactory.path_name      = new Regex (@"^[a-zA-Z0-9_\""\'\$\+\-\=\@\&\(\)\!]([a-zA-Z0-9_\""\'\$\+\-\=\@\&\(\)\!]*((?![\.\/\ ]$)(?<X>[\.\/\ ])(?!\k<X>))*)*$", options);
			RegexFactory.r_full_name    = new Regex (@"^([a-zA-Z_][a-zA-Z0-9_]*)(\.([a-zA-Z0-9_]+))*" + @"(\#([a-zA-Z_][a-zA-Z0-9_]*)(\.([a-zA-Z0-9_]+))*)*" + @"(\[[0-9]{1,4}\])?" + @"$", options);
			RegexFactory.r_bundle_name  = new Regex (@"^([a-zA-Z_][a-zA-Z0-9_]*)(\.([a-zA-Z0-9_]+))*$", options);
			RegexFactory.r_field_name   = new Regex (@"^([a-zA-Z_][a-zA-Z0-9_]*)(\.([a-zA-Z0-9_]+))*$", options);
			
			//	L'expression régulière utilisée pour déterminer si un nombre est formaté correctement
			//	devrait être recalculée chaque fois que la culture active change, mais on ne le fait
			//	pas encore :
			
			char decimal_separator;
			
			decimal_separator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
			
			//	TODO: regénérer loc_decimal_num à chaque changement de culture
			
			RegexFactory.loc_decimal_num = new Regex (@"^(\-|\+)?((\d{1,12}(\" + decimal_separator +
				/**/								 @"\d{0,12})?0*)|(\d{0,12}\" + decimal_separator +
				/**/								 @"(\d{0,12})?0*))$", options);
			
			decimal_separator = System.Globalization.CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator[0];
			
			RegexFactory.inv_decimal_num = new Regex (@"^(\-|\+)?((\d{1,12}(\" + decimal_separator +
				/**/								 @"\d{0,12})?0*)|(\d{0,12}\" + decimal_separator +
				/**/								 @"(\d{0,12})?0*))$", options);
		}
		
		
		public static Regex FromSimpleJoker(string pattern)
		{
			return RegexFactory.FromSimpleJoker (pattern, Options.None);
		}
		
		public static Regex FromSimpleJoker(string pattern, Options options)
		{
			RegexOptions              regex_options = RegexOptions.ExplicitCapture;
			System.Text.StringBuilder regex_pattern = new System.Text.StringBuilder ();
			
			bool escape     = false;
			bool capture    = (options & Options.Capture) != 0;
			int  capture_id = 1;
			
			if ((options & Options.IgnoreCase) != 0)	regex_options |= RegexOptions.IgnoreCase;
			if ((options & Options.Compiled) != 0)		regex_options |= RegexOptions.Compiled;
			
			regex_pattern.Append (@"\A");						//	force ancrage au début
			
			for (int i = 0; i < pattern.Length; i++)
			{
				char c = pattern[i];
				
				if (escape)
				{
					regex_pattern.Append (Regex.Escape (pattern.Substring (i, 1)));
					escape = false;
				}
				else if (c == '\\')
				{
					escape = true;
				}
				else if (c == '*')
				{
					if (capture)
					{
						regex_pattern.Append (@"(?<");			//	groupe nommé..
						regex_pattern.Append (capture_id++);	//	..avec comme nom le range 'capture_id'..
						regex_pattern.Append (@">(.){0,}?)");	//	..et acceptant zero ou plus de caractère (minimum possible)
					}
					else
					{
						regex_pattern.Append (@"(.){0,}?");		//	zero ou plus de caractères (minimum possible)
					}
				}
				else if (c == '?')
				{
					if (capture)
					{
						regex_pattern.Append (@"(?<");			//	groupe nommé..
						regex_pattern.Append (capture_id++);	//	..avec comme nom le range 'capture_id'..
						regex_pattern.Append (@">(.){1})");		//	..et acceptant exactement un caractère
					}
					else
					{
						regex_pattern.Append (@"(.){1}");		//	exactement un caractère
					}
				}
				else
				{
					regex_pattern.Append (Regex.Escape (pattern.Substring (i, 1)));
				}
			}
			
			regex_pattern.Append (@"\z");						//	force ancrage à la fin
			
			return new Regex (regex_pattern.ToString (), regex_options);
		}
		
		
		public static Regex	FromPredefinedRegex(PredefinedRegex regex)
		{
			switch (regex)
			{
				case PredefinedRegex.Alpha:					return RegexFactory.AlphaName;
				case PredefinedRegex.AlphaNum:				return RegexFactory.AlphaNumName;
				case PredefinedRegex.AlphaNumDot:			return RegexFactory.AlphaNumDotName;
				case PredefinedRegex.FileName:				return RegexFactory.FileName;
				case PredefinedRegex.PathName:				return RegexFactory.PathName;
				case PredefinedRegex.ResourceFullName:		return RegexFactory.ResourceFullName;
				case PredefinedRegex.ResourceBundleName:	return RegexFactory.ResourceBundleName;
				case PredefinedRegex.ResourceFieldName:		return RegexFactory.ResourceFieldName;
				case PredefinedRegex.InvariantDecimalNum:	return RegexFactory.InvariantDecimalNum;
				case PredefinedRegex.LocalizedDecimalNum:	return RegexFactory.LocalizedDecimalNum;
			}
			
			return null;
		}
		
		
		public static Regex						AlphaName
		{
			get
			{
				return RegexFactory.alpha_name;
			}
		}
		
		public static Regex						AlphaNumName
		{
			get
			{
				return RegexFactory.alpha_num_name;
			}
		}
		
		public static Regex						AlphaNumDotName
		{
			get
			{
				return RegexFactory.alpha_dot_name;
			}
		}
		
		public static Regex						FileName
		{
			get
			{
				return RegexFactory.file_name;
			}
		}
		
		public static Regex						PathName
		{
			get
			{
				return RegexFactory.path_name;
			}
		}
		
		public static Regex						ResourceFullName
		{
			get
			{
				return RegexFactory.r_full_name;
			}
		}
		
		public static Regex						ResourceBundleName
		{
			get
			{
				return RegexFactory.r_bundle_name;
			}
		}
		
		public static Regex						ResourceFieldName
		{
			get
			{
				return RegexFactory.r_field_name;
			}
		}
		
		public static Regex						LocalizedDecimalNum
		{
			get
			{
				return RegexFactory.loc_decimal_num;
			}
		}
		
		public static Regex						InvariantDecimalNum
		{
			get
			{
				return RegexFactory.inv_decimal_num;
			}
		}
		
		
		
		[System.Flags] public enum Options
		{
			None			= 0,
			IgnoreCase		= 0x0001,
			Compiled		= 0x0002,
			Capture			= 0x0004,
		}
		
		
		private static Regex					alpha_name;
		private static Regex					alpha_num_name;
		private static Regex					alpha_dot_name;
		private static Regex					file_name;
		private static Regex					path_name;
		private static Regex					r_full_name;
		private static Regex					r_bundle_name;
		private static Regex					r_field_name;
		private static Regex					loc_decimal_num;
		private static Regex					inv_decimal_num;
	}
}
