//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System;

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
					PreferredWidth = 160,
					ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
					Parent = this.toolsFrame
				};

				WorkspaceController.CreateLabel (commandFrame, "Opérations");

				var commandButtonsFrame = new FrameBox()
				{
					Name = "buttons",
					Dock = DockStyle.Stacked,
					Margins = new Margins (2, 2, 2, 6),
					ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
					PreferredHeight = 48,
					Parent = commandFrame,
				};

				this.workspaceFrame = new FrameBox ()
				{
					Name = "workspace",
					Dock = DockStyle.Fill,
					Parent = this.window.Root,
				};

				MainWindowController.CreateTools (commandButtonsFrame);
			}
		}

		
		private static void CreateTools(FrameBox frame)
		{
			var groups = from command in MainWindowController.GetToolCommands ()
						 group command by command.GroupName;

			foreach (var group in groups)
			{
				var groupFrame = new FrameBox ()
				{
					Dock = DockStyle.Stacked,
					Parent = frame,
					DrawFullFrame = true,
					PreferredWidth = 8,
					Margins = new Margins (0, 1, 0, 0),
					Padding = new Margins (2, 2, 2, 2),
					ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
					BackColor = Color.FromBrightness (0.95),
				};

				foreach (var command in group)
				{
					double iconSize = 20;

					switch (command.CommandSize)
					{
						case CommandSize.Large:
							iconSize = 31;
							break;
					}

					var button = new MetaButton ()
					{
						Dock = DockStyle.Stacked,
						ButtonClass = ButtonClass.FlatButton,
						CommandObject = command.Command,
						PreferredSize = new Size (iconSize+2, iconSize+2),
						PreferredIconSize = new Size (iconSize, iconSize),
						Parent = groupFrame,
						VerticalAlignment = VerticalAlignment.Top,
					};

					button.AddEventHandler (Visual.VisibilityProperty, (sender, e) => MainWindowController.UpdateFrameVisibility (groupFrame));
				}
			}
		}

		private static void UpdateFrameVisibility(FrameBox frame)
		{
			IEnumerable<Visual> children = frame.Children;
			frame.Visibility = children.Any (x => x.Visibility);
		}

        private static IEnumerable<CommandDescription> GetToolCommands()
		{
			yield return new CommandDescription (ApplicationCommands.New, CommandSize.Large, "Fichier");
			yield return new CommandDescription (ApplicationCommands.Open, CommandSize.Large, "Fichier");
			yield return new CommandDescription (ApplicationCommands.Save, CommandSize.Large, "Fichier");
			yield return new CommandDescription (ApplicationCommands.SaveAs, CommandSize.Medium, "Fichier");
//-			yield return new CommandDescription (Res.Commands.General.Kill, CommandSize.Medium);
			yield return new CommandDescription (ApplicationCommands.Undo, CommandSize.Medium, "Actions");
			yield return new CommandDescription (ApplicationCommands.Redo, CommandSize.Medium, "Actions");
			yield return new CommandDescription (Res.Commands.General.DownloadUpdate, CommandSize.Large, "Maintenance");
		}

		class CommandDescription
		{
			public CommandDescription(Command command, CommandSize size, string groupName)
			{
				this.Command = command;
				this.CommandSize = size;
				this.GroupName = groupName;
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

			public string GroupName
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
