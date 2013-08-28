using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Data.Job
{
    internal static class WarningParishGroupPathFixer
    {
        public static void StartJob(CoreData coreData)
        {
            Console.WriteLine("AIDER PERSON WARNING ENTITY : START PARISH GROUP PATH FIXER");
            using (var businessContext = new BusinessContext(coreData, false))
            {
                var count = 0;
                foreach (var warning in businessContext.GetAllEntities<AiderPersonWarningEntity>())
                {
                    warning.ParishGroupPath = warning.Person.ParishGroupPathCache;
                    count++;
                }
                Console.WriteLine("AIDER PERSON WARNING ENTITY : " + count + " ENTITY FIXED, SAVING...");
                businessContext.SaveChanges(LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
                Console.WriteLine("AIDER PERSON WARNING ENTITY : JOB DONE!");
            }
            
        }
    }
}
