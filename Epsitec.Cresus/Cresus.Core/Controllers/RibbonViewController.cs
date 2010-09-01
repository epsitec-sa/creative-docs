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
			this.CreateRibbonClipboardSection ();
			this.CreateRibbonFontSection ();
			this.CreateRibbonDatabaseSection ();
			this.CreateRibbonStateSection ();
			this.CreateRibbonSettingsSection ();
			this.CreateRibbonNavigationSection ();
		}

		private void CreateRibbonEditSection()
		{
			var section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Edit",
				Title = "Édition",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = RibbonViewController.GetButtonWidth (RibbonViewController.buttonLargeWidth) * 4 +
								 RibbonViewController.GetButtonWidth (RibbonViewController.buttonSmallWidth) * 1,
			};

			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Edition.SaveRecord));
			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Edition.DiscardRecord));
			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Edition.Print));
			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Edition.Preview));

			var frame = new FrameBox
			{
				Parent = section,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth = RibbonViewController.GetButtonWidth (RibbonViewController.buttonSmallWidth),
			};

			frame.Children.Add (RibbonViewController.CreateButton (Res.Commands.File.ImportV11, dx: RibbonViewController.buttonSmallWidth));
		}

		private void CreateRibbonClipboardSection()
		{
			var section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Clipboard",
				Title = "Presse-papier",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = RibbonViewController.GetButtonWidth (RibbonViewController.buttonSmallWidth) + RibbonViewController.GetButtonWidth (RibbonViewController.buttonLargeWidth),
			};

			var frame = new FrameBox
			{
				Parent = section,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth = RibbonViewController.GetButtonWidth (RibbonViewController.buttonSmallWidth),
			};

			frame.Children.Add (RibbonViewController.CreateButton (ApplicationCommands.Cut,  dx: RibbonViewController.buttonSmallWidth));
			frame.Children.Add (RibbonViewController.CreateButton (ApplicationCommands.Copy, dx: RibbonViewController.buttonSmallWidth));

			section.Children.Add (RibbonViewController.CreateButton (ApplicationCommands.Paste));
		}

		private void CreateRibbonFontSection()
		{
			var section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Font",
				Title = "Police",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = RibbonViewController.GetButtonWidth (RibbonViewController.buttonSmallWidth) * 3,
			};

			var frame = new FrameBox
			{
				Parent = section,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
			};

			var topFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			var bottomFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			topFrame.Children.Add (RibbonViewController.CreateButton (Res.Commands.Font.Bold,           dx: RibbonViewController.buttonSmallWidth));
			topFrame.Children.Add (RibbonViewController.CreateButton (Res.Commands.Font.Italic,         dx: RibbonViewController.buttonSmallWidth));
			topFrame.Children.Add (RibbonViewController.CreateButton (Res.Commands.Font.Underline,      dx: RibbonViewController.buttonSmallWidth));

			bottomFrame.Children.Add (RibbonViewController.CreateButton (Res.Commands.Font.Subscript,   dx: RibbonViewController.buttonSmallWidth));
			bottomFrame.Children.Add (RibbonViewController.CreateButton (Res.Commands.Font.Superscript, dx: RibbonViewController.buttonSmallWidth));
		}

		private void CreateRibbonDatabaseSection()
		{
			var section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Database",
				Title = "Bases de données",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = RibbonViewController.GetButtonWidth (RibbonViewController.buttonLargeWidth) * 4,
			};

			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Base.ShowCustomers));
			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Base.ShowArticleDefinitions));
			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Base.ShowDocuments));
			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Base.ShowInvoiceDocuments));
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

		private void CreateRibbonNavigationSection()
		{
			var section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Navigation",
				Title = "Navigation",
				PreferredWidth = RibbonViewController.GetButtonWidth (RibbonViewController.buttonLargeWidth) * 2,
				Dock = DockStyle.Right,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.History.NavigateBackward));
			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.History.NavigateForward));
		}

		private void CreateRibbonSettingsSection()
		{
			var section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Settings",
				Title = "Réglages",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Right,
				PreferredWidth = RibbonViewController.GetButtonWidth (RibbonViewController.buttonLargeWidth) * 1,
			};

			section.Children.Add (RibbonViewController.CreateButton (Res.Commands.Global.Settings));
		}

		
		private static IconButton CreateButton(Command command, DockStyle dockStyle = DockStyle.StackBegin, CommandEventHandler handler = null, int? dx = null)
		{
			if (handler != null)
			{
				CoreProgram.Application.CommandDispatcher.Register (command, handler);
			}

			if (!dx.HasValue)
			{
				dx = RibbonViewController.buttonLargeWidth;
			}

			double buttonWidth = RibbonViewController.GetButtonWidth (dx.Value);

			return new RibbonIconButton
			{
				CommandObject = command,
				PreferredIconSize = new Size (dx.Value, dx.Value),
				PreferredSize = new Size (buttonWidth, buttonWidth),
				Dock = dockStyle,
				Name = command.Name,
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center,
				AutoFocus = false,
			};
		}

		private static double GetButtonWidth(int dx)
		{
			return 2 * ((dx + 1) / 2 + 5);
		}


		private static readonly int buttonSmallWidth = 14;
		private static readonly int buttonLargeWidth = 31;


		private RibbonBook						ribbonBook;
		private RibbonPage						ribbonPageHome;
	}
}
