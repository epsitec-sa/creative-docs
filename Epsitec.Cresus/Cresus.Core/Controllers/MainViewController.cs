﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.BrowserControllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;
using System.Collections;

namespace Epsitec.Cresus.Core.Controllers
{
	public class MainViewController : CoreViewController, ICommandHandler
	{
		public MainViewController(CoreData data, CommandContext commandContext)
			: base ("MainView")
		{
			this.data = data;
			this.commandContext = commandContext;

			this.data.AboutToSaveDataContext += this.HandleAboutToSaveDataContext;
			this.data.AboutToDiscardDataContext += this.HandleAboutToDiscardDataContext;

			this.navigator = new NavigationOrchestrator (this);
			this.Orchestrator = new DataViewOrchestrator (this);

			this.dataViewController = new DataViewController ("Data", data)
			{
				Orchestrator = this.Orchestrator,
				Navigator = this.Navigator,
			};

			this.actionViewController  = new ActionViewController ("ActionPanel", data);
			this.previewViewController = new PreviewViewController ("Preview", data);

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

			this.commandContext.AttachCommandHandler (this);
		}

		public CommandContext CommandContext
		{
			get
			{
				return this.commandContext;
			}
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

		public ActionViewController ActionViewController
		{
			get
			{
				return this.actionViewController;
			}
		}

		public PreviewViewController PreviewViewController
		{
			get
			{
				return this.previewViewController;
			}
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.browserViewController;
			yield return this.browserSettingsController;
			yield return this.dataViewController;
			yield return this.actionViewController;
			yield return this.previewViewController;
		}

		public override void CreateUI(Widget container)
		{
			this.CreateUIFrame (container);

			this.browserViewController.CreateUI (this.leftPanel);
			this.browserSettingsController.CreateUI (this.browserSettingsPanel);
			this.dataViewController.CreateUI (this.mainPanel);
			this.previewViewController.CreateUI (this.rightPreviewPanel);
			this.actionViewController.CreateUI (this.rightActionPanel);

			this.BrowserSettingsMode = BrowserSettingsMode.Compact;
		}

		public static MainViewController Find(CommandContextChain contextChain)
		{
			return contextChain.Contexts.Select (x => x.GetCommandHandler<MainViewController> ()).Where (x => x != null).FirstOrDefault ();
		}

		public void SetActionActionVisibility(bool visibility)
		{
			this.rightActionPanel.Visibility = visibility;
			this.rightSplitter.Visibility = this.rightActionPanel.Visibility | this.rightPreviewPanel.Visibility;
		}

		public void SetPreviewPanelVisibility(bool visibility)
		{
			this.rightPreviewPanel.Visibility = visibility;
			this.rightSplitter.Visibility = this.rightActionPanel.Visibility | this.rightPreviewPanel.Visibility;
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.commandContext.DetachCommandHandler (this);

				this.data.AboutToSaveDataContext -= this.HandleAboutToSaveDataContext;
				this.data.AboutToDiscardDataContext -= this.HandleAboutToDiscardDataContext;
			}

			base.Dispose (disposing);
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
			this.CreateUISplitter ();
			this.CreateUIMainPanel ();
			this.CreateUIRightPanels ();
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

		private void CreateUIMainPanel()
		{
			this.mainPanel = new FrameBox
			{
				Parent = this.frame,
				Name = "MainPanel",
				Dock = DockStyle.Fill,
				Padding = new Margins (0, 0, 0, 0),
			};
		}

		private void CreateUIRightPanels()
		{
			this.rightActionPanel = new FrameBox
			{
				Parent = this.frame,
				Name = "RightActionPanel",
				Dock = DockStyle.Right,
				Padding = new Margins (0, 0, 0, 0),
				Visibility = false,
			};
			
			this.rightPreviewPanel = new FrameBox
			{
				Parent = this.frame,
				Name = "RightPreviewPanel",
				Dock = DockStyle.Right,
				Padding = new Margins (0, 0, 0, 0),
				Visibility = false,
			};
			
			this.rightSplitter = new VSplitter
			{
				Parent = this.frame,
				Dock = DockStyle.Right,
				PreferredWidth = 8,
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


		private void HandleAboutToSaveDataContext(object sender, DataContextEventArgs e)
		{
			this.GetCoreViewControllers ().ForEach (controller => controller.AboutToSave (e.DataContext));
		}

		private void HandleAboutToDiscardDataContext(object sender, DataContextEventArgs e)
		{
			this.GetCoreViewControllers ().ForEach (controller => controller.AboutToDiscard (e.DataContext));
		}

		private IEnumerable<CoreViewController> GetCoreViewControllers()
		{
			return this.GetSubControllers ().Where (x => x is CoreViewController).Cast<CoreViewController> ();
		}



		
		public void Print()
		{
			var entityKey = this.GetVisiblePersistedEntities ().Where (x => PrintEngine.CanPrint (x)).Select (x => DataContextPool.Instance.FindEntityKey (x)).FirstOrDefault ();
			var context   = this.data.CreateDataContext ();
			var entity    = context.ResolveEntity (entityKey);

			PrintEngine.Print (entity);

			this.data.DisposeDataContext (context);
		}

		public void Preview()
		{
			var entityKey = this.GetVisiblePersistedEntities ().Where (x => PrintEngine.CanPrint (x)).Select (x => DataContextPool.Instance.FindEntityKey (x)).FirstOrDefault ();
			var context   = this.data.CreateDataContext ();
			var entity    = context.ResolveEntity (entityKey);

			PrintEngine.Preview (entity);

			this.data.DisposeDataContext (context);
//			var context = this.data.DataContext;
//			var entity = this.browserViewController.GetActiveEntity (context);
//			PrintEngine.Preview (entity);
		}

		private IEnumerable<AbstractEntity> GetVisiblePersistedEntities()
		{
			var leaf = this.dataViewController.GetLeafViewController ();

			if (leaf != null)
			{
				foreach (var node in leaf.GetControllerChain ().Select (x => x as EntityViewController))
				{
					if (node != null)
					{
						var entity = node.GetEntity ();
						var context = DataContextPool.Instance.FindDataContext (entity);

						if (context.IsPersistent (entity))
						{
							yield return entity;
						}
					}
				}
			}
		}

		public IEnumerable<AbstractEntity> GetVisibleEntities()
		{
			var leaf = this.dataViewController.GetLeafViewController ();

			if (leaf != null)
			{
				foreach (var node in leaf.GetControllerChain ().Select (x => x as EntityViewController))
				{
					if (node != null)
					{
						yield return node.GetEntity ();
					}
				}
			}
		}

		private readonly CoreData data;
		private readonly CommandContext commandContext;
		private readonly BrowserViewController browserViewController;
		private readonly BrowserSettingsController browserSettingsController;
		private readonly DataViewController dataViewController;
		private readonly ActionViewController actionViewController;
		private readonly PreviewViewController previewViewController;
		private readonly NavigationOrchestrator navigator;

		private FrameBox frame;

		private FrameBox topPanel;
		private FrameBox browserSettingsPanel;
		private FrameBox leftPanel;
		private VSplitter splitter;
		private VSplitter rightSplitter;
		private FrameBox mainPanel;
		private FrameBox rightActionPanel;
		private FrameBox rightPreviewPanel;

		private BrowserSettingsMode browserSettingsMode;
	}
}
