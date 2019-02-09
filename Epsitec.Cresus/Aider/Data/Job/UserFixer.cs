//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
    /// <summary>
    /// Update CalculatedAge on persons
    /// </summary>
    internal static class UserFixer
    {
        public static void RemoveEmptyUsers(CoreData coreData)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var exampleActive = new AiderUserEntity
                {
                    Disabled = false,
                };
                var exampleDisabled = new AiderUserEntity
                {
                    Disabled = true,
                };
                
                var activeUsers = businessContext.DataContext
                    .GetByExample (exampleActive);
                var disabledUsers = businessContext.DataContext
                    .GetByExample (exampleDisabled);

                var adminUsers = activeUsers
                    .Concat (disabledUsers)
                    .Where (u => u.PowerLevel != Cresus.Core.Business.UserManagement.UserPowerLevel.None)
                    .Where (u => u.PowerLevel <= Cresus.Core.Business.UserManagement.UserPowerLevel.Administrator)
                    .ToList ();
                
                System.Console.WriteLine ("{0} active users found, {1} archived", activeUsers.Count, activeUsers.Count (u => u.IsArchive));
                System.Console.WriteLine ("{0} disabled users found, {1} archived", disabledUsers.Count, disabledUsers.Count (u => u.IsArchive));
                System.Console.WriteLine ("{0} active admins found, {1} inactive ", adminUsers.Count (u => u.IsActive), adminUsers.Count (u => u.IsActive == false));

                foreach (var user in activeUsers.Concat (disabledUsers).Where (x => string.IsNullOrEmpty (x.DisplayName)))
                {
                    businessContext.DeleteEntity (user);
                }

                System.Console.WriteLine ("{0} users without e-mail address", activeUsers.Concat (disabledUsers).Count (x => string.IsNullOrEmpty (x.Email)));



                businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
            }
        }
        public static void RemoveInvalidUsers(CoreData coreData)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var example = new AiderUserEntity
                {
                    LoginName = "",
                    IsArchive = false,
                };

                var users = businessContext.DataContext
                    .GetByExample (example);

                System.Console.WriteLine ("{0} invalid users found", users.Count);

                foreach (var user in users)
                {
                    user.Delete (businessContext);
                }

                businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
            }
        }
    }
}
