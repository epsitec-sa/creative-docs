//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Vérifie le bon fonctionnement de la classe TextStory.
	/// </summary>
	public sealed class CheckTextStory
	{
		public static void RunTests()
		{
			CheckTextStory.TestStyles ();
			CheckTextStory.TestInsertUndoRedo ();
			CheckTextStory.TestDeleteUndoRedo ();
			CheckTextStory.TestWriteUndoRedo ();
			CheckTextStory.TestBreaks ();
		}

		
		private static void TestStyles()
		{
			TextStory story = new TextStory ();
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText ("Affiche", properties, out text);
			
			Debug.Assert.IsTrue (text.Length == 7);
			
			OpenType.Font font;
			double fontSize;
			double fontScale;
			
			story.TextContext.GetFontAndSize (text[0], out font, out fontSize, out fontScale);
			
			Debug.Assert.IsTrue (font.FontIdentity.InvariantFaceName == "Arial");
			Debug.Assert.IsTrue (font.FontIdentity.InvariantStyleName == "Regular");
			Debug.Assert.IsTrue (fontSize == 12.0);
			
			System.Diagnostics.Trace.WriteLine ("Timing TextContext.GetFontAndSize :");
			for (int i = 0; i < 1000000; i++)
			{
				story.TextContext.GetFontAndSize (text[1], out font, out fontSize, out fontScale);
			}
			System.Diagnostics.Trace.WriteLine ("Done");
			
			Debug.Assert.IsTrue (font.FontIdentity.InvariantFaceName == "Arial");
			Debug.Assert.IsTrue (font.FontIdentity.InvariantStyleName == "Regular");
			Debug.Assert.IsTrue (fontSize == 12.0);
		}
		
		private static void TestInsertUndoRedo()
		{
			TextStory story = new TextStory ();
			
			ICursor cursor   = new Cursors.SimpleCursor ();
			ICursor cursorX = new Cursors.SimpleCursor ();
			ICursor cursorY = new Cursors.SimpleCursor ();
			ICursor cursorZ = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] textAbc = { 65ul, 66ul, 67ul };
			ulong[] textDef = { 68ul, 69ul, 70ul };
			
			Debug.Assert.IsTrue (cursor.Direction == 0);
			
			story.DebugDisableOpletMerge = true;
			
			story.OpletQueue.PurgeUndo ();
			story.InsertText (cursor, textAbc);
			
			Debug.Assert.IsTrue (cursor.Direction == 1);
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			System.Diagnostics.Trace.WriteLine ("Insert ABC.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.InsertText (cursor, textDef);
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 6);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == 1);
			
			System.Diagnostics.Trace.WriteLine ("Insert DEF.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.DebugDisableOpletQueue = true;
			
			story.NewCursor (cursorX);
			story.NewCursor (cursorY);
			story.NewCursor (cursorZ);
			
			story.SetCursorPosition (cursorX, 2, 0);
			story.SetCursorPosition (cursorY, 3, -1);
			story.SetCursorPosition (cursorZ, 4, 1);
			
			story.DebugDisableOpletQueue = false;
			
			story.OpletQueue.UndoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 3);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == 1);
			
			System.Diagnostics.Trace.WriteLine ("Undone DEF.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorX) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorY) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorZ) == 3);
			Debug.Assert.IsTrue (cursorX.Direction == 0);
			Debug.Assert.IsTrue (cursorY.Direction == -1);
			Debug.Assert.IsTrue (cursorZ.Direction == 1);
						
			story.OpletQueue.UndoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 0);
			Debug.Assert.IsTrue (story.UndoLength == 6);
			Debug.Assert.IsFalse (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 0);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == 0);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorX) == 0);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorY) == 0);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorZ) == 0);
			Debug.Assert.IsTrue (cursorX.Direction == 0);
			Debug.Assert.IsTrue (cursorY.Direction == -1);
			Debug.Assert.IsTrue (cursorZ.Direction == 1);
			
			System.Diagnostics.Trace.WriteLine ("Undone ABC.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.OpletQueue.RedoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 3);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == 1);
						
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorX) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorY) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorZ) == 3);
			Debug.Assert.IsTrue (cursorX.Direction == 0);
			Debug.Assert.IsTrue (cursorY.Direction == -1);
			Debug.Assert.IsTrue (cursorZ.Direction == 1);
			
			System.Diagnostics.Trace.WriteLine ("Redone ABC.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.OpletQueue.PurgeRedo ();
			
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			System.Diagnostics.Trace.WriteLine ("Purged redo.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.OpletQueue.PurgeUndo ();
			
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsFalse (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			System.Diagnostics.Trace.WriteLine ("Purged undo.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
		}
		
		private static void TestDeleteUndoRedo()
		{
			TextStory story = new TextStory ();
			
			ICursor cursor   = new Cursors.SimpleCursor ();
			ICursor cursorX = new Cursors.SimpleCursor ();
			ICursor cursorY = new Cursors.SimpleCursor ();
			ICursor cursorZ = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text = { 65ul, 66ul, 67ul, 68ul, 69ul, 70ul };
			
			story.InsertText (cursor, text);
			story.OpletQueue.PurgeUndo ();
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsFalse (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 6);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == 1);
			
			System.Diagnostics.Trace.WriteLine ("Starting with ABCDEF.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.DebugDisableOpletQueue = true;
			
			story.NewCursor (cursorX);
			story.NewCursor (cursorY);
			story.NewCursor (cursorZ);
			
			story.MoveCursor (cursorX, 2);
			story.MoveCursor (cursorY, 3);
			story.MoveCursor (cursorZ, 4);
			
			story.DebugDisableOpletQueue = false;
			
			story.MoveCursor (cursor, -3);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == -1);
			
			story.DeleteText (cursor, 2);
			
			Debug.Assert.IsTrue (story.TextLength == 4);
			Debug.Assert.IsTrue (story.UndoLength == 2);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == 1);
			
			System.Diagnostics.Trace.WriteLine ("Deleted DE.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorX) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorY) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorZ) == 3);
			
			story.OpletQueue.UndoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == -1);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorX) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorY) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorZ) == 4);
			
			System.Diagnostics.Trace.WriteLine ("Undone delete DE.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.OpletQueue.UndoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsFalse (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 6);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == 1);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorX) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorY) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorZ) == 4);
			
			System.Diagnostics.Trace.WriteLine ("Undone move cursor.");
			
			story.OpletQueue.RedoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorX) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorY) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorZ) == 4);
			
			System.Diagnostics.Trace.WriteLine ("Redone move cursor.");
			
			story.OpletQueue.RedoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 4);
			Debug.Assert.IsTrue (story.UndoLength == 2);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorX) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorY) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursorZ) == 3);
			
			System.Diagnostics.Trace.WriteLine ("Redone delete DE.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
		}
		
		private static void TestWriteUndoRedo()
		{
			TextStory story = new TextStory ();
			
			ICursor cursor   = new Cursors.SimpleCursor ();
			ICursor cursorX = new Cursors.SimpleCursor ();
			ICursor cursorY = new Cursors.SimpleCursor ();
			ICursor cursorZ = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text = { 65ul, 66ul, 67ul, 68ul, 69ul, 70ul };
			
			story.InsertText (cursor, text);
			story.OpletQueue.PurgeUndo ();
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsFalse (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 6);
			
			Debug.Assert.IsTrue (story.GetDebugText () == "ABCDEF");
			
			story.MoveCursor (cursor, -3);
			story.WriteText (cursor, new ulong[] { 48ul, 49ul });
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			Debug.Assert.IsTrue (story.GetDebugText () == "ABC01F");
			
			story.OpletQueue.UndoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			Debug.Assert.IsTrue (story.GetDebugText () == "ABCDEF");
			
			story.OpletQueue.RedoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			Debug.Assert.IsTrue (story.GetDebugText () == "ABC01F");
			
			story.OpletQueue.PurgeUndo ();
		}
		
		private static void TestBreaks()
		{
			TextStory story = new TextStory ();
			
			ICursor cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			string text1 = "Hello you world !\n";
			string text2 = "wonderful ";
			
			ulong[] utf32Text1;
			ulong[] utf32Text2;
			
			TextConverter.ConvertFromString (text1, out utf32Text1);
			TextConverter.ConvertFromString (text2, out utf32Text2);
			
			story.OpletQueue.PurgeUndo ();
			story.InsertText (cursor, utf32Text1);
			
			ulong[] buffer = new ulong[text1.Length];
			
			story.MoveCursor (cursor, -text1.Length);
			story.ReadText (cursor, text1.Length, buffer);
			
			for (int i = 0; i < buffer.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0:00}: {1}, {2}", i, buffer[i].ToString ("X8"), Unicode.Bits.GetBreakInfo (buffer[i])));
			}
			
			story.ChangeMarkers (cursor, text1.Length, story.TextContext.Markers.RequiresSpellChecking, false);
			story.ReadText (cursor, text1.Length, buffer);
			
			for (int i = 0; i < buffer.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0:00}: {1}, {2}", i, buffer[i].ToString ("X8"), Unicode.Bits.GetBreakInfo (buffer[i])));
			}
			
			story.MoveCursor (cursor, 9);
			story.InsertText (cursor, utf32Text2);
			story.MoveCursor (cursor, -9-utf32Text2.Length);
			
			buffer = new ulong[story.TextLength];
			story.ReadText (cursor, story.TextLength, buffer);
			
			for (int i = 0; i < buffer.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0:00}: {1}, {2}", i, buffer[i].ToString ("X8"), Unicode.Bits.GetBreakInfo (buffer[i])));
			}
			
			story.ChangeMarkers (cursor, text1.Length, story.TextContext.Markers.RequiresSpellChecking, false);
			story.ReadText (cursor, text1.Length, buffer);
			
			for (int i = 0; i < buffer.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0:00}: {1}, {2}", i, buffer[i].ToString ("X8"), Unicode.Bits.GetBreakInfo (buffer[i])));
			}
			
			story.OpletQueue.UndoAction ();
			story.OpletQueue.UndoAction ();
			story.OpletQueue.UndoAction ();
			
			buffer = new ulong[story.TextLength];
			story.ReadText (cursor, story.TextLength, buffer);
			
			for (int i = 0; i < buffer.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0:00}: {1}, {2}", i, buffer[i].ToString ("X8"), Unicode.Bits.GetBreakInfo (buffer[i])));
			}
		}
	}
}
