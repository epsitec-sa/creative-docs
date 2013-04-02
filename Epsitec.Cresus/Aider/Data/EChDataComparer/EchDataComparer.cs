using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Aider.Data.EChDataComparer
{



	class EchDataComparer
	{
		//Filespath
		private string fileA;
		private string fileB;

		//Entity List
		private List<EChReportedPerson> origineEch;
		private List<EChReportedPerson> versionEch;


		//Comparison dictionary
		private Dictionary<string,EChReportedPerson> dicFamilyA;
		private Dictionary<string,EChReportedPerson> dicFamilyB;
		private Dictionary<string,EChPerson> dicPersonA;
		private Dictionary<string,EChPerson> dicPersonB;

		


		public EchDataComparer(string oldEchFile,string newEchFile)
		{
			this.fileA = oldEchFile;
			this.fileB = newEchFile;

			this.LoadFilesToCompare ();
			this.CreateDictionaryFromEntity ();
		}


		public List<EChReportedPerson> getFamilyToRemove()
		{
			var familyToBeRemoved = (from e in dicFamilyA
									 where !dicFamilyB.ContainsKey (e.Key)
									 select e.Value).ToList ();

			return familyToBeRemoved;
		}

		public List<EChReportedPerson> getFamilyToAdd()
		{
			var familyToBeAdded = (from n in dicFamilyB
								   where !dicFamilyA.ContainsKey (n.Key)
								   select n.Value).ToList ();

			return familyToBeAdded;
		}

		public List<EChReportedPerson> getFamilyToChange()
		{
			var familyToCheck = from e in dicFamilyB
								where dicFamilyA.ContainsKey (e.Key)
								select e;

			var familyToChange = (from c in familyToCheck
								  join e in dicFamilyA on c.Key equals e.Key
								  where !c.Value.CheckData (e.Value.Address.HouseNumber, e.Value.Address.CountryCode, e.Value.Address.AddressLine1, e.Value.Address.Street, e.Value.Address.SwissZipCode, e.Value.Address.SwissZipCodeAddOn, e.Value.Address.SwissZipCodeId, e.Value.Address.Town)
								  select c.Value).ToList ();

			return familyToChange;
		}

		public List<EChPerson> getPersonToRemove()
		{
			var personToBeRemoved = (from e in dicPersonA
									 where !dicPersonB.ContainsKey (e.Key)
									 select e.Value).ToList ();

			return personToBeRemoved;
		}

		public List<EChPerson> getPersonToAdd()
		{
			var personToBeAdded = (from n in dicPersonB
								   where !dicPersonA.ContainsKey (n.Key)
								   select n.Value).ToList ();

			return personToBeAdded;
		}

		public List<EChPerson> getPersonToChange()
		{
			var personToCheck =  from e in dicPersonB
								 where dicPersonA.ContainsKey (e.Key)
								 select e;

			var personToChange = (from c in personToCheck
								  join e in dicPersonA on c.Key equals e.Key
								  where !c.Value.CheckData (e.Value.OfficialName, e.Value.FirstNames, e.Value.DateOfBirth, e.Value.Sex, e.Value.NationalityStatus, e.Value.NationalCountryCode, e.Value.MaritalStatus, string.Join ("", e.Value.OriginPlaces.Select (k => k.Name + k.Canton)))
								  select c.Value).ToList ();

			return personToChange;
		}

		private void LoadFilesToCompare()
		{
			this.origineEch = EChDataLoader.Load (new FileInfo (this.fileA), int.MaxValue).ToList ();
			this.versionEch = EChDataLoader.Load (new FileInfo (this.fileB), int.MaxValue).ToList ();
		}

		private void CreateDictionaryFromEntity()
		{

			this.dicFamilyA = new Dictionary<string, EChReportedPerson> ();
			this.dicFamilyB = new Dictionary<string, EChReportedPerson> ();
			this.dicPersonA = new Dictionary<string, EChPerson> ();
			this.dicPersonB = new Dictionary<string, EChPerson> ();

			foreach (EChReportedPerson fam in origineEch)
			{
				this.dicFamilyA.Add (fam.familyKey, fam);

				this.dicPersonA.Add (fam.Adult1.Id, fam.Adult1);
				if (fam.Adult2!=null)
				{
					this.dicPersonA.Add (fam.Adult2.Id, fam.Adult2);
				}
				foreach (EChPerson per in fam.Children)
				{
					if (!this.dicPersonA.ContainsKey (per.Id))
						this.dicPersonA.Add (per.Id, per);
				}
			}
			

			foreach (EChReportedPerson fam in versionEch)
			{
				this.dicFamilyB.Add (fam.familyKey, fam);
				this.dicPersonB.Add (fam.Adult1.Id, fam.Adult1);
				if (fam.Adult2!=null)
				{
					this.dicPersonB.Add (fam.Adult2.Id, fam.Adult2);
				}
				foreach (EChPerson per in fam.Children)
				{
					if (!this.dicPersonB.ContainsKey (per.Id))
						this.dicPersonB.Add (per.Id, per);
				}
			}
			
		}


	}

 
}
