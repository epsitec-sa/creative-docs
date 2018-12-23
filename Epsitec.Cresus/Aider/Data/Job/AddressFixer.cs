using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using System;

using System.Linq;


namespace Epsitec.Aider.Data.Job
{
    internal static class AddressFixer
    {
        public static void FixForZip(CoreData coreData, int swissZip)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var townExample = new AiderTownEntity ()
                {
                    SwissZipCode = swissZip
                };

                var town = businessContext.DataContext.GetByExample (townExample).Single ();

                var contactExample = new AiderContactEntity ()
                {
                    Address = new AiderAddressEntity ()
                    {
                        Town = town
                    }
                };

                var contacts = businessContext.DataContext.GetByExample (contactExample);

                int count = 0;

                foreach (var contact in contacts)
                {
                    contact.RefreshCache ();
                    count++;
                }

                businessContext.SaveChanges
                (
                    LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
                );
            }

        }
    }
}
