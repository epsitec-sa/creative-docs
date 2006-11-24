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
		public void CheckGroups()
		{
			ItemPanel panel = new ItemPanel ();

			panel.Items = ItemPanelTest.GetGroupedItems ();
			panel.Layout = ItemPanelLayout.VerticalList;
			panel.ItemSelection = ItemPanelSelectionMode.ExactlyOne;
			panel.GroupSelection = ItemPanelSelectionMode.ExactlyOne;
			panel.Aperture = new Drawing.Rectangle (0, 0, 80, 60);

			Assert.AreEqual (2, panel.GetItemViewCount ());
			Assert.AreEqual (new Drawing.Size (80, 20*2), panel.PreferredSize);
			Assert.IsNotNull (panel.GetItemView (0).Widget);
			Assert.IsNotNull (panel.GetItemView (1).Widget);
			Assert.AreEqual (typeof (ItemPanelGroup), panel.GetItemView (0).Widget.GetType ());
			Assert.AreEqual (typeof (ItemPanelGroup), panel.GetItemView (1).Widget.GetType ());
			Assert.AreEqual (3, (panel.GetItemView (0).Item as CollectionViewGroup).ItemCount);
			Assert.AreEqual (3, (panel.GetItemView (1).Item as CollectionViewGroup).ItemCount);

			//	Expanded subpanel
			
			panel.ExpandItemView (panel.GetItemView (0), true);

			Assert.IsTrue (panel.GetItemView (0).IsExpanded);
			Assert.AreEqual (new Drawing.Size (80, 20+20*3), panel.GetItemView (0).Size);
			Assert.AreEqual (3, (panel.GetItemView (0).Widget as ItemPanelGroup).ChildPanel.GetItemViewCount ());
			
			Assert.AreEqual (new Drawing.Size (80, 20*2), panel.PreferredSize);
			Widgets.Application.ExecuteAsyncCallbacks ();
			Assert.AreEqual (new Drawing.Size (80, 20+20*3+20), panel.PreferredSize);
			Assert.AreEqual (new Drawing.Rectangle (0, 20, 80, 20+20*3), panel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 0, 80, 20), panel.GetItemView (1).Bounds);

			//	Compact subpanel
			
			panel.ExpandItemView (panel.GetItemView (0), false);

			Assert.IsFalse (panel.GetItemView (0).IsExpanded);
			Assert.AreEqual (new Drawing.Size (80, 20), panel.GetItemView (0).Size);
			Assert.AreEqual (0, (panel.GetItemView (0).Widget as ItemPanelGroup).ChildPanel.GetItemViewCount ());

			Assert.AreEqual (new Drawing.Size (80, 20+20*3+20), panel.PreferredSize);
			Widgets.Application.ExecuteAsyncCallbacks ();
			Assert.AreEqual (new Drawing.Size (80, 20*2), panel.PreferredSize);

			//	Expanded subpanel

			panel.ExpandItemView (panel.GetItemView (0), true);
			panel.Show (panel.GetItemView (0));

			Assert.AreEqual (new Drawing.Rectangle (0, 20, 80, 20+20*3), panel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 40, 80, 60), panel.Aperture);
		}

		[Test]
		public void CheckSelection()
		{
			ItemPanel panel = new ItemPanel ();

			panel.Items = ItemPanelTest.GetStringItems ();
			panel.Layout = ItemPanelLayout.VerticalList;

			panel.ItemSelection = ItemPanelSelectionMode.None;

			panel.SelectItemView (panel.GetItemView (0));
			Assert.AreEqual (0, panel.GetSelectedItemViews ().Count);
			Assert.IsFalse (panel.GetItemView (0).IsSelected);

			panel.ItemSelection = ItemPanelSelectionMode.ZeroOrOne;
			
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

			panel.ItemSelection = ItemPanelSelectionMode.ExactlyOne;

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

			panel.ItemSelection = ItemPanelSelectionMode.Multiple;

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
			panel.Layout = ItemPanelLayout.VerticalList;

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

			Assert.AreEqual (new Drawing.Rectangle (0, 10, 80, 20), panel.Aperture);
			
			List<string> items = panel.Items.SourceCollection as List<string>;
			items.Add ("--any--");

			panel.Items.Refresh ();
			
			Assert.AreEqual (8, panel.GetItemViewCount ());
			Assert.AreEqual (new Drawing.Size (80, 20*8), panel.PreferredSize);
			
			Assert.AreEqual (new Drawing.Rectangle (0, 20*7, 80, 20), panel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 20*1, 80, 20), panel.GetItemView (6).Bounds);

			Assert.AreEqual (new Drawing.Rectangle (0, 30, 80, 20), panel.Aperture);
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

		private static CollectionView GetGroupedItems()
		{
			List<Record> source = new List<Record> ();
			CollectionView view = new CollectionView (source);

			ItemPanelTest.AddRecords (source);

			view.SortDescriptions.Add (new SortDescription ("Article"));
			view.SortDescriptions.Add (new SortDescription ("Price"));
			view.GroupDescriptions.Add (new PropertyGroupDescription ("Category"));

			return view;
		}

		private static void AddRecords(IList<Record> source)
		{
			source.Add (new Record ("Vis M3", 10, 0.15M, Category.Part));
			source.Add (new Record ("Ecrou M3", 19, 0.10M, Category.Part));
			source.Add (new Record ("Rondelle", 41, 0.05M, Category.Part));
			source.Add (new Record ("Clé M3", 7, 15.00M, Category.Tool));
			source.Add (new Record ("Tournevis", 2, 8.45M, Category.Tool));
			source.Add (new Record ("Tournevis", 7, 25.70M, Category.Tool));
		}

		private enum Category
		{
			Unknown,
			Part,
			Tool,
			ElectronicEquipment
		}

		private class Record
		{
			public Record(string article, int stock, decimal price, Category category)
			{
				this.article = article;
				this.stock = stock;
				this.price = price;
				this.category = category;
			}

			public string Article
			{
				get
				{
					return this.article;
				}
				set
				{
					this.article = value;
				}
			}

			public int Stock
			{
				get
				{
					return this.stock;
				}
				set
				{
					this.stock = value;
				}
			}

			public decimal Price
			{
				get
				{
					return this.price;
				}
				set
				{
					this.price = value;
				}
			}

			public Category Category
			{
				get
				{
					return this.category;
				}
				set
				{
					this.category = value;
				}
			}

			private string article;
			private int stock;
			private decimal price;
			private Category category;
		}

		private Support.ResourceManager manager;
	}
}
