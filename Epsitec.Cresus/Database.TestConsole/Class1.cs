using System;

namespace Epsitec.Cresus.Database.TestConsole
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			DbFactory.Initialise ();
			DbFactory.DebugDumpRegisteredDbAbstractions ();
		}
	}
}
