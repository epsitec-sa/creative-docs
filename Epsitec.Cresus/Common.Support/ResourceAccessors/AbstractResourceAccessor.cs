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

		public abstract void Load();

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
			this.dirtyItems[item] = true;
		}

		#endregion

		protected abstract Druid CreateId();
		
		protected abstract void PersistItem(CultureMap item);


		private readonly CultureMapList items = new CultureMapList ();
		private readonly Dictionary<CultureMap, bool> dirtyItems = new Dictionary<CultureMap, bool> ();
		private readonly IDataBroker dataBroker;
	}
}
