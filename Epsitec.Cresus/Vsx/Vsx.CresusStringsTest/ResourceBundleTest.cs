using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Cresus.Strings.Bundles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class ResourceBundleTest
	{
		[TestMethod]
		public void Load()
		{
			var bundle = ResourceBundle.Load (TestData.Strings00Path);
		}
	}
}
