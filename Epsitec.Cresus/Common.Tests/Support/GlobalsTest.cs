using NUnit.Framework;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture] public class GlobalsTest
	{
		[Test] public void CheckDirectories()
		{
			System.Console.Out.WriteLine ("Common App Data: {0}", Globals.Directories.CommonAppDataRevision);
			System.Console.Out.WriteLine ("User App Data:   {0}", Globals.Directories.UserAppDataRevision);
			System.Console.Out.WriteLine ("Executable:      {0}", Globals.Directories.Executable);
			System.Console.Out.WriteLine ("Executable Name: {0}", Globals.ExecutableName);
		}
	}
}
