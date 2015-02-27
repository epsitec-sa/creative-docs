using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Entities;

using Epsitec.Common.IO;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;


namespace Epsitec.Aider.Data.Job
{


	/// <summary>
	/// This fixer is used to fix a problem with some addresses in the database. Because of a bug
	/// in the algorithm that finds the street data in the MAT[CH] street light file, some
	/// addresses where registered in the wrong town. This job fixes these addresses for those that
	/// come from the ECH file. See commits 20964 and 20974 for more information.
	/// </summary>
	internal static class AmbiguousAddressFixer
	{


		public static void FixAmbiguousAddresses
		(
			SwissPostStreetRepository streetRepository,
			IEnumerable<EChReportedPerson> echReportedPersons,
			CoreData coreData
		)
		{
			var ambiguousStreets = AmbiguousAddressFixer.GetAmbiguousStreets (streetRepository);

			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var echReportedPerson in echReportedPersons)
				{
					var isAmbiguous = AmbiguousAddressFixer.IsAmbiguous
					(
						streetRepository, ambiguousStreets, echReportedPerson.Address
					);

					if (isAmbiguous)
					{
						AmbiguousAddressFixer.FixAmbiguousAddress
						(
							echReportedPersons, streetRepository, ambiguousStreets,
							businessContext, echReportedPerson
						);
					}
				}

				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);
			}
		}


		private static bool IsAmbiguous
		(
			SwissPostStreetRepository streetRepository,
			ISet<SwissPostStreetInformation> ambiguousStreets,
			EChAddress address
		)
		{
			var zipCode = address.SwissZipCode;
			var zipAddOn = address.SwissZipCodeAddOn;
			var streetName = address.Street;

			var street = streetRepository.FindStreetFromStreetName (zipCode, zipAddOn, streetName);

			return ambiguousStreets.Contains (street);
		}


		private static void FixAmbiguousAddress
		(
			IEnumerable<EChReportedPerson> echReportedPersons,
			SwissPostStreetRepository streetRepository,
			ISet<SwissPostStreetInformation> ambiguousStreets,
			BusinessContext businessContext,
			EChReportedPerson echReportedPerson
		)
		{
			var address = echReportedPerson.Address;

			var persons = AmbiguousAddressFixer.GetAiderPersons
			(
				businessContext, echReportedPerson
			);

			// Here we find the eCH_reportedPersons to correct. Normally, there is only one, but it
			// might happen that there are two, if a member is in two households that are both in
			// the same ambiguous streets.

			var eCH_reportedPersons = persons
				.SelectMany (p => p.eCH_Person.ReportedPersons)
				.Distinct ()
				.Where (rp => rp.Address.SwissZipCode == address.SwissZipCode)
				.Where (rp => rp.Address.Street == address.Street)
				.ToList ();

			// If we can't find an eCH_Address with the proper data, there is a problem and we
			// throw an exception. This should never happen.

			if (eCH_reportedPersons.Count == 0)
			{
				throw new Exception ();
			}

			var message1 = "==================================================================\n"
				+ "[" + DateTime.Now + "] FOUND AMBIGUOUS ADDRESS\n"
				+ "REAL ADDRESS:".PadRight (20)  + address.Street + ", " + address.SwissZipCode
				+ ", " + address.SwissZipCodeAddOn + ", " + address.SwissZipCodeId + ", "
				+ address.Town;

			Logger.LogToConsole (message1);

			// Here we check that if we have two households, they are really at the same address
			// and not at different addresses. If that where the case, we would probably not be
			// able to make corrections to the AiderHouseholds because we have no way to find which
			/// eCH_reportedPerson maps to which AiderHousehold. This case should never happen.

			if (eCH_reportedPersons.Count > 1)
			{
				var echPersonIds = echReportedPerson
					.GetMembers ()
					.Select (p => p.Id)
					.ToList ();

				var echAddresses = echReportedPersons
					.Where (rp => rp.GetMembers ().Select (p => p.Id).Intersect (echPersonIds).Any ())
					.Where (rp => rp.Address.SwissZipCode == address.SwissZipCode)
					.Where (rp => rp.Address.Street == address.Street)
					.Select (rp => rp.Address)
					.ToList ();

				var distinctAddresses = echAddresses
					.Select (a => a.SwissZipCodeAddOn)
					.Distinct ()
					.ToList ();

				if (distinctAddresses.Count > 1)
				{
					throw new Exception ();
				}
			}

			var correction = false;

			// Here we correct the eCH_addresses if they differ from the real address.

			foreach (var eCH_reportedPerson in eCH_reportedPersons)
			{
				var eCH_address = eCH_reportedPerson.Address;

				var message2 = "DB ECH_ADDRESS:".PadRight (20) + eCH_address.Street + ", "
					+ eCH_address.SwissZipCode + ", " + eCH_address.SwissZipCodeAddOn + ", "
					+ eCH_address.SwissZipCodeId + ", " + eCH_address.Town;

				Logger.LogToConsole (message2);

				if (eCH_address.SwissZipCodeAddOn != address.SwissZipCodeAddOn)
				{
					Logger.LogToConsole ("CORRECTING ECH_ADDRESS");

					eCH_address.SwissZipCodeAddOn = address.SwissZipCodeAddOn;
					eCH_address.SwissZipCodeId = address.SwissZipCodeId;
					eCH_address.Town = address.Town;

					correction = true;
				}
			}

			// If the eCH_addresses where wrong, so are the AiderAddresses and we correct them
			// here.

			if (correction)
			{
				var aiderHouseholds = persons
					.SelectMany (p => p.Households)
					.Distinct ()
					.Where (h => h.Address.Town.SwissZipCode == address.SwissZipCode)
					.Where (h => h.Address.Street == address.Street)
					.ToList ();

				// If we don't find corresponding AiderAddresses, that means that the user has
				// changed it, and we don't want to make a correction on it.

				if (aiderHouseholds.Count == 0)
				{
					return;
				}

				// We register the households to that their business rules as well as the business
				// rules of their associated entities are executed.

				businessContext.Register (aiderHouseholds);

				var townExample = new AiderTownEntity ()
				{
					SwissZipCode = address.SwissZipCode,
					SwissZipCodeId = address.SwissZipCodeId,
				};

				var town = businessContext.DataContext.GetByExample (townExample).Single ();

				foreach (var aiderHousehold in aiderHouseholds)
				{
					var aiderAddress = aiderHousehold.Address;
					var aiderTown = aiderAddress.Town;

					var message3 = "DB AIDER_ADDRESS:".PadRight (20) + aiderAddress.Street + ", "
						+ aiderTown.SwissZipCode + ", " + (SwissPostFullZip.GetZipCodeAddOn (aiderTown.SwissZipCodeAddOn) ?? "<null>") + ", "
						+ aiderTown.SwissZipCodeId + ", " + aiderTown.Name + "\n"
						+ "CORRECTING AIDER_ADDRESS";

					Logger.LogToConsole (message3);

					aiderAddress.Town = town;
				}
			}
		}


		private static IList<AiderPersonEntity> GetAiderPersons
		(
			BusinessContext businessContext,
			EChReportedPerson echReportedPerson
		)
		{
			var personIds = echReportedPerson
				.GetMembers ()
				.Select (m => m.Id)
				.ToList ();

			var personExample = new AiderPersonEntity ()
			{
				eCH_Person = new eCH_PersonEntity ()
			};

			var request = new Request ()
			{
				RootEntity = personExample
			};

			request.AddCondition
			(
				businessContext.DataContext,
				personExample.eCH_Person,
				p => SqlMethods.IsInSet (p.PersonId, personIds)
			);

			return businessContext.DataContext.GetByRequest<AiderPersonEntity> (request);
		}


		private static ISet<SwissPostStreetInformation> GetAmbiguousStreets
		(
			SwissPostStreetRepository streetRepository
		)
		{
			var streetComparer = new LambdaComparer<SwissPostStreetInformation>
			(
				(s1, s2) => s1.StreetName == s2.StreetName
					&& s1.ZipCode == s2.ZipCode
					&& s1.ZipCodeAddOn == s2.ZipCodeAddOn,
				(s) => s.StreetName.GetHashCode ()
			);

			return streetRepository
				.Streets
				.Distinct (streetComparer)
				.GroupBy (s => Tuple.Create (s.StreetName, s.ZipCode))
				.Select (g => g.ToList ())
				.Where (g => g.Count > 1)
				.SelectMany (g => g)
				.ToSet ();
		}


	}


}
