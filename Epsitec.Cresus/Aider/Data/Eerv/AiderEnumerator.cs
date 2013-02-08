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
			AiderEnumerator.Execute (coreDataManager, AiderEnumerator.GetContactBatch, action);
		}


		public static void Execute(CoreDataManager coreDataManager, Action<BusinessContext, IEnumerable<AiderPersonEntity>> action)
		{
			AiderEnumerator.Execute (coreDataManager, AiderEnumerator.GetPersonBatch, action);
		}


		private static void Execute<T>(CoreDataManager coreDataManager, Func<DataContext, int, int, IEnumerable<T>> batchGetter, Action<BusinessContext, IEnumerable<T>> action)
		{
			const int size = 1000;

			for (int i = 0; ; i += size)
			{
				int skip = i;

				bool done = false;

				coreDataManager.Execute (b =>
				{
					var batch = batchGetter (b.DataContext, skip, size);

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


		private static IList<AiderContactEntity> GetContactBatch(DataContext dataContext, int skip, int take)
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

			AiderEnumerator.LoadRelatedData (dataContext, aiderContacts);

			return aiderContacts;
		}



		public static void LoadRelatedData(DataContext dataContext, IEnumerable<AiderContactEntity> aiderContacts)
		{
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
		}


		private static IList<AiderPersonEntity> GetPersonBatch(DataContext dataContext, int skip, int take)
		{
			var aiderPerson = new AiderPersonEntity ();

			var request = new Request ()
			{
				RootEntity = aiderPerson,
				Skip = skip,
				Take = take,
			};

			request.AddSortClause (InternalField.CreateId (aiderPerson));

			var aiderPersons = dataContext.GetByRequest<AiderPersonEntity> (request);

			AiderEnumerator.LoadRelatedData (dataContext, aiderPersons);

			return aiderPersons;
		}


		public static void LoadRelatedData(DataContext dataContext, IEnumerable<AiderPersonEntity> aiderPersons)
		{
			dataContext.LoadRelatedData (aiderPersons, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderPersonEntity p) => p.eCH_Person),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Parish),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Parish.Group),
			});

			var aiderContacts = aiderPersons
				.SelectMany (p => p.Contacts)
				.ToList ();

			dataContext.LoadRelatedData (aiderContacts, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderContactEntity c) => c.Address),
				LambdaUtils.Convert ((AiderContactEntity c) => c.Address.Town),
			});
		}




	}


}
