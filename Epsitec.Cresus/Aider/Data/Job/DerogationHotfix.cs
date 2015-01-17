using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Data.Job
{
	internal static class DerogationHotfix
    {
        public static void Hotfix(CoreData coreData)
        {
            Console.WriteLine("HOTFIX: SLO");
            using (var businessContext = new BusinessContext(coreData, false))
            {
				var fixDate = new Epsitec.Common.Types.Date (2014,3,18);
				var derogationLetters = businessContext.GetByExample<AiderOfficeLetterReportEntity> (new AiderOfficeLetterReportEntity ());
				foreach (var letter in derogationLetters)
				{
					var contact = letter.RecipientContact;
					var participationsExample = new AiderGroupParticipantEntity ()
					{
						Contact = contact
					};

					if (contact.Person.HasDerogation)
					{
						var oldParishGroup = AiderGroupEntity.FindGroups (businessContext, contact.Person.GeoParishGroupPathCache).Single ();

						var closedParticipationsToRestore = businessContext.GetByExample<AiderGroupParticipantEntity> (participationsExample).Where (p => p.EndDate > fixDate 
																				&& p.Group != oldParishGroup);

						foreach (var participation in closedParticipationsToRestore)
						{
							AiderGroupParticipantEntity.RestoreParticipation (businessContext, participation);
						}
					}
					else
					{
						var falseParishGroup = letter.Office.ParishGroup;

						var closedParticipationsToRestore = businessContext.GetByExample<AiderGroupParticipantEntity> (participationsExample).Where (p => p.EndDate > fixDate 
																	&& p.Group != falseParishGroup 
																	&& p.Group.GroupDef.Classification != Enumerations.GroupClassification.DerogationIn
																	&& p.Group.GroupDef.Classification != Enumerations.GroupClassification.DerogationOut
													);
						foreach (var participation in closedParticipationsToRestore)
						{
							AiderGroupParticipantEntity.RestoreParticipation (businessContext, participation);
						}
					}
					
				}
			

                businessContext.SaveChanges(LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);

            }
            
        }
    }
}
