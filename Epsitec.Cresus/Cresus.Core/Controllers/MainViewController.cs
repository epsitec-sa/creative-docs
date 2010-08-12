//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.BrowserControllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class MainViewController : CoreViewController
	{
		public MainViewController(CoreData data)
			: base ("MainView")
		{
			this.data = data;

			this.navigator = new NavigationOrchestrator (this);
			this.Orchestrator = new DataViewOrchestrator ();
			this.printEngine = new PrintEngine ();

			this.dataViewController = new DataViewController ("Data", data)
			{
				Orchestrator = this.Orchestrator,
				Navigator = this.Navigator,
			};

			this.browserViewController = new BrowserViewController ("Browser", data)
			{
				Orchestrator = this.Orchestrator,
			};

			this.browserSettingsController = new BrowserSettingsController ("BrowserSettings", this.browserViewController)
			{
				Orchestrator = this.Orchestrator,
			};

			this.Orchestrator.Controller = this.dataViewController;

			this.browserViewController.CurrentChanging +=
				delegate
				{
					System.Diagnostics.Debug.WriteLine ("CurrentChanging");
					this.dataViewController.ClearActiveEntity ();
				};

			this.browserViewController.CurrentChanged +=
				delegate
				{
					System.Diagnostics.Debug.WriteLine ("CurrentChanged");
					this.browserViewController.SelectActiveEntity (this.dataViewController);
				};
		}

		public new NavigationOrchestrator Navigator
		{
			get
			{
				return this.navigator;
			}
		}

		public BrowserSettingsMode BrowserSettingsMode
		{
			get
			{
				return this.browserSettingsMode;
			}
			set
			{
				if (this.browserSettingsMode != value)
                {
					this.browserSettingsMode = value;
					this.UpdateBrowserSettingsPanel ();
                }
			}
		}

		public BrowserViewController BrowserViewController
		{
			get
			{
				return this.browserViewController;
			}
		}

		public DataViewController DataViewController
		{
			get
			{
				return this.dataViewController;
			}
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.browserViewController;
			yield return this.browserSettingsController;
			yield return this.dataViewController;
		}

		public override void CreateUI(Widget container)
		{
			this.CreateUIFrame (container);

			this.browserViewController.CreateUI (this.leftPanel);
			this.browserSettingsController.CreateUI (this.browserSettingsPanel);
			this.dataViewController.CreateUI (this.rightPanel);

			this.BrowserSettingsMode = BrowserSettingsMode.Compact;

			CoreProgram.Application.Commands.PushHandler (Res.Commands.Edition.Print, () => this.Print ());
			CoreProgram.Application.Commands.PushHandler (Res.Commands.Edition.Preview, () => this.Preview ());
		}


		private void CreateUIFrame(Widget container)
		{
			this.frame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Fill,
				DrawFullFrame = false,
				DrawFrameState = FrameState.None
			};
			
			this.CreateUITopPanel ();
			this.CreateUISettingsPanel ();
			this.CreateUILeftPanel ();
			this.CreateUIRightPanel ();
			this.CreateUISplitter ();
		}

		private void CreateUITopPanel()
		{
			this.topPanel = new FrameBox
			{
				Parent = this.frame,
				Name = "TopPanel",
				Dock = DockStyle.Top,
				PreferredHeight = 40,
			};
		}

		private void CreateUISettingsPanel()
		{
			this.browserSettingsPanel = new FrameBox
			{
				Parent = this.topPanel,
				Name = "SettingsPanel",
				Dock = DockStyle.Fill,
			};
		}

		private void CreateUILeftPanel()
		{
			this.leftPanel = new FrameBox
			{
				Parent = this.frame,
				Name = "LeftPanel",
				Dock = DockStyle.Left,
				Padding = new Margins (0, 0, 0, 0),
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
				Padding = new Margins (0, 0, 0, 0),
			};
		}

		private void CreateUISplitter()
		{
			this.splitter = new VSplitter
			{
				Parent = this.frame,
				Dock = DockStyle.Left,
				PreferredWidth = 8,
			};
		}

		private void UpdateBrowserSettingsPanel()
		{
			var expandedPanel = this.topPanel;
			var compactPanel  = this.browserViewController.SettingsPanel;

			switch (this.browserSettingsMode)
			{
				case BrowserSettingsMode.Hidden:
					expandedPanel.Visibility = false;
					compactPanel.Visibility  = false;
					this.browserSettingsPanel.Parent = null;
					break;

				case BrowserSettingsMode.Compact:
					expandedPanel.Visibility = false;
					compactPanel.Visibility  = true;
					this.browserSettingsPanel.Parent = compactPanel;
					break;

				case BrowserSettingsMode.Expanded:
					expandedPanel.Visibility = true;
					compactPanel.Visibility  = false;
					this.browserSettingsPanel.Parent = expandedPanel;
					break;

				default:
					throw new System.NotSupportedException (string.Format ("BrowserSettingsMode.{0} not supported", this.browserSettingsMode));
			}
		}


		private void Print()
		{
			var context = this.data.DataContext;
			var entity = this.browserViewController.GetActiveEntity (context);
			this.printEngine.Print (entity);
		}

		private void Preview()
		{
			var context = this.data.DataContext;
			var entity = this.browserViewController.GetActiveEntity (context);
			this.printEngine.Preview (entity);
		}

		
		private readonly CoreData data;
		private readonly BrowserViewController browserViewController;
		private readonly BrowserSettingsController browserSettingsController;
		private readonly DataViewController dataViewController;
		private readonly NavigationOrchestrator navigator;
		private readonly PrintEngine printEngine;

		private FrameBox frame;

		private FrameBox topPanel;
		private FrameBox browserSettingsPanel;
		private FrameBox leftPanel;
		private VSplitter splitter;
		private FrameBox rightPanel;

		private BrowserSettingsMode browserSettingsMode;
	}
}
