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

		[Test]
		public void CheckCreateModuleInfo()
		{
			ResourceModuleInfo info = new ResourceModuleInfo ();

			info.FullId = new ResourceModuleId ("Common.Support", @"S:\Epsitec.Cresus\Common.Support.Tests\Resources\Customizations", 7, ResourceModuleLayer.System);
			info.ReferenceModulePath = @"S:\Epsitec.Cresus\Common.Support\Resources\Common.Support";

			ResourceModule.SaveManifest (info);

			System.Console.Out.WriteLine (System.IO.File.ReadAllText (System.IO.Path.Combine (info.FullId.Path, "module.info")));
		}
	}
}


