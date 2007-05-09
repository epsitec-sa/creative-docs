//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	public abstract class AbstractResourceAccessor : IResourceAccessor
	{
		protected AbstractResourceAccessor(IDataBroker dataBroker)
		{
			this.dataBroker = dataBroker;
			this.items.CollectionChanged += this.HandleItemsCollectionChanged;
		}

		public ResourceManager ResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}
		
		public abstract void Load(ResourceManager manager);

		#region IResourceAccessor Members

		public CultureMapList Collection
		{
			get
			{
				return this.items;
			}
		}

		public IDataBroker DataBroker
		{
			get
			{
				return this.dataBroker;
			}
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

		public void NotifyItemChanged(CultureMap item)
		{
			if (this.suspendNotifications == 0)
			{
				this.dirtyItems[item] = true;
			}
		}

		public abstract Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName);

		#endregion

		protected abstract Druid CreateId();

		protected abstract void DeleteItem(CultureMap item);
		
		protected abstract void PersistItem(CultureMap item);

		protected void Initialize(ResourceManager manager)
		{
			this.resourceManager = manager;
			
			this.items.Clear ();
			this.dirtyItems.Clear ();
		}

		protected System.IDisposable SuspendNotifications()
		{
			return new Suspender (this);
		}

		private void HandleItemsCollectionChanged(object sender, Types.CollectionChangedEventArgs e)
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
		private readonly IDataBroker dataBroker;

		private ResourceManager resourceManager;
		private int suspendNotifications;
	}
}
