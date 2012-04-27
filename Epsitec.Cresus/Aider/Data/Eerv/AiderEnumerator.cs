using Epsitec.Aider.Entities;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class AiderEnumerator
	{


		public static void ExecuteUglyHack()
		{
			// HACK As the name of this method implies, this is a ugly hack. The idea is that at
			// some point, we need to iterate over all the persons in the database. But I don't have
			// implemented yet the operators in sql required to do this in a clean way, which are
			// orderby and first/skip. The workaround this is to have an auto incremented field in
			// the  eCH_PersonEntity so that we can simulate the orderby/first/skip by requiring
			// enties that are in a given range of this field, which is called HackIndex by the way.
			// But we can't have auto incrmented fields in Designer, so I add the auto incrment
			// manually here. That's really ugly but at least it works.

			// TODO Now, when the orderby/first/skip operators will be implemented, this hack should
			// be corrected. For this, we need to remove this method and modify the request in such
			// a way that it will use the orderby/first/skip operators to order the result set with
			// a criteria which will ensure that the order in which the entites are given is always
			// the same, and then create batches of entities with the first/skip operator to return
			// the 10000 at a time.

			using (var dbInfrastructure = new DbInfrastructure ())
			{
				var dbAccess = CoreData.GetDatabaseAccess ();

				dbInfrastructure.AttachToDatabase (dbAccess);

				using (var dbTransaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					dbTransaction.SqlBuilder.SetAutoIncrementOnTableColumn ("MUD_LVA", "U_LVGO02", 0);

					dbInfrastructure.ExecuteSilent (dbTransaction);

					dbTransaction.Commit ();
				}
			}
		}


		public static void Execute(BusinessContextManager businessContextManager, Action<BusinessContext, IEnumerable<AiderPersonEntity>> action)
		{
			foreach (var batchBounds in AiderEnumerator.GetPersonBatchBounds ())
			{
				var lowerBound = batchBounds.Item1;
				var upperBound = batchBounds.Item2;

				bool done = false;

				businessContextManager.Execute (b =>
				{
					var batch = AiderEnumerator.GetPersonBatch (b, lowerBound, upperBound);

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


		private static IEnumerable<AiderPersonEntity> GetPersonBatch(BusinessContext businessContext, long lowerBound, long upperBound)
		{
			var dataContext = businessContext.DataContext;

			var persons = AiderEnumerator.LoadPersons (dataContext, lowerBound, upperBound);

			AiderEnumerator.LoadHouseholds (dataContext, (p, h) => p.Household1 = h, lowerBound, upperBound);
			AiderEnumerator.LoadHouseholds (dataContext, (p, h) => p.Household2 = h, lowerBound, upperBound);
			
			return persons;
		}


		private static IEnumerable<AiderPersonEntity> LoadPersons(DataContext dataContext, long lowerBound, long upperBound)
		{
			var result = AiderEnumerator.GetRequestForAiderPersons (lowerBound, upperBound);

			var request = result.Item1;
			var aiderPersonExample = result.Item2;

			request.RootEntity = aiderPersonExample;

			var aiderPersons = dataContext.GetByRequest<AiderPersonEntity> (request);

			request.RequestedEntity = aiderPersonExample.eCH_Person;

			dataContext.GetByRequest<eCH_PersonEntity> (request);

			return aiderPersons;
		}


		private static void LoadHouseholds(DataContext dataContext, Action<AiderPersonEntity, AiderHouseholdEntity> householdSetter, long lowerBound, long upperBound)
		{
			var result = AiderEnumerator.GetRequestForAiderPersons (lowerBound, upperBound);

			var request = result.Item1;
			var aiderPersonExample = result.Item2;

			request.RootEntity = aiderPersonExample;

			var aiderHouseholdExample = new AiderHouseholdEntity ();
			householdSetter (aiderPersonExample, aiderHouseholdExample);
					
			request.RequestedEntity = aiderHouseholdExample;
			dataContext.GetByRequest<AiderHouseholdEntity> (request);

			var aiderAddressExample = new AiderAddressEntity ();
			aiderHouseholdExample.Address = aiderAddressExample;
			
			request.RequestedEntity = aiderAddressExample;
			dataContext.GetByRequest<AiderAddressEntity> (request);

			var aiderTownExample = new AiderTownEntity ();
			aiderAddressExample.Town = aiderTownExample;

			request.RequestedEntity = aiderTownExample;
			dataContext.GetByRequest<AiderTownEntity> (request);
		}


		private static Tuple<Request, AiderPersonEntity> GetRequestForAiderPersons(long lowerBound, long upperBound)
		{
			var aiderPersonExample = new AiderPersonEntity ();
			var eChPersonExample = new eCH_PersonEntity ();

			aiderPersonExample.eCH_Person = eChPersonExample;

			var request = new Request ();

			request.AddLocalConstraint (eChPersonExample, new ComparisonFieldValue
			(
				new Field (new Druid ("[LVGO02]")),
				BinaryComparator.IsGreaterOrEqual,
				new Constant (lowerBound)
			));

			request.AddLocalConstraint (eChPersonExample, new ComparisonFieldValue
			(
				new Field (new Druid ("[LVGO02]")),
				BinaryComparator.IsLowerOrEqual,
				new Constant (upperBound)
			));

			return Tuple.Create (request, aiderPersonExample);
		}


		private static IEnumerable<Tuple<long, long>> GetPersonBatchBounds()
		{
			long size = 10000;

			for (long i = 0; ;i += size)
			{
				yield return Tuple.Create (i, i + size - 1);
			}
		}


	}


}
