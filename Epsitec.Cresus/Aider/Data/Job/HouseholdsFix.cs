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

namespace Epsitec.Aider.Data.Job
{
	/// <summary>
	/// Various dataquality fixes for households
	/// </summary>
	internal static class HouseholdsFix
	{
		public static void EchHouseholdsQuality(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var dataContext = businessContext.DataContext;

				Logger.LogToConsole ("Ech Households Quality Job");
				Logger.LogToConsole ("~             START             ~");


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
				var unfixed = new List<eCH_PersonEntity> ();
				Logger.LogToConsole ("//////////////////////////////////////");
				Logger.LogToConsole ("// CHECK ECH PERSONS HOUSEHOLDS");
				HouseholdsFix.SelectColumn (
					businessContext,
					"a.PERSON_ECHID",
					"ECH_PERSON_WITH_HOUSEHOLDS a", "a.IS_HEAD = 0 and ECH_STATUS = 1 and a.HOUSEOLD1_ADULT1_PERSON IS NULL",
					(personIds) =>
					{
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
								Logger.LogToConsole ("// Fixed!");
							}
							else
							{
								result = businessContext.GetByExample<eCH_ReportedPersonEntity> (eCH_HouseholdCase2).FirstOrDefault ();
								if (result.IsNotNull ())
								{
									personToFix.ReportedPerson1 = result;
									Logger.LogToConsole ("// Fixed!");
								}
								else
								{
									HouseholdsFix.SelectColumn (
										businessContext,
										"a.PERSON_ECHID",
										"ECH_PERSON_WITH_HOUSEHOLDS a", "a.IS_HEAD = 0 and ECH_STATUS = 1 and a.HOUSEOLD1_ADULT1_PERSON IS NULL",
										(personIds) =>
										{
										}
									);
									var parentHousehold = new eCH_ReportedPersonEntity ();
									var request = new Request ()
									{
										RootEntity = parentHousehold,
										RequestedEntity = parentHousehold
									};
									request.AddCondition (dataContext, parentHousehold, h => SqlMethods.IsInSet (personToFix, h.Children));
									result = businessContext.GetByRequest<eCH_ReportedPersonEntity> (request).FirstOrDefault ();
									if (result.IsNotNull ())
									{
										personToFix.ReportedPerson1 = result;
										Logger.LogToConsole ("// Fixed!");
									}
									else
									{
										Logger.LogToConsole ("// Nothing to do... :/");
										// Ouch
									}
								}
							}
							 

						}
					}
				);

				Logger.LogToConsole ("~             Saving...             ~");
				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);
				Logger.LogToConsole ("~             DONE!             ~");
			}
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

			action (ids);
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

			action (values);
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
