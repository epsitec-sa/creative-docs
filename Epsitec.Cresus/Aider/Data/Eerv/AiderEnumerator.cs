using Epsitec.Aider.Entities;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class AiderEnumerator
	{


		public static void Execute(CoreDataManager coreDataManager, Action<BusinessContext, IEnumerable<AiderPersonEntity>> action)
		{
			int size = 1000;

			for (int i = 0; ;i += size)
			{
				int skip = i;
				int take = size;

				bool done = false;

				coreDataManager.Execute (b =>
				{
					var batch = AiderEnumerator.GetPersonBatch (b, skip, take);

					if (batch.Any ())
					{
						action (b, batch);
					}
					else
					{
						done = true;
					}			
				});

				if (done)
				{
					break;
				}
			}
		}


		private static IEnumerable<AiderPersonEntity> GetPersonBatch(BusinessContext businessContext, int skip, int take)
		{
			var dataContext = businessContext.DataContext;

			var aiderPersonExample = new AiderPersonEntity ();
			var eChPersonExample = new eCH_PersonEntity ();

			aiderPersonExample.eCH_Person = eChPersonExample;

			var request = new Request ()
			{
				RootEntity = aiderPersonExample,
				Skip = skip,
				Take = take,
			};

			request.SortClauses.Add
			(
				new SortClause
				(
					InternalField.CreateId (aiderPersonExample),
					SortOrder.Ascending
				)
			);

			request.RequestedEntity = aiderPersonExample;
			var aiderPersons = dataContext.GetByRequest<AiderPersonEntity> (request);

			var lambdas = new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderPersonEntity p) => p.eCH_Person),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Household1),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Household2),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Household1.Address),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Household2.Address),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Household1.Address.Town),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Household2.Address.Town),
			};

			dataContext.LoadRelatedData (aiderPersons, lambdas);

			return aiderPersons;
		}
	}


}
