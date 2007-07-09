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
		public void CheckCreateModuleInfo()
		{
			string root = @"S:\Epsitec.Cresus\Common.Support.Tests\Resources";

			ResourceModuleInfo info = new ResourceModuleInfo ();

			info.FullId = new ResourceModuleId ("Common.Support", root + @"\Customizations", 7, ResourceModuleLayer.System);
			info.ReferenceModulePath = @"%test%\Common.Support";

			ResourceModule.SaveManifest (info);

			System.Console.Out.WriteLine (System.IO.File.ReadAllText (System.IO.Path.Combine (info.FullId.Path, "module.info")));

			ResourceManagerPool pool = new ResourceManagerPool ("Test");

			pool.AddModuleRootPath ("%epsitec%", @"S:\Epsitec.Cresus");
			pool.AddModuleRootPath ("%test%", root);

			ResourceModuleInfo info1 = pool.GetModuleInfo (@"%test%\Customizations");
			ResourceModuleInfo info2 = pool.GetModuleInfo (info1.ReferenceModulePath);

			Assert.AreEqual (7, info1.FullId.Id);
			Assert.AreEqual (7, info2.FullId.Id);
			Assert.AreEqual (info.ReferenceModulePath, info1.ReferenceModulePath);
			Assert.IsNull (info2.ReferenceModulePath);

			Assert.AreEqual (2, pool.FindModuleInfos (7).Count);

			Assert.AreEqual (1, pool.FindPatchModuleInfos (info2).Count);
			Assert.AreEqual (info1, pool.FindPatchModuleInfos (info2)[0]);

			Assert.IsTrue (pool.FindReferenceModules ().Count < Types.Collection.Count (pool.Modules));
		}

		[Test]
		public void CheckFindModuleInfos()
		{
			ResourceManagerPool pool = new ResourceManagerPool ("Test");

			pool.AddModuleRootPath ("%epsitec%", @"S:\Epsitec.Cresus");

			pool.ScanForModules ("%epsitec%");

			Assert.IsTrue (Types.Collection.Count (pool.Modules) > 80);
			
			Assert.IsTrue (pool.FindModuleInfos ("Common.Support").Count > 4);
			Assert.AreEqual (pool.FindModuleInfos ("Common.Support").Count, pool.FindModuleInfos (7).Count);

			foreach (ResourceModuleInfo info in pool.FindModuleInfos ("Common.Support"))
			{
				System.Console.Out.WriteLine ("{0}: {1} in {2}", info.FullId.Name, info.FullId.Id, info.FullId.Path);
			}
		}

		[Test]
		public void CheckModuleRootPaths()
		{
			ResourceManagerPool pool = new ResourceManagerPool ("Test");

			pool.AddModuleRootPath ("%program files%", @"C:\Program Files");
			pool.AddModuleRootPath ("%sys drive%", @"C:");
			pool.AddModuleRootPath ("%app%", @"C:\Program Files\Internet Explorer");
			pool.AddModuleRootPath ("%app%", @"C:\Program Files\Epsitec");
			pool.AddModuleRootPath ("%source drive%", @"S:");
			pool.AddModuleRootPath ("%source drive%", @"");
			
			Assert.AreEqual (@"S:\abc", pool.GetRootRelativePath (@"S:\abc"));
			Assert.AreEqual (@"%sys drive%\abc", pool.GetRootRelativePath (@"C:\abc"));
			Assert.AreEqual (@"%program files%\abc", pool.GetRootRelativePath (@"C:\Program Files\abc"));
			Assert.AreEqual (@"%app%\abc", pool.GetRootRelativePath (@"C:\Program Files\Epsitec\abc"));
			
			Assert.AreEqual (@"S:\abc", pool.GetRootAbsolutePath (@"S:\abc"));
			Assert.AreEqual (@"C:\abc", pool.GetRootAbsolutePath (@"%sys drive%\abc"));
			Assert.AreEqual (@"C:\Program Files\abc", pool.GetRootAbsolutePath (@"%program files%\abc"));
			Assert.AreEqual (@"C:\Program Files\Epsitec\abc", pool.GetRootAbsolutePath (@"%app%\abc"));
		}
	}
}


