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
using Epsitec.Common.Text.Internal;
using Epsitec.Common.Text.Properties;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// Vérifie le bon fonctionnement des propriétés.
    /// </summary>
    [TestFixture]
    public sealed class CheckProperties
    {

        [Test]
        public static void TestFont()
        {
            FontProperty fontA = new FontProperty("Futura", "Roman");
            FontProperty fontB = new FontProperty(null, "Heavy");
            FontProperty fontC = new FontProperty("Arial", null);

            FontProperty fontAb = fontA.GetCombination(fontB) as FontProperty;
            FontProperty fontAc = fontA.GetCombination(fontC) as FontProperty;

            Assert.IsTrue(fontAb.FaceName == "Futura");
            Assert.IsTrue(fontAb.StyleName == "Heavy");
            Assert.IsTrue(fontAc.FaceName == "Arial");
            Assert.IsTrue(fontAc.StyleName == "Roman");

            Assert.IsTrue(fontA.ToString() == "Futura/Roman/[null]");
            Assert.IsTrue(fontB.ToString() == "[null]/Heavy/[null]");
            Assert.IsTrue(fontC.ToString() == "Arial/[null]/[null]");
        }

        [Test]
        public static void TestFontSize()
        {
            FontSizeProperty fontSizeA = new FontSizeProperty(
                12.0,
                SizeUnits.Points
            );
            FontSizeProperty fontSizeB = new FontSizeProperty(
                50.0 / 100,
                SizeUnits.Percent
            );
            FontSizeProperty fontSizeC = new FontSizeProperty(
                -2.0,
                SizeUnits.DeltaPoints
            );
            FontSizeProperty fontSizeD = new FontSizeProperty(
                200.0 / 100,
                SizeUnits.Percent
            );

            FontSizeProperty fontSizeAb =
                fontSizeA.GetCombination(fontSizeB) as FontSizeProperty;
            FontSizeProperty fontSizeBa =
                fontSizeB.GetCombination(fontSizeA) as FontSizeProperty;
            FontSizeProperty fontSizeAc =
                fontSizeA.GetCombination(fontSizeC) as FontSizeProperty;
            FontSizeProperty fontSizeCb =
                fontSizeC.GetCombination(fontSizeB) as FontSizeProperty;
            FontSizeProperty fontSizeBd =
                fontSizeB.GetCombination(fontSizeD) as FontSizeProperty;

            Assert.IsTrue(fontSizeAb.ToString() == "6/pt/[NaN]");
            Assert.IsTrue(fontSizeBa.ToString() == "12/pt/[NaN]");
            Assert.IsTrue(fontSizeAc.ToString() == "10/pt/[NaN]");
            Assert.IsTrue(fontSizeCb.ToString() == "-1/+pt/[NaN]");
            Assert.IsTrue(fontSizeBd.ToString() == "1/%/[NaN]");
        }

        [Test]
        public static void TestMargins()
        {
            MarginsProperty marginsA = new MarginsProperty();
            MarginsProperty marginsB = new MarginsProperty(
                15.0,
                20.0,
                0.0,
                0.0,
                SizeUnits.Points,
                0,
                0,
                0,
                0,
                0,
                ThreeState.False
            );

            MarginsProperty marginsC =
                marginsA.GetCombination(marginsB) as MarginsProperty;
            MarginsProperty marginsD =
                marginsB.GetCombination(marginsA) as MarginsProperty;

            marginsA = new MarginsProperty(
                double.NaN,
                10,
                double.NaN,
                10,
                SizeUnits.Points,
                0.0,
                0.0,
                0.0,
                0.0,
                0.0,
                ThreeState.Undefined
            );

            MarginsProperty marginsE =
                marginsA.GetCombination(marginsB) as MarginsProperty;
            MarginsProperty marginsF =
                marginsB.GetCombination(marginsA) as MarginsProperty;

            Assert.IsTrue(marginsC.LeftMarginFirstLine == 15.0);
            Assert.IsTrue(marginsC.LeftMarginBody == 20.0);
            Assert.IsTrue(marginsC.RightMarginBody == 0.0);

            Assert.IsTrue(marginsD.LeftMarginFirstLine == 15.0);
            Assert.IsTrue(marginsD.LeftMarginBody == 20.0);
            Assert.IsTrue(marginsD.RightMarginBody == 0.0);

            Assert.IsTrue(marginsE.LeftMarginFirstLine == 15.0);
            Assert.IsTrue(marginsE.LeftMarginBody == 20.0);
            Assert.IsTrue(marginsE.RightMarginBody == 0.0);

            Assert.IsTrue(marginsF.LeftMarginFirstLine == 15.0);
            Assert.IsTrue(marginsF.LeftMarginBody == 10.0);
            Assert.IsTrue(marginsF.RightMarginBody == 10.0);

            Assert.IsTrue(
                marginsA.ToString() == "[NaN]/10/[NaN]/10/pt/0/0/0/0/0/[?]/-1/[null]"
            );
            Assert.IsTrue(marginsB.ToString() == "15/20/0/0/pt/0/0/0/0/0/[false]/-1/[null]");
        }

        [Test]
        public static void TestXlines()
        {
            TextStory story = new TextStory();

            ulong[] text;
            System.Collections.ArrayList properties = new System.Collections.ArrayList();

            properties.Add(new FontProperty("Arial", "Regular"));
            properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
            properties.Add(
                new UnderlineProperty(
                    double.NaN,
                    SizeUnits.None,
                    double.NaN,
                    SizeUnits.None,
                    "underline",
                    "-"
                )
            );
            properties.Add(
                new StrikeoutProperty(
                    double.NaN,
                    SizeUnits.None,
                    double.NaN,
                    SizeUnits.None,
                    "underline",
                    "color=red"
                )
            );
            properties.Add(
                new TextBoxProperty(
                    double.NaN,
                    SizeUnits.None,
                    double.NaN,
                    SizeUnits.None,
                    "frame",
                    "backcolor=yellow;color=black"
                )
            );
            properties.Add(
                new OverlineProperty(
                    double.NaN,
                    SizeUnits.None,
                    2.0,
                    SizeUnits.Points,
                    "underline",
                    "color=red"
                )
            );
            properties.Add(new LinkProperty("http://www.epsitec.ch"));
            properties.Add(new LinkProperty("mailto:epsitec@epsitec.ch"));

            story.ConvertToStyledText("Abc", properties, out text);

            AbstractXlineProperty[] underlines;
            LinkProperty[] links;

            story.TextContext.GetXlines(text[0], out underlines);
            story.TextContext.GetLinks(text[0], out links);

            Assert.IsTrue(underlines.Length == 4);
            Assert.IsTrue(links.Length == 2);

            //			System.Array.Sort (underlines, AbstractXlineProperty.Comparer);
            //			System.Array.Sort (links, LinkProperty.Comparer);

            Assert.IsTrue(underlines[0].WellKnownType == WellKnownType.Underline);
            Assert.IsTrue(underlines[1].WellKnownType == WellKnownType.Strikeout);
            Assert.IsTrue(underlines[2].WellKnownType == WellKnownType.Overline);
            Assert.IsTrue(underlines[3].WellKnownType == WellKnownType.TextBox);

            Assert.IsTrue(underlines[0].DrawStyle == "-");
            Assert.IsTrue(underlines[1].DrawStyle == "color=red");
            Assert.IsTrue(underlines[2].DrawStyle == "color=red");
            Assert.IsTrue(underlines[3].DrawStyle == "backcolor=yellow;color=black");

            Assert.IsTrue(underlines[1].ThicknessUnits == SizeUnits.None);
            Assert.IsTrue(underlines[2].ThicknessUnits == SizeUnits.Points);

            Assert.IsTrue(links[0].Link == "http://www.epsitec.ch");
            Assert.IsTrue(links[1].Link == "mailto:epsitec@epsitec.ch");
        }

        [Test]
        public static void TestUserTags()
        {
            TextStory story = new TextStory();

            ulong[] text;
            System.Collections.ArrayList properties = new System.Collections.ArrayList();

            properties.Add(new FontProperty("Arial", "Regular"));
            properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
            properties.Add(new UserTagProperty("x", "foo", 0));
            properties.Add(new UserTagProperty("x", "bar", 1));
            properties.Add(new UserTagProperty("Comment", "Hello :-)", 2));

            story.ConvertToStyledText("Abc", properties, out text);

            UserTagProperty[] usertags;

            story.TextContext.GetUserTags(text[0], out usertags);

            Assert.IsTrue(usertags.Length == 3);

            Assert.IsTrue(usertags[0].TagType == "Comment");
            Assert.IsTrue(usertags[1].TagType == "x");
            Assert.IsTrue(usertags[2].TagType == "x");

            Assert.IsTrue(usertags[0].TagData == "Hello :-)");
            Assert.IsTrue(usertags[1].TagData == "bar");
            Assert.IsTrue(usertags[2].TagData == "foo");

            Assert.IsTrue(usertags[0].Id == 2);
            Assert.IsTrue(usertags[1].Id == 1);
            Assert.IsTrue(usertags[2].Id == 0);
        }

        [Test]
        public static void TestSerialization()
        {
            FontProperty p1 = new FontProperty("Futura", "Roman");
            FontSizeProperty p2 = new FontSizeProperty(
                12.0,
                SizeUnits.Points
            );
            MarginsProperty p3 = new MarginsProperty(
                15.0,
                20.0,
                0.0,
                0.0,
                SizeUnits.Points,
                0,
                0,
                0,
                0,
                0,
                ThreeState.False
            );

            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            int end1,
                end2,
                end3;

            Property.SerializeToText(buffer, p1);
            end1 = buffer.Length;
            Property.SerializeToText(buffer, p2);
            end2 = buffer.Length;
            Property.SerializeToText(buffer, p3);
            end3 = buffer.Length;

            Property p;

            FontProperty p1x;
            FontSizeProperty p2x;
            MarginsProperty p3x;

            Property.DeserializeFromText(null, buffer.ToString(), 0, end1, out p);
            p1x = p as FontProperty;
            Property.DeserializeFromText(null, buffer.ToString(), end1, end2 - end1, out p);
            p2x = p as FontSizeProperty;
            Property.DeserializeFromText(null, buffer.ToString(), end2, end3 - end2, out p);
            p3x = p as MarginsProperty;

            Assert.IsNotNull(p1x);
            Assert.IsNotNull(p2x);
            Assert.IsNotNull(p3x);

            Assert.IsTrue(p1.FaceName == p1x.FaceName);
            Assert.IsTrue(p1.StyleName == p1x.StyleName);

            Assert.IsTrue(p2.Size == p2x.Size);
            Assert.IsTrue(p2.Units == p2x.Units);

            Assert.IsTrue(p3.EnableHyphenation == p3x.EnableHyphenation);
        }

        [Test]
        public static void TestGeneratorProperties()
        {
            TextStory story = new TextStory();
            TempCursor cursor = new TempCursor();

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

            story.ConvertToStyledText("Texte ", style, null, out text); //	6
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(new GeneratorProperty("G1", 0, 1));
            story.ConvertToStyledText("généré", style, properties, out text); //	12
            story.InsertText(cursor, text);

            story.ConvertToStyledText(" automatiquement ", style, null, out text); //	29
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(new GeneratorProperty("G1", 0, 2));
            story.ConvertToStyledText("[1]", style, properties, out text); //	32
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(new GeneratorProperty("G1", 0, 3));
            story.ConvertToStyledText("[2]", style, properties, out text); //	35
            story.InsertText(cursor, text);

            story.ConvertToStyledText("...\n", style, null, out text);
            story.InsertText(cursor, text);

            text = new ulong[story.TextLength];
            story.SetCursorPosition(cursor, 0);
            story.ReadText(cursor, text.Length, text);

            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));

            GeneratorCursor[] cursors = GeneratorEnumerator.CreateCursors(
                story,
                "G1"
            );

            Assert.IsTrue(cursors.Length == 3);
            Assert.IsTrue(story.GetCursorPosition(cursors[0]) == 6);
            Assert.IsTrue(story.GetCursorPosition(cursors[1]) == 29);
            Assert.IsTrue(story.GetCursorPosition(cursors[2]) == 32);

            properties.Clear();
            properties.Add(new GeneratorProperty("G1", 0, 4));
            story.ConvertToStyledText("[1]", style, properties, out text); //	3
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(new GeneratorProperty("G1", 0, 5));
            story.ConvertToStyledText("[2]", style, properties, out text); //	6
            story.InsertText(cursor, text);

            cursors = GeneratorEnumerator.CreateCursors(story, "G1");

            Assert.IsTrue(cursors.Length == 5);
            Assert.IsTrue(story.GetCursorPosition(cursors[0]) == 0);
            Assert.IsTrue(story.GetCursorPosition(cursors[1]) == 3);
            Assert.IsTrue(story.GetCursorPosition(cursors[2]) == 6 + 6);
            Assert.IsTrue(story.GetCursorPosition(cursors[3]) == 29 + 6);
            Assert.IsTrue(story.GetCursorPosition(cursors[4]) == 32 + 6);
        }

        [Test]
        public static void TestTraverseText()
        {
            TextStory story = new TextStory();
            TempCursor cursor = new TempCursor();

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

            story.ConvertToStyledText("1234567890", style, null, out text);
            story.InsertText(cursor, text);

            Traverser traverser = new Traverser();

            TextStory.CodeCallback cbFalse = new TextStory.CodeCallback(traverser.TestFalse);
            TextStory.CodeCallback cbTrue = new TextStory.CodeCallback(traverser.TestTrue);

            story.SetCursorPosition(cursor, 5);

            Assert.IsTrue(-1 == story.TextTable.TraverseText(cursor.CursorId, 5, cbFalse));
            Assert.IsTrue(0 == story.TextTable.TraverseText(cursor.CursorId, 5, cbTrue));
            Assert.IsTrue(-1 == story.TextTable.TraverseText(cursor.CursorId, -5, cbFalse));
            Assert.IsTrue(0 == story.TextTable.TraverseText(cursor.CursorId, -5, cbTrue));
        }

        private class Traverser
        {
            public bool TestTrue(ulong code)
            {
                return true;
            }

            public bool TestFalse(ulong code)
            {
                return false;
            }
        }

        [Test]
        public static void TestGetTextDistance()
        {
            TextStory story = new TextStory();
            TempCursor cursor = new TempCursor();

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

            story.ConvertToStyledText("Abc", style, null, out text);
            story.InsertText(cursor, text);

            GeneratorProperty g1A = new GeneratorProperty("G1", 0, 1);
            GeneratorProperty g1B = new GeneratorProperty("G1", 0, 2);

            properties.Clear();
            properties.Add(g1A);

            story.ConvertToStyledText("Def", style, properties, out text);
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(g1B);

            story.ConvertToStyledText("Ghi", style, properties, out text);
            story.InsertText(cursor, text);

            story.ConvertToStyledText("Klm", style, null, out text);
            story.InsertText(cursor, text);

            story.SetCursorPosition(cursor, 0);

            Assert.IsTrue('A' == (char)Unicode.Bits.GetCode(story.ReadChar(cursor, 0)));
            Assert.IsTrue('b' == (char)Unicode.Bits.GetCode(story.ReadChar(cursor, 1)));
            Assert.IsTrue('c' == (char)Unicode.Bits.GetCode(story.ReadChar(cursor, 2)));

            Assert.IsFalse(story.TextContext.ContainsProperty(story, cursor, 0, g1A));
            Assert.IsTrue(story.TextContext.ContainsProperty(story, cursor, 3, g1A));

            story.SetCursorPosition(cursor, 3);

            Assert.IsTrue('A' == (char)Unicode.Bits.GetCode(story.ReadChar(cursor, -3)));
            Assert.IsTrue('b' == (char)Unicode.Bits.GetCode(story.ReadChar(cursor, -2)));
            Assert.IsTrue('c' == (char)Unicode.Bits.GetCode(story.ReadChar(cursor, -1)));

            Assert.IsFalse(story.TextContext.ContainsProperty(story, cursor, -1, g1A));
            Assert.IsTrue(story.TextContext.ContainsProperty(story, cursor, 0, g1A));
            Assert.IsTrue(story.TextContext.ContainsProperty(story, cursor, 3, g1B));

            TextContext context = story.TextContext;

            Assert.IsTrue(0 == context.GetTextStartDistance(story, cursor, g1A));
            Assert.IsTrue(3 == context.GetTextEndDistance(story, cursor, g1A));
            Assert.IsTrue(-1 == context.GetTextEndDistance(story, cursor, g1B));

            story.SetCursorPosition(cursor, 4);

            Assert.IsTrue(1 == context.GetTextStartDistance(story, cursor, g1A));
            Assert.IsTrue(2 == context.GetTextEndDistance(story, cursor, g1A));

            story.SetCursorPosition(cursor, 5);

            Assert.IsTrue(2 == context.GetTextStartDistance(story, cursor, g1A));
            Assert.IsTrue(1 == context.GetTextEndDistance(story, cursor, g1A));

            story.SetCursorPosition(cursor, 6);

            Assert.IsTrue(-1 == context.GetTextStartDistance(story, cursor, g1A));
            Assert.IsTrue(-1 == context.GetTextEndDistance(story, cursor, g1A));
        }

        [Test]
        public static void Ex1()
        {
            Assert.Throws<System.InvalidOperationException>(() =>
            {
                FontSizeProperty a = new FontSizeProperty(
                    80.0 / 100,
                    SizeUnits.Percent
                );
                FontSizeProperty b = new FontSizeProperty(
                    -5.0,
                    SizeUnits.DeltaPoints
                );

                a.GetCombination(b);
            });
        }
    }
}
