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
	/// application, with its tools and document tab book.
	/// </summary>
	internal class MainWindowController
	{
		public MainWindowController()
		{
		}


		public Window Window
		{
			get
			{
				if (this.window == null)
				{
					this.CreateUI ();
				}

				return this.window;
			}
		}

		public TabBook DocTabBook
		{
			get
			{
				if (this.window == null)
				{
					this.CreateUI ();
				}
				
				return this.docTabBook;
			}
		}

		
		private void CreateUI()
		{
			if (this.window == null)
			{
				this.window = new Window ()
				{
					Text = "Crésus Graphe",
					Name = "Application",
					ClientSize = new Size (960, 600)
				};

				this.toolsFrame = new FrameBox ()
				{
					Name = "ToolsFrame",
					Dock = DockStyle.Top,
					ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
					Parent = this.window.Root,
					Margins = new Margins (0, 0, 0, 4),
					Padding = new Margins (2, 2, 2, 2)
				};

				this.docAreaFrame = new FrameBox ()
				{
					Name = "DocAreaFrame",
					Dock = DockStyle.Fill,
					Parent = this.window.Root
				};

				this.docTabBook = new TabBook ()
				{
					Name = "DocTabBook",
					Dock = DockStyle.Fill,
					Parent = this.docAreaFrame
				};

				MainWindowController.CreateTools (this.toolsFrame);
			}
		}

		
		private static void CreateTools(FrameBox frame)
		{
			new MetaButton ()
			{
				ButtonClass = ButtonClass.FlatButton,
				Dock = DockStyle.Stacked,
				CommandObject = ApplicationCommands.Undo,
				PreferredWidth = 32,
				Parent = frame
			};

			new MetaButton ()
			{
				ButtonClass = ButtonClass.FlatButton,
				Dock = DockStyle.Stacked,
				CommandObject = ApplicationCommands.Redo,
				PreferredWidth = 32,
				Parent = frame
			};

			new IconButton ()
			{
				Dock = DockStyle.Stacked,
				CommandObject = ApplicationCommands.Save,
				PreferredWidth = 32,
				Parent = frame
			};
		}


		private Window							window;
		private FrameBox						toolsFrame;
		private FrameBox						docAreaFrame;
		private TabBook							docTabBook;
	}
}
