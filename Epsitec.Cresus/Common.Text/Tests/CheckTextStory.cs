//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			double font_size;
			
			story.TextContext.GetFont (text[0], out font, out font_size);
			
			Debug.Assert.IsTrue (font.FontIdentity.InvariantFaceName == "Arial");
			Debug.Assert.IsTrue (font.FontIdentity.InvariantStyleName == "Regular");
			Debug.Assert.IsTrue (font_size == 12.0);
			
			System.Diagnostics.Trace.WriteLine ("Timing Context.GetFont :");
			for (int i = 0; i < 1000000; i++)
			{
				story.TextContext.GetFont (text[1], out font, out font_size);
			}
			System.Diagnostics.Trace.WriteLine ("Done");
			
			Debug.Assert.IsTrue (font.FontIdentity.InvariantFaceName == "Arial");
			Debug.Assert.IsTrue (font.FontIdentity.InvariantStyleName == "Regular");
			Debug.Assert.IsTrue (font_size == 12.0);
		}
		
		private static void TestInsertUndoRedo()
		{
			TextStory story = new TextStory ();
			
			ICursor cursor   = new Cursors.SimpleCursor ();
			ICursor cursor_x = new Cursors.SimpleCursor ();
			ICursor cursor_y = new Cursors.SimpleCursor ();
			ICursor cursor_z = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text_abc = { 65ul, 66ul, 67ul };
			ulong[] text_def = { 68ul, 69ul, 70ul };
			
			Debug.Assert.IsTrue (cursor.Direction == 0);
			
			story.DebugDisableOpletMerge = true;
			
			story.OpletQueue.PurgeUndo ();
			story.InsertText (cursor, text_abc);
			
			Debug.Assert.IsTrue (cursor.Direction == 1);
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			System.Diagnostics.Trace.WriteLine ("Insert ABC.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.InsertText (cursor, text_def);
			
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
			
			story.NewCursor (cursor_x);
			story.NewCursor (cursor_y);
			story.NewCursor (cursor_z);
			
			story.SetCursorPosition (cursor_x, 2, 0);
			story.SetCursorPosition (cursor_y, 3, -1);
			story.SetCursorPosition (cursor_z, 4, 1);
			
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
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 3);
			Debug.Assert.IsTrue (cursor_x.Direction == 0);
			Debug.Assert.IsTrue (cursor_y.Direction == -1);
			Debug.Assert.IsTrue (cursor_z.Direction == 1);
						
			story.OpletQueue.UndoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 0);
			Debug.Assert.IsTrue (story.UndoLength == 6);
			Debug.Assert.IsFalse (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 0);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == 0);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 0);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 0);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 0);
			Debug.Assert.IsTrue (cursor_x.Direction == 0);
			Debug.Assert.IsTrue (cursor_y.Direction == -1);
			Debug.Assert.IsTrue (cursor_z.Direction == 1);
			
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
						
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 3);
			Debug.Assert.IsTrue (cursor_x.Direction == 0);
			Debug.Assert.IsTrue (cursor_y.Direction == -1);
			Debug.Assert.IsTrue (cursor_z.Direction == 1);
			
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
			ICursor cursor_x = new Cursors.SimpleCursor ();
			ICursor cursor_y = new Cursors.SimpleCursor ();
			ICursor cursor_z = new Cursors.SimpleCursor ();
			
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
			
			story.NewCursor (cursor_x);
			story.NewCursor (cursor_y);
			story.NewCursor (cursor_z);
			
			story.MoveCursor (cursor_x, 2);
			story.MoveCursor (cursor_y, 3);
			story.MoveCursor (cursor_z, 4);
			
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
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 3);
			
			story.OpletQueue.UndoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			Debug.Assert.IsTrue (story.GetCursorDirection (cursor) == -1);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 4);
			
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
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 4);
			
			System.Diagnostics.Trace.WriteLine ("Undone move cursor.");
			
			story.OpletQueue.RedoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 4);
			
			System.Diagnostics.Trace.WriteLine ("Redone move cursor.");
			
			story.OpletQueue.RedoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 4);
			Debug.Assert.IsTrue (story.UndoLength == 2);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 3);
			
			System.Diagnostics.Trace.WriteLine ("Redone delete DE.");
			System.Diagnostics.Trace.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Trace.WriteLine ("  Undo: " + story.GetDebugUndo ());
		}
		
		private static void TestWriteUndoRedo()
		{
			TextStory story = new TextStory ();
			
			ICursor cursor   = new Cursors.SimpleCursor ();
			ICursor cursor_x = new Cursors.SimpleCursor ();
			ICursor cursor_y = new Cursors.SimpleCursor ();
			ICursor cursor_z = new Cursors.SimpleCursor ();
			
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
			
			string text_1 = "Hello you world !\n";
			string text_2 = "wonderful ";
			
			ulong[] utf32_text_1;
			ulong[] utf32_text_2;
			
			TextConverter.ConvertFromString (text_1, out utf32_text_1);
			TextConverter.ConvertFromString (text_2, out utf32_text_2);
			
			story.OpletQueue.PurgeUndo ();
			story.InsertText (cursor, utf32_text_1);
			
			ulong[] buffer = new ulong[text_1.Length];
			
			story.MoveCursor (cursor, -text_1.Length);
			story.ReadText (cursor, text_1.Length, buffer);
			
			for (int i = 0; i < buffer.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0:00}: {1}, {2}", i, buffer[i].ToString ("X8"), Unicode.Bits.GetBreakInfo (buffer[i])));
			}
			
			story.ChangeMarkers (cursor, text_1.Length, story.TextContext.Markers.RequiresSpellChecking, false);
			story.ReadText (cursor, text_1.Length, buffer);
			
			for (int i = 0; i < buffer.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0:00}: {1}, {2}", i, buffer[i].ToString ("X8"), Unicode.Bits.GetBreakInfo (buffer[i])));
			}
			
			story.MoveCursor (cursor, 9);
			story.InsertText (cursor, utf32_text_2);
			story.MoveCursor (cursor, -9-utf32_text_2.Length);
			
			buffer = new ulong[story.TextLength];
			story.ReadText (cursor, story.TextLength, buffer);
			
			for (int i = 0; i < buffer.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0:00}: {1}, {2}", i, buffer[i].ToString ("X8"), Unicode.Bits.GetBreakInfo (buffer[i])));
			}
			
			story.ChangeMarkers (cursor, text_1.Length, story.TextContext.Markers.RequiresSpellChecking, false);
			story.ReadText (cursor, text_1.Length, buffer);
			
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
