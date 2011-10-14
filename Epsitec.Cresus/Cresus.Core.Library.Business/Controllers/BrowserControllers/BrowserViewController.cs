//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Data.Extraction;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	/// <summary>
	/// The <c>BrowserViewController</c> manages the browser view, which lists
	/// entities in a compact form (this is used as the entity selector in the
	/// UI).
	/// </summary>
	public sealed partial class BrowserViewController : CoreViewController, INotifyCurrentChanged, IWidgetUpdater
	{
		public BrowserViewController(DataViewOrchestrator orchestrator)
			: base ("Browser", orchestrator)
		{
			this.data = orchestrator.Data;
			this.Orchestrator.Host.RegisterComponent (this);
		}


		public FrameBox							SettingsPanel
		{
			get
			{
				return this.settingsPanel;
			}
		}

		public string							DataSetName
		{
			get
			{
				return this.dataSetName;
			}
		}


		/// <summary>
		/// Selects the specified data set.
		/// </summary>
		/// <param name="dataSetName">Name of the data set.</param>
		public void SelectDataSet(string dataSetName)
		{
			if (this.dataSetName != dataSetName)
			{
				this.Orchestrator.ClearActiveEntity ();

				this.DisposeBrowserDataContext ();
				this.dataSetName = dataSetName;
				this.CreateBrowserDataContext ();

				this.SetContentsBasedOnDataSet ();
				this.OnDataSetSelected ();
			}
		}

		/// <summary>
		/// Selects the specified entity in the list.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void Select(AbstractEntity entity)
		{
			if ((this.collection == null) ||
				(this.scrollList == null))
			{
				return;
			}

			//	The specified entity does most probably not belong to our data context,
			//	therefore we would not find it in the collection. Look for it based on
			//	its key :

			var entityKey = this.data.FindEntityKey (entity);
			int index     = this.collection.GetIndex (entityKey);

			this.scrollList.SelectedItemIndex = index;
		}

		/// <summary>
		/// Adds a new entity to the list. This will start an interactive creation if
		/// there is an associated creation controller.
		/// </summary>
		public void AddNewEntity()
		{
			using (var creator = new BrowserViewController.ItemCreator (this))
			{
				creator.Create ();
			}
		}

		/// <summary>
		/// Removes the active entity from the list. This will either archive the entity
		/// or delete it from the database, if it does not implement <see cref="ILifetime"/>.
		/// </summary>
		public void RemoveActiveEntity()
		{
			int active = this.scrollList.SelectedItemIndex;
			var entity = this.collection.RemoveAt (active);

			if (entity != null)
			{
				//	Remove the entity from the scroll-list now that we have removed it from
				//	our internal collection; make sure nobody is still using it by clearing
				//	the active entity :

				this.suspendUpdates++;
				this.scrollList.Items.RemoveAt (active);
				this.Orchestrator.ClearActiveEntity ();
				this.suspendUpdates--;

				//	Archive or delete the entity, depending on the presence of a ILifetime
				//	implementation :

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

				this.SelectItem (active);
			}
		}
		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);

			var frame = new FrameBox ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
			};

			this.CreateUISettingsPanel (frame);
			this.CreateUIScrollList (frame);
		}


		private void CreateUISettingsPanel(FrameBox frame)
		{
			this.settingsPanel = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Top,
				PreferredHeight = 28,
			};
		}
		
		private void CreateUIScrollList(FrameBox frame)
		{
			var listFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
			};

			this.scrollList = new ScrollList ()
			{
				Parent = listFrame,
				Anchor = AnchorStyles.All,
				ScrollListStyle = ScrollListStyle.Standard,
				Margins = new Common.Drawing.Margins (-1, -1, -1, -1),
			};

			this.scrollList.SelectedItemChanged += this.HandleScrollListSelectedItemChanged;
		}
		
		
		private void CreateBrowserDataContext()
		{
			this.browserDataContext = this.data.CreateDataContext (string.Format ("Browser.DataSet={0}", this.DataSetName));
			this.collection         = new BrowserList (this.browserDataContext);

			this.scrollList.Items.ValueConverter   = this.collection.ConvertBrowserListItemToString;
			this.browserDataContext.EntityChanged += this.HandleDataContextEntityChanged;
		}

		private void DisposeBrowserDataContext()
		{
			if (this.browserDataContext != null)
			{
				this.browserDataContext.EntityChanged -= this.HandleDataContextEntityChanged;
				this.scrollList.Items.ValueConverter   = null;

				this.collection.Dispose ();
				this.data.DisposeDataContext (this.browserDataContext);
				
				this.collection         = null;
				this.browserDataContext = null;
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

		private void HandleScrollListSelectedItemChanged(object sender)
		{
			if (this.suspendUpdates == 0)
			{
				this.NotifySelectedItemChange ();
			}
		}

		
		private void SetContentsBasedOnDataSet()
		{
			var component = this.data.GetComponent<DataSetGetter> ();
			var getter = component.ResolveDataSet (this.dataSetName);
			
			this.SetContents (getter, component.GetRootEntityId (this.dataSetName));
		}

		private void SetContents(DataSetCollectionGetter collectionGetter, Druid entityId)
		{
			//	When switching to some other contents, the browser first has to ensure that the
			//	UI no longer has an actively selected entity; clearing the active entity will
			//	also make sure that any changes will be automatically persisted:

			this.Orchestrator.ClearActiveEntity ();

			this.collectionGetter    = collectionGetter;
			this.collectionEntityId  = entityId;
			this.extractor           = null;
			this.extractedCollection = null;

			if (EntityInfo<CustomerEntity>.GetTypeId () == entityId)
			{
				this.extractor =
					new EntityDataExtractor (
						new EntityDataMetadataRecorder<CustomerEntity> ()
							.Column (x => x.MainRelation.DefaultMailContact.Address.Location.PostalCode)
							.Column (x => x.MainRelation.DefaultMailContact.Address.Location.Name)
							.Column (x => x.MainRelation.DefaultMailContact.Address.Street.StreetName)
						.GetMetadata ());

				this.extractedCollection = this.extractor.CreateCollection (EntityDataRowComparer.Instance);
			}

			this.UpdateCollection ();
		}

		private bool SelectActiveEntity()
		{
			if (this.activeEntityKey.HasValue)
			{
				var activeEntityKey       = this.activeEntityKey.Value;
				var navigationPathElement = new BrowserNavigationPathElement (this, activeEntityKey);

				this.Orchestrator.SetActiveEntity (activeEntityKey, navigationPathElement);

				return true;
			}
			else
			{
				this.Orchestrator.ClearActiveEntity ();
				return false;
			}
		}

		private bool ReselectActiveEntity()
		{
			if (this.activeEntityKey.HasValue)
			{
				var activeEntityKey       = this.activeEntityKey.Value;
				var navigationPathElement = new BrowserNavigationPathElement (this, activeEntityKey);
				
				this.Orchestrator.ResetActiveEntity (activeEntityKey, navigationPathElement);

				return true;
			}
			else
			{
				this.Orchestrator.ClearActiveEntity ();
				return false;
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

		private void InsertIntoCollection(AbstractEntity entity)
		{
			this.collection.Add (entity);

			if (this.extractor != null)
			{
				this.extractor.Insert (entity);
			}
		}

		private void NotifySelectedItemChange()
		{
			int active    = this.scrollList.SelectedItemIndex;
			var entityKey = this.collection == null ? null : this.collection.GetEntityKey (active);

//-			System.Diagnostics.Debug.WriteLine (string.Format ("SelectedItemChanged : old key = {0} / new key = {1}", this.activeEntityKey, entityKey));

			this.SetActiveEntityKey (entityKey);
		}

		private void SelectItem(int index)
		{
			if (index >= this.collection.Count)
			{
				index = this.collection.Count-1;
			}

			if (index < 0)
			{
				this.SetActiveEntityKey (null);
				this.scrollList.SelectedItemIndex = -1;
			}
			else
			{
				this.SetActiveEntityKey (this.collection.GetEntityKey (index));
				this.scrollList.SelectedItemIndex = index;
			}

			this.SelectActiveEntity ();
		}

		private void SetActiveEntityKey(EntityKey? entityKey)
		{
			if (this.activeEntityKey != entityKey)
			{
				this.activeEntityKey = entityKey;
				this.OnCurrentChanging (new CurrentChangingEventArgs (isCancelable: false));
				this.OnCurrentChanged ();
			}
		}

		private void OnCurrentChanged()
		{
			var handler = this.CurrentChanged;

			if (handler != null)
			{
				handler (this);
			}

			this.SelectActiveEntity ();
		}

		private void OnCurrentChanging(CurrentChangingEventArgs e)
		{
			var handler = this.CurrentChanging;

			if (handler != null)
			{
				handler (this, e);
			}
		}

		private void OnDataSetSelected()
		{
			this.Orchestrator.MainViewController.CommandContext.UpdateCommandStates (this);

			var handler = this.DataSetSelected;

			if (handler != null)
			{
				handler (this);
			}
		}

		private void RefreshScrollList(bool reset = false)
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

				this.NotifySelectedItemChange ();
			}
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			if (this.collection != null)
			{
//				this.collection.Invalidate ();
//				this.RefreshScrollList ();
			}
		}

		#endregion

		#region INotifyCurrentChanged Members

		public event EventHandler  CurrentChanged;

		public event EventHandler<CurrentChangingEventArgs>  CurrentChanging;

		#endregion


		public event EventHandler				DataSetSelected;

		private readonly CoreData				data;
		
		private BrowserList						collection;
		private string							dataSetName;
		private DataContext						browserDataContext;

		private EntityDataExtractor             extractor;
		private EntityDataCollection			extractedCollection;

		private DataSetCollectionGetter			collectionGetter;
		private Druid							collectionEntityId;
		private int								suspendUpdates;

		private ScrollList						scrollList;
		private EntityKey?						activeEntityKey;
		private FrameBox						settingsPanel;
	}
}
