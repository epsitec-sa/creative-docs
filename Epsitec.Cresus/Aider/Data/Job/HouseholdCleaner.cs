using Epsitec.Aider.BusinessCases;
using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Common.IO;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Epsitec.Aider.Data.Job
{


	/// <summary>
	/// </summary>
	internal static class HouseholdCleaner
	{


		public static void FixHouseholds(CoreData coreData)
		{
			Logger.LogToConsole ("START ALL BATCHES");
			
			AiderEnumerator.Execute (coreData, HouseholdCleaner.FixHouseholds);

			Logger.LogToConsole ("DONE ALL BATCHES");
		}


        private static void FixHouseholds
        (
            BusinessContext businessContext,
            IEnumerable<AiderHouseholdEntity> households)
        {
            Logger.LogToConsole("START BATCH");

            foreach (var household in households)
            {
                household.RefreshMembers ();
                if (household.Members.Count == 0)
                {
                    businessContext.DeleteEntity (household);
                    continue;
                }
                var badQuality = false;
                var address = household.Address;
  
                if (address.IsNull ())
                {
                    badQuality = true;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace (address.GetDisplayAddress ().ToSimpleText ()))
                    {
                        badQuality = true;
                    }
                }

                if (badQuality)
                {
                    if (household.Members.Any (m => m.IsGovernmentDefined))
                    {
                        var refMember = household.Members.First(m => m.IsGovernmentDefined);
                        EChDataHelpers.UpdateAiderHouseholdAddress(businessContext, household, refMember.eCH_Person.ReportedPerson1);
                        continue;
                    }
                    else
                    {
                        if (household.Members.SelectMany(m => m.GetParticipations().Where(p => !p.Group.IsNoParish())).Any())
                        {
                            var participations = household.Members.SelectMany(m => m.GetParticipations().Where(p => !p.Group.IsNoParish()));
                            var persons = participations.Select(p => p.Contact.Person).Distinct();
                            foreach (var person in persons)
                            {
                                AiderPersonsProcess.StartExitProcess(businessContext, person, Enumerations.OfficeProcessType.PersonsOutputProcess);
                            }
                            continue;

                        }
                        else
                        {
                            businessContext.DeleteEntity(household);
                            continue;
                        }
                    }


                }


            }
            businessContext.SaveChanges(LockingPolicy.ReleaseLock, EntitySaveMode.None);
			Logger.LogToConsole ("DONE BATCH");
		}

	}


}
