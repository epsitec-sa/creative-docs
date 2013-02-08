//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
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
					person.Parish = AiderGroupParticipantEntity.StartParticipation (businessContext, person, parishGroup, null, null);
				}
				else
				{
					// Here we recycle the participation to the no parish group, so we don't delete
					// an entity to create a similar one right after.

					noParishGroupParticipation.Group = parishGroup;
					person.Parish = noParishGroupParticipation;
				}

				//TODO This should be moved in a business rule.

				foreach (var contact in person.Contacts)
				{
					contact.ParishGroupPathCache = parishGroup.Path;
				}
			}
		}


		private AiderGroupEntity FindParishGroup(AiderPersonEntity person)
		{
			var mainAddress = ParishAssigner.FindMainAddress (person);

			if (mainAddress.IsNull ())
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
			var noParishGroup = this.FindNoParishGroup ();

			AiderGroupParticipantEntity.StartParticipation (businessContext, person, noParishGroup, null, null);
		}


		private AiderGroupEntity FindParishGroup(AiderAddressEntity address)
		{
			var parishName = ParishAssigner.FindParishName (parishRepository, address);

			if (string.IsNullOrEmpty (parishName))
			{
				return null;
			}

			return this.FindGroup
			(
				parishName,
				() => ParishAssigner.FindParishGroup (this.businessContext, parishName)
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


		private static AiderAddressEntity FindMainAddress(AiderPersonEntity person)
		{
			var addressContacts = person.Contacts
				.Where (c => c.Address.IsNotNull () && c.Address.Town.IsNotNull ())
				.ToList ();

			var mainContact = addressContacts
				.Where (c => c.ContactType == ContactType.PersonHousehold)
				.FirstOrDefault ();

			if (mainContact.IsNull ())
			{
				var personalContacts =
					from c in addressContacts
					where c.ContactType == ContactType.PersonAddress
					let score = ParishAssigner.GetScore (c.AddressType)
					where score != null
					orderby score
					select c;

				mainContact = personalContacts.FirstOrDefault ();
			}

			if (mainContact.IsNull ())
			{
				return null;
			}

			return mainContact.Address;
		}


		private static int? GetScore(AddressType type)
		{
			// A low score is makes a type come in priority to another. Null is an invalid type.

			switch (type)
			{
				case AddressType.Default:
					return 2;

				case AddressType.Other:
					return 1;

				case AddressType.Professional:
					return null;

				case AddressType.Secondary:
					return 0;

				default:
					throw new NotImplementedException ();
			}
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


		public static void AssignToNoParishGroup(BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons)
		{
			var assigner = new ParishAssigner (null, businessContext);

			foreach (var person in persons)
			{
				assigner.AssignToNoParishGroup (person);
			}
		}


		public static bool IsInNoParishGroup(AiderPersonEntity person)
		{
			return ParishAssigner.GetNoParishGroupParticipation (person) != null;
		}

		public static AiderGroupParticipantEntity GetNoParishGroupParticipation(AiderPersonEntity person)
		{
			return person.Groups.FirstOrDefault (g => g.Group.Path == AiderGroupIds.NoParish);
		}


		public static bool IsInValidParish(ParishAddressRepository parishRepository, BusinessContext businessContext, AiderPersonEntity person)
		{
			var currentGroupName = person.Parish.Group.Name;

			return person.Contacts
				.Where (c => c.Address.Town.IsNotNull ())
				.Select (c => ParishAssigner.FindParishName (parishRepository, c.Address))
				.Where (n => !string.IsNullOrEmpty (n))
				.Select (n => ParishAssigner.GetParishGroupName (n))
				.Contains (currentGroupName);
		}


		public static AiderGroupEntity FindRegionGroup(BusinessContext businessContext, int regionNumber)
		{
			var groupName = ParishAssigner.GetRegionGroupName (regionNumber);

			return ParishAssigner.FindGroup (businessContext, groupName, GroupClassification.Region);
		}


		public static AiderGroupEntity FindParishGroup(BusinessContext businessContext, string parishName)
		{
			var groupName = ParishAssigner.GetParishGroupName (parishName);

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
				Path = AiderGroupIds.NoParish
			};

			return businessContext.DataContext.GetByExample (example).Single ();
		}


		public static bool IsParishGroup(AiderGroupEntity group)
		{
			return group.GroupDef.PathTemplate == AiderGroupIds.Parish;
		}


		private static string FindParishName(ParishAddressRepository repository, AiderAddressEntity address)
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


		public static string GetParishGroupName(string parishName)
		{
			return "Paroisse de " + parishName;
		}


		private readonly ParishAddressRepository parishRepository;


		private readonly BusinessContext businessContext;


		private readonly Dictionary<string, AiderGroupEntity> cache;


	}


}
