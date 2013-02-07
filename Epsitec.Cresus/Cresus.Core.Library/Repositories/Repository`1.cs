//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Repositories
{
	public abstract class Repository<T> : Repository
		where T : AbstractEntity, new ()
	{
		protected Repository(CoreData data, DataContext context, DataLifetimeExpectancy lifetimeExpectancy = DataLifetimeExpectancy.Unknown)
			: base (data, context, lifetimeExpectancy)
		{
		}


		public override Common.Support.Druid GetEntityType()
		{
			return EntityInfo<T>.GetTypeId ();
		}

		public IEnumerable<T> GetAllEntities()
		{
			// This method is the incarnation of evil and should never ever be called. The problem
			// is that it will fetch all entities of a given type in the database. Imagine what will
			// happen when there are lots of entities of the type you request, like say 10000000.
			// You should always filter entities by making a proper request or an example. It is
			// wrong to filter them in memory afterwards.
			// For the rare cases where you really know what you are doing and really want to fetch
			// all entities of a given type in the database, you can still do it by creating an
			// empty example. But this should be the exception and you should really not do it
			// without carefully thinking at all the implications.

			var message = "\n=================================================================="
						+ "\n= WARNING: Repository<T>.GetAllEntities() has been called !      ="
						+ "\n==================================================================";

			System.Diagnostics.Debug.WriteLine (message);

			var example = this.CreateExample ();
			return this.GetByExample (example);
		}

		public IList<T> GetAllEntitiesIncludingLiveEntities()
		{
			//	Force loading of all entities returned by the repository, so that they will
			//	show up in the data context, then query the data context in order to get
			//	back all entities of the given type.
			
			this.GetAllEntities ().ToList ();

			return this.dataContext.GetEntities ().OfType<T> ().ToList ();
		}


		public IEnumerable<T> GetByExample(T example)
		{
			if (this.HasMapper)
			{
				return this.dataContext.GetByExample<T> (example).Select (x => this.Map (x));
			}
			else
			{
				return this.dataContext.GetByExample<T> (example);
			}
		}

		public IEnumerable<T> GetByRequest(Request request)
		{
			if (this.HasMapper)
			{
				return this.dataContext.GetByRequest<T> (request).Select (x => this.Map (x));
			}
			else
			{
				return this.dataContext.GetByRequest<T> (request);
			}
		}

		public T CreateExample()
		{
			return new T ();
		}
	}
}
