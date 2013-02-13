using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Data.Eerv;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;



namespace Epsitec.Aider.Data.ECh
{


	internal static class EChDataImporter
	{


		public static void Import(CoreData coreData, ParishAddressRepository parishRepository, IList<EChReportedPerson> eChReportedPersons)
		{
			EChDataImporter.ImportCountries (coreData);
			var zipCodeIdToEntityKey = EChDataImporter.ImportTowns (coreData, eChReportedPersons);

			try
			{
				coreData.EnableIndexes (false);

				EChDataImporter.ImportPersons (coreData, parishRepository, eChReportedPersons, zipCodeIdToEntityKey);
			}
			finally
			{
				coreData.EnableIndexes (true);
				coreData.ResetIndexes ();
			}
		}


		private static void ImportCountries(CoreData coreData)
		{
			using (var businessContext = new BusinessContext(coreData, false))
			{
				EChDataImporter.ImportCountries (businessContext);
			}
		}


		private static void ImportCountries(BusinessContext businessContext)
		{
			var countries = Iso3166.GetCountries ("FR");

			foreach (var country in countries)
			{
				var isoCode = country.IsoAlpha2;
				var name = country.Name;

				AiderCountryEntity.FindOrCreate (businessContext, isoCode, name);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static Dictionary<int, EntityKey> ImportTowns(CoreData coreData, IEnumerable<EChReportedPerson> echReportedPersons)
		{
			using (var businessContext = new BusinessContext(coreData, false))
			{
				return EChDataImporter.ImportTowns (businessContext, echReportedPersons);
			}
		}


		private static Dictionary<int, EntityKey> ImportTowns(BusinessContext businessContext, IEnumerable<EChReportedPerson> echReportedPersons)
		{
			var repository = EChDataImporter.ImportTowns (businessContext);

			return echReportedPersons
				.Select (rp => rp.Address.SwissZipCodeId)
				.Distinct ()
				.Select (id => repository.GetTown (id))
				.ToDictionary
				(
					t => t.SwissZipCodeId.Value,
					t => businessContext.DataContext.GetNormalizedEntityKey (t).Value
				);
		}


		private static AiderTownRepository ImportTowns(BusinessContext businessContext)
		{
			var repository = new AiderTownRepository (businessContext);
			repository.AddMissingSwissTowns ();
			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			return repository;
		}


		private static void ImportPersons(CoreData coreData, ParishAddressRepository parishRepository, IList<EChReportedPerson> eChReportedPersons, Dictionary<int, EntityKey> zipCodeIdToEntityKey)
		{
			int batchSize = 1000;
			int nbBatches = 0;

			// NOTE This dictionary will store the mapping between the eChPersonIds and the entity
			// key for the entities that have been processed and saved to the database.

			var eChPersonIdToEntityKey = new Dictionary<string, EntityKey> ();

			foreach (var batch in EChDataImporter.GetBatches (eChReportedPersons, batchSize))
			{
				using (var businessContext = new BusinessContext (coreData, false))
				{
					EChDataImporter.ImportBatch (businessContext, parishRepository, batch, eChPersonIdToEntityKey, zipCodeIdToEntityKey);
				}

				nbBatches++;

				Debug.WriteLine (string.Format ("[{0}]\tImported: {1}/{2}", DateTime.Now, nbBatches * batchSize, eChReportedPersons.Count));
			}
		}


		private static void ImportBatch(BusinessContext businessContext, ParishAddressRepository parishRepository, IEnumerable<EChReportedPerson> batch, Dictionary<string, EntityKey> eChPersonIdToEntityKey, Dictionary<int, EntityKey> zipCodeIdToEntityKey)
		{
			// NOTE This dictionary will store the mapping between the eChpersonIds and the
			// entities for the entities that have been processed but not yet saved to the
			// database.

			var eChPersonIdToEntity = new Dictionary<string, AiderPersonEntity> ();

			foreach (var eChReportedPerson in batch)
			{
				EChDataImporter.ImportHousehold (businessContext, eChPersonIdToEntityKey, eChPersonIdToEntity, eChReportedPerson, zipCodeIdToEntityKey);
			}

			ParishAssigner.AssignToParish (parishRepository, businessContext, eChPersonIdToEntity.Values);

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			// NOTE Now that the changes are saved, the newly created entities have an
			// entity key which we can store in the dictionary.

			foreach (var item in eChPersonIdToEntity)
			{
				var entityKey = businessContext.DataContext.GetNormalizedEntityKey (item.Value).Value;

				eChPersonIdToEntityKey[item.Key] = entityKey;
			}
		}


		private static IEnumerable<IEnumerable<EChReportedPerson>> GetBatches(IList<EChReportedPerson> eChReportedPersons, int batchSize)
		{
			for (int index = 0; index < eChReportedPersons.Count; index += batchSize)
			{
				yield return EChDataImporter.GetBatch (eChReportedPersons, index, batchSize);
			}
		}


		private static IEnumerable<EChReportedPerson> GetBatch(IList<EChReportedPerson> eChReportedPerson, int startIndex, int size)
		{
			int upperBound = Math.Min (startIndex + size, eChReportedPerson.Count);

			for (int i = startIndex; i < upperBound; i++)
			{
				yield return eChReportedPerson[i];
			}
		}


		private static eCH_ReportedPersonEntity ImportHousehold(BusinessContext businessContext, Dictionary<string, EntityKey> eChPersonIdToEntityKey, Dictionary<string, AiderPersonEntity> eChPersonIdToEntity, EChReportedPerson eChReportedPerson, Dictionary<int, EntityKey> zipCodeIdToEntityKey)
		{
			var eChReportedPersonEntity = businessContext.CreateAndRegisterEntity<eCH_ReportedPersonEntity> ();
			var eChAddress = eChReportedPerson.Address;
			var eChAddressEntity = EChDataImporter.ImportEchAddressEntity (businessContext, eChAddress);
			eChReportedPersonEntity.Address = eChAddressEntity;

			var aiderHousehold = businessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
			aiderHousehold.HouseholdMrMrs = HouseholdMrMrs.Auto;
			EChDataImporter.ImportAiderAddressEntity (businessContext, aiderHousehold.Address, eChAddress, zipCodeIdToEntityKey);

			var eChAdult1 = eChReportedPerson.Adult1;

			if (eChAdult1 != null)
			{
				var aiderPerson = EChDataImporter.ImportPerson (businessContext, eChPersonIdToEntityKey, eChPersonIdToEntity, eChAdult1);

				EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isHead1: true);
			}

			var eChAdult2 = eChReportedPerson.Adult2;

			if (eChAdult2 != null)
			{
				var aiderPerson = EChDataImporter.ImportPerson (businessContext, eChPersonIdToEntityKey, eChPersonIdToEntity, eChAdult2);

				EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isHead2: true);
			}

			foreach (var eChChild in eChReportedPerson.Children)
			{
				var aiderPerson = EChDataImporter.ImportPerson (businessContext, eChPersonIdToEntityKey, eChPersonIdToEntity, eChChild);

				EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isChild: true);
			}

			return eChReportedPersonEntity;
		}


		private static AiderPersonEntity ImportPerson(BusinessContext businessContext, Dictionary<string, EntityKey> eChPersonIdToEntityKey, Dictionary<string, AiderPersonEntity> eChPersonIdToEntity, EChPerson eChPerson)
		{
			EntityKey entityKey;
			AiderPersonEntity aiderPersonEntity;

			// NOTE Before we create the entities for an EChPerson, we check if it has already been
			// created. We check that by looking at the entities that have already been created but
			// not yet saved and at the entities that have already been saved.

			if (eChPersonIdToEntity.TryGetValue (eChPerson.Id, out aiderPersonEntity))
			{
				// NOTE Nothing to do here. We simply want the side effect of the boolean
				// expression.
			}
			else if (eChPersonIdToEntityKey.TryGetValue (eChPerson.Id, out entityKey))
			{
				aiderPersonEntity = businessContext.ResolveEntity<AiderPersonEntity> (entityKey);
				businessContext.Register (aiderPersonEntity);
			}
			else
			{
				aiderPersonEntity = EChDataImporter.ImportPerson (businessContext, eChPerson);

				// NOTE We add the newly created entity to the dictionary of the entities that have
				// been created but not yet saved.

				eChPersonIdToEntity[eChPerson.Id] = aiderPersonEntity;
			}

			return aiderPersonEntity;
		}


		private static AiderPersonEntity ImportPerson(BusinessContext businessContext, EChPerson eChPerson)
		{
			var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
			
			var eChPersonEntity = aiderPersonEntity.eCH_Person;

			eChPersonEntity.PersonId = eChPerson.Id;
			eChPersonEntity.PersonOfficialName = eChPerson.OfficialName;
			eChPersonEntity.PersonFirstNames = eChPerson.FirstNames;
			eChPersonEntity.PersonDateOfBirth = eChPerson.DateOfBirth;
			eChPersonEntity.PersonSex = eChPerson.Sex;
			eChPersonEntity.NationalityStatus = eChPerson.NationalityStatus;
			eChPersonEntity.NationalityCountryCode = eChPerson.NationalCountryCode;
			if (eChPerson.NationalityStatus == PersonNationalityStatus.Defined)
			{
				eChPersonEntity.Nationality = AiderCountryEntity.Find (businessContext, eChPerson.NationalCountryCode);
			}
			eChPersonEntity.Origins = eChPerson.OriginPlaces
				.Select (p => p.Name + " (" + p.Canton + ")")
				.Join ("\n");
			eChPersonEntity.AdultMaritalStatus = eChPerson.MaritalStatus;

			eChPersonEntity.CreationDate = Date.Today;
			eChPersonEntity.DataSource = Enumerations.DataSource.Government;
			eChPersonEntity.DeclarationStatus = PersonDeclarationStatus.Declared;
			eChPersonEntity.RemovalReason = RemovalReason.None;

			aiderPersonEntity.MrMrs = EChDataImporter.GuessMrMrs (eChPerson.Sex, eChPerson.DateOfBirth, eChPerson.MaritalStatus);
			aiderPersonEntity.Confession = PersonConfession.Protestant;

			return aiderPersonEntity;
		}


		private static void ImportAiderAddressEntity(BusinessContext businessContext, AiderAddressEntity aiderAddressEntity, EChAddress eChAddress, Dictionary<int, EntityKey> zipCodeIdToEntityKey)
		{
			var houseNumber = StringUtils.ParseNullableInt (SwissPostStreet.StripHouseNumber (eChAddress.HouseNumber));
			var houseNumberComplement = SwissPostStreet.GetHouseNumberComplement (eChAddress.HouseNumber);

			if (string.IsNullOrWhiteSpace (houseNumberComplement))
			{
				houseNumberComplement = null;
			}

			var townEntityKey = zipCodeIdToEntityKey[eChAddress.SwissZipCodeId];
			var town = businessContext.ResolveEntity<AiderTownEntity> (townEntityKey);

			aiderAddressEntity.AddressLine1 = eChAddress.AddressLine1;
			aiderAddressEntity.Street = eChAddress.Street;
			aiderAddressEntity.HouseNumber = houseNumber;
			aiderAddressEntity.HouseNumberComplement = houseNumberComplement;
			aiderAddressEntity.Town = town;
		}


		private static eCH_AddressEntity ImportEchAddressEntity(BusinessContext businessContext, EChAddress eChAddress)
		{
			var eChAddressEntity = businessContext.CreateAndRegisterEntity<eCH_AddressEntity> ();

			eChAddressEntity.AddressLine1 = eChAddress.AddressLine1;
			eChAddressEntity.Street = eChAddress.Street;
			eChAddressEntity.HouseNumber = eChAddress.HouseNumber;
			eChAddressEntity.Town = eChAddress.Town;
			eChAddressEntity.SwissZipCode = eChAddress.SwissZipCode;
			eChAddressEntity.SwissZipCodeAddOn = eChAddress.SwissZipCodeAddOn;
			eChAddressEntity.SwissZipCodeId = eChAddress.SwissZipCodeId;
			eChAddressEntity.Country = eChAddress.CountryCode;

			return eChAddressEntity;
		}


		private static void SetupHousehold(BusinessContext businessContext, AiderPersonEntity aiderPerson, AiderHouseholdEntity aiderHousehold, eCH_ReportedPersonEntity eChReportedPerson, bool isHead1 = false, bool isHead2 = false, bool isChild = false)
		{
			var isHead = isHead1 || isHead2;

			AiderContactEntity.Create (businessContext, aiderPerson, aiderHousehold, isHead);

			var eChPerson = aiderPerson.eCH_Person;

			if (eChPerson.ReportedPerson1.IsNull ())
			{
				eChPerson.ReportedPerson1 = eChReportedPerson;
				eChPerson.Address1 = eChReportedPerson.Address;
			}
			else
			{
				eChPerson.ReportedPerson2 = eChReportedPerson;
				eChPerson.Address2 = eChReportedPerson.Address;
			}

			if (isHead1)
			{
				eChReportedPerson.Adult1 = eChPerson;
			}
			else if (isHead2)
			{
				eChReportedPerson.Adult2 = eChPerson;
			}
			else
			{
				eChReportedPerson.Children.Add (eChPerson);
			}
		}


		private static PersonMrMrs GuessMrMrs(PersonSex personSex, Date dateOfBirth, PersonMaritalStatus maritalStatus)
		{
			int? age = dateOfBirth.ComputeAge ();

			switch (personSex)
			{
				case PersonSex.Female:
					if ((age.HasValue) &&
						(age.Value < 20) &&
						(maritalStatus == PersonMaritalStatus.None || maritalStatus == PersonMaritalStatus.Unmarried || maritalStatus == PersonMaritalStatus.Single))
					{
						return PersonMrMrs.Mademoiselle;
					}
					else
					{
						return PersonMrMrs.Madame;
					}

				case PersonSex.Male:
					return PersonMrMrs.Monsieur;
				
				case PersonSex.Unknown:
					return PersonMrMrs.None;
			}

			throw new System.NotSupportedException ();
		}



	}


}
