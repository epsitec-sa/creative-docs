//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Summary description for CheckInternalCursor.
	/// </summary>
	public sealed class CheckInternalCursor
	{
		public static void RunTests()
		{
			Internal.Cursor c1 = new Internal.Cursor ();
			Internal.Cursor c2 = new Internal.Cursor (c1);
			Internal.Cursor c3 = c1;
			
			Debug.Assert.IsTrue (c1.CursorState == Internal.CursorState.Copied);
			Debug.Assert.IsTrue (c2.CursorState == Internal.CursorState.Copied);
			Debug.Assert.IsTrue (c3.CursorState == Internal.CursorState.Copied);
			
			c3.DefineCursorState (Internal.CursorState.Allocated);
			
			Internal.Cursor c4 = c3;
			Internal.Cursor c5 = new Internal.Cursor (c3);
			
			Debug.Assert.IsTrue (c3.CursorState == Internal.CursorState.Allocated);
			Debug.Assert.IsTrue (c4.CursorState == Internal.CursorState.Allocated);
			Debug.Assert.IsTrue (c5.CursorState == Internal.CursorState.Copied);
			
			CursorInfo[] infos = new CursorInfo[4];
			
			infos[0] = new CursorInfo (0, 10, 0);
			infos[1] = new CursorInfo (5, 10, 1);
			infos[2] = new CursorInfo (3, 10, -1);
			infos[3] = new CursorInfo (8, 8, 1);
			
			System.Array.Sort (infos, CursorInfo.PositionComparer);
			
			Debug.Assert.IsTrue (infos[0].Position == 8);
			Debug.Assert.IsTrue (infos[1].Position == 10);
			Debug.Assert.IsTrue (infos[2].Position == 10);
			Debug.Assert.IsTrue (infos[3].Position == 10);
			Debug.Assert.IsTrue (infos[1].Direction == -1);
			Debug.Assert.IsTrue (infos[2].Direction == 0);
			Debug.Assert.IsTrue (infos[3].Direction == 1);
			
			System.Array.Sort (infos, CursorInfo.CursorIdComparer);
			
			Debug.Assert.IsTrue (infos[0].CursorId == 0);
			Debug.Assert.IsTrue (infos[1].CursorId == 3);
			Debug.Assert.IsTrue (infos[2].CursorId == 5);
			Debug.Assert.IsTrue (infos[3].CursorId == 8);
		}
		
		
		public static void OptimizerTest()
		{
			Internal.CursorId id1 = 10;
			Internal.CursorId id2 = id1;
			CheckInternalCursor.DumpCursorId (id2);
		}
		
		private static void DumpCursorId(int value)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor ID is {0}.", value));
		}
	}
}
