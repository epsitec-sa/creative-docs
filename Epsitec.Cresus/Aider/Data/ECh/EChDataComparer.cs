//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel Loup, Maintainer: Samuel Loup


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{

	/// <summary>
	/// Compare Ech XML data files, provide extraction methods for diff based on buisiness rules
	/// </summary>
    class EChDataComparer : IDisposable
    {

        /// <summary>
        /// Initialize a new EChDataComparer with two files
        /// </summary>
        /// <param name="oldEchFile">old ECh XML data file</param>
        /// <param name="newEchFile">new ECh XML data file</param>
        public EChDataComparer(string oldEchFile, string newEchFile)
        {
            var origineEch = EChDataLoader.Load(new FileInfo(oldEchFile), int.MaxValue).ToList();
            var versionEch = EChDataLoader.Load(new FileInfo(newEchFile), int.MaxValue).ToList();
            this.CreateDictionaryFromEntity(origineEch, versionEch);
            origineEch = null;
            versionEch = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DicFamilyA = null;
                this.DicFamilyB = null;
                this.DicPersonA = null;
                this.DicPersonB = null;
            }          
        }

        /// <summary>
        /// Create a list of ECh ReportedPerson to remove from database
        /// </summary>
        /// <returns>a list of ReportedPerson to remove</returns>
        public IList<EChReportedPerson> GetFamilyToRemove()
        {
            var FamilyToBeRemoved = (from e in DicFamilyA
                                     where !DicFamilyB.ContainsKey(e.Key)
                                     select e.Value).ToList();

            return FamilyToBeRemoved;
        }

        /// <summary>
        /// Create a list of ECh ReportedPerson to add in the database
        /// </summary>
        /// <returns>a list of ReportedPerson to add</returns>
        public IList<EChReportedPerson> GetFamilyToAdd()
        {
            var FamilyToBeAdded = (from n in DicFamilyB
                                   where !DicFamilyA.ContainsKey(n.Key)
                                   select n.Value).ToList();

            return FamilyToBeAdded;
        }

        /// <summary>
        /// Create a list of ECh ReportedPerson to modify
        /// </summary>
        /// <returns>a list of modified ECh ReportedPerson</returns>
        public IList<System.Tuple<EChReportedPerson, EChReportedPerson>> GetFamilyToChange()
        {
            var FamilyToCheck = from e in DicFamilyB
                                where DicFamilyA.ContainsKey(e.Key)
                                select e;

            var FamilyToChange = (from c in FamilyToCheck
                                  join e in DicFamilyA on c.Key equals e.Key
                                  where !c.Value.CheckData(e.Value.Address.HouseNumber, e.Value.Address.CountryCode, e.Value.Address.AddressLine1, e.Value.Address.Street, e.Value.Address.SwissZipCode, e.Value.Address.SwissZipCodeAddOn, e.Value.Address.SwissZipCodeId, e.Value.Address.Town)
                                  select System.Tuple.Create(c.Value, e.Value)).ToList();

            return FamilyToChange;
        }

        /// <summary>
        /// Create a list of EChPerson to remove from database
        /// </summary>
        /// <returns>a list of EChPerson to remove</returns>
        public IList<EChPerson> GetPersonToRemove()
        {
            var PersonToBeRemoved = (from e in DicPersonA
                                     where !DicPersonB.ContainsKey(e.Key)
                                     select e.Value).ToList();

            return PersonToBeRemoved;
        }

        /// <summary>
        /// Create a list of EChPerson to add in the database
        /// </summary>
        /// <returns>a list of EChPerson to add</returns>
        public IList<EChPerson> GetPersonToAdd()
        {
            var PersonToBeAdded = (from n in DicPersonB
                                   where !DicPersonA.ContainsKey(n.Key)
                                   select n.Value).ToList();

            return PersonToBeAdded;
        }

        /// <summary>
        /// Create a list of tuple containing new EChPerson and old EChPerson
        /// </summary>
        /// <returns>a list of modified EChPerson</returns>
        public IList<System.Tuple<EChPerson, EChPerson>> GetPersonToChange()
        {
            var PersonToCheck = from e in DicPersonB
                                where DicPersonA.ContainsKey(e.Key)
                                select e;

            var PersonToChange = (from c in PersonToCheck
                                  join e in DicPersonA on c.Key equals e.Key
                                  where !c.Value.CheckData(e.Value.OfficialName, e.Value.FirstNames, e.Value.DateOfBirth, e.Value.Sex, e.Value.NationalityStatus, e.Value.NationalCountryCode, e.Value.MaritalStatus, e.Value.OriginPlaces)
                                  select System.Tuple.Create(c.Value, e.Value)).ToList();

            return PersonToChange;
        }

        private void CreateDictionaryFromEntity(List<EChReportedPerson> origineEch, List<EChReportedPerson> versionEch)
        {

            this.DicFamilyA = new Dictionary<string, EChReportedPerson>();
            this.DicFamilyB = new Dictionary<string, EChReportedPerson>();
            this.DicPersonA = new Dictionary<string, EChPerson>();
            this.DicPersonB = new Dictionary<string, EChPerson>();

            foreach (EChReportedPerson Fam in origineEch)
            {
                this.DicFamilyA.Add(Fam.FamilyKey, Fam);

                this.DicPersonA.Add(Fam.Adult1.Id, Fam.Adult1);
                if (Fam.Adult2 != null)
                {
                    this.DicPersonA.Add(Fam.Adult2.Id, Fam.Adult2);
                }
                foreach (EChPerson per in Fam.Children)
                {
                    if (!this.DicPersonA.ContainsKey(per.Id))
                        this.DicPersonA.Add(per.Id, per);
                }
            }


            foreach (EChReportedPerson Fam in versionEch)
            {
                this.DicFamilyB.Add(Fam.FamilyKey, Fam);
                this.DicPersonB.Add(Fam.Adult1.Id, Fam.Adult1);
                if (Fam.Adult2 != null)
                {
                    this.DicPersonB.Add(Fam.Adult2.Id, Fam.Adult2);
                }
                foreach (EChPerson per in Fam.Children)
                {
                    if (!this.DicPersonB.ContainsKey(per.Id))
                        this.DicPersonB.Add(per.Id, per);
                }
            }

        }

        //Comparison dictionary
        private Dictionary<string, EChReportedPerson> DicFamilyA;
        private Dictionary<string, EChReportedPerson> DicFamilyB;
        private Dictionary<string, EChPerson> DicPersonA;
        private Dictionary<string, EChPerson> DicPersonB;

    }
}
