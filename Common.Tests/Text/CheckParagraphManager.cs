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
using Epsitec.Common.Text.ParagraphManagers;
using Epsitec.Common.Text.Properties;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// La batterie de tests CheckParagraphManager vérifie le bon fonctionnement
    /// des gestionnaires de paragraphes (puces, etc.)
    /// </summary>
    [TestFixture]
    public sealed class CheckParagraphManager
    {
        [Test]
        public static void TestNavigation()
        {
            TextStory story = new TextStory();
            TextNavigator navigator = new TextNavigator(story);

            System.Collections.ArrayList properties = new System.Collections.ArrayList();

            properties.Clear();
            properties.Add(new FontProperty("Verdana", "Regular"));
            properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
            properties.Add(
                new LeadingProperty(
                    14.0,
                    SizeUnits.Points,
                    AlignMode.None
                )
            );
            properties.Add(
                new MarginsProperty(
                    5.0,
                    5.0,
                    5.0,
                    5.0,
                    SizeUnits.Points,
                    1.0,
                    0.0,
                    0.0,
                    15,
                    5,
                    ThreeState.True
                )
            );

            story.TextContext.DefaultParagraphStyle = story.StyleList.NewTextStyle(
                null,
                "Normal",
                TextStyleClass.Paragraph,
                properties
            );

            navigator.Insert("Abcdef");
            navigator.MoveTo(3, 1);

            ulong[] text1;
            ulong[] text2;

            AutoTextProperty at;
            AutoTextProperty at1 = new AutoTextProperty(
                "NX"
            );
            AutoTextProperty at2 = new AutoTextProperty(
                "NN"
            );
            AutoTextProperty at3 = new AutoTextProperty(
                "NN"
            );

            //	Deux propriétés AutoText identiques ne le sont jamais (à cause de
            //	leur identificateur unique) :

            Assert.IsFalse(Property.CompareEqualContents(at2, at3));

            story.ConvertToStyledText(
                "X",
                story.TextContext.DefaultParagraphStyle,
                new Property[] { at1 },
                out text1
            );
            story.ConvertToStyledText(
                "12",
                story.TextContext.DefaultParagraphStyle,
                new Property[] { at2 },
                out text2
            );

            story.InsertText(navigator.ActiveCursor, text1);
            story.InsertText(navigator.ActiveCursor, text2);

            //	Le texte est maintenant "Abc" + "X" + "12" + "def" avec les
            //	fragments "X" -> at1 et "12" -> at2.

            Assert.AreEqual("AbcX12def", story.GetDebugText());
            Assert.AreEqual(6, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.CharacterNext, 1);
            Assert.AreEqual(7, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 1);
            Assert.AreEqual(6, navigator.CursorPosition);

            //	On recule d'un caractère, mais on en saute 2 à cause de 'at2'.

            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 1);
            Assert.AreEqual(4, navigator.CursorPosition);
            Assert.IsTrue(
                story.TextContext.GetAutoText(story.ReadChar(navigator.ActiveCursor), out at)
            );
            Assert.IsTrue(Property.CompareEqualContents(at, at2));

            //	On recule d'un caractère et on en saute effectivement 1, même
            //	si 'at1' décore "X" (vérifie que le code de navigation est OK).

            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 1);
            Assert.AreEqual(3, navigator.CursorPosition);
            Assert.IsTrue(
                story.TextContext.GetAutoText(story.ReadChar(navigator.ActiveCursor), out at)
            );
            Assert.IsTrue(Property.CompareEqualContents(at, at1));

            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 1);
            Assert.AreEqual(2, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.CharacterNext, 2);
            Assert.AreEqual(4, navigator.CursorPosition);

            //	On se trouve à cheval entre "X" et "12". Les propriétés visibles
            //	par le navigateur ne reflètent jamais AutoText, car une insertion
            //	à ce point insère du texte normal !

            foreach (Property property in navigator.AccumulatedTextProperties)
            {
                Assert.IsFalse(property.WellKnownType == WellKnownType.AutoText);
            }
        }

        [Test]
        public static void TestAttachDetach()
        {
            TextStory story = new TextStory();
            ICursor cursor = new SimpleCursor();

            story.NewCursor(cursor);

            ulong[] text;
            System.Collections.ArrayList properties = new System.Collections.ArrayList();

            properties.Clear();
            properties.Add(new FontProperty("Verdana", "Regular"));
            properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
            properties.Add(
                new LeadingProperty(
                    14.0,
                    SizeUnits.Points,
                    AlignMode.None
                )
            );
            properties.Add(
                new MarginsProperty(
                    5.0,
                    5.0,
                    5.0,
                    5.0,
                    SizeUnits.Points,
                    1.0,
                    0.0,
                    0.0,
                    15,
                    5,
                    ThreeState.True
                )
            );

            TextStyle style1 = story.StyleList.NewTextStyle(
                null,
                "Normal",
                TextStyleClass.Paragraph,
                properties
            );

            Generator generator = story.TextContext.GeneratorList.NewGenerator("liste");

            generator.Add(Generator.CreateSequence(Generator.SequenceType.Alphabetic, "", ")"));

            ItemListManager.Parameters items =
                new ItemListManager.Parameters();

            TabList tabs = story.TextContext.TabList;

            items.Generator = generator;
            items.TabItem = tabs.NewTab(
                "T.item",
                10.0,
                SizeUnits.Points,
                0.0,
                null,
                TabPositionMode.Absolute
            );
            items.TabBody = tabs.NewTab(
                "T.body",
                40.0,
                SizeUnits.Points,
                0.0,
                null,
                TabPositionMode.Absolute
            );

            properties.Clear();
            properties.Add(new FontProperty("Verdana", "Regular"));
            properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
            properties.Add(
                new LeadingProperty(
                    14.0,
                    SizeUnits.Points,
                    AlignMode.None
                )
            );
            properties.Add(new ManagedParagraphProperty("ItemList", items.Save()));
            properties.Add(
                new MarginsProperty(
                    0,
                    40.0,
                    double.NaN,
                    double.NaN,
                    SizeUnits.Points,
                    1.0,
                    0.0,
                    0.0,
                    15,
                    5,
                    ThreeState.Undefined
                )
            );

            TextStyle style2 = story.StyleList.NewTextStyle(
                null,
                "Puces",
                TextStyleClass.Paragraph,
                properties
            );

            story.ConvertToStyledText("Xyz\n", style1, null, out text);
            story.InsertText(cursor, text);

            Assert.IsTrue(story.TextLength == 4);

            text = new ulong[story.TextLength];
            story.SetCursorPosition(cursor, 0);
            story.ReadText(cursor, story.TextLength, text);

            System.Diagnostics.Debug.WriteLine("Before SetParagraphStylesAndProperties (style2) :");
            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));

            //	Crée la liste à puces :

            ICursor temp = new TempCursor();

            story.NewCursor(temp);
            story.SetCursorPosition(temp, 2);
            Navigator.SetParagraphStyles(story, temp, style2);

            ManagedParagraphProperty[] mpp;

            Assert.IsTrue(story.TextLength == 1 + 2 + 1 + 4);
            Assert.IsTrue(
                Navigator.GetManagedParagraphProperties(story, temp, 0, out mpp)
            );
            Assert.IsTrue(mpp.Length == 1);
            Assert.IsTrue(mpp[0].ManagerName == "ItemList");

            text = new ulong[story.TextLength];
            story.SetCursorPosition(temp, 0);
            story.ReadText(temp, story.TextLength, text);

            System.Diagnostics.Debug.WriteLine("After SetParagraphStylesAndProperties (style2) :");
            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));

            //	Supprime la liste à puces :

            story.SetCursorPosition(temp, 1);
            Navigator.SetParagraphStyles(story, temp, style1);

            Assert.IsTrue(
                Navigator.GetManagedParagraphProperties(story, temp, 0, out mpp)
            );
            Assert.IsTrue(mpp.Length == 0);

            text = new ulong[story.TextLength];
            story.SetCursorPosition(temp, 0);
            story.ReadText(temp, story.TextLength, text);

            System.Diagnostics.Debug.WriteLine("After SetParagraphStylesAndProperties (style1) :");
            System.Diagnostics.Debug.WriteLine(story.GetDebugStyledText(text));

            Assert.IsTrue(story.TextLength == 4);
        }
    }
}
