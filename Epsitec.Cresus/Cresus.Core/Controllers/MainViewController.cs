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
		public MainViewController(List<AbstractEntity> entities, List<Entities.ContactRoleEntity> roles, List<Entities.LocationEntity> locations, List<Entities.CountryEntity> countries)
			: base ("MainView")
		{
			this.entities = entities;
			MainViewController.roles = roles;
			MainViewController.locations = locations;
			MainViewController.countries = countries;

			this.browserController = new BrowserViewController ("MainBrowser");
			this.dataViewController = new DataViewController ("MainViewer")
			{
				DataContext = CoreProgram.Application.Data.DataContext,
			};

			this.browserController.SetContents (this.entities);
			this.browserController.CurrentChanged += sender => this.dataViewController.SetActiveEntity (this.browserController.ActiveEntity);
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
		public static List<Entities.ContactRoleEntity> roles;  // accès statique grâce au 'public', beurk
		public static List<Entities.LocationEntity> locations;  // accès statique grâce au 'public', beurk
		public static List<Entities.CountryEntity> countries;  // accès statique grâce au 'public', beurk

		private FrameBox frame;

		private FrameBox leftPanel;
		private VSplitter splitter;
		private FrameBox rightPanel;
	}
}
