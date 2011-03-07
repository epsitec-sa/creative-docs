//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
//-using Epsitec.Cresus.Core.Print;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;
using System.Collections;
using Epsitec.Cresus.Core.Print;

namespace Epsitec.Cresus.Core.Controllers
{
	public class MainViewController : CoreViewController, ICommandHandler
	{
		public MainViewController(DataViewOrchestrator orchestrator, CommandContext commandContext)
			: base ("MainView", orchestrator)
		{
			this.commandContext = commandContext;
			this.Orchestrator.Host.RegisterComponent (this);

			this.actionViewController  = new ActionViewController (this.Orchestrator);
//-			this.previewViewController = new PreviewViewController (this.Orchestrator);

			this.browserViewController = new BrowserViewController (this.Orchestrator);
			this.browserSettingsController = new BrowserSettingsController (this.browserViewController);

			
			
			
			//	TODO: check if following code is still meaningful

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
				return this.Orchestrator.Navigator;
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
				return this.Orchestrator.DataViewController;
			}
		}

		public ActionViewController ActionViewController
		{
			get
			{
				return this.actionViewController;
			}
		}

#if false
		public PreviewViewController PreviewViewController
		{
			get
			{
				return this.previewViewController;
			}
		}
#endif

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.browserViewController;
			yield return this.browserSettingsController;
			yield return this.DataViewController;
			yield return this.actionViewController;
//-			yield return this.previewViewController;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);
			this.CreateUIFrame (container);

			this.browserViewController.CreateUI (this.leftPanel);
			this.browserSettingsController.CreateUI (this.browserSettingsPanel);
			this.DataViewController.CreateUI (this.mainPanel);
//-			this.previewViewController.CreateUI (this.rightPreviewPanel);
			this.actionViewController.CreateUI (this.rightActionPanel);

			this.BrowserSettingsMode = BrowserSettingsMode.Compact;
		}

		public static MainViewController Find(CommandContextChain contextChain)
		{
			return contextChain.Contexts.Select (x => x.GetCommandHandler<MainViewController> ()).Where (x => x != null).FirstOrDefault ();
		}

		public void SetActionPanelVisibility(bool visibility)
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
				PreferredWidth = 400,
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


		private IEnumerable<CoreViewController> GetCoreViewControllers()
		{
			return this.GetSubControllers ().Where (x => x is CoreViewController).Cast<CoreViewController> ();
		}


#if true
		
		public void Print()
		{
			var entityKey = this.GetVisiblePersistedEntities ().Select (x => this.Data.FindEntityKey (x)).FirstOrDefault ();
			var context   = this.Data.CreateDataContext ("PrintEngine:Print");
			var entity    = context.ResolveEntity (entityKey);

			PrintEngine.PrintCommand (this.BusinessContext, entity);

			this.Data.DisposeDataContext (context);
		}

		public void Preview()
		{
			var entityKey = this.GetVisiblePersistedEntities ().Select (x => this.Data.FindEntityKey (x)).FirstOrDefault ();
			var context   = this.Data.CreateDataContext ("PrintEngine:Preview");
			var entity    = context.ResolveEntity (entityKey);

			PrintEngine.PreviewCommand (this.BusinessContext, entity);

			this.Data.DisposeDataContext (context);
		}

		private IEnumerable<AbstractEntity> GetVisiblePersistedEntities()
		{
			var leaf = this.DataViewController.GetLeafViewController ();

			if (leaf != null)
			{
				foreach (var node in leaf.GetControllerChain ().Select (x => x as EntityViewController))
				{
					if (node != null)
					{
						var entity = node.GetEntity ();
						var context = this.Data.DataContextPool.FindDataContext (entity);

						if (context.IsPersistent (entity))
						{
							//	HACK
							//	TODO: remove this
							if (entity is BusinessDocumentEntity)
							{
								DocumentMetadataEntity example = new DocumentMetadataEntity ();
								example.BusinessDocument = entity as BusinessDocumentEntity;
								example.IsArchive = false;
								yield return context.GetByExample<DocumentMetadataEntity> (example).FirstOrDefault ();
							}
							yield return entity;
						}
					}
				}
			}
		}

#endif

		public IEnumerable<AbstractEntity> GetVisibleEntities()
		{
			var leaf = this.DataViewController.GetLeafViewController ();

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

		#region ICommandHandler Members

		void ICommandHandler.UpdateCommandStates(object sender)
		{
		}

		#endregion

		private readonly CommandContext commandContext;
		private readonly BrowserViewController browserViewController;
		private readonly BrowserSettingsController browserSettingsController;
		private readonly ActionViewController actionViewController;
//-		private readonly PreviewViewController previewViewController;

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
