//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>CoreApplication</c> class implements the central application
	/// logic.
	/// </summary>
	public class CoreApplication : Application
	{
		public CoreApplication()
		{
			this.stateManager = new StateManager ()
			{
				Application = this
			};

			this.data = new CoreData ();
			this.exceptionManager = new CoreLibrary.ExceptionManager ();
		}


		public CoreData Data
		{
			get
			{
				return this.data;
			}
		}

		public IExceptionManager ExceptionManager
		{
			get
			{
				return this.exceptionManager;
			}
		}
		
		public override string ShortWindowTitle
		{
			get
			{
				return Res.Strings.ProductName;
			}
		}

		
		public void SetupInterface()
		{
			Window window = new Window ();

			window.Text = this.ShortWindowTitle;
			window.ClientSize = new Epsitec.Common.Drawing.Size (600, 400);

			this.Window = window;

			this.ribbonBox = new FrameBox (window.Root)
			{
				Dock = DockStyle.Top
			};

			this.defaultBox = new FrameBox (window.Root)
			{
				Dock = DockStyle.Fill
			};

			this.defaultBoxId = this.stateManager.DefineBox (this.defaultBox);

			this.CreateRibbon ();
			this.CreateWorkspace ();
		}

		public void SetupData()
		{
			this.data.SetupDatabase ();
		}

		private void CreateRibbon()
		{
			this.ribbonBook = new RibbonBook (this.ribbonBox)
			{
				Dock = DockStyle.Fill
			};
			
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
				Title = "Edition",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = 200,
			};

			frame = new FrameBox (section)
			{
				Dock = DockStyle.Stacked,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow
			};

			section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Bases",
				Title = "Bases de données",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

			section.Children.Add (CoreApplication.CreateButton (Command.Get (Druid.Parse ("[9VA5]"))));
			section.Children.Add (CoreApplication.CreateButton (Command.Get (Druid.Parse ("[9VA9]"))));
			section.Children.Add (CoreApplication.CreateButton (Command.Get (Druid.Parse ("[9VA8]"))));
			section.Children.Add (CoreApplication.CreateButton (Command.Get (Druid.Parse ("[9VA7]"))));
			section.Children.Add (CoreApplication.CreateButton (Command.Get (Druid.Parse ("[9VA6]"))));

			section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "States",
				Title = "Etats",
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

			section.Children.Add (CoreApplication.CreateButton (Command.Get (Druid.Parse ("[9VAD]"))));
			section.Children.Add (CoreApplication.CreateButton (Command.Get (Druid.Parse ("[9VAC]"))));

			section.Children.Add (
				new Widgets.StateStackWidget ()
				{
					Dock = DockStyle.Stacked,
					StateManager = this.stateManager,
					StateDeck = States.StateDeck.History,
					PreferredWidth = 48
				});

			section.Children.Add (
				new Widgets.StateStackWidget ()
				{
					Dock = DockStyle.StackFill,
					StateManager = this.stateManager,
					StateDeck = States.StateDeck.StandAlone,
					PreferredWidth = 48
				});

			section.Children.Add (CoreApplication.CreateButton (Command.Get (Druid.Parse ("[9VAA]")), DockStyle.StackEnd));
			section.Children.Add (CoreApplication.CreateButton (Command.Get (Druid.Parse ("[9VAB]")), DockStyle.StackEnd));

#if false
			subFrame = new FrameBox (frame);
			subFrame.Dock = DockStyle.Stacked;
			subFrame.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			subFrame.Children.Add (new IconButton (ApplicationCommands.New, new Size (47, 36), DockStyle.Stacked));

			Widget groupOpen = new Widget (subFrame);
			groupOpen.Dock = DockStyle.Stacked;
			groupOpen.PreferredSize = new Size (47, 47);
			GlyphButton buttonLastFiles = new GlyphButton (groupOpen);
			IconButton buttonOpen = new IconButton (ApplicationCommands.Open, new Size (47, 36), DockStyle.Top);
#endif
		}

		private void CreateWorkspace()
		{
			States.FormWorkspaceState state = new States.FormWorkspaceState (this.stateManager)
			{
				BoxId = this.defaultBoxId,
				StateDeck = States.StateDeck.History,
				Workspace = new Workspaces.FormWorkspace ()
				{
#if true
					FormId   = Epsitec.Cresus.AddressBook.FormIds.AdressePersonne,
					EntityId = Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity.EntityStructuredTypeId
#else
					FormId   = Epsitec.Cresus.Mai2008.FormIds.Facture,
					EntityId = Epsitec.Cresus.Mai2008.Entities.FactureEntity.EntityStructuredTypeId
#endif
				}
			};

			this.stateManager.Push (state);
			this.stateManager.Push (
				new States.FormWorkspaceState (this.stateManager)
				{
					StateDeck = States.StateDeck.History,
					Workspace = new Workspaces.FormWorkspace ()
				});

			for (int i = 0; i < 3; i++)
			{
				this.stateManager.Push (
					new States.FormWorkspaceState (this.stateManager)
					{
						StateDeck = States.StateDeck.StandAlone,
						Workspace = new Workspaces.FormWorkspace (),
						Title = string.Format ("{0}", (char)('A'+i))
					});
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.data != null)
				{
					this.data.Dispose ();
					this.data = null;
				}
				if (this.exceptionManager != null)
				{
					this.exceptionManager.Dispose ();
					this.exceptionManager = null;
				}
			}

			base.Dispose (disposing);
		}

		private static IconButton CreateButton(Command command)
		{
			return CoreApplication.CreateButton (command, DockStyle.StackBegin);
		}

		private static IconButton CreateButton(Command command, DockStyle dockStyle)
		{
			return new IconButton (command, new Epsitec.Common.Drawing.Size (31, 31), dockStyle)
			{
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
		}



		StateManager							stateManager;
		CoreData								data;
		CoreLibrary.ExceptionManager			exceptionManager;
		
		private FrameBox						ribbonBox;
		private FrameBox						defaultBox;

		private RibbonBook						ribbonBook;
		private RibbonPage						ribbonPageHome;

		private int								defaultBoxId;
	}
}
