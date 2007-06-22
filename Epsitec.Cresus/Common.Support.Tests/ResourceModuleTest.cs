using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ResourceModuleTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void CheckFindModulePaths()
		{
			foreach (string path in ResourceModule.FindModulePaths (@"S:\Epsitec.Cresus"))
			{
				System.Console.Out.WriteLine (path);
			}
		}
	}
}


