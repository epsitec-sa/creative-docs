using System;
using System.IO;
using System.Linq;
using Epsitec.Cresus.ResourceManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class ResourceModuleTest
	{
		[TestMethod]
		public void Load()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath);
			Assert.AreEqual (3, module.Count ());
		}

		[TestMethod]
		public void ByNameFirst()
		{
			var resources = ResourceModule.Load (TestData.ModuleInfoPath).ByNameFirst;
			var stringsFr = resources["Strings"]["fr"];
			var stringsDe = resources["Strings"]["de"];
			var captionsFr = resources["Captions"]["fr"];

			var item1 = resources["Strings"]["fr"].ByName["Application.Name"];
			var item2 = resources["Strings"]["fr"].ById["A"];

			Assert.AreEqual (item1, item2);
		}

		[TestMethod]
		public void ByCultureFirst()
		{
			var resources = ResourceModule.Load (TestData.ModuleInfoPath).ByCultureFirst;
			var frStrings = resources["fr"]["Strings"];
			var deStrings = resources["de"]["Strings"];
			var frCaptions = resources["fr"]["Captions"];
		}

		[TestMethod]
		public void ByName()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath);
			var byName =
				(from n in module
				 group n by n.Name into ng
				 select new
				 {
					 Name = ng.Key,
					 NameGroups = ng
				 }).ToDictionary (a => a.Name, a => a.NameGroups);

			foreach (var kvBundles in byName)
			{
				var name = kvBundles.Key;
				foreach (var bundle in kvBundles.Value)
				{
					var culture = bundle.Culture;
					foreach (var item in bundle)
					{
					}
				}
			}
		}
	}
}
