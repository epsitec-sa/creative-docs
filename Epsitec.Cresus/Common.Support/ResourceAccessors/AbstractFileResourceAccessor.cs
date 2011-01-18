//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo = System.Globalization.CultureInfo;

	public abstract class AbstractFileResourceAccessor : DependencyObject, IResourceAccessor
	{
		protected AbstractFileResourceAccessor()
		{
			this.items = new CultureMapList (null);
			this.items.CollectionChanged += this.HandleItemsCollectionChanged;
			this.dirtyItems = new Dictionary<CultureMap, bool> ();
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

		public void Load(ResourceManager manager)
		{
			this.manager = manager;

			this.items.Clear ();
			this.dirtyItems.Clear ();

			//	We maintain a list of structured type resource accessors associated
			//	with the resource manager pool. This is required since we must mark
			//	all entities as dirty if any entity is modified in the pool...

			AccessorsCollection accessors = AbstractFileResourceAccessor.GetAccessors (manager.Pool);

			if (accessors == null)
			{
				accessors = new AccessorsCollection ();
				AbstractFileResourceAccessor.SetAccessors (manager.Pool, accessors);
			}
			else
			{
				accessors.Remove (this);
			}

			accessors.Add (this);

			if (this.manager.BasedOnPatchModule)
			{
				ResourceManager patchModuleManager = this.manager;
				ResourceManager refModuleManager   = this.manager.GetManagerForReferenceModule ();

				List<ResourceBundle> refBundles   = this.CreateBundleList (refModuleManager);
				List<ResourceBundle> patchBundles = this.CreateBundleList (patchModuleManager);

				using (this.SuspendNotifications ())
				{
					foreach (ResourceBundle refBundle in refBundles)
					{
						ResourceBundle patchBundle = patchBundles.Find (b => b.Name == refBundle.Name);

						if (patchBundle == null)
						{
							this.AddItem (refBundle, null, CultureMapSource.ReferenceModule);
						}
						else
						{
							this.AddItem (refBundle, patchBundle, CultureMapSource.DynamicMerge);
							patchBundles.Remove (patchBundle);
						}
					}

					foreach (ResourceBundle patchBundle in patchBundles)
					{
						this.AddItem (null, patchBundle, CultureMapSource.PatchModule);
					}
				}

			}
			else
			{
				List<ResourceBundle> bundles = this.CreateBundleList (this.manager);

				using (this.SuspendNotifications ())
				{
					foreach (ResourceBundle bundle in bundles)
					{
						this.AddItem (bundle, null, CultureMapSource.ReferenceModule);
					}
				}
			}
		}

		private void AddItem(ResourceBundle bundle, ResourceBundle patchBundle, CultureMapSource source)
		{
			CultureMap     item;
			StructuredData data = new StructuredData (this.GetResourceType ());

			switch (source)
			{
				case CultureMapSource.ReferenceModule:
					item = new CultureMap (this, bundle.Id, source);
					item.Name = bundle.Caption;
					
					if (this.manager.BasedOnPatchModule)
					{
//-						data.SetValue (Res.Fields.ResourceBaseFile.Bundle, null);
						data.SetValue (Res.Fields.ResourceBaseFile.BundleAux, bundle);
					}
					else
					{
						data.SetValue (Res.Fields.ResourceBaseFile.Bundle, bundle);
						data.SetValue (Res.Fields.ResourceBaseFile.BundleAux, bundle);
					}
					
					this.FillDataFromBundle (source, data, bundle, bundle);
					break;
				
				case CultureMapSource.PatchModule:
					item = new CultureMap (this, patchBundle.Id, source);
					item.Name = patchBundle.Caption;
					data.SetValue (Res.Fields.ResourceBaseFile.Bundle, patchBundle);
//-					data.SetValue (Res.Fields.ResourceBaseFile.BundleAux, bundle);
					this.FillDataFromBundle (source, data, patchBundle, null);
					break;
				
				case CultureMapSource.DynamicMerge:
					item = new CultureMap (this, bundle.Id, source);
					item.Name = bundle.Caption;
					data.SetValue (Res.Fields.ResourceBaseFile.Bundle, patchBundle);
					data.SetValue (Res.Fields.ResourceBaseFile.BundleAux, bundle);
					this.FillDataFromBundle (source, data, patchBundle, bundle);
					break;

				default:
					throw new System.ArgumentException ("Invalid source", "source");
			}

			item.RecordCultureData (Resources.DefaultTwoLetterISOLanguageName, data);
			this.Collection.Add (item);
		}

		private List<ResourceBundle> CreateBundleList(ResourceManager manager)
		{
			string[] names = manager.GetBundleIds ("*", this.GetResourceFileType (), ResourceLevel.Default);
			List<ResourceBundle> bundles = new List<ResourceBundle> ();

			foreach (string name in names)
			{
				bundles.Add (manager.GetBundle (name, ResourceLevel.Default, null));
			}

			bundles.Sort (
				delegate (ResourceBundle a, ResourceBundle b)
				{
					if (a.Rank < b.Rank)
					{
						return -1;
					}
					if (a.Rank > b.Rank)
					{
						return 1;
					}
					return 0;
				});

			return bundles;
		}

		/// <summary>
		/// Saves the resources.
		/// </summary>
		/// <param name="saverCallback">The saver callback.</param>
		public void Save(ResourceBundleSaver saverCallback)
		{
			this.PersistChanges ();
			this.RenumberBundles ();

			foreach (ResourceBundle bundle in this.deletedBundles)
			{
				saverCallback (this.manager, bundle, ResourceSetMode.Remove);
			}
			foreach (ResourceBundle bundle in this.createdBundles)
			{
				saverCallback (this.manager, bundle, ResourceSetMode.CreateOnly);
			}
			foreach (ResourceBundle bundle in this.editedBundles)
			{
				saverCallback (this.manager, bundle, ResourceSetMode.UpdateOnly);
			}

			this.deletedBundles.Clear ();
			this.createdBundles.Clear ();
			this.editedBundles.Clear ();
		}

		/// <summary>
		/// Resets the specified field to its original value.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="container">The data record.</param>
		/// <param name="fieldId">The field id.</param>
		public void ResetToOriginalValue(CultureMap item, StructuredData container, Druid fieldId)
		{
			throw new System.NotImplementedException ();
		}

		#region IResourceAccessor Members

		public CultureMapList Collection
		{
			get
			{
				return this.items;
			}
		}

		public bool BasedOnPatchModule
		{
			get
			{
				return this.manager.BasedOnPatchModule;
			}
		}

		public bool ContainsChanges
		{
			get
			{
				return this.dirtyItems.Count > 0;
			}
		}

		public IDataBroker GetDataBroker(Epsitec.Common.Types.StructuredData container, string fieldId)
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

			list.Add (Resources.DefaultTwoLetterISOLanguageName);

			return list;
		}

		public CultureMap CreateItem()
		{
			CultureMapSource source = this.manager.BasedOnPatchModule ? CultureMapSource.PatchModule : CultureMapSource.ReferenceModule;
			CultureMap item = new CultureMap (this, this.CreateId (), source);
			item.IsNewItem = true;
			return item;
		}

		/// <summary>
		/// Persists the changes to the underlying data store.
		/// </summary>
		/// <returns>
		/// The number of items which have been persisted.
		/// </returns>
		public int PersistChanges()
		{
			List<CultureMap> list = new List<CultureMap> (this.dirtyItems.Keys);

			this.dirtyItems.Clear ();

			using (this.SuspendNotifications ())
			{
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
			}

			return list.Count;
		}

		/// <summary>
		/// Reverts the changes applied to the accessor.
		/// </summary>
		/// <returns>
		/// The number of items which have been reverted.
		/// </returns>
		public int RevertChanges()
		{
			List<CultureMap> list = new List<CultureMap> (this.dirtyItems.Keys);

			this.dirtyItems.Clear ();

			using (this.SuspendNotifications ())
			{
				foreach (CultureMap item in list)
				{
					item.ClearCultureData ();

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
		public void NotifyItemChanged(CultureMap item, StructuredData container, DependencyPropertyChangedEventArgs e)
		{
			if (this.suspendNotifications == 0)
			{
				this.dirtyItems[item] = true;

				if ((this.BasedOnPatchModule) &&
					(item.Source == CultureMapSource.ReferenceModule))
				{
					if ((e != null) &&
						(e.PropertyName == Res.Fields.ResourceForm.XmlSourceMerge.ToString ()))
					{
						//	The modification of the "merged" information should not be
						//	considered as a modification which promotes the resource to
						//	the dynamic merge state.
					}
					else
					{
						//	The user edited a resource originally only found in the
						//	reference module, while in a patch module : this promotes
						//	the resource to the dynamic merge state.

						item.Source = CultureMapSource.DynamicMerge;
					}
				}
			}
		}

		public void NotifyCultureDataCleared(CultureMap item, string twoLetterISOLanguageName, Epsitec.Common.Types.StructuredData data)
		{
		}

		public StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName)
		{
			if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
			{
				StructuredData data = new StructuredData (this.GetResourceType ());

				this.FillData (data);

				item.RecordCultureData (Resources.DefaultTwoLetterISOLanguageName, data);
				
				return data;
			}
			else
			{
				throw new System.InvalidOperationException ();
			}
		}

		#endregion

		protected abstract string GetResourceFileType();

		protected abstract IStructuredType GetResourceType();

		protected abstract void FillDataFromBundle(CultureMapSource source, StructuredData data, ResourceBundle bundle, ResourceBundle auxBundle);

		protected abstract void FillData(StructuredData data);

		protected abstract void SetupBundleFields(ResourceBundle bundle);

		protected abstract void SetBundleFields(ResourceBundle bundle, StructuredData data);

		protected void HandleItemsCollectionChanged(object sender, Types.CollectionChangedEventArgs e)
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

		protected static Druid ToDruid(ResourceBundle.Field field)
		{
			if (field.IsValid)
			{
				string id = field.AsString;

				if (string.IsNullOrEmpty (id) == false)
				{
					return Druid.Parse (id);
				}
			}

			return Druid.Empty;
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
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item to delete.</param>
		protected void DeleteItem(CultureMap item)
		{
			if (this.manager.BasedOnPatchModule)
			{
				System.Diagnostics.Debug.Assert (item.Source != CultureMapSource.ReferenceModule);
			}

			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			ResourceBundle bundle = data.GetValue (Res.Fields.ResourceBaseFile.Bundle) as ResourceBundle;

			if (bundle == null)
			{
				return;
			}

			this.createdBundles.Remove (bundle);
			this.editedBundles.Remove (bundle);

			System.Diagnostics.Debug.Assert (bundle != null);

			if (!this.deletedBundles.Contains (bundle))
			{
				this.deletedBundles.Add (bundle);
			}
		}

		/// <summary>
		/// Persists the specified item.
		/// </summary>
		/// <param name="item">The item to store as a resource.</param>
		protected void PersistItem(CultureMap item)
		{
			if (this.manager.BasedOnPatchModule)
			{
				System.Diagnostics.Debug.Assert (item.Source != CultureMapSource.ReferenceModule);
			}

			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			ResourceBundle bundle = data.GetValue (Res.Fields.ResourceBaseFile.Bundle) as ResourceBundle;
			
			if (bundle == null)
			{
				string      prefix  = this.manager.ActivePrefix;
				CultureInfo culture = this.manager.ActiveCulture ?? Resources.FindCultureInfo ("fr");

				bundle = ResourceBundle.Create (this.manager, prefix, item.Id.ToBundleId (), ResourceLevel.Default, culture);
				bundle.DefineType (this.GetResourceFileType ());
				bundle.DefineRank (this.Collection.IndexOf (item));

				this.SetupBundleFields (bundle);
				this.createdBundles.Add (bundle);

				data.SetValue (Res.Fields.ResourceBaseFile.Bundle, bundle);
			}
			else
			{
				this.RecordBundleAsEdited (bundle);
			}

			bundle.DefineCaption (item.Name);
			this.SetBundleFields (bundle, data);
		}

		private void RecordBundleAsEdited(ResourceBundle bundle)
		{
			System.Diagnostics.Debug.Assert (bundle != null);

			if ((!this.createdBundles.Contains (bundle)) &&
				(!this.editedBundles.Contains (bundle)))
			{
				this.deletedBundles.Remove (bundle);
				this.editedBundles.Add (bundle);
			}
		}

		private void RenumberBundles()
		{
			int rank = 0;

			foreach (CultureMap item in this.Collection)
			{
				rank++;

				StructuredData data   = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
				ResourceBundle bundle = data.GetValue (Res.Fields.ResourceBaseFile.Bundle) as ResourceBundle;

				if ((bundle != null) &&
					(bundle.Rank != rank))
				{
					bundle.DefineRank (rank);
					this.RecordBundleAsEdited (bundle);
				}
			}
		}

		private Druid CreateId()
		{
			return AbstractResourceAccessor.CreateId (this.GetFullCollection (), this.manager.DefaultModuleId, this.manager.PatchDepth, null);
		}

		private IEnumerable<CultureMap> GetFullCollection()
		{
			AccessorsCollection accessors = AbstractFileResourceAccessor.GetAccessors (this.manager.Pool);
			
			foreach (AbstractFileResourceAccessor accessor in accessors.Collection)
			{
				if (accessor.manager == this.manager)
				{
					foreach (CultureMap item in accessor.Collection)
					{
						yield return item;
					}
				}
			}
		}

		#region AccessorsCollection Class

		/// <summary>
		/// The <c>AccessorsCollection</c> maintains a collection of <see cref="AbstractFileReourceAccessor"/>
		/// instances.
		/// </summary>
		private class AccessorsCollection
		{
			public AccessorsCollection()
			{
				this.list = new List<Weak<AbstractFileResourceAccessor>> ();
			}

			public void Add(AbstractFileResourceAccessor item)
			{
				this.list.Add (new Weak<AbstractFileResourceAccessor> (item));
			}

			public void Remove(AbstractFileResourceAccessor item)
			{
				this.list.RemoveAll (
					delegate (Weak<AbstractFileResourceAccessor> probe)
					{
						if (probe.IsAlive)
						{
							return probe.Target == item;
						}
						else
						{
							return true;
						}
					});
			}

			public IEnumerable<AbstractFileResourceAccessor> Collection
			{
				get
				{
					foreach (Weak<AbstractFileResourceAccessor> item in this.list)
					{
						AbstractFileResourceAccessor accessor = item.Target;

						if (accessor != null)
						{
							yield return accessor;
						}
					}
				}
			}

			private List<Weak<AbstractFileResourceAccessor>> list;
		}

		#endregion

		#region Suspender Class

		private class Suspender : System.IDisposable
		{
			public Suspender(AbstractFileResourceAccessor host)
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

			private AbstractFileResourceAccessor host;
		}

		#endregion

		private static void SetAccessors(DependencyObject obj, AccessorsCollection collection)
		{
			if (collection == null)
			{
				obj.ClearValue (AbstractFileResourceAccessor.AccessorsProperty);
			}
			else
			{
				obj.SetValue (AbstractFileResourceAccessor.AccessorsProperty, collection);
			}
		}

		private static AccessorsCollection GetAccessors(DependencyObject obj)
		{
			return obj.GetValue (AbstractFileResourceAccessor.AccessorsProperty) as AccessorsCollection;
		}

		private static DependencyProperty AccessorsProperty = DependencyProperty.RegisterAttached ("Accessors", typeof (AccessorsCollection), typeof (AbstractFileResourceAccessor), new DependencyPropertyMetadata ().MakeNotSerializable ());

		private readonly CultureMapList items;
		private readonly Dictionary<CultureMap, bool> dirtyItems;

		private readonly List<ResourceBundle> editedBundles  = new List<ResourceBundle> ();
		private readonly List<ResourceBundle> deletedBundles = new List<ResourceBundle> ();
		private readonly List<ResourceBundle> createdBundles = new List<ResourceBundle> ();
		
		protected ResourceManager manager;
		private int suspendNotifications;
		private bool forceModuleMerge;
	}
}
