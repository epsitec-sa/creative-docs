using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class XmlAccessTest
	{
		[TestMethod]
		public void XAttributeAccessTime()
		{
			var doc = XDocument.Load (TestData.Strings00Path);
			var bundle = doc.Root;
			using (new TimeTrace ())
			{
				foreach (var _ in Enumerable.Repeat (bundle.Attribute ("name"), 1000))
				{
				}
			}
		}
		[TestMethod]
		public void DictionaryAccessTime()
		{
			var doc = XDocument.Load (TestData.Strings00Path);
			var bundle = new Dictionary<string, string> ()
			{
				{ "name", "Value" }
			};
			using (new TimeTrace ())
			{
				foreach (var _ in Enumerable.Repeat (bundle["name"], 1000))
				{
				}
			}
		}
	}
}
