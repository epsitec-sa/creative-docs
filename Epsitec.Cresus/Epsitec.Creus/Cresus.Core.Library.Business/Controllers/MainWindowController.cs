//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public sealed class MainWindowController : ViewControllerComponent<MainWindowController>, IWidgetUpdater
	{
		private MainWindowController(DataViewOrchestrator orchestrator)
			: base (orchestrator)
		{
			this.app = orchestrator.Host;

			this.data           = this.app.FindComponent<CoreData> ();
			this.commandContext = this.app.CommandContext;
			
			this.mainViewController = this.Orchestrator.MainViewController;

			Library.UI.Services.UpdateRequested += sender => this.Update ();
		}


		public new CoreApp Host
		{
			get
			{
				return this.app;
			}
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			var ribbonViewController = this.Orchestrator.GetComponents ().First (x => x.GetType ().Name == "RibbonViewController");
			//	HACK: clean up
			yield return ribbonViewController;
			yield return this.mainViewController;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);
			this.CreateUIRootBoxes (container);
			this.CreateUIControllers ();
		}

		private void CreateUIRootBoxes(Widget container)
		{
			this.ribbonBox = new FrameBox ()
			{
				Parent = container,
				Name = "RibbonBox",
				Dock = DockStyle.Top,
			};

			this.contentBox = new FrameBox ()
			{
				Parent = container,
				Name = "ContentBox",
				Dock = DockStyle.Fill,
			};
		}

		private void CreateUIControllers()
		{
			var ribbonViewController = this.Orchestrator.GetComponents ().First (x => x.GetType ().Name == "RibbonViewController");
			//	HACK: clean up
			ribbonViewController.CreateUI (this.ribbonBox);
			this.mainViewController.CreateUI (this.contentBox);
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			this.mainViewController.GetSubControllers ().Select (x => x as IWidgetUpdater).Where (x => x != null).ForEach (x => x.Update ());
		}

		#endregion

		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultViewControllerComponentFactory<MainWindowController>
		{
			public override bool CanCreate(DataViewOrchestrator host)
			{
				return host.MainViewController != null;
			}
		}

		#endregion
		
		
		private readonly CoreApp				app;
		private readonly CoreData				data;
		private readonly MainViewController		mainViewController;
		private readonly CommandContext			commandContext;
		
		private FrameBox						ribbonBox;
		private FrameBox						contentBox;
	}
}
