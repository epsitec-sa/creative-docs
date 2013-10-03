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
		public EChDataUpdater(string oldEchFile, string newEchFile, string reportFile, CoreData coreData, ParishAddressRepository parishAddressRepository)
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


		public void ProcessJob()
		{
			//	Try to repair broken things, and warn as Ech for corrections if needed
			this.PerformDataQuality ();
			
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


		private void PerformDataQuality()
		{
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					this.LogToConsole ("Perform DataQuality on Contacts");
					var db = businessContext.DataContext.DbInfrastructure;
					var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
					var sqlEngine = dbAbstraction.SqlEngine;
					var sqlSelect = new SqlSelect ();

					//FROM mud_lvaf
					SqlField table1 = SqlField.CreateName ("mud_lvaf");
					table1.Alias = "T1";
					sqlSelect.Tables.Add (table1);

					//INNER JOIN mud_lva
					SqlField table2 = SqlField.CreateName ("mud_lva");
					table2.Alias = "T2";
					//ON mud_lvaf.u_lvau1 = mud_lva.cr_id
					SqlJoin sqlJoin1 = SqlJoin.Create
					(
						SqlJoinCode.Inner,
						table2,
						SqlField.CreateName ("T1", "u_lvau1"),
						SqlField.CreateName ("T2", "cr_id")
					);
					sqlSelect.Joins.Add (SqlField.CreateJoin (sqlJoin1));

					//SELECT mud_lva.lva1
					sqlSelect.Fields.Add (SqlField.CreateName ("T2", "u_lva1"));

					//LEFT OUTER JOIN mud_lvard
					SqlField table3 = SqlField.CreateName ("mud_lvard");
					table3.Alias = "T3";
					//ON mud_lvaf.cr_id = mud_lvard.u_lva5e
					SqlJoin sqlJoin2 = SqlJoin.Create
					(
						SqlJoinCode.OuterLeft,
						table3,
						SqlField.CreateName ("T1", "cr_id"),
						SqlField.CreateName ("T3", "u_lva5e")
					);
					sqlSelect.Joins.Add (SqlField.CreateJoin (sqlJoin2));
					//WHERE mud_lvard.cr_id IS NULL
					SqlFunction condition1 = new SqlFunction
					(
						SqlFunctionCode.CompareIsNull,
						SqlField.CreateName ("T3", "cr_id")
					);
					sqlSelect.Conditions.Add (SqlField.CreateFunction (condition1));


					var sqlBuilder = dbAbstraction.SqlBuilder;
					sqlBuilder.SelectData (sqlSelect);
					var command = sqlBuilder.Command;
					command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

					DataSet dataSet;
					sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

					this.LogToConsole ("DataQuality SQL Results:");
					var personIdsToCorrect = new List<string> ();
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						if (!row[0].ToString ().IsNullOrWhiteSpace ())
						{
							personIdsToCorrect.Add (row[0].ToString ());
							this.LogToConsole (row[0] + " added");
						}
					}


					this.LogToConsole (personIdsToCorrect.Count + " persons without contacts detected");
					foreach (var eChPersonId in personIdsToCorrect)
					{
						//retreive AiderPerson
						var person = this.GetAiderPersonEntity (businessContext, eChPersonId);

						if (person.IsNull ())
						{
							this.LogToConsole ("No eCH person found for ID {0}", eChPersonId);
							continue;
						}

						//Retreive person aider household
						var household = this.GetAiderHousehold (businessContext, person.eCH_Person.ReportedPerson1.Adult1);
						if (household.IsNotNull ())
						{
							AiderContactEntity.Create (businessContext, person, household, isHead: household.IsHead (person));
							this.LogToConsole ("Corrected: {0}", person.GetDisplayName ());
						}
						else //warn
						{
							var warningMessage = FormattedText.FromSimpleText ("Ménage a recréer (problème de qualité de données)");
							this.CreateWarning (businessContext, person, person.ParishGroupPathCache, WarningType.EChHouseholdAdded, this.warningTitleMessage, warningMessage);
							this.LogToConsole ("Warning added for: {0}", person.GetDisplayName ());
						}
					}
				});
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
								AiderPersonBusinessRules.UpdatePersonOfficialName (aiderPerson);
								AiderPersonBusinessRules.UpdatePersonSex (aiderPerson);
								AiderPersonBusinessRules.UpdateVisibility (aiderPerson);

								aiderPerson.RefreshCache ();
								aiderPerson.Households.ForEach (x => x.RefreshCache ());
								aiderPerson.Contacts.ForEach (x => x.RefreshCache ());
							
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

		private bool UpdateHouseholdsAndPropagate()
		{
			this.LogToConsole ("UpdateHouseholdsAndPropagate()");

			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var item in this.houseHoldsToUpdate)
					{
						try
						{
							var family  = this.GetEchReportedPersonEntity (businessContext, item.NewValue);
							var changes = new List<FormattedText> ();
							var changed = false;

							changes.Add (TextFormatter.FormatText ("Changement dans l'adresse:"));

							var oldAddress = family.Address.GetSummary ();

							if (StringUtils.NotEqualOrEmpty (family.Address.AddressLine1, item.NewValue.Address.AddressLine1))
							{
								family.Address.AddressLine1 = item.NewValue.Address.AddressLine1 ?? "";
								changed = true;
							}

							if (StringUtils.NotEqualOrEmpty (family.Address.HouseNumber, item.NewValue.Address.HouseNumber))
							{
								family.Address.HouseNumber = item.NewValue.Address.HouseNumber ?? "";
								changed = true;
							}
							
							if (StringUtils.NotEqualOrEmpty (family.Address.Street, item.NewValue.Address.Street))
							{
								family.Address.Street = item.NewValue.Address.Street ?? "";
								changed = true;
							}

							if ((family.Address.SwissZipCode != item.NewValue.Address.SwissZipCode) ||
								(family.Address.SwissZipCodeAddOn != item.NewValue.Address.SwissZipCodeAddOn) ||
								(family.Address.SwissZipCodeId != item.NewValue.Address.SwissZipCodeId))
							{
								family.Address.SwissZipCode      = item.NewValue.Address.SwissZipCode;
								family.Address.SwissZipCodeAddOn = item.NewValue.Address.SwissZipCodeAddOn;
								family.Address.SwissZipCodeId    = item.NewValue.Address.SwissZipCodeId;
								changed = true;
							}

							if (StringUtils.NotEqualOrEmpty (family.Address.Town, item.NewValue.Address.Town))
							{
								family.Address.Town = item.NewValue.Address.Town ?? "";
								changed = true;
							}

							if (StringUtils.NotEqualOrEmpty (family.Address.Country, item.NewValue.Address.CountryCode))
							{
								family.Address.Country = item.NewValue.Address.CountryCode ?? "";
								changed = true;
							}

							if (changed == false)
							{
								continue;
							}

							var newAddress = family.Address.GetSummary ();

							changes.Add (TextFormatter.FormatText (oldAddress, "\n->\n", newAddress));

							var refPerson      = this.GetAiderPersonEntity (businessContext,family.Adult1);
							var potentialAiderHousehold = this.GetAiderHousehold (businessContext, refPerson);

							if (potentialAiderHousehold.IsNotNull ())
							{
								var isSameHead = potentialAiderHousehold.IsHead(refPerson);
								var isSameMemberCount = potentialAiderHousehold.Members.Count.Equals (family.MembersCount);
								//Ensure that potential family is like ECh ReportedPerson before apply a full relocate
								if (isSameHead&&isSameMemberCount)
								{

									var members = potentialAiderHousehold.Members;

									//	First, reassign the parents, then the children. This simplifies the updating
									//	of the children' household address in UpdateAiderHouseholdAndSubscription.

									foreach (var member in members.Where (x => potentialAiderHousehold.IsHead (x)))
									{

										this.UpdateAiderHouseholdAndSubscription (businessContext, family, member);
										this.ReassignAndWarnParish (businessContext, member, changes);

									}

									foreach (var member in members.Where (x => potentialAiderHousehold.IsHead (x) == false))
									{
										this.UpdateAiderHouseholdAndSubscription (businessContext, family, member);
										this.ReassignAndWarnParish (businessContext, member, changes);
									}
								}
								else //potential family is different
								{
									var warningMessage = FormattedText.FromSimpleText ("Cette personne a maintenant son propre ménage.");
									this.RelocateAndCreateNewAiderHousehold (businessContext, family);
									this.CreateWarning (businessContext, refPerson, refPerson.ParishGroupPathCache, WarningType.EChHouseholdAdded, this.warningTitleMessage, warningMessage);
									this.ReassignAndWarnParish (businessContext, refPerson, changes);
								}
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
							existingPersonEntity.RemovalReason = RemovalReason.Departed;
						}
					}
				});
		}

		private void TagAiderPersonsForDeletion()
		{
			this.LogToConsole ("TagAiderPersonsForDeletion()");
			this.ExecuteWithBusinessContext (
				businessContext =>
				{
					foreach (var eChPerson in this.personsToRemove)
					{
						var existingPerson      = this.GetEchPersonEntity (businessContext, eChPerson);
						var existingAiderPerson = this.GetAiderPersonEntity (businessContext, existingPerson);

						if (existingAiderPerson.IsNotNull ())
						{
							this.CreateWarning (businessContext, existingAiderPerson, existingAiderPerson.ParishGroupPathCache,
								/**/			WarningType.EChPersonMissing, this.warningTitleMessage,
								/**/			TextFormatter.FormatText (existingAiderPerson.GetDisplayName (), "n'est plus dans le RCH."));

							var personKey = businessContext.DataContext.GetNormalizedEntityKey (existingAiderPerson).Value;

							this.aiderPersonsTaggedForDeletion.Add (personKey, existingAiderPerson);
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

							var personKey = businessContext.DataContext.GetNormalizedEntityKey (existingAiderPerson).Value;

							if (this.aiderPersonsTaggedForDeletion.ContainsKey (personKey))
							{
								foreach (var warn in existingAiderPerson.Warnings)
								{
									if ((warn.WarningType == WarningType.EChPersonMissing) &&
										(warn.StartDate == Date.Today))
									{
										warn.WarningType = WarningType.EChProcessDeparture;
										break;
									}
								}
							}
							else
							{
								this.CreateWarning (businessContext, existingAiderPerson, existingAiderPerson.ParishGroupPathCache,
									/**/			WarningType.EChHouseholdMissing, this.warningTitleMessage,
									/**/			TextFormatter.FormatText (existingAiderPerson.GetDisplayName (), " n'a plus de famille dans le RCH."));
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
							var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();

							aiderPersonEntity.eCH_Person = eChPersonEntity;
							aiderPersonEntity.MrMrs = EChDataImporter.GuessMrMrs (eChPerson.Sex, eChPerson.DateOfBirth, eChPerson.MaritalStatus);
							aiderPersonEntity.Confession = PersonConfession.Protestant;
							
							ParishAssigner.AssignToParish (parishAddressRepository, businessContext, aiderPersonEntity, this.startDate);

							//if this person come from new household
							if (this.eChPersonIdWithNewHousehold.Contains (eChPerson.Id))
							{
								this.CreateWarning (businessContext, aiderPersonEntity, aiderPersonEntity.ParishGroupPathCache, WarningType.EChProcessArrival, this.warningTitleMessage, FormattedText.FromSimpleText (aiderPersonEntity.GetDisplayName () + ", nouvelle famille dans le RCH."));
							}
							else
							{
								this.eChPersonIdWithNewPerson.Add(aiderPersonEntity.eCH_Person.PersonId);
							}
						}
						else
						{
							this.CreateWarning (businessContext, existingAiderPersonEntity, existingAiderPersonEntity.ParishGroupPathCache, WarningType.EChPersonDuplicated, this.warningTitleMessage, FormattedText.FromSimpleText (existingAiderPersonEntity.GetDisplayName () + " existait déjà dans Aider."));
						}

					}
				});
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
						
						var aiderPersonEntity     = this.GetAiderPersonEntity (businessContext, eChReportedPerson.Adult1);

						if (aiderPersonEntity.IsNotNull ())
						{
							var personKey      = businessContext.DataContext.GetNormalizedEntityKey (aiderPersonEntity).Value;
							if (!aiderPersonEntity.Contacts.IsEmpty())
							{
								var aiderHousehold = this.GetAiderHousehold(businessContext, aiderPersonEntity);

								if (aiderHousehold.IsNotNull())
								{
									this.aiderPersonEntitiesWithDeletedHousehold.Add(personKey, aiderHousehold);
								}
							}
							
						}

						if (eChReportedPersonEntity.IsNotNull ())
						{
							businessContext.DeleteEntity (eChReportedPersonEntity.Address);
							businessContext.DeleteEntity (eChReportedPersonEntity);
						}					
					}
				});
		}

		private void CreateNewEChReportedPersons()
		{
			this.LogToConsole ("CreateNewEChReportedPersons()");
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
						var aiderPersonA1 = this.GetAiderPersonEntity (businessContext, eChPersonA1);
						if (aiderPersonA1.IsNull ())
						{
							var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
							aiderPersonEntity.eCH_Person = eChPersonA1;
						}

						//autoassign person to AiderHousehold if needed
						if (aiderPersonA1.Households.IsEmpty ())
						{
							var aiderHousehold = this.GetAiderHousehold (businessContext, eChPersonA1);
							if (aiderHousehold.IsNotNull ())
							{
								EChDataImporter.SetupHousehold (businessContext, aiderPersonA1, aiderHousehold, eChReportedPersonEntity, isHead1: true);
								this.eChPersonIdWithHouseholdSetupDone.Add (aiderPersonA1.eCH_Person.PersonId);
								ParishAssigner.AssignToParish (parishAddressRepository, businessContext, aiderPersonA1, this.startDate);
							}
						}
						if (this.eChPersonIdWithNewPerson.Contains (aiderPersonA1.eCH_Person.PersonId))
						{
							this.CreateWarning (businessContext, aiderPersonA1, aiderPersonA1.ParishGroupPathCache, WarningType.EChPersonNew, this.warningTitleMessage, FormattedText.FromSimpleText (aiderPersonA1.GetDisplayName () + ": nouveau dans le RCH."));
						}

						if (eChReportedPerson.Adult2 != null)
						{
							var eChPersonA2 = this.GetEchPersonEntity (businessContext, eChReportedPerson.Adult2);
							eChPersonA2.ReportedPerson1 = eChReportedPersonEntity;
							eChReportedPersonEntity.Adult2 = eChPersonA2;

							//autoassign person to AiderHousehold if needed
							var aiderPersonA2 = this.GetAiderPersonEntity (businessContext, eChPersonA2);
							if (aiderPersonA2.IsNull ())
							{
								var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
								aiderPersonEntity.eCH_Person = eChPersonA2;
							}
							if (aiderPersonA2.Households.IsEmpty ())
							{
								var aiderHousehold = this.GetAiderHousehold (businessContext, eChPersonA1);
								if (aiderHousehold.IsNotNull ())
								{
									EChDataImporter.SetupHousehold (businessContext, aiderPersonA2, aiderHousehold, eChReportedPersonEntity, isHead2: true);
									this.eChPersonIdWithHouseholdSetupDone.Add (aiderPersonA2.eCH_Person.PersonId);
									ParishAssigner.AssignToParish (parishAddressRepository, businessContext, aiderPersonA2, this.startDate);
								}
							}
							if (this.eChPersonIdWithNewPerson.Contains (aiderPersonA2.eCH_Person.PersonId))
							{
								this.CreateWarning (businessContext, aiderPersonA2, aiderPersonA2.ParishGroupPathCache, WarningType.EChPersonNew, this.warningTitleMessage, FormattedText.FromSimpleText (aiderPersonA2.GetDisplayName () + ": nouveau dans le RCH."));
							}
						}

						foreach (var eChChild in eChReportedPerson.Children)
						{
							var eChPersonC = this.GetEchPersonEntity (businessContext, eChChild);
							eChPersonC.ReportedPerson1 = eChReportedPersonEntity;
							eChReportedPersonEntity.Children.Add (eChPersonC);
							eChReportedPersonEntity.RemoveDuplicates ();

							//	Autoassign person to AiderHousehold if needed
							var aiderPersonC = this.GetAiderPersonEntity (businessContext, eChPersonC);
							if (aiderPersonC.Households.IsEmpty ())
							{
								var aiderHousehold = this.GetAiderHousehold (businessContext, eChPersonA1);
								if (aiderPersonC.IsNull ())
								{
									var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
									aiderPersonEntity.eCH_Person = eChPersonC;
								}
								if (aiderHousehold.IsNotNull ())
								{
									EChDataImporter.SetupHousehold (businessContext, aiderPersonC, aiderHousehold, eChReportedPersonEntity);
									this.eChPersonIdWithHouseholdSetupDone.Add (aiderPersonC.eCH_Person.PersonId);
									ParishAssigner.AssignToParish (parishAddressRepository, businessContext, aiderPersonC, this.startDate);
								}
								if (this.eChPersonIdWithNewPerson.Contains (aiderPersonC.eCH_Person.PersonId))
								{
									this.CreateWarning (businessContext, aiderPersonC, aiderPersonC.ParishGroupPathCache, WarningType.EChPersonNew, this.warningTitleMessage, FormattedText.FromSimpleText (aiderPersonC.GetDisplayName () + ": nouveau dans le RCH."));
								}
							}
						}

						//	Create warnings for the AiderPerson entity:

						var referenceAdult = eChReportedPersonEntity.Adult1;
						if (aiderPersonA1.IsNotNull ())
						{
							AiderHouseholdEntity oldHousehold;

							var key = businessContext.DataContext.GetNormalizedEntityKey (aiderPersonA1).Value;

							if (this.aiderPersonEntitiesWithDeletedHousehold.TryGetValue (key, out oldHousehold))
							{
								var warningMessage = FormattedText.FromSimpleText ("Un changement de composition a eu lieu dans le ménage.");

								this.CreateWarning (businessContext, aiderPersonA1, aiderPersonA1.ParishGroupPathCache, WarningType.EChHouseholdChanged, this.warningTitleMessage, warningMessage);
							}
							else
							{
								var warningMessage = FormattedText.FromSimpleText ("Cette personne a maintenant son propre ménage.");
								//Check AiderHousehold addresses
								if (!aiderPersonA1.Contacts.IsEmpty ())
								{
									var currentHouseholdAddress = aiderPersonA1.Contacts.Where (c => c.Household.IsNotNull ()).First ().Household.Address;
									if (currentHouseholdAddress.IsNotNull ())
									{
										//if the address or town is different put a warning
										if (!(eChAddressEntity.StreetUserFriendly == currentHouseholdAddress.StreetUserFriendly) || !(eChAddressEntity.SwissZipCodeId == currentHouseholdAddress.Town.SwissZipCodeId))
										{
											this.CreateWarning (businessContext, aiderPersonA1, aiderPersonA1.ParishGroupPathCache, WarningType.EChHouseholdAdded, this.warningTitleMessage, warningMessage);
										}
										else
										{
											//this person is at the same place -> do nothing
										}

									}
									else
									{
										this.CreateWarning (businessContext, aiderPersonA1, aiderPersonA1.ParishGroupPathCache, WarningType.EChHouseholdAdded, this.warningTitleMessage, warningMessage);
									}


								}
								else
								{
									this.CreateWarning (businessContext, aiderPersonA1, aiderPersonA1.ParishGroupPathCache, WarningType.EChHouseholdAdded, this.warningTitleMessage, warningMessage);
								}



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
			this.LogToConsole ("CreateNewAiderHouseholds()");
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

							//Setup household if needed
							if (!this.eChPersonIdWithHouseholdSetupDone.Contains (aiderPerson.eCH_Person.PersonId))
							{
								//we check that no equivalent (based on display name) member exist before doing the setup
								var nonEchEquivalentMemberExist = aiderHousehold.Members.Where (m => m.GetDisplayName ().Equals (aiderPerson.GetDisplayName ()) && m.IsGovernmentDefined == false).Any ();
								if (nonEchEquivalentMemberExist)
								{
									var nonEChmember = aiderHousehold.Members.Where (m => m.GetDisplayName ().Equals (aiderPerson.GetDisplayName ()) && m.IsGovernmentDefined == false).First ();

									if (nonEChmember.IsNotNull ())
									{
										var contact = nonEChmember.Contacts.Where (c => c.Household == aiderHousehold).First ();
										if (contact.IsNotNull ())
										{
											businessContext.DeleteEntity (contact);
										}
										businessContext.DeleteEntity (nonEChmember);
									}
								}

								EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isHead1: true);
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

							if (!this.eChPersonIdWithHouseholdSetupDone.Contains (aiderPerson.eCH_Person.PersonId))
							{
								EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity, isHead2: true);
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
							if (!this.eChPersonIdWithHouseholdSetupDone.Contains (aiderPerson.eCH_Person.PersonId))
							{
								EChDataImporter.SetupHousehold (businessContext, aiderPerson, aiderHousehold, eChReportedPersonEntity);
							}

						}
					}
				});
		}

		private void RelocateAndCreateNewAiderHousehold(BusinessContext businessContext,eCH_ReportedPersonEntity eChReportedPerson)
		{
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
			aiderAddressEntity.Town = this.GetAiderTownEntity (businessContext, eChReportedPerson.Address.SwissZipCode);


			//Link household to ECh Entity
			if (eChReportedPerson.Adult1.IsNotNull ())
			{
				var aiderPerson = this.GetAiderPersonEntity (businessContext, eChReportedPerson.Adult1);
				var oldHousehold = this.GetAiderHousehold (businessContext, aiderPerson);
				//Remove Old Contact
				if (oldHousehold.IsNotNull ())
				{
					var contactToRemove = aiderPerson.Contacts.Where (c => c.Household == oldHousehold).FirstOrDefault ();
					businessContext.DeleteEntity (contactToRemove);
				}

				//Setup household if needed
				if (!this.eChPersonIdWithHouseholdSetupDone.Contains (aiderPerson.eCH_Person.PersonId))
				{
					//we check that no equivalent (based on display name) member exist before doing the setup
					var nonEchEquivalentMemberExist = aiderHousehold.Members.Where (m => m.GetDisplayName ().Equals (aiderPerson.GetDisplayName ()) && m.IsGovernmentDefined == false).Any ();
					if (nonEchEquivalentMemberExist)
					{
						var nonEChmember = aiderHousehold.Members.Where (m => m.GetDisplayName ().Equals (aiderPerson.GetDisplayName ()) && m.IsGovernmentDefined == false).First ();

						if (nonEChmember.IsNotNull ())
						{
							var contact = nonEChmember.Contacts.Where (c => c.Household == aiderHousehold).First ();
							if (contact.IsNotNull ())
							{
								businessContext.DeleteEntity (contact);
							}
							businessContext.DeleteEntity (nonEChmember);
						}
					}

					AiderContactEntity.Create (businessContext, aiderPerson, aiderHousehold, true);
				}

			}

			if (eChReportedPerson.Adult2.IsNotNull ())
			{
				var aiderPerson = this.GetAiderPersonEntity (businessContext, eChReportedPerson.Adult2);
				var oldHousehold = this.GetAiderHousehold (businessContext, aiderPerson);
				//Remove Old Contact
				if (oldHousehold.IsNotNull ())
				{
					var contactToRemove = aiderPerson.Contacts.Where (c => c.Household == oldHousehold).FirstOrDefault ();
					businessContext.DeleteEntity (contactToRemove);
				}

				AiderContactEntity.Create (businessContext, aiderPerson, aiderHousehold, false);

			}

			foreach (var child in eChReportedPerson.Children)
			{
				var aiderPerson = this.GetAiderPersonEntity (businessContext, child);
				var oldHousehold = this.GetAiderHousehold (businessContext, aiderPerson);
				
				//Remove or not for child?
				//Remove Old Contact
				//if (oldHousehold.IsNotNull ())
				//{
				//	var contactToRemove = aiderPerson.Contacts.Where (c => c.Household == oldHousehold).FirstOrDefault ();
				//	businessContext.DeleteEntity (contactToRemove);
				//}

				AiderContactEntity.Create (businessContext, aiderPerson, aiderHousehold, false);

			}
			
		}
		

		private void ReassignAndWarnParish(BusinessContext businessContext, AiderPersonEntity aiderPersonEntity, List<FormattedText> changes)
		{
			var oldParishGroupPath = aiderPersonEntity.ParishGroupPathCache;

			ParishAssigner.ReassignToParish (this.parishAddressRepository, businessContext, aiderPersonEntity, this.startDate);

			var newParishGroupPath = aiderPersonEntity.ParishGroupPathCache;

			if (oldParishGroupPath != newParishGroupPath)
			{
				this.CreateWarning (businessContext, aiderPersonEntity, oldParishGroupPath, WarningType.ParishDeparture, this.warningTitleMessage, changes);
				this.CreateWarning (businessContext, aiderPersonEntity, newParishGroupPath, WarningType.ParishArrival, this.warningTitleMessage, changes);
			}
			else //if no change in parish group path, we create an simple address change warning
			{
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
			var aiderHouseholds = this.GetAiderHouseholds (businessContext, person).ToList ();

			if (aiderHouseholds.Any ())
			{
				foreach (var household in aiderHouseholds)
				{
					//	Only update households where this person is a head; children and
					//	other person won't affect the address of the household...

					if (household.IsHead (person))
					{
						this.UpdateAiderHouseholdAddress (businessContext, household, family);
						this.UpdateAiderSubscription (businessContext, household);
					}
				}

				person.Contacts.ForEach (x => x.RefreshCache ());
			}
			else
			{
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
	}
}
