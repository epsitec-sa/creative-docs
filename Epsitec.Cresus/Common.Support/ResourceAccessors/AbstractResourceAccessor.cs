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
			return new CultureMap (this, this.CreateId ());
		}

		public int PersistChanges()
		{
			List<CultureMap> list = new List<CultureMap> (this.dirtyItems.Keys);
			
			this.dirtyItems.Clear ();
			
			foreach (CultureMap item in list)
			{
				if (this.Collection.Contains (item))
				{
					this.PersistItem (item);
				}
				else
				{
					this.DeleteItem (item);
				}
			}

			this.ResourceManager.ClearMergedBundlesFromBundleCache ();

			return list.Count;
		}

		public virtual void NotifyItemChanged(CultureMap item)
		{
			if (this.suspendNotifications == 0)
			{
				this.dirtyItems[item] = true;
			}
		}

		public abstract Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName);

		#endregion

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

		private readonly CultureMapList items = new CultureMapList ();
		private readonly Dictionary<CultureMap, bool> dirtyItems = new Dictionary<CultureMap, bool> ();

		private ResourceManager resourceManager;
		private int suspendNotifications;
	}
}
