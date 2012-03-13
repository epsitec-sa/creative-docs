using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;


namespace Epsitec.Aider.Data.ECh
{


	internal static class EChDataImporter
	{


		public static void Import(Func<BusinessContext> businessContextCreator, Action<BusinessContext> businessContextCleaner, IList<EChReportedPerson> eChReportedPersons)
		{
			int batchSize = 1000;
			int nbBatches = 0;

			// NOTE This dictionary will store the mapping between the eChPersonIds and the entity
			// key for the entities that have been processed and saved to the database.

			var eChPersonIdToEntityKey = new Dictionary<string, EntityKey> ();

			foreach (var batch in EChDataImporter.GetBatches (eChReportedPersons, batchSize))
			{
				BusinessContext businessContext = null;

				try
				{
					businessContext = businessContextCreator ();

					// NOTE This dictionary will store the mapping between the eChpersonIds and the
					// entities for the entities that have been processed but not yet saved to the
					// database.

					var eChPersonIdToEntity = new Dictionary<string, AiderPersonEntity> ();

					foreach (var eChReportedPerson in batch)
					{
						EChDataImporter.Import (businessContext, eChPersonIdToEntityKey, eChPersonIdToEntity, eChReportedPerson);
					}

					businessContext.SaveChanges ();
					businessContextCleaner (businessContext);

					// NOTE Now that the changes are saved, the newly created entities have an
					// entity key which we can store in the dictionary.

					foreach (var item in eChPersonIdToEntity)
					{
						var entityKey = businessContext.DataContext.GetNormalizedEntityKey (item.Value).Value;

						eChPersonIdToEntityKey[item.Key] = entityKey;
					}
				}
				finally
				{
					if (businessContext != null)
					{
						businessContext.Dispose ();
					}
				}

				nbBatches++;

				Debug.WriteLine (string.Format ("[{0}]\tImported: {1}/{2}", DateTime.Now, nbBatches * batchSize, eChReportedPersons.Count));
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


		private static eCH_ReportedPersonEntity Import(BusinessContext businessContext, Dictionary<string, EntityKey> eChPersonIdToEntityKey, Dictionary<string, AiderPersonEntity> eChPersonIdToEntity, EChReportedPerson eChReportedPerson)
		{
			var eChReportedPersonEntity = EChDataImporter.CreateAndRegisterEntity<eCH_ReportedPersonEntity> (businessContext);
			var aiderHousehold = EChDataImporter.CreateAndRegisterEntity<AiderHouseholdEntity> (businessContext);

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
				var result = EChDataImporter.Import (businessContext, eChPersonIdToEntityKey, eChPersonIdToEntity, eChAdult1, eChReportedPersonEntity, eChAddressEntity, aiderHousehold);

				eChReportedPersonEntity.Adult1 = result.Item1;
				aiderHousehold.Head1 = result.Item2;
			}

			var eChAdult2 = eChReportedPerson.Adult2;

			if (eChAdult2 != null)
			{
				var result = EChDataImporter.Import (businessContext, eChPersonIdToEntityKey, eChPersonIdToEntity, eChAdult2, eChReportedPersonEntity, eChAddressEntity, aiderHousehold);

				eChReportedPersonEntity.Adult1 = result.Item1;
				aiderHousehold.Head2 = result.Item2;
			}

			foreach (var eChChild in eChReportedPerson.Children)
			{
				var result = EChDataImporter.Import (businessContext, eChPersonIdToEntityKey, eChPersonIdToEntity, eChChild, eChReportedPersonEntity, eChAddressEntity, aiderHousehold);

				eChReportedPersonEntity.Children.Add (result.Item1);
			}

			return eChReportedPersonEntity;
		}


		private static Tuple<eCH_PersonEntity, AiderPersonEntity> Import(BusinessContext businessContext, Dictionary<string, EntityKey> eChPersonIdToEntityKey, Dictionary<string, AiderPersonEntity> eChPersonIdToEntity, EChPerson eChPerson, eCH_ReportedPersonEntity eChReportedPersonEntity, eCH_AddressEntity eChAddressEntity, AiderHouseholdEntity household)
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
				aiderPersonEntity = (AiderPersonEntity) businessContext.DataContext.ResolveEntity (entityKey);
			}

			if (aiderPersonEntity != null)
			{
				aiderPersonEntity.Household2 = household;

				var eChPersonEntity = aiderPersonEntity.eCH_Person;

				eChPersonEntity.ReportedPerson2 = eChReportedPersonEntity;
				eChPersonEntity.Address2 = eChAddressEntity;

				return Tuple.Create (eChPersonEntity, aiderPersonEntity);
			}
			else
			{
				var result = EChDataImporter.Import (businessContext, eChPerson, eChReportedPersonEntity, eChAddressEntity, household);

				// NOTE We add the newly created entity to the dictionary of the entities that have
				// been created but not yet saved.

				eChPersonIdToEntity[eChPerson.Id] = result.Item2;

				return result;
			}
		}


		private static Tuple<eCH_PersonEntity, AiderPersonEntity> Import(BusinessContext businessContext, EChPerson eChPerson, eCH_ReportedPersonEntity eChReportedPersonEntity, eCH_AddressEntity eChAddressEntity, AiderHouseholdEntity household)
		{
			var aiderPersonEntity = EChDataImporter.CreateAndRegisterEntity<AiderPersonEntity> (businessContext);
			aiderPersonEntity.Household1 = household;

			var eChPersonEntity = aiderPersonEntity.eCH_Person;

			eChPersonEntity.PersonId = eChPerson.Id;
			eChPersonEntity.PersonOfficialName = eChPerson.OfficialName;
			eChPersonEntity.PersonFirstNames = eChPerson.FirstNames;
			eChPersonEntity.PersonDateOfBirth = eChPerson.DateOfBirth;
			eChPersonEntity.PersonSex = eChPerson.Sex;
			eChPersonEntity.NationalityStatus = eChPerson.NationalityStatus;
			eChPersonEntity.NationalityCountryCode = eChPerson.NationalCountryCode;
			eChPersonEntity.Origins = eChPerson.OriginPlaces
				.Select (p => p.Name + " (" + p.Canton + ")")
				.Join ("\n");
			eChPersonEntity.AdultMaritalStatus = eChPerson.MaritalStatus;

			eChPersonEntity.CreationDate = Date.Today;
			eChPersonEntity.DataSource = Enumerations.DataSource.Government;
			eChPersonEntity.DeclarationStatus = PersonDeclarationStatus.Declared;
			eChPersonEntity.RemovalReason = RemovalReason.None;

			eChPersonEntity.ReportedPerson1 = eChReportedPersonEntity;
			eChPersonEntity.Address1 = eChAddressEntity;

			return Tuple.Create (eChPersonEntity, aiderPersonEntity);
		}


		private static eCH_AddressEntity Import(BusinessContext businessContext, EChAddress eChAddress)
		{
			var eChAddressEntity = EChDataImporter.CreateAndRegisterEntity<eCH_AddressEntity> (businessContext);

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


		private static T CreateAndRegisterEntity<T>(BusinessContext businessContext) where T : AbstractEntity, new ()
		{
			var entity = businessContext.CreateEntity<T> ();

			businessContext.Register (entity);

			return entity;
		}


	}


}
