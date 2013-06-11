//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.Diagnostics;

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


		private void AssignToParish(AiderPersonEntity person)
		{
			var parishGroup = this.FindParishGroup (person);

			if (parishGroup.IsNull ())
			{
				this.AssignToNoParishGroup (person);
			}
			else
			{
				var noParishGroupParticipation = ParishAssigner.GetNoParishGroupParticipation (person);

				if (noParishGroupParticipation.IsNull ())
				{
					var participationData = new ParticipationData (person);
					AiderGroupParticipantEntity.StartParticipation (businessContext, parishGroup, participationData);

					person.ParishGroup = parishGroup;
				}
				else
				{
					// Here we recycle the participation to the no parish group, so we don't delete
					// an entity to create a similar one right after.

					noParishGroupParticipation.Group = parishGroup;
					person.ParishGroup = parishGroup;
				}
			}
		}


		private void AssignToParish(AiderLegalPersonEntity legalPerson)
		{
			var address = legalPerson.Address;
			var parishName = ParishAssigner.FindParishName (this.parishRepository, address);

			if (string.IsNullOrEmpty (parishName))
			{
				legalPerson.ParishGroup = null;
			}
			else
			{
				legalPerson.ParishGroup = this.FindParishGroup (parishName);
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

				Debug.WriteLine (string.Format (format, nameText, addressText));
			}

			return group;
		}


		private void AssignToNoParishGroup(AiderPersonEntity person)
		{
			var participationData = new ParticipationData (person);
			var group = this.FindNoParishGroup ();

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
			return this.FindGroup
			(
				parishName,
				() => ParishAssigner.FindParishGroup (this.businessContext, this.parishRepository, parishName)
			);
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


		public static void AssignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, AiderPersonEntity person)
		{
			var assigner = new ParishAssigner (parishRepository, businessContext);

			assigner.AssignToParish (person);
		}


		public static void AssignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons)
		{
			var assigner = new ParishAssigner (parishRepository, businessContext);

			foreach (var person in persons)
			{
				assigner.AssignToParish (person);
			}
		}


		public static void ReassignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, AiderPersonEntity person)
		{
			ParishAssigner.UnassignFromParish (businessContext, person);

			ParishAssigner.AssignToParish (parishRepository, businessContext, person);
		}


		public static void ReassignToParish(ParishAddressRepository parishRepository, BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons)
		{
			foreach (var person in persons)
			{
				ParishAssigner.UnassignFromParish (businessContext, person);
			}

			ParishAssigner.AssignToParish (parishRepository, businessContext, persons);
		}


		private static void UnassignFromParish(BusinessContext businessContext, AiderPersonEntity person)
		{
			var participations = ParishAssigner.GetParishGroupParticipations (person).ToList ();

			var noParishParticipation = ParishAssigner.GetNoParishGroupParticipation (person);
			if (noParishParticipation.IsNotNull ())
			{
				participations.Add (noParishParticipation);
			}

			foreach (var participation in participations)
			{
				AiderGroupParticipantEntity.StopParticipation (participation, Date.Today);
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
			return ParishAssigner.GetNoParishGroupParticipation (person) != null;
		}

		public static AiderGroupParticipantEntity GetNoParishGroupParticipation(AiderPersonEntity person)
		{
			return person.Groups.FirstOrDefault (g => g.Group.IsNoParish ());
		}

		public static IEnumerable<AiderGroupParticipantEntity> GetParishGroupParticipations(AiderPersonEntity person)
		{
			return person.Groups.Where (g => g.Group.IsParish ());
		}


		public static bool IsInValidParish(ParishAddressRepository parishRepository, AiderPersonEntity person)
		{
			var contact = person.GetMainContact ();
			var address = contact.IsNull ()
				? null
				: contact.Address;

			var parishGroup = person.ParishGroup;

			return ParishAssigner.IsInValidParish (parishRepository, address, parishGroup);
		}


		public static bool IsInValidParish(ParishAddressRepository parishRepository, AiderLegalPersonEntity legalPerson)
		{
			var address = legalPerson.Address;
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
				return !canHaveParish
					|| ParishAssigner.FindParishName (parishRepository, address) == null;
			}

			if (!canHaveParish)
			{
				return false;
			}

			var parishName = ParishAssigner.FindParishName (parishRepository, address);

			if (parishName == null)
			{
				return false;
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


		private static AiderGroupEntity FindNoParishGroup(BusinessContext businessContext)
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


		private readonly BusinessContext businessContext;


		private readonly Dictionary<string, AiderGroupEntity> cache;


	}


}
