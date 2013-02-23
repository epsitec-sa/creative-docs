//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Favorites
{
	public abstract class FavoritesCollection
	{
		public FavoritesCollection(IEnumerable<AbstractEntity> collection, Druid databaseId)
		{
			this.ids = new List<long> ();
			this.databaseId = databaseId;

			if (collection.Any ())
			{
				var entity = collection.First ();
				var context = DataContextPool.GetDataContext (entity);

				this.ids.AddRange (collection.Select (e => context.GetNormalizedEntityKey (e).Value.RowKey.Id.Value));
			}
		}

		public int Count
		{
			get
			{
				return this.ids.Count;
			}
		}

		public Druid DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		public abstract AbstractEntity GetExample();

		public abstract Druid GetTypeId();

		public Request GetRequest()
		{
			var example = this.GetExample ();
			var request = new Request ()
			{
				RequestedEntity = example,
				RootEntity = example
			};

			request.Conditions.Add (new ValueSetComparison (InternalField.CreateId (example), SetComparator.In, this.ids.Select (id => new Constant (id))));

			return request;
		}

		public IEnumerable<AbstractEntity> GetByRequest(DataContext dataContext)
		{
			return dataContext.GetByRequest (this.GetRequest ());
		}

		private readonly List<long>				ids;
		private readonly Druid					databaseId;
	}
}
