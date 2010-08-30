//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core.Data
{
	public abstract class Repository<T> : Repository
		where T : AbstractEntity, new ()
	{
		protected Repository(DataContext dataContext)
			: base (dataContext)
		{
		}

		public IEnumerable<T> GetAllEntities()
		{
			var example = this.CreateExample ();
			return this.GetEntitiesByExample (example);
		}

		public IList<T> GetAllEntitiesIncludingLiveEntities()
		{
			//	Force loading of all entities returned by the repository, so that they will
			//	show up in the data context, then query the data context in order to get
			//	back all entities of the given type.
			
			this.GetAllEntities ().ToList ();
			
			return this.dataContext.GetEntitiesOfType<T> ();
		}


		protected IEnumerable<T> GetEntitiesByExample(T example)
		{
			return this.GetEntitiesByExample<T> (example);
		}

		protected T CreateExample()
		{
			return this.CreateExample<T> ();
		}
	}
}
