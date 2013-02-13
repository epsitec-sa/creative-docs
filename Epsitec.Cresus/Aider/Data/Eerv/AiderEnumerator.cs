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

			var aiderGroups = aiderPersons
				.SelectMany (p => p.Groups)
				.ToList ();

			dataContext.LoadRelatedData (aiderGroups, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderGroupParticipantEntity p) => p.Group),
				LambdaUtils.Convert ((AiderGroupParticipantEntity p) => p.Group.GroupDef),
			});
		}


		public static void LoadRelatedData(DataContext dataContext, IEnumerable<AiderHouseholdEntity> aiderHouseholds)
		{
			dataContext.LoadRelatedData (aiderHouseholds, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderHouseholdEntity h) => h.Address),
				LambdaUtils.Convert ((AiderHouseholdEntity h) => h.Address.Town),
			});
			
			var aiderContacts = aiderHouseholds
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
