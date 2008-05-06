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
	public class NavigatorTest
	{
		[Test]
		public void Check01Create()
		{
			GenericEntity root = new GenericEntity (Druid.Empty);
			DataViewContext context = new DataViewContext ();
			DataView view = DataView.CreateRoot (context, root);
			DataNavigator navigator = new DataNavigator (view);

			Assert.AreEqual ("", navigator.CurrentDataPath);
			Assert.AreEqual (1, Collection.Count (navigator.CurrentViewStack));
			Assert.IsNotNull (navigator.CurrentDataItem);
			Assert.AreEqual (DataItemType.Vector, navigator.CurrentDataItem.ItemType);
		}

		[Test]
		public void Check02NavigateToAndReset()
		{
			GenericEntity root = new GenericEntity (Druid.Empty);
			
			root.SetField<GenericEntity> ("loop", root);
			root.SetField<int> ("value", 123);

			DataViewContext context = new DataViewContext ();
			DataView view = DataView.CreateRoot (context, root);
			DataNavigator navigator = new DataNavigator (view);

			navigator.NavigateTo ("value");

			Assert.AreEqual ("value", navigator.CurrentDataPath);
			Assert.AreEqual (2, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Value, navigator.CurrentDataItem.ItemType);
			Assert.AreEqual (123, (int) navigator.CurrentDataItem.ObjectValue);

			navigator.NavigateTo ("loop.loop.loop");
			
			Assert.AreEqual ("loop.loop.loop", navigator.CurrentDataPath);
			Assert.AreEqual (4, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Vector, navigator.CurrentDataItem.ItemType);

			navigator.NavigateToParent ();

			Assert.AreEqual ("loop.loop", navigator.CurrentDataPath);
			Assert.AreEqual (3, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Vector, navigator.CurrentDataItem.ItemType);

			navigator.Reset ();

			Assert.AreEqual ("", navigator.CurrentDataPath);
			Assert.AreEqual (1, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Vector, navigator.CurrentDataItem.ItemType);
			
			navigator.NavigateTo ("loop.loop");
			navigator.NavigateToRelative ("loop.value");

			Assert.AreEqual ("loop.loop.loop.value", navigator.CurrentDataPath);
			Assert.AreEqual (5, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Value, navigator.CurrentDataItem.ItemType);
			Assert.AreEqual (123, (int) navigator.CurrentDataItem.ObjectValue);
		}

		[Test]
		public void Check03SimpleTable()
		{
			DataNavigator navigator = NavigatorTest.CreateSimpleTableNavigator ();

			navigator.NavigateTo ("table");

			Assert.AreEqual ("table", navigator.CurrentDataPath);
			Assert.AreEqual (2, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Table, navigator.CurrentDataItem.ItemType);

			navigator.NavigateTo ("table.@0");

			Assert.AreEqual ("table.@0", navigator.CurrentDataPath);
			Assert.AreEqual (3, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Vector, navigator.CurrentDataItem.ItemType);

			navigator.NavigateToRelative ("1st_name");

			Assert.AreEqual ("table.@0.1st_name", navigator.CurrentDataPath);
			Assert.AreEqual (4, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Value, navigator.CurrentDataItem.ItemType);
			Assert.AreEqual ("Pierre", navigator.CurrentDataItem.ObjectValue);

			Assert.IsTrue (navigator.NavigateToNext ());
			
			Assert.AreEqual ("table.@0.2nd_name", navigator.CurrentDataPath);
			Assert.AreEqual (4, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Value, navigator.CurrentDataItem.ItemType);
			Assert.AreEqual ("Arnaud", navigator.CurrentDataItem.ObjectValue);

			Assert.IsTrue (navigator.NavigateToNext ());

			Assert.AreEqual ("table.@0.year", navigator.CurrentDataPath);
			Assert.AreEqual (4, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Value, navigator.CurrentDataItem.ItemType);
			Assert.AreEqual (1972, navigator.CurrentDataItem.ObjectValue);

			//	No more field in the vector after 1st_name, 2nd_name and year
			Assert.IsFalse (navigator.NavigateToNext ());

			
			//	Move up to parent (row in table) and then to the next row
			Assert.IsTrue (navigator.NavigateToParent ());
			Assert.IsTrue (navigator.NavigateToNext ());

			Assert.AreEqual ("table.@1", navigator.CurrentDataPath);
			Assert.AreEqual (3, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Vector, navigator.CurrentDataItem.ItemType);

			//	No more rows
			Assert.IsFalse (navigator.NavigateToNext ());

			navigator.NavigateTo ("table");

			Assert.AreEqual ("table", navigator.CurrentDataPath);
			Assert.AreEqual (2, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Table, navigator.CurrentDataItem.ItemType);
			
			//	Enter table, then enter row, then fail entering value
			Assert.IsTrue (navigator.NavigateToFirstChild ());
			Assert.AreEqual ("table.@0", navigator.CurrentDataPath);
			Assert.IsTrue (navigator.NavigateToFirstChild ());
			Assert.AreEqual ("table.@0.1st_name", navigator.CurrentDataPath);
			Assert.IsFalse (navigator.NavigateToFirstChild ());
		}

		[Test]
		public void Check04SimpleTableWithSettings()
		{
			DataNavigator navigator = NavigatorTest.CreateSimpleTableNavigator ();
			DataViewContext context = navigator.Context;

			navigator.EnableSyntheticNodes = true;

			Settings.VectorSetting tableVectorSetting = new Settings.VectorSetting ()
			{
				new Settings.VectorValueSetting () { Id = "1st_name", Title = "Prénom" },
				new Settings.VectorValueSetting () { Id = "2nd_name", Title = "Nom" }
			};

			Settings.CollectionSetting tableCollectionSetting = new Settings.CollectionSetting () { Title = "Employés" };

			context.DefineVectorSetting ("table", tableVectorSetting);
			context.DefineCollectionSetting ("table", tableCollectionSetting);

			navigator.NavigateTo ("table");

			Assert.AreEqual ("table", navigator.CurrentDataPath);
			Assert.AreEqual (DataItemType.Table, navigator.CurrentDataItem.ItemType);

			Assert.IsTrue (navigator.NavigateToFirstChild ());
			Assert.AreEqual ("table.%Head1", navigator.CurrentDataPath);
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.AreEqual ("table.%Head2", navigator.CurrentDataPath);
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.AreEqual ("table.@0", navigator.CurrentDataPath);

			Assert.IsTrue (navigator.NavigateToFirstChild ());

			Assert.AreEqual ("table.@0.1st_name", navigator.CurrentDataPath);
		}


		private static DataNavigator CreateSimpleTableNavigator()
		{
			GenericEntity root = new GenericEntity (Druid.Empty);
			GenericEntity row0 = new GenericEntity (Druid.Empty);
			GenericEntity row1 = new GenericEntity (Druid.Empty);

			IList<GenericEntity> tableRows = root.GetFieldCollection<GenericEntity> ("table");

			row0.SetField<string> ("1st_name", "Pierre");
			row0.SetField<string> ("2nd_name", "Arnaud");
			row0.SetField<int> ("year", 1972);

			row1.SetField<string> ("1st_name", "Daniel");
			row1.SetField<string> ("2nd_name", "Roux");
			row1.SetField<int> ("year", 1958);

			tableRows.Add (row0);
			tableRows.Add (row1);

			DataViewContext context = new DataViewContext ();
			DataView        view    = DataView.CreateRoot (context, root);

			DataNavigator navigator = new DataNavigator (view)
			{
				EnableSyntheticNodes = false
			};

			return navigator;
		}
	}
}
