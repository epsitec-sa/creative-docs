//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			Unicode.BreakAnalyzer analyzer = new Unicode.BreakAnalyzer ();
			
			System.Diagnostics.Debug.Write ("Loading line break information.");
			analyzer.LoadFile (@"..\..\..\LineBreak.txt");
			System.Diagnostics.Debug.Write ("Done.");
			
			CheckUnicode.CheckText (analyzer, "ABC",			@"ABC");
			CheckUnicode.CheckText (analyzer, "AB C",			@"AB *C");
			CheckUnicode.CheckText (analyzer, "AB  C",			@"AB  *C");
			CheckUnicode.CheckText (analyzer, "AB !  C",		@"AB !  *C");
			CheckUnicode.CheckText (analyzer, "AB :  C",		@"AB :  *C");
			CheckUnicode.CheckText (analyzer, "AB\nC",			@"AB\n#C");
			CheckUnicode.CheckText (analyzer, "AB  \u000c"+"C", "AB  \u000c"+"#C");
			CheckUnicode.CheckText (analyzer, "AB  \r  \nC",	@"AB  \r#  \n#C");
			CheckUnicode.CheckText (analyzer, "AB  \n\r  \nC",	@"AB  \n#\r#  \n#C");
			CheckUnicode.CheckText (analyzer, "AB  \r\n  \nC",	@"AB  \r\n#  \n#C");
		}
		
		
		private static void CheckText(Unicode.BreakAnalyzer analyzer, string plain_text, string expected)
		{
			ulong[] utf32_text;
			Unicode.BreakInfo[] breaks;
			
			TextConverter.ConvertFromString (plain_text, out utf32_text);
			
			breaks = new Unicode.BreakInfo[utf32_text.Length];
			
			analyzer.GenerateBreaks (utf32_text, 0, utf32_text.Length, breaks);
			
			System.Diagnostics.Debug.WriteLine (CheckUnicode.GenerateBreakModel (plain_text, breaks));
			
			Debug.Assert.IsTrue (CheckUnicode.GenerateBreakModel (plain_text, breaks) == expected);
		}
		
		private static string GenerateBreakModel(string text, Unicode.BreakInfo[] breaks)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				
				switch (c)
				{
					case '\r':	buffer.Append (@"\r");	break;
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
				}
			}
			
			return buffer.ToString ();
		}
	}
}
