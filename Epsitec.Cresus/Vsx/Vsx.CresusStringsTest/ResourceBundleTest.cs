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
		public void TracingVisit()
		{
			var bundle = new ResourceBundle (TestData.Strings00Path, null);
			bundle.Accept (new TracingVisitor ());
		}

		[TestMethod]
		public void MappingVisit()
		{
			var bundle1 = new ResourceBundle (TestData.Strings00Path, null);
			var bundle2 = new ResourceBundle (TestData.StringsDePath, bundle1);
			var bundle3 = new ResourceBundle (TestData.Captions00Path, null);

			var visitor = new MappingVisitor ();
			bundle1.Accept (visitor);
			bundle2.Accept (visitor);
			bundle3.Accept (visitor);
		}

		private class TracingVisitor : ResourceVisitor
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

		private class MappingVisitor : ResourceVisitor
		{
			public override ResourceNode VisitItem(ResourceItem item)
			{
				item = base.VisitItem (item) as ResourceItem;

				var subkeys = new string[] { this.bundle.Name }.Concat (item.Name.Split ('.'));

				var map = this.map;
				foreach (var subkey in subkeys)
				{
					map = map.GetOrAdd (subkey, key => new Dictionary<string, object> ()) as Dictionary<string, object>;
				}
				map[this.bundle.Culture.Name] = item;
				return item;
			}

			public override ResourceNode VisitBundle(ResourceBundle bundle)
			{
				this.bundle = bundle;
				return base.VisitBundle (bundle);
			}

			private ResourceBundle bundle;
			public readonly Dictionary<string, object> map = new Dictionary<string, object> ();
		}
	}
}
