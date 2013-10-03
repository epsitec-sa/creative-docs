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
	internal static class ContactsFixer
	{
		public static void TryFixAll(CoreData coreData)
		{
			
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var jobDateTime    = System.DateTime.Now;
				var jobName        = "ContactsFixer.TryFixAll()";
				var jobDescription = string.Format ("Qualité de données sur les contacts");

				var warningSource = AiderPersonWarningSourceEntity.Create (businessContext, jobDateTime, jobName, TextFormatter.FormatText (jobDescription));
				var warningTitleMessage = TextFormatter.FormatText ("Qualité de donnée sur les contacts ", jobDateTime.ToShortDateString ());



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

				var personIdsToCorrect = new List<string> ();
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					if (!row[0].ToString ().IsNullOrWhiteSpace ())
					{
						personIdsToCorrect.Add (row[0].ToString ());
					}
				}

				foreach (var eChPersonId in personIdsToCorrect)
				{
					//retreive AiderPerson
					var person = ContactsFixer.GetAiderPersonEntity (businessContext, eChPersonId);

					if (person.IsNull ())
					{
						continue;
					}

					//Retreive person aider household
					var household = ContactsFixer.GetAiderHousehold (businessContext, person.eCH_Person.ReportedPerson1.Adult1);
					if (household.IsNotNull ())
					{
						AiderContactEntity.Create (businessContext, person, household, isHead: household.IsHead (person));
					}
					else //warn
					{
						var warningMessage = FormattedText.FromSimpleText ("Ménage a recréer (problème de qualité de données)");
						ContactsFixer.CreateWarning (businessContext, person, person.ParishGroupPathCache, WarningType.EChHouseholdAdded, warningTitleMessage, warningMessage, warningSource);
					}
				}
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
