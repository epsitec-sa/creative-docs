using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Cresus.Strings.ViewModels;
using Epsitec.Cresus.Strings.Views;
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
			bundle.Accept (new ResourceBundleTracer ());
		}

		[TestMethod]
		public void MapBundle()
		{
			var bundle1 = ResourceBundle.Load (TestData.Strings00Path);
			var bundle2 = ResourceBundle.Load (TestData.StringsDePath);
			var bundle3 = ResourceBundle.Load (TestData.Captions00Path);

			var mapper = new ResourceBundleTracer ();
			bundle1.Accept (mapper);
			bundle2.Accept (mapper);
			bundle3.Accept (mapper);
		}

		private class ResourceBundleTracer : ResourceVisitor
		{
			public override ResourceNode VisitItem(ResourceItem item)
			{
				var node = base.VisitItem (item);
				Trace.WriteLine (node.ToString ());
				return node;
			}
			public override ResourceNode VisitBundle(ResourceBundle bundle)
			{
				var node = base.VisitBundle (bundle);
				Trace.WriteLine (node.ToString ());
				return node;
			}
		}
	}
}
