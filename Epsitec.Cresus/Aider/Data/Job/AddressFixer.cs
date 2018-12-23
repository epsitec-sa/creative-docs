//	Copyright © 2018, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using System.Linq;


namespace Epsitec.Aider.Data.Job
{
    internal static class AddressFixer
    {
        public static void FixForZip(CoreData coreData, int swissZip)
        {
            AddressFixer.FixContactsBasedOnZip (coreData, swissZip);
            AddressFixer.FixHouseholdsBasedOnZip (coreData, swissZip);
            AddressFixer.FixLegalPersonBasedOnZip (coreData, swissZip);
            AddressFixer.FixSubscriptionsBasedOnZip (coreData, swissZip);
        }

        private static void FixContactsBasedOnZip(CoreData coreData, int swissZip)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var townExample = new AiderTownEntity ()
                {
                    SwissZipCode = swissZip
                };

                var town = businessContext.DataContext.GetByExample (townExample).Single ();

                var example = new AiderContactEntity ()
                {
                    Address = new AiderAddressEntity ()
                    {
                        Town = town
                    }
                };

                businessContext.DataContext
                    .GetByExample (example)
                    .ForEach (x => x.RefreshCache ());

                businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
            }
        }

        private static void FixHouseholdsBasedOnZip(CoreData coreData, int swissZip)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var townExample = new AiderTownEntity ()
                {
                    SwissZipCode = swissZip
                };

                var town = businessContext.DataContext.GetByExample (townExample).Single ();

                var example = new AiderHouseholdEntity ()
                {
                    Address = new AiderAddressEntity ()
                    {
                        Town = town
                    }
                };

                businessContext.DataContext
                    .GetByExample (example)
                    .ForEach (x => x.RefreshCache ());

                businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
            }
        }

        private static void FixLegalPersonBasedOnZip(CoreData coreData, int swissZip)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var townExample = new AiderTownEntity ()
                {
                    SwissZipCode = swissZip
                };

                var town = businessContext.DataContext.GetByExample (townExample).Single ();

                var example = new AiderLegalPersonEntity ()
                {
                    Address = new AiderAddressEntity ()
                    {
                        Town = town
                    }
                };

                businessContext.DataContext
                    .GetByExample (example)
                    .ForEach (x => x.RefreshCache ());

                businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
            }
        }

        private static void FixSubscriptionsBasedOnZip(CoreData coreData, int swissZip)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var example = new AiderSubscriptionEntity ()
                {
                    DisplayZipCode = swissZip.ToString ()
                };

                businessContext.DataContext
                    .GetByExample (example)
                    .ForEach (x => x.RefreshCache ());

                businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
            }
        }
    }
}
