//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.BigList.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListController : System.IDisposable
	{
		public BrowserListController(DataViewOrchestrator orchestrator, ItemScrollList scrollList, DataSetMetadata metadata)
		{
			this.orchestrator    = orchestrator;
			this.data            = orchestrator.Data;
			this.itemScrollList  = scrollList;
			this.dataSetMetadata = metadata;

			var entityType		 = metadata.EntityTableMetadata.EntityType.Name;
			var dataContextName	 = string.Format ("Browser.DataSet={0}", entityType);
			this.dataContext     = this.data.CreateDataContext (dataContextName);

			this.suspendUpdates  = new SafeCounter ();
			this.context         = new BrowserListContext ();

			this.SetUpItemList ();
			this.AttachEventHandlers ();
		}

		
		public DataContext						DataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		public EntityKey?						SelectedEntityKey
		{
			get
			{
				return this.activeEntityKey;
			}
			set
			{
				if (this.activeEntityKey != value)
				{
					this.activeEntityKey = value;

					this.OnCurrentChanging (new CurrentChangingEventArgs (isCancelable: false));
					this.OnCurrentChanged ();
				}
			}
		}
		
		public AbstractEntity					SelectedEntity
		{
			get
			{
				return this.dataContext.ResolveEntity (this.SelectedEntityKey);
			}
		}

		public int								SelectedIndex
		{
			get
			{
				return this.itemScrollList.ActiveIndex;
			}
			set
			{
				this.SetSelectedIndex (value);
			}
		}

		public System.Predicate<AbstractEntity> Filter
		{
			get
			{
				return this.filter;
			}
			set
			{
				this.filter = value;
				this.UpdateCollection ();
			}
		}


		public void Delete(AbstractEntity entity)
		{
			if (entity.IsNull ())
			{
				return;
			}
			
			//	Archive or delete the entity, depending on the presence of an ILifetime
			//	implementation :

			var lifetime = entity as ILifetime;

			if (lifetime != null)
			{
				lifetime.IsArchive = true;
			}
			else
			{
				this.dataContext.DeleteEntity (entity);
			}

			this.dataContext.SaveChanges ();
		}
		
		public void RefreshCollection()
		{
			this.UpdateAccessor ();
			this.UpdateContentSortOrder ();
			this.UpdateCollection (reset: false);
		}

		public void RefreshScrollList(bool reset = false)
		{
			if (this.itemScrollList != null)
			{
				int newCount = this.context.ItemProvider.Count;

				int oldActive = reset ? 0 : this.context.ItemProvider.IndexOf (this.activeEntityKey);
				int newActive = oldActive < newCount ? oldActive : newCount-1;

				this.itemScrollList.RefreshContents ();
				
				using (this.suspendUpdates.Enter ())
				{
					this.SetItemScrollListSelectedIndex (newActive);
				}

				this.OnSelectedItemChange ();
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.TearDownItemList ();
			this.DetachEventHandlers ();
			this.context.Dispose ();
			this.data.DisposeDataContext (this.dataContext);
		}

		#endregion

		#region INotifyCurrentChanged Members

		public event EventHandler  CurrentChanged;

		public event EventHandler<CurrentChangingEventArgs>  CurrentChanging;

		#endregion


		private void SetUpItemList()
		{
			this.collectionEntityId = this.dataSetMetadata.EntityTableMetadata.EntityId;
			this.UpdateAccessor ();
			this.UpdateContentSortOrder ();
			
			this.itemScrollList.SetUpItemList<BrowserListItem> (this.context.ItemProvider, this.context.ItemMapper, this.context.ItemRenderer);
			
			this.itemCache = this.itemScrollList.ItemCache;

			this.itemScrollList.ActiveIndexChanged += this.HandleItemListActiveIndexChanged;
		}

		private void TearDownItemList()
		{
			this.itemScrollList.ActiveIndexChanged -= this.HandleItemListActiveIndexChanged;
		}

		private void SetSelectedIndex(int index)
		{
			if (index >= this.context.ItemProvider.Count)
			{
				index = this.context.ItemProvider.Count-1;
			}

			if (index < 0)
			{
				this.SelectedEntityKey = null;
				this.SetItemScrollListSelectedIndex (-1);
			}
			else
			{
				this.SelectedEntityKey = this.GetEntityKey (index);
				this.SetItemScrollListSelectedIndex (index);
			}
		}

		private EntityKey? GetEntityKey(int index)
		{
			if (index < 0)
			{
				return null;
			}
			else
			{
				return this.itemCache.GetItemData (index).GetData<BrowserListItem> ().EntityKey;
			}
		}

		private void SetItemScrollListSelectedIndex(int index)
		{
			this.itemScrollList.Selection.Select (index, ItemSelection.Select);
			this.itemScrollList.ActiveIndex = index;
		}

		private DataSetAccessor GetContentAccessor()
		{
			var component = this.data.GetComponent<DataSetGetter> ();
			var accessor  = component.ResolveAccessor (this.dataSetMetadata);

			return accessor;
		}

		private void UpdateContentSortOrder()
		{
			// TODO...
		}

		private void AttachEventHandlers()
		{
			this.dataContext.EntityChanged  += this.HandleDataContextEntityChanged;
			this.orchestrator.SavingChanges += this.HandleOrchestratorSavingChanges;
		}

		private void DetachEventHandlers()
		{
			this.orchestrator.SavingChanges -= this.HandleOrchestratorSavingChanges;
			this.dataContext.EntityChanged  -= this.HandleDataContextEntityChanged;
		}


		private void HandleOrchestratorSavingChanges(object sender, CancelEventArgs e)
		{
			this.UpdateAccessor ();
			this.UpdateContentSortOrder ();
		}

		private void HandleItemListActiveIndexChanged(object sender, ItemListIndexEventArgs e)
		{
			if (this.suspendUpdates.IsZero)
			{
				this.OnSelectedItemChange ();
			}
		}
		
		private void HandleDataContextEntityChanged(object sender, EntityChangedEventArgs e)
		{
			this.UpdateCollection (reset: false);
		}

		private void UpdateAccessor()
		{
			this.context.SetAccessor (this.GetContentAccessor ());
		}
		
		private void UpdateCollection(bool reset = true)
		{
			if (this.context != null)
			{
				this.context.ItemProvider.Reset ();
				this.RefreshScrollList (reset);
			}
		}

		private void OnSelectedItemChange()
		{
			int active    = this.itemScrollList.ActiveIndex;
			var entityKey = this.GetEntityKey (active);

			this.SelectedEntityKey = entityKey;
			this.SelectedItemChange.Raise (this);
		}

		private void OnCurrentChanged()
		{
			this.CurrentChanged.Raise (this);
		}

		private void OnCurrentChanging(CurrentChangingEventArgs e)
		{
			this.CurrentChanging.Raise (this, e);
		}


		public event EventHandler				SelectedItemChange;

		private readonly DataViewOrchestrator	orchestrator;
		private readonly CoreData				data;
		private readonly ItemScrollList			itemScrollList;
		private readonly DataSetMetadata		dataSetMetadata;
		private readonly DataContext			dataContext;
		private readonly SafeCounter			suspendUpdates;

		private ItemCache						itemCache;

		private System.Predicate<AbstractEntity> filter;

		private BrowserListContext				context;
		private Druid							collectionEntityId;
		
		private EntityKey?						activeEntityKey;
	}
}
