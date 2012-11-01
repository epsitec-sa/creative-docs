using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator;

using System;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	internal sealed class Caches : IDisposable
	{


		public Caches()
		{
			this.propertyAccessorCache = new PropertyAccessorCache ();
			this.autoCreatorCache = new AutoCreatorCache ();
			this.columnIdCache = new IdCache<string> ();
		}


		internal PropertyAccessorCache PropertyAccessorCache
		{
			get
			{
				return this.propertyAccessorCache;
			}
		}


		internal AutoCreatorCache AutoCreatorCache
		{
			get
			{
				return this.autoCreatorCache;
			}
		}


		internal IdCache<string> ColumnIdCache
		{
			get
			{
				return this.columnIdCache;
			}
		}


		public void Dispose()
		{
			this.propertyAccessorCache.Dispose ();
			this.autoCreatorCache.Dispose ();
			this.columnIdCache.Dispose ();
		}


		private readonly PropertyAccessorCache propertyAccessorCache;


		private readonly AutoCreatorCache autoCreatorCache;


		private readonly IdCache<string> columnIdCache;


	}


}
