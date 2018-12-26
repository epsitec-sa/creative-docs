//	Copyright © 2013-2018, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Diagnostics;

namespace Epsitec.Aider.Data.Job
{
    /// <summary>
    /// Update CalculatedAge on persons
    /// </summary>
    internal class AgeCalculator
    {
        public static void Start(CoreData coreData)
        {
            var calculator = new AgeCalculator (coreData);
            calculator.Process ();
        }

        public AgeCalculator(CoreData coreData)
        {
            this.coreData = coreData;
            this.watch = new Stopwatch ();
            this.countAgeChange = 0;
            this.countMademoiselle = 0;
            this.countMadame = 0;

        }

        private void Process()
        {
            this.watch.Start ();
            AiderEnumerator.Execute (this.coreData, this.CalculateAge);
            this.watch.Stop ();

            System.Console.WriteLine ("Global update took {0}ms", this.watch.ElapsedMilliseconds);
            System.Console.WriteLine ("Updated {0} ages, {1} Mademoiselle => Madame, {2} Madame => Mademoiselle", this.countAgeChange, this.countMademoiselle, this.countMadame);
            System.Console.ReadLine ();
        }


        private void CalculateAge(BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons)
        {
            foreach (var person in persons)
            {
                var age = person.Age;

                if (person.CalculatedAge != age)
                {
                    person.CalculatedAge = age;
                    this.countAgeChange++;
                }

                if (age.HasValue)
                {
                    if ((age.Value >= 18) && (person.MrMrs == Enumerations.PersonMrMrs.Mademoiselle))
                    {
                        person.MrMrs = Enumerations.PersonMrMrs.Madame;
                        this.countMademoiselle++;
                    }
                    else if ((age.Value < 18) && (person.MrMrs == Enumerations.PersonMrMrs.Madame))
                    {
                        person.MrMrs = Enumerations.PersonMrMrs.Mademoiselle;
                        this.countMadame++;
                    }
                }
            }

            businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
        }

        private static void UpdateCalculatedAge(CoreData coreData)
        {
            using (var businessContext = new BusinessContext (coreData, false))
            {
                var example = new AiderPersonEntity ()
                {
                    BirthdayDay = 1,
                    BirthdayMonth = 1,
                };

                businessContext.DataContext
                    .GetByExample (example)
                    .ForEach (x => x.CalculatedAge = x.Age);

                businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
            }
        }

        private readonly CoreData coreData;
        private readonly Stopwatch watch;
        private int countAgeChange;
        private int countMademoiselle;
        private int countMadame;
    }
}
