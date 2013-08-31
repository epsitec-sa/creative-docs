//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel Loup, Maintainer: Samuel Loup

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using Epsitec.Data.Platform;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
	internal class EChDataUpdater
	{
		public static void StartJob(string oldEchFile, string newEchFile, string reportFile, CoreData coreData, ParishAddressRepository parishAddressRepository)
		{
			if (System.IO.File.Exists (oldEchFile) && System.IO.File.Exists (newEchFile))
			{
				Console.WriteLine ("ECH DATA UPDATER : START ANALYSER");
				
				using (var analyser = new EChDataAnalyserReporter (oldEchFile, newEchFile, reportFile))
				{
					var personsToCreate = analyser.GetPersonsToAdd ().ToList ();
					var personsToRemove = analyser.GetPersonsToRemove ().ToList ();
					var personsToUpdate = analyser.GetPersonsToChange ().ToList ();

					var houseHoldsToCreate = analyser.GetFamiliesToAdd ().ToList ();
					var houseHoldsToRemove = analyser.GetFamiliesToRemove ().ToList ();
					var houseHoldsToUpdate = analyser.GetFamiliesToChange ().ToList ();

					var newHouseHoldsToCreate     = analyser.GetNewFamilies ().ToList ();
					var missingHouseHoldsToRemove = analyser.GetMissingFamilies ().ToList ();

					
					//Appli update to EChPerson and add a DataChangedECh warning on AiderPerson
					EChDataUpdater.UpdateEChPersonEntities (coreData, personsToUpdate);
					EChDataUpdater.UpdateHouseholdsAndPropagate (coreData, houseHoldsToUpdate, parishAddressRepository);
					EChDataUpdater.TagForDeletionEChPersonEntities (coreData, personsToRemove);
                    EChDataUpdater.AiderPersonEntitiesTagedForDeletion = new Dictionary<EntityKey, AiderPersonEntity>();
                    EChDataUpdater.TagForDeletionAiderPersonEntities(coreData, personsToRemove);
                    EChDataUpdater.TagAiderPersonEntitiesForHouseholdMissing(coreData, missingHouseHoldsToRemove);
					EChDataUpdater.CreateNewEChPersonEntities (coreData, personsToCreate);
                    EChDataUpdater.CreateNewAiderPersonEntities(coreData, personsToCreate, parishAddressRepository);
                    EChDataUpdater.AiderPersonEntitiesWithDeletedHousehold = new Dictionary<EntityKey, AiderHouseholdEntity>();
                    EChDataUpdater.RemoveOldEChReportedPersonEntities(coreData, houseHoldsToRemove);
                    EChDataUpdater.CreateNewEChReportedPersonEntities(coreData, houseHoldsToCreate);
                    EChDataUpdater.CreateNewAiderHouseholdEntities(coreData, newHouseHoldsToCreate);		
				}
			}
			else
			{
				Console.WriteLine ("ECH DATA UPDATER : FAIL... VERIFY YOUR ECH FILES PARAMETERS");
			}


		}

		private static void CreateNewAiderPersonEntities(CoreData coreData, List<EChPerson> personsToCreate, ParishAddressRepository parishAddressRepository)
		{
			Console.WriteLine ("ECH DATA UPDATER : START CREATE AIDER PERSON JOB");

			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var eChPerson in personsToCreate)
				{
					var eChPersonEntity = EChDataUpdater.GetEchPersonEntity (businessContext, eChPerson);
					var existingAiderPersonEntity = EChDataUpdater.GetAiderPersonEntity (businessContext, eChPersonEntity);

					if (existingAiderPersonEntity == null)
					{
						var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();

						aiderPersonEntity.eCH_Person = eChPersonEntity;
						aiderPersonEntity.MrMrs = EChDataImporter.GuessMrMrs (eChPerson.Sex, eChPerson.DateOfBirth, eChPerson.MaritalStatus);
						aiderPersonEntity.Confession = PersonConfession.Protestant;

						ParishAssigner.AssignToParish (parishAddressRepository, businessContext, aiderPersonEntity);

						AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChProcessArrival, EChDataUpdater.WarningTitleMessage, FormattedText.FromSimpleText (aiderPersonEntity.GetDisplayName () + " est arrivée dans le registre ECh!"));

					}
					else
					{
						AiderPersonWarningEntity.Create (businessContext, existingAiderPersonEntity, existingAiderPersonEntity.ParishGroupPathCache, WarningType.EChPersonDuplicated, EChDataUpdater.WarningTitleMessage, FormattedText.FromSimpleText (existingAiderPersonEntity.GetDisplayName () + " existe déjà dans Aider"));
					}

				}
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
			}
		}

		private static void CreateNewEChPersonEntities(CoreData coreData, List<EChPerson> personsToCreate)
		{
			Console.WriteLine ("ECH DATA UPDATER : START CREATE ECH PERSON JOB");

			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var eChPerson in personsToCreate)
				{
					var existingPersonEntity = EChDataUpdater.GetEchPersonEntity (businessContext, eChPerson);

					if (existingPersonEntity == null)
					{
						var newPersonEntity = businessContext.CreateAndRegisterEntity<eCH_PersonEntity> ();
						newPersonEntity.PersonId = eChPerson.Id;
						newPersonEntity.PersonOfficialName = eChPerson.OfficialName;
						newPersonEntity.PersonFirstNames = eChPerson.FirstNames;
						newPersonEntity.PersonDateOfBirth = eChPerson.DateOfBirth;
						newPersonEntity.PersonSex = eChPerson.Sex;
						newPersonEntity.NationalityStatus = eChPerson.NationalityStatus;
						newPersonEntity.NationalityCountryCode = eChPerson.NationalCountryCode;
						newPersonEntity.Origins = eChPerson.OriginPlaces
							.Select (p => p.Name + " (" + p.Canton + ")")
							.Join ("\n");
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
							.Select (p => p.Name + " (" + p.Canton + ")")
							.Join ("\n");
						existingPersonEntity.AdultMaritalStatus = eChPerson.MaritalStatus;
						existingPersonEntity.CreationDate = Date.Today;
						existingPersonEntity.DataSource = Enumerations.DataSource.Government;
						existingPersonEntity.DeclarationStatus = PersonDeclarationStatus.Declared;
						existingPersonEntity.RemovalReason = RemovalReason.None;
					}
				}
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
			}
		}

		private static void TagForDeletionEChPersonEntities(CoreData coreData, List<EChPerson> personsToRemove)
		{
			Console.WriteLine ("ECH DATA UPDATER : START TAG FOR DELETION ECH PERSON JOB");
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var eChPerson in personsToRemove)
				{
					var existingPersonEntity = EChDataUpdater.GetEchPersonEntity (businessContext, eChPerson);

					if (existingPersonEntity != null)
					{
						existingPersonEntity.RemovalReason = RemovalReason.Departed;
					}
				}
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
			}
		}

		private static void TagForDeletionAiderPersonEntities(CoreData coreData, List<EChPerson> personsToRemove)
		{
			Console.WriteLine ("ECH DATA UPDATER : START TAG FOR DELETION AIDER PERSON JOB");
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var eChPerson in personsToRemove)
				{
					var existingPersonEntity = EChDataUpdater.GetEchPersonEntity (businessContext, eChPerson);
					var existingAiderPersonEntity = EChDataUpdater.GetAiderPersonEntity (businessContext, existingPersonEntity);

					if (existingAiderPersonEntity != null)
					{
						AiderPersonWarningEntity.Create (businessContext, existingAiderPersonEntity, existingAiderPersonEntity.ParishGroupPathCache, WarningType.EChPersonMissing, EChDataUpdater.WarningTitleMessage, FormattedText.FromSimpleText (existingAiderPersonEntity.GetDisplayName () + " n'est plus dans le registre ECh!"));

						var key = businessContext.DataContext.GetNormalizedEntityKey (existingAiderPersonEntity).Value;
						EChDataUpdater.AiderPersonEntitiesTagedForDeletion.Add (key, existingAiderPersonEntity);
					}

				}
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
			}
		}

		private static void CreateNewEChReportedPersonEntities(CoreData coreData, List<EChReportedPerson> houseHoldsToCreate)
		{
			Console.WriteLine ("ECH DATA UPDATER : START CREATE ECH HOUSEHOLD JOB");
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var eChReportedPerson in houseHoldsToCreate)
				{
					//	Create eCH ReportedPerson entity, with the new eCH address, and assign the
					//	two adults (if any) and the children found in the eCH data:

					var eChAddressEntity          = EChDataImporter.ImportEchAddressEntity (businessContext, eChReportedPerson.Address);
					var eChReportedPersonEntity   = businessContext.CreateAndRegisterEntity<eCH_ReportedPersonEntity> ();

					eChReportedPersonEntity.Address = eChAddressEntity;

					if (eChReportedPerson.Adult1 != null)	//	always true...
					{
						eChReportedPersonEntity.Adult1 = EChDataUpdater.GetEchPersonEntity (businessContext, eChReportedPerson.Adult1);
					}

					if (eChReportedPerson.Adult2 != null)
					{
						eChReportedPersonEntity.Adult2 = EChDataUpdater.GetEchPersonEntity (businessContext, eChReportedPerson.Adult2);
					}

					foreach (var eChChild in eChReportedPerson.Children)
					{
						eChReportedPersonEntity.Children.Add (EChDataUpdater.GetEchPersonEntity (businessContext, eChChild));
					}

					//	Create warnings for the AiderPerson entity:

					var referenceAdult    = eChReportedPersonEntity.Adult1;
					var aiderPersonEntity = EChDataUpdater.GetAiderPersonEntity (businessContext, referenceAdult);

					if (aiderPersonEntity.IsNotNull ())
					{
						AiderHouseholdEntity oldHousehold;

						var key = businessContext.DataContext.GetNormalizedEntityKey (aiderPersonEntity).Value;

						if (EChDataUpdater.AiderPersonEntitiesWithDeletedHousehold.TryGetValue (key, out oldHousehold))
						{
							var warningMessage = FormattedText.FromSimpleText ("Un changement de composition a eu lieu dans ce ménage.");

							AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChHouseholdChanged, EChDataUpdater.WarningTitleMessage, warningMessage);
						}
						else
						{
							var warningMessage = FormattedText.FromSimpleText ("Cette personne a maintenant son propre ménage");

							AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChHouseholdAdded, EChDataUpdater.WarningTitleMessage, warningMessage);
						}
					}
					else
					{
						Console.WriteLine ("Error: Adult with PersonId={0} has no matching AiderPerson", referenceAdult.PersonId);
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
			}
		}

		private static void CreateNewAiderHouseholdEntities(CoreData coreData, List<EChReportedPerson> newHouseHoldsToCreate)
		{
			Console.WriteLine ("ECH DATA UPDATER : START CREATE NEW AIDER HOUSEHOLD JOB");
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var eChReportedPerson in newHouseHoldsToCreate)
				{
					var aiderHousehold = businessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
					aiderHousehold.HouseholdMrMrs = HouseholdMrMrs.Auto;

					var aiderAddressEntity = aiderHousehold.Address;
					var eChAddressEntity = EChDataUpdater.GetEchAddressEntity (businessContext, eChReportedPerson.Address);


					var houseNumber = StringUtils.ParseNullableInt (SwissPostStreet.StripHouseNumber (eChAddressEntity.HouseNumber));
					var houseNumberComplement = SwissPostStreet.GetHouseNumberComplement (eChAddressEntity.HouseNumber);

					if (string.IsNullOrWhiteSpace (houseNumberComplement))
					{
						houseNumberComplement = null;
					}

					aiderAddressEntity.AddressLine1 = eChAddressEntity.AddressLine1;
					aiderAddressEntity.Street = eChAddressEntity.Street;
					aiderAddressEntity.HouseNumber = houseNumber;
					aiderAddressEntity.HouseNumberComplement = houseNumberComplement;
					aiderAddressEntity.Town = EChDataUpdater.GetAiderTownEntity (businessContext, eChReportedPerson.Address);


					//Link household to ECh Entity
					var eChReportedPersonEntity = EChDataUpdater.GetEchReportedPersonEntity (businessContext, eChReportedPerson);
					if (eChReportedPersonEntity.Adult1.IsNotNull ())
					{
						var aiderPerson = EChDataUpdater.GetAiderPersonEntity (businessContext, eChReportedPersonEntity.Adult1);

						EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isHead1: true);
					}

					if (eChReportedPersonEntity.Adult2.IsNotNull ())
					{
						var aiderPerson = EChDataUpdater.GetAiderPersonEntity (businessContext, eChReportedPersonEntity.Adult2);

						EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isHead2: true);
					}

					foreach (var child in eChReportedPersonEntity.Children)
					{
						var aiderPerson = EChDataUpdater.GetAiderPersonEntity (businessContext, child);

						EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isChild: true);
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
			}
		}

		private static void RemoveOldEChReportedPersonEntities(CoreData coreData, List<EChReportedPerson> houseHoldsToRemove)
		{
			Console.WriteLine ("ECH DATA UPDATER : START ECH HOUSEHOLD DELETION JOB");
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var eChReportedPerson in houseHoldsToRemove)
				{
					var eChReportedPersonEntity = EChDataUpdater.GetEchReportedPersonEntity (businessContext, eChReportedPerson);
					
					//	Unlink ECh Person Entity

					var eChPersonEntityAdult1 = EChDataUpdater.GetEchPersonEntity (businessContext, eChReportedPerson.Adult1);
					var aiderPersonEntity     = EChDataUpdater.GetAiderPersonEntity (businessContext, eChPersonEntityAdult1);
					
					if (aiderPersonEntity.IsNotNull ())
					{
						var personKey      = businessContext.DataContext.GetNormalizedEntityKey(aiderPersonEntity).Value;
						var aiderHousehold = EChDataUpdater.GetAiderHousehold (businessContext, aiderPersonEntity);

						if (aiderHousehold.IsNotNull ())
						{
							EChDataUpdater.AiderPersonEntitiesWithDeletedHousehold.Add (personKey, aiderHousehold);
						}
						else
						{
							Console.WriteLine ("Error: AiderPerson {0} ({1}) has no associated AiderHousehold", aiderPersonEntity.DisplayName, personKey);
						}
					}

					eChPersonEntityAdult1.ReportedPerson1 = null;

					if (eChReportedPerson.Adult2 != null)
					{
						var eChPersonEntityAdult2 = EChDataUpdater.GetEchPersonEntity (businessContext, eChReportedPerson.Adult2);
						eChPersonEntityAdult2.ReportedPerson2 = null;
					}

					businessContext.DeleteEntity (eChReportedPersonEntity.Address);
					businessContext.DeleteEntity (eChReportedPersonEntity);
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
			}
		}

		private static void TagAiderPersonEntitiesForHouseholdMissing(CoreData coreData, List<EChReportedPerson> missingHouseHoldsToRemove)
		{
			Console.WriteLine ("ECH DATA UPDATER : START HOUSEHOLD MISSING TAG JOB");
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var eChReportedPerson in missingHouseHoldsToRemove)
				{
					foreach (var eChPerson in eChReportedPerson.GetMembers ())
					{
						var eChPersonEntity   = EChDataUpdater.GetEchPersonEntity (businessContext, eChPerson);
						var aiderPersonEntity = EChDataUpdater.GetAiderPersonEntity (businessContext, eChPersonEntity);

						var personKey = businessContext.DataContext.GetNormalizedEntityKey (aiderPersonEntity).Value;

						if (EChDataUpdater.AiderPersonEntitiesTagedForDeletion.ContainsKey (personKey))
						{
							foreach (var warn in aiderPersonEntity.Warnings)
							{
								if (warn.WarningType.Equals (WarningType.EChPersonMissing) && EChDataUpdater.WarningTitleMessage.Equals (warn.Title))
								{
									warn.WarningType = WarningType.EChProcessDeparture;
								}
							}
						}
						else
						{
							AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChHouseholdMissing, EChDataUpdater.WarningTitleMessage, FormattedText.FromSimpleText (aiderPersonEntity.GetDisplayName () + " n'est plus assigné a une famille dans le registre ECh!"));
						}

					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
			}
		}

		private static bool UpdateEChPersonEntities(CoreData coreData, List<Change<EChPerson>> personsToUpdate)
		{
			Console.WriteLine ("ECH DATA UPDATER : START UPDATE PERSON JOB");
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var toChange in personsToUpdate)
				{
					try
					{
						var personEntityToUpdate = EChDataUpdater.GetEchPersonEntity (businessContext, toChange.OldValue);
						var aiderPersonEntity = EChDataUpdater.GetAiderPersonEntity (businessContext, personEntityToUpdate);
						var changedEChPersonEntity = new eCH_PersonEntity ();
						EChDataImporter.ConvertEChPersonToEntity (toChange.OldValue, changedEChPersonEntity);
						var changes = new List<string> ();
						var mustWarn = false;
						changes.Add (aiderPersonEntity.GetFullName ());

						if (!toChange.OldValue.OfficialName.Equals (toChange.NewValue.OfficialName))
						{
							personEntityToUpdate.PersonOfficialName = changedEChPersonEntity.PersonOfficialName;
							changes.Add ("Nom: " + toChange.NewValue.OfficialName + " -> " + changedEChPersonEntity.PersonOfficialName);
							mustWarn = true;
						}

						if (!toChange.OldValue.FirstNames.Equals (toChange.NewValue.FirstNames))
						{
							personEntityToUpdate.PersonFirstNames = changedEChPersonEntity.PersonFirstNames;
							changes.Add ("Prénom: " + toChange.NewValue.FirstNames + " -> " + changedEChPersonEntity.PersonFirstNames);
							mustWarn = true;
						}

						if (!toChange.OldValue.DateOfBirth.Equals (toChange.NewValue.DateOfBirth))
						{
							personEntityToUpdate.PersonDateOfBirth = changedEChPersonEntity.PersonDateOfBirth;
							changes.Add ("Date de naissance: " + toChange.NewValue.DateOfBirth + " -> " + changedEChPersonEntity.PersonDateOfBirth);
							mustWarn = true;
						}

						if (!toChange.OldValue.MaritalStatus.Equals (toChange.NewValue.MaritalStatus))
						{
							personEntityToUpdate.AdultMaritalStatus = changedEChPersonEntity.AdultMaritalStatus;
							changes.Add ("Etat civil: " + toChange.NewValue.MaritalStatus + " -> " + changedEChPersonEntity.AdultMaritalStatus);
							mustWarn = true;
						}

						if (!toChange.OldValue.NationalCountryCode.Equals (toChange.NewValue.NationalCountryCode))
						{
							personEntityToUpdate.NationalityCountryCode = changedEChPersonEntity.NationalityCountryCode;
							changes.Add ("Nationalité: " + toChange.NewValue.NationalCountryCode + " -> " + changedEChPersonEntity.NationalityCountryCode);
							mustWarn = true;
						}

						if (!toChange.OldValue.NationalityStatus.Equals (toChange.NewValue.NationalityStatus))
						{
							personEntityToUpdate.NationalityStatus = changedEChPersonEntity.NationalityStatus;
							changes.Add ("Statut nationalité: " + toChange.NewValue.NationalityStatus + " -> " + changedEChPersonEntity.NationalityStatus);
							mustWarn = true;
						}

                        var oldOriginList = toChange.OldValue.OriginPlaces.Select(p => System.Tuple.Create(p.Canton, p.Name)).ToList();
                        var newOriginList = toChange.NewValue.OriginPlaces.Select(p => System.Tuple.Create(p.Canton, p.Name)).ToList();

                        if (!oldOriginList.SetEquals(newOriginList))
						{
							personEntityToUpdate.Origins = changedEChPersonEntity.Origins;
							changes.Add ("Origines: " + String.Join (" ", toChange.NewValue.OriginPlaces.Select (o => o.Display ()).ToList ()) + " -> " + String.Join (" ", changedEChPersonEntity.Origins));
						}

						if (!toChange.OldValue.Sex.Equals (toChange.NewValue.Sex))
						{
							personEntityToUpdate.PersonSex = changedEChPersonEntity.PersonSex;
							changes.Add ("Sex: " + toChange.NewValue.Sex + " -> " + changedEChPersonEntity.PersonSex);
							mustWarn = true;
						}

						if (mustWarn)
						{
							AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChPersonDataChanged, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
						}
					}
					catch (Exception)
					{
						Console.WriteLine ("Error: EChPerson {0} {1} throw exception",toChange.NewValue.OfficialName,toChange.NewValue.FirstNames);
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
				return true;
			}
		}

		private static bool UpdateHouseholdsAndPropagate(CoreData coreData, List<Change<EChReportedPerson>> houseHoldsToUpdate, ParishAddressRepository parishAddressRepository)
		{
			Console.WriteLine ("ECH DATA UPDATER : START UPDATE REPORTED PERSON JOB");
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var toChange in houseHoldsToUpdate)
				{
					try
					{
						var reportedPersonEntityToUpdate = EChDataUpdater.GetEchReportedPersonEntity (businessContext, toChange.OldValue);
						var changes = new List<string> ();
						changes.Add ("Changement dans l'adresse:");
						if (!String.IsNullOrEmpty (toChange.OldValue.Address.AddressLine1))
						{
							if (!toChange.OldValue.Address.AddressLine1.Equals (toChange.NewValue.Address.AddressLine1))
							{
								reportedPersonEntityToUpdate.Address.AddressLine1 = toChange.OldValue.Address.AddressLine1;
								changes.Add ("Ligne adresse: " + toChange.NewValue.Address.AddressLine1 + " -> " + reportedPersonEntityToUpdate.Address.AddressLine1);
							}
						}

						if (!String.IsNullOrEmpty (toChange.OldValue.Address.HouseNumber))
						{
							if (!toChange.OldValue.Address.HouseNumber.Equals (toChange.NewValue.Address.HouseNumber))
							{
								reportedPersonEntityToUpdate.Address.HouseNumber = toChange.OldValue.Address.HouseNumber;
								changes.Add ("N° de maison: " + toChange.NewValue.Address.HouseNumber + " -> " + reportedPersonEntityToUpdate.Address.HouseNumber);
							}
						}
						if (!String.IsNullOrEmpty (toChange.OldValue.Address.Street))
						{
							if (!toChange.OldValue.Address.Street.Equals (toChange.NewValue.Address.Street))
							{
								reportedPersonEntityToUpdate.Address.Street = toChange.OldValue.Address.Street;
								changes.Add ("Rue: " + toChange.NewValue.Address.Street + " -> " + reportedPersonEntityToUpdate.Address.Street);
							}
						}
						if (!toChange.OldValue.Address.SwissZipCode.Equals (toChange.NewValue.Address.SwissZipCode))
						{
							reportedPersonEntityToUpdate.Address.SwissZipCode = toChange.OldValue.Address.SwissZipCode;
							changes.Add ("NPA: " + toChange.NewValue.Address.SwissZipCode + " -> " + toChange.OldValue.Address.SwissZipCode);
						}

						if (!toChange.OldValue.Address.SwissZipCodeAddOn.Equals (toChange.NewValue.Address.SwissZipCodeAddOn))
						{
							reportedPersonEntityToUpdate.Address.SwissZipCodeAddOn = toChange.OldValue.Address.SwissZipCodeAddOn;
							changes.Add ("NPA+: " + toChange.NewValue.Address.SwissZipCodeAddOn + " -> " + reportedPersonEntityToUpdate.Address.SwissZipCodeAddOn);
						}

						if (!toChange.OldValue.Address.SwissZipCodeId.Equals (toChange.NewValue.Address.SwissZipCodeId))
						{
							reportedPersonEntityToUpdate.Address.SwissZipCodeId = toChange.OldValue.Address.SwissZipCodeId;
							changes.Add ("NPA ID: " + toChange.NewValue.Address.SwissZipCodeId + " -> " + reportedPersonEntityToUpdate.Address.SwissZipCodeId);
						}

						if (!toChange.OldValue.Address.Town.Equals (toChange.NewValue.Address.Town))
						{
							reportedPersonEntityToUpdate.Address.Town = toChange.OldValue.Address.Town;
							changes.Add ("Localité: " + toChange.NewValue.Address.Town + " -> " + reportedPersonEntityToUpdate.Address.Town);
						}

						if (!toChange.OldValue.Address.CountryCode.Equals (toChange.NewValue.Address.CountryCode))
						{
							reportedPersonEntityToUpdate.Address.Country = toChange.OldValue.Address.CountryCode;
							changes.Add ("Pays: " + toChange.NewValue.Address.CountryCode + " -> " + reportedPersonEntityToUpdate.Address.Country);
						}




						if (reportedPersonEntityToUpdate.Adult1.IsNotNull ())
						{
							var aiderPersonEntity = EChDataUpdater.GetAiderPersonEntity (businessContext, reportedPersonEntityToUpdate.Adult1);


							//Update AiderHouseholdEntity
							EChDataUpdater.UpdateAiderHouseholdAndSubsription (
								  businessContext,
								  reportedPersonEntityToUpdate,
								  aiderPersonEntity);

							//Keep a copy of ParishGroupPath
							var oldParishGroupPath = String.Copy (aiderPersonEntity.ParishGroupPathCache);

							//Reasign to parish
							ParishAssigner.ReassignToParish (parishAddressRepository, businessContext, aiderPersonEntity);

							//Check if we need warn parish for arrival/departure
							if (!oldParishGroupPath.Equals (aiderPersonEntity.ParishGroupPathCache))
							{
								//Create the first warning with the old ParishGroupPath
								AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, oldParishGroupPath, WarningType.ParishDeparture, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));


								AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.ParishArrival, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
							}
							else //if no change in parish group path, we create an simple address change warning
							{
								AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChAddressChanged, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
							}

						}

						if (reportedPersonEntityToUpdate.Adult2.IsNotNull ())
						{
							var aiderPersonEntity = EChDataUpdater.GetAiderPersonEntity (businessContext, reportedPersonEntityToUpdate.Adult2);
							AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChAddressChanged, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));

							//Keep a copy of ParishGroupPath
							var oldParishGroupPath = String.Copy (aiderPersonEntity.ParishGroupPathCache);

							//Reasign to parish
							ParishAssigner.ReassignToParish (parishAddressRepository, businessContext, aiderPersonEntity);

							//Check if we need warn parish for arrival/departure
							if (!oldParishGroupPath.Equals (aiderPersonEntity.ParishGroupPathCache))
							{

								//Create the first warning with the old ParishGroupPath
								AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, oldParishGroupPath, WarningType.ParishDeparture, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));


								AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.ParishArrival, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
							}
							else //if no change in parish group path, we create an simple address change warning
							{
								AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChAddressChanged, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
							}
						}

						foreach (var child in reportedPersonEntityToUpdate.Children)
						{
							var aiderPersonEntity = EChDataUpdater.GetAiderPersonEntity (businessContext, child);

							//Keep a copy of ParishGroupPath
							var oldParishGroupPath = String.Copy (aiderPersonEntity.ParishGroupPathCache);

							//Reasign to parish
							ParishAssigner.ReassignToParish (parishAddressRepository, businessContext, aiderPersonEntity);

							//Check if we need warn parish for arrival/departure
							if (!oldParishGroupPath.Equals (aiderPersonEntity.ParishGroupPathCache))
							{

								//Create the first warning with the old ParishGroupPath
								AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, oldParishGroupPath, WarningType.ParishDeparture, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));


								AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.ParishArrival, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
							}
							else //if no change in parish group path, we create an simple address change warning
							{
								AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChAddressChanged, EChDataUpdater.WarningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
							}
						}

					}
					catch (Exception)
					{
						Console.WriteLine ("Error: EChReportedPerson (FAMILYKEY:{0}) throw exception", toChange.NewValue.FamilyKey);
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
				return true;
			}
		}

		private static void RefreshAiderSubscription(BusinessContext businessContext, AiderHouseholdEntity household)
		{
			var subscriptionExample = new AiderSubscriptionEntity()
			{
				Household = household
			};

			var subscription = businessContext.DataContext.GetByExample<AiderSubscriptionEntity>(subscriptionExample).FirstOrDefault();
			if (subscription.IsNotNull())
			{
				subscription.RefreshCache();
			}
		}

		private static void UpdateAiderHouseholdAndSubsription(BusinessContext businessContext, eCH_ReportedPersonEntity reportedPersonEntityToUpdate, AiderPersonEntity aiderPersonEntity)
		{
			var aiderHousehold = EChDataUpdater.GetAiderHousehold (businessContext, aiderPersonEntity);
			if (aiderHousehold != null)
			{
				var houseNumber = StringUtils.ParseNullableInt (SwissPostStreet.StripHouseNumber (reportedPersonEntityToUpdate.Address.HouseNumber));
				var houseNumberComplement = SwissPostStreet.GetHouseNumberComplement (reportedPersonEntityToUpdate.Address.HouseNumber);

				if (string.IsNullOrWhiteSpace (houseNumberComplement))
				{
					houseNumberComplement = null;
				}
				var aiderAddressEntity = aiderHousehold.Address;
				aiderAddressEntity.AddressLine1 = reportedPersonEntityToUpdate.Address.AddressLine1;
				aiderAddressEntity.Street = reportedPersonEntityToUpdate.Address.Street;
				aiderAddressEntity.HouseNumber = houseNumber;
				aiderAddressEntity.HouseNumberComplement = houseNumberComplement;
				aiderAddressEntity.Town = EChDataUpdater.GetAiderTownEntity (businessContext, reportedPersonEntityToUpdate.Address.SwissZipCodeId);

				EChDataUpdater.RefreshAiderSubscription (businessContext, aiderHousehold);
			}
			else
			{
				AiderPersonWarningEntity.Create (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.Mismatch, EChDataUpdater.WarningTitleMessage, FormattedText.FromSimpleText (aiderPersonEntity.GetDisplayName () + " n'a pas de ménage dans Aider"));
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

			return businessContext.DataContext.GetByExample<eCH_PersonEntity> (personExample).FirstOrDefault ();
		}

		private static AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, eCH_PersonEntity person)
		{
			if (person == null)
			{
				return null;
			}

			var personExample = new AiderPersonEntity ();

			personExample.eCH_Person = new eCH_PersonEntity ()
			{
				PersonId = person.PersonId
			};

			return businessContext.DataContext.GetByExample<AiderPersonEntity> (personExample).FirstOrDefault ();
		}

		private static eCH_ReportedPersonEntity GetEchReportedPersonEntity(BusinessContext businessContext, EChReportedPerson reportedPerson)
		{
			var reportedPersonExample = new eCH_ReportedPersonEntity ();
			var req = new Request ();
			if (reportedPerson.Adult1 != null && reportedPerson.Adult2 != null)
			{
				reportedPersonExample.Adult1 = new eCH_PersonEntity ()
				{
					PersonId = reportedPerson.Adult1.Id
				};
				reportedPersonExample.Adult2 = new eCH_PersonEntity ()
				{
					PersonId = reportedPerson.Adult2.Id
				};

			}

			if (reportedPerson.Adult1 != null && reportedPerson.Adult2 == null)
			{
				reportedPersonExample.Adult1 = new eCH_PersonEntity ()
				{
					PersonId = reportedPerson.Adult1.Id
				};
			}

			return businessContext.DataContext.GetByExample<eCH_ReportedPersonEntity> (reportedPersonExample).FirstOrDefault ();
		}

		private static eCH_AddressEntity GetEchAddressEntity(BusinessContext businessContext, EChAddress address)
		{
			var addressExample = new eCH_AddressEntity ()
			{
				SwissZipCode = address.SwissZipCode,
				SwissZipCodeAddOn = address.SwissZipCodeAddOn,
				Street = address.Street,
				HouseNumber = address.HouseNumber
			};

			return businessContext.DataContext.GetByExample<eCH_AddressEntity> (addressExample).FirstOrDefault ();
		}

		private static AiderHouseholdEntity GetAiderHousehold(BusinessContext businessContext, AiderPersonEntity refPerson)
		{
			var contactExample = new AiderContactEntity();
			var householdExample = new AiderHouseholdEntity();
			contactExample.Person = refPerson;
			contactExample.Household = householdExample;
			var request = new Request()
			{
				RootEntity = contactExample,
				RequestedEntity = householdExample
			};

			return businessContext.DataContext.GetByRequest<AiderHouseholdEntity>(request).FirstOrDefault();
		}

		private static AiderTownEntity GetAiderTownEntity(BusinessContext businessContext, EChAddress address)
		{
			var townExample = new AiderTownEntity ()
			{
				SwissZipCodeId = address.SwissZipCodeId
			};

			return businessContext.DataContext.GetByExample<AiderTownEntity> (townExample).FirstOrDefault ();
		}

		private static AiderTownEntity GetAiderTownEntity(BusinessContext businessContext, int swissZipCodeId)
		{
			var townExample = new AiderTownEntity()
			{
				SwissZipCodeId = swissZipCodeId
			};

			return businessContext.DataContext.GetByExample<AiderTownEntity>(townExample).FirstOrDefault();
		}

		private static Dictionary<EntityKey, AiderPersonEntity> AiderPersonEntitiesTagedForDeletion;
		private static Dictionary<EntityKey, AiderHouseholdEntity> AiderPersonEntitiesWithDeletedHousehold;
		private static FormattedText WarningTitleMessage = FormattedText.FromSimpleText ("Mise à jour ECh du " + Date.Today.Day + " " + Date.Today.Month + " " + Date.Today.Year);


		private readonly List<EChPerson> personsToCreate;
		private readonly List<EChPerson> personsToRemove;
		private readonly List<EChPerson> personsToUpdate;
	}
}
