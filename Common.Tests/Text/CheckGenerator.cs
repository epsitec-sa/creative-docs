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
using Epsitec.Common.Text.Cursors;
using Epsitec.Common.Text.Properties;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// La batterie de tests CheckGenerator vérifie le bon fonctionnement des
    /// générateurs.
    /// </summary>
    [TestFixture]
    public sealed class CheckGenerator
    {

        [Test]
        public static void TestGenerator()
        {
            Generator generator = new Generator("Test");

            Generator.Sequence sNum = Generator.CreateSequence(Generator.SequenceType.Numeric);
            Generator.Sequence sAlf = Generator.CreateSequence(Generator.SequenceType.Alphabetic);
            Generator.Sequence sALF = Generator.CreateSequence(Generator.SequenceType.Alphabetic);

            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            sNum.Suffix = ".";
            sAlf.Suffix = ")";
            sALF.Prefix = "<";
            sALF.Casing = Generator.Casing.Upper;
            sALF.Suffix = ">";

            buffer.Append(sNum.Prefix == null ? "" : sNum.Prefix);
            buffer.Append(sNum.GenerateText(1, System.Globalization.CultureInfo.CurrentCulture));
            buffer.Append(sNum.Suffix == null ? "" : sNum.Suffix);
            buffer.Append(sNum.Prefix == null ? "" : sNum.Prefix);
            buffer.Append(sNum.GenerateText(10, System.Globalization.CultureInfo.CurrentCulture));
            buffer.Append(sNum.Suffix == null ? "" : sNum.Suffix);
            buffer.Append(sAlf.Prefix == null ? "" : sAlf.Prefix);
            buffer.Append(sAlf.GenerateText(3, System.Globalization.CultureInfo.CurrentCulture));
            buffer.Append(sAlf.Suffix == null ? "" : sAlf.Suffix);
            buffer.Append(sALF.Prefix == null ? "" : sALF.Prefix);
            buffer.Append(sALF.GenerateText(1, System.Globalization.CultureInfo.CurrentCulture));
            buffer.Append(sALF.Suffix == null ? "" : sALF.Suffix);

            Assert.IsTrue(buffer.ToString() == "1.10.c)<A>");

            generator.Add(sNum);
            generator.Add(sNum);
            generator.Add(sAlf);
            generator.Add(sNum);

            int[] ranks = new int[] { 1, 10, 3, 2, 3, 4 };

            Assert.IsTrue(
                "1.10.c)2.3.4."
                    == generator.GenerateTextString(
                        ranks,
                        System.Globalization.CultureInfo.CurrentCulture
                    )
            );

            Generator.Series series;
            generator.StartVector = null;

            series = generator.NewSeries(System.Globalization.CultureInfo.CurrentCulture);

            Assert.IsTrue("1." == series.GetNextTextString(0));
            Assert.IsTrue("2." == series.GetNextTextString(0));
            Assert.IsTrue("2.1." == series.GetNextTextString(1));
            Assert.IsTrue("2.1.a)1." == series.GetNextTextString(3));
            Assert.IsTrue("2.1.b)" == series.GetNextTextString(2));
            Assert.IsTrue("2.1.c)" == series.GetNextTextString(2));
            Assert.IsTrue("3." == series.GetNextTextString(0));
            Assert.IsTrue("3.1.a)1.1.1." == series.GetNextTextString(5));
        }

        [Test]
        public static void TestTextStory()
        {
            TextStory story = new TextStory();
            ICursor cursor = new SimpleCursor();

            story.NewCursor(cursor);

            ulong[] text;
            System.Collections.ArrayList properties = new System.Collections.ArrayList();

            properties.Add(new FontProperty("Verdana", "Regular"));
            properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));

            TextStyle style = story.StyleList.NewTextStyle(
                null,
                "Normal",
                TextStyleClass.Paragraph,
                properties
            );

            GeneratorProperty g1A = new GeneratorProperty("G1", 0, 101);
            GeneratorProperty g1B = new GeneratorProperty("G1", 0, 102);
            GeneratorProperty g1C = new GeneratorProperty("G1", 1, 103);
            GeneratorProperty g1D = new GeneratorProperty("G1", 2, 104);
            GeneratorProperty g1E = new GeneratorProperty("G1", 1, 105);

            properties.Clear();
            properties.Add(g1A);
            properties.Add(new AutoTextProperty("g1a"));

            story.ConvertToStyledText("X", style, properties, out text);
            story.InsertText(cursor, text);

            story.ConvertToStyledText("Chapitre premier\n", style, null, out text);
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(g1B);
            properties.Add(new AutoTextProperty("g1b"));

            story.ConvertToStyledText("X", style, properties, out text);
            story.InsertText(cursor, text);

            story.ConvertToStyledText("Chapitre second\n", style, null, out text);
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(g1C);
            properties.Add(new AutoTextProperty("g1c"));

            story.ConvertToStyledText("X", style, properties, out text);
            story.InsertText(cursor, text);

            story.ConvertToStyledText("Introduction\n", style, null, out text);
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(g1D);
            properties.Add(new AutoTextProperty("g1d"));

            story.ConvertToStyledText("X", style, properties, out text);
            story.InsertText(cursor, text);

            story.ConvertToStyledText("Plan\n", style, null, out text);
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(g1E);
            properties.Add(new AutoTextProperty("g1e"));

            story.ConvertToStyledText("X", style, properties, out text);
            story.InsertText(cursor, text);

            story.ConvertToStyledText("Blabla...\n", style, null, out text);
            story.InsertText(cursor, text);

            text = new ulong[story.TextLength];
            story.SetCursorPosition(cursor, 0);
            story.ReadText(cursor, story.TextLength, text);

            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));

            Generator generator = story.TextContext.GeneratorList.NewGenerator("G1");

            generator.Add(Generator.CreateSequence(Generator.SequenceType.Numeric, "", "."));
            generator.Add(Generator.CreateSequence(Generator.SequenceType.Numeric, "", "."));
            generator.Add(Generator.CreateSequence(Generator.SequenceType.Alphabetic));

            int count1 = generator.UpdateAllFields(
                story,
                null,
                System.Globalization.CultureInfo.CurrentCulture
            );
            int count2 = generator.UpdateAllFields(
                story,
                null,
                System.Globalization.CultureInfo.CurrentCulture
            );

            generator.StartVector = new int[] { 8, 12 };

            int count3 = generator.UpdateAllFields(
                story,
                null,
                System.Globalization.CultureInfo.CurrentCulture
            );

            Assert.IsTrue(count1 == 5);
            Assert.IsTrue(count2 == 0);
            Assert.IsTrue(count3 == 5);

            text = new ulong[story.TextLength];
            story.SetCursorPosition(cursor, 0);
            story.ReadText(cursor, story.TextLength, text);

            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));
        }
    }
}
