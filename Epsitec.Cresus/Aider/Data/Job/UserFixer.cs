//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
    internal static class UserFixer
    {
        public static void RemoveEmptyUsers(CoreData coreData, UserRemovalMode mode)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var exampleActive   = new AiderUserEntity { Disabled = false };
                var exampleDisabled = new AiderUserEntity { Disabled = true };
                
                var activeUsers   = businessContext.GetByExample (exampleActive);
                var disabledUsers = businessContext.GetByExample (exampleDisabled);
                var allUsers      = activeUsers.Concat (disabledUsers).ToList ();

                var adminUsers = allUsers
                    .Where (u => u.IsAdmin ())
                    .ToList ();

                var emptyUsers = allUsers
                    .Where (u => string.IsNullOrEmpty (u.DisplayName))
                    .ToList ();

                var contactlessUsers = allUsers
                    .Where (u => u.Contact.IsNull ())
                    .ToList ();

                var noEmailUsers = allUsers
                    .Where (x => string.IsNullOrEmpty (x.Email) || x.Email == "truc@truc.ch")
                    .ToList ();

                var notAnEmployeeUsers = allUsers
                    .Where (x => x.Contact.Person.Employee.IsNull ())
                    .ToList ();

                var brokenEmployeeUsers = allUsers
                    .Where (x => x.Contact.Person.Employee.IsNotNull ())
                    .Where (x => x.Contact.Person.Employee.IsUser () == false)
                    .ToList ();

                var buffer = new System.Text.StringBuilder ();

                buffer.AppendFormat ("{0} active users found, {1} archived\r\n", activeUsers.Count, activeUsers.Count (u => u.IsArchive));
                buffer.AppendFormat ("{0} disabled users found, {1} archived\r\n", disabledUsers.Count, disabledUsers.Count (u => u.IsArchive));
                buffer.AppendFormat ("{0} active admins found, {1} inactive\r\n", adminUsers.Count (u => u.IsActive), adminUsers.Count (u => u.IsActive == false));
                buffer.AppendFormat ("{0} users without an e-mail address\r\n", noEmailUsers.Count);
                buffer.AppendFormat ("{0} empty users\r\n", emptyUsers.Count);
                buffer.AppendFormat ("{0} contactless users\r\n", contactlessUsers.Count);
                buffer.AppendFormat ("{0} not employees\r\n", notAnEmployeeUsers.Count);
                buffer.AppendFormat ("{0} broken employees\r\n", brokenEmployeeUsers.Count);

                adminUsers.ForEach (x => buffer.AppendFormat ("Administrator {0} ({1})\r\n", x.DisplayName, x.Email));
                contactlessUsers.ForEach (x => buffer.AppendFormat ("Contactless User {0} ({1})\r\n", x.DisplayName, x.Email));
                noEmailUsers.ForEach (x => buffer.AppendFormat ("No e-mail for {0}\r\n", x.DisplayName));
                notAnEmployeeUsers.ForEach (x => buffer.AppendFormat ("Not an employee: {0} ({1})\r\n", x.DisplayName, x.Email));
                brokenEmployeeUsers.ForEach (x => buffer.AppendFormat ("Broken employee: {0} ({1})\r\n", x.DisplayName, x.Email));

                System.Console.OutputEncoding = System.Text.Encoding.GetEncoding (850);
                System.Console.WriteLine (buffer.ToString ());

                System.IO.File.WriteAllText ("aider-users.log", buffer.ToString (), System.Text.Encoding.UTF8);

                var removes = new HashSet<AiderUserEntity> ();

                if (mode.HasFlag (UserRemovalMode.Empty))
                {
                    removes.AddRange (emptyUsers);
                }
                if (mode.HasFlag (UserRemovalMode.NoContact))
                {
                    removes.AddRange (contactlessUsers);
                }
                if (mode.HasFlag (UserRemovalMode.NoEmail))
                {
                    removes.AddRange (noEmailUsers);
                }

                removes.RemoveWhere (x => x.IsAdmin ());
                removes.ForEach (x => System.Console.WriteLine ("Remove {0} ({1})", x.DisplayName, x.Email));

                System.Console.WriteLine ("Remove {0} users? (type yes)", removes.Count);
                var answer = System.Console.ReadLine ();

                if (answer == "yes")
                {
                    foreach (var user in removes)
                    {
                        businessContext.DeleteEntity (user);
                    }
                }

                businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
            }
        }
    }
}
