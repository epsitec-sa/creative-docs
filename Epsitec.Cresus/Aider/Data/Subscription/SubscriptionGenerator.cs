using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

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


		public static void Create(CoreData coreData, ParishAddressRepository parishRepository)
		{
			// TODO Also add a subscription for the legal persons in the database ?

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
			// TODO Make a subscription only if the person is in vaud county. Or whatever complex
			// logic they want us to use.
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
				parishRepository, regionGroups, household
			);

			if (regionalEdition == null)
			{
				// TODO This is temporary. Here we might want to fall back to a default region or
				// something like that.
				return;
			}

			AiderSubscriptionEntity.Create (businessContext, household, regionalEdition, count);
		}


		private static AiderGroupEntity FindRegionGroup
		(
			ParishAddressRepository parishRepository,
			Dictionary<int, AiderGroupEntity> regionGroups,
			AiderHouseholdEntity household
		)
		{
			var parishName = ParishAssigner.FindParishName (parishRepository, household.Address);

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
