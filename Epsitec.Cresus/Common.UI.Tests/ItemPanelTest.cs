//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	[TestFixture]
	public class ItemPanelTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			Epsitec.Common.UI.ItemViewFactories.Factory.Setup ();

			this.manager = new Support.ResourceManager ();
		}

		[Test]
		public void CheckSelection()
		{
			ItemPanel panel = new ItemPanel ();

			panel.Items = ItemPanelTest.GetStringItems ();
			panel.Layout = ItemViewLayout.VerticalList;

			panel.SelectionMode = ItemViewSelectionMode.None;

			panel.SelectItemView (panel.GetItemView (0));
			Assert.AreEqual (0, panel.GetSelectedItemViews ().Count);
			Assert.IsFalse (panel.GetItemView (0).IsSelected);

			panel.SelectionMode = ItemViewSelectionMode.ZeroOrOne;
			
			panel.SelectItemView (panel.GetItemView (0));
			Assert.AreEqual (1, panel.GetSelectedItemViews ().Count);
			Assert.IsTrue (panel.GetItemView (0).IsSelected);
			Assert.IsFalse (panel.GetItemView (1).IsSelected);

			panel.SelectItemView (panel.GetItemView (1));
			Assert.AreEqual (1, panel.GetSelectedItemViews ().Count);
			Assert.IsFalse (panel.GetItemView (0).IsSelected);
			Assert.IsTrue (panel.GetItemView (1).IsSelected);

			panel.DeselectItemView (panel.GetItemView (1));
			Assert.AreEqual (0, panel.GetSelectedItemViews ().Count);
			Assert.IsFalse (panel.GetItemView (0).IsSelected);
			Assert.IsFalse (panel.GetItemView (1).IsSelected);

			panel.SelectionMode = ItemViewSelectionMode.ExactlyOne;

			panel.SelectItemView (panel.GetItemView (0));
			Assert.AreEqual (1, panel.GetSelectedItemViews ().Count);
			Assert.IsTrue (panel.GetItemView (0).IsSelected);
			Assert.IsFalse (panel.GetItemView (1).IsSelected);

			panel.SelectItemView (panel.GetItemView (1));
			Assert.AreEqual (1, panel.GetSelectedItemViews ().Count);
			Assert.IsFalse (panel.GetItemView (0).IsSelected);
			Assert.IsTrue (panel.GetItemView (1).IsSelected);

			panel.DeselectItemView (panel.GetItemView (1));
			Assert.AreEqual (1, panel.GetSelectedItemViews ().Count);
			Assert.IsFalse (panel.GetItemView (0).IsSelected);
			Assert.IsTrue (panel.GetItemView (1).IsSelected);

			panel.SelectionMode = ItemViewSelectionMode.Multiple;

			panel.SelectItemView (panel.GetItemView (0));
			Assert.AreEqual (2, panel.GetSelectedItemViews ().Count);
			Assert.IsTrue (panel.GetItemView (0).IsSelected);
			Assert.IsTrue (panel.GetItemView (1).IsSelected);

			panel.SelectItemView (panel.GetItemView (1));
			Assert.AreEqual (2, panel.GetSelectedItemViews ().Count);
			Assert.IsTrue (panel.GetItemView (0).IsSelected);
			Assert.IsTrue (panel.GetItemView (1).IsSelected);

			panel.DeselectItemView (panel.GetItemView (0));
			Assert.AreEqual (1, panel.GetSelectedItemViews ().Count);
			Assert.IsFalse (panel.GetItemView (0).IsSelected);
			Assert.IsTrue (panel.GetItemView (1).IsSelected);

			panel.DeselectItemView (panel.GetItemView (1));
			Assert.AreEqual (0, panel.GetSelectedItemViews ().Count);
			Assert.IsFalse (panel.GetItemView (0).IsSelected);
			Assert.IsFalse (panel.GetItemView (1).IsSelected);
		}

		[Test]
		public void CheckVerticalLayout()
		{
			ItemPanel panel = new ItemPanel ();

			panel.Items = ItemPanelTest.GetStringItems ();
			panel.Layout = ItemViewLayout.VerticalList;

			Assert.AreEqual (7, panel.GetItemViewCount ());
			Assert.AreEqual ("Monday", panel.GetItemView (0).Item);
			Assert.AreEqual ("Sunday", panel.GetItemView (6).Item);
			Assert.AreEqual (2, panel.GetItemView (2).Index);

			Assert.AreEqual (new Drawing.Size (80, 20*7), panel.PreferredSize);

			Assert.AreEqual (new Drawing.Rectangle (0, 20*6, 80, 20), panel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 0, 80, 20), panel.GetItemView (6).Bounds);

			Assert.AreEqual ("Sunday", panel.Detect (new Drawing.Point (40, 10)).Item);
			Assert.AreEqual ("Monday", panel.Detect (new Drawing.Point (40, 20*6+10)).Item);
			Assert.AreEqual ("Sunday", panel.Detect (new Drawing.Point (40, 10)).Item);
			Assert.AreEqual ("Saturday", panel.Detect (new Drawing.Point (40, 20*1+10)).Item);

			System.GC.Collect ();

			Assert.IsNull (panel.GetItemView (0).Widget);

			panel.Aperture = new Drawing.Rectangle (0, 100, 80, 40);

			Assert.IsNotNull (panel.GetItemView (0).Widget);
			Assert.IsNotNull (panel.GetItemView (1).Widget);
			Assert.IsNull (panel.GetItemView (2).Widget);

			Assert.AreEqual ("Monday", panel.GetItemView (0).Widget.Text);
			Assert.AreEqual ("Tuesday", panel.GetItemView (1).Widget.Text);

			panel.Aperture = new Drawing.Rectangle (0, 10, 80, 20);

			Assert.IsNull (panel.GetItemView (0).Widget);
			Assert.IsNull (panel.GetItemView (1).Widget);
			Assert.IsNotNull (panel.GetItemView (5).Widget);
			Assert.IsNotNull (panel.GetItemView (6).Widget);

			Assert.AreEqual (panel, panel.GetItemView (5).Widget.Parent);
			Assert.AreEqual (panel, panel.GetItemView (6).Widget.Parent);
			Assert.AreEqual (new Drawing.Rectangle (0, 20, 80, 20), panel.GetItemView (5).Widget.ActualBounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 0, 80, 20), panel.GetItemView (6).Widget.ActualBounds);
		}

		private static CollectionView GetStringItems()
		{
			List<string> items = new List<string> ();
			
			items.Add ("Monday");
			items.Add ("Tuesday");
			items.Add ("Wednesday");
			items.Add ("Thursday");
			items.Add ("Friday");
			items.Add ("Saturday");
			items.Add ("Sunday");

			CollectionView view = new CollectionView (items);
			
			return view;
		}

		private Support.ResourceManager manager;
	}
}
