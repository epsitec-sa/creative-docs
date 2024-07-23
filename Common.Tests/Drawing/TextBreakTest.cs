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

using Epsitec.Common.Drawing;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Drawing
{
    [TestFixture]
    public class TextBreakTest
    {
        [Test]
        public void CheckBreak()
        {
            TextBreak tb = new TextBreak();

            string[] text = new string[3];

            text[0] = "Bonjour, ";
            text[1] = "comment";
            text[2] = " va-t-il ? C'est   juste une question.";

            tb.SetText(string.Concat(text), TextBreakMode.Hyphenate | TextBreakMode.Ellipsis);

            Font[] fonts = new Font[2];

            fonts[0] = Font.GetFont("Tahoma", "Regular");
            fonts[1] = Font.GetFont("Tahoma", "Bold");

            TextBreak.XRun[] runs = new TextBreak.XRun[3];

            runs[0] = new TextBreak.XRun();
            runs[1] = new TextBreak.XRun();
            runs[2] = new TextBreak.XRun();

            runs[0].Locale = "fr-ch";
            runs[1].Locale = "fr-ch";
            runs[2].Locale = "fr-ch";

            runs[0].Font = fonts[0];
            runs[1].Font = fonts[1];
            runs[2].Font = fonts[0];

            runs[0].Length = text[0].Length;
            runs[1].Length = text[1].Length;
            runs[2].Length = text[2].Length;

            runs[0].FontSize = 12.0;
            runs[1].FontSize = 12.0;
            runs[2].FontSize = 12.0;

            tb.SetRuns(runs);

            double space = 300;
            double width;
            int n_char;
            string chunk;

            double last_width = -1;
            int last_n_char = -1;

            while (space > 1)
            {
                space--;

                tb.Rewind();
                tb.GetNextBreak(space, out chunk, out width, out n_char);

                if ((n_char == last_n_char) && (width == last_width))
                {
                    continue;
                }

                double width_2;
                int n_char_2;
                string chunk_2;

                tb.GetNextBreak(1000, out chunk_2, out width_2, out n_char_2);

                System.Console.Out.WriteLine(
                    "{0}, used {1} => {2} chars,\t'{3}'//'{4}'",
                    space,
                    (int)width,
                    n_char,
                    chunk,
                    chunk_2
                );

                last_width = width;
                last_n_char = n_char;
            }
        }
    }
}
