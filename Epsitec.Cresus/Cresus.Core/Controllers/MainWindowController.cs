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
	public class MainWindowController : CoreViewController
	{
		public MainWindowController(CoreData data, CommandContext commandContext)
			: base ("MainWindow")
		{
			this.data = data;
			this.commandContext = commandContext;
			
			this.ribbonController = new RibbonViewController ();
			this.contentController = new MainViewController (this.data, this.commandContext);
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.ribbonController;
			yield return this.contentController;
		}

		public override void CreateUI(Widget container)
		{
			this.CreateUIRootBoxes (container);
			this.CreateUIControllers ();

			CoreProgram.Application.Commands.PushHandler (Res.Commands.Global.Settings, () => this.Settings ());
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
			this.contentController.CreateUI (this.contentBox);
		}


		private void Settings()
		{
			var manager = new Dialogs.PrinterManagerDialog (CoreProgram.Application);
			manager.OpenDialog ();
		}


		private readonly CoreData				data;
		private readonly RibbonViewController	ribbonController;
		private readonly MainViewController		contentController;
		private readonly CommandContext			commandContext;
		
		private FrameBox						ribbonBox;
		private FrameBox						contentBox;
	}
}
