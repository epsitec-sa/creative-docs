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

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserScrollListController : System.IDisposable
	{
		public BrowserScrollListController(CoreData data, ScrollList scrollList, string dataSetName)
		{
			this.data = data;
			this.scrollList = scrollList;
			this.dataSetName = dataSetName;

			this.browserDataContext = this.data.CreateDataContext (string.Format ("Browser.DataSet={0}", this.dataSetName));
			this.collection         = new BrowserList (this.browserDataContext);

			this.scrollList.Items.ValueConverter   = this.collection.ConvertBrowserListItemToString;
			this.scrollList.SelectedItemChanged += this.HandleScrollListSelectedItemChanged;
			this.browserDataContext.EntityChanged += this.HandleDataContextEntityChanged;
			this.SetContentsBasedOnDataSet ();
		}

		public DataContext DataContext
		{
			get
			{
				return this.browserDataContext;
			}
		}

		public BrowserList Collection
		{
			get
			{
				return this.collection;
			}
		}

		public void Delete(AbstractEntity entity)
		{
			var lifetime = entity as ILifetime;

			if (lifetime != null)
			{
				lifetime.IsArchive = true;
			}
			else
			{
				this.browserDataContext.DeleteEntity (entity);
			}

			this.browserDataContext.SaveChanges ();
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
			this.browserDataContext.EntityChanged -= this.HandleDataContextEntityChanged;
			this.scrollList.Items.ValueConverter   = null;

			this.collection.Dispose ();
			this.data.DisposeDataContext (this.browserDataContext);
		}

		#endregion

		private void SetContentsBasedOnDataSet()
		{
			var component = this.data.GetComponent<DataSetGetter> ();
			var getter = component.ResolveDataSet (this.dataSetName);

			this.SetContents (getter, component.GetRootEntityId (this.dataSetName));
		}

		private void SetContents(DataSetCollectionGetter collectionGetter, Druid entityId)
		{
			this.collectionGetter    = collectionGetter;
			this.collectionEntityId  = entityId;
			this.extractor           = null;
			this.extractedCollection = null;

			if (EntityInfo<CustomerEntity>.GetTypeId () == entityId)
			{
				this.extractor =
					new EntityDataExtractor (
						new EntityDataMetadataRecorder<CustomerEntity> ()
							.Column (x => x.MainRelation.DefaultMailContact.Location.PostalCode)
							.Column (x => x.MainRelation.DefaultMailContact.Location.Name)
							.Column (x => x.MainRelation.DefaultMailContact.StreetName)
						.GetMetadata ());

				this.extractedCollection = this.extractor.CreateCollection (EntityDataRowComparer.Instance);
			}

			this.UpdateCollection ();
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
			if ((this.collection != null) &&
					(this.scrollList != null))
			{
				this.UpdateCollection (reset: false);
			}
		}

		private AbstractEntity[] GetCollectionEntities()
		{
			if (this.collectionGetter == null)
			{
				return new AbstractEntity[0];
			}
			else
			{
				return this.collectionGetter (this.browserDataContext).ToArray ();
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

				int oldActive = reset ? 0 : this.collection.GetIndex (this.activeEntityKey);
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
			this.SelectedItemChange.Raise (this);
		}


		public event EventHandler				SelectedItemChange;

		private readonly CoreData				data;
		private readonly ScrollList				scrollList;
		private readonly BrowserList			collection;
		private readonly string					dataSetName;
		private readonly DataContext			browserDataContext;

		private EntityDataExtractor             extractor;
		private EntityDataCollection			extractedCollection;

		private DataSetCollectionGetter			collectionGetter;
		private Druid							collectionEntityId;
		
		private EntityKey?						activeEntityKey;

		private int								suspendUpdates;
	}
}
