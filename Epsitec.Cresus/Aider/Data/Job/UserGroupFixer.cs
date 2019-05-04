//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
    internal static class UserGroupFixer
    {
        public static void UpgradeUserGroups(CoreData coreData)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var exampleAdminSyst = new SoftwareUserGroupEntity
                {
                    UserPowerLevel = Cresus.Core.Business.UserManagement.UserPowerLevel.Administrator
                };
                var exampleAdminUser = new SoftwareUserGroupEntity
                {
                    UserPowerLevel = Cresus.Core.Business.UserManagement.UserPowerLevel.AdminUser
                };

                var groupAdminSyst = businessContext.GetByExample (exampleAdminSyst).FirstOrDefault ();
                var groupAdminUser = businessContext.GetByExample (exampleAdminUser).FirstOrDefault ();

                groupAdminSyst.Name = "Administrateurs système";

                if (groupAdminUser == null)
                {
                    groupAdminUser = DatabaseInitializer.CreateUserGroup (businessContext, groupAdminSyst.Roles.Single (), "Administrateurs standards", "?", Cresus.Core.Business.UserManagement.UserPowerLevel.AdminUser);
                }

                businessContext.SaveChanges (LockingPolicy.ReleaseLock);
            }
        }
    }
}
