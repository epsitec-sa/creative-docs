//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Repositories
{
	public abstract class Repository
	{
		protected Repository(CoreData data, DataContext context)
		{
			this.data = data;
			this.dataContext = context ?? data.DataContext;
		}

		public abstract Druid GetEntityType();


		protected T CreateExample<T>() where T : AbstractEntity, new()
		{
			return new T ();
		}


		protected IEnumerable<T> GetEntitiesByExample<T>(T example)
			where T : AbstractEntity
		{
			return this.dataContext.GetByExample<T> (example);
		}


		protected IEnumerable<T> GetEntitiesByRequest<T>(Request request)
			where T : AbstractEntity
		{
			return this.dataContext.GetByRequest<T> (request);
		}


		protected IEnumerable<T> GetEntitiesByExample<T>(T example, int index, int count)
			where T : AbstractEntity
		{
			return this.dataContext.GetByExample<T> (example).Skip (index).Take (count);
		}


		protected IEnumerable<T> GetEntitiesByRequest<T>(Request request, int index, int count)
			where T : AbstractEntity
		{
			return this.dataContext.GetByRequest<T> (request).Skip (index).Take (count);
		}


		protected readonly CoreData data;
		protected readonly DataContext dataContext;
	}
}
