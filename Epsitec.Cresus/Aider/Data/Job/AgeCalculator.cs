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
    internal sealed class AgeCalculator : System.IDisposable
    {
        public static void Start(CoreData coreData)
        {
            using (var calculator = new AgeCalculator (coreData))
            {
                AiderEnumerator.Execute (coreData, calculator.UpdateCalculatedAge);
            }
        }

        public static void UpdateBirthdayOfToday(CoreData coreData)
        {
            using (var calculator = new AgeCalculator (coreData))
            {
                calculator.UpdateCalculatedAge (System.DateTime.Now);
            }
        }

        private AgeCalculator(CoreData coreData)
        {
            this.coreData = coreData;
            this.watch = new Stopwatch ();
            this.countAgeChange = 0;
            this.countMademoiselle = 0;
            this.countMadame = 0;
            this.countMonsieur = 0;

            this.watch.Start ();
        }

        void System.IDisposable.Dispose()
        {
            this.watch.Stop ();

            System.Console.WriteLine ("Global update took {0}ms", this.watch.ElapsedMilliseconds);
            var message = string.Format (
                "Updated {0} ages, {1} => Madame, {2} => Mademoiselle, {3} => Monsieur",
                this.countAgeChange, this.countMademoiselle, this.countMadame, this.countMonsieur);

            System.Console.Write (message);
            System.Diagnostics.Trace.WriteLine (message);
        }


        private void UpdateCalculatedAge(BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons)
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
                    if ((age.Value >= 18) &&
                        (person.MrMrs != Enumerations.PersonMrMrs.Madame) &&
                        (person.eCH_Person.PersonSex == Enumerations.PersonSex.Female))
                    {
                        person.MrMrs = Enumerations.PersonMrMrs.Madame;
                        this.countMademoiselle++;
                    }
                    else if ((age.Value < 18) &&
                        (person.MrMrs != Enumerations.PersonMrMrs.Mademoiselle) &&
                        (person.eCH_Person.PersonSex == Enumerations.PersonSex.Female))
                    {
                        person.MrMrs = Enumerations.PersonMrMrs.Mademoiselle;
                        this.countMadame++;
                    }
                    else if ((age.Value >= 18) &&
                        (person.MrMrs != Enumerations.PersonMrMrs.Monsieur) &&
                        (person.eCH_Person.PersonSex == Enumerations.PersonSex.Male))
                    {
                        person.MrMrs = Enumerations.PersonMrMrs.Monsieur;
                        this.countMonsieur++;
                    }
                }
            }

            businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
        }

        private void UpdateCalculatedAge(System.DateTime today)
        {
            using (var businessContext = new BusinessContext (this.coreData, false))
            {
                var example = new AiderPersonEntity ()
                {
                    BirthdayDay = today.Day,
                    BirthdayMonth = today.Month,
               };

                var persons = businessContext.DataContext
                    .GetByExample (example);

                System.Console.WriteLine ("{0} persons found", persons.Count);

                this.UpdateCalculatedAge (businessContext, persons);
            }
        }

        private readonly CoreData coreData;
        private readonly Stopwatch watch;

        private int countAgeChange;
        private int countMademoiselle;
        private int countMadame;
        private int countMonsieur;
    }
}
