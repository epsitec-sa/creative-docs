//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Controllers
{
	public class MainViewController : CoreViewController
	{
		public MainViewController(List<AbstractEntity> entities)
			: base ("MainView")
		{
			this.entities = entities;

			this.browserController = new BrowserViewController ("MainBrowser");
			this.dataViewController = new DataViewController ("MainViewer");


			this.browserController.SetContents (this.entities);
			this.browserController.CurrentChanged += sender => this.dataViewController.SetEntity (this.browserController.ActiveEntity);
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.browserController;
			yield return this.dataViewController;
		}

		public override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.entities != null);

			this.frame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Fill,
				DrawFullFrame = true,
			};

			//	Crée les panneaux gauche et droite séparés par un splitter.
			this.leftPanel = new FrameBox
			{
				Parent = this.frame,
				Name = "LeftPanel",
				Dock = DockStyle.Left,
				Padding = new Margins (5),
				PreferredWidth = 150,
			};

			this.rightPanel = new FrameBox
			{
				Parent = this.frame,
				Name = "RightPanel",
				Dock = DockStyle.Fill,
				Padding = new Margins (5, 0, 5, 5),
			};

			this.splitter = new VSplitter
			{
				Parent = this.frame,
				Dock = DockStyle.Left,
			};

			this.browserController.CreateUI (this.leftPanel);
			this.dataViewController.CreateUI (this.rightPanel);
		}


		private readonly BrowserViewController browserController;
		private readonly DataViewController dataViewController;

		private List<AbstractEntity> entities;

		private FrameBox frame;

		private FrameBox leftPanel;
		private VSplitter splitter;
		private FrameBox rightPanel;
	}
}
