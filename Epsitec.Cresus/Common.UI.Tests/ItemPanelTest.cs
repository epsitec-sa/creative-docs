//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckGroups()
		{
			ItemTable table = new ItemTable ();
			ItemPanel panel = table.ItemPanel;

			table.SourceType = ItemPanelTest.GetStructuredType ();

			table.Columns.Add ("Stock", 20);
			table.Columns.Add ("Article", 100);
			table.Columns.Add ("Category", 30);
			table.Columns.Add ("Price", 50);

			panel.Items = ItemPanelTest.GetGroupedItems ();
			panel.Layout = ItemPanelLayout.VerticalList;
			panel.ItemSelectionMode = ItemPanelSelectionMode.ExactlyOne;
			panel.GroupSelectionMode = ItemPanelSelectionMode.ExactlyOne;
			panel.ItemViewDefaultSize = new Drawing.Size (200, 20);

			table.SetManualBounds (new Drawing.Rectangle (0, 0, 100, 80));

			Assert.AreEqual (new Drawing.Size (100-17-2*1, 80-26-17-2*1), panel.Aperture.Size);
			
			Widgets.Application.ExecuteAsyncCallbacks ();

			Assert.AreEqual (2, panel.GetItemViewCount ());
			Assert.AreEqual (new Drawing.Size (200, 20*2), panel.GetContentsSize ());
			Assert.IsNotNull (panel.GetItemView (0).Widget);
			Assert.IsNotNull (panel.GetItemView (1).Widget);
			Assert.AreEqual (typeof (ItemPanelGroup), panel.GetItemView (0).Widget.GetType ());
			Assert.AreEqual (typeof (ItemPanelGroup), panel.GetItemView (1).Widget.GetType ());
			Assert.AreEqual (3, (panel.GetItemView (0).Item as CollectionViewGroup).ItemCount);
			Assert.AreEqual (3, (panel.GetItemView (1).Item as CollectionViewGroup).ItemCount);

			//	Expanded subpanel
			
			panel.ExpandItemView (panel.GetItemView (0), true);

			Assert.IsTrue (panel.GetItemView (0).IsExpanded);
			Assert.AreEqual (new Drawing.Size (200, 20+20*3), panel.GetItemView (0).Size);
			Assert.AreEqual (3, (panel.GetItemView (0).Group).ChildPanel.GetItemViewCount ());

			Assert.AreEqual (new Drawing.Size (200, 20*2), panel.GetContentsSize ());
			Widgets.Application.ExecuteAsyncCallbacks ();
			Assert.AreEqual (new Drawing.Size (200, 20+20*3+20), panel.GetContentsSize ());
			Assert.AreEqual (new Drawing.Rectangle (0, 20, 200, 20+20*3), panel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 0, 200, 20), panel.GetItemView (1).Bounds);

			Assert.AreEqual (new Drawing.Rectangle (0, 40, 200, 20), panel.GetItemView (0).Group.ChildPanel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 20, 200, 20), panel.GetItemView (0).Group.ChildPanel.GetItemView (1).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0,  0, 200, 20), panel.GetItemView (0).Group.ChildPanel.GetItemView (2).Bounds);
			
			Assert.AreEqual (new Drawing.Rectangle (0, 60-0*20, 200, 20), panel.GetItemViewBounds (panel.GetItemView (0).Group.ChildPanel.GetItemView (0)));
			Assert.AreEqual (new Drawing.Rectangle (0, 60-1*20, 200, 20), panel.GetItemViewBounds (panel.GetItemView (0).Group.ChildPanel.GetItemView (1)));
			Assert.AreEqual (new Drawing.Rectangle (0, 60-2*20, 200, 20), panel.GetItemViewBounds (panel.GetItemView (0).Group.ChildPanel.GetItemView (2)));

			//	Compact subpanel
			
			panel.ExpandItemView (panel.GetItemView (0), false);

			Assert.IsFalse (panel.GetItemView (0).IsExpanded);
			Assert.AreEqual (new Drawing.Size (200, 20), panel.GetItemView (0).Size);
			Assert.AreEqual (3, (panel.GetItemView (0).Group.ChildPanel.GetItemViewCount ()));

			Assert.AreEqual (new Drawing.Size (200, 20+20*3+20), panel.GetContentsSize ());
			Widgets.Application.ExecuteAsyncCallbacks ();
			Assert.AreEqual (new Drawing.Size (200, 20*2), panel.GetContentsSize ());

			//	Expanded subpanel

			panel.ExpandItemView (panel.GetItemView (0), true);
			panel.Show (panel.GetItemView (0));

			Assert.AreEqual (new Drawing.Rectangle (0, 20, 200, 20+20*3), panel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 0, 200, 20), panel.GetItemView (1).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 65, 81, 35), panel.Aperture);
			Assert.AreEqual (new Drawing.Size (200, 20+80), panel.GetContentsSize ());
			
			panel.ExpandItemView (panel.GetItemView (1), true);
			Widgets.Application.ExecuteAsyncCallbacks ();

			Assert.AreEqual (new Drawing.Rectangle (0, 0, 200, 20+20*3), panel.GetItemView (1).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 20+20*3, 200, 20+20*3), panel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 65+20*3, 81, 35), panel.Aperture);
			Assert.AreEqual (new Drawing.Size (200, 80+80), panel.GetContentsSize ());
			
			panel.Show (panel.GetItemView (1));
			
			Assert.AreEqual (new Drawing.Rectangle (0, 45, 81, 35), panel.Aperture);

			ItemPanelGroup group = panel.GetItemView (1).Group;
			
			Assert.IsNotNull (group.ChildPanel);
			Assert.AreEqual (panel, group.Parent);
			Assert.AreEqual (panel, group.ParentPanel);
			Assert.AreEqual (panel.GetItemView (1), group.ItemView);
			Assert.AreEqual ("Part: 19 x Ecrou M3 @ 0.10", group.ChildPanel.GetItemView (0).Widget.Children.Widgets[0].Text);
		}

		[Test]
		public void CheckGroupsStructured()
		{
			ItemTable table = new ItemTable ();
			ItemPanel panel = table.ItemPanel;

			table.SourceType = ItemPanelTest.GetStructuredType ();

			table.Columns.Add ("Stock", 40);
			table.Columns.Add ("Article", 200);
			table.Columns.Add ("Price", 60);

			panel.Items = ItemPanelTest.GetStructuredItems ();
			panel.Layout = ItemPanelLayout.VerticalList;
			panel.ItemSelectionMode = ItemPanelSelectionMode.ExactlyOne;
			panel.GroupSelectionMode = ItemPanelSelectionMode.ExactlyOne;
			panel.ItemViewDefaultSize = new Drawing.Size (300, 20);

			panel.Items.GroupDescriptions.Add (new PropertyGroupDescription ("Category"));

			table.SetManualBounds (new Drawing.Rectangle (0, 0, 99, 105));

			Assert.AreEqual (new Drawing.Size (99-17-2*1, 105-26-17-2*1), panel.Aperture.Size);
			
			Widgets.Application.ExecuteAsyncCallbacks ();
			
			Assert.AreEqual (2, panel.GetItemViewCount ());
			Assert.AreEqual (new Drawing.Size (300, 20*2), panel.GetContentsSize ());
			Assert.IsNotNull (panel.GetItemView (0).Widget);
			Assert.IsNotNull (panel.GetItemView (1).Widget);
			Assert.AreEqual (typeof (ItemPanelGroup), panel.GetItemView (0).Widget.GetType ());
			Assert.AreEqual (typeof (ItemPanelGroup), panel.GetItemView (1).Widget.GetType ());
			Assert.AreEqual (3, (panel.GetItemView (0).Item as CollectionViewGroup).ItemCount);
			Assert.AreEqual (3, (panel.GetItemView (1).Item as CollectionViewGroup).ItemCount);

			panel.ExpandItemView (panel.GetItemView (0), true);
			Widgets.Application.ExecuteAsyncCallbacks ();
			panel.Show (panel.GetItemView (0));
			
			ItemPanelGroup group = panel.GetItemView (0).Group;
			ItemPanelColumnHeader header = table.ColumnHeader;

			Assert.IsNotNull (group.ChildPanel);
			Assert.AreEqual (panel, group.Parent);
			Assert.AreEqual (panel, group.ParentPanel);
			Assert.AreEqual (panel.GetItemView (0), group.ItemView);

			Assert.AreEqual (3, header.ColumnCount);
			Assert.AreEqual (panel, header.ItemPanel);
			
			panel.Refresh ();

			Assert.AreEqual (new Drawing.Rectangle (0, 40, 80, 60), panel.Aperture);
			Assert.AreEqual (new Drawing.Rectangle (0, 20, 300, 80), group.ItemView.Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 40, 300, 20), group.ChildPanel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 20, 300, 20), group.ChildPanel.GetItemView (1).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0,  0, 300, 20), group.ChildPanel.GetItemView (2).Bounds);

			Assert.AreEqual (3, group.ChildPanel.GetItemView (0).Widget.Children.Count);
			Assert.AreEqual (3, group.ChildPanel.GetItemView (1).Widget.Children.Count);
			Assert.IsNull (group.ChildPanel.GetItemView (2).Widget);
			
			Assert.AreEqual ("41", ((Widgets.Widget)group.ChildPanel.GetItemView (1).Widget.Children[0]).Text);
			Assert.AreEqual ("Rondelle", ((Widgets.Widget) group.ChildPanel.GetItemView (1).Widget.Children[1]).Text);
			Assert.AreEqual ("0.05", ((Widgets.Widget) group.ChildPanel.GetItemView (1).Widget.Children[2]).Text);
		}

		[Test]
		public void CheckInteractiveTable()
		{
			Widgets.Window window = new Widgets.Window ();

			double dx = 400;
			double dy = 420;

			window.Text = "CheckInteractiveTable";
			window.ClientSize = new Drawing.Size (dx, dy);
			window.Root.Padding = new Drawing.Margins (4, 4, 4, 4);

			dx -= 8;
			dy -= 8;

			ItemTable table = new ItemTable (window.Root);
			table.Dock = Widgets.DockStyle.Fill;

			table.SourceType = ItemPanelTest.GetStructuredType ();

			table.Columns.Add ("Stock", 40);
			table.Columns.Add ("Article", 200);
			table.Columns.Add ("Price", 60);
			
			ItemPanel panel = table.ItemPanel;

			table.ColumnHeader.SetColumnText (0, "Quantité");
			table.ColumnHeader.SetColumnText (1, "Description");
			table.ColumnHeader.SetColumnText (2, "Prix unitaire");

			panel.Items = ItemPanelTest.GetStructuredItems ();
			panel.Layout = ItemPanelLayout.VerticalList;
			panel.ItemSelectionMode = ItemPanelSelectionMode.ExactlyOne;
			panel.GroupSelectionMode = ItemPanelSelectionMode.ExactlyOne;
			panel.Aperture = new Drawing.Rectangle (0, 0, dx, dy);
			panel.ItemViewDefaultSize = new Drawing.Size (dx, 20);
			panel.CurrentItemTrackingMode = CurrentItemTrackingMode.AutoSelect;

			ItemPanelTest.CreateTableButtons (window, table, panel);
			
			panel.Show (panel.GetItemView (0));
			
			window.Show ();
			panel.Focus ();

			Widgets.Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckInteractiveTableWithGroups_1_Level()
		{
			Widgets.Window window = new Widgets.Window ();

			double dx = 400;
			double dy = 420;

			window.Text = "CheckInteractiveTableWithGroups_1_Level";
			window.ClientSize = new Drawing.Size (dx, dy);
			window.Root.Padding = new Drawing.Margins (4, 4, 4, 4);

			dx -= 8;
			dy -= 8;

			ItemTable table = new ItemTable (window.Root);
			table.Dock = Widgets.DockStyle.Fill;

			table.SourceType = ItemPanelTest.GetStructuredType ();
			
			table.Columns.Add ("Stock", 40);
			table.Columns.Add ("Article", 200);
			table.Columns.Add ("Price", 60);
			
			ItemPanel panel = table.ItemPanel;

			panel.Items = ItemPanelTest.GetStructuredItems ();
			panel.Layout = ItemPanelLayout.VerticalList;
			panel.ItemSelectionMode = ItemPanelSelectionMode.ZeroOrOne;
			panel.GroupSelectionMode = ItemPanelSelectionMode.None;
			panel.ItemViewDefaultSize = new Drawing.Size (300, 20);
			panel.ItemViewDefaultExpanded = true;
			panel.CurrentItemTrackingMode = CurrentItemTrackingMode.AutoSelect;

			panel.Items.GroupDescriptions.Add (new PropertyGroupDescription ("Category"));

			ItemPanelTest.CreateTableButtons (window, table, panel);
			
			panel.Show (panel.GetItemView (0));

			window.Show ();
			panel.Focus ();

			Widgets.Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckInteractiveTableWithGroups_2_Levels()
		{
			Widgets.Window window = new Widgets.Window ();

			double dx = 400;
			double dy = 420;

			window.Text = "CheckInteractiveTableWithGroups_2_Levels";
			window.ClientSize = new Drawing.Size (dx, dy);
			window.Root.Padding = new Drawing.Margins (4, 4, 4, 4);

			dx -= 8;
			dy -= 8;

			ItemTable table = new ItemTable (window.Root);
			table.Dock = Widgets.DockStyle.Fill;

			table.SourceType = ItemPanelTest.GetStructuredType ();

			table.Columns.Add ("Stock", 40);
			table.Columns.Add ("Article", 200);
			table.Columns.Add ("Price", 60);

			ItemPanel panel = table.ItemPanel;

			panel.Items = ItemPanelTest.GetStructuredItems ();
			panel.Layout = ItemPanelLayout.VerticalList;
			panel.ItemSelectionMode = ItemPanelSelectionMode.ZeroOrOne;
			panel.GroupSelectionMode = ItemPanelSelectionMode.None;
			panel.ItemViewDefaultSize = new Drawing.Size (320, 20);
			panel.ItemViewDefaultExpanded = false;
			panel.CurrentItemTrackingMode = CurrentItemTrackingMode.AutoSelect;

			panel.Items.GroupDescriptions.Add (new PropertyGroupDescription ("Category"));
			panel.Items.GroupDescriptions.Add (new PropertyGroupDescription ("Article"));

			ItemPanelTest.CreateTableButtons (window, table, panel);

			panel.Show (panel.GetItemView (0));

			window.Show ();
			panel.Focus ();

			Widgets.Window.RunInTestEnvironment (window);
		}

		private static System.Random random = new System.Random (0);

		private static StructuredData CreateRandomRecord()
		{
			Category   cat = (Category) (ItemPanelTest.random.Next (3)+1);
			decimal  price = (decimal) (ItemPanelTest.random.Next (100)+1) * 0.05M;
			int   quantity = ItemPanelTest.random.Next (50);
			string article = new string[] { "Foo", "Bar", "Grok", "Glup", "Munch", "Zap" }[ItemPanelTest.random.Next (6)];

			return ItemPanelTest.NewStructuredData (article, quantity, price, cat);
		}

		private static void CreateTableButtons(Widgets.Window window, ItemTable table, ItemPanel panel)
		{
			table.TabIndex = 1;
			table.TabNavigationMode = Epsitec.Common.Widgets.TabNavigationMode.ActivateOnTab;

			Widgets.FrameBox box;

			box = new Widgets.FrameBox (window.Root);

			box.Dock = Widgets.DockStyle.Bottom;
			box.PreferredHeight = 36;
			box.Padding = new Drawing.Margins (4, 4, 8, 8);
			box.ContainerLayoutMode = Widgets.ContainerLayoutMode.HorizontalFlow;

			Widgets.Button button0 = ItemPanelTest.CreateButton (box, "0");
			Widgets.Button button1 = ItemPanelTest.CreateButton (box, "1");
			Widgets.Button buttonN = ItemPanelTest.CreateButton (box, "n");

			Widgets.Button buttonSA = ItemPanelTest.CreateButton (box, "A");
			Widgets.Button buttonSM = ItemPanelTest.CreateButton (box, "M");
			Widgets.Button buttonSO = ItemPanelTest.CreateButton (box, "1");

			Widgets.Button buttonA = ItemPanelTest.CreateButton (box, "Item");
			Widgets.Button buttonB = ItemPanelTest.CreateButton (box, "Linear");

			Widgets.TextField text = new Widgets.TextField (box);

			text.Margins = new Drawing.Margins (16, 0, 0, 0);
			text.Dock = Widgets.DockStyle.Stacked;
			text.PreferredWidth = 40;
			text.VerticalAlignment = Widgets.VerticalAlignment.Center;

			text.TabIndex = 2;
			text.TabNavigationMode = Epsitec.Common.Widgets.TabNavigationMode.ActivateOnTab;

			button0.Clicked += delegate
			{
				panel.ItemSelectionMode = ItemPanelSelectionMode.None;
			};
			
			button1.Clicked += delegate
			{
				panel.ItemSelectionMode = ItemPanelSelectionMode.ExactlyOne;
			};
			
			buttonN.Clicked += delegate
			{
				panel.ItemSelectionMode = ItemPanelSelectionMode.Multiple;
			};

			buttonSA.Margins = new Drawing.Margins (8, 0, 0, 0);

			buttonSA.Clicked += delegate
			{
				panel.SelectionBehavior = ItemPanelSelectionBehavior.Automatic;
			};
			
			buttonSM.Clicked += delegate
			{
				panel.SelectionBehavior = ItemPanelSelectionBehavior.Manual;
			};
			
			buttonSO.Clicked += delegate
			{
				panel.SelectionBehavior = ItemPanelSelectionBehavior.ManualOne;
			};

			buttonA.Margins = new Drawing.Margins (8, 0, 0, 0);
			buttonA.PreferredWidth = 40;
			buttonB.PreferredWidth = 40;

			buttonA.Clicked += delegate
			{
				table.VerticalScrollMode = ItemTableScrollMode.ItemBased;
			};
			
			buttonB.Clicked += delegate
			{
				table.VerticalScrollMode = ItemTableScrollMode.Linear;
			};

			IList<StructuredData> source = panel.Items.SourceCollection as IList<StructuredData>;

			box = new Widgets.FrameBox (window.Root);

			box.Dock = Widgets.DockStyle.Bottom;
			box.PreferredHeight = 36;
			box.Padding = new Drawing.Margins (4, 4, 8, 8);
			box.ContainerLayoutMode = Widgets.ContainerLayoutMode.HorizontalFlow;

			Widgets.Button buttonClear  = ItemPanelTest.CreateButton (box, "*");
			Widgets.Button buttonTop    = ItemPanelTest.CreateButton (box, "^");
			Widgets.Button buttonBottom = ItemPanelTest.CreateButton (box, "v");
			Widgets.Button buttonPrev   = ItemPanelTest.CreateButton (box, "&lt;");
			Widgets.Button buttonNext   = ItemPanelTest.CreateButton (box, "&gt;");
			Widgets.Button buttonCreate = ItemPanelTest.CreateButton (box, "+");
			Widgets.Button buttonRefresh= ItemPanelTest.CreateButton (box, "R");

			buttonClear.Clicked += delegate
			{
				panel.Items.MoveCurrentToPosition (-1);
			};
			
			buttonTop.Clicked += delegate
			{
				panel.Items.MoveCurrentToFirst ();
			};
			
			buttonBottom.Clicked += delegate
			{
				panel.Items.MoveCurrentToLast ();
			};
			
			buttonPrev.Clicked += delegate
			{
				panel.Items.MoveCurrentToPrevious ();
			};
			
			buttonNext.Clicked += delegate
			{
				panel.Items.MoveCurrentToNext ();
			};
			
			buttonCreate.Clicked += delegate
			{
				source.Add (ItemPanelTest.CreateRandomRecord ());
			};

			buttonRefresh.Clicked += delegate
			{
				panel.Items.Refresh ();
			};

			buttonCreate.Margins = new Drawing.Margins (8, 0, 0, 0);
		}

		private static Widgets.Button CreateButton(Epsitec.Common.Widgets.FrameBox box, string text)
		{
			Widgets.Button button = new Widgets.Button (text);
			
			button.Dock = Widgets.DockStyle.Stacked;
			button.PreferredWidth = 20;
			button.AutoFocus = false;
			
			box.Children.Add (button);

			return button;
		}

		[Test]
		public void CheckSelection()
		{
			ItemPanel panel = new ItemPanel ();

			panel.Items = ItemPanelTest.GetStringItems ();
			panel.Layout = ItemPanelLayout.VerticalList;
			panel.ItemSelectionMode = ItemPanelSelectionMode.None;

			Widgets.Application.ExecuteAsyncCallbacks ();

			panel.SelectItemView (panel.GetItemView (0));
			Assert.AreEqual (0, panel.GetSelectedItemViews ().Count);
			Assert.IsFalse (panel.GetItemView (0).IsSelected);

			panel.ItemSelectionMode = ItemPanelSelectionMode.ZeroOrOne;
			
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

			panel.ItemSelectionMode = ItemPanelSelectionMode.ExactlyOne;

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

			panel.ItemSelectionMode = ItemPanelSelectionMode.Multiple;

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

			Widgets.Application.ExecuteAsyncCallbacks ();

			Assert.AreEqual (7, panel.GetItemViewCount ());
			Assert.AreEqual ("Monday", panel.GetItemView (0).Item);
			Assert.AreEqual ("Sunday", panel.GetItemView (6).Item);
			Assert.AreEqual (2, panel.GetItemView (2).Index);

			Assert.AreEqual (new Drawing.Size (80, 20*7), panel.GetContentsSize ());

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

			Assert.AreEqual ("Monday", panel.GetItemView (0).Widget.Children.Widgets[0].Text);
			Assert.AreEqual ("Tuesday", panel.GetItemView (1).Widget.Children.Widgets[0].Text);

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
			Widgets.Application.ExecuteAsyncCallbacks ();

			Assert.AreEqual (8, panel.GetItemViewCount ());
			Assert.AreEqual (new Drawing.Size (80, 20*8), panel.GetContentsSize ());
			
			Assert.AreEqual (new Drawing.Rectangle (0, 20*7, 80, 20), panel.GetItemView (0).Bounds);
			Assert.AreEqual (new Drawing.Rectangle (0, 20*1, 80, 20), panel.GetItemView (6).Bounds);

			Assert.AreEqual (panel.GetItemView (0).Bounds, panel.Aperture);
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

		private static CollectionView GetStructuredItems()
		{
			Types.Collections.ObservableList<StructuredData> source = new Types.Collections.ObservableList<StructuredData> ();
			CollectionView view = new CollectionView (source);

			ItemPanelTest.AddStructuredRecords (source);

			view.SortDescriptions.Add (new SortDescription ("Category"));
			view.SortDescriptions.Add (new SortDescription ("Article"));
			view.SortDescriptions.Add (new SortDescription ("Price"));

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

		private static void AddStructuredRecords(IList<StructuredData> source)
		{
			source.Add (ItemPanelTest.NewStructuredData ("Vis M3", 10, 0.15M, Category.Part));
			source.Add (ItemPanelTest.NewStructuredData ("Ecrou M3", 19, 0.10M, Category.Part));
			source.Add (ItemPanelTest.NewStructuredData ("Rondelle", 41, 0.05M, Category.Part));
			source.Add (ItemPanelTest.NewStructuredData ("Clé M3", 7, 15.00M, Category.Tool));
			source.Add (ItemPanelTest.NewStructuredData ("Tournevis", 2, 8.45M, Category.Tool));
			source.Add (ItemPanelTest.NewStructuredData ("Tournevis", 7, 25.70M, Category.Tool));
		}

		private static StructuredType GetStructuredType()
		{
			if (ItemPanelTest.structuredType == null)
			{
				ItemPanelTest.structuredType = new StructuredType ();

				EnumType categoryType = new EnumType (typeof (Category));

				ItemPanelTest.structuredType.Fields.Add ("Article", StringType.NativeDefault);
				ItemPanelTest.structuredType.Fields.Add ("Stock", IntegerType.Default);
				ItemPanelTest.structuredType.Fields.Add ("Price", new DecimalType (0.00M, 999999.95M, 0.05M));
				ItemPanelTest.structuredType.Fields.Add ("Category", categoryType);
			}

			return ItemPanelTest.structuredType;
		}

		private static StructuredData NewStructuredData(string article, int stock, decimal price, Category category)
		{
			StructuredData data = new StructuredData (ItemPanelTest.GetStructuredType ());
			
			data.SetValue ("Article", article);
			data.SetValue ("Stock", stock);
			data.SetValue ("Price", price);
			data.SetValue ("Category", category);

			return data;
		}

		private static StructuredType structuredType;

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

			public override string ToString()
			{
				return string.Format ("{0}: {1} x {2} @ {3}", this.category, this.stock, this.article, this.price);
			}

			private string article;
			private int stock;
			private decimal price;
			private Category category;
		}

		private Support.ResourceManager manager;
	}
}
