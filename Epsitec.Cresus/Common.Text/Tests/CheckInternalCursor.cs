//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
