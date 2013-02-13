using Epsitec.Aider.Entities;

using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
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


		public static void Execute(CoreData coreData, Action<BusinessContext, IEnumerable<AiderContactEntity>> action)
		{
			AiderEnumerator.Execute (coreData, AiderEnumerator.GetContactBatch, action);
		}


		private static void Execute<T>(CoreData coreData, Func<DataContext, int, int, IEnumerable<T>> batchGetter, Action<BusinessContext, IEnumerable<T>> action)
		{
			const int size = 1000;

			for (int i = 0; ; i += size)
			{
				int skip = i;

				bool done = false;

				using (var businessContext = new BusinessContext (coreData, false))
				{
					var batch = batchGetter (businessContext.DataContext, skip, size);

					if (batch.Any ())
					{
						action (businessContext, batch);
					}
					else
					{
						done = true;
					}
				}

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
				LambdaUtils.Convert ((AiderContactEntity c) => c.LegalPerson),
				LambdaUtils.Convert ((AiderContactEntity c) => c.LegalPerson.Address),
				LambdaUtils.Convert ((AiderContactEntity c) => c.LegalPerson.Address.Town),
			});
		}


		public static void LoadRelatedData(DataContext dataContext, IEnumerable<AiderPersonEntity> aiderPersons)
		{
			dataContext.LoadRelatedData (aiderPersons, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderPersonEntity p) => p.eCH_Person),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Parish),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.Parish.Group),
			});

			var aiderPersonContacts = aiderPersons
				.SelectMany (p => p.Contacts)
				.ToList ();

			dataContext.LoadRelatedData (aiderPersonContacts, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderContactEntity c) => c.Address),
				LambdaUtils.Convert ((AiderContactEntity c) => c.Address.Town),
				LambdaUtils.Convert ((AiderContactEntity c) => c.Household),
				LambdaUtils.Convert ((AiderContactEntity c) => c.LegalPerson),
			});

			var aiderHouseholds = aiderPersons
				.SelectMany (p => p.Households)
				.Distinct ()
				.ToList ();

			var aiderHouseholdContacts = aiderHouseholds
				.SelectMany (p => p.Contacts)
				.ToList ();

			dataContext.LoadRelatedData (aiderHouseholdContacts, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert((AiderContactEntity c) => c.Person),
			});

			var aiderGroups = aiderPersons
				.SelectMany (p => p.Groups)
				.ToList ();

			dataContext.LoadRelatedData (aiderGroups, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderGroupParticipantEntity p) => p.Group),
				LambdaUtils.Convert ((AiderGroupParticipantEntity p) => p.Group.GroupDef),
			});
		}


	}


}
