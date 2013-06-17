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
	class EChDataComparer
	{
		
		/// <summary>
		/// Initialize a new EChDataComparer with two files
		/// </summary>
		/// <param name="OldEchFile">old ECh XML data file</param>
		/// <param name="NewEchFile">new ECh XML data file</param>
		public EChDataComparer(string OldEchFile,string NewEchFile)
		{
			this.EchFileA = OldEchFile;
			this.EchFileB = NewEchFile;

			this.LoadFilesToCompare ();
			this.CreateDictionaryFromEntity ();
		}

		/// <summary>
		/// Create a list of ECh ReportedPerson to remove from database
		/// </summary>
		/// <returns>a list of ReportedPerson to remove</returns>
		public List<EChReportedPerson> GetFamilyToRemove()
		{
			var FamilyToBeRemoved = (from e in DicFamilyA
									 where !DicFamilyB.ContainsKey (e.Key)
									 select e.Value).ToList ();

			return FamilyToBeRemoved;
		}

		/// <summary>
		/// Create a list of ECh ReportedPerson to add in the database
		/// </summary>
		/// <returns>a list of ReportedPerson to add</returns>
		public List<EChReportedPerson> GetFamilyToAdd()
		{
			var FamilyToBeAdded = (from n in DicFamilyB
								   where !DicFamilyA.ContainsKey (n.Key)
								   select n.Value).ToList ();

			return FamilyToBeAdded;
		}

		/// <summary>
		/// Create a list of ECh ReportedPerson to modify
		/// </summary>
		/// <returns>a list of modified ECh ReportedPerson</returns>
		public List<System.Tuple<EChReportedPerson,EChReportedPerson>> GetFamilyToChange()
		{
			var FamilyToCheck = from e in DicFamilyB
								where DicFamilyA.ContainsKey (e.Key)
								select e;

			var FamilyToChange = (from c in FamilyToCheck
								  join e in DicFamilyA on c.Key equals e.Key
								  where !c.Value.CheckData (e.Value.Address.HouseNumber, e.Value.Address.CountryCode, e.Value.Address.AddressLine1, e.Value.Address.Street, e.Value.Address.SwissZipCode, e.Value.Address.SwissZipCodeAddOn, e.Value.Address.SwissZipCodeId, e.Value.Address.Town)
                                  select System.Tuple.Create(c.Value, e.Value)).ToList();

			return FamilyToChange;
		}

		/// <summary>
		/// Create a list of EChPerson to remove from database
		/// </summary>
		/// <returns>a list of EChPerson to remove</returns>
		public List<EChPerson> GetPersonToRemove()
		{
			var PersonToBeRemoved = (from e in DicPersonA
									 where !DicPersonB.ContainsKey (e.Key)
									 select e.Value).ToList ();

			return PersonToBeRemoved;
		}

		/// <summary>
		/// Create a list of EChPerson to add in the database
		/// </summary>
		/// <returns>a list of EChPerson to add</returns>
		public List<EChPerson> GetPersonToAdd()
		{
			var PersonToBeAdded = (from n in DicPersonB
								   where !DicPersonA.ContainsKey (n.Key)
								   select n.Value).ToList ();

			return PersonToBeAdded;
		}

		/// <summary>
		/// Create a list of tuple containing new EChPerson and old EChPerson
		/// </summary>
		/// <returns>a list of modified EChPerson</returns>
		public List<System.Tuple<EChPerson,EChPerson>> GetPersonToChange()
		{
			var PersonToCheck =  from e in DicPersonB
								 where DicPersonA.ContainsKey (e.Key)
								 select e;

			var PersonToChange = (from c in PersonToCheck
								  join e in DicPersonA on c.Key equals e.Key
								  where !c.Value.CheckData (e.Value.OfficialName, e.Value.FirstNames, e.Value.DateOfBirth, e.Value.Sex, e.Value.NationalityStatus, e.Value.NationalCountryCode, e.Value.MaritalStatus,  e.Value.OriginPlaces)
								  select System.Tuple.Create(c.Value,e.Value)).ToList ();

			return PersonToChange;
		}


        public void AnalyseChanges()
        {
            var familyToAdd = new Dictionary<string, EChReportedPerson>();
            var familyToRemove = new Dictionary<string, EChReportedPerson>();
            var personToAdd = new Dictionary<string, EChPerson>();
            var personToRemove = new Dictionary<string, EChPerson>();

            var newFamily = new List<EChReportedPerson>();
            var newFamilyPersonOnly = new List<EChReportedPerson>();
            var newFamilyPersonOnlyWithChildren = new List<EChReportedPerson>();
            var newBirth = new List<EChPerson>();
            var childMissing = new List<EChPerson>();
            var gainMajority = new List<EChReportedPerson>();
            var missingFamily = new List<EChReportedPerson>();
            var missingFamilyPersonOnly = new List<EChReportedPerson>();
            var missingFamilyPersonOnlyWithChildren = new List<EChReportedPerson>();
            var errorFamily = new List<EChReportedPerson>();

            var addCaseToResolve = new List<EChReportedPerson>();
            var remCaseToResolve = new List<EChReportedPerson>();



            foreach (var person in this.GetPersonToAdd())
            {
                personToAdd.Add(person.Id, person);
            }

            foreach (var person in this.GetPersonToRemove())
            {
                personToRemove.Add(person.Id, person);
            }

            foreach (var family in this.GetFamilyToAdd())
            {
                familyToAdd.Add(family.FamilyKey, family);
            }

            foreach (var family in this.GetFamilyToRemove())
            {
                familyToRemove.Add(family.FamilyKey, family);
            }

            foreach (var family in familyToAdd.Values)
            {
                //check adult composition
                if (family.Adult2 != null)
                {
                    var isNewFamily = isNewFamilyArrival(family, personToAdd); //check for a completely new family
                    if (isNewFamily)
                    {
                        newFamily.Add(family);
                    }
                    else
                    {
                        foreach (var child in family.Children)
                        {
                            if (personToAdd.ContainsKey(child.Id))
                            {
                                //birth!
                                newBirth.Add(child);
                            }
                            if (personToRemove.ContainsKey(child.Id))
                            {
                                //Miss :/
                                childMissing.Add(child);
                            }
                        }
                    }
                }
                else
                {
                    //check mono-adult cases
                    if (personToAdd.ContainsKey(family.Adult1.Id))
                    {
                        if (family.Children.Count > 0)
                        {
                            newFamilyPersonOnlyWithChildren.Add(family);
                        }
                        else
                        {
                            newFamilyPersonOnly.Add(family);
                        }
                        
                    }
                    else
                    {
                        addCaseToResolve.Add(family);
                    }
                }
            }

            foreach (var family in familyToRemove.Values)
            {
                //check adult composition
                if (family.Adult2 != null)
                {
                    var isFamilyDep = isFamilyDeparture(family, personToRemove); //check for a completely removal of the family in the register
                    if (isFamilyDep)
                    {
                        missingFamily.Add(family);
                    }
                    else
                    {
                        foreach (var child in family.Children)
                        {
                            if (familyToAdd.ContainsKey(child.Id))
                            {
                                //Majority? 
                                gainMajority.Add(familyToAdd[child.Id]);
                            }
                        }
                    }
                }
                else
                {
                    //check mono-adult cases
                    if (personToRemove.ContainsKey(family.Adult1.Id))
                    {

                        if (family.Children.Count > 0)
                        {
                            missingFamilyPersonOnlyWithChildren.Add(family);
                        }
                        else
                        {
                            missingFamilyPersonOnly.Add(family);
                        }
                    }
                    else
                    {
                        remCaseToResolve.Add(family);
                    }
                } 
            }


            //A LITTLE REPORT IN MARKDOWN ;)
            TextWriter tw = new StreamWriter("s:\\EChUpdateAnalyse.md");

            tw.WriteLine("# Rapport Analyse ECH du " + DateTime.Now);
            tw.WriteLine("## Résumé des modifications détéctée");
            tw.WriteLine((newFamily.Count) + " familles sont arrivées dans le registre");
            tw.WriteLine("");
            tw.WriteLine(newFamilyPersonOnlyWithChildren.Count + " ménages mono-parentaux sont arrivées dans le registre");
            tw.WriteLine("");
            tw.WriteLine(missingFamilyPersonOnlyWithChildren.Count + " ménages mono-parentaux sont sortis du registre");
            tw.WriteLine("");
            tw.WriteLine((missingFamily.Count) + " familles sont sorties du registre");
            tw.WriteLine("");
            tw.WriteLine(newFamilyPersonOnly.Count + " personnes seules sont arrivées dans le registre");
            tw.WriteLine("");
            tw.WriteLine(gainMajority.Count + " cas de majorité");
            tw.WriteLine("");
            tw.WriteLine((missingFamilyPersonOnly.Count) + " personnes seules sont sorties registre");
            tw.WriteLine("");
            var structAddCount = (familyToAdd.Count - (newFamily.Count + newFamilyPersonOnly.Count + newFamilyPersonOnlyWithChildren.Count));
            tw.WriteLine( structAddCount  + " ménages a recréer suite a une modification de leurs structures");
            tw.WriteLine("");
            tw.WriteLine(newBirth.Count + " naissances dans ces restructurations");
            tw.WriteLine("");
            tw.WriteLine(childMissing.Count + " enfants sortis du registre lors de ces restructurations");
            tw.WriteLine("");
            var structRemoveCount = (familyToRemove.Count - (missingFamily.Count + missingFamilyPersonOnly.Count + missingFamilyPersonOnlyWithChildren.Count));
            tw.WriteLine(structRemoveCount  + " ménages a supprimer suite a une modification de leurs structures");
            tw.WriteLine("");
            tw.WriteLine(addCaseToResolve.Count + " ajouts non-résolus");
            tw.WriteLine("");
            tw.WriteLine(remCaseToResolve.Count + " suppressions non-résolues");
            tw.WriteLine("");
            tw.WriteLine("## Nouvelles familles dans le registre (" + newFamily.Count + ")");
            foreach (var family in newFamily)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) "+ family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("## Nouvelles familles mono-parentales dans le registre (" + newFamilyPersonOnlyWithChildren.Count + ")");
            foreach (var family in newFamilyPersonOnlyWithChildren)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("## Naissances (" + newBirth.Count + ")");
            foreach (var child in newBirth)
            {
                tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName  + " né le " + child.DateOfBirth);
            }

            tw.WriteLine("## Enfants sortis du registre (" + childMissing.Count + ")");
            foreach (var child in childMissing)
            {
                tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
            }

            tw.WriteLine("## Majorités (" + gainMajority.Count + ")");
            foreach (var family in gainMajority)
            {
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
            }

            tw.WriteLine("## Nouvelles personnes seules dans le registre (" + newFamilyPersonOnly.Count + ")");
            foreach (var family in newFamilyPersonOnly)
            {
                tw.WriteLine("* (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
            }

            tw.WriteLine("## Départ de familles dans le registre (" + missingFamily.Count + ")");
            foreach (var family in missingFamily)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("## Départ de familles mono-parentales dans le registre (" + missingFamilyPersonOnlyWithChildren.Count + ")");
            foreach (var family in missingFamilyPersonOnlyWithChildren)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("## Personnes seules sortantes du registre (" + missingFamilyPersonOnly.Count + ")");
            foreach (var family in missingFamilyPersonOnly)
            {
                tw.WriteLine("* (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
            }


            tw.WriteLine("# Erreurs concernant les ménages");
            tw.WriteLine("### Famille avec personnes manquantes dans le registe (" + errorFamily.Count + ")");
            foreach (var family in errorFamily)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }
            tw.Close();

        }


        private bool isNewFamilyArrival(EChReportedPerson family, Dictionary<string, EChPerson> personToAdd)
        {
            if (personToAdd.ContainsKey(family.Adult1.Id) && personToAdd.ContainsKey(family.Adult2.Id))
            {
                foreach (var child in family.Children)
                {
                    if (!personToAdd.ContainsKey(child.Id))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isFamilyDeparture(EChReportedPerson family, Dictionary<string, EChPerson> personToRemove)
        {
            if (personToRemove.ContainsKey(family.Adult1.Id) && personToRemove.ContainsKey(family.Adult2.Id))
            {
                foreach (var child in family.Children)
                {
                    if (!personToRemove.ContainsKey(child.Id))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

		private void LoadFilesToCompare()
		{
			this.OrigineEch = EChDataComparerLoader.Load (new FileInfo (this.EchFileA), int.MaxValue).ToList ();
			this.VersionEch = EChDataComparerLoader.Load (new FileInfo (this.EchFileB), int.MaxValue).ToList ();
		}

		private void CreateDictionaryFromEntity()
		{

			this.DicFamilyA = new Dictionary<string, EChReportedPerson> ();
			this.DicFamilyB = new Dictionary<string, EChReportedPerson> ();
			this.DicPersonA = new Dictionary<string, EChPerson> ();
			this.DicPersonB = new Dictionary<string, EChPerson> ();

			foreach (EChReportedPerson Fam in OrigineEch)
			{
				this.DicFamilyA.Add (Fam.FamilyKey, Fam);

				this.DicPersonA.Add (Fam.Adult1.Id, Fam.Adult1);
				if (Fam.Adult2!=null)
				{
					this.DicPersonA.Add (Fam.Adult2.Id, Fam.Adult2);
				}
				foreach (EChPerson per in Fam.Children)
				{
					if (!this.DicPersonA.ContainsKey (per.Id))
						this.DicPersonA.Add (per.Id, per);
				}
			}
			

			foreach (EChReportedPerson Fam in VersionEch)
			{
				this.DicFamilyB.Add (Fam.FamilyKey, Fam);
				this.DicPersonB.Add (Fam.Adult1.Id, Fam.Adult1);
				if (Fam.Adult2!=null)
				{
					this.DicPersonB.Add (Fam.Adult2.Id, Fam.Adult2);
				}
				foreach (EChPerson per in Fam.Children)
				{
					if (!this.DicPersonB.ContainsKey (per.Id))
						this.DicPersonB.Add (per.Id, per);
				}
			}
			
		}

		//Filespath
		private string EchFileA;
		private string EchFileB;

		//Entity List
		private List<EChReportedPerson> OrigineEch;
		private List<EChReportedPerson> VersionEch;


		//Comparison dictionary
		private Dictionary<string,EChReportedPerson> DicFamilyA;
		private Dictionary<string,EChReportedPerson> DicFamilyB;
		private Dictionary<string,EChPerson> DicPersonA;
		private Dictionary<string,EChPerson> DicPersonB;

	}
}
