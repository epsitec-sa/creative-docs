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
		}
	}
}
