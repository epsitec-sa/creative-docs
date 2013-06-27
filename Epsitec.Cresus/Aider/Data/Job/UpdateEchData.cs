using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Loader;

namespace Epsitec.Aider.Data.Job
{
	internal static class UpdateEChData
	{

		public static void StartJob(string oldEchFile, string newEchFile,string reportFile, CoreData coreData)
		{

			if (System.IO.File.Exists (oldEchFile) && System.IO.File.Exists (newEchFile))
			{

                Console.WriteLine("ECH DATA UPDATER : START ANALYSER");
				UpdateEChData.Analyser = new EChDataAnalyser(oldEchFile, newEchFile,true);
				
                //Console.WriteLine("ECH DATA UPDATER : CREATING REPORT OF CHANGES ON " + reportFile);

                //UpdateEChData.Analyser.CreateReport(reportFile);


				//if (UpdateEChData.UpdateEChPerson (coreData))
				//{
				//	UpdateEChData.UpdateEChReportedPersons (coreData);

				//	UpdateEChData.CreateNewEChReportedPerson (coreData);

				//}
				//else
				//{
				//	Console.WriteLine ("ECH DATA UPDATER : FAIL... VERIFY YOUR DATA");
				//}

			}
			else
			{
				Console.WriteLine("ECH DATA UPDATER : FAIL... VERIFY YOUR ECH FILES PARAMETERS");
			}

		}

		private static  EChDataAnalyser Analyser;

        public static void CreateNewEChPerson(CoreData coreData)
        {
            Console.WriteLine("ECH DATA UPDATER : START CREATE ECH PERSON JOB");

            using (var businessContext = new BusinessContext(coreData, false))
            {
                foreach (var eChPerson in UpdateEChData.Analyser.GetPersonToAdd())
                {
                    var existingPersonEntity = UpdateEChData.GetEchPersonEntity(businessContext, eChPerson);

                    if (existingPersonEntity == null)
                    {
                        var newPersonEntity = businessContext.CreateAndRegisterEntity<eCH_PersonEntity>();
                        newPersonEntity.PersonId = eChPerson.Id;
                        newPersonEntity.PersonOfficialName = eChPerson.OfficialName;
                        newPersonEntity.PersonFirstNames = eChPerson.FirstNames;
                        newPersonEntity.PersonDateOfBirth = eChPerson.DateOfBirth;
                        newPersonEntity.PersonSex = eChPerson.Sex;
                        newPersonEntity.NationalityStatus = eChPerson.NationalityStatus;
                        newPersonEntity.NationalityCountryCode = eChPerson.NationalCountryCode;
                        newPersonEntity.Origins = eChPerson.OriginPlaces
                            .Select(p => p.Name + " (" + p.Canton + ")")
                            .Join("\n");
                        newPersonEntity.AdultMaritalStatus = eChPerson.MaritalStatus;
                        newPersonEntity.CreationDate = Date.Today;
                        newPersonEntity.DataSource = Enumerations.DataSource.Government;
                        newPersonEntity.DeclarationStatus = PersonDeclarationStatus.Declared;
                        newPersonEntity.RemovalReason = RemovalReason.None;
                    }
                    else
                    {
                        existingPersonEntity.PersonOfficialName = eChPerson.OfficialName;
                        existingPersonEntity.PersonFirstNames = eChPerson.FirstNames;
                        existingPersonEntity.PersonDateOfBirth = eChPerson.DateOfBirth;
                        existingPersonEntity.PersonSex = eChPerson.Sex;
                        existingPersonEntity.NationalityStatus = eChPerson.NationalityStatus;
                        existingPersonEntity.NationalityCountryCode = eChPerson.NationalCountryCode;
                        existingPersonEntity.Origins = eChPerson.OriginPlaces
                            .Select(p => p.Name + " (" + p.Canton + ")")
                            .Join("\n");
                        existingPersonEntity.AdultMaritalStatus = eChPerson.MaritalStatus;
                        existingPersonEntity.CreationDate = Date.Today;
                        existingPersonEntity.DataSource = Enumerations.DataSource.Government;
                        existingPersonEntity.DeclarationStatus = PersonDeclarationStatus.Declared;
                        existingPersonEntity.RemovalReason = RemovalReason.None;
                    }
                }
                businessContext.SaveChanges(LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
                Console.WriteLine("ECH DATA UPDATER : JOB DONE!");
            }
        }

        public static void TagForDeletionEChPersonEntity(CoreData coreData)
        {
            Console.WriteLine("ECH DATA UPDATER : START TAG FOR DELETION ECH PERSON JOB");
            using (var businessContext = new BusinessContext(coreData, false))
            {
                foreach (var eChPerson in UpdateEChData.Analyser.GetPersonToRemove())
                {
                    var existingPersonEntity = UpdateEChData.GetEchPersonEntity(businessContext, eChPerson);
                    var existingAiderPersonEntity = UpdateEChData.GetAiderPersonEntity(businessContext, existingPersonEntity);

                    if (existingPersonEntity != null)
                    {
                        existingPersonEntity.RemovalReason = RemovalReason.Departed;

                        if (existingAiderPersonEntity != null)
                        {
                            var warning = businessContext.CreateAndRegisterEntity<AiderPersonWarningEntity>();
                            warning.Title = "Mise à jour ECh";
                            warning.Description = "Cette personne n'est plus dans le registre ECh depuis la dernière mise à jour";
                            warning.WarningType = WarningType.Mismatch;

                            existingAiderPersonEntity.Warnings.Add(warning);
                        }
                    }
                }
                businessContext.SaveChanges(LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
                Console.WriteLine("ECH DATA UPDATER : JOB DONE!");
            }
        }

        public static void CreateNewEChReportedPerson(CoreData coreData)
        {
            Console.WriteLine("ECH DATA UPDATER : START CREATE FAMILY JOB");

            var newFamilies = UpdateEChData.Analyser.GetNewFamilies();
            Console.WriteLine(newFamilies.Count() + " NEW ECH REPORTED PERSON TO IMPORT");
            var parishRepository = ParishAddressRepository.Current;

            EChDataImporter.Import(coreData, parishRepository, newFamilies.ToList());


            Console.WriteLine("ECH DATA UPDATER : JOB DONE!");
        }

		private static bool UpdateEChPerson(CoreData coreData)
		{

			Console.WriteLine("ECH DATA UPDATER : START UPDATE PERSON JOB");
			var personsToChange = UpdateEChData.Analyser.GetPersonToChange();
			Console.WriteLine(personsToChange.Count + " ECH PERSON TO CHANGE");

			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach(var toChange in personsToChange)
				{
					try
					{
						var personEntityToUpdate = UpdateEChData.GetEchPersonEntity(businessContext, toChange.Item1);
						var changedEChPersonEntity = new eCH_PersonEntity ();
						EChDataImporter.ConvertEChPersonToEntity (toChange.Item1, changedEChPersonEntity);

						if (!toChange.Item1.DateOfBirth.Equals(toChange.Item2.DateOfBirth))
						{
							personEntityToUpdate.PersonDateOfBirth = changedEChPersonEntity.PersonDateOfBirth;
						}

						if (!toChange.Item1.FirstNames.Equals(toChange.Item2.FirstNames))
						{
							personEntityToUpdate.PersonFirstNames = changedEChPersonEntity.PersonFirstNames;
						}

						if (!toChange.Item1.MaritalStatus.Equals(toChange.Item2.MaritalStatus))
						{
							personEntityToUpdate.AdultMaritalStatus = changedEChPersonEntity.AdultMaritalStatus;
						}

						if (!toChange.Item1.NationalCountryCode.Equals(toChange.Item2.NationalCountryCode))
						{
							personEntityToUpdate.NationalityCountryCode = changedEChPersonEntity.NationalityCountryCode;
						}

						if (!toChange.Item1.NationalityStatus.Equals(toChange.Item2.NationalityStatus))
						{
							personEntityToUpdate.NationalityStatus = changedEChPersonEntity.NationalityStatus;
						}

						if (!toChange.Item1.OfficialName.Equals(toChange.Item2.OfficialName))
						{
							personEntityToUpdate.PersonOfficialName = changedEChPersonEntity.PersonOfficialName;
						}

						if (!toChange.Item1.OriginPlaces.Equals(toChange.Item2.OriginPlaces))
						{
							personEntityToUpdate.Origins = changedEChPersonEntity.Origins;
						}

						if (!toChange.Item1.Sex.Equals(toChange.Item2.Sex))
						{
							personEntityToUpdate.PersonSex = changedEChPersonEntity.PersonSex;
						}
					}
					catch(Exception)
					{
						Console.WriteLine("ECH DATA UPDATER : ERROR DURING UPDATE, ABORT");
						return false;
					}
				}

				businessContext.SaveChanges(LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
				return true;
			}
		}

		private static bool UpdateEChReportedPersons(CoreData coreData)
		{

			Console.WriteLine("ECH DATA UPDATER : START UPDATE REPORTED PERSON JOB");
			var reportedPersonsToChange = UpdateEChData.Analyser.GetFamilyToChange();
			Console.WriteLine(reportedPersonsToChange.Count + " ECH REPORTED PERSON TO CHANGE");
			using (var businessContext = new BusinessContext(coreData, false))
			{
				foreach (var toChange in reportedPersonsToChange)
				{
					try
					{
						var reportedPersonEntityToUpdate = UpdateEChData.GetEchReportedPersonEntity(businessContext, toChange.Item1);

						if (!String.IsNullOrEmpty(toChange.Item1.Address.AddressLine1))
						{
							if (!toChange.Item1.Address.AddressLine1.Equals(toChange.Item2.Address.AddressLine1))
							{
								reportedPersonEntityToUpdate.Address.AddressLine1 = toChange.Item1.Address.AddressLine1;
							}
						}
						if (!toChange.Item1.Address.CountryCode.Equals(toChange.Item2.Address.CountryCode))
						{
							reportedPersonEntityToUpdate.Address.Country = toChange.Item1.Address.CountryCode;
						}

						if (!toChange.Item1.Address.HouseNumber.Equals(toChange.Item2.Address.HouseNumber))
						{
							reportedPersonEntityToUpdate.Address.HouseNumber = toChange.Item1.Address.HouseNumber;
						}

						if (!toChange.Item1.Address.Street.Equals(toChange.Item2.Address.Street))
						{
							reportedPersonEntityToUpdate.Address.Street = toChange.Item1.Address.Street;
						}

						if (!toChange.Item1.Address.SwissZipCode.Equals(toChange.Item2.Address.SwissZipCode))
						{
							reportedPersonEntityToUpdate.Address.SwissZipCode = toChange.Item1.Address.SwissZipCode;
						}

						if (!toChange.Item1.Address.SwissZipCodeAddOn.Equals(toChange.Item2.Address.SwissZipCodeAddOn))
						{
							reportedPersonEntityToUpdate.Address.SwissZipCodeAddOn = toChange.Item1.Address.SwissZipCodeAddOn;
						}

						if (!toChange.Item1.Address.SwissZipCodeId.Equals(toChange.Item2.Address.SwissZipCodeId))
						{
							reportedPersonEntityToUpdate.Address.SwissZipCodeId = toChange.Item1.Address.SwissZipCodeId;
						}

						if (!toChange.Item1.Address.Town.Equals(toChange.Item2.Address.Town))
						{
							reportedPersonEntityToUpdate.Address.Town = toChange.Item1.Address.Town;
						}
					}
					catch(Exception)
					{
						Console.WriteLine("ECH DATA UPDATER : ERROR DURING UPDATE, ABORT");
						return false;
					}
				}

				businessContext.SaveChanges(LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine("ECH DATA UPDATER : JOB DONE!");
				return true;
			}
		}

		private static eCH_PersonEntity GetEchPersonEntity(BusinessContext businessContext, EChPerson person)
		{

			if (person == null)
			{
				return null;
			}
			var personExample = new eCH_PersonEntity ()
			{
				PersonId = person.Id
			};

			var request = new Request ()
			{
				RootEntity = personExample
			};


			return businessContext.DataContext.GetByRequest<eCH_PersonEntity> (request).First ();
		}

        private static AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, eCH_PersonEntity person)
        {

            if (person == null)
            {
                return null;
            }
            var personExample = new AiderPersonEntity()
            {
                eCH_Person = person
            };

            var request = new Request()
            {
                RootEntity = personExample
            };

            return businessContext.DataContext.GetByRequest<AiderPersonEntity>(request).First();
        }

		private static eCH_ReportedPersonEntity GetEchReportedPersonEntity(BusinessContext businessContext, EChReportedPerson reportedPerson)
		{
			var adult1 = GetEchPersonEntity(businessContext, reportedPerson.Adult1);
			var adult2 = GetEchPersonEntity(businessContext, reportedPerson.Adult2);

			var reportedPersonExample = new eCH_ReportedPersonEntity() { };

			if (adult1 != null && adult2 != null)
			{
				reportedPersonExample = new eCH_ReportedPersonEntity()
				{
					Adult1 = adult1,
					Adult2 = adult2
				};
			}
			if (adult1 != null && adult2 == null)
			{
				reportedPersonExample = new eCH_ReportedPersonEntity()
				{
					Adult1 = adult1
				};
			}

			var request = new Request()
			{
				RootEntity = reportedPersonExample
			};


			return businessContext.DataContext.GetByRequest<eCH_ReportedPersonEntity>(request).First();
		}
	}
}
