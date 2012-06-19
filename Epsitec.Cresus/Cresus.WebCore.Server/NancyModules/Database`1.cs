using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public sealed class Database<T> : Database
		where T : AbstractEntity, new ()
	{


		public override int GetCount(BusinessContext businessContext)
		{
			return businessContext.DataContext.GetCount (new T ());
		}


		public override IEnumerable<AbstractEntity> GetEntities(BusinessContext businessContext, int skip, int take)
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

			return businessContext.DataContext.GetByRequest<T> (request);
		}


	}


}