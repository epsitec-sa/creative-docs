using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class GlobalsTest
	{
		[Test] public void CheckDirectories()
		{
			System.Console.Out.WriteLine ("Common App Data: {0}", Globals.Directories.CommonAppData);
			System.Console.Out.WriteLine ("User App Data:   {0}", Globals.Directories.UserAppData);
			System.Console.Out.WriteLine ("Executable:      {0}", Globals.Directories.Executable);
		}
	}
}
