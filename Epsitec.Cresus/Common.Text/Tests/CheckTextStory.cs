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
			CheckTextStory.TestUndoRedo ();
		}

		
		private static void TestUndoRedo()
		{
			TextStory story = new TextStory ();
			
			int     cursor = story.NewCursor ();
			ulong[] text_abc = { 65ul, 66ul, 67ul };
			ulong[] text_def = { 68ul, 69ul, 70ul };
			
			story.InsertText (cursor, text_abc);
			
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			System.Diagnostics.Debug.WriteLine ("Insert ABC.");
			System.Diagnostics.Debug.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Debug.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.InsertText (cursor, text_def);
			
			Debug.Assert.IsTrue (story.TextLength == 6);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 6);
			
			System.Diagnostics.Debug.WriteLine ("Insert DEF.");
			System.Diagnostics.Debug.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Debug.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			int cursor_x = story.NewCursor ();
			int cursor_y = story.NewCursor ();
			int cursor_z = story.NewCursor ();
			
			story.MoveCursor (cursor_x, 2);
			story.MoveCursor (cursor_y, 3);
			story.MoveCursor (cursor_z, 4);
			
			story.OpletQueue.UndoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 3);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			System.Diagnostics.Debug.WriteLine ("Undone DEF.");
			System.Diagnostics.Debug.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Debug.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 3);
			
			story.OpletQueue.UndoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 0);
			Debug.Assert.IsTrue (story.UndoLength == 6);
			Debug.Assert.IsFalse (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 0);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 0);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 0);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 0);
			
			System.Diagnostics.Debug.WriteLine ("Undone ABC.");
			System.Diagnostics.Debug.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Debug.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.OpletQueue.RedoAction ();
			
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 3);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsTrue (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_x) == 2);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_y) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor_z) == 3);
			
			System.Diagnostics.Debug.WriteLine ("Redone ABC.");
			System.Diagnostics.Debug.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Debug.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.OpletQueue.PurgeRedo ();
			
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsTrue (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			System.Diagnostics.Debug.WriteLine ("Purged redo.");
			System.Diagnostics.Debug.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Debug.WriteLine ("  Undo: " + story.GetDebugUndo ());
			
			story.OpletQueue.PurgeUndo ();
			
			Debug.Assert.IsTrue (story.TextLength == 3);
			Debug.Assert.IsTrue (story.UndoLength == 0);
			Debug.Assert.IsFalse (story.OpletQueue.CanUndo);
			Debug.Assert.IsFalse (story.OpletQueue.CanRedo);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 3);
			
			System.Diagnostics.Debug.WriteLine ("Purged undo.");
			System.Diagnostics.Debug.WriteLine ("  Text: " + story.GetDebugText ());
			System.Diagnostics.Debug.WriteLine ("  Undo: " + story.GetDebugUndo ());
		}
	}
}
