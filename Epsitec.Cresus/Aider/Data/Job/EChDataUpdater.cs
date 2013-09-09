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

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
	internal class EChDataUpdater
	{
		public EChDataUpdater(string oldEchFile, string newEchFile, string reportFile, CoreData coreData, ParishAddressRepository parishAddressRepository)
		{
			this.coreData = coreData;
			this.parishAddressRepository = parishAddressRepository;

			this.jobDateTime = System.DateTime.Now;
			this.jobName     = "EChDataUpdate";
			this.jobDescription = string.Format ("Importation des données eCH.\nBase {0}\nInc. {1}", oldEchFile, newEchFile);

			this.warningTitleMessage = TextFormatter.FormatText ("Mise à jour ECh ", this.jobDateTime.ToShortDateString ());


			this.aiderPersonEntitiesTaggedForDeletion = new Dictionary<EntityKey, AiderPersonEntity> ();
			this.aiderPersonEntitiesWithDeletedHousehold = new Dictionary<EntityKey, AiderHouseholdEntity> ();

			if (System.IO.File.Exists (oldEchFile) && System.IO.File.Exists (newEchFile))
			{
				var time1 = this.LogToConsole ("generating diff");
				
				using (var analyzer = new EChDataAnalyzerReporter (oldEchFile, newEchFile, reportFile))
				{
					var time2 = this.LogToConsole ("starting analysis");

					this.personsToCreate = analyzer.GetPersonsToAdd ().ToList ();
					this.personsToRemove = analyzer.GetPersonsToRemove ().ToList ();
					this.personsToUpdate = analyzer.GetPersonsToChange ().ToList ();

					this.houseHoldsToCreate = analyzer.GetFamiliesToAdd ().ToList ();
					this.houseHoldsToRemove = analyzer.GetFamiliesToRemove ().ToList ();
					this.houseHoldsToUpdate	= analyzer.GetFamiliesToChange ().ToList ();

					this.newHouseHoldsToCreate     = analyzer.GetNewFamilies ().ToList ();
					this.missingHouseHoldsToRemove = analyzer.GetMissingFamilies ().ToList ();

					this.PrepareHashSetForAnalytics ();

					this.LogToConsole (time2, "analysis done");
				}

				this.LogToConsole (time1, "diff done, ready");
			}
			else
			{
				throw new System.Exception ("Failed to load ECh files");
			}
		}


		public void StartJob()
		{
			//	Update the EChPerson entities and add all required DataChangedECh warnings on
			//	the corresponding AiderPersons.

			var time = this.LogToConsole ("starting main job");

			this.UpdateEChPersonEntities ();

			this.UpdateHouseholdsAndPropagate ();

			this.TagEChPersonsForDeletion ();
			this.TagAiderPersonsForDeletion ();
			this.TagAiderPersonsForMissingHousehold ();

			this.CreateNewEChPersons ();
			this.CreateNewAiderPersons ();

			this.RemoveOldEChReportedPersons ();
			this.CreateNewEChReportedPersons ();
			this.CreateNewAiderHouseholds ();

			this.LogToConsole (time, "done");
		}

		
		private void ExecuteWithBusinessContext(System.Action<BusinessContext> action)
		{
			var stackTrace    = new System.Diagnostics.StackTrace ();
			var stackFrames   = stackTrace.GetFrames ();
			var callingMethod = stackFrames[0].GetMethod ();

			var callerName = callingMethod.Name;

			var time = this.LogToConsole ("{0}, start job", callerName);

			using (var businessContext = new BusinessContext (this.coreData, false))
			{
				this.EnsureWarningSource (businessContext);

				action (businessContext);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
			}
			
			this.LogToConsole (time, "{0}, done", callerName);
		}

		private void EnsureWarningSource(BusinessContext businessContext)
		{
			if (this.warningSource == null)
			{
				this.warningSource = AiderWarningSourceEntity.Create<AiderPersonWarningEntity> (
										businessContext, this.jobDateTime, this.jobName, TextFormatter.FormatText (this.jobDescription));
			}
			else
			{
				var example = new AiderWarningSourceEntity
				{
					CreationDate = this.jobDateTime,
					Name = this.jobName,
				};

				this.warningSource = businessContext.DataContext.GetSingleByExample (example);
			}
		}

		private System.Diagnostics.Stopwatch LogToConsole(string format, params object[] args)
		{
			var message = string.Format (format, args);

			if (message.StartsWith ("Error"))
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
			}

			System.Console.WriteLine ("EChDataUpdater: {0}", message);
			System.Console.ResetColor ();

			var time = new System.Diagnostics.Stopwatch ();

			time.Start ();

			return time;
		}

		private void LogToConsole(System.Diagnostics.Stopwatch time, string format, params object[] args)
		{
			var message = string.Format (format, args);
			System.Console.WriteLine ("EChDataUpdater: {0} - {1} ms", message, time.ElapsedMilliseconds);
		}


		private void CreateNewAiderPersons()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChPerson in this.personsToCreate)
					{
						var eChPersonEntity = this.GetEchPersonEntity (businessContext, eChPerson);
						var existingAiderPersonEntity = this.GetAiderPersonEntity (businessContext, eChPersonEntity);

						if (existingAiderPersonEntity.IsNull ())
						{
							var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();

							aiderPersonEntity.eCH_Person = eChPersonEntity;
							aiderPersonEntity.MrMrs = EChDataImporter.GuessMrMrs (eChPerson.Sex, eChPerson.DateOfBirth, eChPerson.MaritalStatus);
							aiderPersonEntity.Confession = PersonConfession.Protestant;

							ParishAssigner.AssignToParish (parishAddressRepository, businessContext, aiderPersonEntity);

							//if this person come from new household
							if (this.eChPersonIdWithNewHousehold.Contains (eChPerson.Id))
							{
								this.CreateWarning (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChProcessArrival, this.warningTitleMessage, FormattedText.FromSimpleText (aiderPersonEntity.GetDisplayName () + " est arrivée avec sa famille dans le registre ECh!"));
							}
							else
							{
                                this.eChPersonIdWithNewPerson.Add(aiderPersonEntity.eCH_Person.PersonId);
                            }
						}
						else
						{
							this.CreateWarning (businessContext, existingAiderPersonEntity, existingAiderPersonEntity.ParishGroupPathCache, WarningType.EChPersonDuplicated, this.warningTitleMessage, FormattedText.FromSimpleText (existingAiderPersonEntity.GetDisplayName () + " existe déjà dans Aider"));
						}

					}
				});
		}

		private void CreateWarning(BusinessContext context, AiderPersonEntity person, string parishGroupPath,
								   WarningType warningType, FormattedText title, FormattedText description)
		{
			AiderPersonWarningEntity.Create (context, person, parishGroupPath, warningType, title, description, this.warningSource);
		}

		private void CreateNewEChPersons()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChPerson in this.personsToCreate)
					{
						var existingPersonEntity = this.GetEchPersonEntity (businessContext, eChPerson);

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
				});
		}

		private void TagEChPersonsForDeletion()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChPerson in this.personsToRemove)
					{
						var existingPersonEntity = this.GetEchPersonEntity (businessContext, eChPerson);

						if (existingPersonEntity != null)
						{
							existingPersonEntity.RemovalReason = RemovalReason.Departed;
						}
					}
				});
		}

		private void TagAiderPersonsForDeletion()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChPerson in this.personsToRemove)
					{
						var existingPersonEntity = this.GetEchPersonEntity (businessContext, eChPerson);
						var existingAiderPersonEntity = this.GetAiderPersonEntity (businessContext, existingPersonEntity);

						if (existingAiderPersonEntity.IsNotNull())
						{
							this.CreateWarning (businessContext, existingAiderPersonEntity, existingAiderPersonEntity.ParishGroupPathCache, WarningType.EChPersonMissing, this.warningTitleMessage, FormattedText.FromSimpleText (existingAiderPersonEntity.GetDisplayName () + " n'est plus dans le registre ECh!"));

							var key = businessContext.DataContext.GetNormalizedEntityKey (existingAiderPersonEntity).Value;
							this.aiderPersonEntitiesTaggedForDeletion.Add (key, existingAiderPersonEntity);
						}

					}
				});
		}

		private void CreateNewEChReportedPersons()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChReportedPerson in this.houseHoldsToCreate)
					{
						//	Create eCH ReportedPerson entity, with the new eCH address, and assign the
						//	two adults (if any) and the children found in the eCH data:

						var eChAddressEntity          = EChDataImporter.ImportEchAddressEntity (businessContext, eChReportedPerson.Address);
						var eChReportedPersonEntity   = businessContext.CreateAndRegisterEntity<eCH_ReportedPersonEntity> ();

						eChReportedPersonEntity.Address = eChAddressEntity;
						var eChPersonA1 = this.GetEchPersonEntity (businessContext, eChReportedPerson.Adult1);
						eChPersonA1.ReportedPerson1 = eChReportedPersonEntity;
						eChReportedPersonEntity.Adult1 = eChPersonA1;

                        //create aiderPerson if needed (weird case)
                        var aiderPersonA1 = this.GetAiderPersonEntity(businessContext, eChPersonA1);
                        if (aiderPersonA1.IsNull())
                        {
                            var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity>();
                            aiderPersonEntity.eCH_Person = eChPersonA1;
                        }

                        //autoassign person to AiderHousehold if needed
                        if (aiderPersonA1.Households.IsEmpty())
                        {
                            var aiderHousehold = this.GetAiderHousehold(businessContext, eChPersonA1);
                            if (aiderHousehold.IsNotNull())
                            {
                                EChDataImporter.SetupHousehold(businessContext, aiderPersonA1, aiderHousehold, eChReportedPersonEntity, isHead1: true);
                                this.eChPersonIdWithHouseholdSetupDone.Add (aiderPersonA1.eCH_Person.PersonId);
                                ParishAssigner.AssignToParish(parishAddressRepository, businessContext, aiderPersonA1);
                            }
                        }
                        if (this.eChPersonIdWithNewPerson.Contains(aiderPersonA1.eCH_Person.PersonId))
                        {
                            this.CreateWarning(businessContext, aiderPersonA1, aiderPersonA1.ParishGroupPathCache, WarningType.EChPersonNew, this.warningTitleMessage, FormattedText.FromSimpleText(aiderPersonA1.GetDisplayName() + " est arrivée dans le registre ECh!"));
                        }

						if (eChReportedPerson.Adult2 != null)
						{
							var eChPersonA2 = this.GetEchPersonEntity (businessContext, eChReportedPerson.Adult2);
							eChPersonA2.ReportedPerson1 = eChReportedPersonEntity;
							eChReportedPersonEntity.Adult2 = eChPersonA2;

                            //autoassign person to AiderHousehold if needed
                            var aiderPersonA2 = this.GetAiderPersonEntity(businessContext, eChPersonA2);
                            if (aiderPersonA2.IsNull())
                            {
                                var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity>();
                                aiderPersonEntity.eCH_Person = eChPersonA2;
                            }
                            if (aiderPersonA2.Households.IsEmpty())
                            {
                                var aiderHousehold = this.GetAiderHousehold(businessContext, eChPersonA1);
                                if (aiderHousehold.IsNotNull())
                                {
                                    EChDataImporter.SetupHousehold(businessContext, aiderPersonA2, aiderHousehold, eChReportedPersonEntity, isHead2: true);
                                    this.eChPersonIdWithHouseholdSetupDone.Add(aiderPersonA2.eCH_Person.PersonId);
                                    ParishAssigner.AssignToParish(parishAddressRepository, businessContext, aiderPersonA2);
                                }
                            }
                            if (this.eChPersonIdWithNewPerson.Contains(aiderPersonA2.eCH_Person.PersonId))
                            {
                                this.CreateWarning(businessContext, aiderPersonA2, aiderPersonA2.ParishGroupPathCache, WarningType.EChPersonNew, this.warningTitleMessage, FormattedText.FromSimpleText(aiderPersonA2.GetDisplayName() + " est arrivée dans le registre ECh!"));
                            }
						}

						foreach (var eChChild in eChReportedPerson.Children)
						{
							var eChPersonC = this.GetEchPersonEntity (businessContext, eChChild);
							eChPersonC.ReportedPerson1 = eChReportedPersonEntity;
							eChReportedPersonEntity.Children.Add (eChPersonC);

                            //autoassign person to AiderHousehold if needed
                            var aiderPersonC = this.GetAiderPersonEntity(businessContext, eChPersonC);
                            if (aiderPersonC.Households.IsEmpty())
                            {
                                var aiderHousehold = this.GetAiderHousehold(businessContext,eChPersonA1);
                                if (aiderPersonC.IsNull())
                                {
                                    var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity>();
                                    aiderPersonEntity.eCH_Person = eChPersonC;
                                }
                                if(aiderHousehold.IsNotNull())
                                {
                                    EChDataImporter.SetupHousehold(businessContext, aiderPersonC, aiderHousehold, eChReportedPersonEntity, isChild: true);
                                    this.eChPersonIdWithHouseholdSetupDone.Add(aiderPersonC.eCH_Person.PersonId);
                                    ParishAssigner.AssignToParish(parishAddressRepository, businessContext, aiderPersonC);
                                }
                                if (this.eChPersonIdWithNewPerson.Contains(aiderPersonC.eCH_Person.PersonId))
                                {
                                    this.CreateWarning(businessContext, aiderPersonC, aiderPersonC.ParishGroupPathCache, WarningType.EChPersonNew, this.warningTitleMessage, FormattedText.FromSimpleText(aiderPersonC.GetDisplayName() + " est arrivée dans le registre ECh!"));
                                }
                            }
						}

						//	Create warnings for the AiderPerson entity:

						var referenceAdult    = eChReportedPersonEntity.Adult1;
						if (aiderPersonA1.IsNotNull ())
						{
							AiderHouseholdEntity oldHousehold;

							var key = businessContext.DataContext.GetNormalizedEntityKey (aiderPersonA1).Value;

							if (this.aiderPersonEntitiesWithDeletedHousehold.TryGetValue (key, out oldHousehold))
							{
								var warningMessage = FormattedText.FromSimpleText ("Un changement de composition a eu lieu dans ce ménage.");

								this.CreateWarning (businessContext, aiderPersonA1, aiderPersonA1.ParishGroupPathCache, WarningType.EChHouseholdChanged, this.warningTitleMessage, warningMessage);
							}
							else
							{
								var warningMessage = FormattedText.FromSimpleText ("Cette personne a maintenant son propre ménage");

								this.CreateWarning (businessContext, aiderPersonA1, aiderPersonA1.ParishGroupPathCache, WarningType.EChHouseholdAdded, this.warningTitleMessage, warningMessage);
							}
						}
						else
						{
							this.LogToConsole ("Error: Adult with PersonId={0} has no matching AiderPerson", referenceAdult.PersonId);
						}
					}
				});
		}

		private void CreateNewAiderHouseholds()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChReportedPerson in this.newHouseHoldsToCreate)
					{
						var aiderHousehold = businessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
						aiderHousehold.HouseholdMrMrs = HouseholdMrMrs.Auto;

						var aiderAddressEntity = aiderHousehold.Address;
						var eChAddressEntity = this.GetEchAddressEntity (businessContext, eChReportedPerson.Address);


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
						aiderAddressEntity.Town = this.GetAiderTownEntity (businessContext, eChReportedPerson.Address);


						//Link household to ECh Entity
						var eChReportedPersonEntity = this.GetEchReportedPersonEntity (businessContext, eChReportedPerson);
						if (eChReportedPersonEntity.Adult1.IsNotNull ())
						{
							var aiderPerson = this.GetAiderPersonEntity (businessContext, eChReportedPersonEntity.Adult1);

							//remove EChHouseholdAdded warning added previously (if exist)
							foreach (var warning in aiderPerson.Warnings)
							{
								if (warning.WarningType.Equals (WarningType.EChHouseholdAdded) && warning.Title.Equals (this.warningTitleMessage))
								{
									businessContext.DeleteEntity (warning);
								}
							}
                            if (!this.eChPersonIdWithHouseholdSetupDone.Contains(aiderPerson.eCH_Person.PersonId))
                            {
                                EChDataImporter.SetupHousehold(businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isHead1: true);
                            }
							
						}

						if (eChReportedPersonEntity.Adult2.IsNotNull ())
						{
							var aiderPerson = this.GetAiderPersonEntity (businessContext, eChReportedPersonEntity.Adult2);

							//remove EChHouseholdAdded warning added previously (if exist)
							foreach (var warning in aiderPerson.Warnings)
							{
								if (warning.WarningType.Equals (WarningType.EChHouseholdAdded) && warning.Title.Equals (this.warningTitleMessage))
								{
									businessContext.DeleteEntity (warning);
								}
							}

                            if (!this.eChPersonIdWithHouseholdSetupDone.Contains(aiderPerson.eCH_Person.PersonId))
                            {
                                EChDataImporter.SetupHousehold(businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isHead2: true);
                            }
							
						}

						foreach (var child in eChReportedPersonEntity.Children)
						{
							var aiderPerson = this.GetAiderPersonEntity (businessContext, child);

							//remove EChHouseholdAdded warning added previously (if exist)
							foreach (var warning in aiderPerson.Warnings)
							{
								if (warning.WarningType.Equals (WarningType.EChHouseholdAdded) && warning.Title.Equals (this.warningTitleMessage))
								{
									businessContext.DeleteEntity (warning);
								}
							}
                            if (!this.eChPersonIdWithHouseholdSetupDone.Contains(aiderPerson.eCH_Person.PersonId))
                            {
                                EChDataImporter.SetupHousehold(businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isChild: true);
                            }
							
						}
					}
				});
		}

		private void RemoveOldEChReportedPersons()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChReportedPerson in this.houseHoldsToRemove)
					{
						var eChReportedPersonEntity = this.GetEchReportedPersonEntity (businessContext, eChReportedPerson);

						//	Unlink ECh Person Entity

						var eChPersonEntityAdult1 = this.GetEchPersonEntity (businessContext, eChReportedPerson.Adult1);
						var aiderPersonEntity     = this.GetAiderPersonEntity (businessContext, eChPersonEntityAdult1);

						if (aiderPersonEntity.IsNotNull ())
						{
							var personKey      = businessContext.DataContext.GetNormalizedEntityKey (aiderPersonEntity).Value;
							var aiderHousehold = this.GetAiderHousehold (businessContext, aiderPersonEntity);

							if (aiderHousehold.IsNotNull ())
							{
								this.aiderPersonEntitiesWithDeletedHousehold.Add (personKey, aiderHousehold);
							}
							else
							{
								this.LogToConsole ("Error: AiderPerson {0} ({1}) has no associated AiderHousehold", aiderPersonEntity.DisplayName, personKey);
							}
						}

						eChPersonEntityAdult1.ReportedPerson1 = null;

						if (eChReportedPerson.Adult2 != null)
						{
							var eChPersonEntityAdult2 = this.GetEchPersonEntity (businessContext, eChReportedPerson.Adult2);
							eChPersonEntityAdult2.ReportedPerson2 = null;
						}

						businessContext.DeleteEntity (eChReportedPersonEntity.Address);
						businessContext.DeleteEntity (eChReportedPersonEntity);
					}
				});
		}

		private void TagAiderPersonsForMissingHousehold()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChReportedPerson in this.missingHouseHoldsToRemove)
					{
						foreach (var eChPerson in eChReportedPerson.GetMembers ())
						{
							var eChPersonEntity   = this.GetEchPersonEntity (businessContext, eChPerson);
							var aiderPersonEntity = this.GetAiderPersonEntity (businessContext, eChPersonEntity);

							var personKey = businessContext.DataContext.GetNormalizedEntityKey (aiderPersonEntity).Value;

							if (this.aiderPersonEntitiesTaggedForDeletion.ContainsKey (personKey))
							{
								foreach (var warn in aiderPersonEntity.Warnings)
								{
									if (warn.WarningType.Equals (WarningType.EChPersonMissing) && this.warningTitleMessage.Equals (warn.Title))
									{
										warn.WarningType = WarningType.EChProcessDeparture;
									}
								}
							}
							else
							{
								this.CreateWarning (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChHouseholdMissing, this.warningTitleMessage, FormattedText.FromSimpleText (aiderPersonEntity.GetDisplayName () + " n'est plus assigné a une famille dans le registre ECh!"));
							}

						}
					}
				});
		}

		private bool UpdateEChPersonEntities()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var toChange in this.personsToUpdate)
					{
						try
						{
							var personEntityToUpdate = this.GetEchPersonEntity (businessContext, toChange.OldValue);
							var aiderPersonEntity = this.GetAiderPersonEntity (businessContext, personEntityToUpdate);
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
							}

							if (!toChange.OldValue.DateOfBirth.Equals (toChange.NewValue.DateOfBirth))
							{
								personEntityToUpdate.PersonDateOfBirth = changedEChPersonEntity.PersonDateOfBirth;
								changes.Add ("Date de naissance: " + toChange.NewValue.DateOfBirth + " -> " + changedEChPersonEntity.PersonDateOfBirth);
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
							}

							if (!toChange.OldValue.NationalityStatus.Equals (toChange.NewValue.NationalityStatus))
							{
								personEntityToUpdate.NationalityStatus = changedEChPersonEntity.NationalityStatus;
								changes.Add ("Statut nationalité: " + toChange.NewValue.NationalityStatus + " -> " + changedEChPersonEntity.NationalityStatus);
							}

							var oldOriginList = toChange.OldValue.OriginPlaces.Select(p => System.Tuple.Create(p.Canton, p.Name)).ToList();
							var newOriginList = toChange.NewValue.OriginPlaces.Select(p => System.Tuple.Create(p.Canton, p.Name)).ToList();

							if (!oldOriginList.SetEquals(newOriginList))
							{
								personEntityToUpdate.Origins = changedEChPersonEntity.Origins;
								changes.Add ("Origines: " + string.Join (" ", toChange.NewValue.OriginPlaces.Select (o => o.Display ()).ToList ()) + " -> " + string.Join (" ", changedEChPersonEntity.Origins));
							}

							if (!toChange.OldValue.Sex.Equals (toChange.NewValue.Sex))
							{
								personEntityToUpdate.PersonSex = changedEChPersonEntity.PersonSex;
								changes.Add ("Sex: " + toChange.NewValue.Sex + " -> " + changedEChPersonEntity.PersonSex);
								mustWarn = true;
							}

							if (mustWarn)
							{
								this.CreateWarning (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChPersonDataChanged, this.warningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
							}
						}
						catch (System.Exception)
						{
							this.LogToConsole ("Error: EChPerson {0} {1} throw exception",toChange.NewValue.OfficialName,toChange.NewValue.FirstNames);
						}
					}
				});
			
			return true;
		}

		private bool UpdateHouseholdsAndPropagate()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var toChange in this.houseHoldsToUpdate)
					{
						try
						{
							var reportedPersonEntityToUpdate = this.GetEchReportedPersonEntity (businessContext, toChange.OldValue);
							var changes = new List<string> ();
							changes.Add ("Changement dans l'adresse:");
							if (!string.IsNullOrEmpty (toChange.OldValue.Address.AddressLine1))
							{
								if (!toChange.OldValue.Address.AddressLine1.Equals (toChange.NewValue.Address.AddressLine1))
								{
									reportedPersonEntityToUpdate.Address.AddressLine1 = toChange.OldValue.Address.AddressLine1;
									changes.Add ("Ligne adresse: " + toChange.NewValue.Address.AddressLine1 + " -> " + reportedPersonEntityToUpdate.Address.AddressLine1);
								}
							}

							if (!string.IsNullOrEmpty (toChange.OldValue.Address.HouseNumber))
							{
								if (!toChange.OldValue.Address.HouseNumber.Equals (toChange.NewValue.Address.HouseNumber))
								{
									reportedPersonEntityToUpdate.Address.HouseNumber = toChange.OldValue.Address.HouseNumber;
									changes.Add ("N° de maison: " + toChange.NewValue.Address.HouseNumber + " -> " + reportedPersonEntityToUpdate.Address.HouseNumber);
								}
							}
							if (!string.IsNullOrEmpty (toChange.OldValue.Address.Street))
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
								var aiderPersonEntity = this.GetAiderPersonEntity (businessContext, reportedPersonEntityToUpdate.Adult1);


								//Update AiderHouseholdEntity
								this.UpdateAiderHouseholdAndSubscription (
									  businessContext,
									  reportedPersonEntityToUpdate,
									  aiderPersonEntity);

								this.ReassignAndWarnParish (businessContext, aiderPersonEntity, changes);
							}

							if (reportedPersonEntityToUpdate.Adult2.IsNotNull ())
							{
								var aiderPersonEntity = this.GetAiderPersonEntity (businessContext, reportedPersonEntityToUpdate.Adult2);

								this.ReassignAndWarnParish (businessContext, aiderPersonEntity, changes);
							}

							foreach (var child in reportedPersonEntityToUpdate.Children)
							{
								var aiderPersonEntity = this.GetAiderPersonEntity (businessContext, child);

								this.ReassignAndWarnParish (businessContext, aiderPersonEntity, changes);
							}

						}
						catch (System.Exception)
						{
							this.LogToConsole ("Error: EChReportedPerson (FAMILYKEY:{0}) throw exception", toChange.NewValue.FamilyKey);
						}
					}
				});
			
			return true;
		}


		private void ReassignAndWarnParish(BusinessContext businessContext, AiderPersonEntity aiderPersonEntity, List<string> changes)
		{
			var oldParishGroupPath = aiderPersonEntity.ParishGroupPathCache;

			ParishAssigner.ReassignToParish (this.parishAddressRepository, businessContext, aiderPersonEntity);

			var newParishGroupPath = aiderPersonEntity.ParishGroupPathCache;

			if (oldParishGroupPath != newParishGroupPath)
			{
				this.CreateWarning (businessContext, aiderPersonEntity, oldParishGroupPath, WarningType.ParishDeparture, this.warningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
				this.CreateWarning (businessContext, aiderPersonEntity, newParishGroupPath, WarningType.ParishArrival, this.warningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
			}
			else //if no change in parish group path, we create an simple address change warning
			{
				this.CreateWarning (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChAddressChanged, this.warningTitleMessage, TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
			}
		}

		private void PrepareHashSetForAnalytics()
		{
            this.eChPersonIdWithNewPerson = new HashSet<string> ();
			this.eChPersonIdWithNewHousehold = new HashSet<string> ();
            this.eChPersonIdWithHouseholdSetupDone = new HashSet<string> ();

			foreach (var eChReportedPerson in this.newHouseHoldsToCreate)
			{
				this.eChPersonIdWithNewHousehold.Add (eChReportedPerson.Adult1.Id);
				if (eChReportedPerson.Adult2 != null)
				{
					this.eChPersonIdWithNewHousehold.Add (eChReportedPerson.Adult2.Id);
				}
				foreach (var child in eChReportedPerson.Children)
				{
					this.eChPersonIdWithNewHousehold.Add (child.Id);
				}
			}
		}

		private void RefreshAiderSubscription(BusinessContext businessContext, AiderHouseholdEntity household)
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

		private void UpdateAiderHouseholdAndSubscription(BusinessContext businessContext, eCH_ReportedPersonEntity reportedPersonEntityToUpdate, AiderPersonEntity aiderPersonEntity)
		{
			var aiderHousehold = this.GetAiderHousehold (businessContext, aiderPersonEntity);
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
				aiderAddressEntity.Town = this.GetAiderTownEntity (businessContext, reportedPersonEntityToUpdate.Address.SwissZipCodeId);

				this.RefreshAiderSubscription (businessContext, aiderHousehold);
			}
			else
			{
				this.CreateWarning (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.Mismatch, this.warningTitleMessage, FormattedText.FromSimpleText (aiderPersonEntity.GetDisplayName () + " n'a pas de ménage dans Aider"));
			}


		}

		private eCH_PersonEntity GetEchPersonEntity(BusinessContext businessContext, EChPerson person)
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

		private AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, eCH_PersonEntity person)
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

		private eCH_ReportedPersonEntity GetEchReportedPersonEntity(BusinessContext businessContext, EChReportedPerson reportedPerson)
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

		private eCH_AddressEntity GetEchAddressEntity(BusinessContext businessContext, EChAddress address)
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

        private AiderHouseholdEntity GetAiderHousehold(BusinessContext businessContext, eCH_PersonEntity refPerson)
        {
            var personExample = new AiderPersonEntity();
            var contactExample = new AiderContactEntity();
            var householdExample = new AiderHouseholdEntity();
            personExample.eCH_Person = refPerson;
            contactExample.Person = personExample;
            contactExample.Household = householdExample;
            var request = new Request()
            {
                RootEntity = contactExample,
                RequestedEntity = householdExample
            };

            return businessContext.DataContext.GetByRequest<AiderHouseholdEntity>(request).FirstOrDefault();
        }

		private AiderHouseholdEntity GetAiderHousehold(BusinessContext businessContext, AiderPersonEntity refPerson)
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

		private AiderTownEntity GetAiderTownEntity(BusinessContext businessContext, EChAddress address)
		{
			var townExample = new AiderTownEntity ()
			{
				SwissZipCodeId = address.SwissZipCodeId
			};

			return businessContext.DataContext.GetByExample<AiderTownEntity> (townExample).FirstOrDefault ();
		}

		private AiderTownEntity GetAiderTownEntity(BusinessContext businessContext, int swissZipCodeId)
		{
			var townExample = new AiderTownEntity()
			{
				SwissZipCodeId = swissZipCodeId
			};

			return businessContext.DataContext.GetByExample<AiderTownEntity>(townExample).FirstOrDefault();
		}


		private CoreData						coreData;
		private ParishAddressRepository			parishAddressRepository;
		
		private readonly Dictionary<EntityKey, AiderPersonEntity>	 aiderPersonEntitiesTaggedForDeletion;
		private readonly Dictionary<EntityKey, AiderHouseholdEntity> aiderPersonEntitiesWithDeletedHousehold;
		private HashSet<string>					eChPersonIdWithNewHousehold;
        private HashSet<string>                 eChPersonIdWithNewPerson;
        private HashSet<string>                 eChPersonIdWithHouseholdSetupDone;

		private readonly List<EChPerson>		 personsToCreate;
		private readonly List<EChPerson>		 personsToRemove;
		private readonly List<Change<EChPerson>> personsToUpdate;

		private readonly List<EChReportedPerson>		  houseHoldsToCreate;
		private readonly List<EChReportedPerson>		  houseHoldsToRemove;
		private readonly List<Change<EChReportedPerson>>  houseHoldsToUpdate;

		private readonly List<EChReportedPerson> newHouseHoldsToCreate;
		private readonly List<EChReportedPerson> missingHouseHoldsToRemove;

		private System.DateTime					jobDateTime;
		private string							jobName;
		private string							jobDescription;
		
		private AiderWarningSourceEntity		warningSource;
		private FormattedText					warningTitleMessage;
	}
}
