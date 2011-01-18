//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	/// <summary>
	/// The <c>AbstractResourceAccessor</c> class is the base class used by
	/// all resource accessors found in the <c>Epsitec.Common.Support</c>
	/// namespace.
	/// </summary>
	public abstract class AbstractResourceAccessor : DependencyObject, IResourceAccessor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractResourceAccessor"/> class.
		/// </summary>
		protected AbstractResourceAccessor()
		{
			this.items = new CultureMapList (this);
			this.items.CollectionChanged += this.HandleItemsCollectionChanged;
			this.dirtyItems = new Dictionary<CultureMap, bool> ();
		}

		/// <summary>
		/// Gets the resource manager associated with this accessor.
		/// </summary>
		/// <value>The resource manager.</value>
		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force a module merge when
		/// persisting an item.
		/// </summary>
		/// <value><c>true</c> to force a module merge; otherwise, <c>false</c>.</value>
		public bool ForceModuleMerge
		{
			get
			{
				return this.forceModuleMerge;
			}
			set
			{
				this.forceModuleMerge = value;
			}
		}

		/// <summary>
		/// Loads resources from the specified resource manager. The resource
		/// manager will be used for all upcoming accesses.
		/// </summary>
		/// <param name="manager">The resource manager.</param>
		public abstract void Load(ResourceManager manager);

		/// <summary>
		/// Saves the resources.
		/// </summary>
		/// <param name="saverCallback">The saver callback.</param>
		public void Save(ResourceBundleSaver saverCallback)
		{
			ResourceManager     manager        = this.ResourceManager;
			ResourceManagerPool pool           = manager.Pool;
			string              bundleName     = this.GetBundleName ();
			int                 bundleModuleId = manager.DefaultModuleId;

			//	Find all resource bundles which match exactly what this accessor
			//	usually works with, and save them.

			foreach (ResourceBundle bundle in pool.FindAllLoadedBundles (
				
				delegate (ResourceBundle candidate)
				{
					if ((candidate.ResourceManager == manager) &&
						(candidate.Name == bundleName) &&
						(candidate.Module.Id == bundleModuleId) &&
						(candidate.ResourceLevel != ResourceLevel.Merged) &&
						((candidate.IsEmpty == false) || (candidate.FieldsChangedCount > 0)))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				
				))
			{
				saverCallback (manager, bundle, ResourceSetMode.Write);
			}
		}

		/// <summary>
		/// Resets the specified field to its original value.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="container">The data record.</param>
		/// <param name="fieldId">The field id.</param>
		public void ResetToOriginalValue(CultureMap item, StructuredData container, Druid fieldId)
		{
			if ((this.BasedOnPatchModule) &&
				(item.Source == CultureMapSource.DynamicMerge))
			{
				this.ResetToOriginal (item, container, fieldId);

				bool usesOriginalData;
				container.GetValue (fieldId, out usesOriginalData);

				System.Diagnostics.Debug.Assert (usesOriginalData);
			}
		}

		#region IResourceAccessor Members

		/// <summary>
		/// Gets the collection of <see cref="CultureMap"/> items.
		/// </summary>
		/// <value>The collection of <see cref="CultureMap"/> items.</value>
		public CultureMapList Collection
		{
			get
			{
				return this.items;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this accessor is taking data from
		/// resources based on a patch module.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the data are based on a patch module; otherwise, <c>false</c>.
		/// </value>
		public bool BasedOnPatchModule
		{
			get
			{
				return this.ResourceManager.BasedOnPatchModule;
			}
		}

		public IDataBroker GetDataBroker(StructuredData container, Druid fieldId)
		{
			return this.GetDataBroker (container, fieldId.ToString ());
		}

		/// <summary>
		/// Gets the data broker associated with the specified field. Usually,
		/// this is only meaningful if the field defines a collection of
		/// <see cref="StructuredData"/> items.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="fieldId">The id for the field in the specified container.</param>
		/// <returns>The data broker or <c>null</c>.</returns>
		public virtual IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			return null;
		}

		/// <summary>
		/// Gets a list of all available cultures for the specified accessor.
		/// </summary>
		/// <returns>A list of two letter ISO language names.</returns>
		public IList<string> GetAvailableCultures()
		{
			List<string> list = new List<string> ();
			string[] ids = this.resourceManager.GetBundleIds (this.GetBundleName (), ResourceLevel.All);

			foreach (string id in ids)
			{
				list.Add (Resources.ExtractSuffix (id));
			}

			list.Sort ();

			return list;
		}

		/// <summary>
		/// Gets a value indicating whether this accessor contains changes.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this accessor contains changes; otherwise, <c>false</c>.
		/// </value>
		public bool ContainsChanges
		{
			get
			{
				return this.dirtyItems.Count > 0;
			}
		}

		/// <summary>
		/// Creates a new item which can then be added to the collection.
		/// </summary>
		/// <returns>A new <see cref="CultureMap"/> item.</returns>
		public virtual CultureMap CreateItem()
		{
			CultureMap item = new CultureMap (this, this.CreateId (), this.GetCultureMapSource (null));
			item.IsNewItem = true;
			return item;
		}

		/// <summary>
		/// Persists the changes to the underlying data store.
		/// </summary>
		/// <returns>
		/// The number of items which have been persisted.
		/// </returns>
		public virtual int PersistChanges()
		{
			List<CultureMap> list = new List<CultureMap> (this.dirtyItems.Keys);
			
			this.dirtyItems.Clear ();
			
			foreach (CultureMap item in list)
			{
				if (this.Collection.Contains (item))
				{
					this.PersistItem (item);
					item.IsNewItem = false;
				}
				else
				{
					this.DeleteItem (item);
				}
			}

			if (list.Count > 0)
			{
				this.ResourceManager.ClearMergedBundlesFromBundleCache ();
			}

			return list.Count;
		}

		/// <summary>
		/// Reverts the changes applied to the accessor.
		/// </summary>
		/// <returns>
		/// The number of items which have been reverted.
		/// </returns>
		public virtual int RevertChanges()
		{
			List<CultureMap> list = new List<CultureMap> (this.dirtyItems.Keys);

			this.dirtyItems.Clear ();

			using (this.SuspendNotifications ())
			{
				foreach (CultureMap item in list)
				{
					item.ClearCultureData ();
					item.IsRefreshNeeded = true;
					
					if (this.Collection.Contains (item))
					{
						if (item.IsNewItem)
						{
							this.Collection.Remove (item);
						}
					}
					else
					{
						if (!item.IsNewItem)
						{
							this.Collection.Add (item);
						}
					}
				}
			}

			return list.Count;
		}

		/// <summary>
		/// Notifies the resource accessor that the specified item changed.
		/// </summary>
		/// <param name="item">The item which was modified.</param>
		/// <param name="container">The container which changed, if any.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		public virtual void NotifyItemChanged(CultureMap item, StructuredData container, DependencyPropertyChangedEventArgs e)
		{
			if (this.suspendNotifications == 0)
			{
				this.dirtyItems[item] = true;

				if ((this.BasedOnPatchModule) &&
					(item.Source == CultureMapSource.ReferenceModule))
				{
					item.Source = CultureMapSource.DynamicMerge;
				}
			}
		}

		/// <summary>
		/// Notifies the resource accessor that the specified culture data was
		/// just cleared.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <param name="data">The data which is cleared.</param>
		public virtual void NotifyCultureDataCleared(CultureMap item, string twoLetterISOLanguageName, StructuredData data)
		{
		}

		/// <summary>
		/// Loads the data for the specified culture into an existing item.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>
		/// The data loaded from the resources which was stored in the specified item.
		/// </returns>
		public abstract StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName);

		#endregion

		/// <summary>
		/// Creates a unique id, making sure there are no collisions.
		/// </summary>
		/// <param name="collection">The collection of <see cref="CultureMap"/> items (or <c>null</c>)
		/// which contains ids that are already in use.</param>
		/// <param name="bundles">The bundle(s) where to look for existing ids.</param>
		/// <returns>A unique id.</returns>
		internal static Druid CreateId(IEnumerable<CultureMap> collection, ResourceManager manager, params ResourceBundle[] bundles)
		{
			return AbstractResourceAccessor.CreateId (collection, manager.DefaultModuleId, manager.PatchDepth, bundles);
		}

		/// <summary>
		/// Creates a unique id, making sure there are no collisions.
		/// </summary>
		/// <param name="collection">The collection of <see cref="CultureMap"/> items (or <c>null</c>)
		/// which contains ids that are already in use.</param>
		/// <param name="moduleId">The module id.</param>
		/// <param name="patchDepth">The patch depth (<code>0</code> means root reference module).</param>
		/// <param name="bundles">The bundle(s) where to look for existing ids.</param>
		/// <returns>A unique id.</returns>
		internal static Druid CreateId(IEnumerable<CultureMap> collection, int moduleId, int patchDepth, ResourceBundle[] bundles)
		{
			//	Derive the developer id from the value stored in the global properties
			//	repository. This must have been initialized by the user application,
			//	usually the Designer.

			object devIdValue = Support.Globals.Properties.GetProperty (AbstractResourceAccessor.DeveloperIdPropertyName);

			int devId    = -1;
			int localId  = -1;

			if (UndefinedValue.IsUndefinedValue (devIdValue))
			{
				throw new System.InvalidOperationException ("Undefined developer id");
			}

			if ((patchDepth < 0) ||
				(patchDepth >= Druid.DeveloperIdMultiplier))
			{
				throw new System.InvalidOperationException ("Invalid patch depth");
			}

			devId = (int) devIdValue;

			if (!Druid.IsValidDeveloperAndPatchId (devId))
			{
				throw new System.InvalidOperationException ("Invalid developer id");
			}

			devId = patchDepth + Druid.DeveloperIdMultiplier * devId;

			if (!Druid.IsValidDeveloperAndPatchId (devId))
			{
				throw new System.InvalidOperationException ("Invalid developer id");
			}

			//	Locate the largest local id for the active developer in
			//	the specified bundle(s) :

			if (bundles != null)
			{
				foreach (ResourceBundle bundle in bundles)
				{
					if (bundle == null)
					{
						continue;
					}

					if (moduleId == -1)
					{
						moduleId = bundle.Module.Id;
					}

					System.Diagnostics.Debug.Assert (moduleId == bundle.Module.Id);

					foreach (ResourceBundle.Field field in bundle.Fields)
					{
						Druid id = field.Id;

						System.Diagnostics.Debug.Assert (id.IsValid);

						if (id.DeveloperAndPatchLevel == devId)
						{
							localId = System.Math.Max (localId, id.Local);
						}
					}
				}
			}

			//	If the caller provided a collection of items, browse it and
			//	check if there are higher local ids in there :

			if (collection != null)
			{
				foreach (CultureMap item in collection)
				{
					Druid id = item.Id;

					System.Diagnostics.Debug.Assert (id.IsValid);

					if (id.DeveloperAndPatchLevel == devId)
					{
						localId = System.Math.Max (localId, id.Local);
					}
				}
			}

			System.Diagnostics.Debug.Assert (moduleId >= 0);
			
			return new Druid (moduleId, devId, localId+1);
		}

		/// <summary>
		/// Calls the protected <see cref="RefreshItem"/> method; this is intended
		/// for the <see cref="CultureMapList"/> class only.
		/// </summary>
		/// <param name="item">The item.</param>
		internal void InternalRefreshItem(CultureMap item)
		{
			this.RefreshItem (item);
		}

		/// <summary>
		/// Refreshes the item. The default implementation simply clears the
		/// item's <c>IsRefreshNeeded</c>.
		/// </summary>
		/// <param name="item">The item.</param>
		protected virtual void RefreshItem(CultureMap item)
		{
			item.IsRefreshNeeded = false;
		}
		
		/// <summary>
		/// Creates a new unique id.
		/// </summary>
		/// <returns>The new unique id.</returns>
		protected abstract Druid CreateId();
		
		/// <summary>
		/// Gets the source for this culture map, based on where the field is
		/// defined (reference or patch module). If there is no bundle field,
		/// then the default resource manager source is used.
		/// </summary>
		/// <param name="field">The field or <c>null</c>.</param>
		/// <returns>The <see cref="CultureMapSource"/>.</returns>
		protected CultureMapSource GetCultureMapSource(ResourceBundle.Field field)
		{
			if (field == null)
			{
				return this.ResourceManager.BasedOnPatchModule ? CultureMapSource.PatchModule : CultureMapSource.ReferenceModule;
			}
			else
			{
				return field.BasedOnPatchModule ? CultureMapSource.PatchModule : CultureMapSource.ReferenceModule;
			}
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item to delete.</param>
		protected abstract void DeleteItem(CultureMap item);

		/// <summary>
		/// Persists the specified item.
		/// </summary>
		/// <param name="item">The item to store as a resource.</param>
		protected abstract void PersistItem(CultureMap item);

		/// <summary>
		/// Resets the specified field to its original value. This is the
		/// internal implementation which can be overridden.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="container">The data record.</param>
		/// <param name="fieldId">The field id.</param>
		protected virtual void ResetToOriginal(CultureMap item, StructuredData container, Druid fieldId)
		{
			container.ResetToOriginalValue (fieldId);
		}

		/// <summary>
		/// Loads all items found in a bundle into the <c>Collection</c>.
		/// </summary>
		/// <param name="bundle">The resource bundle.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		protected void LoadFromBundle(ResourceBundle bundle, string twoLetterISOLanguageName)
		{
			if (bundle != null)
			{
				using (this.SuspendNotifications ())
				{
					ResourceBundle refBundle = null;

					bool patch = bundle.BasedOnPatchModule;
					int module = bundle.Module.Id;

					if (patch)
					{
						refBundle = bundle.ReferenceBundle;
					}

					foreach (ResourceBundle.Field field in bundle.Fields)
					{
						string name = field.Name;

						if ((name == null) &&
							(patch))
						{
							name = refBundle[field.Id].Name;
						}

						if (this.FilterField (field, name))
						{
							this.LoadFromField (field, module, twoLetterISOLanguageName);
						}
					}
				}
			}
		}

		/// <summary>
		/// Executes the cleanup code after all resources have been added to
		/// the collection, while loading data (called by method <see cref="Load"/>.
		/// </summary>
		protected void PostLoadCleanup()
		{
			foreach (CultureMap item in this.Collection)
			{
				item.IsNewItem = false;
			}
		}

		/// <summary>
		/// Loads data from a resource bundle field.
		/// </summary>
		/// <param name="field">The resource bundle field.</param>
		/// <param name="module">The source module id.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>The data which describes the specified resource.</returns>
		protected abstract StructuredData LoadFromField(ResourceBundle.Field field, int module, string twoLetterISOLanguageName);

		/// <summary>
		/// Checks if the data stored in the field matches this accessor. This
		/// can be used to filter out specific fields.
		/// </summary>
		/// <param name="field">The field to check.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns>
		/// 	<c>true</c> if data should be loaded from the field; otherwise, <c>false</c>.
		/// </returns>
		protected abstract bool FilterField(ResourceBundle.Field field, string fieldName);

		/// <summary>
		/// Gets the bundle name used by this accessor.
		/// </summary>
		/// <returns>The name of the bundle.</returns>
		protected abstract string GetBundleName();

		/// <summary>
		/// Initializes the accessor and defines the resource manager.
		/// </summary>
		/// <param name="manager">The resource manager.</param>
		protected void Initialize(ResourceManager manager)
		{
			this.resourceManager = manager;
			
			this.items.Clear ();
			this.dirtyItems.Clear ();
		}

		/// <summary>
		/// Disables any change notifications until <c>Dispose</c> is called
		/// on the returned object. Use this with the <c>using</c> construct.
		/// </summary>
		/// <returns>The object which will automatically restore notifications on disposal.</returns>
		protected System.IDisposable SuspendNotifications()
		{
			return new Suspender (this);
		}

		/// <summary>
		/// Handles changes to the item collection.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Epsitec.Common.CollectionChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void HandleItemsCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			if (this.suspendNotifications == 0)
			{
				switch (e.Action)
				{
					case Epsitec.Common.Types.CollectionChangedAction.Reset:
						throw new System.InvalidOperationException ("The collection may not be reset");

					case Epsitec.Common.Types.CollectionChangedAction.Add:
					case Epsitec.Common.Types.CollectionChangedAction.Replace:
					case Epsitec.Common.Types.CollectionChangedAction.Remove:
						if (e.OldItems != null)
						{
							foreach (CultureMap item in e.OldItems)
							{
								this.dirtyItems[item] = true;
							}
						}
						if (e.NewItems != null)
						{
							foreach (CultureMap item in e.NewItems)
							{
								this.dirtyItems[item] = true;
							}
						}
						break;

					case Epsitec.Common.Types.CollectionChangedAction.Move:
						break;

					default:
						throw new System.InvalidOperationException (string.Format ("Unknown operation {0} applied to collection", e.Action));
				}
			}
		}

		#region Suspender Class

		private class Suspender : System.IDisposable
		{
			public Suspender(AbstractResourceAccessor host)
			{
				this.host = host;
				System.Threading.Interlocked.Increment (ref this.host.suspendNotifications);
			}

			#region IDisposable Members

			public void Dispose()
			{
				System.Threading.Interlocked.Decrement (ref this.host.suspendNotifications);
			}

			#endregion

			private AbstractResourceAccessor host;
		}

		#endregion

		/// <summary>
		/// This constant is used to tag a global property which contains the
		/// <c>DeveloperId</c> value (an <c>int</c>).
		/// </summary>
		public const string		DeveloperIdPropertyName = "DeveloperId";

		private readonly CultureMapList					items;
		private readonly Dictionary<CultureMap, bool>	dirtyItems;

		private ResourceManager					resourceManager;
		private int								suspendNotifications;
		private bool							forceModuleMerge;
	}
}
