//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Tools;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class ParishAssigner
	{


		public static void AssignToParishes(ParishAddressRepository parishRepository, BusinessContext businessContext, IEnumerable<AiderContactEntity> contacts)
		{
			var parishNameToGroups = new Dictionary<string, AiderGroupEntity> ();

			foreach (var contact in contacts)
			{
				ParishAssigner.AssignToParish (businessContext, parishRepository, parishNameToGroups, contact);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static void AssignToParish(BusinessContext businessContext, ParishAddressRepository parishRepository,
			Dictionary<string, AiderGroupEntity> parishNameToGroups, AiderContactEntity contact)
		{
			var address = contact.Address;
			var person  = contact.Person;

			if (person.IsNotNull () && address.IsNotNull ())
			{
				ParishAssigner.AssignToParish (businessContext, parishRepository, parishNameToGroups, person, address);

				var parish = person.ParishGroup;

				if (parish.IsNotNull ())
				{
					contact.ParishGroupPathCache = parish.Path;
				}
			}
		}

		private static void AssignToParish(BusinessContext businessContext, ParishAddressRepository parishRepository,
			Dictionary<string, AiderGroupEntity> parishNameToGroups, AiderPersonEntity person, AiderAddressEntity address)
		{
			if (address.Town.IsNull ())
			{
				return;
			}

			var parishGroup = ParishAssigner.FindParishGroup (businessContext, parishRepository, parishNameToGroups, address);

			if (parishGroup == null)
			{
				var nameText    = person.DisplayName;
				var addressText = address.GetSummary ().ToSimpleText ().Replace ("\n", "; ");
				var format      = "WARNING: parish not found for {0} at address {1}";

				Debug.WriteLine (string.Format (format, nameText, addressText));

				return;
			}

			ParishAssigner.AssignToParish (businessContext, person, parishGroup);
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


		private static void AssignToParish(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity parishGroup)
		{
			var participation = parishGroup.AddParticipant (businessContext, person, null, null, null);

			if (person.Parish.IsNull ())
			{
				person.Parish = participation;
			}
		}


	}


}
