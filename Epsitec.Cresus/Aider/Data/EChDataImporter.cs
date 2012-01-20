using Epsitec.Aider.Entities;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Diagnostics;


namespace Epsitec.Aider.Data
{


	internal static class EChDataImporter
	{


		public static void Import(Func<BusinessContext> businessContextCreator, IEnumerable<EChReportedPerson> eChReportedPersons)
		{
			var maxBatchSize = 1000;
			var currentBatchSize = 0;

			var tmp = eChReportedPersons.ToList (); // TMP
			eChReportedPersons = tmp; // TMP
			
			BusinessContext businessContext = null;
			try
			{
				businessContext = EChDataImporter.InitializeBusinessContext (businessContextCreator);

				int nb = 0; // TMP

				var stopwatch = Stopwatch.StartNew (); // TMP

				foreach (var eChReportedPerson in eChReportedPersons)
				{
					EChDataImporter.Import (businessContext, eChReportedPerson);

					if (currentBatchSize == maxBatchSize)
					{
						EChDataImporter.SaveAndDisposeBusinessContext (businessContext);

						businessContext = EChDataImporter.InitializeBusinessContext (businessContextCreator);

						currentBatchSize = 0;
					}
					else
					{
						currentBatchSize += 1;
					}

					nb++; // TMP

					if (nb % 100 == 0) // TMP
					{
						System.Diagnostics.Debug.WriteLine ("[" + DateTime.Now + "] Imported: " + nb + "/" + tmp.Count); // TMP
					}
				}

				stopwatch.Stop (); // TMP

				if (nb % 100 != 0) // TMP
				{
					System.Diagnostics.Debug.WriteLine ("[" + DateTime.Now + "] Imported: " + nb + "/" + tmp.Count); // TMP
				}

				System.Diagnostics.Debug.WriteLine ("Total time: " + stopwatch.Elapsed); // TMP
			}
			finally
			{
				EChDataImporter.SaveAndDisposeBusinessContext (businessContext);
			}
		}


		private static BusinessContext InitializeBusinessContext(Func<BusinessContext> businessContextCreator)
		{
			return businessContextCreator ();
		}


		private static void SaveAndDisposeBusinessContext(BusinessContext businessContext)
		{
			if (businessContext != null)
			{
				businessContext.SaveChanges ();
				businessContext.Dispose ();
			}
		}


		private static eCH_ReportedPersonEntity Import(BusinessContext businessContext, EChReportedPerson eChReportedPerson)
		{
			var eChReportedPersonEntity = businessContext.CreateEntity<eCH_ReportedPersonEntity> ();
			var aiderHouseHold = businessContext.CreateEntity<AiderHouseholdEntity> ();

			eCH_AddressEntity eChAddressEntity = null;
			var eChAddress = eChReportedPerson.Address;

			if (eChAddress != null)
			{
				eChAddressEntity = EChDataImporter.Import (businessContext, eChAddress);

				eChReportedPersonEntity.Address = eChAddressEntity;
			}

			var eChAdult1 = eChReportedPerson.Adult1;

			if (eChAdult1 != null)
			{
				var result = EChDataImporter.Import (businessContext, eChAdult1, eChReportedPersonEntity, eChAddressEntity, aiderHouseHold);
				
				eChReportedPersonEntity.Adult1 = result.Item1;
				aiderHouseHold.Head1 = result.Item2;
			}

			var eChAdult2 = eChReportedPerson.Adult2;

			if (eChAdult2 != null)
			{
				var result = EChDataImporter.Import (businessContext, eChAdult2, eChReportedPersonEntity, eChAddressEntity, aiderHouseHold);

				eChReportedPersonEntity.Adult1 = result.Item1;
				aiderHouseHold.Head2 = result.Item2;
			}

			foreach (var eChChild in eChReportedPerson.Children)
			{
				var result = EChDataImporter.Import (businessContext, eChChild, eChReportedPersonEntity, eChAddressEntity, aiderHouseHold);

				eChReportedPersonEntity.Children.Add (result.Item1);
			}

			return eChReportedPersonEntity;
		}


		private static Tuple<eCH_PersonEntity, AiderPersonEntity> Import(BusinessContext businessContext, EChPerson eChPerson, eCH_ReportedPersonEntity eChReportedPerson, eCH_AddressEntity eChAddressEntity, AiderHouseholdEntity houseHold)
		{
			var aiderPersonEntity = businessContext.CreateEntity<AiderPersonEntity> ();
			aiderPersonEntity.Household = houseHold;			
			
			var eChPersonEntity = aiderPersonEntity.eCH_Person;

			eChPersonEntity.PersonId = eChPerson.Id;
			eChPersonEntity.PersonOfficialName = eChPerson.OfficialName;
			eChPersonEntity.PersonFirstNames = eChPerson.FirstNames;
			eChPersonEntity.PersonDateOfBirthType = eChPerson.DateOfBirthPrecision;
			eChPersonEntity.PersonDateOfBirthYear = eChPerson.DateOfBirthYear;
			eChPersonEntity.PersonDateOfBirthMonth = eChPerson.DateOfBirthMonth;
			eChPersonEntity.PersonDateOfBirthDay = eChPerson.DateOfBirthDay;
			eChPersonEntity.PersonSex = eChPerson.Sex;
			eChPersonEntity.NationalityStatus = eChPerson.NationalityStatus;
			eChPersonEntity.NationalityCountryCode = eChPerson.NationalCountryCode;
			eChPersonEntity.Origins = eChPerson.OriginPlaces
				.Select (p => p.Name + " (" + p.Canton + ")")
				.Join ("\n");
			eChPersonEntity.AdultMaritalStatus = eChPerson.MaritalStatus;

			eChPersonEntity.CreationDate = Date.Today;
			eChPersonEntity.DataSource = eCH.DataSource.Government;
			eChPersonEntity.DeclarationStatus = eCH.PersonDeclarationStatus.Declared;
			eChPersonEntity.RemovalReason = eCH.RemovalReason.None;

			eChPersonEntity.ReportedPerson1 = eChReportedPerson;
			eChPersonEntity.Address = eChAddressEntity;

			// What is echPersonEntity.ReportedPerson2 ?
			// Should we make some kind of join ?
			// There are some duplicates in the xml file, where a person is in two family.

			return Tuple.Create (eChPersonEntity, aiderPersonEntity);
		}


		private static eCH_AddressEntity Import(BusinessContext businessContext, EChAddress eChAddress)
		{
			var eChAddressEntity = businessContext.CreateEntity<eCH_AddressEntity> ();

			eChAddressEntity.AddressLine1 = eChAddress.AddressLine1;
			eChAddressEntity.Street = eChAddress.Street;
			eChAddressEntity.HouseNumber = eChAddress.HouseNumber;
			eChAddressEntity.Town = eChAddress.Town;
			eChAddressEntity.SwissZipCode = eChAddressEntity.SwissZipCode;
			eChAddressEntity.SwissZipCodeAddOn = eChAddress.SwissZipCodeAddOn;
			eChAddressEntity.SwissZipCodeId = eChAddress.SwissZipCodeId;
			eChAddressEntity.Country = eChAddress.CountryCode;

			return eChAddressEntity;
		}


	}


}
