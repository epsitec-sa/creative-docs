using Epsitec.Aider.Entities;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;
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


		public static void Execute(CoreDataManager coreDataManager, Action<BusinessContext, IEnumerable<AiderContactEntity>> action)
		{
			const int size = 1000;

			for (int i = 0; ;i += size)
			{
				int skip = i;

				bool done = false;

				coreDataManager.Execute (b =>
				{
					var batch = AiderEnumerator.GetBatch (b.DataContext, skip, size);

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


		private static IList<AiderContactEntity> GetBatch(DataContext dataContext, int skip, int take)
		{
			var aiderContact = new AiderContactEntity ();

			var request = new Request ()
			{
				RootEntity = aiderContact,
				Skip = skip,
				Take = take,
			};

			request.AddSortClause (InternalField.CreateId (aiderContact));

			var aiderContacts = dataContext.GetByRequest<AiderContactEntity> (request);

			dataContext.LoadRelatedData (aiderContacts, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderContactEntity c) => c.Address),
				LambdaUtils.Convert ((AiderContactEntity c) => c.Address.Town),
				LambdaUtils.Convert ((AiderContactEntity c) => c.Person),
				LambdaUtils.Convert ((AiderContactEntity c) => c.Person.eCH_Person),
				LambdaUtils.Convert ((AiderContactEntity c) => c.Household),
				LambdaUtils.Convert ((AiderContactEntity c) => c.Household.Address),
				LambdaUtils.Convert ((AiderContactEntity c) => c.Household.Address.Town),
			});

			return aiderContacts;
		}
	}


}
