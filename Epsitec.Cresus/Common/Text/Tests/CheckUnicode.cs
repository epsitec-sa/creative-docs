//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Summary description for CheckUnicode.
	/// </summary>
	public sealed class CheckUnicode
	{
		public static void RunTests()
		{
			Unicode.BreakAnalyzer analyzer = Unicode.DefaultBreakAnalyzer;
			
			CheckUnicode.CheckText (analyzer, "ABC",			@"ABC");
			CheckUnicode.CheckText (analyzer, "AB C",			@"AB *C");
			CheckUnicode.CheckText (analyzer, "AB  C",			@"AB  *C");
			CheckUnicode.CheckText (analyzer, "AB !  C",		@"AB !  *C");
			CheckUnicode.CheckText (analyzer, "AB :  C",		@"AB :  *C");
			CheckUnicode.CheckText (analyzer, "AB\nC",			@"AB\n#C");
			CheckUnicode.CheckText (analyzer, "AB  \u000c"+"C", "AB  \u000c"+"#C");
			CheckUnicode.CheckText (analyzer, "AB  \u2028  \nC",	@"AB  \r#  \n#C");
			CheckUnicode.CheckText (analyzer, "AB  \u2029\u2028  \nC",	@"AB  \n#\r#  \n#C");
//-			CheckUnicode.CheckText (analyzer, "AB  \r\n  \nC",	@"AB  \r\n#  \n#C");
			CheckUnicode.CheckText (analyzer, "readme.txt",		@"readme.*txt");
			CheckUnicode.CheckText (analyzer, "100.000",		@"100.000");
			CheckUnicode.CheckText (analyzer, "100,000",		@"100,000");
			CheckUnicode.CheckText (analyzer, "100'000",		@"100'000");
			CheckUnicode.CheckText (analyzer, "100/000",		@"100/000");
			
			CheckUnicode.CheckText (analyzer, "abc \u2014 x \u2014 def", "abc *\u2014 x *\u2014 def");
			
			CheckUnicode.CheckText (analyzer, "abc\tdef",		@"abc\t|def");
		}
		
		
		private static void CheckText(Unicode.BreakAnalyzer analyzer, string plainText, string expected)
		{
			ulong[] utf32Text;
			Unicode.BreakInfo[] breaks;
			
			TextConverter.ConvertFromString (plainText, out utf32Text);
			
			breaks = new Unicode.BreakInfo[utf32Text.Length];
			
			analyzer.GenerateBreaks (utf32Text, 0, utf32Text.Length, breaks);
			
//-			System.Diagnostics.Debug.WriteLine (CheckUnicode.GenerateBreakModel (plainText, breaks));
			
			Debug.Assert.IsTrue (CheckUnicode.GenerateBreakModel (plainText, breaks) == expected);
		}
		
		private static string GenerateBreakModel(string text, Unicode.BreakInfo[] breaks)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				
				switch (c)
				{
					case '\u2028':
					case '\r':	buffer.Append (@"\r");	break;
					case '\u2029':
					case '\n':	buffer.Append (@"\n");	break;
					case '\t':	buffer.Append (@"\t");	break;
					case '\\':	buffer.Append (@"\\");	break;
					default:	buffer.Append (c);		break;
				}
				
				switch (breaks[i])
				{
					case Unicode.BreakInfo.No:
					case Unicode.BreakInfo.NoAlpha:
						break;
					case Unicode.BreakInfo.Yes:
						buffer.Append ("#");
						break;
					case Unicode.BreakInfo.Optional:
						buffer.Append ("*");
						break;
					case Unicode.BreakInfo.HorizontalTab:
						buffer.Append ("|");
						break;
				}
			}
			
			return buffer.ToString ();
		}
	}
}
