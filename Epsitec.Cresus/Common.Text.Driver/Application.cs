//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Driver
{
	/// <summary>
	/// Summary description for Application.
	/// </summary>
	class Application
	{
		[System.STAThread] static void Main(string[] args)
		{
			Tests.CheckInternalCursor.RunTests ();
			Tests.CheckInternalCursorTable.RunTests ();
			Tests.CheckInternalCursorIdArray.RunTests ();
			
//			Tests.CheckInternalCursor.OptimizerTest ();
//			System.Diagnostics.Debugger.Break ();
//			Tests.CheckInternalCursor.OptimizerTest ();
		}
	}
}
