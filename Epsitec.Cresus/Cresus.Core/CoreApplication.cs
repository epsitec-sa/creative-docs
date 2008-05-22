﻿//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
			this.stateManager = new StateManager (this);

			this.data = new CoreData ();
			this.exceptionManager = new CoreLibrary.ExceptionManager ();
			this.commands = new CoreCommands (this);
		}


		public bool IsReady
		{
			get;
			private set;
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

		public StateManager StateManager
		{
			get
			{
				return this.stateManager;
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

			this.defaultBoxId = this.stateManager.RegisterBox (this.defaultBox);

			this.CreateRibbon ();
			this.CreateWorkspace ();

			this.IsReady = true;
		}

		public void SetupData()
		{
			this.data.SetupDatabase ();
		}


		public void SaveApplicationState()
		{
			if (this.IsReady)
			{
				System.Diagnostics.Debug.WriteLine ("Saving application state.");
				System.DateTime now = System.DateTime.Now.ToUniversalTime ();
				string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

				XDocument doc = new XDocument (
					new XDeclaration ("1.0", "utf-8", "yes"),
					new XComment ("Saved on " + timeStamp),
					new XElement ("store",
						this.StateManager.SaveStates ("stateManager"),
						UI.SaveWindowPositions ("windowPositions")));

				doc.Save (@"S:\cresus.core.xml");
				System.Diagnostics.Debug.WriteLine ("Save done.");
			}
		}

		public void StartNewSearch(Druid entityId, Druid formId)
		{
			Workspaces.FormWorkspace workspace =
				new Workspaces.FormWorkspace ()
				{
					EntityId = entityId,
					FormId = formId,
					Mode = FormWorkspaceMode.Search
				};

			States.FormWorkspaceState state =
				new States.FormWorkspaceState (this.stateManager)
				{
					BoxId = this.defaultBoxId,
					StateDeck = States.StateDeck.History,
					Title = "Recherche",
					Workspace = workspace
				};

			this.stateManager.Push (state);
		}

		public void StartEdit(AbstractEntity entity, Druid formId)
		{
			Druid entityId = entity.GetEntityStructuredTypeId ();

			Workspaces.FormWorkspace workspace =
				new Workspaces.FormWorkspace ()
				{
					EntityId = entityId,
					FormId = formId,
					Mode = FormWorkspaceMode.Edition,
					CurrentItem = entity
				};

			States.FormWorkspaceState state =
				new States.FormWorkspaceState (this.stateManager)
				{
					BoxId = this.defaultBoxId,
					StateDeck = States.StateDeck.StandAlone,
					Title = "Edition",
					Workspace = workspace
				};

			this.stateManager.Push (state);
		}

		public bool EndEdit(bool accept)
		{
			States.CoreState state = Collection.GetFirst (this.stateManager.GetHistoryStates (HistorySortMode.NewestFirst));
			States.FormWorkspaceState formState = state as States.FormWorkspaceState;

			if ((formState != null) &&
				(formState.Workspace.Mode == FormWorkspaceMode.Edition))
			{
				if (accept)
				{
					formState.Workspace.AcceptEdition ();
					this.data.DataContext.SaveChanges ();
				}

				this.stateManager.Pop (formState);
				formState.Dispose ();
				return true;
			}

			return false;
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

			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.History.NavigatePrev, (s, e) => this.stateManager.NavigateHistoryPrev ()));
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.History.NavigateNext, (s, e) => this.stateManager.NavigateHistoryNext ()));

			section.Children.Add (
				new Widgets.StateDeckWidget ()
				{
					Dock = DockStyle.Stacked,
					StateManager = this.stateManager,
					StateDeck = States.StateDeck.History,
					PreferredWidth = 48
				});

			section.Children.Add (
				new Widgets.StateDeckWidget ()
				{
					Dock = DockStyle.StackFill,
					StateManager = this.stateManager,
					StateDeck = States.StateDeck.StandAlone,
					PreferredWidth = 48
				});

			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.Edition.Accept, DockStyle.StackEnd, null));
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.Edition.Cancel, DockStyle.StackEnd, null));
		}

		private void CreateWorkspace()
		{
			string path = @"S:\cresus.core.xml";

			if (System.IO.File.Exists (path))
			{
				XDocument doc = XDocument.Load (path);
				XElement store = doc.Element ("store");

				this.stateManager.RestoreStates (store.Element ("stateManager"));
				UI.RestoreWindowPositions (store.Element ("windowPositions"));
			}
#if false
			this.stateManager.Push (
				new States.FormWorkspaceState (this.stateManager)
				{
					BoxId = this.defaultBoxId,
					StateDeck = States.StateDeck.History,
					Title = "Clients",
					Workspace = new Workspaces.FormWorkspace ()
					{
						FormId   = Epsitec.Cresus.AddressBook.FormIds.AdressePersonne,
						EntityId = Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity.EntityStructuredTypeId
					}
				});
			
			this.stateManager.Push (
				new States.FormWorkspaceState (this.stateManager)
				{
					BoxId = this.defaultBoxId,
					StateDeck = States.StateDeck.History,
					Title = "Factures",
					Workspace = new Workspaces.FormWorkspace ()
					{
						FormId   = Epsitec.Cresus.Mai2008.FormIds.Facture,
						EntityId = Epsitec.Cresus.Mai2008.Entities.FactureEntity.EntityStructuredTypeId
					}
				});
#endif

#if false
			for (int i = 0; i < 3; i++)
			{
				this.stateManager.Push (
					new States.FormWorkspaceState (this.stateManager)
					{
						BoxId = this.defaultBoxId,
						StateDeck = States.StateDeck.StandAlone,
						Workspace = new Workspaces.FormWorkspace (),
						Title = string.Format ("{0}", (char)('A'+i))
					});
			}
#endif
		}

		private IconButton CreateButton(Command command)
		{
			return this.CreateButton (command, DockStyle.StackBegin, null);
		}

		private IconButton CreateButton(Command command, CommandEventHandler handler)
		{
			return this.CreateButton (command, DockStyle.StackBegin, handler);
		}

		private IconButton CreateButton(Command command, DockStyle dockStyle, CommandEventHandler handler)
		{
			if (handler != null)
			{
				this.CommandDispatcher.Register (command, handler);
			}

			return new IconButton (command, new Epsitec.Common.Drawing.Size (31, 31), dockStyle)
			{
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
		}



		StateManager							stateManager;
		CoreData								data;
		CoreLibrary.ExceptionManager			exceptionManager;
		CoreCommands							commands;
		
		private FrameBox						ribbonBox;
		private FrameBox						defaultBox;

		private RibbonBook						ribbonBook;
		private RibbonPage						ribbonPageHome;

		private int								defaultBoxId;
	}
}
