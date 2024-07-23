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
    /// Vérifie le bon fonctionnement de la classe TextStory.
    /// </summary>
    [TestFixture]
    public sealed class CheckTextStory
    {

        [Test]
        public static void TestStyles()
        {
            TextStory story = new TextStory();

            ulong[] text;
            System.Collections.ArrayList properties = new System.Collections.ArrayList();

            properties.Add(new FontProperty("Arial", "Regular"));
            properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));

            story.ConvertToStyledText("Affiche", properties, out text);

            Assert.IsTrue(text.Length == 7);

            Common.OpenType.Font font;
            double fontSize;
            double fontScale;

            story.TextContext.GetFontAndSize(text[0], out font, out fontSize, out fontScale);

            Assert.IsTrue(font.FontIdentity.InvariantFaceName == "Arial");
            Assert.IsTrue(font.FontIdentity.InvariantStyleName == "Regular");
            Assert.IsTrue(fontSize == 12.0);

            System.Diagnostics.Trace.WriteLine("Timing TextContext.GetFontAndSize :");
            for (int i = 0; i < 1000000; i++)
            {
                story.TextContext.GetFontAndSize(text[1], out font, out fontSize, out fontScale);
            }
            System.Diagnostics.Trace.WriteLine("Done");

            Assert.IsTrue(font.FontIdentity.InvariantFaceName == "Arial");
            Assert.IsTrue(font.FontIdentity.InvariantStyleName == "Regular");
            Assert.IsTrue(fontSize == 12.0);
        }

        [Test]
        public static void TestInsertUndoRedo()
        {
            TextStory story = new TextStory();

            ICursor cursor = new SimpleCursor();
            ICursor cursorX = new SimpleCursor();
            ICursor cursorY = new SimpleCursor();
            ICursor cursorZ = new SimpleCursor();

            story.NewCursor(cursor);

            ulong[] textAbc = { 65ul, 66ul, 67ul };
            ulong[] textDef = { 68ul, 69ul, 70ul };

            Assert.IsTrue(cursor.Direction == 0);

            story.DebugDisableOpletMerge = true;

            story.OpletQueue.PurgeUndo();
            story.InsertText(cursor, textAbc);

            Assert.IsTrue(cursor.Direction == 1);
            Assert.IsTrue(story.TextLength == 3);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);

            System.Diagnostics.Trace.WriteLine("Insert ABC.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());

            story.InsertText(cursor, textDef);

            Assert.IsTrue(story.TextLength == 6);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 6);
            Assert.IsTrue(story.GetCursorDirection(cursor) == 1);

            System.Diagnostics.Trace.WriteLine("Insert DEF.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());

            story.DebugDisableOpletQueue = true;

            story.NewCursor(cursorX);
            story.NewCursor(cursorY);
            story.NewCursor(cursorZ);

            story.SetCursorPosition(cursorX, 2, 0);
            story.SetCursorPosition(cursorY, 3, -1);
            story.SetCursorPosition(cursorZ, 4, 1);

            story.DebugDisableOpletQueue = false;

            story.OpletQueue.UndoAction();

            Assert.IsTrue(story.TextLength == 3);
            Assert.IsTrue(story.UndoLength == 3);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsTrue(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);
            Assert.IsTrue(story.GetCursorDirection(cursor) == 1);

            System.Diagnostics.Trace.WriteLine("Undone DEF.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());

            Assert.IsTrue(story.GetCursorPosition(cursorX) == 2);
            Assert.IsTrue(story.GetCursorPosition(cursorY) == 3);
            Assert.IsTrue(story.GetCursorPosition(cursorZ) == 3);
            Assert.IsTrue(cursorX.Direction == 0);
            Assert.IsTrue(cursorY.Direction == -1);
            Assert.IsTrue(cursorZ.Direction == 1);

            story.OpletQueue.UndoAction();

            Assert.IsTrue(story.TextLength == 0);
            Assert.IsTrue(story.UndoLength == 6);
            Assert.IsFalse(story.OpletQueue.CanUndo);
            Assert.IsTrue(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 0);
            Assert.IsTrue(story.GetCursorDirection(cursor) == 0);

            Assert.IsTrue(story.GetCursorPosition(cursorX) == 0);
            Assert.IsTrue(story.GetCursorPosition(cursorY) == 0);
            Assert.IsTrue(story.GetCursorPosition(cursorZ) == 0);
            Assert.IsTrue(cursorX.Direction == 0);
            Assert.IsTrue(cursorY.Direction == -1);
            Assert.IsTrue(cursorZ.Direction == 1);

            System.Diagnostics.Trace.WriteLine("Undone ABC.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());

            story.OpletQueue.RedoAction();

            Assert.IsTrue(story.TextLength == 3);
            Assert.IsTrue(story.UndoLength == 3);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsTrue(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);
            Assert.IsTrue(story.GetCursorDirection(cursor) == 1);

            Assert.IsTrue(story.GetCursorPosition(cursorX) == 2);
            Assert.IsTrue(story.GetCursorPosition(cursorY) == 3);
            Assert.IsTrue(story.GetCursorPosition(cursorZ) == 3);
            Assert.IsTrue(cursorX.Direction == 0);
            Assert.IsTrue(cursorY.Direction == -1);
            Assert.IsTrue(cursorZ.Direction == 1);

            System.Diagnostics.Trace.WriteLine("Redone ABC.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());

            story.OpletQueue.PurgeRedo();

            Assert.IsTrue(story.TextLength == 3);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);

            System.Diagnostics.Trace.WriteLine("Purged redo.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());

            story.OpletQueue.PurgeUndo();

            Assert.IsTrue(story.TextLength == 3);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsFalse(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);

            System.Diagnostics.Trace.WriteLine("Purged undo.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());
        }

        [Test]
        public static void TestDeleteUndoRedo()
        {
            TextStory story = new TextStory();

            ICursor cursor = new SimpleCursor();
            ICursor cursorX = new SimpleCursor();
            ICursor cursorY = new SimpleCursor();
            ICursor cursorZ = new SimpleCursor();

            story.NewCursor(cursor);

            ulong[] text = { 65ul, 66ul, 67ul, 68ul, 69ul, 70ul };

            story.InsertText(cursor, text);
            story.OpletQueue.PurgeUndo();

            Assert.IsTrue(story.TextLength == 6);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsFalse(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 6);
            Assert.IsTrue(story.GetCursorDirection(cursor) == 1);

            System.Diagnostics.Trace.WriteLine("Starting with ABCDEF.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());

            story.DebugDisableOpletQueue = true;

            story.NewCursor(cursorX);
            story.NewCursor(cursorY);
            story.NewCursor(cursorZ);

            story.MoveCursor(cursorX, 2);
            story.MoveCursor(cursorY, 3);
            story.MoveCursor(cursorZ, 4);

            story.DebugDisableOpletQueue = false;

            story.MoveCursor(cursor, -3);

            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);
            Assert.IsTrue(story.GetCursorDirection(cursor) == -1);

            story.DeleteText(cursor, 2);

            Assert.IsTrue(story.TextLength == 4);
            Assert.IsTrue(story.UndoLength == 2);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);
            Assert.IsTrue(story.GetCursorDirection(cursor) == 1);

            System.Diagnostics.Trace.WriteLine("Deleted DE.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());

            Assert.IsTrue(story.GetCursorPosition(cursorX) == 2);
            Assert.IsTrue(story.GetCursorPosition(cursorY) == 3);
            Assert.IsTrue(story.GetCursorPosition(cursorZ) == 3);

            story.OpletQueue.UndoAction();

            Assert.IsTrue(story.TextLength == 6);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsTrue(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);
            Assert.IsTrue(story.GetCursorDirection(cursor) == -1);

            Assert.IsTrue(story.GetCursorPosition(cursorX) == 2);
            Assert.IsTrue(story.GetCursorPosition(cursorY) == 3);
            Assert.IsTrue(story.GetCursorPosition(cursorZ) == 4);

            System.Diagnostics.Trace.WriteLine("Undone delete DE.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());

            story.OpletQueue.UndoAction();

            Assert.IsTrue(story.TextLength == 6);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsFalse(story.OpletQueue.CanUndo);
            Assert.IsTrue(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 6);
            Assert.IsTrue(story.GetCursorDirection(cursor) == 1);

            Assert.IsTrue(story.GetCursorPosition(cursorX) == 2);
            Assert.IsTrue(story.GetCursorPosition(cursorY) == 3);
            Assert.IsTrue(story.GetCursorPosition(cursorZ) == 4);

            System.Diagnostics.Trace.WriteLine("Undone move cursor.");

            story.OpletQueue.RedoAction();

            Assert.IsTrue(story.TextLength == 6);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsTrue(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);

            Assert.IsTrue(story.GetCursorPosition(cursorX) == 2);
            Assert.IsTrue(story.GetCursorPosition(cursorY) == 3);
            Assert.IsTrue(story.GetCursorPosition(cursorZ) == 4);

            System.Diagnostics.Trace.WriteLine("Redone move cursor.");

            story.OpletQueue.RedoAction();

            Assert.IsTrue(story.TextLength == 4);
            Assert.IsTrue(story.UndoLength == 2);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);

            Assert.IsTrue(story.GetCursorPosition(cursorX) == 2);
            Assert.IsTrue(story.GetCursorPosition(cursorY) == 3);
            Assert.IsTrue(story.GetCursorPosition(cursorZ) == 3);

            System.Diagnostics.Trace.WriteLine("Redone delete DE.");
            System.Diagnostics.Trace.WriteLine("  Text: " + story.GetDebugText());
            System.Diagnostics.Trace.WriteLine("  Undo: " + story.GetDebugUndo());
        }

        [Test]
        public static void TestWriteUndoRedo()
        {
            TextStory story = new TextStory();

            ICursor cursor = new SimpleCursor();
            ICursor cursorX = new SimpleCursor();
            ICursor cursorY = new SimpleCursor();
            ICursor cursorZ = new SimpleCursor();

            story.NewCursor(cursor);

            ulong[] text = { 65ul, 66ul, 67ul, 68ul, 69ul, 70ul };

            story.InsertText(cursor, text);
            story.OpletQueue.PurgeUndo();

            Assert.IsTrue(story.TextLength == 6);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsFalse(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 6);

            Assert.IsTrue(story.GetDebugText() == "ABCDEF");

            story.MoveCursor(cursor, -3);
            story.WriteText(cursor, new ulong[] { 48ul, 49ul });

            Assert.IsTrue(story.TextLength == 6);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);

            Assert.IsTrue(story.GetDebugText() == "ABC01F");

            story.OpletQueue.UndoAction();

            Assert.IsTrue(story.TextLength == 6);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsTrue(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);

            Assert.IsTrue(story.GetDebugText() == "ABCDEF");

            story.OpletQueue.RedoAction();

            Assert.IsTrue(story.TextLength == 6);
            Assert.IsTrue(story.UndoLength == 0);
            Assert.IsTrue(story.OpletQueue.CanUndo);
            Assert.IsFalse(story.OpletQueue.CanRedo);
            Assert.IsTrue(story.GetCursorPosition(cursor) == 3);

            Assert.IsTrue(story.GetDebugText() == "ABC01F");

            story.OpletQueue.PurgeUndo();
        }

        [Test]
        public static void TestBreaks()
        {
            TextStory story = new TextStory();

            ICursor cursor = new SimpleCursor();

            story.NewCursor(cursor);

            string text1 = "Hello you world !\n";
            string text2 = "wonderful ";

            ulong[] utf32Text1;
            ulong[] utf32Text2;

            TextConverter.ConvertFromString(text1, out utf32Text1);
            TextConverter.ConvertFromString(text2, out utf32Text2);

            story.OpletQueue.PurgeUndo();
            story.InsertText(cursor, utf32Text1);

            ulong[] buffer = new ulong[text1.Length];

            story.MoveCursor(cursor, -text1.Length);
            story.ReadText(cursor, text1.Length, buffer);

            for (int i = 0; i < buffer.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "{0:00}: {1}, {2}",
                        i,
                        buffer[i].ToString("X8"),
                        Unicode.Bits.GetBreakInfo(buffer[i])
                    )
                );
            }

            story.ChangeMarkers(
                cursor,
                text1.Length,
                story.TextContext.Markers.RequiresSpellChecking,
                false
            );
            story.ReadText(cursor, text1.Length, buffer);

            for (int i = 0; i < buffer.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "{0:00}: {1}, {2}",
                        i,
                        buffer[i].ToString("X8"),
                        Unicode.Bits.GetBreakInfo(buffer[i])
                    )
                );
            }

            story.MoveCursor(cursor, 9);
            story.InsertText(cursor, utf32Text2);
            story.MoveCursor(cursor, -9 - utf32Text2.Length);

            buffer = new ulong[story.TextLength];
            story.ReadText(cursor, story.TextLength, buffer);

            for (int i = 0; i < buffer.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "{0:00}: {1}, {2}",
                        i,
                        buffer[i].ToString("X8"),
                        Unicode.Bits.GetBreakInfo(buffer[i])
                    )
                );
            }

            story.ChangeMarkers(
                cursor,
                text1.Length,
                story.TextContext.Markers.RequiresSpellChecking,
                false
            );
            story.ReadText(cursor, text1.Length, buffer);

            for (int i = 0; i < buffer.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "{0:00}: {1}, {2}",
                        i,
                        buffer[i].ToString("X8"),
                        Unicode.Bits.GetBreakInfo(buffer[i])
                    )
                );
            }

            story.OpletQueue.UndoAction();
            story.OpletQueue.UndoAction();
            story.OpletQueue.UndoAction();

            buffer = new ulong[story.TextLength];
            story.ReadText(cursor, story.TextLength, buffer);

            for (int i = 0; i < buffer.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "{0:00}: {1}, {2}",
                        i,
                        buffer[i].ToString("X8"),
                        Unicode.Bits.GetBreakInfo(buffer[i])
                    )
                );
            }
        }
    }
}
