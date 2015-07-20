//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.IO;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Loader;
using System.Linq;
using System.Data;
using Epsitec.Cresus.Database;
using System.Collections.Generic;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Aider.Data.ECh;

namespace Epsitec.Aider.Data.Job
{
	/// <summary>
	/// Various dataquality fixes for households
	/// </summary>
	internal static class HouseholdsFix
	{
		public static void EchHouseholdsQuality(CoreData coreData, string currentEchFile)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var dataContext = businessContext.DataContext;
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("////:::DATAQUALITY JOB STARTED::://///");
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// CLEAN BAD ECH HOUSEHOLDS");
				HouseholdsFix.SelectDbIds (
					businessContext, 
					"MUD_LVAG e","e.U_LVAH IS NULL AND e.U_LVAI IS NULL",
					(ids) => {
						foreach (var id in ids)
						{
							var entity = businessContext.DataContext.ResolveEntity<eCH_ReportedPersonEntity> (new DbKey (id));
							if (entity.Children.Count () == 0)
							{
								businessContext.DeleteEntity (entity);
								Logger.LogToConsole ("// DELETED");
							}
						}
					}
				);
				Logger.LogToConsole ("// Done!");
				HouseholdsFix.SaveChanges (businessContext);
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// CHECK PERSON WITH EMPTY HOUSEHOLDS");
				var fixedPersons = new List<eCH_PersonEntity> ();
				var unfixable    = new List<eCH_PersonEntity> ();
				var setupNeeded  = new Dictionary<AiderHouseholdEntity, List<eCH_PersonEntity>> ();
				var personWithEmptyHoushold = 0;
				HouseholdsFix.SelectColumn (
					businessContext,
					"a.PERSON_ECHID",
					"ECH_PERSON_WITH_HOUSEHOLDS a", "ECH_STATUS = 1 and a.HOUSEOLD1_ADULT1_CRID IS NULL",
					(personIds) =>
					{
						personWithEmptyHoushold = personIds.Count ();
						foreach (var personId in personIds)
						{
							var eCH_Person = new eCH_PersonEntity ()
							{
								PersonId = personId
							};

							var eCH_HouseholdCase1 = new eCH_ReportedPersonEntity ()
							{
								Adult1 = eCH_Person
							};

							var eCH_HouseholdCase2 = new eCH_ReportedPersonEntity ()
							{
								Adult2 = eCH_Person
							};

							var personToFix = businessContext.GetByExample<eCH_PersonEntity> (eCH_Person).FirstOrDefault ();
							var result = businessContext.GetByExample<eCH_ReportedPersonEntity> (eCH_HouseholdCase1).FirstOrDefault ();
							if (result.IsNotNull ())
							{							
								personToFix.ReportedPerson1 = result;
								HouseholdsFix.SetPersonAsFixed (fixedPersons, personToFix);
							}
							else
							{
								result = businessContext.GetByExample<eCH_ReportedPersonEntity> (eCH_HouseholdCase2).FirstOrDefault ();
								if (result.IsNotNull ())
								{
									personToFix.ReportedPerson1 = result;
									HouseholdsFix.SetPersonAsFixed (fixedPersons, personToFix);
								}
								else
								{
									HouseholdsFix.SelectColumn (
										businessContext,
										"a.AIDER_HOUSEHOLD_CRID",
										"ECH_PERSON_WITH_HOUSEHOLDS a", "a.PERSON_ECHID = '" + personId + "' and ECH_STATUS = 1",
										(householdIds) =>
										{
											var householdId = householdIds.FirstOrDefault ();
											if (householdId != null)
											{
												var dbkey     = new DbKey (HouseholdsFix.ToDbId (householdId));
												var household = businessContext.DataContext.ResolveEntity<AiderHouseholdEntity> (dbkey);
												if (household.IsNotNull ())
												{
													List<eCH_PersonEntity> members;
													if (setupNeeded.TryGetValue (household, out members))
													{
														members.Add (personToFix);
													}
													else
													{
														members = new List<eCH_PersonEntity> ();
														members.Add (personToFix);
														setupNeeded.Add (household, members);
													}					
												}
												else
												{
													unfixable.Add (personToFix);
												}
											}
										}
									);
								}
							}		 
						}
					}
				);
				Logger.LogToConsole ("// Done!");
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// QUALITY CHECK'UP");
				Logger.LogToConsole ("// Detected: " + personWithEmptyHoushold);
				Logger.LogToConsole ("// FixedPersons: " + fixedPersons.Count ());
				Logger.LogToConsole ("// SetupNeededForHousehold: " + setupNeeded.Count ());
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// LOADING ECH REPOSITORY FROM XML...");
				var echData    = EChDataLoader.Load (new System.IO.FileInfo (currentEchFile), int.MaxValue);
				var householdByAdult1   = echData.Where (h => h.Adult2 != null).ToDictionary (k => k.Adult1.Id, v => v);
				var householdByAdult2   = echData.Where (h => h.Adult2 != null).ToDictionary (k => k.Adult2.Id, v => v);
				var householdByChildren = echData.Where (h => h.Children.Count > 0).SelectMany (h => h.Children, (h, c) => new
				{
					household = h,
					id = c.Id
				}).ToLookup (k => k.id, v => v.household);
				Logger.LogToConsole ("// Done!");
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// SETUP AND FIX ECH HOUSEHOLDS");
				
				var childHasMoved   = new List<eCH_PersonEntity> ();
				var adultHasMoved   = new List<eCH_PersonEntity> ();
				var lostChilds      = new List<eCH_PersonEntity> ();
				var badEchStatus    = new List<eCH_PersonEntity> (); 
				foreach (var entry in setupNeeded)
				{
					var household    = entry.Key;
					var childToFix   = entry.Value;
					var heads        = household.Members.Where (m => household.IsHead (m) && m.eCH_Person.DeclarationStatus == Enumerations.PersonDeclarationStatus.Declared);
					var nonHeads     = household.Members.Where (m => !household.IsHead (m) && m.eCH_Person.DeclarationStatus == Enumerations.PersonDeclarationStatus.Declared);
					var familyKey    = HouseholdsFix.BuildFamilyKey (heads.Union (nonHeads).ToList ());

					if (heads.Any ())
					{
						foreach(var head in heads)
						{
							EChReportedPerson echHousehold = null;

							var headId = head.eCH_Person.PersonId;
							if (householdByAdult1.ContainsKey (headId))
							{
								echHousehold = householdByAdult1[headId];
							}
							else
							{
								if (householdByAdult2.ContainsKey (headId))
								{
									echHousehold = householdByAdult2[headId];
								}
								else
								{
									if (householdByChildren.Contains (headId))
									{
										echHousehold = householdByChildren[headId].ElementAtOrDefault (0);
									}
									else
									{
										badEchStatus.Add (head.eCH_Person);
										continue;
									}
								}
							}

							
							if (echHousehold != null)
							{
								var existingEchReportedPersonEntity = EChDataHelpers.GetEchReportedPersonEntity (businessContext, echHousehold);
								if (existingEchReportedPersonEntity != null)
								{
									foreach (var child in childToFix)
									{
										if (HouseholdsFix.ExecuteIfInEChHousehold (existingEchReportedPersonEntity, child.PersonId, (p) => child.ReportedPerson1 = existingEchReportedPersonEntity))
										{
											HouseholdsFix.SetPersonAsFixed (fixedPersons, child);
										}
										else
										{
											childHasMoved.Add (child);
										}
									}
								}
								else
								{
									var newReportedPersonEntity = HouseholdsFix.CreateEchReportedPersonEntity (businessContext, echHousehold);

									if (!HouseholdsFix.ExecuteIfInHousehold (household, echHousehold.Adult1.Id,
										(p) => EChDataHelpers.SetupHousehold (businessContext, p, household, newReportedPersonEntity, true, false)
									))
									{
										var aiderPerson = EChDataHelpers.GetEchPersonEntity (businessContext, echHousehold.Adult1);
										adultHasMoved.Add (aiderPerson);
									}

									if (echHousehold.Adult2 != null)
									{
										if (!HouseholdsFix.ExecuteIfInHousehold (household, echHousehold.Adult2.Id,
											(p) => EChDataHelpers.SetupHousehold (businessContext, p, household, newReportedPersonEntity, false, true)
										))
										{
											var aiderPerson = EChDataHelpers.GetEchPersonEntity (businessContext, echHousehold.Adult2);
											adultHasMoved.Add (aiderPerson);
										}
									}

									foreach (var child in childToFix)
									{
										if (!HouseholdsFix.ExecuteIfInEChHouseholdRepository (echHousehold, child.PersonId,
											(p) =>
											{
												var aiderPerson = EChDataHelpers.GetAiderPersonEntity (businessContext, p);
												EChDataHelpers.SetupHousehold (businessContext, aiderPerson, household, newReportedPersonEntity, false, false);
												HouseholdsFix.SetPersonAsFixed (fixedPersons, child);
											}
										))
										{
											childHasMoved.Add (child);
										}
									}

									foreach (var otherChild in echHousehold.Children.Where (c => !childToFix.Select (f => f.PersonId).Contains (c.Id)))
									{
										if (!HouseholdsFix.ExecuteIfInHousehold (household, otherChild.Id,
											(p) => EChDataHelpers.SetupHousehold (businessContext, p, household, newReportedPersonEntity, false, false)
										))
										{
											var aiderPerson = EChDataHelpers.GetEchPersonEntity (businessContext, otherChild);
											childHasMoved.Add (aiderPerson);
										}
									}
								}
							}
							else
							{
								//
							}
						}
					}
					else
					{
						foreach (var child in childToFix)
						{
							lostChilds.Add (child);
						}	
					}
				}


				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// QUALITY CHECK'UP");
				Logger.LogToConsole ("// Detected: " + personWithEmptyHoushold);
				Logger.LogToConsole ("// FixedPersons: " + fixedPersons.Count ());
				Logger.LogToConsole ("// Bad ECH: " + badEchStatus.Count ());
				Logger.LogToConsole ("// Unfixable: " + unfixable.Count ());
				HouseholdsFix.SaveChanges (businessContext);
				foreach (var person in badEchStatus)
				{
					switch (person.RemovalReason)
					{
						case Enumerations.RemovalReason.Unknown: 
						case Enumerations.RemovalReason.None:
							person.DeclarationStatus = Enumerations.PersonDeclarationStatus.Undefined;
							break;
						case Enumerations.RemovalReason.Departed:
						case Enumerations.RemovalReason.Deceased:
							person.DeclarationStatus = Enumerations.PersonDeclarationStatus.Removed;
							break;
						default:
							person.DeclarationStatus = Enumerations.PersonDeclarationStatus.NotDeclared;
							break;
					}

					HouseholdsFix.SetPersonAsFixed (fixedPersons, person);
				}

				var moves =  adultHasMoved.Where (p => p.DeclarationStatus == Enumerations.PersonDeclarationStatus.Declared)
					/**/			.Union (childHasMoved.Where (p => p.DeclarationStatus == Enumerations.PersonDeclarationStatus.Declared))
					/**/			.Union (lostChilds.Where (p => p.DeclarationStatus == Enumerations.PersonDeclarationStatus.Declared));
				var noEch =  adultHasMoved.Where (p => p.DeclarationStatus != Enumerations.PersonDeclarationStatus.Declared)
					/**/			.Union (childHasMoved.Where (p => p.DeclarationStatus != Enumerations.PersonDeclarationStatus.Declared))
					/**/			.Union (lostChilds.Where (p => p.DeclarationStatus != Enumerations.PersonDeclarationStatus.Declared));
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// QUALITY CHECK'UP");
				Logger.LogToConsole ("// Detected: " + personWithEmptyHoushold);
				Logger.LogToConsole ("// FixedPersons: " + fixedPersons.Count ());
				Logger.LogToConsole ("// Bad ECH: " + badEchStatus.Count ());
				Logger.LogToConsole ("// SetupNeededForHousehold: " + setupNeeded.Count ());
				Logger.LogToConsole ("// - HasMoved: " + moves.Count ());
				Logger.LogToConsole ("// - Non ECH: " + noEch.Count ());
				Logger.LogToConsole ("// Unfixable: " + unfixable.Count ());
				HouseholdsFix.SaveChanges (businessContext);
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// CHECK MOVES");
				var isChildOrAdult2 = new List<eCH_PersonEntity> ();
				foreach (var personToMove in moves)
				{
					var aiderPerson = EChDataHelpers.GetAiderPersonEntity (businessContext, personToMove);
					var echHousehold = echData.Where (h => h.FamilyKey.StartsWith (personToMove.PersonId)).FirstOrDefault ();
					if (echHousehold != null)
					{
						var existingEchReportedPersonEntity = EChDataHelpers.GetEchReportedPersonEntity (businessContext, echHousehold);
						if (existingEchReportedPersonEntity != null)
						{
							personToMove.ReportedPerson1 = existingEchReportedPersonEntity;
							HouseholdsFix.SetPersonAsFixed (fixedPersons, personToMove);
						}
						else
						{
							if (aiderPerson.eCH_Person.PersonId == echHousehold.Adult1.Id)
							{
								HouseholdsFix.CreateAiderHousehold (businessContext, aiderPerson, echHousehold, null);
								HouseholdsFix.SetPersonAsFixed (fixedPersons, personToMove);
							}
							else
							{
								isChildOrAdult2.Add (personToMove);
							}
							
						}
					}
					else
					{
						isChildOrAdult2.Add (personToMove);
					}
				}
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// QUALITY CHECK'UP");
				Logger.LogToConsole ("// FixedPersons: " + fixedPersons.Count ());
				Logger.LogToConsole ("// SetupNeededForHousehold: " + setupNeeded.Count ());
				Logger.LogToConsole ("// - HasMoved: " + moves.Count ());
				Logger.LogToConsole ("// |- IsChildOrAdult2: " + isChildOrAdult2.Count ());
				Logger.LogToConsole ("// - Non ECH: " + noEch.Count ());
				Logger.LogToConsole ("// Unfixable: " + unfixable.Count ());
				HouseholdsFix.SaveChanges (businessContext);
				foreach (var person in isChildOrAdult2)
				{
					if (householdByAdult2.ContainsKey (person.PersonId))
					{
						var echHousehold = householdByAdult2[person.PersonId];
						var existingEchReportedPersonEntity = EChDataHelpers.GetEchReportedPersonEntity (businessContext, echHousehold);
						if (existingEchReportedPersonEntity != null)
						{
							HouseholdsFix.FixPersonHousehold (businessContext, person, echHousehold, existingEchReportedPersonEntity, false, true);
							HouseholdsFix.SetPersonAsFixed (fixedPersons, person);
						}
						else
						{
							HouseholdsFix.CreateAiderHousehold (businessContext, null, echHousehold, null);
							HouseholdsFix.SetPersonAsFixed (fixedPersons, person);
						}
					}
					else
					{
						if(householdByChildren.Contains (person.PersonId))
						{
							if (householdByChildren[person.PersonId].ElementAtOrDefault (0) != null)
							{
								var echHousehold = householdByChildren[person.PersonId].ElementAtOrDefault (0);
								var existingEchReportedPersonEntity = EChDataHelpers.GetEchReportedPersonEntity (businessContext, echHousehold);
								if (existingEchReportedPersonEntity != null)
								{
									HouseholdsFix.FixPersonHousehold (businessContext, person, echHousehold, existingEchReportedPersonEntity, false, false);
									HouseholdsFix.SetPersonAsFixed (fixedPersons, person);
								}
								else
								{
									HouseholdsFix.CreateAiderHousehold (businessContext, null, echHousehold, null);
									HouseholdsFix.SetPersonAsFixed (fixedPersons, person);
								}
							}

							if (householdByChildren[person.PersonId].ElementAtOrDefault (1) != null)
							{
								var echHousehold = householdByChildren[person.PersonId].ElementAtOrDefault (1);
								var existingEchReportedPersonEntity = EChDataHelpers.GetEchReportedPersonEntity (businessContext,echHousehold);
								if (existingEchReportedPersonEntity != null)
								{
									HouseholdsFix.FixPersonHousehold (businessContext, person, echHousehold, existingEchReportedPersonEntity, false, false);
									HouseholdsFix.SetPersonAsFixed (fixedPersons, person);
								}
								else
								{
									HouseholdsFix.CreateAiderHousehold (businessContext, null, echHousehold, null);
									HouseholdsFix.SetPersonAsFixed (fixedPersons, person);
								}
							}
						}
					}
				}
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// QUALITY FINAL CHECK'UP");
				Logger.LogToConsole ("// Detected: " + personWithEmptyHoushold);
				Logger.LogToConsole ("// FixedPersons: " + fixedPersons.Count ());
				Logger.LogToConsole ("// Unfixable: " + unfixable.Count ());
				HouseholdsFix.SaveChanges (businessContext);
				Logger.LogToConsole ("~             DONE!             ~");
			}
		}

		public static void SetPersonAsFixed (List<eCH_PersonEntity> list, eCH_PersonEntity person)
		{
			list.Add (person);
			Logger.LogToConsole ("// Fixed!");
		}

		public static void FixPersonHousehold(BusinessContext businessContext, eCH_PersonEntity child, EChReportedPerson echHousehold, eCH_ReportedPersonEntity householdEntity, bool isHead1, bool isHead2)
		{
			var aiderChild   = EChDataHelpers.GetAiderPersonEntity (businessContext, child);
			var refPerson    = EChDataHelpers.GetAiderPersonEntity (businessContext, householdEntity.Adult1);
			if (refPerson.MainContact.IsNotNull ())
			{
				var newHousehold = EChDataHelpers.GetAiderHousehold (businessContext, refPerson);
				EChDataHelpers.SetupHousehold (businessContext, aiderChild, newHousehold, householdEntity, isHead1, isHead2);
			}
			else
			{
				HouseholdsFix.CreateAiderHousehold (businessContext, refPerson, echHousehold, householdEntity);
			}

		}

		public static AiderHouseholdEntity CreateAiderHousehold(BusinessContext businessContext, AiderPersonEntity refAiderPerson, EChReportedPerson echHousehold, eCH_ReportedPersonEntity householdEntity)
		{
			if (echHousehold == null)
			{
				throw new BusinessRuleException ("Cannot create household from missing ECH data");
			}

			if (refAiderPerson.IsNull ())
			{
				var refEchPerson  = EChDataHelpers.GetEchPersonEntity (businessContext, echHousehold.Adult1);
				refAiderPerson    = EChDataHelpers.GetAiderPersonEntity (businessContext, refEchPerson);
				if (refAiderPerson.IsNull ())
				{
					throw new BusinessRuleException ("Cannot create household: missing refAiderPerson");
				}
			}
			else
			{
				if (refAiderPerson.eCH_Person.PersonId != echHousehold.Adult1.Id)
				{
					throw new BusinessRuleException ("Cannot create household: bad refAiderPerson entity provided (id mismatch)");
				}
			}

			if (householdEntity.IsNull ())
			{
				householdEntity = HouseholdsFix.CreateEchReportedPersonEntity (businessContext, echHousehold);
			}

			var addressTemplate         = EChDataHelpers.CreateAiderAddressEntityTemplate (businessContext, householdEntity);
			var newHousehold            = AiderHouseholdEntity.Create (businessContext, addressTemplate);

			EChDataHelpers.SetupHousehold (businessContext, refAiderPerson, newHousehold, householdEntity, true, false);

			if (echHousehold.Adult2 != null)
			{
				var person2       = EChDataHelpers.GetEchPersonEntity (businessContext, echHousehold.Adult2);
				var aiderPerson2 = EChDataHelpers.GetAiderPersonEntity (businessContext, person2);
				EChDataHelpers.SetupHousehold (businessContext, aiderPerson2, newHousehold, householdEntity, false, true);
			}

			foreach (var child in echHousehold.Children)
			{
				var childPerson = EChDataHelpers.GetEchPersonEntity (businessContext, child);
				var aiderChild  = EChDataHelpers.GetAiderPersonEntity (businessContext, childPerson);
				EChDataHelpers.SetupHousehold (businessContext, aiderChild, newHousehold, householdEntity, false, false);
			}

			return newHousehold;
		}

		public static bool ExecuteIfInHousehold(AiderHouseholdEntity household, string personId, System.Action<AiderPersonEntity> action)
		{
			try
			{
				var member = household.Members.Where (m => m.eCH_Person.PersonId == personId).Single ();
				action (member);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool ExecuteIfInEChHousehold(eCH_ReportedPersonEntity household, string personId, System.Action<eCH_PersonEntity> action)
		{
			try
			{
				var member = household.Members.Where (m => m.PersonId == personId).Single ();
				action (member);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool ExecuteIfInEChHouseholdRepository(EChReportedPerson household, string personId, System.Action<EChPerson> action)
		{
			try
			{
				var members = household.Children.Union (household.Adult1.ToEnumerable ()).Union (household.Adult2.ToEnumerable ());
				var member = members.Where (m => m.Id == personId).Single ();
				action (member);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static eCH_ReportedPersonEntity CreateEchReportedPersonEntity(BusinessContext businessContext, EChReportedPerson eChReportedPerson)
		{
			var eChAddressEntity          = EChDataImporter.ImportEchAddressEntity (businessContext, eChReportedPerson.Address);
			var eChReportedPersonEntity   = businessContext.CreateAndRegisterEntity<eCH_ReportedPersonEntity> ();
			eChReportedPersonEntity.Address = eChAddressEntity;
			return eChReportedPersonEntity;
		}

		public static void SaveChanges (BusinessContext businessContext)
		{
			Logger.LogToConsole ("//////////////////////////////////////");
			Logger.LogToConsole ("// SAVING");
			businessContext.SaveChanges
			(
				LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
			);
			Logger.LogToConsole ("// Done!");
		}

		public static string BuildFamilyKey (IList<AiderPersonEntity> members)
		{
			var sortedIds = members
				.Select (m => m.eCH_Person.PersonId)
				.OrderBy (id => id);

			return string.Concat (sortedIds);
		}

		public static string BuildFamilyKey(IList<eCH_PersonEntity> members)
		{
			var sortedIds = members
				.Select (m => m.PersonId)
				.OrderBy (id => id);

			return string.Concat (sortedIds);
		}

		public static DbId ToDbId (string crid)
		{
			return new DbId (System.Convert.ToInt64 (crid));
		}

		public static void SelectDbIds (BusinessContext businessContext, string from, string where, System.Action<List<DbId>> action)
		{
			var db = businessContext.DataContext.DbInfrastructure;
			var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
			var sqlEngine = dbAbstraction.SqlEngine;
			var sqlCommand = "select CR_ID " +
							 "from " + from + " " +
							 "where " + where;

			var sqlBuilder = dbAbstraction.SqlBuilder;
			var command = sqlBuilder.CreateCommand (dbAbstraction.BeginReadOnlyTransaction (), sqlCommand);
			DataSet dataSet;
			sqlEngine.Execute (command, DbCommandType.ReturningData, 1, out dataSet);

			var ids = new List<DbId> ();
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				if (!row[0].ToString ().IsNullOrWhiteSpace ())
				{
					ids.Add (new DbId ((long) row[0]));
				}
			}

			if (ids.Any ())
			{
				action (ids);
			}
		}

		public static void SelectColumn(BusinessContext businessContext, string column, string from, string where, System.Action<List<string>> action)
		{
			var db = businessContext.DataContext.DbInfrastructure;
			var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
			var sqlEngine = dbAbstraction.SqlEngine;
			var sqlCommand = "select " + column +  " " +
							 "from " + from + " " +
							 "where " + where;

			var sqlBuilder = dbAbstraction.SqlBuilder;
			var command = sqlBuilder.CreateCommand (dbAbstraction.BeginReadOnlyTransaction (), sqlCommand);
			DataSet dataSet;
			sqlEngine.Execute (command, DbCommandType.ReturningData, 1, out dataSet);

			var values = new List<string> ();
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				if (!row[0].ToString ().IsNullOrWhiteSpace ())
				{
					values.Add (row[0].ToString ());
				}
			}

			if (values.Any ())
			{
				action (values);
			}
		}

		public static void RemoveHiddenPersonFromHouseholds(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				Logger.LogToConsole ("Removing hidden person from households...");
				Logger.LogToConsole ("~             START             ~");
				var personExample  = new AiderPersonEntity ();
				var request = new Request ()
				{
					RootEntity = personExample
				};

				request.AddCondition
				(
					businessContext.DataContext,
					personExample,
					p => p.Visibility != Enumerations.PersonVisibilityStatus.Default
				);

				var personsToCheck = businessContext.DataContext.GetByRequest<AiderPersonEntity> (request).ToArray ();

				foreach (var person in personsToCheck)
				{
					if (person.MainContact.IsNotNull ())
					{
						var household = person.MainContact.Household;

						if (household.IsNotNull ())
						{
							Logger.LogToConsole (string.Format ("Household members: {0}", household.GetMembersSummary ()));
							Logger.LogToConsole (string.Format ("removing: {0}", person.GetCompactSummary ()));
							Logger.LogToConsole (string.Format ("cause: {0}", person.Visibility));
							person.RemoveFromHousehold (businessContext, household);
							AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, household.ToEnumerable (), true);
							household.RefreshCache ();
							Logger.LogToConsole ("~             -next-             ~");
						}
					}
				}

				Logger.LogToConsole ("~             Saving...             ~");
				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);
				Logger.LogToConsole ("~             DONE!             ~");
			}
		}
	}
}
