//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	public partial class CoreApplication : Application
	{
		public CoreApplication()
		{
			this.stateManager = new StateManager (this);
			this.persistanceManager = new UI.PersistanceManager ();

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
			this.RestoreApplicationState ();

			this.IsReady = true;
		}

		public void SetupData()
		{
			this.data.SetupDatabase ();
		}


		public void AsyncSaveApplicationState()
		{
			Application.QueueAsyncCallback (this.SaveApplicationState);
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
					Title = "Rech.",
					Workspace = workspace
				};

			this.stateManager.Push (state);
		}

		public void StartEdit()
		{
			States.FormWorkspaceState formState = this.StateManager.ActiveState as States.FormWorkspaceState;

			if (formState == null)
			{
				return;
			}
			
			AbstractEntity entity = formState.CurrentEntity;

			if (entity == null)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (EntityContext.IsSearchEntity (entity) == false);
			
			Druid formId   = formState.Workspace.FormId;
			Druid entityId = entity.GetEntityStructuredTypeId ();
			
			//	Recycle existing edition form, if there is one :

			foreach (var item in States.FormWorkspaceState.FindAll (this.StateManager, s => s.Workspace.Mode == FormWorkspaceMode.Edition))
			{
				if (item.Workspace.CurrentItem == entity)
				{
					this.StateManager.Push (item);
					return;
				}
			}
			
			//	Create new workspace for the edition :

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
					Workspace = workspace,
					LinkedState = formState
				};

			this.stateManager.Push (state);
			this.stateManager.Hide (formState);
		}

		public bool EndEdit(bool accept)
		{
			States.CoreState state = this.stateManager.ActiveState;
			States.FormWorkspaceState formState = state as States.FormWorkspaceState;

			if ((formState != null) &&
				(formState.Workspace.Mode == FormWorkspaceMode.Edition))
			{
				if (accept)
				{
					formState.Workspace.AcceptEdition ();
					this.data.DataContext.SaveChanges ();
				}

				this.stateManager.Show (formState.LinkedState);
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
				Dock = DockStyle.Fill,
				Name = "Ribbon"
			};

			this.persistanceManager.Register (this.ribbonBook);
			
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
			section.Children.Add (this.CreateButton (Mai2008.Res.Commands.Edition.Edit, DockStyle.StackEnd, null, 63));
		}

		private void RestoreApplicationState()
		{
			if (System.IO.File.Exists (CoreApplication.Paths.SettingsPath))
			{
				XDocument doc = XDocument.Load (CoreApplication.Paths.SettingsPath);
				XElement store = doc.Element ("store");

				this.stateManager.RestoreStates (store.Element ("stateManager"));
				UI.RestoreWindowPositions (store.Element ("windowPositions"));
				this.persistanceManager.Restore (store.Element ("uiSettings"));
			}
			
			this.persistanceManager.DiscardChanges ();
			this.persistanceManager.SettingsChanged += (sender) => Application.QueueAsyncCallback (this.SaveApplicationState);

			this.stateManager.StackChanged += (sender, e) => this.UpdateCommandsAfterStateChange ();
			this.stateManager.StackChanged += (sender, e) => Application.QueueAsyncCallback (this.SaveApplicationState);

			this.UpdateCommandsAfterStateChange ();
		}

		private void UpdateCommandsAfterStateChange()
		{
			States.FormWorkspaceState formState = this.StateManager.ActiveState as States.FormWorkspaceState;

			if (formState != null)
			{
				switch (formState.Workspace.Mode)
				{
					case FormWorkspaceMode.Edition:
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Edit).Enable   = false;
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Accept).Enable = true;
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Cancel).Enable = true;

						this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Edit).Visibility   = false;
						this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Accept).Visibility = true;
						this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Cancel).Visibility = true;
						break;

					case FormWorkspaceMode.Search:
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Edit).Enable   = true;
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Accept).Enable = false;
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Cancel).Enable = false;

						this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Edit).Visibility   = true;
						this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Accept).Visibility = false;
						this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Cancel).Visibility = false;
						break;
				}
			}
		}

		private void SaveApplicationState()
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
						UI.SaveWindowPositions ("windowPositions"),
						this.persistanceManager.Save ("uiSettings")));

				doc.Save (CoreApplication.Paths.SettingsPath);
				System.Diagnostics.Debug.WriteLine ("Save done.");
			}
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
			return this.CreateButton (command, dockStyle, handler, 31);
		}

		private IconButton CreateButton(Command command, DockStyle dockStyle, CommandEventHandler handler, double dx)
		{
			if (handler != null)
			{
				this.CommandDispatcher.Register (command, handler);
			}

			return new IconButton (command, new Epsitec.Common.Drawing.Size (dx, 31), dockStyle)
			{
				Name = command.Name,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
		}


		

		StateManager							stateManager;
		UI.PersistanceManager					persistanceManager;
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
