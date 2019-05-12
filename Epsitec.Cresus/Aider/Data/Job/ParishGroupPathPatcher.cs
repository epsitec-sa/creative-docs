//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
    internal static class ParishGroupPathPatcher
    {
        public static void PatchParishGroupPath(CoreData coreData, string oldPath, string newPath, string oldRegion, string newRegion)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var dataContext = businessContext.DataContext;
                var pathPattern = oldPath + "%";

                var exampleGroup = new AiderGroupEntity ();
                var groupRequest = Request.Create (exampleGroup);

                groupRequest.AddCondition (dataContext, exampleGroup, x => SqlMethods.Like (x.Path, pathPattern));

                var listOfGroup = dataContext.GetByRequest (groupRequest);


                var exampleReferee = new AiderRefereeEntity ();
                var requestReferee = Request.Create (exampleReferee);

                var exampleEmployee = new AiderEmployeeEntity ();
                var requestEmployee = Request.Create (exampleEmployee);

                var exampleEmployeeJob = new AiderEmployeeJobEntity ();
                var requestEmployeeJob = Request.Create (exampleEmployeeJob);

                var examplePersonWarning = new AiderPersonWarningEntity ();
                var requestPersonWarning = Request.Create (examplePersonWarning);

                requestEmployee.AddCondition (dataContext, exampleEmployee, x => SqlMethods.Like (x.ParishGroupPath, pathPattern));
                requestEmployeeJob.AddCondition (dataContext, exampleEmployeeJob, x => SqlMethods.Like (x.ParishGroupPath, pathPattern));
                requestPersonWarning.AddCondition (dataContext, examplePersonWarning, x => SqlMethods.Like (x.ParishGroupPath, pathPattern));
                requestReferee.AddCondition (dataContext, exampleReferee, x => SqlMethods.Like (x.ParishGroupPath, pathPattern));

                var listOfEmployee = dataContext.GetByRequest (requestEmployee);
                var listOfEmployeeJob = dataContext.GetByRequest (requestEmployeeJob);
                var listOfPersonWarning  = dataContext.GetByRequest (requestPersonWarning);
                var listOfReferee = dataContext.GetByRequest (requestReferee);


                var examplePerson = new AiderPersonEntity ();
                var requestPerson = Request.Create (examplePerson);

                var exampleHousehold = new AiderHouseholdEntity ();
                var requestHousehold = Request.Create (exampleHousehold);

                var exampleEvent = new AiderEventEntity ();
                var requestEvent = Request.Create (exampleEvent);

                var exampleEventParticipant = new AiderEventParticipantEntity ();
                var requestEventParticipant = Request.Create (exampleEventParticipant);

                var exampleUser = new AiderUserEntity ();
                var requestUser = Request.Create (exampleUser);

                var exampleContact = new AiderContactEntity ();
                var requestContact = Request.Create (exampleContact);

                var exampleLegalPerson = new AiderLegalPersonEntity ();
                var requestLegalPerson = Request.Create (exampleLegalPerson);

                var exampleSubscription = new AiderSubscriptionEntity ();
                var requestSubscription = Request.Create (exampleSubscription);

                var exampleSubscriptionRefusal = new AiderSubscriptionRefusalEntity ();
                var requestSubscriptionRefusal = Request.Create (exampleSubscriptionRefusal);

                var exampleMailing = new AiderMailingEntity ();
                var requestMailing = Request.Create (exampleMailing);

                var exampleOfficeManagement = new AiderOfficeManagementEntity ();
                var requestOfficeManagement = Request.Create (exampleOfficeManagement);

                var exampleOfficeSender = new AiderOfficeSenderEntity ();
                var requestOfficeSender = Request.Create (exampleOfficeSender);

                var exampleDataManagers = new AiderDataManagersEntity ();
                var requestDataManagers = Request.Create (exampleDataManagers);

                requestPerson.AddCondition (dataContext, examplePerson, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestHousehold.AddCondition (dataContext, exampleHousehold, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestEvent.AddCondition (dataContext, exampleEvent, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestEventParticipant.AddCondition (dataContext, exampleEventParticipant, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestUser.AddCondition (dataContext, exampleUser, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestContact.AddCondition (dataContext, exampleContact, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestLegalPerson.AddCondition (dataContext, exampleLegalPerson, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestSubscription.AddCondition (dataContext, exampleSubscription, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestSubscriptionRefusal.AddCondition (dataContext, exampleSubscriptionRefusal, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestMailing.AddCondition (dataContext, exampleMailing, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestOfficeManagement.AddCondition (dataContext, exampleOfficeManagement, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestOfficeSender.AddCondition (dataContext, exampleOfficeSender, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));
                requestDataManagers.AddCondition (dataContext, exampleDataManagers, x => SqlMethods.Like (x.ParishGroupPathCache, pathPattern));

                var listOfPerson = dataContext.GetByRequest (requestPerson);
                var listOfHousehold = dataContext.GetByRequest (requestHousehold);
                var listOfEvent = dataContext.GetByRequest (requestEvent);
                var listOfEventParticipant = dataContext.GetByRequest (requestEventParticipant);
                var listOfUser = dataContext.GetByRequest (requestUser);
                var listOfContact = dataContext.GetByRequest (requestContact);
                var listOfLegalPerson = dataContext.GetByRequest (requestLegalPerson);
                var listOfSubscription = dataContext.GetByRequest (requestSubscription);
                var listOfSubscriptionRefusal = dataContext.GetByRequest (requestSubscriptionRefusal);
                var listOfMailing = dataContext.GetByRequest (requestMailing);
                var listOfOfficeManagement = dataContext.GetByRequest (requestOfficeManagement);
                var listOfOfficeSender = dataContext.GetByRequest (requestOfficeSender);
                var listOfDataManagers = dataContext.GetByRequest (requestDataManagers);

                listOfGroup.ForEach (x => x.Path = x.Path.Replace (oldPath, newPath));

                listOfEmployee.ForEach (x => x.ParishGroupPath = x.ParishGroupPath.Replace (oldPath, newPath));
                listOfEmployeeJob.ForEach (x => x.ParishGroupPath = x.ParishGroupPath.Replace (oldPath, newPath));
                listOfPersonWarning.ForEach (x => x.ParishGroupPath = x.ParishGroupPath.Replace (oldPath, newPath));
                listOfReferee.ForEach (x => x.ParishGroupPath = x.ParishGroupPath.Replace (oldPath, newPath));

                listOfPerson.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfHousehold.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfEvent.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfEventParticipant.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfUser.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfContact.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfLegalPerson.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfSubscription.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfSubscriptionRefusal.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfMailing.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfOfficeManagement.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfOfficeManagement.ForEach (x => x.Region = x.Region.Replace (oldRegion, newRegion));
                listOfOfficeSender.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));
                listOfDataManagers.ForEach (x => x.ParishGroupPathCache = x.ParishGroupPathCache.Replace (oldPath, newPath));

                businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
            }
        }
    }
}
