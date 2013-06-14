using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Aider.Data.Common
{


	internal sealed class AiderEnumerator
	{


		public static void Execute(CoreData coreData, Action<BusinessContext, IEnumerable<AiderContactEntity>> action)
		{
			AiderEnumerator.Execute (coreData, AiderEnumerator.GetContactBatch, action);
		}


		public static void Execute(CoreData coreData, Action<BusinessContext, IEnumerable<AiderHouseholdEntity>> action)
		{
			AiderEnumerator.Execute (coreData, AiderEnumerator.GetHouseholdBatch, action);
		}


		public static void Execute(CoreData coreData, Action<BusinessContext, IEnumerable<AiderSubscriptionEntity>> action)
		{
			AiderEnumerator.Execute (coreData, AiderEnumerator.GetSubscriptionBatch, action);
		}


		public static void Execute(CoreData coreData, Action<BusinessContext, IEnumerable<AiderGroupParticipantEntity>> action)
		{
			AiderEnumerator.Execute (coreData, AiderEnumerator.GetParticipantBatch, action);
		}


		public static void Execute(CoreData coreData, Action<BusinessContext, IEnumerable<AiderPersonEntity>> action)
		{
			AiderEnumerator.Execute (coreData, AiderEnumerator.GetPersonBatch, action);
		}


		public static void Execute(CoreData coreData, Action<BusinessContext, IEnumerable<AiderLegalPersonEntity>> action)
		{
			AiderEnumerator.Execute (coreData, AiderEnumerator.GetLegalPersonBatch, action);
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
					System.Console.WriteLine ("Batch {0}...{1}, {2}", skip, skip+size-1, typeof (T).Name);

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
			var request = AiderEnumerator.CreateBatchRequest<AiderContactEntity> (skip, take);
			var aiderContacts = dataContext.GetByRequest<AiderContactEntity> (request);

			AiderEnumerator.LoadRelatedData (dataContext, aiderContacts);

			return aiderContacts;
		}


		private static IList<AiderHouseholdEntity> GetHouseholdBatch(DataContext dataContext, int skip, int take)
		{
			var request = AiderEnumerator.CreateBatchRequest<AiderHouseholdEntity> (skip, take);
			var aiderHouseholds = dataContext.GetByRequest<AiderHouseholdEntity> (request);

			AiderEnumerator.LoadRelatedData (dataContext, aiderHouseholds);

			return aiderHouseholds;
		}


		private static IList<AiderSubscriptionEntity> GetSubscriptionBatch(DataContext dataContext, int skip, int take)
		{
			var request = AiderEnumerator.CreateBatchRequest<AiderSubscriptionEntity> (skip, take);
			var aiderSubscriptions = dataContext.GetByRequest<AiderSubscriptionEntity> (request);

			AiderEnumerator.LoadRelatedData (dataContext, aiderSubscriptions);

			return aiderSubscriptions;
		}


		private static IList<AiderGroupParticipantEntity> GetParticipantBatch(DataContext dataContext, int skip, int take)
		{
			var request = AiderEnumerator.CreateBatchRequest<AiderGroupParticipantEntity> (skip, take);
			var aiderSubscriptions = dataContext.GetByRequest<AiderGroupParticipantEntity> (request);

			return aiderSubscriptions;
		}


		private static IList<AiderPersonEntity> GetPersonBatch(DataContext dataContext, int skip, int take)
		{
			var request = AiderEnumerator.CreateBatchRequest<AiderPersonEntity> (skip, take);

			return dataContext.GetByRequest<AiderPersonEntity> (request);
		}


		private static IList<AiderLegalPersonEntity> GetLegalPersonBatch(DataContext dataContext, int skip, int take)
		{
			var request = AiderEnumerator.CreateBatchRequest<AiderLegalPersonEntity> (skip, take);

			return dataContext.GetByRequest<AiderLegalPersonEntity> (request);
		}


		private static Request CreateBatchRequest<T>(int skip, int take)
			where T : AbstractEntity, new()
		{
			var example = new T ();

			var request = new Request ()
			{
				RootEntity = example,
				Skip = skip,
				Take = take,
			};

			request.AddSortClause (InternalField.CreateId (example));

			return request;
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


		public static void LoadRelatedData(DataContext dataContext, IEnumerable<AiderHouseholdEntity> aiderHouseholds)
		{
			dataContext.LoadRelatedData (aiderHouseholds, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderHouseholdEntity h) => h.Address),
				LambdaUtils.Convert ((AiderHouseholdEntity h) => h.Address.Town),
			});

			var aiderPersons = aiderHouseholds
				.SelectMany (h => h.Members)
				.Distinct ()
				.ToList ();

			dataContext.LoadRelatedData (aiderPersons, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderPersonEntity p) => p.eCH_Person),
			});
		}


		public static void LoadRelatedData(DataContext dataContext, IEnumerable<AiderPersonEntity> aiderPersons)
		{
			dataContext.LoadRelatedData (aiderPersons, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderPersonEntity p) => p.eCH_Person),
				LambdaUtils.Convert ((AiderPersonEntity p) => p.ParishGroup),
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


		public static void LoadRelatedData(DataContext dataContext, IEnumerable<AiderLegalPersonEntity> aiderLegalPersons)
		{
			dataContext.LoadRelatedData (aiderLegalPersons, new List<LambdaExpression> ()
			{
				LambdaUtils.Convert ((AiderLegalPersonEntity p) => p.Address),
				LambdaUtils.Convert ((AiderLegalPersonEntity p) => p.Address.Town),
			});
		}


		public static void LoadRelatedData(DataContext dataContext, IEnumerable<AiderSubscriptionEntity> aiderSubscriptions)
		{
			// TODO load legal subscriptions data.

			var aiderHouseholds = aiderSubscriptions
				.Where (s => s.SubscriptionType == SubscriptionType.Household)
				.Select (s => s.Household)
				.ToList ();

			AiderEnumerator.LoadRelatedData (dataContext, aiderHouseholds);
		}


	}


}
