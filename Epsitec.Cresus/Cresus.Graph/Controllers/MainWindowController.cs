//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	/// <summary>
	/// The <c>MainWindowController</c> class creates and manages the main window of the
	/// application, with its tools and the workspace area.
	/// </summary>
	internal class MainWindowController
	{
		public MainWindowController(GraphApplication application)
		{
		}


		public Window Window
		{
			get
			{
				if (this.window == null)
				{
					this.SetupUI ();
				}

				return this.window;
			}
		}

		public Widget WorkspaceFrame
		{
			get
			{
				return this.workspaceFrame;
			}
		}

		public Widget ToolsFrame
		{
			get
			{
				return this.toolsFrame;
			}
		}


		public void SetupUI()
		{
			if (this.window == null)
			{
				this.window = new Window ()
				{
					Text = GraphProgram.Application.ShortWindowTitle,
					Name = "Application",
					ClientSize = new Size (960, 600),
					Icon = Bitmap.FromManifestResource ("Epsitec.Cresus.Graph.Images.Application.icon", this.GetType ().Assembly)
				};

				this.toolsFrame = new FrameBox ()
				{
					Name = "tools",
					Dock = DockStyle.Top,
					Parent = this.window.Root,
					PreferredHeight = 80,
				};

				var commandFrame = new FrameBox ()
				{
					Name = "commands",
					Dock = DockStyle.Left,
					PreferredWidth = 200,
					ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
					Margins = new Margins (2, 2, 2, 6),
					Parent = this.toolsFrame,
				};

				this.workspaceFrame = new FrameBox ()
				{
					Name = "workspace",
					Dock = DockStyle.Fill,
					Parent = this.window.Root,
				};

				MainWindowController.CreateTools (commandFrame);
			}
		}

		
		private static void CreateTools(FrameBox frame)
		{
			foreach (var command in MainWindowController.GetToolCommands ())
			{
				double iconSize = 20;

				switch (command.CommandSize)
				{
					case CommandSize.Large:
						iconSize = 31;
						break;
				}

				new IconButton ()
				{
					Dock = DockStyle.Stacked,
					CommandObject = command.Command,
					PreferredSize = new Size (iconSize+2, iconSize+2),
					PreferredIconSize = new Size (iconSize, iconSize),
					Parent = frame,
					VerticalAlignment = VerticalAlignment.Top,
				};
			}
		}

		private static IEnumerable<CommandDescription> GetToolCommands()
		{
			yield return new CommandDescription (ApplicationCommands.Open, CommandSize.Large);
			yield return new CommandDescription (ApplicationCommands.Save, CommandSize.Large);
			yield return new CommandDescription (ApplicationCommands.SaveAs, CommandSize.Medium);
//-			yield return new CommandDescription (Res.Commands.General.Kill, CommandSize.Medium);
			yield return new CommandDescription (ApplicationCommands.Undo, CommandSize.Medium);
			yield return new CommandDescription (ApplicationCommands.Redo, CommandSize.Medium);
			yield return new CommandDescription (Res.Commands.General.DownloadUpdate, CommandSize.Large);
		}

		class CommandDescription
		{
			public CommandDescription(Command command, CommandSize size)
			{
				this.Command = command;
				this.CommandSize = size;
			}
			
			public Command Command
			{
				get;
				set;
			}

			public CommandSize CommandSize
			{
				get;
				set;
			}
		}

		enum CommandSize
		{
			Small,
			Medium,
			Large,
		}


		private Window							window;
		private FrameBox						toolsFrame;
		private FrameBox						workspaceFrame;
	}
}
