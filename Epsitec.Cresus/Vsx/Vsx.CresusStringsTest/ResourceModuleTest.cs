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
			var module = ResourceModule.Load (TestData.ModuleInfoPath).ByNameFirst;
			var stringsFr = module["Strings"]["fr"];
			var stringsDe = module["Strings"]["de"];
			var captionsFr = module["Captions"]["fr"];

			var item1 = module["Strings"]["fr"].ByName["Application.Name"];
			var item2 = module["Strings"]["fr"].ById["A"];

			Assert.AreEqual (item1, item2);
		}

		[TestMethod]
		public void ByCultureFirst()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath).ByCultureFirst;
			var frStrings = module["fr"]["Strings"];
			var deStrings = module["de"]["Strings"];
			var frCaptions = module["fr"]["Captions"];
		}
	}
}
