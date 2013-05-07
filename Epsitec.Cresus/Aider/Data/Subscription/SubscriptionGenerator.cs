using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.IO;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Subscription
{


	internal static class SubscriptionGenerator
	{


		public static void Create
		(
			CoreData coreData,
			ParishAddressRepository parishRepository,
			bool subscribeHouseholds,
			bool subscribeLegalPersons
		)
		{
			if (subscribeHouseholds)
			{
				var households = SubscriptionGenerator.GetHouseholds (coreData);
				Logger.LogToConsole ("Households loaded");

				var householdsToSubscribe = SubscriptionGenerator.GetHouseholdsToSubscribe
				(
					households
				);
				Logger.LogToConsole ("Household compared");

				SubscriptionGenerator.SubscribeHouseholds
				(
					coreData, parishRepository, householdsToSubscribe
				);
				Logger.LogToConsole ("Household subscribed");
			}

			if (subscribeLegalPersons)
			{
				SubscriptionGenerator.SubscribeLegalPersons (coreData, parishRepository);
				Logger.LogToConsole ("Legal persons subscribed");
			}
		}


		private static IEnumerable<SubscriptionHousehold> GetHouseholds
		(
			CoreData coreData
		)
		{
			var allHouseholds = new List<SubscriptionHousehold> ();

			AiderEnumerator.Execute (coreData, (b, h) =>
			{
				var households = SubscriptionGenerator.GetHouseholds (b, h);

				allHouseholds.AddRange (households);
			});

			return allHouseholds;
		}

		private static IEnumerable<SubscriptionHousehold> GetHouseholds
		(
			BusinessContext businessContext,
			IEnumerable<AiderHouseholdEntity> households
		)
		{
			var dataContext = businessContext.DataContext;

			return households
				.Select (h => SubscriptionGenerator.GetHousehold (dataContext, h))
				.ToList ();
		}


		private static SubscriptionHousehold GetHousehold
		(
			DataContext dataContext,
			AiderHouseholdEntity household
		)
		{
			var address = household.Address;
			var town = address.Town;
			var members = household.Members;

			return new SubscriptionHousehold
			(
				dataContext.GetNormalizedEntityKey (household).Value,
				address.AddressLine1,
				address.PostBox,
				address.Street,
				address.HouseNumber.HasValue
					? InvariantConverter.ToString (address.HouseNumber.Value)
					: "",
				town.ZipCode,
				town.Name,
				town.Country.IsoCode,
				members.Select (m => m.eCH_Person.PersonOfficialName),
				members.Select (m => m.Age).Max () ?? 0,
				members.Count ()
			);
		}


		private static IList<SubscriptionHousehold> GetHouseholdsToSubscribe
		(
			IEnumerable<SubscriptionHousehold> households
		)
		{
			var comparer = new SubscriptionHouseholdComparer ();

			return households
				.GroupBy (h => h, comparer)
				.Select (g => SubscriptionGenerator.GetHouseholdToSubscribe (g))
				.ToList ();
		}


		private static SubscriptionHousehold GetHouseholdToSubscribe
		(
			IEnumerable<SubscriptionHousehold> households
		)
		{
			return households
				.OrderByDescending (h => h.MemberCount)
				.ThenByDescending (h => h.MemberMaxAge)
				.First ();
		}


		private static void SubscribeHouseholds
		(
			CoreData coreData,
			ParishAddressRepository parishRepository,
			IList<SubscriptionHousehold> households
		)
		{
			int i = 1;

			foreach (var batch in households.ToBatches (1000))
			{
				using (var businessContext = new BusinessContext (coreData, false))
				{
					SubscriptionGenerator.SubscribeHouseholds
					(
						businessContext, parishRepository, batch
					);

					businessContext.SaveChanges
					(
						LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
					);
				}

				Logger.LogToConsole ("Generated subscription for batch " + (i * 1000) + "/" + households.Count);
				i++;
			}
		}


		private static void SubscribeHouseholds
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			IEnumerable<SubscriptionHousehold> households
		)
		{
			var regionGroups = SubscriptionGenerator.GetRegionGroups (businessContext);

			foreach (var household in households)
			{
				var aiderHousehold = businessContext.ResolveEntity<AiderHouseholdEntity>
				(
					household.EntityKey
				);

				if (!SubscriptionGenerator.DiscardHousehold (aiderHousehold))
				{
					SubscriptionGenerator.SubscribeHousehold
					(
						businessContext, parishRepository, regionGroups, aiderHousehold
					);
				}
			}
		}


		private static Dictionary<int, AiderGroupEntity> GetRegionGroups
		(
			BusinessContext businessContext
		)
		{
			return AiderGroupEntity
				.FindRegionRootGroups (businessContext)
				.ToDictionary (g => g.GetRegionId ());
		}


		private static bool DiscardHousehold(AiderHouseholdEntity household)
		{
			// We skip the households that are not in vaud county.

			if (!household.Address.Town.IsInVaudCounty ())
			{
				return true;
			}

			// Only the household where at least one member is protestant should have a
			// subscription.

			var isProtestant = household
				.Members
				.Select (m => m.Confession)
				.Contains (PersonConfession.Protestant);

			if (!isProtestant)
			{
				return true;
			}

			//At this point, we know that the household should have a subscription.

			return false;
		}


		private static void SubscribeHousehold
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			Dictionary<int, AiderGroupEntity> regionGroups,
			AiderHouseholdEntity household
		)
		{

			var count = 1;
			var regionalEdition = SubscriptionGenerator.FindRegionGroup
			(
				parishRepository, regionGroups, household.Address
			);

			if (regionalEdition == null)
			{
				// TODO This is temporary. Here we might want to fall back to a default region or
				// something like that.
				return;
			}

			AiderSubscriptionEntity.Create (businessContext, household, regionalEdition, count);
		}


		private static void SubscribeLegalPersons
		(
			CoreData coreData,
			ParishAddressRepository parishRepository
		)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var example = new AiderLegalPersonEntity ();
				var legalPersons = businessContext.DataContext.GetByExample (example);

				var regionGroups = SubscriptionGenerator.GetRegionGroups (businessContext);

				foreach (var legalPerson in legalPersons)
				{
					SubscriptionGenerator.SubscribeLegalPerson
					(
						businessContext, parishRepository, regionGroups, legalPerson
					);
				}

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.None);
			}
		}


		private static void SubscribeLegalPerson
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			Dictionary<int, AiderGroupEntity> regionGroups,
			AiderLegalPersonEntity legalPerson
		)
		{
			var count = 1;
			var regionalEdition = SubscriptionGenerator.FindRegionGroup
			(
				parishRepository, regionGroups, legalPerson.Address
			);

			regionalEdition = regionGroups.First ().Value;

			AiderSubscriptionEntity.Create (businessContext, legalPerson, regionalEdition, count);
		}


		private static AiderGroupEntity FindRegionGroup
		(
			ParishAddressRepository parishRepository,
			Dictionary<int, AiderGroupEntity> regionGroups,
			AiderAddressEntity address
		)
		{
			var parishName = ParishAssigner.FindParishName (parishRepository, address);

			if (parishName == null)
			{
				return null;
			}
			
			var parishInformation = parishRepository.GetDetails (parishName);
			var regionCode = parishInformation.RegionCode;

			return regionGroups[regionCode];
		}


	}


}
