using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public sealed class Database<T> : Database
		where T : AbstractEntity, new ()
	{


		public override int GetCount(DataContext dataContext)
		{
			return dataContext.GetCount (new T ());
		}


		public override IEnumerable<AbstractEntity> GetEntities(DataContext dataContext, int skip, int take)
		{
			var example = new T ();

			var request = new DataLayer.Loader.Request ()
			{
				RootEntity = example,
				Skip = skip,
				Take = take,
			};

			request.AddSortClause
			(
				InternalField.CreateId (example),
				SortOrder.Ascending
			);

			return dataContext.GetByRequest<T> (request);
		}


	}


}