//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.BigList;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DebugViewer.Accessors;

namespace Epsitec.Cresus.DebugViewer.ViewControllers
{
	public class MainViewController
	{
		public MainViewController(CoreInteractiveApp host)
		{
			this.host = host;
		}


		public void CreateUI(Widget windowRoot)
		{
			this.container = new FrameBox ()
			{
				Parent = windowRoot,
				Name = "Container",
				Dock = DockStyle.Fill,
			};

			this.CreateUISettingsFrame (windowRoot);


			var left1 = new FrameBox ()
			{
				Parent = this.container,
				Name = "Left1",
				Dock = DockStyle.Left,
				PreferredWidth = 240,
			};

			var splitterLeft1 = new VSplitter ()
			{
				Parent = this.container,
				Name = "SplitterLeft1",
				Dock = DockStyle.Left,
			};

			var left2 = new FrameBox ()
			{
				Parent = this.container,
				Name = "Left2",
				Dock = DockStyle.Left,
				PreferredWidth = 240,
			};

			var splitterLeft2 = new VSplitter ()
			{
				Parent = this.container,
				Name = "SplitterLeft2",
				Dock = DockStyle.Left,
			};

			var right = new FrameBox ()
			{
				Parent = this.container,
				Name = "Right",
				Dock = DockStyle.Fill,
			};

			var bottom = new FrameBox ()
			{
				Parent = right,
				Name = "Bottom",
				Dock = DockStyle.Bottom,
				PreferredHeight = 80,
			};

			var splitter2 = new HSplitter ()
			{
				Parent = right,
				Name = "Splitter2",
				Dock = DockStyle.Bottom,
			};

			this.view = new FrameBox ()
			{
				Parent = right,
				Name = "View",
				Dock = DockStyle.Fill,
			};

			this.CreateUIListForFolderItems (left1);
			this.CreateUIListForHistoryItems (left2);

			this.RefreshContents ();
		}

		
		public void DefineHistoryAccessor(LogDataAccessor accessor)
		{
			this.historyAccessor = accessor;
			
			this.historyData = new ItemList<Data.LogRecord> (this.historyAccessor, this.historyAccessor, selection: null);
			this.historyList.ItemList = this.historyData;
			this.historyData.Marks.Add (new ItemListMark
			{
				Attachment = ItemListMarkAttachment.After,
				Index = 2,
				Breadth = 2
			});

			this.historyData.Reset ();
			this.historyList.Invalidate ();
		}

		public void DefineFolderAccessor(LogFolderDataAccessor accessor)
		{
			this.folderAccessor = accessor;

#if false
			this.folderData = new ItemList<Data.LogFolderRecord> (this.folderAccessor, this.folderAccessor);
			this.folderList.ItemList = this.folderData;
#else

			var renderer = new Epsitec.Common.BigList.Renderers.StringRenderer<Data.LogFolderRecord> (x => this.folderAccessor.GetMessage (x));

			this.folderScrollList.SetUpItemList<Data.LogFolderRecord> (this.folderAccessor, this.folderAccessor, renderer);
#endif

			this.RefreshContents ();
		}


		private void CreateUISettingsFrame(Widget windowRoot)
		{
			var settingsFrame = new FrameBox ()
						{
							Parent = windowRoot,
							Name = "Settings",
							Dock = DockStyle.Bottom,
							PreferredHeight = 30,
							ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
						};

			var radio1 = new RadioButton ()
			{
				Parent = settingsFrame,
				Group = "mode",
				Dock = DockStyle.Stacked,
				Text = "Exactly one",
				PreferredWidth = 60,
				ActiveState = ActiveState.Yes,
			};

			var radio2 = new RadioButton ()
			{
				Parent = settingsFrame,
				Group = "mode",
				Dock = DockStyle.Stacked,
				Text = "Zero or one",
				PreferredWidth = 60,
			};

			var radio3 = new RadioButton ()
			{
				Parent = settingsFrame,
				Group = "mode",
				Dock = DockStyle.Stacked,
				Text = "Multiple",
				PreferredWidth = 60,
			};

			var radio4 = new RadioButton ()
			{
				Parent = settingsFrame,
				Group = "mode",
				Dock = DockStyle.Stacked,
				Text = "One or more",
				PreferredWidth = 60,
			};



			radio1.ActiveStateChanged += _ =>
			{
				if (radio1.IsActive)
					this.historyData.Features.SelectionMode = ItemSelectionMode.ExactlyOne;
			};
			radio2.ActiveStateChanged += _ =>
			{
				if (radio2.IsActive)
					this.historyData.Features.SelectionMode = ItemSelectionMode.ZeroOrOne;
			};
			radio3.ActiveStateChanged += _ =>
			{
				if (radio3.IsActive)
					this.historyData.Features.SelectionMode = ItemSelectionMode.Multiple;
			};
			radio4.ActiveStateChanged += _ =>
			{
				if (radio4.IsActive)
					this.historyData.Features.SelectionMode = ItemSelectionMode.OneOrMore;
			};
		}
		private void CreateUIListForFolderItems(FrameBox left)
		{
			ItemListColumnHeaderView header;

#if false
			this.folderList = new ItemListVerticalContentView ()
			{
				Parent = left,
				Dock = DockStyle.Fill,
				ItemRenderer = new Epsitec.Common.BigList.Renderers.StringRenderer<Data.LogFolderRecord> (x => this.folderAccessor.GetMessage (x)),
			};

			this.folderList.ActiveIndexChanged += this.HandleFolderListActiveIndexChanged;

			header = new ItemListColumnHeaderView ()
			{
				Parent = left,
				Dock = DockStyle.Top,
				PreferredHeight = 24,
				BackColor = Color.FromBrightness (1.0),
			};
#else
			this.folderScrollList = new Common.BigList.Widgets.ItemScrollList ()
			{
				Parent = left,
				Dock = DockStyle.Fill,
			};

			this.folderScrollList.ActiveIndexChanged += this.HandleFolderScrollListActiveIndexChanged;

			header = this.folderScrollList.Header;
			header.BackColor = Color.FromBrightness (1.0);
#endif
			
			var col1 = new ItemListColumn ();
			var col2 = new ItemListColumn ();
			var col3 = new ItemListColumn ();

			col1.Title = "Id";
			col1.Index = 1;
			col1.CanSort = true;
			col1.Layout.Definition.LeftBorder = 1;
			col1.Layout.Definition.RightBorder = 0;
			col1.Layout.Definition.MinWidth = 20;
			col1.Layout.Definition.Width = new Common.Widgets.Layouts.GridLength (32, Common.Widgets.Layouts.GridUnitType.Absolute);

			col2.Title = "Timestamp";
			col2.Index = 2;
			col2.CanSort = true;
			col2.Layout.Definition.LeftBorder = 1;
			col2.Layout.Definition.RightBorder = 0;
			col2.Layout.Definition.MinWidth = 40;
			col2.Layout.Definition.Width = new Common.Widgets.Layouts.GridLength (80, Common.Widgets.Layouts.GridUnitType.Absolute);

			col3.Title = "Machine";
			col3.Index = 3;
			col3.CanSort = true;
			col3.Layout.Definition.LeftBorder = 1;
			col3.Layout.Definition.RightBorder = 0;
			col3.Layout.Definition.MinWidth = 40;
			col3.Layout.Definition.Width = new Common.Widgets.Layouts.GridLength (200, Common.Widgets.Layouts.GridUnitType.Absolute);

			header.Columns.Add (col1);
			header.Columns.Add (col2);
			header.Columns.Add (col3);
		}
		
		private void CreateUIListForHistoryItems(FrameBox left)
		{
			var header = new ItemListColumnHeaderView ()
						{
							Parent = left,
							Dock = DockStyle.Top,
							PreferredHeight = 24,
							BackColor = Color.FromBrightness (1.0),
						};

			var col1 = new ItemListColumn ();
			var col2 = new ItemListColumn ();
			var col3 = new ItemListColumn ();

			col1.Title = "Id";
			col1.Index = 2;
			col1.CanSort = true;
			col1.Layout.Definition.LeftBorder = 1;
			col1.Layout.Definition.RightBorder = 0;
			col1.Layout.Definition.MinWidth = 20;
			col1.Layout.Definition.Width = new Common.Widgets.Layouts.GridLength (32, Common.Widgets.Layouts.GridUnitType.Proportional);

			col2.Title = "Timestamp";
			col2.Index = 1;
			col2.CanSort = true;
			col2.Layout.Definition.LeftBorder = 1;
			col2.Layout.Definition.RightBorder = 0;
			col2.Layout.Definition.MinWidth = 40;
			col2.Layout.Definition.Width = new Common.Widgets.Layouts.GridLength (80, Common.Widgets.Layouts.GridUnitType.Absolute);

			col3.Title = "Message";
			col3.Index = 3;
			col3.CanSort = true;
			col3.Layout.Definition.LeftBorder = 1;
			col3.Layout.Definition.RightBorder = 0;
			col3.Layout.Definition.MinWidth = 40;
			col3.Layout.Definition.Width = new Common.Widgets.Layouts.GridLength (200, Common.Widgets.Layouts.GridUnitType.Absolute);

			header.Columns.Add (col1);
			header.Columns.Add (col2);
			header.Columns.Add (col3);

			this.historyList = new ItemListVerticalContentView ()
			{
				Parent = left,
				Dock = DockStyle.Fill,
				ItemRenderer = new Epsitec.Common.BigList.Renderers.StringRenderer<Data.LogRecord> (x => this.historyAccessor.GetMessage (x))
				{
					AlternateBackgroundColor = true,
				},
				MarkRenderer = new Epsitec.Common.BigList.Renderers.MarkRenderer (),
			};

			this.historyList.ActiveIndexChanged += this.HandleHistoryListActiveIndexChanged;
		}


		private void HandleFolderListActiveIndexChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var index = (int) e.NewValue;

			if (index < 0)
			{
				return;
			}

			var item = this.folderList.ItemList.Cache.GetItemData (index).GetData<Data.LogFolderRecord> ();

			this.DefineHistoryAccessor (new LogDataAccessor (item.Path));
		}

		private void HandleFolderScrollListActiveIndexChanged(object sender, ItemListIndexEventArgs e)
		{
			var index = e.NewIndex;

			if (index < 0)
			{
				return;
			}

			var item = this.folderScrollList.GetItemData<Data.LogFolderRecord> (index);

			this.DefineHistoryAccessor (new LogDataAccessor (item.Path));
		}

		private void HandleHistoryListActiveIndexChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var index = (int) e.NewValue;

			if (index < 0)
			{
				return;
			}

		    var item = this.historyList.ItemList.Cache.GetItemData (index).GetData<Data.LogRecord> ();
		    var time = item.TimeStamp;

		    this.SetMainImage (this.historyAccessor.GetStaticImage (this.historyAccessor.Images.FirstOrDefault (x => x.TimeStamp >= time)));
		}

		
		private void SetMainImage(StaticImage staticImage)
		{
			if (this.mainImage != null)
			{
				this.mainImage.Dispose ();
			}

			this.mainImage = staticImage;

			if (this.mainImage != null)
			{
				this.mainImage.Dock = DockStyle.Fill;
				this.mainImage.Parent = this.view;
			}
		}

		private void RefreshContents()
		{
			if (this.container == null)
			{
				return;
			}

			if (this.folderAccessor != null)
			{
#if false
				this.folderData.Reset ();
				this.folderList.Invalidate ();
#else		
				this.folderScrollList.Invalidate ();
#endif
			}

			if (this.historyAccessor != null)
			{
				this.historyData.Reset ();
				this.historyList.Invalidate ();
			}
		}

		
		private readonly CoreInteractiveApp		host;

		private FrameBox						container;
		private FrameBox						view;
		private ItemListVerticalContentView		historyList;
		private ItemList						historyData;
		private ItemListVerticalContentView		folderList;
		private Epsitec.Common.BigList.Widgets.ItemScrollList folderScrollList;
		private ItemList						folderData;
		private StaticImage						mainImage;
		private LogDataAccessor					historyAccessor;
		private LogFolderDataAccessor			folderAccessor;
	}
}
