//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	class RibbonViewController : CoreViewController
	{
		public RibbonViewController()
			: base ("Ribbon")
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.CreateRibbonBook (container);
			this.CreateRibbonHomePage ();
		}

		
		private void CreateRibbonBook(Widget container)
		{
			this.ribbonBook = new RibbonBook ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
				Name = "Ribbon"
			};

			CoreProgram.Application.PersistanceManager.Register (this.ribbonBook);
		}

		private static RibbonPage CreateRibbonPage(RibbonBook book, string name, string title)
		{
			return new RibbonPage (book)
			{
				Name = name,
				RibbonTitle = title,
				PreferredHeight = 78,
			};
		}

		private void CreateRibbonHomePage()
		{
			this.ribbonPageHome = RibbonViewController.CreateRibbonPage (this.ribbonBook, "Home", "Principal");

			this.CreateRibbonEditSection ();
			this.CreateRibbonDatabaseSection ();
			this.CreateRibbonStateSection ();
		}

		private void CreateRibbonEditSection()
		{
			var section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Edit",
				Title = "Édition",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = 200,
			};

			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Edition.SaveRecord));

			new FrameBox ()
			{
				Parent = section,
				Dock = DockStyle.Stacked,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow
			};
		}

		private void CreateRibbonDatabaseSection()
		{
			var section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Database",
				Title = "Bases de données",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = 240,
			};

			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Base.ShowCustomers));
		}

		private void CreateRibbonStateSection()
		{
			var section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "State",
				Title = "États",
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};
		}

		
		private static IconButton CreateButton(Command command, DockStyle dockStyle = DockStyle.StackBegin, CommandEventHandler handler = null, int dx = 31)
		{
			if (handler != null)
			{
				CoreProgram.Application.CommandDispatcher.Register (command, handler);
			}

			double buttonDx = 2 * ((dx + 1) / 2 + 5);

			return new RibbonIconButton
			{
				CommandObject = command,
				PreferredIconSize = new Size (dx, 31),
				PreferredSize = new Size (buttonDx, buttonDx),
				Dock = dockStyle,
				Name = command.Name,
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center,
				AutoFocus = false,
			};
		}


		private RibbonBook						ribbonBook;
		private RibbonPage						ribbonPageHome;
	}
}
