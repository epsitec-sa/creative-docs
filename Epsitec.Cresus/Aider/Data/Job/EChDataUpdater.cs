//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Rules;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Data.Platform;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Epsitec.Aider.Data.Job
{
	internal class EChDataUpdater
	{
		public EChDataUpdater(string oldEchFile, string newEchFile, string reportFile, CoreData coreData, ParishAddressRepository parishAddressRepository,bool verboseLogging)
		{
			this.coreData = coreData;
			this.parishAddressRepository = parishAddressRepository;

			this.jobDateTime    = System.DateTime.Now;
			this.jobName        = "EChDataUpdate";
			this.jobDescription = string.Format ("Importation des données du RCH.\nBase {0}\nInc. {1}", oldEchFile, newEchFile);
			this.startDate      = Date.Today;

			this.warningTitleMessage = TextFormatter.FormatText ("Mise à jour ECh ", this.jobDateTime.ToShortDateString ());


			this.aiderPersonsTaggedForDeletion = new Dictionary<EntityKey, AiderPersonEntity> ();
			this.aiderPersonEntitiesWithDeletedHousehold = new Dictionary<EntityKey, AiderHouseholdEntity> ();


			this.verboseLogging = verboseLogging;

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

					this.DumpCollections (System.IO.Path.Combine (System.IO.Path.GetDirectoryName (reportFile), System.IO.Path.GetFileNameWithoutExtension (reportFile)));
					
					this.LogToConsole (time2, "analysis done");
				}

				this.LogToConsole (time1, "diff done, ready");
			}
			else
			{
				throw new System.Exception ("Failed to load ECh files");
			}
		}

		private void DumpCollections(string reportFile)
		{
			this.Dump (reportFile + ".pcreate.txt", this.personsToCreate);
			this.Dump (reportFile + ".premove.txt", this.personsToRemove);
			this.Dump (reportFile + ".pupdate.txt", this.personsToUpdate);
			this.Dump (reportFile + ".hcreate.txt", this.houseHoldsToCreate);
			this.Dump (reportFile + ".hremove.txt", this.houseHoldsToRemove);
			this.Dump (reportFile + ".hupdate.txt", this.houseHoldsToUpdate);
			this.Dump (reportFile + ".hnew.txt", this.newHouseHoldsToCreate);
			this.Dump (reportFile + ".hmissing.txt", this.missingHouseHoldsToRemove);
		}

		private void Dump(string path, List<Change<EChReportedPerson>> list)
		{
			System.IO.File.WriteAllLines (path, list.Select (x => x.OldValue.ToString () + " *** " + x.NewValue.ToString ()));
		}

		private void Dump(string path, List<Change<EChPerson>> list)
		{
			System.IO.File.WriteAllLines (path, list.Select (x => x.OldValue.ToString () + " *** " + x.NewValue.ToString ()));
		}

		private void Dump(string path, List<EChReportedPerson> list)
		{
			System.IO.File.WriteAllLines (path, list.Select (x => x.ToString ()));
		}

		private void Dump(string path, List<EChPerson> list)
		{
			System.IO.File.WriteAllLines (path, list.Select (x => x.ToString ()));
		}


		public void ProcessJob ()
		{		
			//	Update the EChPerson entities and add all required DataChangedECh warnings on
			//	the corresponding AiderPersons.
			var time = this.LogToConsole ("starting main job");

			this.UpdateEChPersonEntities ();
			this.UpdateHouseholdsAndPropagate (false);

			this.TagEChPersonsForDeletion ();
			this.TagAiderPersonsForMissingHousehold ();

			this.CreateNewEChPersons ();
			this.CreateNewAiderPersons ();

			this.RemoveOldEChReportedPersons ();
			this.CreateNewEChReportedPersons ();			//	houseHoldsToCreate
			this.CreateNewAiderHouseholds ();	

			this.LogToConsole (time, "done");
		}

		public void FixHouseholdUpdate()
		{
			var time = this.LogToConsole ("fixing broken household addresses");
			this.FixNewEChReportedPersons ();
			this.LogToConsole (time, "done");
		}
		
		public void FixPreviousUpdate()
		{
			var time = this.LogToConsole ("fixing last update");
			this.UpdateHouseholdsAndPropagate (true);
			this.LogToConsole (time, "done");
		}

		private bool UpdateEChPersonEntities()
		{
			this.LogToConsole ("UpdateEChPersonEntities()");

			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var item in this.personsToUpdate)
					{
						try
						{
							this.LogToConsole ("Info: Update EChPerson {0} {1}", item.OldValue.OfficialName, item.NewValue.FirstNames);
							var eChPersonNew = EChDataImporter.CopyEChPerson (item.NewValue, new eCH_PersonEntity ());
							var eChPerson    = this.GetEchPersonEntity (businessContext, item.NewValue);
							var aiderPerson  = this.GetAiderPersonEntity (businessContext, eChPerson);

							System.Diagnostics.Debug.Assert (aiderPerson.IsNotNull ());

							var changes = new List<FormattedText> ();
							changes.Add (aiderPerson.GetFullName ());

							if (StringUtils.NotEqualOrEmpty (eChPerson.PersonOfficialName, eChPersonNew.PersonOfficialName))
							{
								changes.Add (TextFormatter.FormatText ("Nom:", eChPerson.PersonOfficialName, "->", eChPersonNew.PersonOfficialName));
								eChPerson.PersonOfficialName = eChPersonNew.PersonOfficialName ?? "";
							}

							if (StringUtils.NotEqualOrEmpty (eChPerson.PersonFirstNames, eChPersonNew.PersonFirstNames))
							{
								changes.Add (TextFormatter.FormatText ("Prénom:", eChPerson.PersonFirstNames, "->", eChPersonNew.PersonFirstNames));
								eChPerson.PersonFirstNames = eChPersonNew.PersonFirstNames ?? "";
							}

							if (eChPerson.PersonDateOfBirth != eChPersonNew.PersonDateOfBirth)
							{
//-								changes.Add (TextFormatter.FormatText ("Date de naissance:", eChPerson.PersonDateOfBirth, "->", eChPersonNew.PersonDateOfBirth));
								eChPerson.PersonDateOfBirth = eChPersonNew.PersonDateOfBirth;
							}

							if (eChPerson.AdultMaritalStatus != eChPersonNew.AdultMaritalStatus)
							{
								if (eChPerson.AdultMaritalStatus != PersonMaritalStatus.None)
								{
									changes.Add (TextFormatter.FormatText ("État civil:", eChPerson.AdultMaritalStatus, "->", eChPersonNew.AdultMaritalStatus));
								}
								eChPerson.AdultMaritalStatus = eChPersonNew.AdultMaritalStatus;
							}

							if (eChPerson.PersonSex != eChPersonNew.PersonSex)
							{
//-								changes.Add (TextFormatter.FormatText ("Sexe:", eChPerson.PersonSex, "->", eChPersonNew.PersonSex));
								eChPerson.PersonSex = eChPersonNew.PersonSex;
							}

							if (eChPerson.NationalityCountryCode != eChPersonNew.NationalityCountryCode)
							{
//-								changes.Add (TextFormatter.FormatText ("Nationalité:", eChPerson.NationalityCountryCode, "->", eChPersonNew.NationalityCountryCode));
								eChPerson.NationalityCountryCode = eChPersonNew.NationalityCountryCode;
							}

							if (eChPerson.NationalityStatus != eChPersonNew.NationalityStatus)
							{
//-								changes.Add (TextFormatter.FormatText ("Statut nationalité:", eChPerson.NationalityStatus, "->", eChPersonNew.NationalityStatus));
								eChPerson.NationalityStatus = eChPersonNew.NationalityStatus;
							}

							if (eChPerson.Origins != eChPersonNew.Origins)
							{
//-								changes.Add (TextFormatter.FormatText ("Origines:", eChPerson.Origins, "->", eChPersonNew.Origins));
								eChPerson.Origins = eChPersonNew.Origins;
							}

							if (changes.Count > 1)
							{
								this.LogToConsole ("Info: {0} changes detected",changes.Count);
								AiderPersonBusinessRules.UpdatePersonOfficialName (aiderPerson);
								AiderPersonBusinessRules.UpdatePersonSex (aiderPerson);
								AiderPersonBusinessRules.UpdateVisibility (aiderPerson);
								this.LogToConsole ("Info: Refresh entity cache");
								aiderPerson.RefreshCache ();
								aiderPerson.Households.ForEach (x => x.RefreshCache ());
								aiderPerson.Contacts.ForEach (x => x.RefreshCache ());
								this.LogToConsole ("Info: warning added: EChPersonDataChanged");
								this.CreateWarning (businessContext, aiderPerson, aiderPerson.ParishGroupPathCache, WarningType.EChPersonDataChanged, this.warningTitleMessage, changes);
							}
						}
						catch (System.Exception exception)
						{
							this.LogToConsole ("Error: EChPerson {0} {1} threw an exception.\n{2}", item.OldValue.OfficialName, item.NewValue.FirstNames, exception.GetFullText ());
						}
					}
				});
			
			return true;
		}

		private void RestoreHouseholdAddressAndClearWarnings(BusinessContext businessContext, Change<EChReportedPerson> item, eCH_ReportedPersonEntity family, AiderHouseholdEntity potentialAiderHousehold)
		{
			//	Clear past warnings, then retry and restore the household address...

			//	Setup old ECh household with the previous ECh address

			var oldEChAddress = new eCH_AddressEntity ()
			{
				AddressLine1      = item.OldValue.Address.AddressLine1,
				HouseNumber       = item.OldValue.Address.HouseNumber,
				Street            = item.OldValue.Address.Street ?? "",
				SwissZipCode      = item.OldValue.Address.SwissZipCode,
				SwissZipCodeAddOn = item.OldValue.Address.SwissZipCodeAddOn,
				SwissZipCodeId    = item.OldValue.Address.SwissZipCodeId,
				Town              = item.OldValue.Address.Town ?? "",
				Country           = item.OldValue.Address.CountryCode ?? ""
			};

			var oldEChReportedPerson = new eCH_ReportedPersonEntity ()
			{
				Address = oldEChAddress
			};

			this.UpdateAiderHouseholdAddress (businessContext, potentialAiderHousehold, oldEChReportedPerson);

			var persons = family.Members
				.Select (x => this.GetAiderPersonEntity (businessContext, x))
				.Concat (potentialAiderHousehold.Members)
				.Distinct ();

			foreach (var aiderPerson in persons)
			{
				var warnings = aiderPerson.Warnings.ToArray ();

				foreach (var warning in warnings)
				{
					if (warning.WarningType == WarningType.EChAddressChanged ||
													warning.WarningType == WarningType.ParishArrival ||
													warning.WarningType == WarningType.ParishDeparture)
					{
						AiderPersonWarningEntity.Delete (businessContext, warning);
					}
				}

				//	Never mind history of parish participation -- we probably have none and we want
				//	to make sure that we clean up the current mess...

				var parishes = aiderPerson.Groups.Where (x => x.Group.IsParish ()).ToList ();

				foreach (var parishParticipation in parishes)
				{
					aiderPerson.RemoveParticipationInternal (parishParticipation);
					businessContext.DeleteEntity (parishParticipation);
				}
			}

			ParishAssigner.AssignToParish (parishAddressRepository, businessContext, persons);
		}


		private void ReassignHousehold(BusinessContext businessContext, List<FormattedText> changes, eCH_ReportedPersonEntity family, AiderPersonEntity person, AiderHouseholdEntity household)
		{
			this.LogToConsole ("Info: Reassign household");
			var isSameHead = household.IsHead (person);
			var isSameMemberCount = household.Members.Count (x => x.IsGovernmentDefined).Equals (family.MembersCount);
			
			//	Ensure that potential family is like ECh ReportedPerson before apply a full relocate
			
			if (isSameHead&&isSameMemberCount)
			{
				this.LogToConsole ("Info: Same head and member count detected, processing full relocate");
				var members = household.Members;

				//	First, reassign the parents, then the children. This simplifies the updating
				//	of the children' household address in UpdateAiderHouseholdAndSubscription.

				foreach (var member in members.Where (x => household.IsHead (x)))
				{

					this.UpdateAiderHouseholdAndSubscription (businessContext, family, member);
					this.ReassignAndWarnParish (businessContext, member, changes);

				}

				foreach (var member in members.Where (x => household.IsHead (x) == false))
				{
					this.UpdateAiderHouseholdAndSubscription (businessContext, family, member);
					this.ReassignAndWarnParish (businessContext, member, changes);
				}
			}
			else //potential family is different relocate head from ECh new Data
			{
				this.LogToConsole ("Info: Reassign only {0}", person.GetFullName ());
				var warningMessage = FormattedText.FromSimpleText ("Cette personne a maintenant son propre ménage.");
				this.RelocateAndCreateNewAiderHousehold (businessContext, family);
				this.LogToConsole ("Info: warning added: EChHouseholdAdded");
				this.CreateWarning (businessContext, person, person.ParishGroupPathCache, WarningType.EChHouseholdAdded, this.warningTitleMessage, warningMessage);
				this.ReassignAndWarnParish (businessContext, person, changes);
			}
		}

		private List<FormattedText> GetAddressChanges(AiderAddressEntity familyAddress, EChAddress newRchAddress)
		{
			var oldAddress = familyAddress.GetPostalAddress ();
			var newAddress = newRchAddress.GetSwissPostalAddress ();
			var changes = new List<FormattedText> ();

			changes.Add (TextFormatter.FormatText ("Changement dans l'adresse:"));
			changes.Add (TextFormatter.FormatText (oldAddress, "\n->\n", newAddress));

			return changes;
		}

		private bool UpdateAddress(eCH_AddressEntity familyAddress, EChAddress newRchAddress, List<FormattedText> changes)
		{
			var oldAddress = familyAddress.GetSummary ();

			if (this.UpdateAddress (familyAddress, newRchAddress))
			{
				var newAddress = familyAddress.GetSummary ();

				changes.Add (TextFormatter.FormatText ("Changement dans l'adresse:"));
				changes.Add (TextFormatter.FormatText (oldAddress, "\n->\n", newAddress));
				
				return true;
			}

			return false;
		}

		private bool UpdateAddress(eCH_AddressEntity familyAddress, EChAddress newRchAddress)
		{
			bool changed = false;

			if (StringUtils.NotEqualOrEmpty (familyAddress.AddressLine1, newRchAddress.AddressLine1))
			{
				familyAddress.AddressLine1 = newRchAddress.AddressLine1 ?? "";
			}

			if (StringUtils.NotEqualOrEmpty (familyAddress.HouseNumber, newRchAddress.HouseNumber))
			{
				familyAddress.HouseNumber = newRchAddress.HouseNumber ?? "";
				changed = true;
			}

			if (StringUtils.NotEqualOrEmpty (familyAddress.Street, newRchAddress.Street))
			{
				familyAddress.Street = newRchAddress.Street ?? "";
				changed = true;
			}

			if ((familyAddress.SwissZipCode != newRchAddress.SwissZipCode) ||
									(familyAddress.SwissZipCodeAddOn != newRchAddress.SwissZipCodeAddOn) ||
									(familyAddress.SwissZipCodeId != newRchAddress.SwissZipCodeId))
			{
				familyAddress.SwissZipCode      = newRchAddress.SwissZipCode;
				familyAddress.SwissZipCodeAddOn = newRchAddress.SwissZipCodeAddOn;
				familyAddress.SwissZipCodeId    = newRchAddress.SwissZipCodeId;
				changed = true;
			}

			if (StringUtils.NotEqualOrEmpty (familyAddress.Town, newRchAddress.Town))
			{
				familyAddress.Town = newRchAddress.Town ?? "";
				changed = true;
			}

			if (StringUtils.NotEqualOrEmpty (familyAddress.Country, newRchAddress.CountryCode))
			{
				familyAddress.Country = newRchAddress.CountryCode ?? "";
				changed = true;
			}

			return changed;
		}

		private bool UpdateHouseholdsAndPropagate(bool fixPreviousUpdate)
		{
			this.LogToConsole ("UpdateHouseholdsAndPropagate(fixPreviousUpdate={0})", fixPreviousUpdate);

			bool exit = false;

			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var item in this.houseHoldsToUpdate)
					{
						try
						{
							var family  = this.GetEchReportedPersonEntity (businessContext, item.NewValue);

							if (exit)
							{
								break;
							}

							if (family.IsNull ())
							{
								family = this.CreateEChReportedPerson (businessContext, item.OldValue, false);
							}

							eCH_AddressEntity familyAddress = family.Address;

							EChAddress newRchAddress = item.NewValue.Address;
							EChAddress oldRchAddress = item.OldValue.Address;

							if (fixPreviousUpdate)
							{
								//	Restore old state for the eCH family address:
								this.LogToConsole ("Info: Restore old state for the ECh family address");
								familyAddress.AddressLine1      = oldRchAddress.AddressLine1;
								familyAddress.HouseNumber       = oldRchAddress.HouseNumber;
								familyAddress.Street            = oldRchAddress.Street ?? "";
								familyAddress.SwissZipCode      = oldRchAddress.SwissZipCode;
								familyAddress.SwissZipCodeAddOn = oldRchAddress.SwissZipCodeAddOn;
								familyAddress.SwissZipCodeId    = oldRchAddress.SwissZipCodeId;
								familyAddress.Town              = oldRchAddress.Town ?? "";
								familyAddress.Country           = oldRchAddress.CountryCode ?? "";
							}

							var changes = new List<FormattedText> ();

							if (this.UpdateAddress (familyAddress, newRchAddress, changes) == false)
							{
								this.LogToConsole ("Info: No address change detected, skipping");
								continue;
							}

							this.LogToConsole ("Info: Address change detected, gathering Aider informations about adult1 and is potential household");
							var refPerson               = this.GetAiderPersonEntity (businessContext, family.Adult1);
							var potentialAiderHousehold = this.GetAiderHousehold (businessContext, refPerson);

							if (potentialAiderHousehold.IsNotNull ())
							{
								if (fixPreviousUpdate)
								{
									this.LogToConsole ("Info: Restore household address and clear warnings");
									this.RestoreHouseholdAddressAndClearWarnings (businessContext, item, family, potentialAiderHousehold);
								}
								this.ReassignHousehold (businessContext, changes, family, refPerson, potentialAiderHousehold);
							}
							else
							{
								//	Found a person without any contact/household...

								this.LogToConsole ("N'a pas encore été corrigé; pas de ménage pour {0}", refPerson.GetDisplayName ());
							}
						}
						catch (System.Exception)
						{
							this.LogToConsole ("Error: EChReportedPerson (FAMILYKEY:{0}) threw an exception", item.OldValue.FamilyKey);
						}
					}
				});

			return true;
		}

		private void TagEChPersonsForDeletion()
		{
			this.LogToConsole ("TagEChPersonsForDeletion()");
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChPerson in this.personsToRemove)
					{
						var existingPersonEntity = this.GetEchPersonEntity (businessContext, eChPerson);

						if (existingPersonEntity != null)
						{
							this.LogToConsole ("Info: {0} tagged with PersonDeclarationStatus.Removed", existingPersonEntity.GetCompactSummary ());
							existingPersonEntity.DeclarationStatus = PersonDeclarationStatus.Removed;

							this.LogToConsole ("Info: Gathering AiderPerson");
							var existingAiderPerson = this.GetAiderPersonEntity (businessContext, existingPersonEntity);

							if (existingAiderPerson.IsNotNull ())
							{
								this.LogToConsole ("Info: warning added: EChProcessDeparture");
								this.CreateWarning (businessContext, existingAiderPerson, existingAiderPerson.ParishGroupPathCache,
									/**/			WarningType.EChProcessDeparture, this.warningTitleMessage,
									/**/			TextFormatter.FormatText (existingAiderPerson.GetDisplayName (), "n'est plus dans le RCH."));

								var personKey = businessContext.DataContext.GetNormalizedEntityKey (existingAiderPerson).Value;

								this.aiderPersonsTaggedForDeletion.Add (personKey, existingAiderPerson);
							}
							else
							{
								this.LogToConsole ("Info: AiderPerson not found");
							}
						}
						else
						{
							this.LogToConsole ("Info: EChPerson entity not found");
						}
					}
				});
		}

		private void TagAiderPersonsForMissingHousehold()
		{
			this.LogToConsole ("TagAiderPersonsForMissingHousehold()");
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChReportedPerson in this.missingHouseHoldsToRemove)
					{
						
						foreach (var eChPerson in eChReportedPerson.GetMembers ())
						{
							var existingPerson      = this.GetEchPersonEntity (businessContext, eChPerson);
							var existingAiderPerson = this.GetAiderPersonEntity (businessContext, existingPerson);

							if (existingAiderPerson.IsNull ())
							{
								this.LogToConsole ("Info: No existing AiderPerson found, skipping");
								continue;
							}

							var personKey = businessContext.DataContext.GetNormalizedEntityKey (existingAiderPerson).Value;

							if (!this.aiderPersonsTaggedForDeletion.ContainsKey (personKey))
							{
								this.LogToConsole ("Info: warning added: EChHouseholdMissing");
								this.CreateWarning (businessContext, existingAiderPerson, existingAiderPerson.ParishGroupPathCache,
									/**/			WarningType.EChHouseholdMissing, this.warningTitleMessage,
									/**/			TextFormatter.FormatText (existingAiderPerson.GetDisplayName (), " n'a plus de famille dans le RCH."));//
							}
						}
					}
				});
		}

		private void CreateNewEChPersons()
		{
			this.LogToConsole ("CreateNewEChPersons()");
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChPerson in this.personsToCreate)
					{
						var existingPersonEntity = this.GetEchPersonEntity (businessContext, eChPerson);

						if (existingPersonEntity.IsNull ())
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

		private void CreateNewAiderPersons()
		{
			this.LogToConsole ("CreateNewAiderPersons()");
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChPerson in this.personsToCreate)
					{
						var eChPersonEntity = this.GetEchPersonEntity (businessContext, eChPerson);
						var existingAiderPersonEntity = this.GetAiderPersonEntity (businessContext, eChPersonEntity);

						if (existingAiderPersonEntity.IsNull ())
						{
							this.LogToConsole ("New: AiderPerson ECHPERSONID:{0}", eChPerson.Id);
							var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();

							aiderPersonEntity.eCH_Person = eChPersonEntity;
							aiderPersonEntity.MrMrs = EChDataImporter.GuessMrMrs (eChPerson.Sex, eChPerson.DateOfBirth, eChPerson.MaritalStatus);
							aiderPersonEntity.Confession = PersonConfession.Protestant;
							
							ParishAssigner.AssignToParish (parishAddressRepository, businessContext, aiderPersonEntity, this.startDate);

							this.CreateArrivalWarningForNewHousehold (businessContext, aiderPersonEntity);
						}
						else
						{
							this.LogToConsole ("Updated: AiderPerson ECHPERSONID:{0}", eChPerson.Id);

							//Update EChDATA
							existingAiderPersonEntity.eCH_Person = eChPersonEntity;

							this.CreateArrivalWarningForNewHousehold (businessContext, existingAiderPersonEntity);				
						}

					}
				});
		}

		private void CreateArrivalWarningForNewHousehold(BusinessContext businessContext, AiderPersonEntity aiderPersonEntity)
		{
			//if this person come from new household
			if (this.eChPersonIdWithNewHousehold.Contains (aiderPersonEntity.eCH_Person.PersonId))
			{
				this.CreateWarning (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChProcessArrival, this.warningTitleMessage, FormattedText.FromSimpleText (aiderPersonEntity.GetDisplayName () + ", nouvelle famille dans le RCH."));
			}
			else
			{
				this.eChPersonIdWithNewPerson.Add (aiderPersonEntity.eCH_Person.PersonId);
			}
		}

		private void RemoveOldEChReportedPersons()
		{
			this.LogToConsole ("RemoveOldEChReportedPersons()");
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChReportedPerson in this.houseHoldsToRemove)
					{
						var eChReportedPersonEntity = this.GetEchReportedPersonEntity (businessContext, eChReportedPerson);
						
						if (eChReportedPersonEntity.IsNotNull ())
						{
							this.LogToConsole ("Removed: EChReportedPerson FAMILYKEY:{0}", eChReportedPerson.FamilyKey);
							businessContext.DeleteEntity (eChReportedPersonEntity.Address);
							businessContext.DeleteEntity (eChReportedPersonEntity);
						}
						else
						{
							this.LogToConsole ("Warning: Entity not found: EChReportedPerson FAMILYKEY:{0}", eChReportedPerson.FamilyKey);
						}
					}
				});
		}

		private static void RevertEChAddress(eCH_ReportedPersonEntity family, AiderHouseholdEntity household)
		{
			var echAddress = family.Address;
			var oldAddress = household.Address;
			var oldTown = oldAddress.Town;

			echAddress.AddressLine1       = oldAddress.AddressLine1;
			echAddress.StreetUserFriendly = oldAddress.StreetUserFriendly;
			echAddress.HouseNumber        = oldAddress.HouseNumberAndComplement;
			echAddress.SwissZipCode       = oldTown.SwissZipCode.GetValueOrDefault (0);
			echAddress.SwissZipCodeAddOn  = oldTown.SwissZipCodeAddOn.GetValueOrDefault (0);
			echAddress.SwissZipCodeId     = oldTown.SwissZipCodeId.GetValueOrDefault (0);
			echAddress.Country            = oldTown.Country.IsoCode;
			echAddress.Town               = oldTown.Name;
		}

		private void FixNewEChReportedPersons()
		{
			this.LogToConsole ("CreateNewEChReportedPersons()");
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					var households = new List<System.Tuple<EChReportedPerson, AiderHouseholdEntity, AiderPersonEntity>> ();

					foreach (var eChReportedPerson in this.houseHoldsToCreate)
					{
						var adult1    = this.GetEchPersonEntity (businessContext, eChReportedPerson.Adult1);
						var person    = this.GetAiderPersonEntity (businessContext, adult1);

						if (person.IsNull ())
						{
							this.LogToConsole ("COHERENCE ERROR - Person not found: {0}", eChReportedPerson.Adult1.ToString ());
							continue;
						}

						if (person.HouseholdContact.IsNull ())
						{
							this.LogToConsole ("COHERENCE ERROR - Person has no household: {0}", eChReportedPerson.Adult1.ToString ());
							continue;
						}

						var household        = person.HouseholdContact.Household;
						var householdAddress = household.Address;
						var rchAddress       = eChReportedPerson.Address;

						if (!this.AddressComparator (householdAddress, rchAddress))
						{
							households.Add (System.Tuple.Create (eChReportedPerson, household, person));
						}
					}

					this.LogToConsole ("Identified {0} households which need fixing...", households.Select (x => x.Item2).Distinct ().Count ());

					var processedHouseholds = new HashSet<AiderHouseholdEntity> ();

					foreach (var tuple in households)
					{
						if (processedHouseholds.Add (tuple.Item2))
						{
							var changes   = new List<FormattedText> ();
							var family    = this.GetEchReportedPersonEntity (businessContext, tuple.Item1);
							var address   = tuple.Item1.Address;
							var household = tuple.Item2;
							var person    = tuple.Item3;

							//	No idea why some eCH reported persons cannot be resolved, since there is
							//	one at hand through the person -> eCH_Person...

							if (family.IsNull ())
							{
								family = person.eCH_Person.ReportedPerson1;
							}

							EChDataUpdater.RevertEChAddress (family, household);

							if (this.UpdateAddress (family.Address, address, changes))
							{
								this.ReassignHousehold (businessContext, changes, family, person, household);
							}
							else
							{
								this.LogToConsole ("COHERENCE ERROR - Address update did not detect any change");
							}
						}
					}
				}
			);
		}

		private void CreateNewEChReportedPersons()
		{
			this.LogToConsole ("CreateNewEChReportedPersons()");
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChReportedPerson in this.houseHoldsToCreate)
					{
						this.CreateEChReportedPerson (businessContext, eChReportedPerson,true);
					}
				});
		}

		private eCH_ReportedPersonEntity CreateEChReportedPerson(BusinessContext businessContext, EChReportedPerson eChReportedPerson,bool processAiderHouseholdChanges)
		{
			//	Create eCH ReportedPerson entity, with the new eCH address, and assign the
			//	two adults (if any) and the children found in the eCH data:
			var eChAddressEntity          = EChDataImporter.ImportEchAddressEntity (businessContext, eChReportedPerson.Address);
			var eChReportedPersonEntity   = businessContext.CreateAndRegisterEntity<eCH_ReportedPersonEntity> ();
			eChReportedPersonEntity.Address = eChAddressEntity;

			//create ref aiderPerson if needed (weird case)
			var refAiderPerson = this.GetAiderPersonEntity (businessContext, eChReportedPerson.Adult1);
			if (refAiderPerson.IsNull ())
			{
				var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
				aiderPersonEntity.eCH_Person = eChReportedPersonEntity.Adult1;
				this.LogToConsole ("Warning: Need to create the AiderPersonEntity: ECHPERSONID:{0}", eChReportedPersonEntity.Adult1.PersonId);

				refAiderPerson = aiderPersonEntity;
			}

			//retreiving ref aiderHousehold
			var refAiderHousehold = this.GetAiderHousehold (businessContext, refAiderPerson);

			eChReportedPersonEntity.Adult1 = refAiderPerson.eCH_Person;

			if (processAiderHouseholdChanges)
			{
				this.ProcessAiderHouseholdChangesForMember (businessContext, refAiderPerson.eCH_Person, eChReportedPerson.Address, eChReportedPersonEntity, refAiderHousehold);
			}

			if (eChReportedPerson.Adult2 != null)
			{
				var eChPersonEntity = this.GetEchPersonEntity (businessContext, eChReportedPerson.Adult2);
				eChReportedPersonEntity.Adult2 = eChPersonEntity;
				eChReportedPersonEntity.RemoveDuplicates ();

				if (processAiderHouseholdChanges)
				{
					this.ProcessAiderHouseholdChangesForMember (businessContext, eChPersonEntity, eChReportedPerson.Address, eChReportedPersonEntity, refAiderHousehold);
				}
			}

			foreach (var eChChild in eChReportedPerson.Children)
			{
				var eChPersonEntity  = this.GetEchPersonEntity (businessContext, eChChild);
				eChReportedPersonEntity.Children.Add (eChPersonEntity);
				eChReportedPersonEntity.RemoveDuplicates ();

				if (processAiderHouseholdChanges)
				{
					this.ProcessAiderHouseholdChangesForMember (businessContext, eChPersonEntity, eChReportedPerson.Address, eChReportedPersonEntity, refAiderHousehold);
				}
			}

			return eChReportedPersonEntity;
		}

		private void CreateNewAiderHouseholds()
		{
			this.LogToConsole ("CreateNewAiderHouseholds()");
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChReportedPerson in this.newHouseHoldsToCreate)
					{
						this.LogToConsole ("Info: Processing FAMILYKEY:{0}",eChReportedPerson.FamilyKey);
						var existingEChReportedPerson = this.GetEchReportedPersonEntity(businessContext,eChReportedPerson);

						if(existingEChReportedPerson.IsNull ())
						{
							existingEChReportedPerson = this.CreateEChReportedPerson (businessContext, eChReportedPerson, false);
						}
				
						this.LogToConsole ("Info: EChReportedPerson found");
						var existingAiderHousehold = this.GetAiderHousehold (businessContext, existingEChReportedPerson.Adult1);
						if (existingAiderHousehold.IsNotNull ())
						{

							this.LogToConsole ("Info: Aider household already exist, skipping");
							AiderContactEntity.DeleteDuplicateContacts (businessContext, existingAiderHousehold.Contacts);		
							continue;

						}
						else
						{
							this.LogToConsole ("Info: Aider household creation");
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
							if (existingEChReportedPerson.Adult1.IsNotNull ())
							{
								this.SetupAndAiderHouseholdForMember (businessContext, existingEChReportedPerson.Adult1, existingEChReportedPerson, aiderHousehold, true, false);
							}
							else
							{
								this.LogToConsole ("Warning: Adult 1 is null");
							}

							if (existingEChReportedPerson.Adult2.IsNotNull ())
							{
								this.SetupAndAiderHouseholdForMember (businessContext, existingEChReportedPerson.Adult2, existingEChReportedPerson, aiderHousehold, false, true);
							}
								
							foreach (var child in existingEChReportedPerson.Children)
							{
								this.SetupAndAiderHouseholdForMember (businessContext, child, existingEChReportedPerson, aiderHousehold, false, false);
							}
						}
						
					}
				});
		}

		private void SetupAndAiderHouseholdForMember(BusinessContext businessContext, eCH_PersonEntity eChPerson, eCH_ReportedPersonEntity eChReportedPersonEntity, AiderHouseholdEntity aiderHousehold, bool isHead1, bool isHead2)
		{
			this.LogToConsole ("Info: Processing PERSONID:{0} setup", eChPerson.PersonId);
			var aiderPerson = this.GetAiderPersonEntity (businessContext, eChPerson);
			this.LogToConsole ("Info: Try to remove warning added previously");
			//remove EChHouseholdAdded warning added previously (if exist)
			foreach (var warning in aiderPerson.Warnings)
			{
				if (warning.WarningType.Equals (WarningType.EChHouseholdAdded) && warning.Title.Equals (this.warningTitleMessage))
				{
					businessContext.DeleteEntity (warning);
					this.LogToConsole ("Info: Warning removed");
				}
			}

			//Setup household if needed
			if (!this.eChPersonIdWithHouseholdSetupDone.Contains (aiderPerson.eCH_Person.PersonId))
			{
				this.LogToConsole ("Info: No previous setup detected, processing household setup");
				EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isHead1, isHead2);
			}
			else
			{
				this.LogToConsole ("Info: Aiderhousehold already setuped");
			}
		}

		private void ProcessAiderHouseholdChangesForMember(BusinessContext businessContext, eCH_PersonEntity eChPersonEntity,EChAddress rchAddress, eCH_ReportedPersonEntity eChReportedPersonEntity,AiderHouseholdEntity refAiderHousehold)
		{
			this.LogToConsole ("Info: Processing {0}", eChPersonEntity.PersonId);

			//Assign household EChHousehold
			eChPersonEntity.ReportedPerson1 = eChReportedPersonEntity;
			
			var aiderPerson = this.GetAiderPersonEntity (businessContext, eChPersonEntity);
			//autoassign person to AiderHousehold if needed
			if (aiderPerson.Households.IsEmpty ())
			{
				this.LogToConsole ("Info: Household setup needed for :{0}", aiderPerson.GetFullName ());
				
				if (refAiderHousehold.IsNotNull ())
				{
					EChDataImporter.SetupHousehold (businessContext, aiderPerson, refAiderHousehold, eChReportedPersonEntity, isHead1: true);
					this.eChPersonIdWithHouseholdSetupDone.Add (aiderPerson.eCH_Person.PersonId);
					ParishAssigner.AssignToParish (parishAddressRepository, businessContext, aiderPerson, this.startDate);
					this.LogToConsole ("Info: Setup done and parish assigned -> further process will skip household creation");
				}
				else
				{
					this.LogToConsole ("Info: No AiderHousehold found -> further process needed");
				}
			}
			else // We need to check for relocate
			{
				this.LogToConsole ("Info: {0} is already assiged to an household", aiderPerson.GetFullName ());
				var aiderHousehold		= aiderPerson.HouseholdContact.Household;
				var householdAddress	= aiderHousehold.Address;

				if (!this.AddressComparator (householdAddress, rchAddress))
				{
					this.LogToConsole ("Info: new address detected, starting reassign");
					var changes = this.GetAddressChanges (householdAddress, rchAddress);
					this.ReassignHousehold (businessContext, changes, eChReportedPersonEntity, aiderPerson, aiderHousehold);

				}
				else
				{
					this.LogToConsole ("Info: same address detected, nothing to do");
				}

			}

			if (this.eChPersonIdWithNewPerson.Contains (aiderPerson.eCH_Person.PersonId))
			{
				this.LogToConsole ("Info: warning added: EChProcessArrival");
				this.CreateWarning (businessContext, aiderPerson, aiderPerson.ParishGroupPathCache, WarningType.EChProcessArrival, this.warningTitleMessage, FormattedText.FromSimpleText (aiderPerson.GetDisplayName () + ": nouveau dans le RCH."));
			}
		}

		private void RelocateAndCreateNewAiderHousehold(BusinessContext businessContext,eCH_ReportedPersonEntity eChReportedPerson)
		{
			this.LogToConsole ("Info: Relocate and create new AiderHousehold");
			var aiderHousehold = businessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
			aiderHousehold.HouseholdMrMrs = HouseholdMrMrs.Auto;

			var aiderAddressEntity = aiderHousehold.Address;
			var eChAddressEntity = eChReportedPerson.Address;


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
			aiderAddressEntity.Town = this.GetAiderTownEntity (businessContext, eChReportedPerson.Address.SwissZipCodeId);


			//Link household to ECh Entity
			if (eChReportedPerson.Adult1.IsNotNull ())
			{
				this.LogToConsole ("Info: Processing Adult 1 Relocation");
				var aiderPerson = this.GetAiderPersonEntity (businessContext, eChReportedPerson.Adult1);
				var oldHousehold = this.GetAiderHousehold (businessContext, aiderPerson);
				//Remove Old Contact
				if (oldHousehold.IsNotNull ())
				{
					var contactToRemove = aiderPerson.Contacts.Where (c => c.Household == oldHousehold).FirstOrDefault ();
					businessContext.DeleteEntity (contactToRemove);
					this.LogToConsole ("Info: old household detected, removing contact for {0}",aiderPerson.GetFullName ());
				}
				else
				{
					this.LogToConsole ("Info: No old household detected");
				}

				this.LogToConsole ("Info: Create contact for the new household");
				AiderContactEntity.Create (businessContext, aiderPerson, aiderHousehold, true);
			}
			else
			{
				this.LogToConsole ("Warning: Adult 1 is null");
			}

			if (eChReportedPerson.Adult2.IsNotNull ())
			{
				this.LogToConsole ("Info: Processing Adult 2 Relocation");
				var aiderPerson = this.GetAiderPersonEntity (businessContext, eChReportedPerson.Adult2);
				var oldHousehold = this.GetAiderHousehold (businessContext, aiderPerson);
				//Remove Old Contact
				if (oldHousehold.IsNotNull ())
				{
					var contactToRemove = aiderPerson.Contacts.Where (c => c.Household == oldHousehold).FirstOrDefault ();
					businessContext.DeleteEntity (contactToRemove);
					this.LogToConsole ("Info: old household detected, removing contact for {0}", aiderPerson.GetFullName ());
				}
				else
				{
					this.LogToConsole ("Info: No old household detected");
				}

				this.LogToConsole ("Info: Create contact for the new household");
				AiderContactEntity.Create (businessContext, aiderPerson, aiderHousehold, false);

			}

			foreach (var child in eChReportedPerson.Children)
			{
				this.LogToConsole ("Info: Processing Child Relocation");
				var aiderPerson = this.GetAiderPersonEntity (businessContext, child);
				var oldHousehold = this.GetAiderHousehold (businessContext, aiderPerson);
				
				//Remove or not for child?
				//Remove Old Contact
				if (oldHousehold.IsNotNull ())
				{
					var contactToRemove = aiderPerson.Contacts.Where (c => c.Household == oldHousehold).FirstOrDefault ();
					if (contactToRemove.IsNotNull ())
					{
						this.LogToConsole ("Info: We keep the old contact");
					}
					//businessContext.DeleteEntity (contactToRemove);
				}
				this.LogToConsole ("Info: Create contact for the new household");
				AiderContactEntity.Create (businessContext, aiderPerson, aiderHousehold, false);

			}
			
		}
		

		private void ReassignAndWarnParish(BusinessContext businessContext, AiderPersonEntity aiderPersonEntity, List<FormattedText> changes)
		{
			this.LogToConsole ("Info: Reassign and warn parish");
			var oldParishGroupPath = aiderPersonEntity.ParishGroupPathCache;

			ParishAssigner.ReassignToParish (this.parishAddressRepository, businessContext, aiderPersonEntity, this.startDate);

			var newParishGroupPath = aiderPersonEntity.ParishGroupPathCache;

			if (oldParishGroupPath != newParishGroupPath)
			{
				this.LogToConsole ("Info: Parish group path is different, arrival and departure parish warned");
				this.CreateWarning (businessContext, aiderPersonEntity, oldParishGroupPath, WarningType.ParishDeparture, this.warningTitleMessage, changes);
				this.CreateWarning (businessContext, aiderPersonEntity, newParishGroupPath, WarningType.ParishArrival, this.warningTitleMessage, changes);
			}
			else //if no change in parish group path, we create an simple address change warning
			{
				this.LogToConsole ("Info: No parish change detected, warning: EChAddressChanged added");
				this.CreateWarning (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChAddressChanged, this.warningTitleMessage, changes);
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

		private void UpdateAiderHouseholdAndSubscription(BusinessContext businessContext, eCH_ReportedPersonEntity family, AiderPersonEntity person)
		{
			this.LogToConsole ("Info: Try update household and subscription for {0}",person.GetFullName ());
			var aiderHouseholds = this.GetAiderHouseholds (businessContext, person).ToList ();

			if (aiderHouseholds.Any ())
			{
				foreach (var household in aiderHouseholds)
				{
					//	Only update households where this person is a head; children and
					//	other person won't affect the address of the household...

					if (household.IsHead (person))
					{
						this.LogToConsole ("Info: Household and subscription updated", person.GetFullName ());
						this.UpdateAiderHouseholdAddress (businessContext, household, family);
						this.UpdateAiderSubscription (businessContext, household);
					}
					else
					{
						this.LogToConsole ("Info: Skip update, {0} is not head", person.GetFullName ());
					}
				}

				person.Contacts.ForEach (x => x.RefreshCache ());
			}
			else
			{
				this.LogToConsole ("Warning: No household to update detected, warning: MissingHousehold added");
				this.CreateWarning (businessContext, person, person.ParishGroupPathCache, WarningType.MissingHousehold, this.warningTitleMessage,
					/**/			FormattedText.FromSimpleText (person.GetDisplayName () + " n'a pas de ménage dans Aider."));
			}

			person.Contacts.ForEach (x => x.RefreshCache ());
		}

		private void UpdateAiderHouseholdAddress(BusinessContext businessContext, AiderHouseholdEntity aiderHousehold, eCH_ReportedPersonEntity family)
		{
			var houseNumberAlpha      = SwissPostStreet.StripHouseNumber (family.Address.HouseNumber);
			var houseNumber           = StringUtils.ParseNullableInt (houseNumberAlpha);
			var houseNumberComplement = SwissPostStreet.GetHouseNumberComplement (family.Address.HouseNumber);

			if (string.IsNullOrWhiteSpace (houseNumberComplement))
			{
				houseNumberComplement = null;
			}

			var address = aiderHousehold.Address;
			var town    = this.GetAiderTownEntity (businessContext, family.Address.SwissZipCodeId);

			address.AddressLine1          = family.Address.AddressLine1;
			address.Street                = family.Address.Street;
			address.HouseNumber           = houseNumber;
			address.HouseNumberComplement = houseNumberComplement;
			address.Town                  = town;

			aiderHousehold.RefreshCache ();
		}

		private void UpdateAiderSubscription(BusinessContext businessContext, AiderHouseholdEntity household)
		{
			var subscriptionExample = new AiderSubscriptionEntity ()
			{
				Household = household
			};

			var subscription = businessContext.DataContext.GetByExample<AiderSubscriptionEntity> (subscriptionExample).FirstOrDefault ();

			if (subscription.IsNotNull ())
			{
				subscription.RefreshCache ();
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


		private void CreateWarning(BusinessContext context, AiderPersonEntity person, string parishGroupPath, WarningType warningType, FormattedText title, IEnumerable<FormattedText> changes)
		{
			this.CreateWarning (context, person, parishGroupPath, warningType, title, TextFormatter.Join (FormattedText.HtmlBreak, changes));
		}

		private void CreateWarning(BusinessContext context, AiderPersonEntity person, string parishGroupPath, WarningType warningType, FormattedText title, IEnumerable<string> changes)
		{
			this.CreateWarning (context, person, parishGroupPath, warningType, title,
				TextFormatter.Join (FormattedText.HtmlBreak, changes.Select (c => FormattedText.Format (c))));
		}

		private void CreateWarning(BusinessContext context, AiderPersonEntity person, string parishGroupPath,
								   WarningType warningType, FormattedText title, FormattedText description)
		{
			var personId = person.eCH_Person.PersonId;

			if (string.IsNullOrEmpty (personId))
			{
				personId = context.DataContext.GetNormalizedEntityKey (person).ToString ();
			}

			var fullKey = string.Format ("{0}-{1}-{2}", personId, parishGroupPath, (int) warningType);

			if (this.warningKeys.Add (fullKey))
			{
				AiderPersonWarningEntity.Create (context, person, parishGroupPath, warningType, title, description, this.warningSource);
			}
			else
			{
				this.LogToConsole ("Duplicate warning: {0}", fullKey);
			}
		}

		private HashSet<string> warningKeys;

		
		private AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, EChPerson person)
		{
			if (person == null)
			{
				return null;
			}

			var personExample = new AiderPersonEntity();

			personExample.eCH_Person = new eCH_PersonEntity()
			{
				PersonId = person.Id
			};

			return businessContext.DataContext.GetByExample<AiderPersonEntity>(personExample).FirstOrDefault();
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

		private AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, string eChPersonId)
		{
			var personExample = new AiderPersonEntity();

			personExample.eCH_Person = new eCH_PersonEntity()
			{
				PersonId = eChPersonId
			};

			return businessContext.DataContext.GetByExample<AiderPersonEntity>(personExample).FirstOrDefault();
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
			if (refPerson.IsNull ())
			{
				return null;
			}

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
			return this.GetAiderHouseholds (businessContext, refPerson).FirstOrDefault ();
		}

		private IEnumerable<AiderHouseholdEntity> GetAiderHouseholds(BusinessContext businessContext, AiderPersonEntity refPerson)
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

			return businessContext.DataContext.GetByRequest<AiderHouseholdEntity> (request);
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


		private void ExecuteWithBusinessContext(System.Action<BusinessContext> action)
		{
			var stackTrace    = new System.Diagnostics.StackTrace ();
			var stackFrames   = stackTrace.GetFrames ();
			var callingMethod = stackFrames[0].GetMethod ();

			var callerName = callingMethod.Name;

			var time = this.LogToConsole ("{0}, start job", callerName);
			int warningCount = 0;

			using (var businessContext = new BusinessContext (this.coreData, false))
			{
				this.EnsureWarningSource (businessContext);
				this.warningKeys = new HashSet<string> ();

				action (businessContext);

				warningCount = this.warningKeys.Count;

				this.warningKeys = null;
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
			}

			this.LogToConsole (time, "{0}, done ({1} warnings)", callerName, warningCount);
		}

		private void EnsureWarningSource(BusinessContext businessContext)
		{
			if (this.warningSource == null)
			{
				this.warningSource = AiderPersonWarningSourceEntity.Create (businessContext, this.jobDateTime, this.jobName, TextFormatter.FormatText (this.jobDescription));
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
			var verbose = false;
			if (message.StartsWith ("Error"))
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
			}

			if (message.StartsWith ("Info"))
			{
				verbose = true;
			}

			if ((verbose == true && this.verboseLogging == true) || verbose == false)
			{
				System.Console.WriteLine ("EChDataUpdater: {0}", message);
			}

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

		private bool AddressComparator(AiderAddressEntity householdAddress, EChAddress rchAddress)
		{
			if (householdAddress.Street.IsNullOrWhiteSpace () || householdAddress.Town.IsNull ())
			{
				return false;
			}

			var h1 = householdAddress.Street
						.FirstToken (",")
						.Replace(" ","")
						.ToUpper () + 
						householdAddress.HouseNumberAndComplement.ToUpper ()
						.Replace (" ", "") + 
						householdAddress.Town.SwissZipCodeId.ToString ();

			var h2 = rchAddress.Street
						.FirstToken(",")
						.Replace(" ","")
						.ToUpper () + 
						rchAddress.HouseNumber.ToUpper ()
						.Replace (" ", "") + 
						rchAddress.SwissZipCodeId.ToString ();

			if (h1 == h2)
				return true;
			else
				return false;
		}

		
		
		private CoreData						coreData;
		private ParishAddressRepository			parishAddressRepository;
		
		private readonly Dictionary<EntityKey, AiderPersonEntity>	 aiderPersonsTaggedForDeletion;
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
		private Date							startDate;
		
		private AiderWarningSourceEntity		warningSource;
		private FormattedText					warningTitleMessage;

		private bool							verboseLogging;
	}
}
