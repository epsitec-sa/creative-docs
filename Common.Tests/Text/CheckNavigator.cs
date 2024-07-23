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
    /// La batterie de tests CheckNavigator vérifie le bon fonctionnement des
    /// navigateurs.
    /// </summary>
    [TestFixture]
    public sealed class CheckNavigator
    {

        [Test]
        public static void TestParagraph()
        {
            TextStory story = new TextStory();
            ICursor cursor = new SimpleCursor();

            story.NewCursor(cursor);

            ulong[] text;
            System.Collections.ArrayList properties = new System.Collections.ArrayList();

            properties.Add(new FontProperty("Verdana", "Regular"));
            properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
            properties.Add(
                new LeadingProperty(
                    14.0,
                    SizeUnits.Points,
                    AlignMode.None
                )
            );

            TextStyle style1 = story.StyleList.NewTextStyle(
                null,
                "Normal",
                TextStyleClass.Paragraph,
                properties
            );

            properties.Clear();
            properties.Add(new FontProperty("Arial", "Bold"));
            properties.Add(new FontSizeProperty(15.0, SizeUnits.Points));
            properties.Add(
                new LeadingProperty(
                    0,
                    SizeUnits.Points,
                    AlignMode.First
                )
            );
            properties.Add(new FontColorProperty("Blue"));

            TextStyle style2 = story.StyleList.NewTextStyle(
                null,
                "Titre",
                TextStyleClass.Paragraph,
                properties
            );

            properties.Clear();
            properties.Add(new UnderlineProperty());

            story.ConvertToStyledText("Abc ", style1, null, out text); //	4
            story.InsertText(cursor, text);

            story.ConvertToStyledText("underline", style1, properties, out text); //	13
            story.InsertText(cursor, text);

            story.ConvertToStyledText(" xyz...\n", style1, null, out text); //	21
            story.InsertText(cursor, text);

            story.ConvertToStyledText("Underline.\n", style1, properties, out text); //	32
            story.InsertText(cursor, text);

            properties.Clear();
            properties.Add(
                new LeadingProperty(
                    0.0,
                    SizeUnits.Points,
                    AlignMode.Undefined
                )
            );
            properties.Add(
                new MarginsProperty(10.0, 10.0, SizeUnits.Millimeters)
            );

            story.ConvertToStyledText("Leading.\n", style1, properties, out text); //	41
            story.InsertText(cursor, text);

            story.ConvertToStyledText("Titre.\n", style2, null, out text); //	48
            story.InsertText(cursor, text);

            Assert.IsTrue(story.TextLength == 48);

            story.SetCursorPosition(cursor, 0);
            Assert.IsTrue(Navigator.GetParagraphStartOffset(story, cursor) == 0);
            Assert.IsTrue(Navigator.GetParagraphEndLength(story, cursor) == 21);

            story.SetCursorPosition(cursor, 10);
            Assert.IsTrue(Navigator.GetParagraphStartOffset(story, cursor) == -10);
            Assert.IsTrue(Navigator.GetParagraphEndLength(story, cursor) == 11);

            story.SetCursorPosition(cursor, 30);
            Assert.IsTrue(Navigator.GetParagraphStartOffset(story, cursor) == -9);
            Assert.IsTrue(Navigator.GetParagraphEndLength(story, cursor) == 2);

            story.SetCursorPosition(cursor, 48);
            Assert.IsTrue(Navigator.GetParagraphStartOffset(story, cursor) == 0);
            Assert.IsTrue(Navigator.GetParagraphEndLength(story, cursor) == 0);

            text = new ulong[story.TextLength];
            story.SetCursorPosition(cursor, 0);
            story.ReadText(cursor, story.TextLength, text);

            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));

            Assert.IsTrue(Navigator.IsParagraphStart(story, cursor, 0));
            Assert.IsTrue(Navigator.IsParagraphStart(story, cursor, 21));
            Assert.IsTrue(Navigator.IsParagraphStart(story, cursor, 41));
            Assert.IsFalse(Navigator.IsParagraphStart(story, cursor, 3));

            Assert.IsFalse(Navigator.IsParagraphEnd(story, cursor, 0));
            Assert.IsFalse(Navigator.IsParagraphEnd(story, cursor, 4));
            Assert.IsTrue(Navigator.IsParagraphEnd(story, cursor, 20));

            TextStyle[] styles;
            Property[] props;

            Assert.IsTrue(
                Navigator.GetParagraphStyles(story, cursor, 0, out styles)
            );
            Assert.IsTrue(styles.Length == 1);
            Assert.IsTrue(styles[0].Name == "Normal");

            Assert.IsTrue(
                Navigator.GetParagraphStyles(story, cursor, 45, out styles)
            );
            Assert.IsTrue(styles.Length == 1);
            Assert.IsTrue(styles[0].Name == "Titre");

            Assert.IsTrue(
                Navigator.GetParagraphStyles(story, cursor, 0, out styles)
            );
            Assert.IsTrue(styles.Length == 1);
            Assert.IsTrue(styles[0].Name == "Normal");

            Assert.IsTrue(
                Navigator.GetParagraphProperties(story, cursor, 0, out props)
            );
            Assert.IsTrue(props.Length == 0);

            Assert.IsTrue(
                Navigator.GetParagraphProperties(story, cursor, 40, out props)
            );
            Assert.IsTrue(props.Length == 2);
            Assert.IsTrue(props[0].WellKnownType == WellKnownType.Leading);
            Assert.IsTrue(props[1].WellKnownType == WellKnownType.Margins);

            Assert.IsTrue(
                Navigator.GetParagraphProperties(story, cursor, 5, out props)
            );
            Assert.IsTrue(props.Length == 0);

            Assert.IsTrue(Navigator.GetParagraphStartOffset(story, cursor) == 0);

            story.SetCursorPosition(cursor, 10);
            Assert.IsTrue(Navigator.GetParagraphStartOffset(story, cursor) == -10);

            story.SetCursorPosition(cursor, 21);
            Assert.IsTrue(Navigator.GetParagraphStartOffset(story, cursor) == 0);

            story.SetCursorPosition(cursor, 40);
            Assert.IsTrue(Navigator.GetParagraphStartOffset(story, cursor) == -8);

            story.SetCursorPosition(cursor, 26);
            Navigator.StartParagraphIfNeeded(story, cursor);

            Assert.IsTrue(story.TextLength == 49);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 27);

            text = new ulong[story.TextLength];
            story.SetCursorPosition(cursor, 0);
            story.ReadText(cursor, story.TextLength, text);

            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));

            story.SetCursorPosition(cursor, 27);
            Navigator.StartParagraphIfNeeded(story, cursor);

            Assert.IsTrue(story.TextLength == 49);

            text = new ulong[story.TextLength];
            story.SetCursorPosition(cursor, 0);
            story.ReadText(cursor, story.TextLength, text);

            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));

            properties.Clear();
            properties.Add(
                new LeadingProperty(
                    0.0,
                    SizeUnits.Points,
                    AlignMode.Undefined
                )
            );
            properties.Add(
                new MarginsProperty(10.0, 10.0, SizeUnits.Millimeters)
            );

            ICursor temp = new TempCursor();

            story.NewCursor(temp);
            story.SetCursorPosition(temp, 2);
            Navigator.SetParagraphStyles(story, temp, style2);
            Navigator.SetParagraphStyles(story, temp, style1, style2);
            Navigator.SetParagraphStyles(story, temp, style2);

            text = new ulong[story.TextLength];
            story.SetCursorPosition(cursor, 0);
            story.ReadText(cursor, story.TextLength, text);

            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));
        }
    }
}
