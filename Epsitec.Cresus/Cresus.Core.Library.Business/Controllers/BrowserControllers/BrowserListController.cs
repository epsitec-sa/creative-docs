//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.BigList.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Data.Extraction;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListController : System.IDisposable
	{
		public BrowserListController(CoreData data, ItemScrollList scrollList, System.Type dataSetType)
		{
			this.data           = data;
			this.itemScrollList = scrollList;
			this.dataSetType    = dataSetType;
			this.dataContext    = this.data.CreateDataContext (string.Format ("Browser.DataSet={0}", this.dataSetType.Name));
			this.collection     = new BrowserList (this.dataContext);
			this.suspendUpdates = new SafeCounter ();
			this.context        = new BrowserListContext ();

			this.SetUpItemList ();
			this.AttachEventHandlers ();

			this.SetContentsBasedOnDataSet ();
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
//#				return this.scrollList.SelectedItemIndex;
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


		#region IDisposable Members

		public void Dispose()
		{
			this.TearDownItemList ();
			this.DetachEventHandlers ();
			this.collection.Dispose ();
			this.data.DisposeDataContext (this.dataContext);
		}

		#endregion

		#region INotifyCurrentChanged Members

		public event EventHandler  CurrentChanged;

		public event EventHandler<CurrentChangingEventArgs>  CurrentChanging;

		#endregion


		private void SetUpItemList()
		{
			this.itemProvider = new BrowserListItemProvider (this.collection);
			this.itemMapper   = new BrowserListItemMapper ();
			this.itemRenderer = new BrowserListItemRenderer (this.context);
			
			this.itemScrollList.SetUpItemList<BrowserListItem> (this.itemProvider, this.itemMapper, this.itemRenderer);
			
			this.itemCache = this.itemScrollList.ItemCache;

			this.itemScrollList.ActiveIndexChanged += this.HandleItemListActiveIndexChanged;
		}

		private void TearDownItemList()
		{
			this.itemScrollList.ActiveIndexChanged -= this.HandleItemListActiveIndexChanged;
		}

		private void SetSelectedIndex(int index)
		{
			if (index >= this.itemProvider.Count)
			{
				index = this.itemProvider.Count-1;
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

		private EntityKey GetEntityKey(int index)
		{
			return this.itemCache.GetItemData (index).GetData<BrowserListItem> ().EntityKey;
		}

		private void SetItemScrollListSelectedIndex(int index)
		{
			this.itemScrollList.Selection.Select (index, ItemSelection.Select);
			this.itemScrollList.ActiveIndex = index;
		}

		private void SetContentsBasedOnDataSet()
		{
			this.SetContents (this.GetContentGetter (), EntityInfo.GetTypeId (this.dataSetType));
		}

		private DataSetCollectionGetter GetContentGetter()
		{
			var component = this.data.GetComponent<DataSetGetter> ();
			var getter    = component.ResolveDataSet (this.dataSetType);
			
			return getter;
		}

		private void SetContents(DataSetCollectionGetter collectionGetter, Druid entityId)
		{
			this.collectionGetter   = collectionGetter;
			this.collectionEntityId = entityId;

			this.SetUpContentExtractor ();
			
			this.UpdateCollection ();
		}

		private void SetUpContentExtractor()
		{
			if (EntityInfo<CustomerEntity>.GetTypeId () == this.collectionEntityId)
			{
				this.extractor =
						new EntityDataExtractor (
						new EntityDataMetadataRecorder<CustomerEntity> ()
							.Column (x => x.MainRelation.DefaultMailContact.Location.PostalCode)
							.Column (x => x.MainRelation.DefaultMailContact.Location.Name)
							.Column (x => x.MainRelation.DefaultMailContact.StreetName)
							.Column (x => x.MainRelation.DefaultMailContact.HouseNumber)
						.GetMetadata ());

				this.extractedCollection = this.extractor.CreateCollection (EntityDataRowComparer.Instance);
				
				return;
			}

			this.extractor           = null;
			this.extractedCollection = null;
		}

		private void AttachEventHandlers()
		{
			this.dataContext.EntityChanged += this.HandleDataContextEntityChanged;
		}

		private void DetachEventHandlers()
		{
			this.dataContext.EntityChanged -= this.HandleDataContextEntityChanged;
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

		private AbstractEntity[] GetCollectionEntities()
		{
			if (this.collectionGetter == null)
			{
				return new AbstractEntity[0];
			}
			else
			{
				if (this.filter == null)
				{
					return this.collectionGetter (this.dataContext).ToArray ();
				}
				else
				{
					return this.collectionGetter (this.dataContext).Where (x => this.filter (x)).ToArray ();
				}
			}
		}

		private void UpdateCollection(bool reset = true)
		{
			if (this.collectionGetter == null)
			{
				return;
			}

			if (this.extractedCollection != null)
			{
				this.extractor.Fill (this.GetCollectionEntities ());
				this.collection.ClearAndAddRange (this.extractedCollection.Rows.Select (x => x.Entity));
			}
			else
			{
				this.collection.ClearAndAddRange (this.GetCollectionEntities ());
			}

			this.RefreshScrollList (reset);
		}

		public void RefreshCollection()
		{
			this.UpdateCollection (reset: false);
		}

		public void RefreshScrollList(bool reset = false)
		{
			if (this.itemScrollList != null)
			{
				int newCount = this.itemProvider.Count;

				int oldActive = reset ? 0 : this.itemProvider.IndexOf (this.activeEntityKey);
				int newActive = oldActive < newCount ? oldActive : newCount-1;

				this.itemScrollList.RefreshContents ();
				
				using (this.suspendUpdates.Enter ())
				{
					this.SetItemScrollListSelectedIndex (newActive);
				}

				this.OnSelectedItemChange ();
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

		private readonly CoreData				data;
		private readonly ItemScrollList			itemScrollList;
		private readonly BrowserList			collection;
		private readonly System.Type			dataSetType;
		private readonly DataContext			dataContext;
		private readonly SafeCounter			suspendUpdates;

		private BrowserListItemProvider			itemProvider;
		private BrowserListItemMapper			itemMapper;
		private BrowserListItemRenderer			itemRenderer;
		private ItemCache						itemCache;

		private System.Predicate<AbstractEntity> filter;
		private EntityDataExtractor             extractor;
		private EntityDataCollection			extractedCollection;

		private BrowserListContext				context;
		private DataSetCollectionGetter			collectionGetter;
		private Druid							collectionEntityId;
		
		private EntityKey?						activeEntityKey;
	}
}
