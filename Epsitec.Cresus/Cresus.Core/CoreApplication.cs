//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Common.Dialogs;

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
			this.persistenceManager = new UI.PersistenceManager ();

			this.data = new CoreData ();
			this.exceptionManager = new CoreLibrary.ExceptionManager ();
			this.commands = new CoreCommands (this);
		}


		public bool								IsReady
		{
			get;
			private set;
		}

		public CoreData							Data
		{
			get
			{
				return this.data;
			}
		}

		public IExceptionManager				ExceptionManager
		{
			get
			{
				return this.exceptionManager;
			}
		}

		public StateManager						StateManager
		{
			get
			{
				return this.stateManager;
			}
		}
		
		public override string					ShortWindowTitle
		{
			get
			{
				return Res.Strings.ProductName.ToSimpleText ();
			}
		}

		
		internal void SetupInterface()
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

		internal void SetupData()
		{
			this.data.SetupDatabase ();
		}


		internal void AsyncSaveApplicationState()
		{
			Application.QueueAsyncCallback (this.SaveApplicationState);
		}


		internal void StartNewSearch(Druid entityId, Druid formId)
		{
			States.FormState state =
				new States.FormState (this.stateManager)
				{
					BoxId = this.defaultBoxId,
					StateDeck = States.StateDeck.History,
					Title = "Rech.",
					EntityId = entityId,
					FormId = formId,
					Mode = FormStateMode.Search
				};

			this.stateManager.Push (state);
		}

		internal void StartEdit()
		{
			States.FormState formState = this.GetCurrentFormWorkspaceState ();

			if (formState == null)
			{
				return;
			}
			
			AbstractEntity entity = formState.Item;

			if (entity == null)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (EntityContext.IsSearchEntity (entity) == false);
			
			Druid entityId = entity.GetEntityStructuredTypeId ();
			Druid formId   = this.FindCreationFormId (entityId);
			
			//	Recycle existing edition form, if there is one :

			foreach (var item in States.CoreState.FindAll<States.FormState> (this.StateManager, s => s.Mode == FormStateMode.Edition))
			{
				if (item.Item == entity)
				{
					this.StateManager.Push (item);
					return;
				}
			}
			
			//	Create new workspace for the edition :

			States.FormState state =
				new States.FormState (this.stateManager)
				{
					BoxId = this.defaultBoxId,
					StateDeck = States.StateDeck.StandAlone,
					Title = "Edition",
					EntityId = entityId,
					FormId = formId,
					Mode = FormStateMode.Edition,
					Item = entity,
					LinkedState = formState
				};

			this.stateManager.Push (state);
			this.stateManager.Hide (formState);
		}

		internal States.FormState GetCurrentFormWorkspaceState()
		{
			States.FormState formState = this.StateManager.ActiveState as States.FormState;
			return formState;
		}

		internal bool EndEdit(bool accept)
		{
			States.CoreState state = this.stateManager.ActiveState;
			States.FormState formState = state as States.FormState;

			if ((formState != null) &&
				(formState.Mode != FormStateMode.Search))
			{
				if (accept)
				{
					formState.AcceptEdition ();
					this.data.DataContext.SaveChanges ();

					if ((formState.Mode == FormStateMode.Creation) &&
						(formState.LinkedStateFocusPath != null))
					{
						States.FormState linkedFormState = formState.LinkedState as States.FormState;
						linkedFormState.SetFieldValue (formState.LinkedStateFocusPath, formState.Item);

					}
				}

				//	TODO: reselect edited entity

				this.stateManager.Show (formState.LinkedState);
				this.stateManager.Pop (formState);
				formState.Dispose ();
				return true;
			}

			return false;
		}

		internal bool CreateRecord()
		{
			States.FormState formState = this.GetCurrentFormWorkspaceState ();

			if (formState == null)
			{
				return false;
			}

			Druid  entityId      = Druid.Empty;
			string linkFieldPath = null;

			if (formState.Mode == FormStateMode.Search)
			{
				//	The form is in the general search mode. We will create a fresh record
				//	matching the data being currently visualized.

				entityId = formState.EntityId;
			}
			else
			{
				//	The form is in edition (or creation) mode and we want to create a new
				//	item for the currently active reference placeholder.

				ISearchContext context = DialogSearchController.GetGlobalSearchContext ();

				if (context == null)
				{
					return false;
				}

				List<Druid> entityIds = new List<Druid> (context.GetEntityIds ());

				if (entityIds.Count == 0)
				{
					return false;
				}

				System.Diagnostics.Debug.Assert (entityIds.Count == 1);

				entityId      = entityIds[0];
				linkFieldPath = formState.FocusPath;
			}

			System.Diagnostics.Debug.Assert (entityId.IsValid);

			return this.CreateRecord (entityId, linkFieldPath, null);
		}

		internal bool CreateRecord(Druid entityId, string linkFieldPath, System.Action<AbstractEntity> initializer)
		{
			Druid formId = this.FindCreationFormId (entityId);

			if (formId.IsEmpty)
			{
				return false;
			}

			States.FormState formState = this.GetCurrentFormWorkspaceState ();
			AbstractEntity entity = this.data.DataContext.CreateEntity (entityId);

			if (initializer != null)
			{
				initializer (entity);
			}
			
			//	Create new workspace for the edition :

			States.FormState state =
				new States.FormState (this.stateManager)
				{
					BoxId = this.defaultBoxId,
					StateDeck = States.StateDeck.StandAlone,
					Title = "Création",
					EntityId = entityId,
					FormId = formId,
					Mode = FormStateMode.Creation,
					Item = entity,
					LinkedState = formState,
					LinkedStateFocusPath = linkFieldPath
				};

			//	TODO: better linking -- when exiting with validation, should fill in the missing
			//	element...

			this.stateManager.Push (state);

			return true;
		}

		private Druid FindCreationFormId(Druid entityId)
		{
			//	TODO: find dynamically FormId based on EntityId...

			if (entityId == Mai2008.Entities.ArticleEntity.EntityStructuredTypeId)
			{
				return Mai2008.FormIds.Article;
			}
			if (entityId == Mai2008.Entities.FactureEntity.EntityStructuredTypeId)
			{
				return Mai2008.FormIds.Facture;
			}
//			if (entityId == Mai2008.Entities.LigneFactureEntity.EntityStructuredTypeId)
//			{
//				return Mai2008.FormIds.TableLigneFacture;
//			}
			if (entityId == Mai2008.Entities.ClientEntity.EntityStructuredTypeId)
			{
				return Mai2008.FormIds.Client;
			}
			if (entityId == AddressBook.Entities.TitrePersonneEntity.EntityStructuredTypeId)
			{
				return Mai2008.FormIds.TitrePersonne;
			}

			return Druid.Empty;
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

			this.persistenceManager.Register (this.ribbonBook);
			
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
				this.persistenceManager.Restore (store.Element ("uiSettings"));
			}
			
			this.persistenceManager.DiscardChanges ();
			this.persistenceManager.SettingsChanged += (sender) => Application.QueueAsyncCallback (this.SaveApplicationState);

			this.stateManager.StackChanged += (sender, e) => this.UpdateCommandsAfterStateChange ();
			this.stateManager.StackChanged += (sender, e) => Application.QueueAsyncCallback (this.SaveApplicationState);

			this.UpdateCommandsAfterStateChange ();
		}

		private void UpdateCommandsAfterStateChange()
		{
			States.FormState formState = this.StateManager.ActiveState as States.FormState;

			if (formState != null)
			{
				switch (formState.Mode)
				{
					case FormStateMode.Creation:
					case FormStateMode.Edition:
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Edit).Enable   = false;
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Accept).Enable = true;	//	TODO: use validity check
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Cancel).Enable = true;

						this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Edit).Visibility   = false;
						this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Accept).Visibility = true;
						this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Cancel).Visibility = true;
						break;

					case FormStateMode.Search:
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
						this.persistenceManager.Save ("uiSettings")));

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
		UI.PersistenceManager					persistenceManager;
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
