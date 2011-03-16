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
	public class MainWindowController : CoreViewController, IWidgetUpdater, ICoreManualComponent
	{
		public MainWindowController(CoreApp app, CoreData data, CommandContext commandContext, CoreViewController ribbonController)
			: base ("MainWindow", app.FindActiveComponent<DataViewOrchestrator> ())
		{
			app.RegisterComponent (this);
			app.RegisterComponentAsDisposable (this);
			app.ActivateComponent (this);

			this.data = data;
			this.commandContext = commandContext;
			
			this.ribbonController = ribbonController;
			this.mainViewController = this.Orchestrator.MainViewController;

			Library.UI.UpdateRequested += sender => this.Update ();
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.ribbonController;
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
			this.ribbonController.CreateUI (this.ribbonBox);
			this.mainViewController.CreateUI (this.contentBox);
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			this.mainViewController.GetSubControllers ().Select (x => x as IWidgetUpdater).Where (x => x != null).ForEach (x => x.Update ());
		}

		#endregion
		


		private readonly CoreData				data;
		private readonly CoreViewController		ribbonController;
		private readonly MainViewController		mainViewController;
		private readonly CommandContext			commandContext;
		
		private FrameBox						ribbonBox;
		private FrameBox						contentBox;
	}
}
