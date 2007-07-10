//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	/// <summary>
	/// The <c>AbstractResourceAccessor</c> class is the base class used by
	/// all resource accessors found in the <c>Epsitec.Common.Support</c>
	/// namespace.
	/// </summary>
	public abstract class AbstractResourceAccessor : Types.DependencyObject, IResourceAccessor
	{
		protected AbstractResourceAccessor()
		{
			this.items.CollectionChanged += this.HandleItemsCollectionChanged;
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

		protected bool AreNotificationsEnabled
		{
			get
			{
				return (this.suspendNotifications == 0);
			}
		}

		/// <summary>
		/// Loads resources from the specified resource manager. The resource
		/// manager will be used for all upcoming accesses.
		/// </summary>
		/// <param name="manager">The resource manager.</param>
		public abstract void Load(ResourceManager manager);

		#region IResourceAccessor Members

		public CultureMapList Collection
		{
			get
			{
				return this.items;
			}
		}

		public virtual IDataBroker GetDataBroker(Types.StructuredData container, string fieldId)
		{
			return null;
		}

		public bool ContainsChanges
		{
			get
			{
				return this.dirtyItems.Count > 0;
			}
		}

		public virtual CultureMap CreateItem()
		{
			CultureMap item = new CultureMap (this, this.CreateId ());
			item.IsNewItem = true;
			return item;
		}

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

			this.ResourceManager.ClearMergedBundlesFromBundleCache ();

			return list.Count;
		}

		public virtual int RevertChanges()
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

		public virtual void NotifyItemChanged(CultureMap item)
		{
			if (this.suspendNotifications == 0)
			{
				this.dirtyItems[item] = true;
			}
		}

		public virtual void NotifyCultureDataCleared(CultureMap item, string twoLetterISOLanguageName, Types.StructuredData data)
		{
		}

		public abstract Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName);

		#endregion

		internal static Druid CreateId(ResourceBundle bundle)
		{
			object devIdValue = Support.Globals.Properties.GetProperty (AbstractResourceAccessor.DeveloperIdPropertyName);

			int devId   = -1;
			int localId = -1;

			if (Types.UndefinedValue.IsUndefinedValue (devIdValue))
			{
				throw new System.InvalidOperationException ("Undefined developer id");
			}

			devId = (int) devIdValue;

			if (devId < 0)
			{
				throw new System.InvalidOperationException ("Invalid developer id");
			}

			foreach (ResourceBundle.Field field in bundle.Fields)
			{
				Druid id = field.Id;

				System.Diagnostics.Debug.Assert (id.IsValid);

				if (id.Developer == devId)
				{
					localId = System.Math.Max (localId, id.Local);
				}
			}

			return new Druid (bundle.Module.Id, devId, localId+1);
		}


		
		
		/// <summary>
		/// Creates a new unique id.
		/// </summary>
		/// <returns>The new unique id.</returns>
		protected abstract Druid CreateId();

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
		/// <param name="e">The <see cref="Epsitec.Common.Types.CollectionChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void HandleItemsCollectionChanged(object sender, Types.CollectionChangedEventArgs e)
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

		public const string DeveloperIdPropertyName = "DeveloperId";
		
		private readonly CultureMapList items = new CultureMapList ();
		private readonly Dictionary<CultureMap, bool> dirtyItems = new Dictionary<CultureMap, bool> ();

		private ResourceManager resourceManager;
		private int suspendNotifications;
	}
}
