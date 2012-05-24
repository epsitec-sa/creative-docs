//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	public class PrésentationController
	{
		public PrésentationController(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;

			this.commands = new List<Command> ();
		}


		public void CreateUI(Widget parent)
		{
			this.tabsPane = new TabsPane
			{
				Parent          = parent,
				TabLookStyle    = TabLook.Simple,
				SelectionColor  = UIBuilder.WindowBackColor3,
				IconSize        = 32,
				Dock            = DockStyle.Fill,
			};

			this.tabsPane.SelectedIndexChanged += delegate
			{
				this.ChangePrésentation ();
			};

			this.CreateRightUI ();
			this.UpdateTabItems ();
			this.tabsPane.SelectedIndex = 4;
		}

		private void CreateRightUI()
		{
			var rightFrame = new FrameBox
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth      = 10,
				PreferredHeight     = 24,
				Padding             = new Margins (0, 0, 0, 5),
			};

			this.CreateLargeButton (rightFrame, Res.Commands.Edit.Create);
			this.CreateLargeButton (rightFrame, Res.Commands.Edit.Accept);
			this.CreateLargeButton (rightFrame, Res.Commands.Edit.Cancel);

			Widget topSection, bottomSection;
			PrésentationController.CreateSubsections (rightFrame, out topSection, out bottomSection);

			this.CreateSmallButton (topSection, Res.Commands.Edit.Duplicate);
			this.CreateSmallButton (topSection, Res.Commands.Edit.Up);

			this.CreateSmallButton (bottomSection, Res.Commands.Edit.Delete);
			this.CreateSmallButton (bottomSection, Res.Commands.Edit.Down);

			new FrameBox
			{
				Parent         = rightFrame,
				PreferredWidth = 10,
				Dock           = DockStyle.StackBegin,
			};

			this.tabsPane.AddRightWidget (rightFrame);
		}

		private static void CreateSubsections(Widget section, out Widget topSection, out Widget bottomSection)
		{
			//	Crée deux sous-sections dans le faux ruban.
			var frame = new FrameBox
			{
				Parent              = section,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.StackBegin,
			};

			topSection = new FrameBox
			{
				Parent              = frame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.Top,
			};

			bottomSection = new FrameBox
			{
				Parent              = frame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.Bottom,
			};
		}

		private void CreateLargeButton(Widget parent, Command cmd)
		{
			UIBuilder.CreateButton (parent, cmd, 40, 32);
		}

		private void CreateSmallButton(Widget parent, Command cmd)
		{
			UIBuilder.CreateButton (parent, cmd, 20, 20);
		}


		public void UpdateTabItems()
		{
			this.tabsPane.Clear ();
			this.commands.Clear ();

			foreach (var cmd in Présentations.PrésentationCommands)
			{
				this.CreateTabItem (cmd);
			}
		}

		private void CreateTabItem(Command cmd)
		{
			CommandState cs = this.mainWindowController.CommandContext.GetCommandState (cmd);
			if (cs.Enable)
			{
				var type = Présentations.GetControllerType (cmd);

				var tabItem = new TabItem
				{
					Icon    = Présentations.GetIcon (type),
					Tooltip = Présentations.GetGroupName (type),
				};

				this.tabsPane.Add (tabItem);
				this.commands.Add (cmd);

				if (cs.ActiveState == ActiveState.Yes)
				{
					this.tabsPane.SelectedIndex = this.tabsPane.Count-1;
				}
			}
		}


		private void ChangePrésentation()
		{
			int sel = this.tabsPane.SelectedIndex;

			if (sel != -1)
			{
				var cmd = this.commands[sel];

				this.tabsPane.Window.QueueCommand (this.tabsPane.Window.Root, cmd);
			}
		}


		private readonly MainWindowController	mainWindowController;
		private readonly List<Command>			commands;

		private TabsPane						tabsPane;
	}
}
