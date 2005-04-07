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
			Tests.CheckSerializerSupport.RunTests ();
			Tests.CheckTextFitter.RunTests ();
			Tests.CheckLayout.RunTests ();
			Tests.CheckProperties.RunTests ();
			Tests.CheckStretchProfile.RunTests ();
			
			OpenType.Tests.CheckTables.RunTests ();
			
			Tests.CheckUnicode.RunTests ();
			Tests.CheckInternalCursor.RunTests ();
			Tests.CheckInternalCursorTable.RunTests ();
			Tests.CheckInternalCursorIdArray.RunTests ();
			Tests.CheckTextConverter.RunTests ();
//			Tests.CheckTextTable.RunTests ();
			Tests.CheckTextStory.RunTests ();
			
//			CheckPerformance.RunTests (100*1000, 1000);
			
//			Tests.CheckInternalCursor.OptimizerTest ();
//			System.Diagnostics.Debugger.Break ();
//			Tests.CheckInternalCursor.OptimizerTest ();
		}
		
	}
}
