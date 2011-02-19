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

			Assert.AreEqual (DataItemType.Vector, item1.ItemType);
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
			Assert.AreEqual ("text.foo.bar", DataView.GetPath (item5, "foo", "bar"));
		}

		[Test]
		public void Check05CollectionValue()
		{
			GenericEntity root = new GenericEntity (Druid.Empty);
			GenericEntity node1 = new GenericEntity (Druid.Empty);
			GenericEntity node2 = new GenericEntity (Druid.Empty);

			node1.SetField<string> ("name", "node 1");
			node2.SetField<string> ("name", "node 2");
			node2.SetField<string> ("xyz", "other data");
			
			ICollection<GenericEntity> nodes = root.GetFieldCollection<GenericEntity> ("nodes");

			nodes.Add (node1);
			nodes.Add (node2);

			DataViewContext context = new DataViewContext ();
			DataView view = DataView.CreateRoot (context, root);

			DataView.DataItem item1 = DataView.GetDataItem (view, "nodes") as DataView.DataItem;
			DataView.DataItem item2 = DataView.GetDataItem (view, "nodes.@0") as DataView.DataItem;
			DataView.DataItem item3 = DataView.GetDataItem (view, "nodes.@1") as DataView.DataItem;
			DataView.DataItem item4 = DataView.GetDataItem (view, "nodes.@1.name") as DataView.DataItem;

			Assert.IsNotNull (item1);
			Assert.IsNotNull (item2);
			Assert.IsNotNull (item3);
			Assert.IsNotNull (item4);
			
			Assert.AreEqual (DataItemType.Table, item1.ItemType);
			Assert.AreEqual (2, item1.Count);
			Assert.AreEqual (DataItemType.Vector, item2.ItemType);
			Assert.AreEqual (DataItemType.Vector, item3.ItemType);
			Assert.AreEqual (DataItemType.Value, item4.ItemType);
			Assert.AreEqual ("node 2", item4.ObjectValue);

			Assert.AreEqual ("nodes", DataView.GetPath (item1));
			Assert.AreEqual ("nodes.@0", DataView.GetPath (item2));
			Assert.AreEqual ("nodes.@1", DataView.GetPath (item3));
			Assert.AreEqual ("nodes.@1.name", DataView.GetPath (item4));

			Assert.AreEqual ("@1", item1.GetNextChildId ("@0"));
			Assert.AreEqual (null, item1.GetNextChildId ("@1"));
			Assert.AreEqual ("@0", item1.GetPreviousChildId ("@1"));
			Assert.AreEqual (null, item1.GetPreviousChildId ("@0"));
			Assert.AreEqual (null, item2.GetNextChildId ("name"));
			Assert.AreEqual (null, item2.GetPreviousChildId ("name"));
			
			Assert.AreEqual ("xyz",  item3.GetNextChildId ("name"));
			Assert.AreEqual (null,   item3.GetPreviousChildId ("name"));
			Assert.AreEqual (null,   item3.GetNextChildId ("xyz"));
			Assert.AreEqual ("name", item3.GetPreviousChildId ("xyz"));
		}
	}
}
