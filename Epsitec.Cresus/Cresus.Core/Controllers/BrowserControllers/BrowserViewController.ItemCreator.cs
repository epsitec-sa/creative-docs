//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Orchestrators;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public sealed partial class BrowserViewController
	{
		private class ItemCreator
		{
			public ItemCreator(BrowserViewController browser)
			{
				this.browser      = browser;
				this.orchestrator = this.browser.Orchestrator;
				this.dataSetName  = this.browser.DataSetName;
			}


			public void StartCreateNewItemInteraction()
			{
				this.orchestrator.ClearActiveEntity ();

				var controller = this.CreateCreationEntityViewController ();

				if (controller == null)
				{
					//	There is no specific creation controller; therefore, create a real entity,
					//	which will add it to the browser list, then make it active (which will in
					//	turn create the controller needed to view/edit it).

					var realEntity = this.CreateRealEntity ();

#if false
					controller = EntityViewControllerFactory.Create ("EmptyItem", realEntity, ViewControllerMode.Summary, this.orchestrator,
																	 resolutionMode: Resolvers.ResolutionMode.NullOnError);

					if (controller == null)
                    {
						return;
                    }
#endif
				}
				else
				{
					this.orchestrator.ShowSubView (null, controller);
				}
			}

			private EntityViewController CreateCreationEntityViewController()
			{
				var dummyEntity = this.CreateDummyEntity ();

				System.Diagnostics.Debug.Assert (dummyEntity != null);

				var controller = EntityViewControllerFactory.Create ("ItemCreation", dummyEntity, ViewControllerMode.Creation, this.orchestrator,
																	 resolutionMode: Resolvers.ResolutionMode.NullOnError);
				var creator    = controller as ICreationController;

				if (creator == null)
				{
					//	No creation controller could be found: dispose the dummy entity immediately,
					//	since it is no longer of any use.

					System.Diagnostics.Debug.Assert (controller == null);

					this.DisposeDummyEntity (dummyEntity);
				}
				else
				{
					//	OK, we have really been able to create a specific creation controller, which
					//	will be used to bootstrap the entity creation...

					creator.RegisterDisposeAction (() => this.DisposeDummyEntity (dummyEntity));
					creator.RegisterEntityCreator (() => this.CreateRealEntity ());
				}
				
				return controller;
			}

			private void DisposeCreationEntityViewController(EntityViewController controller)
			{
				if (controller != null)
				{
					this.DisposeDummyEntity (controller.GetEntity ());
					
					controller.Dispose ();
				}
			}

			private AbstractEntity CreateDummyEntity()
			{
				var rootEntityId = this.GetRootEntityId ();
				var data = this.orchestrator.Data;

				return data.CreateDummyEntity (rootEntityId);
			}

			private void DisposeDummyEntity(AbstractEntity entity)
			{
				var data = this.orchestrator.Data;

				data.DisposeDummyEntity (entity);
			}

			private AbstractEntity CreateRealEntity()
			{
				var rootEntityId = this.GetRootEntityId ();
				
				CoreData        data    = this.orchestrator.Data;
				BusinessContext context = data.CreateBusinessContext ();

				//	Create a real entity which will immediately be persisted to the database,
				//	so that it has an entity key. Saving an empty entity would do nothing, so
				//	we have to mark the entity as the active entity in the business context:
				
				var entity = context.CreateEntity (rootEntityId);

				context.SetActiveEntity (entity);
				context.SaveChanges (EntitySaveMode.IncludeEmptyActive);

				//	Load the new entity in the browser context (in order to pick it up in the
				//	list) and in the current business context (so that the user can work on it).
				
				var businessContext = this.orchestrator.DefaultBusinessContext;
				var localEntity     = businessContext.GetLocalEntity (entity);
				var localEntityKey  = businessContext.DataContext.GetNormalizedEntityKey (localEntity);
				var browserEntity   = this.browser.browserDataContext.ResolveEntity (localEntityKey);

				data.DisposeBusinessContext (context);

				this.browser.UpdateCollectionAfterInsert (browserEntity);
				this.browser.SetActiveEntityKey (localEntityKey);
				this.browser.SelectActiveEntity ();
				this.browser.RefreshScrollList ();

				return localEntity;
			}

			private Druid GetRootEntityId()
			{
				switch (this.dataSetName)
				{
					case "Customers":
						return EntityInfo<RelationEntity>.GetTypeId ();

					case "ArticleDefinitions":
						return EntityInfo<ArticleDefinitionEntity>.GetTypeId ();

					case "Documents":
					case "InvoiceDocuments":
						return EntityInfo<BusinessDocumentEntity>.GetTypeId ();

					case "WorkflowDefinitions":
						return EntityInfo<WorkflowDefinitionEntity>.GetTypeId ();
				}

				throw new System.NotImplementedException ();
			}

			private readonly BrowserViewController browser;
			private readonly DataViewOrchestrator orchestrator;
			private readonly string dataSetName;
		}
	}
}
