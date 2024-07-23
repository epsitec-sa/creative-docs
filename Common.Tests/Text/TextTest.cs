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
    [TestFixture]
    public class TextTest
    {

        [Test]
        public void CheckTextNavigator()
        {
            TextStory story = new TextStory();
            TextFitter fitter = new TextFitter(story);
            SimpleTextFrame frame = new SimpleTextFrame(100, 1000);

            SimpleCursor cursor;
            string[] texts;

            fitter.FrameList.Add(frame);

            TextNavigator navigator = new TextNavigator(fitter);

            System.Collections.ArrayList properties = new System.Collections.ArrayList();

            properties.Add(new FontProperty("Verdana", "Regular"));
            properties.Add(new FontSizeProperty(14.0, SizeUnits.Points));

            TextStyle default_style = story.StyleList.NewTextStyle(
                null,
                "Default",
                TextStyleClass.Paragraph,
                properties
            );

            story.TextContext.DefaultParagraphStyle = default_style;

            Assert.AreEqual(0, navigator.TextLength);
            Assert.AreEqual(0, navigator.CursorPosition);

            navigator.Insert("abc\ndef");
            Assert.AreEqual(7, navigator.TextLength);
            Assert.AreEqual(7, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.TextStart, 0);
            Assert.AreEqual(0, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.ParagraphStart, 0);
            Assert.AreEqual(0, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.ParagraphEnd, 0);
            Assert.AreEqual(3, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.ParagraphEnd, 0);
            Assert.AreEqual(3, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 2);
            Assert.AreEqual(1, navigator.CursorPosition);
            navigator.MoveTo(TextNavigator.Target.ParagraphEnd, 1);
            Assert.AreEqual(3, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.ParagraphEnd, 1);
            Assert.AreEqual(7, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.ParagraphStart, 2);
            Assert.AreEqual(0, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.ParagraphEnd, 1);
            Assert.AreEqual(3, navigator.CursorPosition);
            navigator.MoveTo(TextNavigator.Target.CharacterNext, 1);
            Assert.AreEqual(4, navigator.CursorPosition);
            navigator.MoveTo(TextNavigator.Target.ParagraphStart, 0);
            Assert.AreEqual(4, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 1);
            Assert.AreEqual(3, navigator.CursorPosition);

            navigator.Insert(" xyz   qrs"); //	"abc xyz   qrs|\ndef"
            Assert.AreEqual(13, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.WordEnd, 0);
            Assert.AreEqual(13, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.WordStart, 1);
            Assert.AreEqual(10, navigator.CursorPosition); //	"abc xyz   |qrs\ndef"

            navigator.MoveTo(TextNavigator.Target.WordStart, 0);
            Assert.AreEqual(10, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.WordStart, 1);
            Assert.AreEqual(4, navigator.CursorPosition); //	"abc |xyz   qrs\ndef"

            navigator.MoveTo(TextNavigator.Target.WordEnd, 0);
            Assert.AreEqual(4, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 2);
            Assert.AreEqual(2, navigator.CursorPosition);
            navigator.MoveTo(TextNavigator.Target.WordEnd, 0);
            Assert.AreEqual(4, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 2);
            Assert.AreEqual(2, navigator.CursorPosition);
            navigator.MoveTo(TextNavigator.Target.WordEnd, 1);
            Assert.AreEqual(4, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.WordEnd, 1);
            Assert.AreEqual(10, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.WordEnd, 1);
            Assert.AreEqual(13, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.TextStart, 0);
            navigator.MoveTo(TextNavigator.Target.ParagraphEnd, 0);
            navigator.Insert(
                ". "
                    + "Just for fun, "
                    + "some more "
                    + "text in order           "
                    + "to be able to "
                    + "test the line "
                    + "navigation "
                    + "algorithm."
            );

            fitter.ClearAllMarks();
            fitter.GenerateAllMarks();

            int para_count;
            int line_count;

            fitter.GetStatistics(out para_count, out line_count);

            Assert.AreEqual(2, para_count);
            Assert.AreEqual(9, line_count);
            Assert.AreEqual(116, navigator.TextLength);

            //	Vérifie le bon fonctionnement de la détection des débuts et des
            //	fins de lignes, basée sur les informations du fitter :

            cursor = new SimpleCursor();
            story.NewCursor(cursor);

            Assert.IsTrue(Navigator.IsLineStart(story, fitter, cursor, 0, -1));
            Assert.IsTrue(Navigator.IsLineStart(story, fitter, cursor, 15, -1));
            Assert.IsTrue(Navigator.IsLineStart(story, fitter, cursor, 29, -1));
            Assert.IsTrue(Navigator.IsLineStart(story, fitter, cursor, 39, -1));
            Assert.IsTrue(Navigator.IsLineStart(story, fitter, cursor, 63, -1));
            Assert.IsTrue(Navigator.IsLineStart(story, fitter, cursor, 77, -1));
            Assert.IsTrue(Navigator.IsLineStart(story, fitter, cursor, 91, -1));
            Assert.IsTrue(Navigator.IsLineStart(story, fitter, cursor, 102, -1));
            Assert.IsTrue(Navigator.IsLineStart(story, fitter, cursor, 113, -1));
            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 116, -1));

            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 15, 1));
            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 29, 1));
            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 39, 1));
            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 63, 1));
            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 77, 1));
            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 91, 1));
            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 102, 1));
            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 113, 1));
            Assert.IsFalse(Navigator.IsLineStart(story, fitter, cursor, 116, 1));

            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 0, -1));
            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 15, -1));
            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 29, -1));
            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 39, -1));
            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 63, -1));
            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 77, -1));
            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 91, -1));
            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 102, -1));
            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 113, -1));

            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 0, 1));
            Assert.IsTrue(Navigator.IsLineEnd(story, fitter, cursor, 15, 1));
            Assert.IsTrue(Navigator.IsLineEnd(story, fitter, cursor, 29, 1));
            Assert.IsTrue(Navigator.IsLineEnd(story, fitter, cursor, 39, 1));
            Assert.IsTrue(Navigator.IsLineEnd(story, fitter, cursor, 63, 1));
            Assert.IsTrue(Navigator.IsLineEnd(story, fitter, cursor, 77, 1));
            Assert.IsTrue(Navigator.IsLineEnd(story, fitter, cursor, 91, 1));
            Assert.IsTrue(Navigator.IsLineEnd(story, fitter, cursor, 102, 1));
            Assert.IsTrue(Navigator.IsLineEnd(story, fitter, cursor, 112, 1));
            Assert.IsFalse(Navigator.IsLineEnd(story, fitter, cursor, 113, 1));
            Assert.IsTrue(Navigator.IsLineEnd(story, fitter, cursor, 116, 1));

            navigator.MoveTo(TextNavigator.Target.TextStart, 0);
            Assert.AreEqual(-1, navigator.CursorDirection);
            navigator.MoveTo(TextNavigator.Target.LineEnd, 0);
            navigator.MoveTo(TextNavigator.Target.LineEnd, 0);
            Assert.AreEqual(15, navigator.CursorPosition);
            Assert.AreEqual(1, navigator.CursorDirection);

            navigator.MoveTo(TextNavigator.Target.TextStart, 0);
            navigator.MoveTo(TextNavigator.Target.LineEnd, 1);
            Assert.AreEqual(15, navigator.CursorPosition);

            navigator.MoveTo(TextNavigator.Target.TextStart, 0);
            navigator.MoveTo(TextNavigator.Target.LineEnd, 5);
            Assert.AreEqual(77, navigator.CursorPosition);
            Assert.AreEqual(1, navigator.CursorDirection);

            navigator.MoveTo(TextNavigator.Target.CharacterNext, 1);
            Assert.AreEqual(78, navigator.CursorPosition);
            Assert.AreEqual(1, navigator.CursorDirection);
            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 1);
            Assert.AreEqual(77, navigator.CursorPosition);
            Assert.AreEqual(-1, navigator.CursorDirection);
            navigator.MoveTo(TextNavigator.Target.LineStart, 0);
            Assert.AreEqual(77, navigator.CursorPosition);
            Assert.AreEqual(-1, navigator.CursorDirection);

            navigator.MoveTo(TextNavigator.Target.LineStart, 2);
            Assert.AreEqual(39, navigator.CursorPosition);
            Assert.AreEqual(-1, navigator.CursorDirection);

            //	Vérifie le bon fonctionnement de la sélection du texte :

            navigator.MoveTo(TextNavigator.Target.TextStart, 0);
            navigator.MoveTo(TextNavigator.Target.WordEnd, 1);
            Assert.AreEqual(4, navigator.CursorPosition);

            navigator.StartSelection();
            navigator.MoveTo(TextNavigator.Target.WordEnd, 1);
            Assert.AreEqual(10, navigator.CursorPosition);
            navigator.EndSelection();
            Assert.AreEqual(4, navigator.CursorPosition);

            texts = navigator.GetSelectedTexts();
            Assert.AreEqual(1, texts.Length);
            Assert.AreEqual("xyz   ", texts[0]);

            navigator.ClearSelection();

            texts = navigator.GetSelectedTexts();
            Assert.AreEqual(0, texts.Length);

            navigator.OpletQueue.UndoAction(); //	Undo deselect

            texts = navigator.GetSelectedTexts();
            Assert.AreEqual(1, texts.Length);
            Assert.AreEqual("xyz   ", texts[0]);

            navigator.OpletQueue.RedoAction(); //	Redo deselect

            texts = navigator.GetSelectedTexts();
            Assert.AreEqual(0, texts.Length);

            //	Vérifie le bon fonctionnement de sélection disjointes en prenant
            //	deux mots distants dans le texte :

            navigator.StartSelection();
            navigator.MoveTo(TextNavigator.Target.WordEnd, 1);
            Assert.AreEqual(10, navigator.CursorPosition);
            navigator.EndSelection();

            Assert.AreEqual(4, navigator.CursorPosition);
            Assert.AreEqual(1, navigator.CursorDirection);

            navigator.MoveTo(TextNavigator.Target.ParagraphEnd, 2);

            Assert.AreEqual(116, navigator.CursorPosition);
            Assert.AreEqual(1, navigator.CursorDirection);

            navigator.MoveTo(TextNavigator.Target.ParagraphStart, 1);

            Assert.AreEqual(113, navigator.CursorPosition);
            Assert.AreEqual(-1, navigator.CursorDirection);

            navigator.StartDisjointSelection();
            navigator.MoveTo(TextNavigator.Target.WordEnd, 1);
            navigator.MoveTo(TextNavigator.Target.CharacterPrevious, 1);
            navigator.EndSelection();

            texts = navigator.GetSelectedTexts();
            Assert.AreEqual(2, texts.Length);
            Assert.AreEqual("xyz   ", texts[0]);
            Assert.AreEqual("de", texts[1]);

            //	Vérifie le bon fonctionnement de destructions de sélections
            //	disjointes :

            navigator.Delete();

            Assert.AreEqual(107, navigator.CursorPosition); //	supprimé 6 caractères avant 2ème sélection..
            Assert.AreEqual(0, navigator.CursorDirection); //	..ce qui explique le passage de 113 à 107.

            navigator.MoveTo(TextNavigator.Target.TextStart, 0);
            navigator.StartSelection();
            navigator.MoveTo(TextNavigator.Target.TextEnd, 0);
            navigator.EndSelection();

            texts = navigator.GetSelectedTexts();
            Assert.IsTrue(texts[0].EndsWith("algorithm." + "\u2029" + "f"));

            navigator.OpletQueue.UndoAction(); //	Select
            navigator.OpletQueue.UndoAction(); //	MoveTo
            navigator.OpletQueue.UndoAction(); //	Delete + deselect

            texts = navigator.GetSelectedTexts();
            Assert.AreEqual(2, texts.Length);
            Assert.AreEqual("xyz   ", texts[0]);
            Assert.AreEqual("de", texts[1]);

            Assert.AreEqual(113, navigator.CursorPosition);
            Assert.AreEqual(-1, navigator.CursorDirection);
            Assert.IsTrue(navigator.HasSelection);

            navigator.OpletQueue.UndoAction(); //	Select

            Assert.IsFalse(navigator.HasSelection);

            navigator.OpletQueue.UndoAction(); //	MoveTo(s)

            Assert.AreEqual(4, navigator.CursorPosition);
            Assert.AreEqual(1, navigator.CursorDirection);
        }

        [Test]
        public void CheckTabList()
        {
            TabList list = new TabList(null);

            TabProperty tp1 = list.NewTab(
                null,
                10.0,
                SizeUnits.Millimeters,
                0.0,
                null,
                TabPositionMode.Absolute,
                null
            );
            TabProperty tp2 = list.NewTab(
                null,
                20.0,
                SizeUnits.Millimeters,
                0.0,
                null,
                TabPositionMode.Absolute,
                null
            );
            TabProperty tp3 = list.NewTab(
                null,
                30.0,
                SizeUnits.Millimeters,
                0.0,
                null,
                TabPositionMode.Absolute,
                null
            );

            TabProperty tpx;

            tpx = list.FindAutoTab(
                10.0,
                SizeUnits.Millimeters,
                0.0,
                null,
                TabPositionMode.Absolute,
                null
            );

            Assert.IsNotNull(tpx);
            Assert.IsTrue(tpx.TabTag == tp1.TabTag);

            tpx = list.FindAutoTab(
                40.0,
                SizeUnits.Millimeters,
                0.0,
                null,
                TabPositionMode.Absolute,
                null
            );

            Assert.IsNull(tpx);

            tpx = list.FindAutoTab(
                20.0,
                SizeUnits.Millimeters,
                0.5,
                null,
                TabPositionMode.Absolute,
                null
            );

            Assert.IsNull(tpx);

            tpx = list.FindAutoTab(
                20.0,
                SizeUnits.Millimeters,
                0.0,
                ".",
                TabPositionMode.Absolute,
                null
            );

            Assert.IsNull(tpx);

            tpx = list.FindAutoTab(
                30.0,
                SizeUnits.Millimeters,
                0.0,
                null,
                TabPositionMode.Absolute,
                null
            );

            Assert.IsNotNull(tpx);
            Assert.IsTrue(tpx.TabTag == tp3.TabTag);
        }
    }
}
