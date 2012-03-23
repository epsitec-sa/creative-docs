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



			radio1.ActiveStateChanged += _ => { if (radio1.IsActive) this.historyData.Features.SelectionMode = ItemSelectionMode.ExactlyOne; };
			radio2.ActiveStateChanged += _ => { if (radio2.IsActive) this.historyData.Features.SelectionMode = ItemSelectionMode.ZeroOrOne; };
			radio3.ActiveStateChanged += _ => { if (radio3.IsActive) this.historyData.Features.SelectionMode = ItemSelectionMode.Multiple; };
			radio4.ActiveStateChanged += _ => { if (radio4.IsActive) this.historyData.Features.SelectionMode = ItemSelectionMode.OneOrMore; };



			var left = new FrameBox ()
			{
				Parent = this.container,
				Name = "Left",
				Dock = DockStyle.Left,
				PreferredWidth = 240,
			};

			var splitter1 = new VSplitter ()
			{
				Parent = this.container,
				Name = "Splitter1",
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

			this.historyList = new ItemListVerticalContentView ()
			{
				Parent = left,
				Dock = DockStyle.Fill,
				ItemRenderer = new Epsitec.Common.BigList.Renderers.StringRenderer<Data.LogRecord> (x => this.accessor.GetMessage (x)),
			};

			this.historyList.ActiveIndexChanged += this.HandleHistoryListActiveIndexChanged;

			this.RefreshContents ();
		}

		public void DefineAccessor(LogDataAccessor accessor)
		{
			this.accessor = accessor;
			
			this.historyData = new ItemList<Data.LogRecord> (this.accessor, this.accessor);
			this.historyList.ItemList = this.historyData;
			this.historyData.Marks.Add (new ItemListMark
			{
				Attachment = ItemListMarkAttachment.After,
				Index = 2,
				Breadth = 2
			});
			
			this.RefreshContents ();
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

		    this.SetMainImage (this.accessor.GetStaticImage (this.accessor.Images.FirstOrDefault (x => x.TimeStamp >= time)));
		}

		private void SetMainImage(StaticImage staticImage)
		{
			if (this.mainImage != null)
			{
				this.mainImage.Dispose ();
			}

			this.mainImage = staticImage;
			this.mainImage.Dock = DockStyle.Fill;
			this.mainImage.Parent = this.view;
		}

		private void RefreshContents()
		{
			if ((this.container == null) ||
				(this.accessor == null))
			{
				return;
			}

			this.historyData.Reset ();
		}

		private readonly CoreInteractiveApp		host;

		private FrameBox						container;
		private FrameBox						view;
		private ItemListVerticalContentView		historyList;
		private ItemList						historyData;
		private StaticImage						mainImage;
		private LogDataAccessor					accessor;
	}
}
