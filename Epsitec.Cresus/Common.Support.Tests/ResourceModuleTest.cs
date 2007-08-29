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
			info.Versions.Add (new ResourceModuleVersion (1, 1234, System.DateTime.Now));
			info.Versions.Add (new ResourceModuleVersion (2, 789, new System.DateTime (2007, 8, 19, 10, 11, 12, System.DateTimeKind.Local)));

			Assert.AreEqual (2, info.Versions[1].DeveloperId);
			Assert.AreEqual (789, info.Versions[1].BuildNumber);
			Assert.AreEqual (new System.DateTime (2007, 8, 19, 8, 11, 12, System.DateTimeKind.Utc), info.Versions[1].BuildDate);
			
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

			Assert.AreEqual (2, info1.Versions.Count);
			Assert.AreEqual (2, info1.Versions[1].DeveloperId);
			Assert.AreEqual (789, info1.Versions[1].BuildNumber);
			Assert.AreEqual (new System.DateTime (2007, 8, 19, 8, 11, 12, System.DateTimeKind.Utc), info1.Versions[1].BuildDate);

			Assert.IsTrue (pool.FindReferenceModules ().Count < Types.Collection.Count (pool.Modules));

			ResourceManager manager1 = new ResourceManager (pool, info1);
			ResourceManager manager2 = manager1.GetManagerForReferenceModule ();
			ResourceManager manager3 = manager2.GetManagerForReferenceModule ();

			Assert.IsTrue (manager1.BasedOnPatchModule);
			Assert.IsFalse (manager2.BasedOnPatchModule);

			Assert.IsNotNull (manager2);
			Assert.IsNull (manager3);
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
				System.Console.Out.WriteLine ("{0}: {1} in {2}  {3}", info.FullId.Name, info.FullId.Id, info.FullId.Path, info.SourceNamespace);
			}
		}

		[Test]
		public void CheckGetVersionFingerprint()
		{
			string root = @"S:\Epsitec.Cresus\Common.Support.Tests\Resources";

			ResourceModuleInfo info = new ResourceModuleInfo ();

			info.FullId = new ResourceModuleId ("Common.Support", root + @"\Customizations", 7, ResourceModuleLayer.System);

			Assert.AreEqual ("", ResourceModule.GetVersionFingerprint (null));
			Assert.AreEqual ("", ResourceModule.GetVersionFingerprint (info));

			info.Versions.Add (new ResourceModuleVersion (1, 1234, new System.DateTime (2007, 8, 1, 10, 11, 12, System.DateTimeKind.Local)));
			info.Versions.Add (new ResourceModuleVersion (1, 1230, new System.DateTime (2007, 8, 1, 10, 11, 12, System.DateTimeKind.Local)));
			info.Versions.Add (new ResourceModuleVersion (1, 1234, new System.DateTime (2007, 8, 1, 10, 11, 10, System.DateTimeKind.Local)));
			info.Versions.Add (new ResourceModuleVersion (3, 789, new System.DateTime (2007, 8, 19, 10, 11, 12, System.DateTimeKind.Local)));
			info.Versions.Add (new ResourceModuleVersion (2,  789, new System.DateTime (2007, 8, 19, 10, 11, 12, System.DateTimeKind.Local)));

			string fingerprint = ResourceModule.GetVersionFingerprint (info);

			Assert.AreEqual ("1/1230-1/1234-1/1234-2/789-3/789", fingerprint);
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
		
		[Test]
		public void CheckPatchModule()
		{
			ResourceManagerPool pool = new ResourceManagerPool ("Test");

			pool.AddModuleRootPath ("%epsitec%", @"S:\Epsitec.Cresus");
			pool.AddModuleRootPath ("%test%", @"S:\Epsitec.Cresus\Common.Support.Tests\Resources");

			pool.ScanForModules ("%test%");

			ResourceModuleInfo info1 = pool.FindModuleInfo (@"%test%\Common.Support");
			ResourceModuleInfo info2 = pool.FindModuleInfo (@"%test%\Common.Support.Patch");

			Assert.AreEqual (@"%test%\Common.Support", pool.GetRootRelativePath (info1.FullId.Path));
			Assert.AreEqual (@"S:\Epsitec.Cresus\Common.Support.Tests\Resources\Common.Support", pool.GetRootAbsolutePath (info1.FullId.Path));
			Assert.AreEqual (@"S:\Epsitec.Cresus\Common.Support.Tests\Resources\Common.Support.Patch", pool.GetRootAbsolutePath (info2.FullId.Path));
			Assert.AreEqual (@"%test%\Common.Support", pool.GetRootRelativePath (info2, info2.ReferenceModulePath));
			Assert.AreEqual (@"S:\Epsitec.Cresus\Common.Support.Tests\Resources\Common.Support", pool.GetRootAbsolutePath (info2, info2.ReferenceModulePath));

			Assert.IsNotNull (info1);
			Assert.IsNotNull (info2);

			Assert.AreEqual (2, pool.FindPatchModuleInfos (info1).Count);
			Assert.AreEqual (info2, pool.FindPatchModuleInfos (info1)[0]);

			ResourceManager manager1 = new ResourceManager (pool, info1);
			ResourceManager manager2 = new ResourceManager (pool, info2);

			Assert.IsFalse (manager1.BasedOnPatchModule);
			Assert.IsTrue (manager2.BasedOnPatchModule);

			Assert.IsNull (manager1.GetManagerForReferenceModule ());
			Assert.IsNotNull (manager2.GetManagerForReferenceModule ());
			Assert.IsFalse (manager2.GetManagerForReferenceModule ().BasedOnPatchModule);

			ResourceAccessors.StringResourceAccessor stringAccessor1 = new ResourceAccessors.StringResourceAccessor ();
			ResourceAccessors.StringResourceAccessor stringAccessor2 = new ResourceAccessors.StringResourceAccessor ();

			stringAccessor1.Load (manager1);
			stringAccessor2.Load (manager2);

			Assert.AreEqual (4, stringAccessor1.Collection.Count);
			Assert.AreEqual ("Image.Description", stringAccessor1.Collection[0].Name);
			Assert.AreEqual ("Author", stringAccessor1.Collection[1].Name);
			Assert.AreEqual ("Null", stringAccessor1.Collection[2].Name);
			Assert.AreEqual ("CopyrightHolder", stringAccessor1.Collection[3].Name);
			Assert.AreEqual (CultureMapSource.ReferenceModule, stringAccessor1.Collection[0].Source);

			Assert.AreEqual (5, stringAccessor2.Collection.Count);
			Assert.AreEqual ("Image.Description", stringAccessor2.Collection[0].Name);
			Assert.AreEqual ("Author", stringAccessor2.Collection[1].Name);
			Assert.AreEqual ("Null", stringAccessor2.Collection[2].Name);
			Assert.AreEqual ("CopyrightHolder", stringAccessor2.Collection[3].Name);
			Assert.AreEqual ("Foo", stringAccessor2.Collection[4].Name);
			Assert.AreEqual (CultureMapSource.ReferenceModule, stringAccessor2.Collection[0].Source);
			Assert.AreEqual (CultureMapSource.DynamicMerge, stringAccessor2.Collection[1].Source);
			Assert.AreEqual (CultureMapSource.ReferenceModule, stringAccessor2.Collection[2].Source);
			Assert.AreEqual (CultureMapSource.ReferenceModule, stringAccessor2.Collection[3].Source);
			Assert.AreEqual (CultureMapSource.PatchModule, stringAccessor2.Collection[4].Source);

			Assert.IsTrue (ResourceModuleTest.GetText (stringAccessor1, 0, null).StartsWith ("Taille"));
			Assert.IsTrue (ResourceModuleTest.GetText (stringAccessor2, 0, null).StartsWith ("Taille"));
			
			Assert.AreEqual ("Pierre Arnaud", ResourceModuleTest.GetText (stringAccessor1, 1, null));
			Assert.AreEqual (null, ResourceModuleTest.GetText (stringAccessor1, 1, "de"));
			Assert.AreEqual (null, ResourceModuleTest.GetText (stringAccessor1, 1, "en"));
			Assert.AreEqual ("Cf. Common.Support.Tests", ResourceModuleTest.GetComment (stringAccessor1, 1, null));
			Assert.AreEqual ("Author muss nicht �bersetzt werden", ResourceModuleTest.GetComment (stringAccessor1, 1, "de"));
			Assert.AreEqual (null, ResourceModuleTest.GetComment (stringAccessor1, 1, "en"));
			
			Assert.AreEqual ("Pierre ARNAUD", ResourceModuleTest.GetText (stringAccessor2, 1, null));
			Assert.AreEqual (null, ResourceModuleTest.GetText (stringAccessor1, 2, null));
			Assert.AreEqual (null, ResourceModuleTest.GetText (stringAccessor2, 2, null));
			Assert.AreEqual ("Pierre Arnaud &amp; EPSITEC SA", ResourceModuleTest.GetText (stringAccessor1, 3, null));
			Assert.AreEqual ("Pierre Arnaud &amp; EPSITEC SA", ResourceModuleTest.GetText (stringAccessor2, 3, null));
			Assert.AreEqual ("Pierre Arnaud &amp; EPSITEC SA (Schweiz)", ResourceModuleTest.GetText (stringAccessor1, 3, "de"));
			Assert.AreEqual ("Pierre Arnaud &amp; EPSITEC SA (Schweiz)", ResourceModuleTest.GetText (stringAccessor2, 3, "de"));	//<--
			Assert.AreEqual (null, ResourceModuleTest.GetText (stringAccessor1, 3, "en"));
			Assert.AreEqual ("Pierre Arnaud &amp; EPSITEC SA (Switzerland)", ResourceModuleTest.GetText (stringAccessor2, 3, "en"));
			Assert.AreEqual ("foo", ResourceModuleTest.GetText (stringAccessor2, 4, null));
			Assert.AreEqual ("Cf. Common.Support.Tests", ResourceModuleTest.GetComment (stringAccessor1, 1, null));
			Assert.AreEqual ("Cf. Common.Support.Tests", ResourceModuleTest.GetComment (stringAccessor2, 1, null));
		}

		private static string GetText(ResourceAccessors.StringResourceAccessor accessor, int index, string culture)
		{
			return accessor.Collection[index].GetCultureData (culture ?? Resources.DefaultTwoLetterISOLanguageName).GetValue (Common.Support.Res.Fields.ResourceString.Text) as string;
		}

		private static string GetComment(IResourceAccessor accessor, int index, string culture)
		{
			return accessor.Collection[index].GetCultureData (culture ?? Resources.DefaultTwoLetterISOLanguageName).GetValue (Common.Support.Res.Fields.ResourceBase.Comment) as string;
		}
		
		private static int GetModificationId(IResourceAccessor accessor, int index, string culture)
		{
			return (int) accessor.Collection[index].GetCultureData (culture ?? Resources.DefaultTwoLetterISOLanguageName).GetValue (Common.Support.Res.Fields.ResourceBase.ModificationId);
		}
	}
}
