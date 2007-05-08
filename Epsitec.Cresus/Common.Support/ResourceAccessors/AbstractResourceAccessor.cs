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
				this.PersistItem (item);
			}

			return list.Count;
		}

		public void NotifyItemChanged(CultureMap item)
		{
			if (this.suspendNotifications == 0)
			{
				this.dirtyItems[item] = true;
			}
		}

		#endregion

		protected abstract Druid CreateId();
		
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
