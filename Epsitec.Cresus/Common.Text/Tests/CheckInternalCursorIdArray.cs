//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Summary description for CheckInternalCursorIdArray.
	/// </summary>
	public sealed class CheckInternalCursorIdArray
	{
		public static void RunTests()
		{
			Internal.CursorIdArray array = new Internal.CursorIdArray ();
			
			Internal.CursorId id1 = 1;
			Internal.CursorId id2 = 2;
			Internal.CursorId id3 = 3;
			
			array.Add (id1, 0);
			array.Add (id2, 4);
			array.Add (id3, 1);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 0);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 4);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 1);
			
			array.Move (id3, 2);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 0);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 4);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 2);
			
			array.Move (id3, 1);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 0);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 4);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 1);
			
			array.Move (id3, 4);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 0);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 4);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 4);
			
			array.Move (id3, 0);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 0);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 4);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 0);
			
			array.Move (id3, 10);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 0);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 4);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 10);
			
			array.Move (id1, 1);
			array.Move (id2, 0);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 1);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 0);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 10);
			
			array.Move (id1, 1);
			array.Move (id2, 2);
			array.Move (id3, 3);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 1);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 2);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 3);
			
			array.Remove (id1);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 2);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 3);
			Debug.Assert.IsTrue (array.GetElementCount () == 2);
			
			array.Add (id1, 1);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 1);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 2);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 3);
			
			array.Remove (id2);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 1);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 3);
			Debug.Assert.IsTrue (array.GetElementCount () == 2);
			
			array.Add (id2, 2);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 1);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 2);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 3);
			
			array.Remove (id3);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 1);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 2);
			Debug.Assert.IsTrue (array.GetElementCount () == 2);
			
			array.Add (id3, 3);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id1) == 1);
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 2);
			Debug.Assert.IsTrue (array.GetCursorPosition (id3) == 3);
			
			array.Remove (id3);
			array.Remove (id1);
			
			Debug.Assert.IsTrue (array.GetCursorPosition (id2) == 2);
			Debug.Assert.IsTrue (array.GetElementCount () == 1);
			
			array.Remove (id2);
			
			Debug.Assert.IsTrue (array.GetElementCount () == 0);
			
			Internal.CursorIdArray a = new Internal.CursorIdArray ();
			Internal.CursorIdArray b = new Internal.CursorIdArray ();
			
			a.Add (id1, 10);
			a.Add (id2, 20);
			a.Add (id3, 30);
			
			a.ProcessMigration (25, b);
			
			Debug.Assert.IsTrue (a.GetElementCount () == 2);
			Debug.Assert.IsTrue (b.GetElementCount () == 1);
			
			Debug.Assert.IsTrue (a.GetCursorPosition (id1) == 10);
			Debug.Assert.IsTrue (a.GetCursorPosition (id2) == 20);
			Debug.Assert.IsTrue (b.GetCursorPosition (id3) == 5);
			
			a.ProcessMigration (20, b);
			
			Debug.Assert.IsTrue (a.GetElementCount () == 2);
			Debug.Assert.IsTrue (b.GetElementCount () == 1);
			
			Debug.Assert.IsTrue (a.GetCursorPosition (id1) == 10);
			Debug.Assert.IsTrue (a.GetCursorPosition (id2) == 20);
			Debug.Assert.IsTrue (b.GetCursorPosition (id3) == 5);
			
			a.ProcessMigration (18, b);
			
			Debug.Assert.IsTrue (a.GetElementCount () == 1);
			Debug.Assert.IsTrue (b.GetElementCount () == 2);
			
			Debug.Assert.IsTrue (a.GetCursorPosition (id1) == 10);
			Debug.Assert.IsTrue (b.GetCursorPosition (id2) == 2);
			Debug.Assert.IsTrue (b.GetCursorPosition (id3) == 5);
			
			a.Add (id2, 20);
			a.Add (id3, 30);
			
			b.Remove (id2);
			b.Remove (id3);
			
			a.ProcessMigration (5, b);
			
			Debug.Assert.IsTrue (a.GetElementCount () == 0);
			Debug.Assert.IsTrue (b.GetElementCount () == 3);
			
			Debug.Assert.IsTrue (b.GetCursorPosition (id1) == 5);
			Debug.Assert.IsTrue (b.GetCursorPosition (id2) == 15);
			Debug.Assert.IsTrue (b.GetCursorPosition (id3) == 25);
		}
	}
}
