//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	class RibbonController : CoreController
	{
		public RibbonController()
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

		private void CreateRibbonHomePage()
		{
			this.ribbonPageHome = new RibbonPage (this.ribbonBook)
			{
				Name = "Home",
				RibbonTitle = "Principal"
			};

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
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

			section.Children.Add (RibbonController.CreateButton (Mai2008.Res.Commands.SwitchToBase.BillIn));
			section.Children.Add (RibbonController.CreateButton (Mai2008.Res.Commands.SwitchToBase.Suppliers));
			section.Children.Add (RibbonController.CreateButton (Mai2008.Res.Commands.SwitchToBase.Items));
			section.Children.Add (RibbonController.CreateButton (Mai2008.Res.Commands.SwitchToBase.Customers));
			section.Children.Add (RibbonController.CreateButton (Mai2008.Res.Commands.SwitchToBase.BillOut));
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

			section.Children.Add (RibbonController.CreateButton (Mai2008.Res.Commands.History.NavigatePrev));
			section.Children.Add (RibbonController.CreateButton (Mai2008.Res.Commands.History.NavigateNext));

			section.Children.Add (RibbonController.CreateButton (Mai2008.Res.Commands.Edition.Accept, DockStyle.StackEnd, null));
			section.Children.Add (RibbonController.CreateButton (Mai2008.Res.Commands.Edition.Cancel, DockStyle.StackEnd, null));
		}

		
		private static IconButton CreateButton(Command command, DockStyle dockStyle = DockStyle.StackBegin, CommandEventHandler handler = null, double dx = 31.0)
		{
			if (handler != null)
			{
				CoreProgram.Application.CommandDispatcher.Register (command, handler);
			}

			return new IconButton (command, new Epsitec.Common.Drawing.Size (dx, 31), dockStyle)
			{
				Name = command.Name,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
		}


		private RibbonBook						ribbonBook;
		private RibbonPage						ribbonPageHome;
	}
}
