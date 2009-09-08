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


		public void SetupUI()
		{
			if (this.window == null)
			{
				this.window = new Window ()
				{
					Text = Res.Strings.Application.Name.ToSimpleText (),
					Name = "Application",
					ClientSize = new Size (960, 600),
					Icon = Bitmap.FromManifestResource ("Epsitec.Cresus.Graph.Images.Application.icon", this.GetType ().Assembly)
				};

				this.toolsFrame = new FrameBox ()
				{
					Name = "tools",
					Dock = DockStyle.Top,
					ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
					Parent = this.window.Root,
					Margins = new Margins (0, 0, 0, 4),
					Padding = new Margins (2, 2, 2, 2),
					PreferredHeight = 150,
				};

				this.workspaceFrame = new FrameBox ()
				{
					Name = "workspace",
					Dock = DockStyle.Fill,
					Parent = this.window.Root,
				};

				MainWindowController.CreateTools (this.toolsFrame);
			}
		}

		
		private static void CreateTools(FrameBox frame)
		{
			foreach (var command in MainWindowController.GetToolCommands ())
			{
				new IconButton ()
				{
					Dock = DockStyle.Stacked,
					CommandObject = command,
					PreferredWidth = 32,
					PreferredIconSize = new Size (20, 20),
					Parent = frame
				};
			}
		}

		private static IEnumerable<Command> GetToolCommands()
		{
			yield return ApplicationCommands.Undo;
			yield return ApplicationCommands.Redo;
			yield return ApplicationCommands.Save;
			yield return ApplicationCommands.Copy;
			
			yield return Res.Commands.File.ExportImage;
		}


		private Window							window;
		private FrameBox						toolsFrame;
		private FrameBox						workspaceFrame;
	}
}
