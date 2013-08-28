//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel Loup, Maintainer: Samuel Loup

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{
	/// <summary>
	/// Compare Ech XML data files, provide extraction methods for diff based on buisiness rules
	/// </summary>
	internal class EChDataComparer : System.IDisposable
	{
		/// <summary>
		/// Initialize a new EChDataComparer with two files
		/// </summary>
		/// <param name="oldEchFile">old ECh XML data file</param>
		/// <param name="newEchFile">new ECh XML data file</param>
		public EChDataComparer(string oldEchFile, string newEchFile)
		{
			var origineEch = EChDataLoader.Load (new FileInfo (oldEchFile), int.MaxValue).ToList ();
			var versionEch = EChDataLoader.Load (new FileInfo (newEchFile), int.MaxValue).ToList ();

			this.CreateMapsFromEChDataSources (origineEch, versionEch);
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		
		/// <summary>
		/// Create a list of ECh ReportedPerson to remove from database
		/// </summary>
		/// <returns>a list of ReportedPerson to remove</returns>
		public IEnumerable<EChReportedPerson> GetFamiliesToRemove()
		{
			var FamilyToBeRemoved = from e in this.oldFamilyMap
									where !this.newFamilyMap.ContainsKey (e.Key)
									select e.Value;

			return FamilyToBeRemoved;
		}

		/// <summary>
		/// Create a list of ECh ReportedPerson to add in the database
		/// </summary>
		/// <returns>a list of ReportedPerson to add</returns>
		public IEnumerable<EChReportedPerson> GetFamiliesToAdd()
		{
			var FamilyToBeAdded = from n in this.newFamilyMap
								  where !this.oldFamilyMap.ContainsKey (n.Key)
								  select n.Value;

			return FamilyToBeAdded;
		}

		/// <summary>
		/// Create a list of ECh ReportedPerson to modify
		/// </summary>
		/// <returns>a list of modified ECh ReportedPerson</returns>
		public IEnumerable<Change<EChReportedPerson>> GetFamiliesToChange()
		{
			var FamilyToCheck = from e in this.newFamilyMap
								where this.oldFamilyMap.ContainsKey (e.Key)
								select e;

			var FamilyToChange = from c in FamilyToCheck
								 join e in this.oldFamilyMap on c.Key equals e.Key
								 where !c.Value.CheckData(e.Value.Address.HouseNumber, e.Value.Address.CountryCode, e.Value.Address.AddressLine1, e.Value.Address.Street, e.Value.Address.SwissZipCode, e.Value.Address.SwissZipCodeAddOn, e.Value.Address.SwissZipCodeId, e.Value.Address.Town)
								 select Change.Create (c.Value, e.Value);

			return FamilyToChange;
		}

		/// <summary>
		/// Create a list of EChPerson to remove from database
		/// </summary>
		/// <returns>a list of EChPerson to remove</returns>
		public IEnumerable<EChPerson> GetPersonsToRemove()
		{
			var PersonToBeRemoved = from e in this.oldPersonMap
									where !this.newPersonMap.ContainsKey(e.Key)
									select e.Value;

			return PersonToBeRemoved;
		}

		/// <summary>
		/// Create a list of EChPerson to add in the database
		/// </summary>
		/// <returns>a list of EChPerson to add</returns>
		public IEnumerable<EChPerson> GetPersonsToAdd()
		{
			var PersonToBeAdded = from n in this.newPersonMap
								  where !this.oldPersonMap.ContainsKey(n.Key)
								  select n.Value;

			return PersonToBeAdded;
		}

		/// <summary>
		/// Create a list of tuple containing new EChPerson and old EChPerson
		/// </summary>
		/// <returns>a list of modified EChPerson</returns>
		public IEnumerable<Change<EChPerson>> GetPersonsToChange()
		{
			var PersonToCheck = from e in this.newPersonMap
								where this.oldPersonMap.ContainsKey(e.Key)
								select e;

			var PersonToChange = from c in PersonToCheck
								 join e in this.oldPersonMap on c.Key equals e.Key
								 where !c.Value.CheckData (e.Value.OfficialName, e.Value.FirstNames, e.Value.DateOfBirth, e.Value.Sex, e.Value.NationalityStatus, e.Value.NationalCountryCode, e.Value.MaritalStatus, e.Value.OriginPlaces)
								 select Change.Create (c.Value, e.Value);

			return PersonToChange;
		}


		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.oldFamilyMap = null;
				this.newFamilyMap = null;
				this.oldPersonMap = null;
				this.newPersonMap = null;
			}
		}
		
		private void CreateMapsFromEChDataSources(IEnumerable<EChReportedPerson> oldSource, IEnumerable<EChReportedPerson> newSource)
		{
			this.oldFamilyMap = new Dictionary<string, EChReportedPerson> ();
			this.newFamilyMap = new Dictionary<string, EChReportedPerson> ();
			this.oldPersonMap = new Dictionary<string, EChPerson> ();
			this.newPersonMap = new Dictionary<string, EChPerson> ();

			foreach (var family in oldSource)
			{
				this.oldFamilyMap.Add (family.FamilyKey, family);

				this.oldPersonMap.AddIfNotNull (family.Adult1);
				this.oldPersonMap.AddIfNotNull (family.Adult2);
				
				foreach (var child in family.Children)
				{
					this.oldPersonMap[child.Id] = child;
				}
			}


			foreach (var family in newSource)
			{
				this.newFamilyMap.Add (family.FamilyKey, family);

				this.newPersonMap.AddIfNotNull (family.Adult1);
				this.newPersonMap.AddIfNotNull (family.Adult2);

				foreach (var child in family.Children)
				{
					this.newPersonMap[child.Id] = child;
				}
			}
		}

		private Dictionary<string, EChReportedPerson>	oldFamilyMap;
		private Dictionary<string, EChReportedPerson>	newFamilyMap;
		private Dictionary<string, EChPerson>			oldPersonMap;
		private Dictionary<string, EChPerson>			newPersonMap;
	}
}