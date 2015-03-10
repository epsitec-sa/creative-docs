//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	internal static class EChPersonFixer
	{
		public static void CreateMissingContact(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				
			}
		}

		public static void TryFixAll(CoreData coreData)
		{
			
			using (var businessContext = new BusinessContext (coreData, false))
			{
				EChPersonFixer.LogToConsole ("Perform DataQuality on EChPerson with same EChReportedPerson on slot 1 and 2");
				var jobDateTime    = System.DateTime.Now;
				var jobName        = "EChPersonFixer";
				var jobDescription = string.Format ("Qualité de données sur les personnes ECh");

				var warningSource = AiderPersonWarningSourceEntity.Create (businessContext, jobDateTime, jobName, TextFormatter.FormatText (jobDescription));
				var warningTitleMessage = TextFormatter.FormatText ("DataQuality Personne ECH", jobDateTime.ToShortDateString ());

				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;

				var sqlCommand = "select CR_ID " +
								 "from MUD_LVA p " +
								 "where " +
								 "p.U_LVAH2 IS NOT NULL " + 
								 "AND p.U_LVAG2 = p.U_LVAH2";


				var sqlBuilder = dbAbstraction.SqlBuilder;
				var command = sqlBuilder.CreateCommand (dbAbstraction.BeginReadOnlyTransaction (), sqlCommand);
				DataSet dataSet;
				sqlEngine.Execute (command, DbCommandType.ReturningData, 1, out dataSet);

				var personIdsToCorrect = new List<DbId> ();
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					if (!row[0].ToString ().IsNullOrWhiteSpace ())
					{
						personIdsToCorrect.Add (new DbId ((long) row[0]));
					}
				}

				EChPersonFixer.LogToConsole (personIdsToCorrect.Count + " persons with same ReportedPerson 1 and 2");

				foreach (var eChPersonId in personIdsToCorrect)
				{
					//retreive eChPerson
					var person = businessContext.DataContext.ResolveEntity<eCH_PersonEntity> (new DbKey (eChPersonId));

					if (person.IsNull ())
					{
						EChPersonFixer.LogToConsole ("No eCH person found for ID {0}", eChPersonId);
						continue;
					}

					//fix secondary ReportedPerson
					person.ReportedPerson2 = null;

					var aiderPerson = EChPersonFixer.GetAiderPersonEntity (businessContext, person.PersonId);

					if (aiderPerson.IsNull ())
					{
						EChPersonFixer.LogToConsole ("No Aider person found for ID {0}", eChPersonId);
						continue;
					}

					//Retreive main aider household
					var household = EChPersonFixer.GetAiderHousehold (businessContext, person.ReportedPerson1.Adult1);

					if (household.IsNotNull ())
					{
						//verify that a contact exist
						if (!household.Contacts.Any (c => c.Person.eCH_Person.PersonId == person.PersonId))
						{
							
							EChPersonFixer.LogToConsole ("No contact found for ID {0} in household {1} -> Adding contact", eChPersonId, household.GetCompactSummary ());
							if (!person.IsDeceased)
							{
								//We need to create a contact
								AiderContactEntity.Create (businessContext, aiderPerson, household, isHead: household.IsHead (aiderPerson));
							}				
						}
						else
						{
							//too many contact exist?
							if (household.Contacts.Count (c => c.Person.eCH_Person.PersonId == person.PersonId) > 1)
							{
								EChPersonFixer.LogToConsole ("Too many contacts found for ID {0} in household {1} -> Removing contacts, leave only one", eChPersonId, household.GetCompactSummary ());
								
								var duplicateContacts = household.Contacts.Where (c => c.Person.eCH_Person.PersonId == person.PersonId).ToList ();
								AiderContactEntity.DeleteDuplicateContacts (businessContext, duplicateContacts);
							}
						}
					}
					else
					{
						var warningMessage = FormattedText.FromSimpleText ("No household found -> Use PersonWithoutContactFixer job to fix");

					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		private static AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, string eChPersonId)
		{
			var personExample = new AiderPersonEntity ();

			personExample.eCH_Person = new eCH_PersonEntity ()
			{
				PersonId = eChPersonId
			};

			return businessContext.DataContext.GetByExample<AiderPersonEntity> (personExample).FirstOrDefault ();
		}

		private static AiderHouseholdEntity GetAiderHousehold(BusinessContext businessContext, eCH_PersonEntity refPerson)
		{
			if (refPerson.IsNull ())
			{
				return null;
			}

			var personExample = new AiderPersonEntity ();
			var contactExample = new AiderContactEntity ();
			var householdExample = new AiderHouseholdEntity ();
			personExample.eCH_Person = refPerson;
			contactExample.Person = personExample;
			contactExample.Household = householdExample;
			var request = new Request ()
			{
				RootEntity = contactExample,
				RequestedEntity = householdExample
			};

			return businessContext.DataContext.GetByRequest<AiderHouseholdEntity> (request).FirstOrDefault ();
		}

		private static System.Diagnostics.Stopwatch LogToConsole(string format, params object[] args)
		{
			var message = string.Format (format, args);

			if (message.StartsWith ("Error"))
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
			}

			System.Console.WriteLine ("ContactFixer: {0}", message);
			System.Console.ResetColor ();

			var time = new System.Diagnostics.Stopwatch ();

			time.Start ();

			return time;
		}

		private static void CreateWarning(BusinessContext context, AiderPersonEntity person, string parishGroupPath,
								   WarningType warningType, FormattedText title, FormattedText description, AiderWarningSourceEntity source)
		{
			var personId = person.eCH_Person.PersonId;

			if (string.IsNullOrEmpty (personId))
			{
				personId = context.DataContext.GetNormalizedEntityKey (person).ToString ();
			}

			AiderPersonWarningEntity.Create (context, person, parishGroupPath, warningType, title, description, source);
		}
	}
}
