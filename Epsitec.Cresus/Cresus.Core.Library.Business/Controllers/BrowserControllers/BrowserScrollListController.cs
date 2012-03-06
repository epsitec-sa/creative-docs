//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
	public class BrowserScrollListController : System.IDisposable
	{
		public BrowserScrollListController(CoreData data, ScrollList scrollList, System.Type dataSetType, System.Predicate<AbstractEntity> filter = null)
		{
			this.data        = data;
			this.scrollList  = scrollList;
			this.dataSetType = dataSetType;
			this.dataContext = this.data.CreateDataContext (string.Format ("Browser.DataSet={0}", this.dataSetType.Name));
			this.collection  = new BrowserList (this.dataContext);
			this.filter      = filter;

			this.scrollList.Items.ValueConverter = this.collection.ConvertBrowserListItemToString;

			this.scrollList.SelectedItemChanged += this.HandleScrollListSelectedItemChanged;
			this.dataContext.EntityChanged      += this.HandleDataContextEntityChanged;

			this.SetContentsBasedOnDataSet ();
		}

		
		public DataContext						DataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		public BrowserList						Collection
		{
			get
			{
				return this.collection;
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
			set
			{
				this.SelectedEntityKey = this.data.FindEntityKey (value);
			}
		}

		public int								SelectedIndex
		{
			get
			{
				return this.scrollList.SelectedItemIndex;
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


		public void Remove(AbstractEntity entity)
		{
			int index = this.collection.IndexOf (entity);

			if (index < 0)
			{
				return;
			}

			this.suspendUpdates++;
			this.collection.RemoveAt (index);
			this.scrollList.Items.RemoveAt (index);
			this.suspendUpdates--;
		}

		public void Delete(AbstractEntity entity)
		{
			if (entity.IsNull ())
			{
				return;
			}

			this.Remove (entity);

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

		public void InsertIntoCollection(AbstractEntity entity)
		{
			this.collection.Add (entity);

			if (this.extractor != null)
			{
				this.extractor.Insert (entity);
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.scrollList.SelectedItemChanged -= this.HandleScrollListSelectedItemChanged;
			this.dataContext.EntityChanged -= this.HandleDataContextEntityChanged;
			this.scrollList.Items.ValueConverter   = null;

			this.collection.Dispose ();
			this.data.DisposeDataContext (this.dataContext);
		}

		#endregion

		#region INotifyCurrentChanged Members

		public event EventHandler  CurrentChanged;

		public event EventHandler<CurrentChangingEventArgs>  CurrentChanging;

		#endregion


		private void SetSelectedIndex(int index)
		{
			if (index >= this.collection.Count)
			{
				index = this.collection.Count-1;
			}

			if (index < 0)
			{
				this.SelectedEntityKey = null;
				this.scrollList.SelectedItemIndex = -1;
			}
			else
			{
				this.SelectedEntityKey = this.collection.GetEntityKey (index);
				this.scrollList.SelectedItemIndex = index;
			}
		}

		private void SetContentsBasedOnDataSet()
		{
			var component = this.data.GetComponent<DataSetGetter> ();
			var getter    = component.ResolveDataSet (this.dataSetType);

			this.SetContents (getter, EntityInfo.GetTypeId (this.dataSetType));
		}

		private void SetContents(DataSetCollectionGetter collectionGetter, Druid entityId)
		{
			this.collectionGetter    = collectionGetter;
			this.collectionEntityId  = entityId;

			this.SetupContentExtractor ();
			
			this.UpdateCollection ();
		}

		private void SetupContentExtractor()
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

		private void HandleScrollListSelectedItemChanged(object sender)
		{
			if (this.suspendUpdates == 0)
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

		public void RefreshScrollList(bool reset = false)
		{
			if ((this.scrollList != null) &&
				(this.collection != null))
			{
				int newCount = this.collection == null ? 0 : this.collection.Count;

				int oldActive = reset ? 0 : this.collection.IndexOf (this.activeEntityKey);
				int newActive = oldActive < newCount ? oldActive : newCount-1;

				this.suspendUpdates++;
				this.scrollList.Items.Clear ();
				this.scrollList.Items.AddRange (this.collection);
				this.scrollList.SelectedItemIndex = newActive;
				this.suspendUpdates--;

				this.OnSelectedItemChange ();
			}
		}

		private void OnSelectedItemChange()
		{
			int active    = this.scrollList.SelectedItemIndex;
			var entityKey = this.collection.GetEntityKey (active);

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
		private readonly ScrollList				scrollList;
		private readonly BrowserList			collection;
		private readonly System.Type			dataSetType;
		private readonly DataContext			dataContext;

		private System.Predicate<AbstractEntity> filter;
		private EntityDataExtractor             extractor;
		private EntityDataCollection			extractedCollection;

		private DataSetCollectionGetter			collectionGetter;
		private Druid							collectionEntityId;
		
		private EntityKey?						activeEntityKey;

		private int								suspendUpdates;
	}
}
