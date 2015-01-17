using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.IO;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Subscription
{


	internal static class SubscriptionGenerator
	{


		/// <summary>
		/// This method will add subscriptions to households that do not have a subscription or a
		/// refusal yet.
		/// </summary>
		/// <remarks>
		/// This method will consider two households whose member share the same name and that live
		/// at the exact same address as a single household, and will generate a single
		/// subscription for both of them.
		/// This method can be called several time on the same database, it will only add
		/// subscriptions for the household that have none and that don't have a refusal yet.
		/// </remarks>
		public static void SubscribeHouseholds
		(
			CoreData coreData,
			ParishAddressRepository parishRepository
		)
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
			return households
				.Select (h => SubscriptionGenerator.GetHousehold (businessContext, h))
				.ToList ();
		}


		private static SubscriptionHousehold GetHousehold
		(
			BusinessContext businessContext,
			AiderHouseholdEntity household
		)
		{
			var address = household.Address;
			var town = address.Town;
			var members = household.Members;

			var subscribtion = AiderSubscriptionEntity.FindSubscription (businessContext, household);
			var refusal = AiderSubscriptionRefusalEntity.FindRefusal (businessContext, household);

			return new SubscriptionHousehold
			(
				businessContext.DataContext.GetNormalizedEntityKey (household).Value,
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
				members.Count (),
				subscribtion.IsNotNull (),
				refusal.IsNotNull ()
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
				.Select (g => g.ToList ())
				.Where (g => !SubscriptionGenerator.DiscardHouseholdGroup (g))
				.Select (g => SubscriptionGenerator.GetHouseholdToSubscribe (g))
				.ToList ();
		}


		private static bool DiscardHouseholdGroup(IEnumerable<SubscriptionHousehold> households)
		{
			return households.Any (h => h.HasSubscription)
				|| households.Any (h => h.HasRefusal);
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
			var aiderHouseholds = households.Select
			(
				h => businessContext.ResolveEntity<AiderHouseholdEntity>
				(
					h.EntityKey
				)
			);

			SubscriptionGenerator.SubscribeHouseholds
			(
				businessContext, parishRepository, aiderHouseholds.ToList ()
			);
		}


		public static void SubscribeHouseholds
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			IEnumerable<AiderHouseholdEntity> households
		)
		{
			var regionGroups = SubscriptionGenerator.GetRegionGroups (businessContext);

			foreach (var household in households)
			{
				if (!SubscriptionGenerator.DiscardHousehold (household))
				{
					SubscriptionGenerator.SubscribeHousehold
					(
						businessContext, parishRepository, regionGroups, household
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


		public static void SubscribeLegalPersons
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
					var legalPersonContact = legalPerson.Contacts.First ();

					SubscriptionGenerator.SubscribeLegalPerson
					(
						businessContext, parishRepository, regionGroups, legalPersonContact
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
			AiderContactEntity legalPersonContact
		)
		{
			var count = 1;
			var regionalEdition = SubscriptionGenerator.FindRegionGroup
			(
				parishRepository, regionGroups, legalPersonContact.Address
			);

			regionalEdition = regionGroups.First ().Value;

			AiderSubscriptionEntity.Create
			(
				businessContext, legalPersonContact, regionalEdition, count
			);
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
