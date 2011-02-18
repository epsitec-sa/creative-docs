//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Summary description for CheckInternalCursorTable.
	/// </summary>
	public sealed class CheckInternalCursorTable
	{
		public static void RunTests()
		{
			Internal.CursorTable table = new Internal.CursorTable ();
			
			Internal.CursorId id1 = table.NewCursor ();
			
			Debug.Assert.IsTrue (id1 == 1);
			Debug.Assert.IsTrue (table.ReadCursor (id1) == Internal.Cursor.Empty);
			Debug.Assert.IsTrue (table.ReadCursor (id1).CursorState == Internal.CursorState.Copied);
			
			Debug.Expect.Exception (new Debug.Method (Ex1), typeof (Debug.AssertFailedException));
			Debug.Expect.Exception (new Debug.Method (Ex2), typeof (Debug.AssertFailedException));
			Debug.Expect.Exception (new Debug.Method (Ex3), typeof (Debug.AssertFailedException));
			
			table.RecycleCursor (id1);
			
			Internal.CursorId id2 = table.NewCursor ();
			
			Debug.Assert.IsTrue (id1 == 1);
			Debug.Assert.IsTrue (id2 == 1);
			
			Debug.Expect.Exception (new Debug.Method (Ex4), typeof (Debug.AssertFailedException));
			Debug.Expect.Exception (new Debug.Method (Ex5), typeof (Debug.AssertFailedException));
			
			Internal.CursorId id3 = table.NewCursor ();
			Internal.CursorId id4 = table.NewCursor ();
			Internal.CursorId id5 = table.NewCursor ();
			
			table.RecycleCursor (id3);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (Internal.CursorId id in table)
			{
				list.Add (id);
			}
			
			Debug.Assert.IsTrue (list.Count == 3);
			Debug.Assert.IsTrue ((Internal.CursorId)list[0] == id2);
			Debug.Assert.IsTrue ((Internal.CursorId)list[1] == id4);
			Debug.Assert.IsTrue ((Internal.CursorId)list[2] == id5);
		}
		
		
		private static void Ex1()
		{
			Internal.CursorTable table = new Internal.CursorTable ();
			table.ReadCursor (0);
		}
		
		private static void Ex2()
		{
			Internal.CursorTable table = new Internal.CursorTable ();
			table.ReadCursor (1);
		}
		
		private static void Ex3()
		{
			Internal.CursorTable table = new Internal.CursorTable ();
			table.ReadCursor (2);
		}
		
		private static void Ex4()
		{
			Internal.CursorTable table = new Internal.CursorTable ();
			
			Internal.CursorId id = table.NewCursor ();
			
			table.RecycleCursor (id);
			table.RecycleCursor (id);
		}
		
		private static void Ex5()
		{
			Internal.CursorTable table = new Internal.CursorTable ();
			
			Internal.CursorId id = table.NewCursor ();
			
			table.RecycleCursor (id);
			table.ReadCursor (id);
		}
	}
}
