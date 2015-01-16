using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class Spikes
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

		[TestMethod]
		public void RegexTest1()
		{
			var testData = "a.b.c";
			var matches = Regex.Matches (testData, @"((?:\.|^).+?)(?=\.|$)");
			var result = matches.Cast<Match>().Select (m => m.Value).Select(s => Regex.Escape(s)).Reverse().Aggregate((s1, s2) => string.Format("{1}({0})?", s1, s2));
			var pattern = @"(?<=\.|^)" + result + @"(?=\.|$)";
			var match = Regex.Match ("a.b", pattern);
		}
	}
}
