//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Controllers
{
	class RibbonController : AbstractController
	{
		public RibbonController()
		{
		}

		public override void CreateUI(Widget container)
		{
			this.ribbonBook = new RibbonBook ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
				Name = "Ribbon"
			};

			CoreProgram.Application.PersistanceManager.Register (this.ribbonBook);

			this.ribbonPageHome = new RibbonPage (this.ribbonBook)
			{
				Name = "Home",
				RibbonTitle = "Principal"
			};

			RibbonSection section;
			FrameBox      frame;

			section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Edition",
				Title = "Édition",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = 200,
			};

			frame = new FrameBox ()
			{
				Parent = section,
				Dock = DockStyle.Stacked,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow
			};

			section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Bases",
				Title = "Bases de données",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.SwitchToBase.BillIn));
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.SwitchToBase.Suppliers));
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.SwitchToBase.Items));
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.SwitchToBase.Customers));
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.SwitchToBase.BillOut));

			section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "States",
				Title = "Etats",
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.History.NavigatePrev, handler: (s, e) => CoreProgram.Application.StateManager.NavigateHistoryPrev ()));
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.History.NavigateNext, handler: (s, e) => CoreProgram.Application.StateManager.NavigateHistoryNext ()));

			section.Children.Add (
				new Widgets.StateDeckWidget ()
				{
					Dock = DockStyle.Stacked,
					StateManager = CoreProgram.Application.StateManager,
					StateDeck = States.StateDeck.History,
					PreferredWidth = 48
				});

			section.Children.Add (
				new Widgets.StateDeckWidget ()
				{
					Dock = DockStyle.StackFill,
					StateManager = CoreProgram.Application.StateManager,
					StateDeck = States.StateDeck.StandAlone,
					PreferredWidth = 48
				});

			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.Edition.Accept, DockStyle.StackEnd, null));
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.Edition.Cancel, DockStyle.StackEnd, null));
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.Edition.Edit, DockStyle.StackEnd, null, 63));
		}

		private IconButton CreateButton(Command command, DockStyle dockStyle = DockStyle.StackBegin, CommandEventHandler handler = null, double dx = 31.0)
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
