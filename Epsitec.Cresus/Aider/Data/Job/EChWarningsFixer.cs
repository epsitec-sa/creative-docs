//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.Database;
using System.Data;
using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Aider.Enumerations;



namespace Epsitec.Aider.Data.Job
{
	internal static class EChWarningsFixer
	{
		public static void TryFixMissingSubscriptions(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				EChWarningsFixer.LogToConsole ("Try to fix Missing subscriptions...");

				EChWarningsFixer.FixSubscriptionMissingWarnings (businessContext);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		public static void TryFixUselessMissingSubscriptions(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				EChWarningsFixer.LogToConsole ("Fixing incorrect missing subscriptions...");
				EChWarningsFixer.FixSubscriptionWarnings (businessContext);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		public static void TryFixDepartureWarnings(CoreData coreData)
		{

			using (var businessContext = new BusinessContext (coreData, false))
			{
				EChWarningsFixer.LogToConsole ("Fixing useless departure warnings...");
				EChWarningsFixer.FixDepartureWarnings (businessContext);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

			}
		}

		public static void TryFixAll(CoreData coreData)
		{
			
			using (var businessContext = new BusinessContext (coreData, false))
			{
				EChWarningsFixer.LogToConsole ("Fix Reported Person Linkage For arrivals");

				EChWarningsFixer.FixReportedPersonLinkageForArrivals (businessContext);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

//				System.Console.WriteLine ("press ENTER to continue");
//				System.Console.ReadLine ();

				EChWarningsFixer.LogToConsole ("Migrating old Ech Warnings: EChPersonMissing -> EChProcessDeparture");
				
				EChWarningsFixer.MigrateWarning (businessContext, WarningType.EChPersonMissing, WarningType.EChProcessDeparture);

				EChWarningsFixer.LogToConsole ("Fixing old Ech Status from EChProcessDeparture Warnings");

				EChWarningsFixer.ResetEChStatusForProcessDepartureWarnings (businessContext);

				EChWarningsFixer.LogToConsole ("Migrating old Ech Warnings: EChPersonNew -> EChProcessArrival");

				EChWarningsFixer.MigrateWarning (businessContext, WarningType.EChPersonNew, WarningType.EChProcessArrival);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

				EChWarningsFixer.LogToConsole ("Delete old Ech Warnings: EChPersonDuplicated ");

				EChWarningsFixer.DeleteWarnings (businessContext, WarningType.EChPersonDuplicated);

				EChWarningsFixer.LogToConsole ("Delete old Ech Warnings: EChHouseholdChanged ");

				EChWarningsFixer.DeleteWarnings (businessContext, WarningType.EChHouseholdChanged);

				EChWarningsFixer.LogToConsole ("Delete old Ech Warnings: ParishMismatch ");

				EChWarningsFixer.DeleteWarnings (businessContext, WarningType.ParishMismatch);

				EChWarningsFixer.LogToConsole ("Delete old Ech Warnings: EChHouseholdMissing ");

				EChWarningsFixer.DeleteWarnings (businessContext, WarningType.EChHouseholdMissing);

				EChWarningsFixer.LogToConsole ("Delete old Ech Warnings: EChHouseholdAdded ");

				EChWarningsFixer.DeleteWarnings (businessContext, WarningType.EChHouseholdAdded);

				EChWarningsFixer.LogToConsole ("Delete warnings mismatch in time");

				EChWarningsFixer.RemoveWarningInTimeMismatch (businessContext);

				EChWarningsFixer.LogToConsole ("Detecting birth: EChProcessArrival -> PersonBirth");

				EChWarningsFixer.DetectBirthAndMigrate (businessContext);
				
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

				EChWarningsFixer.LogToConsole ("Detect, clean & merge duplicate childs in family for PersonBirth");

				EChWarningsFixer.DetectDuplicatedChildForBirths (businessContext);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		private static void FixDepartureWarnings(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessDeparture
			};

			var warnings = businessContext.GetByExample<AiderPersonWarningEntity> (example).ToArray ();

			foreach (var warning in warnings)
			{
				var arrivals = warning.Person.Warnings.Where (x => x.WarningType == WarningType.EChProcessArrival);

				if (arrivals.Any ())
				{
					System.Diagnostics.Debug.WriteLine ("Discard arrival and departure warnings for " + warning.Person.DisplayName);
					AiderPersonWarningEntity.Delete (businessContext, warning);
					AiderPersonWarningEntity.Delete (businessContext, arrivals.First ());
				}
			}
		}

		private static void FixSubscriptionWarnings(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.SubscriptionMissing
			};

			var warnings = businessContext.GetByExample<AiderPersonWarningEntity> (example).ToArray ();

			foreach (var warning in warnings)
			{
				bool remove = false;

				if (warning.Person.IsNull ())
				{
					remove = true;
				}
				if (warning.Person.Confession != PersonConfession.Protestant)
				{
					remove = true;
				}

				if (remove)
				{
					AiderPersonWarningEntity.Delete (businessContext, warning);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine ("Keep " + warning.Person.DisplayName);
				}
			}
		}

		private static void FixReportedPersonLinkageForArrivals(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessArrival
			};

			var membersInReportedPerson = new Dictionary<string, List<eCH_ReportedPersonEntity>> ();

			var allPersons            = businessContext.GetAllEntities<eCH_PersonEntity> ().ToList ();
			var allReportedPersons    = businessContext.GetAllEntities<eCH_ReportedPersonEntity> ().ToList ();
			var allArrivalsByPersonId = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example)
																   .Select (k => k.Person.eCH_Person)
																   .ToList ();
			var reportedPersonToCheck = new List<eCH_ReportedPersonEntity> ();
			//	Loading all persons before looping over the reported persons ensures that all
			//	entities are already in memory, thus suppressing the need to load each person
			//	individually...

			var total   = allReportedPersons.Count;
			var current = 1;

			foreach (var reportedPerson in allReportedPersons)
			{
				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;

				foreach (var member in reportedPerson.Members)
				{
					if (member.IsNotNull ())
					{
						List<eCH_ReportedPersonEntity> reportedPersonList;
						
						if (membersInReportedPerson.TryGetValue (member.PersonId, out reportedPersonList))
						{
							//System.Diagnostics.Debug.Assert (reportedPersonList.Count < 2);
							reportedPersonList.Add (reportedPerson);

							if (reportedPersonList.Count > 2)
							{
								System.Console.Clear ();
								EChWarningsFixer.LogToConsole ("////////////// {0}", member.GetCompactSummary ());
								EChWarningsFixer.LogToConsole ("RP Count: {0}", member.ReportedPersons.Count ());
								EChWarningsFixer.LogToConsole ("DS: {0}", member.DeclarationStatus);
								for (var i=0; i< reportedPersonList.Count (); i++)
								{
									EChWarningsFixer.LogToConsole ("RP{0}: Adult 1 {1}", i, reportedPersonList[i].Adult1.GetCompactSummary ());
									if(reportedPersonList[i].Adult2.IsNotNull ())
									{
										EChWarningsFixer.LogToConsole ("RP{0}: Adult 2 {1}", i, reportedPersonList[i].Adult2.GetCompactSummary ());
									}
									foreach (var child in reportedPersonList[i].Children)
									{
										EChWarningsFixer.LogToConsole ("RP{0}: Child {1} {2}", i, child.GetCompactSummary (), child.DeclarationStatus);
									}

									var duplicatedChild = reportedPersonList[i].Children.Where (c => c.PersonId == member.PersonId);
									EChWarningsFixer.LogToConsole ("Found {0} duplicated child", duplicatedChild.Count ());
									if (duplicatedChild.Count () > 1)
									{
										reportedPersonList[i].Children.Remove (member);
										EChWarningsFixer.LogToConsole ("Duplicated child removed");
									}

									if (reportedPersonList[i].Adult1 == member && member.PersonDateOfBirth.Value.Year > Date.Today.Year - 18)
									{
										businessContext.DeleteEntity (reportedPersonList[i]);
										EChWarningsFixer.LogToConsole ("Child as Adult mismatch Found, ReportedPerson removed");
									}
								}


								
//								System.Console.WriteLine ("press ENTER to continue");
//								System.Console.ReadLine ();
							}
						}
						else
						{
							reportedPersonList = new List<eCH_ReportedPersonEntity> ();
							reportedPersonList.Add (reportedPerson);

							membersInReportedPerson.Add (member.PersonId, reportedPersonList);
						}
					}
				}
			}

			foreach (var eChPerson in allArrivalsByPersonId)
			{
				if ((eChPerson.ReportedPerson1.IsNull ()) &&
					(eChPerson.ReportedPerson2.IsNull ()))
				{
					List<eCH_ReportedPersonEntity> reportedPersons;

					if (membersInReportedPerson.TryGetValue (eChPerson.PersonId, out reportedPersons))
					{
						eChPerson.ReportedPerson1 = reportedPersons[0];
						eChPerson.ReportedPerson2 = reportedPersons.Skip (1).FirstOrDefault ();
						EChWarningsFixer.LogToConsole ("eCh Person Corrected!");
					}
					else
					{
						EChWarningsFixer.LogToConsole ("{0} has no reported person\n Details:\n{1}\n{2}", eChPerson.PersonId, eChPerson.RemovalReason, eChPerson.DeclarationStatus);

						eChPerson.DeclarationStatus = PersonDeclarationStatus.Removed;

						var existingAiderPerson = EChWarningsFixer.GetAiderPersonEntity (businessContext, eChPerson);

						if (existingAiderPerson.IsNotNull ())
						{
							if (!existingAiderPerson.Warnings.Where (w => w.WarningType == WarningType.EChProcessDeparture).Any ())
							{
								EChWarningsFixer.LogToConsole ("Info: warning added: EChProcessDeparture");
								AiderPersonWarningEntity.Create (businessContext, existingAiderPerson, existingAiderPerson.ParishGroupPathCache, WarningType.EChProcessDeparture, TextFormatter.FormatText ("Correction du modèle ECh", TextFormatter.FormatText (existingAiderPerson.GetDisplayName (), "n'est plus dans le RCH.")));
							}
							else
							{
								EChWarningsFixer.LogToConsole ("Info: EChProcessDeparture warning already exist");
							}


							//Removing arrival warning
							var arrivalWarningToRemove = existingAiderPerson.Warnings.Where (w => w.WarningType == WarningType.EChProcessArrival);
							foreach (var warning in arrivalWarningToRemove)
							{
								EChWarningsFixer.LogToConsole ("Info: error detected: EChProcessArrival warning removed");
								businessContext.DeleteEntity (warning);
							}

						}

//						System.Console.WriteLine ("press ENTER to continue");
//						System.Console.ReadLine ();
					}	
				}
			}

			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
		}

		private static void FixSubscriptionMissingWarnings(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.SubscriptionMissing
			};

			var warningsToCheck = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example);

			var total = warningsToCheck.Count ();
			EChWarningsFixer.LogToConsole ("{0} SubscriptionMissing to check", total);

			var current = 1;
			foreach (var warning in warningsToCheck)
			{
				var person = warning.Person;
				if(person.MainContact.IsNotNull())
				{
					if (person.Address.Town.SwissCantonCode == "VD" && person.Confession == PersonConfession.Protestant)
					{
						var household = person.MainContact.Household;

						var existingSubscription = AiderSubscriptionEntity.FindSubscription (businessContext, household);

						if (existingSubscription.IsNull ())
						{
							AiderSubscriptionEntity.Create (businessContext, household);
						
						}

						businessContext.DeleteEntity (warning);
					}
				}

				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
				
			}
			System.Console.Clear ();
		}

		private static void ResetEChStatusForProcessDepartureWarnings(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessDeparture
			};

			var warningsToFix = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example);

			var total = warningsToFix.Count ();
			EChWarningsFixer.LogToConsole ("{0} EChStatus to Reset", total);

			var current = 1;
			foreach (var warning in warningsToFix)
			{
				warning.Person.eCH_Person.DeclarationStatus = PersonDeclarationStatus.Removed;
				warning.Person.eCH_Person.RemovalReason = RemovalReason.None;
				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static void RemoveWarningInTimeMismatch(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessArrival
			};

			var allArrivalsByPersonId = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example).ToLookup (k => k.Person.eCH_Person);

			example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessDeparture
			};

			var allDepartureByPersonId = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example).ToLookup (k => k.Person.eCH_Person);

			var personsWithMismatch = allDepartureByPersonId.Where (a => allArrivalsByPersonId.Contains (a.Key));

			foreach (var person in personsWithMismatch)
			{
				var personExample = new AiderPersonEntity ()
				{
					eCH_Person = person.Key
				};

				var personEntity = businessContext.DataContext.GetByExample<AiderPersonEntity> (personExample).FirstOrDefault ();
				EChWarningsFixer.LogToConsole ("/// {0}:", personEntity.GetFullName ());

				var warnings = personEntity.Warnings.OrderBy (w => w.StartDate);

				

				foreach (var warn in warnings)
				{
					EChWarningsFixer.LogToConsole ("{0} {1} detected",warn.Title, warn.WarningType);
				}

				var warningToDelete = warnings.Where(w => w.WarningType == WarningType.EChProcessDeparture || w.WarningType == WarningType.EChProcessArrival).First ();
				EChWarningsFixer.LogToConsole ("REMOVED {0} {1}", warningToDelete.Title, warningToDelete.WarningType);
				businessContext.DeleteEntity (warningToDelete);

				EChWarningsFixer.LogToConsole ("/////////////////");

			}			
		}

		private static void DeleteWarnings(BusinessContext businessContext, WarningType deleteValue)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = deleteValue
			};

			var warningsToDelete = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example);

			var total = warningsToDelete.Count ();
			EChWarningsFixer.LogToConsole ("{0} warnings to delete", total);

			var current = 1;
			foreach (var warn in warningsToDelete)
			{
				EChWarningsFixer.ClearWarningAndRefreshCaches (businessContext, warn);
				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static void DeleteEChHouseholdMissingWarnings (BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChHouseholdMissing
			};

			var warningsToDelete = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example);

			var total = warningsToDelete.Count ();
			EChWarningsFixer.LogToConsole ("{0} warnings to delete", total);

			var current = 1;
			foreach (var warn in warningsToDelete)
			{
				EChWarningsFixer.ClearWarningAndRefreshCaches (businessContext, warn);
				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static void MigrateWarning(BusinessContext businessContext, WarningType initialValue, WarningType migratedValue)
		{
			var example = new AiderPersonWarningEntity()
			{
				WarningType = initialValue
			};

			var warningsToMigrate = businessContext.DataContext.GetByExample<AiderPersonWarningEntity>(example);

			var total = warningsToMigrate.Count ();
			EChWarningsFixer.LogToConsole ("{0} warnings to migrate", total);

			var current = 1;
			foreach (var warn in warningsToMigrate)
			{
				warn.WarningType = migratedValue;
				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}",current, total);
				current++;
			}
			System.Console.Clear ();
		}


		private static void DetectBirthAndMigrate(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessArrival
			};

			var warningsToCheck = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example);

			var total = warningsToCheck.Count ();
			EChWarningsFixer.LogToConsole ("{0} warnings to check", total);

			var current = 1;
			foreach (var warn in warningsToCheck)
			{
				if (warn.Person.Age == 0)
				{
					if (warn.Person.MainContact.IsNotNull ())
					{
						var household = warn.Person.MainContact.Household;
						var anyOtherArrivalInHousehold = household.Members.Where (m => m != warn.Person).Any (m => m.Warnings.Any (w => w.WarningType == WarningType.EChProcessArrival));

						if (!anyOtherArrivalInHousehold)
						{
							warn.WarningType = WarningType.PersonBirth;
							EChWarningsFixer.LogToConsole ("{0} detected and migrated", warn.Person.GetCompactSummary ());
						}
					}
					else
					{
						
					}

				}

				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static void DetectDuplicatedChildForBirths(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.PersonBirth
			};

			var warningsToCheck = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example);

			var total = warningsToCheck.Count ();
			EChWarningsFixer.LogToConsole ("{0} warnings to check", total);

			var current = 1;
			foreach (var warn in warningsToCheck)
			{
				var members = warn.Person.MainContact.Household.Members;
				var potentialDuplicateChecker = new Dictionary<string, AiderPersonEntity> ();
				foreach (var person in members)
				{
					if (person.eCH_Person.PersonFirstNames.IsNullOrWhiteSpace () || person.Age > 0)
					{
						continue;
					}

					var checkKey =	person.BirthdayDay.ToString ()
							 +		person.BirthdayMonth.ToString ()
							 +		person.BirthdayYear.ToString ()
							 +		person.eCH_Person.PersonFirstNames.Split (",").First ();

					if (!potentialDuplicateChecker.ContainsKey (checkKey))
					{
						potentialDuplicateChecker.Add (checkKey, person);
					}
					else
					{
						var p1 = potentialDuplicateChecker[checkKey];
						var p2 = person;

						System.Console.Clear ();
						EChWarningsFixer.LogToConsole ("Family Members:\n");
						foreach (var p in members)
						{
							EChWarningsFixer.LogToConsole ("{0} {1} (Ech:{2})\n", p.GetSummary (), person.Age, p.IsGovernmentDefined);
						}

						EChWarningsFixer.LogToConsole ("Found! {0} {1} (Ech:{2})", person.GetSummary (),person.Age, person.IsGovernmentDefined);
						
						//System.Console.WriteLine ("press ENTER to continue");
						//System.Console.ReadLine ();

						//try to merge persons data
						if (!AiderPersonEntity.MergePersons (businessContext, p1, p2))
						{
							//Ech duplicated contact -> Remove the duplicated contact
							AiderContactEntity.Delete (businessContext, p2.MainContact);
						}
					}
				}

				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
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

		private static void ClearWarningAndRefreshCaches(BusinessContext businessContext, AiderPersonWarningEntity warning)
		{
			var person  = warning.Person;
			var context = businessContext;

			person.Contacts.ForEach (x => x.RefreshCache ());
			person.Households.ForEach (x => x.RefreshCache ());

			AiderPersonWarningEntity.Delete (context, warning);

			EChWarningsFixer.CleanUpEchPerson (person);
		}

		private static void CleanUpEchPerson(AiderPersonEntity person)
		{
			var reportedPersons = person.eCH_Person.ReportedPersons.ToArray ();

			foreach (var reportedPerson in reportedPersons)
			{
				reportedPerson.RemoveDuplicates ();
			}
		}

		private static System.Diagnostics.Stopwatch LogToConsole(string format, params object[] args)
		{
			var message = string.Format (format, args);

			if (message.StartsWith ("Error"))
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
			}

			System.Console.WriteLine ("WarningFixer: {0}", message);
			System.Console.ResetColor ();

			var time = new System.Diagnostics.Stopwatch ();

			time.Start ();

			return time;
		}
	}
}
