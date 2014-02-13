//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Loader;


using Epsitec.Common.Support.Extensions;

using System;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Types;
using Epsitec.Cresus.Database;
using System.Data;
using System.Collections.Generic;
using Epsitec.Cresus.DataLayer.Expressions;


namespace Epsitec.Aider.Data.Job
{
	/// <summary>
	/// This fixer removes the subscriptions and the subscription refusals that are empty, i.e.
	/// that don't target either an household or a legal person contact. These have appeared
	/// because for a while, the deletion of an household of a legal person did not delete the
	/// subscriptions or subscription refusals.
	/// </summary>
	public static class SubscriptionAndRefusalFixer
	{
		public static void FixParishGroupPath(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{

				var subscriptions = businessContext.GetAllEntities<AiderSubscriptionEntity> ().ToList ();
				var total = subscriptions.Count ();

				System.Console.Clear ();
				var current = 1;
				foreach (var subscription in subscriptions)
				{
					SubscriptionAndRefusalFixer.LogToConsole ("AiderSubscriptionEntity {0}/{1}", current, total);
					current++;
					subscription.RefreshCache ();
				}

				var subscriptionsRefusal = businessContext.GetAllEntities<AiderSubscriptionRefusalEntity> ().ToList ();
				total = subscriptionsRefusal.Count ();

				System.Console.Clear ();
				current = 1;
				foreach (var subscription in subscriptionsRefusal)
				{
					SubscriptionAndRefusalFixer.LogToConsole ("AiderSubscriptionRefusalEntity {0}/{1}", current, total);
					current++;
					subscription.RefreshCache ();
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}

		}

		public static void FixEmptySubscriptions(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var example = new AiderSubscriptionEntity ();
				var request = Request.Create (example);
				request.AddCondition (businessContext.DataContext, example, e => e.Household == null && e.LegalPersonContact == null);

				var emptySubscriptions = businessContext.DataContext.GetByRequest (request);

				foreach (var subscription in emptySubscriptions)
				{
					businessContext.DeleteEntity (subscription);
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}


		public static void FixEmptyRefusals(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var example = new AiderSubscriptionRefusalEntity ();
				var request = Request.Create (example);
				request.AddCondition (businessContext.DataContext, example, e => e.Household == null && e.LegalPersonContact == null);

				var emptyRefusals = businessContext.DataContext.GetByRequest (request);

				foreach (var refusal in emptyRefusals)
				{
					businessContext.DeleteEntity (refusal);
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}


		/// <summary>
		/// This Fixer Method can't run correctly without ad'hoc databases view's
		///  
		/// </summary>
		/// <param name="coreData"></param>
		public static void WarnHouseholdWithNoSubscription(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var jobDateTime    = System.DateTime.Now;
				var jobName        = "SubscriptionAndRefusalFixer.WarnHouseholdWithNoSubscription()";
				var jobDescription = string.Format ("Ménages sans abonnement à Bonne Nouvelle");

				var warningSource = AiderPersonWarningSourceEntity.Create (businessContext, jobDateTime, jobName, TextFormatter.FormatText (jobDescription));


				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;

				var sqlCommand = "select H1.id " +
									"from " +
									"HOUSEHOLDS H1 " + 
									"where " +
									"H1.id not in ( " +
									"select h2.id " +
									"from SUBSCRIPTIONS S1 " +
									"inner join HOUSEHOLDS H2 on s1.household_id = h2.id) " +
									"and " +
									"H1.id not in ( " +
									"select h3.id " +
									"from SUBSCRIPTIONREFUSALS S2 " +
									"inner join HOUSEHOLDS H3 on s2.household_id = h3.id);";

				var sqlBuilder = dbAbstraction.SqlBuilder;
				var command = sqlBuilder.CreateCommand (dbAbstraction.BeginReadOnlyTransaction (), sqlCommand);
				DataSet dataSet;
				sqlEngine.Execute (command, DbCommandType.ReturningData, 1, out dataSet);

				var householdIdsToCorrect = new List<DbId> ();
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					if (!row[0].ToString ().IsNullOrWhiteSpace ())
					{
						householdIdsToCorrect.Add (new DbId((long)row[0]));
					}
				}


				foreach (var householdId in householdIdsToCorrect)
				{
					var household = businessContext.DataContext.ResolveEntity<AiderHouseholdEntity> (new DbKey(householdId));
					if (household.Members.Count > 0)
					{
						var person = household.Members.Where (m => household.IsHead (m)).FirstOrDefault ();
						if (person.IsNull ())
						{
							person = household.Members.FirstOrDefault ();
						}

						AiderPersonWarningEntity.Create (businessContext, person, person.ParishGroupPathCache, WarningType.SubscriptionMissing, "Ménage sans abonnement", "Ce ménage n'est référencé ni dans les abonnements,\n" + "ni dans les refus.", warningSource);
					}
					else
					{
						AiderHouseholdEntity.Delete (businessContext, household);
					}
				}
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		/// <summary>
		/// This Fixer Method can't run correctly without ad'hoc databases view's
		///  
		/// </summary>
		/// <param name="coreData"></param>
		public static void CreateMissingSubscriptionsOrWarnHousehold(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var jobDateTime    = System.DateTime.Now;
				var jobName        = "SubscriptionAndRefusalFixer.WarnHouseholdWithNoSubscription()";
				var jobDescription = string.Format ("Ménages sans abonnement à Bonne Nouvelle");

				var warningSource = AiderPersonWarningSourceEntity.Create (businessContext, jobDateTime, jobName, TextFormatter.FormatText (jobDescription));


				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;

				var sqlCommand = "select H1.id " +
									"from " +
									"HOUSEHOLDS H1 " + 
									"where " +
									"H1.id not in ( " +
									"select h2.id " +
									"from SUBSCRIPTIONS S1 " +
									"inner join HOUSEHOLDS H2 on s1.household_id = h2.id) " +
									"and " +
									"H1.id not in ( " +
									"select h3.id " +
									"from SUBSCRIPTIONREFUSALS S2 " +
									"inner join HOUSEHOLDS H3 on s2.household_id = h3.id);";

				var sqlBuilder = dbAbstraction.SqlBuilder;
				var command = sqlBuilder.CreateCommand (dbAbstraction.BeginReadOnlyTransaction (), sqlCommand);
				DataSet dataSet;
				sqlEngine.Execute (command, DbCommandType.ReturningData, 1, out dataSet);

				var householdIdsToCorrect = new List<DbId> ();
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					if (!row[0].ToString ().IsNullOrWhiteSpace ())
					{
						householdIdsToCorrect.Add (new DbId ((long) row[0]));
					}
				}


				foreach (var householdId in householdIdsToCorrect)
				{
					var household = businessContext.DataContext.ResolveEntity<AiderHouseholdEntity> (new DbKey (householdId));
					if (household.Members.Count > 0)
					{
						var person = household.Members.Where (m => household.IsHead (m)).FirstOrDefault ();
						if (person.IsNull ())
						{
							person = household.Members.FirstOrDefault ();
						}

						if (person.Address.Town.SwissCantonCode == "VD" && person.Confession == PersonConfession.Protestant)
						{
							AiderSubscriptionEntity.Create (businessContext, household);
						}
						else
						{
							if (!person.Warnings.Any (w => w.WarningType == WarningType.SubscriptionMissing))
							{
								AiderPersonWarningEntity.Create (businessContext, person, person.ParishGroupPathCache, WarningType.SubscriptionMissing, "Ménage sans abonnement", "Ce ménage n'est référencé ni dans les abonnements,\n ni dans les refus.", warningSource);
							}
						}			
					}
					else
					{
						AiderHouseholdEntity.Delete (businessContext, household);
					}
				}
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		public static void DeleteSubscriptionsFromNumberToNumber(CoreData coreData, string fromNumber, string toNumber)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;

				var sqlCommand = "select S1.id " +
									"from SUBSCRIPTIONS S1 " +
									"where cast(SUB_NUMBER as integer) > "+fromNumber+" and cast(SUB_NUMBER as integer) <= "+ toNumber;

				var sqlBuilder = dbAbstraction.SqlBuilder;
				var command = sqlBuilder.CreateCommand (dbAbstraction.BeginReadOnlyTransaction (), sqlCommand);
				DataSet dataSet;
				sqlEngine.Execute (command, DbCommandType.ReturningData, 1, out dataSet);

				var subscriptionsIdsToCorrect = new List<DbId> ();
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					if (!row[0].ToString ().IsNullOrWhiteSpace ())
					{
						subscriptionsIdsToCorrect.Add (new DbId ((long) row[0]));
					}
				}

				foreach (var subsriptionId in subscriptionsIdsToCorrect)
				{
					var subscription = businessContext.DataContext.ResolveEntity<AiderSubscriptionEntity> (new DbKey (subsriptionId));
					if (subscription.SubscriptionType == SubscriptionType.Household)
					{
						if (subscription.Household.IsNotNull ())
						{
							if (!subscription.Household.Members.Any (m => m.Confession == PersonConfession.Protestant))
							{
								businessContext.DeleteEntity (subscription);
							}					
						}
						else
						{
							businessContext.DeleteEntity (subscription);
						}
					}
					else
					{
						businessContext.DeleteEntity (subscription);
					}
				}
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}


		private static void LogToConsole(string format, params object[] args)
		{
			var message = string.Format (format, args);
			
			System.Console.SetCursorPosition (0, 0);
			System.Console.WriteLine ("SubscriptionAndResusalFixer: " + message);
			System.Console.ResetColor ();
		}
	}
}
