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
				new Settings.VectorValueSetting () { Id = "2nd_name", Title = FormattedText.FromSimpleText ("Nom") },
				new Settings.VectorValueSetting () { Id = "1st_name", Title = FormattedText.FromSimpleText ("Prénom") },
			};

			Settings.CollectionSetting tableCollectionSetting = new Settings.CollectionSetting () { Title = FormattedText.FromSimpleText ("Employés") };

			context.DefineVectorSetting ("table", tableVectorSetting);
			context.DefineCollectionSetting ("table", tableCollectionSetting);

			navigator.NavigateTo ("table");

			Assert.AreEqual ("table", navigator.CurrentDataPath);
			Assert.AreEqual (DataItemType.Table, navigator.CurrentDataItem.ItemType);

			Assert.IsTrue (navigator.NavigateToFirstChild ());
			Assert.AreEqual ("table.%Head1", navigator.CurrentDataPath);
			Assert.AreEqual (1, navigator.CurrentDataItem.Count);
			Assert.AreEqual ("Employés", NavigatorTest.Peek<string> (navigator, "@0"));

			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.AreEqual ("table.%Head2", navigator.CurrentDataPath);
			Assert.AreEqual (2, navigator.CurrentDataItem.Count);
			Assert.AreEqual ("Nom", NavigatorTest.Peek<string> (navigator, "@0"));
			Assert.AreEqual ("Prénom", NavigatorTest.Peek<string> (navigator, "@1"));
			
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.AreEqual ("table.@0", navigator.CurrentDataPath);

			//	Row 0 of table has only two columns : 2nd_name and 1st_name, because of the vector definition
			Assert.IsTrue (navigator.NavigateToFirstChild ());
			Assert.AreEqual ("table.@0.2nd_name", navigator.CurrentDataPath);
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.AreEqual ("table.@0.1st_name", navigator.CurrentDataPath);
			Assert.IsFalse (navigator.NavigateToNext ());
			Assert.IsTrue (navigator.NavigateToParent ());

			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.AreEqual ("table.@1", navigator.CurrentDataPath);
			
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.AreEqual ("table.%Foot2", navigator.CurrentDataPath);
			
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.AreEqual ("table.%Foot1", navigator.CurrentDataPath);
			Assert.AreEqual (1, navigator.CurrentDataItem.Count);
			Assert.AreEqual ("Employés", NavigatorTest.Peek<string> (navigator, "@0"));
			Assert.IsFalse (navigator.NavigateToNext ());
			
			//	Get back a few nodes
			Assert.IsTrue (navigator.NavigateToPrevious ());
			Assert.AreEqual ("table.%Foot2", navigator.CurrentDataPath);
			Assert.IsTrue (navigator.NavigateToPrevious ());
			Assert.AreEqual ("table.@1", navigator.CurrentDataPath);
			Assert.IsTrue (navigator.NavigateToPrevious ());
			Assert.AreEqual ("table.@0", navigator.CurrentDataPath);
			Assert.IsTrue (navigator.NavigateToPrevious ());
			Assert.AreEqual ("table.%Head2", navigator.CurrentDataPath);
			Assert.IsTrue (navigator.NavigateToPrevious ());
			Assert.AreEqual ("table.%Head1", navigator.CurrentDataPath);

			//	up to the start, where we cannot go any further
			Assert.IsFalse (navigator.NavigateToPrevious ());

			//	Return to the second last node
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.IsTrue (navigator.NavigateToNext ());

			//	Try navigating to the child of a virtual node
			Assert.IsTrue (navigator.NavigateToFirstChild ());
			Assert.AreEqual ("table.%Foot2.@0", navigator.CurrentDataPath);
			Assert.AreEqual ("Nom", navigator.CurrentDataItem.Value);
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.AreEqual ("table.%Foot2.@1", navigator.CurrentDataPath);
			Assert.AreEqual ("Prénom", navigator.CurrentDataItem.Value);
			Assert.IsTrue (navigator.NavigateToParent ());
			Assert.IsTrue (navigator.NavigateToNext ());
			Assert.IsTrue (navigator.NavigateToFirstChild ());
			Assert.AreEqual ("table.%Foot1.@0", navigator.CurrentDataPath);
			Assert.AreEqual ("Employés", navigator.CurrentDataItem.Value);
		}


		[Test]
		public void Check05SimpleTableWithSettingsTraverse()
		{
			DataNavigator navigator = NavigatorTest.CreateSimpleTableNavigator ();
			DataViewContext context = navigator.Context;

			navigator.EnableSyntheticNodes = true;

			Settings.VectorSetting tableVectorSetting = new Settings.VectorSetting ()
			{
				new Settings.VectorValueSetting () { Id = "2nd_name", Title = FormattedText.FromSimpleText ("Nom") },
				new Settings.VectorValueSetting () { Id = "1st_name", Title = FormattedText.FromSimpleText ("Prénom") },
			};

			Settings.CollectionSetting tableCollectionSetting = new Settings.CollectionSetting ()
			{
				Title = FormattedText.FromSimpleText ("Employés")
			};

			context.DefineVectorSetting ("table", tableVectorSetting);
			context.DefineCollectionSetting ("table", tableCollectionSetting);

			navigator.NavigateTo ("table");

			foreach (var item in navigator.Traverse ())
			{
				if (item.NodeType == DataNodeType.Value)
				{
					System.Console.Out.WriteLine ("{0} {1} {2} --> {3}", item.ItemType, item.ItemClass, item.NodeType, item.Value);
				}
				else
				{
					System.Console.Out.WriteLine ("{0} {1} {2}", item.ItemType, item.ItemClass, item.NodeType);
				}
			}
		}

		private static T Peek<T>(DataNavigator navigator, string id)
		{
			navigator.NavigateToRelative (id);
			object rawValue = navigator.CurrentDataItem.ObjectValue;
			
			if (typeof (T) == typeof (string))
			{
				rawValue = rawValue.ToString ();
			}
			
			T value = (T) rawValue;
			navigator.NavigateToParent ();
			return value;
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
