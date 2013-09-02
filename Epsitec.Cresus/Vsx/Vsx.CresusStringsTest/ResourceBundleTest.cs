using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Cresus.ResourceManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class ResourceBundleTest
	{
		[TestMethod]
		public void Load()
		{
			var bundle = new ResourceBundle (TestData.Strings00Path, null);
		}

		[TestMethod]
		public void TraceBundle()
		{
			var bundle = new ResourceBundle (TestData.Strings00Path, null);
			bundle.Accept (new ResourceBundleTracer ());
		}

		[TestMethod]
		public void MapBundle()
		{
			var bundle1 = new ResourceBundle (TestData.Strings00Path, null);
			var bundle2 = new ResourceBundle (TestData.StringsDePath, bundle1);
			var bundle3 = new ResourceBundle (TestData.Captions00Path, null);

			var mapper = new ResourceMapper ();
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
