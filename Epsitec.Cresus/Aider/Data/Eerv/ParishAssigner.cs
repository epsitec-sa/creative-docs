//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class ParishAssigner
	{


		/// <remarks>
		/// This method should only be called on entities that are persisted in the database and
		/// have not yet been modified in memory.
		/// 
		/// This method should only be called on entities that are not yet assigned to a parish. The
		/// case where a person is alread assigned to a parish is not treated here and will procude
		/// inconsistant data. To treat this case, we would have first to remove any existing
		/// association with a parish group or with the "no parish" group, which we don't. 
		/// </remarks>
		public static void AssignToParishes(ParishAddressRepository parishRepository, BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons)
		{
			var parishNameToGroups = new Dictionary<string, AiderGroupEntity> ();

			foreach (var person in persons)
			{
				ParishAssigner.AssignToParish (businessContext, parishRepository, parishNameToGroups, person);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		/// <remarks>
		/// The same remarks as the function above apply here.
		/// </remarks>
		public static void AssignToParish(BusinessContext businessContext, AiderPersonEntity person)
		{
			var parishRepository = ParishAddressRepository.Current;
			var parishNameToGroups = new Dictionary<string, AiderGroupEntity> ();

			ParishAssigner.AssignToParish (businessContext, parishRepository, parishNameToGroups, person);
		}


		private static void AssignToParish(BusinessContext businessContext, ParishAddressRepository parishRepository,
			Dictionary<string, AiderGroupEntity> parishNameToGroups, AiderPersonEntity person)
		{
			var mainAddress = ParishAssigner.FindMainAddress (person);

			if (mainAddress.IsNotNull ())
			{
				ParishAssigner.AssignToParish (businessContext, parishRepository, parishNameToGroups, person, mainAddress);
			}

			if (person.Parish.IsNull ())
			{
				ParishAssigner.AssignToNoParishGroup (businessContext, parishNameToGroups, person);
			}
			else
			{
				var path = person.Parish.Group.Path;
				
				foreach (var contact in person.Contacts)
				{
					contact.ParishGroupPathCache = path;
				}
			}
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


		private static void AssignToParish(BusinessContext businessContext, ParishAddressRepository parishRepository,
			Dictionary<string, AiderGroupEntity> parishNameToGroups, AiderPersonEntity person, AiderAddressEntity address)
		{
			var parishGroup = ParishAssigner.FindParishGroup (businessContext, parishRepository, parishNameToGroups, address);

			if (parishGroup == null)
			{
				var nameText    = person.DisplayName;
				var addressText = address.GetSummary ().ToSimpleText ().Replace ("\n", "; ");
				var format      = "WARNING: parish not found for {0} at address {1}";

				Debug.WriteLine (string.Format (format, nameText, addressText));
				return;
			}

			person.Parish = parishGroup.AddParticipant (businessContext, person, null, null, null);
		}


		private static void AssignToNoParishGroup(BusinessContext businessContext, Dictionary<string, AiderGroupEntity> parishNameToGroups, AiderPersonEntity person)
		{
			var parishName = "noparish";
			AiderGroupEntity parishGroup = null;

			if (!parishNameToGroups.TryGetValue (parishName, out parishGroup))
			{
				var example = new AiderGroupEntity ()
				{
					Path = AiderGroupIds.NoParish
				};

				parishGroup = businessContext.DataContext.GetByExample (example).Single ();

				parishNameToGroups[parishName] = parishGroup;
			}

			parishGroup.AddParticipant (businessContext, person, null, null, null);
		}


		public static bool IsInNoParishGroup(BusinessContext businessContext, AiderPersonEntity person)
		{
			var dataContext = businessContext.DataContext;

			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, person, AiderGroupIds.NoParish);

			request.Take = 1;

			return dataContext.GetByRequest (request).Any ();
		}


		private static AiderGroupEntity FindParishGroup(BusinessContext businessContext, ParishAddressRepository parishRepository, Dictionary<string, AiderGroupEntity> parishNameToGroups, AiderAddressEntity address)
		{
			var parishName = ParishLocator.FindParishName (parishRepository, address);

			AiderGroupEntity parishGroup = null;

			if (parishName != null)
			{
				if (!parishNameToGroups.TryGetValue (parishName, out parishGroup))
				{
					parishGroup = AiderGroupEntity.FindParishGroup (businessContext, parishName);

					if (parishGroup != null)
					{
						parishNameToGroups[parishName] = parishGroup;
					}
				}
			}

			return parishGroup;
		}


	}


}
