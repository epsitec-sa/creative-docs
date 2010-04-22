//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
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
	public partial class CoreApplication
	{
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

						//this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Edit).Visibility   = false;
						//this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Accept).Visibility = true;
						//this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Cancel).Visibility = true;
						break;

					case FormStateMode.Search:
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Edit).Enable   = true;
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Accept).Enable = false;
						this.CommandContext.GetCommandState (Mai2008.Res.Commands.Edition.Cancel).Enable = false;

						//this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Edit).Visibility   = true;
						//this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Accept).Visibility = false;
						//this.ribbonBook.FindCommandWidget (Mai2008.Res.Commands.Edition.Cancel).Visibility = false;
						break;
				}
			}
		}
	}
}
