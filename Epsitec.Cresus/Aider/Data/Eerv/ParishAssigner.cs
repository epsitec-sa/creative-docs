using Epsitec.Aider.Entities;
using Epsitec.Aider.Tools;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class ParishAssigner
	{


		public static void AssignToParishes(ParishAddressRepository parishRepository,  BusinessContext businessContext,  IEnumerable<AiderPersonEntity> persons)
		{
			var parishNameToGroups = new Dictionary<string, AiderGroupEntity> ();

			foreach (var person in persons)
			{
				ParishAssigner.AssignToParish (businessContext, parishRepository, parishNameToGroups, person);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static void AssignToParish(BusinessContext businessContext, ParishAddressRepository parishRepository, Dictionary<string, AiderGroupEntity> parishNameToGroups, AiderPersonEntity person)
		{
			// TODO Assign to two parishes if the person has two households ?

			var address = person.GetHouseholds ().First ().Address;
			var parishGroup = ParishAssigner.FindParishGroup (businessContext, parishRepository, parishNameToGroups, address);

			if (parishGroup == null)
			{
				var nameText = person.DisplayName;
				var addressText = address.GetSummary ().ToSimpleText ().Replace ("\n", "; ");

				Debug.WriteLine ("WARNING: parish not found for " + nameText + " at address " + addressText);
			}
			else
			{
				ParishAssigner.AssignToParish (businessContext, person, parishGroup);
			}
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
			person.Parish = parishGroup.AddParticipant (businessContext, person, Date.Today, null, null);
		}


	}


}
