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
	internal static class PersonsWithoutContactFixer
	{
		public static void TryFixAll(CoreData coreData)
		{
			
			using (var businessContext = new BusinessContext (coreData, false))
			{
				PersonsWithoutContactFixer.LogToConsole ("Perform DataQuality on Person without Contact");
				var jobDateTime    = System.DateTime.Now;
				var jobName        = "PersonWithoutContactFixer";
				var jobDescription = string.Format ("Qualité de données sur les personnes sans contacts");

				var warningSource = AiderPersonWarningSourceEntity.Create (businessContext, jobDateTime, jobName, TextFormatter.FormatText (jobDescription));
				var warningTitleMessage = TextFormatter.FormatText ("DataQuality Personne sans contact", jobDateTime.ToShortDateString ());

				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;

				var sqlCommand = "SELECT mud_lva.u_lva1 " +
								 "FROM mud_lvaf " +
								 "INNER JOIN mud_lva on mud_lvaf.u_lvau1 = mud_lva.cr_id " +
								 "LEFT OUTER JOIN mud_lvard ON mud_lvaf.cr_id = mud_lvard.u_lva5e " +
								 "WHERE mud_lvard.cr_id IS NULL " +
								 "ORDER BY mud_lva.u_lva2";


				var sqlBuilder = dbAbstraction.SqlBuilder;
				var command = sqlBuilder.CreateCommand (dbAbstraction.BeginReadOnlyTransaction (), sqlCommand);
				DataSet dataSet;
				sqlEngine.Execute (command, DbCommandType.ReturningData, 1, out dataSet);
				PersonsWithoutContactFixer.LogToConsole ("DataQuality SQL Results:");
				var personIdsToCorrect = new List<string> ();
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					if (!row[0].ToString ().IsNullOrWhiteSpace ())
					{
						personIdsToCorrect.Add (row[0].ToString ());
						PersonsWithoutContactFixer.LogToConsole (row[0] + " added");
					}
				}

				PersonsWithoutContactFixer.LogToConsole (personIdsToCorrect.Count + " persons without contacts detected");

				foreach (var eChPersonId in personIdsToCorrect)
				{
					//retreive AiderPerson
					var person = PersonsWithoutContactFixer.GetAiderPersonEntity (businessContext, eChPersonId);

					

					if (person.IsNull ())
					{
						PersonsWithoutContactFixer.LogToConsole ("No eCH person found for ID {0}", eChPersonId);
						continue;
					}

					//Preliminary check
					if (person.eCH_Person.DeclarationStatus == PersonDeclarationStatus.Declared 
						&& person.IsGovernmentDefined
						&& !person.IsDeceased)
					{
						
						//Advanced check
						if(person.eCH_Person.ReportedPerson1.IsNull () && person.eCH_Person.ReportedPerson2.IsNull ())
						{
							PersonsWithoutContactFixer.LogToConsole ("Poor Ech Data, skipping...");
							continue;
						}

						PersonsWithoutContactFixer.LogToConsole ("///// MISSING ECH PERSON DETECTED");


						var hasMultiReportedPerson	= person.eCH_Person.ReportedPerson2.IsNotNull ();
						var isSameReportedPerson	= person.eCH_Person.ReportedPerson1 == person.eCH_Person.ReportedPerson2;

						if (isSameReportedPerson)
						{
							PersonsWithoutContactFixer.LogToConsole ("Warning: Same ReportedPersons -> data corrected by removing second reported person");
							businessContext.DeleteEntity (person.eCH_Person.ReportedPerson2);
						}


						//Retreive person aider the first aider household
						var existingHousehold1 = PersonsWithoutContactFixer.GetAiderHousehold (businessContext, person.eCH_Person.ReportedPerson1.Adult1);
						if (existingHousehold1.IsNotNull ())
						{
							var isHead1					= person.eCH_Person.ReportedPerson1.Adult1 == person.eCH_Person;
							var isHead2					= person.eCH_Person.ReportedPerson1.Adult2 == person.eCH_Person;
							var isHead					= isHead1 || isHead2;

							AiderContactEntity.Create (businessContext, person, existingHousehold1, isHead);
							PersonsWithoutContactFixer.LogToConsole ("Corrected: {0} added to household n°1(Head:{1})", person.GetDisplayName (), existingHousehold1.GetHeadNames ().Item2.First ());

							if(isSameReportedPerson || !hasMultiReportedPerson)
							{
								continue;
							}
						}
						else
						{
							var reportedPerson  = person.eCH_Person.ReportedPerson1;

							var addressTemplate = EChDataHelpers.CreateAiderAddressEntityTemplate (businessContext, person.eCH_Person.ReportedPerson1);
							if (addressTemplate.StreetHouseNumberAndComplement.IsNullOrWhiteSpace ())
							{
								PersonsWithoutContactFixer.LogToConsole ("Warning address imcomplete for: {0}", person.GetDisplayName ());
							}

							var newHousehold	= AiderHouseholdEntity.Create (businessContext, addressTemplate);
							var isHead1					= person.eCH_Person.ReportedPerson1.Adult1 == person.eCH_Person;
							var isHead2					= person.eCH_Person.ReportedPerson1.Adult2 == person.eCH_Person;

							EChDataHelpers.SetupHousehold (businessContext, person, newHousehold, reportedPerson, isHead1, isHead2);
							AiderSubscriptionEntity.Create (businessContext, newHousehold);

							businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
							PersonsWithoutContactFixer.LogToConsole ("Household n°1 created for: {0}", person.GetDisplayName ());
						}

						if (!isSameReportedPerson && hasMultiReportedPerson)
						{
							var isHead1		= person.eCH_Person.ReportedPerson2.Adult1 == person.eCH_Person;
							var isHead2		= person.eCH_Person.ReportedPerson2.Adult2 == person.eCH_Person;
							var isHead		= isHead1 || isHead2;

							var existingHousehold2	= PersonsWithoutContactFixer.GetAiderHousehold (businessContext, person.eCH_Person.ReportedPerson2.Adult1);
							if (existingHousehold2.IsNotNull ())
							{
								AiderContactEntity.Create (businessContext, person, existingHousehold2, isHead);
								PersonsWithoutContactFixer.LogToConsole ("Corrected: {0} added to household n°2(Head:{1})", person.GetDisplayName (), existingHousehold2.GetHeadNames ().Item1);
							}
							else
							{
								var reportedPerson  = person.eCH_Person.ReportedPerson2;

								var addressTemplate = EChDataHelpers.CreateAiderAddressEntityTemplate (businessContext, person.eCH_Person.ReportedPerson2);
								if (addressTemplate.StreetHouseNumberAndComplement.IsNullOrWhiteSpace ())
								{
									PersonsWithoutContactFixer.LogToConsole ("Warning address imcomplete for: {0}", person.GetDisplayName ());
								}

								var newHousehold	= AiderHouseholdEntity.Create (businessContext, addressTemplate);

								EChDataHelpers.SetupHousehold (businessContext, person, newHousehold, reportedPerson, isHead1, isHead2);
								AiderSubscriptionEntity.Create (businessContext, newHousehold);

								businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
								PersonsWithoutContactFixer.LogToConsole ("Household n°1 created for: {0}", person.GetDisplayName ());
							}
						}
					}
					else
					{
						PersonsWithoutContactFixer.LogToConsole ("Skipping ECh status: {0}", person.eCH_Person.DeclarationStatus);
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

			System.Console.WriteLine ("PersonWithoutContactFixer: {0}", message);
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
