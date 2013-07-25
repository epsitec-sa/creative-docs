using System;
using System.IO;
using System.Linq;
using Epsitec.Cresus.Strings.Bundles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class ResourceModuleTest
	{
		[TestMethod]
		public void Load()
		{
			var module = ResourceModule.Load (ModuleInfoFilePath);
			Assert.AreEqual (3, module.Count ());
		}
		[TestMethod]
		public void ByNameFirst()
		{
			var resources = ResourceModule.Load (ModuleInfoFilePath).ByNameFirst;
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
			var resources = ResourceModule.Load (ModuleInfoFilePath).ByCultureFirst;
			var frStrings = resources["fr"]["Strings"];
			var deStrings = resources["de"]["Strings"];
			var frCaptions = resources["fr"]["Captions"];
		}

		private const string ModuleInfoFilePath = @"..\..\TestData\module.info";
	}
}
