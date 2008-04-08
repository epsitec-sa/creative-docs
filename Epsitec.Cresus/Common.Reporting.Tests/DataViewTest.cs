//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	[TestFixture]
	public class DataViewTest
	{
		[Test]
		public void Check01SimpleDataView()
		{
			GenericEntity root = new GenericEntity (Druid.Empty);
			DataViewContext context = new DataViewContext ();
			DataView view = DataView.CreateRoot (context, root);

			Assert.IsNotNull (DataView.GetValue (view, ""));
			Assert.IsNotNull (DataView.GetValue (view, null));

			DataView.DataItem item = DataView.GetValue (view, "") as DataView.DataItem;

			Assert.IsNotNull (item);
			Assert.AreEqual (item.DataView, view);
			Assert.AreEqual (item.ObjectValue, root);
			Assert.AreEqual (item.ValueStore, root);
		}
		
		[Test]
		public void Check02RecursiveDataView()
		{
			GenericEntity root = new GenericEntity (Druid.Empty);
			root.SetField<GenericEntity> ("loop", root);

			Assert.AreEqual (root, root.GetField<GenericEntity> ("loop"));

			DataViewContext context = new DataViewContext ();
			DataView view = DataView.CreateRoot (context, root);

			DataView.DataItem item = DataView.GetValue (view, "loop") as DataView.DataItem;

			Assert.IsNotNull (item);
			Assert.AreNotEqual (item.DataView, view);
			Assert.AreEqual (item.ObjectValue, root);
			Assert.AreEqual (item.ValueStore, root);
		}
	}
}
