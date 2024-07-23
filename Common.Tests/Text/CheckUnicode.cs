/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Text;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// Summary description for CheckUnicode.
    /// </summary>
    [TestFixture]
    public sealed class CheckUnicode
    {
        [Test]
        public static void CheckUnicodeText()
        {
            Unicode.BreakAnalyzer analyzer = Unicode.DefaultBreakAnalyzer;

            CheckUnicode.CheckText(analyzer, "ABC", @"ABC");
            CheckUnicode.CheckText(analyzer, "AB C", @"AB *C");
            CheckUnicode.CheckText(analyzer, "AB  C", @"AB  *C");
            CheckUnicode.CheckText(analyzer, "AB !  C", @"AB !  *C");
            CheckUnicode.CheckText(analyzer, "AB :  C", @"AB :  *C");
            CheckUnicode.CheckText(analyzer, "AB\nC", @"AB\n#C");
            CheckUnicode.CheckText(analyzer, "AB  \u000c" + "C", "AB  \u000c" + "#C");
            CheckUnicode.CheckText(analyzer, "AB  \u2028  \nC", @"AB  \r#  \n#C");
            CheckUnicode.CheckText(analyzer, "AB  \u2029\u2028  \nC", @"AB  \n#\r#  \n#C");
            //-			CheckUnicode.CheckText (analyzer, "AB  \r\n  \nC",	@"AB  \r\n#  \n#C");
            CheckUnicode.CheckText(analyzer, "readme.txt", @"readme.*txt");
            CheckUnicode.CheckText(analyzer, "100.000", @"100.000");
            CheckUnicode.CheckText(analyzer, "100,000", @"100,000");
            CheckUnicode.CheckText(analyzer, "100'000", @"100'000");
            CheckUnicode.CheckText(analyzer, "100/000", @"100/000");

            CheckUnicode.CheckText(
                analyzer,
                "abc \u2014 x \u2014 def",
                "abc *\u2014 x *\u2014 def"
            );

            CheckUnicode.CheckText(analyzer, "abc\tdef", @"abc\t|def");
        }

        private static void CheckText(
            Unicode.BreakAnalyzer analyzer,
            string plainText,
            string expected
        )
        {
            ulong[] utf32Text;
            Unicode.BreakInfo[] breaks;

            TextConverter.ConvertFromString(plainText, out utf32Text);

            breaks = new Unicode.BreakInfo[utf32Text.Length];

            analyzer.GenerateBreaks(utf32Text, 0, utf32Text.Length, breaks);

            //-			System.Diagnostics.Debug.WriteLine (CheckUnicode.GenerateBreakModel (plainText, breaks));

            Assert.IsTrue(CheckUnicode.GenerateBreakModel(plainText, breaks) == expected);
        }

        private static string GenerateBreakModel(string text, Unicode.BreakInfo[] breaks)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                switch (c)
                {
                    case '\u2028':
                    case '\r':
                        buffer.Append(@"\r");
                        break;
                    case '\u2029':
                    case '\n':
                        buffer.Append(@"\n");
                        break;
                    case '\t':
                        buffer.Append(@"\t");
                        break;
                    case '\\':
                        buffer.Append(@"\\");
                        break;
                    default:
                        buffer.Append(c);
                        break;
                }

                switch (breaks[i])
                {
                    case Unicode.BreakInfo.No:
                    case Unicode.BreakInfo.NoAlpha:
                        break;
                    case Unicode.BreakInfo.Yes:
                        buffer.Append("#");
                        break;
                    case Unicode.BreakInfo.Optional:
                        buffer.Append("*");
                        break;
                    case Unicode.BreakInfo.HorizontalTab:
                        buffer.Append("|");
                        break;
                }
            }

            return buffer.ToString();
        }
    }
}
