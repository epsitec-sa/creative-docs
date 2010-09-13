//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

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
		protected Repository(CoreData data, DataContext context = null)
			: base (data, context)
		{
		}

		public override Common.Support.Druid GetEntityType()
		{
			return EntityInfo.GetTypeId<T> ();
		}

		public IEnumerable<T> GetAllEntities()
		{
			var example = this.CreateExample ();
			return this.GetByExample (example);
		}

		public IList<T> GetAllEntitiesIncludingLiveEntities()
		{
			//	Force loading of all entities returned by the repository, so that they will
			//	show up in the data context, then query the data context in order to get
			//	back all entities of the given type.
			
			this.GetAllEntities ().ToList ();
			
			return this.dataContext.GetEntitiesOfType<T> ();
		}


		public IEnumerable<T> GetByExample(T example)
		{
			return this.GetEntitiesByExample<T> (example);
		}

		public IEnumerable<T> GetByRequest(Request request)
		{
			return this.GetEntitiesByRequest<T> (request);
		}

		public T CreateExample()
		{
			return this.CreateExample<T> ();
		}
	}
}
