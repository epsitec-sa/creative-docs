//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class MainViewController : CoreViewController
	{
		public MainViewController(CoreData data)
			: base ("MainView")
		{
			this.data     = data;

			this.browserController = new BrowserViewController ("MainBrowser", data);
			this.dataViewController = new DataViewController ("MainViewer", data);

			this.browserController.SetContents (() => this.data.GetCustomers ());
			
			this.browserController.CurrentChanging +=
				delegate
				{
					this.dataViewController.ClearActiveEntity ();
				};

			this.browserController.CurrentChanged +=
				delegate
				{
					this.dataViewController.SetActiveEntity (this.browserController.GetActiveEntity ());
				};
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.browserController;
			yield return this.dataViewController;
		}

		public override void CreateUI(Widget container)
		{
			this.CreateUIFrame (container);
			this.CreateUILeftPanel ();
			this.CreateUIRightPanel ();
			this.CreateUISplitter ();

			this.browserController.CreateUI (this.leftPanel);
			this.dataViewController.CreateUI (this.rightPanel);
		}


		private void CreateUIFrame(Widget container)
		{
			this.frame = new FrameBox
						{
							Parent = container,
							Dock = DockStyle.Fill,
							DrawFullFrame = true,
						};
		}

		private void CreateUILeftPanel()
		{
			this.leftPanel = new FrameBox
						{
							Parent = this.frame,
							Name = "LeftPanel",
							Dock = DockStyle.Left,
							Padding = new Margins (5),
							PreferredWidth = 150,
						};
		}

		private void CreateUIRightPanel()
		{
			this.rightPanel = new FrameBox
						{
							Parent = this.frame,
							Name = "RightPanel",
							Dock = DockStyle.Fill,
							Padding = new Margins (5, 0, 5, 5),
						};
		}

		private void CreateUISplitter()
		{
			this.splitter = new VSplitter
						{
							Parent = this.frame,
							Dock = DockStyle.Left,
						};
		}
		
		private readonly CoreData data;
		private readonly BrowserViewController browserController;
		private readonly DataViewController dataViewController;

		private FrameBox frame;

		private FrameBox leftPanel;
		private VSplitter splitter;
		private FrameBox rightPanel;
	}
}
