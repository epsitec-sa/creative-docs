﻿//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			DataView view = DataView.CreateRoot (context, root);
			DataNavigator navigator = new DataNavigator (view);

			navigator.NavigateTo ("table");

			Assert.AreEqual ("table", navigator.CurrentDataPath);
			Assert.AreEqual (2, Collection.Count (navigator.CurrentViewStack));
			Assert.AreEqual (DataItemType.Table, navigator.CurrentDataItem.ItemType);
		}
	}
}
