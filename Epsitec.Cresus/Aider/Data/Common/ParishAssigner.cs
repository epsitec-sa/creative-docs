//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Common
{
	internal sealed class ParishAssigner
	{
		private ParishAssigner(ParishAddressRepository parishRepository, BusinessContext businessContext)
		{
			this.parishRepository = parishRepository;
			this.businessContext = businessContext;
			this.cache = new Dictionary<string, AiderGroupEntity> ();
		}


		private void AssignToParish(AiderPersonEntity person, Date? startDate = null)
		{
			var parishGroup = this.FindParishGroup (person);

			if (parishGroup.IsNull ())
			{
				this.AssignToNoParishGroup (person);
				this.UpdateSubscriptionAndRefreshCache (person);
				
				return;
			}

			if (person.ParishGroup == parishGroup)
			{
				this.UpdateSubscriptionAndRefreshCache (person);
				
				return;
			}

			var participations = ParishAssigner.GetParishGroupParticipations (person);
			var matchingParticipation = participations.Where (x => x.EndDate == null && x.Group == parishGroup).FirstOrDefault ();

			if (matchingParticipation != null)
			{
				//	We already have a participation for the specified group, which is
				//	still active (i.e. it has no end date). Make it active. This should
				//	not happen, but it does as duplicate participations might have been
				//	created in the past.

				person.ParishGroup = matchingParticipation.Group;

				this.UpdateSubscriptionAndRefreshCache (person);

				return;
			}
			

			var noParishGroupParticipations = ParishAssigner.GetNoParishGroupParticipations (person);

			if (noParishGroupParticipations.IsEmpty ())
			{
				var participationData = new ParticipationData (person);
				AiderGroupParticipantEntity.StartParticipation (businessContext, parishGroup, participationData, startDate);

				System.Diagnostics.Debug.Assert (person.ParishGroup == parishGroup);
				System.Diagnostics.Debug.Assert (person.ParishGroupPathCache == parishGroup.Path);
					
				this.UpdateSubscriptionAndRefreshCache (person);
			}
			else
			{
				//	Here we recycle the participation to the no parish group, so we don't delete
				//	an entity to create a similar one right after.

				var noParishGroupParticipation = noParishGroupParticipations.First ();

				noParishGroupParticipation.Group = parishGroup;
				person.ParishGroup = parishGroup;
					
				System.Diagnostics.Debug.Assert (person.ParishGroupPathCache == parishGroup.Path);

				this.UpdateSubscriptionAndRefreshCache (person);
			}
		}

		private void RemoveStaleParishes(AiderPersonEntity person, Date stopDate)
		{
			var parishGroup    = person.ParishGroup;
			var participations = ParishAssigner.GetParishGroupParticipations (person).Where (x => x.Group != parishGroup).ToList ();

			foreach (var participation in participations)
			{
				AiderGroupParticipantEntity.StopParticipation (participation, stopDate);
			}
		}

		private void UpdateSubscriptionAndRefreshCache(AiderPersonEntity person)
		{
			var subscriptions = AiderSubscriptionEntity.FindSubscriptions (this.businessContext, person);
			var refusals      = AiderSubscriptionRefusalEntity.FindRefusals (this.businessContext, person);

			if ((person.ParishGroup.IsNull ()) ||
				(person.ParishGroup.Parent.IsNull ()))
			{
				//	No parish...
			}
			else
			{
				var parishGroup = person.ParishGroup;
				var regionGroup = parishGroup.Parent;

				foreach (var subscription in subscriptions)
				{
					subscription.RegionalEdition = regionGroup;
				}
			}

			subscriptions.ForEach (x => x.RefreshCache ());
			refusals.ForEach (x => x.RefreshCache ());
		}

		private void AssignToParish(AiderLegalPersonEntity legalPerson)
		{
			var address     = legalPerson.Address;
			var parishName  = ParishAssigner.FindParishName (this.parishRepository, address);
			var parishGroup = this.FindParishGroup (parishName);

			if (string.IsNullOrEmpty (parishName))
			{
				legalPerson.ParishGroup = null;
				legalPerson.ParishGroupPathCache = null;
			}
			else
			{
				legalPerson.ParishGroup = parishGroup;
				legalPerson.ParishGroupPathCache = legalPerson.ParishGroup.Path;

				//	We should maybe update the subscription; but for legal persons, this might not be
				//	the right choice; don't do anything.
			}
		}

		private AiderGroupEntity FindParishGroup(AiderPersonEntity person)
		{
			var mainAddress = person.Address;

			if (mainAddress.IsNull () || mainAddress.Town.IsNull ())
			{
				return null;
			}

			var group = this.FindParishGroup (mainAddress);

			if (group.IsNull ())
			{
				var nameText = person.DisplayName;
				var addressText = mainAddress.GetSummary ().ToSimpleText ().Replace ("\n", "; ");
				var format = "WARNING: parish not found for {0} at address {1}";

				System.Diagnostics.Debug.WriteLine (string.Format (format, nameText, addressText));
			}

			return group;
		}

		private void AssignToNoParishGroup(AiderPersonEntity person)
		{
			var group = this.FindNoParishGroup ();

			if (person.ParishGroup == group)
			{
				return;
			}
			
			var participationData = new ParticipationData (person);

			AiderGroupParticipantEntity.StartParticipation (businessContext, group, participationData);
		}

		private AiderGroupEntity FindParishGroup(AiderAddressEntity address)
		{
			var parishName = ParishAssigner.FindParishName (this.parishRepository, address);

			if (string.IsNullOrEmpty (parishName))
			{
				return null;
			}

			return this.FindParishGroup (parishName);
		}



		private AiderGroupEntity FindParishGroup(string parishName)
		{
			if (string.IsNullOrEmpty (parishName))
			{
				return null;
			}

			return this.FindGroup (parishName, () => ParishAssigner.FindParishGroup (this.businessContext, this.parishRepository, parishName));
		}


		private AiderGroupEntity FindNoParishGroup()
		{
			return this.FindGroup
			(
				"noparishgroup",
				() => ParishAssigner.FindNoParishGroup (this.businessContext)
			);
		}


		private AiderGroupEntity FindGroup(string name, Func<AiderGroupEntity> groupFinder)
		{
			AiderGroupEntity group;

			if (!this.cache.TryGetValue (name, out group))
			{
				group = groupFinder ();

				this.cache[name] = group;
			}

			return group;
		}


		public static void AssignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, AiderPersonEntity person, Date? startDate = null)
		{
			var assigner = new ParishAssigner (parishRepository, businessContext);

			assigner.AssignToParish (person, startDate);
			assigner.RemoveStaleParishes (person, startDate ?? Date.Today);
		}


		public static void AssignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons)
		{
			var assigner = new ParishAssigner (parishRepository, businessContext);

			foreach (var person in persons)
			{
				assigner.AssignToParish (person);
				assigner.RemoveStaleParishes (person, Date.Today);
			}
		}


		public static void ReassignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, AiderPersonEntity person, Date? startDate = null)
		{
			ParishAssigner.UnassignFromParish (businessContext, person);
			ParishAssigner.AssignToParish (parishRepository, businessContext, person, startDate);
		}

		public static void ReassignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons)
		{
			ParishAssigner.UnassignFromParish (businessContext, persons);
			ParishAssigner.AssignToParish (parishRepository, businessContext, persons);
		}

		
		private static void UnassignFromParish(BusinessContext businessContext, AiderPersonEntity person)
		{
			var participations = ParishAssigner.GetParishGroupParticipations (person);

			foreach (var participation in participations)
			{
				AiderGroupParticipantEntity.StopParticipation (participation, Date.Today);
			}
		}

		private static void UnassignFromParish(BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons)
		{
			foreach (var person in persons)
			{
				ParishAssigner.UnassignFromParish (businessContext, person);
			}
		}


		public static void AssignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, AiderLegalPersonEntity legalPerson)
		{
			var assigner = new ParishAssigner (parishRepository, businessContext);

			assigner.AssignToParish (legalPerson);
		}


		public static void AssignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, IEnumerable<AiderLegalPersonEntity> legalPersons)
		{
			var assigner = new ParishAssigner (parishRepository, businessContext);

			foreach (var legalPerson in legalPersons)
			{
				assigner.AssignToParish (legalPerson);
			}
		}


		public static void ReassignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, AiderLegalPersonEntity legalPerson)
		{
			// Legal persons are not assigned to the parish groups or to the "no parish group", so
			// we don't have to manage this data here.

			ParishAssigner.AssignToParish (parishRepository, businessContext, legalPerson);
		}


		public static void ReassignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, IEnumerable<AiderLegalPersonEntity> legalPersons)
		{
			// Legal persons are not assigned to the parish groups or to the "no parish group", so
			// we don't have to manage this data here.

			ParishAssigner.AssignToParish (parishRepository, businessContext, legalPersons);
		}


		public static bool IsInNoParishGroup(AiderPersonEntity person)
		{
			return ParishAssigner.GetNoParishGroupParticipations (person).Any ();
		}

		private static IEnumerable<AiderGroupParticipantEntity> GetNoParishGroupParticipations(AiderPersonEntity person)
		{
			return person.Groups.Where (g => g.Group.IsNoParish ());
		}

		private static IEnumerable<AiderGroupParticipantEntity> GetParishGroupParticipations(AiderPersonEntity person)
		{
			return person.Groups.Where (g => g.Group.IsParish () || g.Group.IsNoParish ());
		}


		public static bool IsInValidParish(BusinessContext context, ParishAddressRepository parishRepository, AiderPersonEntity person)
		{
			var contact = person.GetMainContact ();
			var address = contact.IsNull () ? null : contact.Address;

			var parishGroup = person.ParishGroup;

			if (person.HasDerogation)
			{
				parishGroup = person.GetGeoParishGroup (context);
			}
			
			return ParishAssigner.IsInValidParish (parishRepository, address, parishGroup);
		}

		public static bool IsInValidParish(ParishAddressRepository parishRepository, AiderLegalPersonEntity legalPerson)
		{
			var address     = legalPerson.Address;
			var parishGroup = legalPerson.ParishGroup;

			return ParishAssigner.IsInValidParish (parishRepository, address, parishGroup);
		}

		
		private static bool IsInValidParish(ParishAddressRepository parishRepository, AiderAddressEntity address, AiderGroupEntity parishGroup)
		{
			var canHaveParish = address.IsNotNull ()
							 && address.Town.IsNotNull ()
							 && address.Town.Country.IsSwitzerland ();

			if (parishGroup.IsNull ())
			{
				return (canHaveParish == false)
					|| (ParishAssigner.FindParishName (parishRepository, address) == null);
			}

			if (canHaveParish == false)
			{
				return false;
			}

			var parishName = ParishAssigner.FindParishName (parishRepository, address);

			if (parishName == null)
			{
				//	No parish found. This is OK if the person already belongs to the "no parish"
				//	group...

				return parishGroup.IsNoParish ();
			}

			var parishGroupName = ParishAssigner.GetParishGroupName (parishRepository, parishName);

			return parishGroupName == parishGroup.Name;
		}


		public static AiderGroupEntity FindRegionGroup(BusinessContext businessContext, int regionNumber)
		{
			var groupName = ParishAssigner.GetRegionGroupName (regionNumber);

			return ParishAssigner.FindGroup (businessContext, groupName, GroupClassification.Region);
		}

		public static AiderGroupEntity FindParishGroup(BusinessContext businessContext, ParishAddressRepository parishRepository, string parishName)
		{
			var groupName = ParishAssigner.GetParishGroupName (parishRepository, parishName);

			return ParishAssigner.FindGroup (businessContext, groupName, GroupClassification.Parish);
		}

		
		private static AiderGroupEntity FindGroup(BusinessContext businessContext, string name, GroupClassification classification)
		{
			var dataContext = businessContext.DataContext;

			var example = new AiderGroupEntity ()
			{
				Name = name,
				GroupDef = new AiderGroupDefEntity ()
				{
					Classification = classification
				}
			};

			return dataContext.GetByExample (example).Single ();
		}

		public static AiderGroupEntity FindNoParishGroup(BusinessContext businessContext)
		{
			var example = new AiderGroupEntity ()
			{
				GroupDef = new AiderGroupDefEntity ()
				{
					Level = AiderGroupIds.NoParishLevel,
					Classification = GroupClassification.NoParish
				}
			};

			return businessContext.DataContext.GetByExample (example).Single ();
		}


		public static string FindParishName(ParishAddressRepository repository, AiderAddressEntity address)
		{
			var zipCode = address.Town.SwissZipCode.GetValueOrDefault ();
			var townName = address.Town.Name;
			var streetName = address.Street;
			var houseNumber = InvariantConverter.ToString (address.HouseNumber.GetValueOrDefault ());

			var normalizedStreetName = SwissPostStreet.NormalizeStreetName (streetName);
			var normalizedHouseNumber = SwissPostStreet.StripAndNormalizeHouseNumber (houseNumber);

			return repository.FindParishName (zipCode, townName, normalizedStreetName, normalizedHouseNumber);
		}

		public static string GetRegionGroupName(int regionNumber)
		{
			return string.Format ("Région {0:00}", regionNumber);
		}

		public static string GetParishGroupName(ParishAddressRepository parishRepository, string parishName)
		{
			var infos = parishRepository.GetDetails (parishName);

			if (infos == null)
			{
				return "Paroisse inconnue (" + parishName + ")";
			}
			else
			{
				return infos.FullParishName;
			}
		}


		private readonly ParishAddressRepository parishRepository;
		private readonly BusinessContext		businessContext;
		private readonly Dictionary<string, AiderGroupEntity> cache;
	}
}
