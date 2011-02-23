//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Orchestrators;

namespace Epsitec.Cresus.Core.Controllers
{
	public class MainWindowController : CoreViewController, IWidgetUpdater
	{
		public MainWindowController(CoreData data, CommandContext commandContext, DataViewOrchestrator orchestrator, CoreViewController ribbonController)
			: base ("MainWindow", orchestrator)
		{
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
