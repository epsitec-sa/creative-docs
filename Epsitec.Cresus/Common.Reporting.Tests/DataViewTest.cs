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

			Assert.IsNotNull (DataView.GetDataItem (view, ""));
			Assert.IsNotNull (DataView.GetDataItem (view, null));

			DataView.DataItem item = DataView.GetDataItem (view, "") as DataView.DataItem;

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

			DataView.DataItem item1 = DataView.GetDataItem (view, "loop") as DataView.DataItem;
			DataView.DataItem item2 = DataView.GetDataItem (view, "loop") as DataView.DataItem;
			DataView.DataItem item3 = DataView.GetDataItem (view, "loop.loop") as DataView.DataItem;

			Assert.AreNotEqual (item1.DataView, view);

			Assert.AreEqual (item1.ValueStore, root);
			Assert.AreEqual (item2.ValueStore, root);
			Assert.AreEqual (item3.ValueStore, root);

			Assert.AreEqual (item1, item2);
			Assert.AreNotEqual (item1, item3);						//	item for loop != loop.loop
			Assert.AreNotEqual (item1.DataView, item3.DataView);	//	view for loop != loop.loop
		}

		[Test]
		public void Check03SimpleValues()
		{
			GenericEntity root = new GenericEntity (Druid.Empty);
			root.SetField<GenericEntity> ("loop", root);
			root.SetField<string> ("text", "hello");
			root.SetField<int> ("number", 123);

			Assert.AreEqual (root, root.GetField<GenericEntity> ("loop"));
			Assert.AreEqual ("hello", root.GetField<string> ("text"));
			Assert.AreEqual (123, root.GetField<int> ("number"));

			DataViewContext context = new DataViewContext ();
			DataView view = DataView.CreateRoot (context, root);

			DataView.DataItem item1 = DataView.GetDataItem (view, "loop") as DataView.DataItem;
			DataView.DataItem item2 = DataView.GetDataItem (view, "loop.text") as DataView.DataItem;
			DataView.DataItem item3 = DataView.GetDataItem (view, "loop.number") as DataView.DataItem;

			Assert.AreEqual ("hello", (string) item2.ObjectValue);
			Assert.AreEqual (123, (int) item3.ObjectValue);

			Assert.AreEqual (DataItemClass.ValueRow, item1.ItemClass);
			Assert.AreEqual (DataItemClass.ValueItem, item2.ItemClass);
			Assert.AreEqual (DataItemClass.ValueItem, item3.ItemClass);

			Assert.AreEqual (DataItemType.Row, item1.ItemType);
			Assert.AreEqual (DataItemType.Value, item2.ItemType);
			Assert.AreEqual (DataItemType.Value, item3.ItemType);
		}

		[Test]
		public void Check04GetPath()
		{
			GenericEntity root = new GenericEntity (Druid.Empty);
			root.SetField<GenericEntity> ("loop", root);
			root.SetField<string> ("text", "hello");
			root.SetField<int> ("number", 123);

			DataViewContext context = new DataViewContext ();
			DataView view = DataView.CreateRoot (context, root);

			DataView.DataItem item1 = DataView.GetDataItem (view, "loop") as DataView.DataItem;
			DataView.DataItem item2 = DataView.GetDataItem (view, "loop.text") as DataView.DataItem;
			DataView.DataItem item3 = DataView.GetDataItem (view, "loop.number") as DataView.DataItem;
			DataView.DataItem item4 = DataView.GetDataItem (view, "loop.loop.loop") as DataView.DataItem;
			DataView.DataItem item5 = DataView.GetDataItem (view, "text") as DataView.DataItem;

			Assert.AreEqual ("loop", DataView.GetPath (item1));
			Assert.AreEqual ("loop.text", DataView.GetPath (item2));
			Assert.AreEqual ("loop.number", DataView.GetPath (item3));
			Assert.AreEqual ("loop.loop.loop", DataView.GetPath (item4));
			Assert.AreEqual ("text", DataView.GetPath (item5));
		}
	}
}
