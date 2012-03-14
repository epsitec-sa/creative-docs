//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
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

			this.historyList = new ScrollList ()
			{
				Parent = left,
				Dock = DockStyle.Fill,
			};

			this.historyList.Items.ValueConverter = x => this.accessor.GetMessage ((Data.LogRecord) x).ToString ();
			this.historyList.SelectedItemChanged += this.HandleHistoryListSelectedItemChanged;

			this.RefreshContents ();
		}

		public void DefineAccessor(LogDataAccessor accessor)
		{
			this.accessor = accessor;
			this.RefreshContents ();
		}

		private void HandleHistoryListSelectedItemChanged(object sender)
		{
			var item = this.historyList.Items.Values[this.historyList.SelectedItemIndex] as Data.LogRecord;
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

			this.historyList.Items.Clear ();
			this.historyList.Items.AddRange (this.accessor.Messages);
		}

		private readonly CoreInteractiveApp		host;

		private FrameBox						container;
		private FrameBox						view;
		private ScrollList						historyList;
		private ScrollList						screenList;
		private StaticImage						mainImage;
		private LogDataAccessor					accessor;
	}
}
