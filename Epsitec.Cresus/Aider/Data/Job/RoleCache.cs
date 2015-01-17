using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Common.IO;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Epsitec.Aider.Helpers;
using System.Data;
using Epsitec.Cresus.Database;


namespace Epsitec.Aider.Data.Job
{
	internal static class RoleCache
	{
		public static void InitBaseSet(CoreData coreData)
		{
			RoleCache.DisableForAll (coreData);

			using (var businessContext = new BusinessContext (coreData, false))
			{
				Logger.LogToConsole ("RoleCache -> Init Base RoleCache Set");

				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;

				var sqlCommand = 
								"UPDATE mud_lva73 SET mud_lva73.u_lvop33 = 0 " +
								"WHERE mud_lva73.cr_id IN "  +
								"(SELECT part.cr_id " +
								"FROM mud_lva73 part, " +
								"mud_lva54 grp, " +
								"mud_lvard contact " +
								"WHERE NOT (grp.u_lvapc LIKE 'R___.P___.' " +
								"OR grp.u_lvapc LIKE 'R___.P___.D006.%' " +
								"OR grp.u_lvapc LIKE 'R___.P___.D002.D010.%' " +
								"OR grp.u_lvapc LIKE 'D002.D005.%' " +
								"OR grp.u_lvapc = 'NOPA.') " +
								"AND grp.cr_id = part.U_LVA84 " +
								"AND contact.cr_id = part.U_LVAIG " +
								"AND part.u_lvao3 IS NULL); ";


				var sqlBuilder = dbAbstraction.SqlBuilder;


				using (IDbTransaction transaction = dbAbstraction.BeginReadWriteTransaction ())
				{
					var command = sqlBuilder.CreateCommand (transaction, sqlCommand);
					command.Transaction = transaction;

					int result;
					sqlEngine.Execute (command, DbCommandType.NonQuery, 1, out result);

					Logger.LogToConsole (string.Format ("DataQuality SQL Results: {0} -> commit", result));
					transaction.Commit ();
					Logger.LogToConsole ("Initialized!");

				}
			}

			RoleCache.Build (coreData);
		}

		public static void 
			DisableForAll(CoreData coreData)
		{

			using (var businessContext = new BusinessContext (coreData, false))
			{
				Logger.LogToConsole ("RoleCache -> Disable RoleCache for all participations");

				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;

				var sqlCommand = "UPDATE mud_lva73 " +
								 "SET mud_lva73.u_lvop33 = 1" +
								 "WHERE mud_lva73.u_lvop33 = 0";


				var sqlBuilder = dbAbstraction.SqlBuilder;


				using (IDbTransaction transaction = dbAbstraction.BeginReadWriteTransaction ())
				{
					var command = sqlBuilder.CreateCommand (transaction, sqlCommand);
					command.Transaction = transaction;

					int result;
					sqlEngine.Execute (command, DbCommandType.NonQuery, 1, out result);

					Logger.LogToConsole (string.Format ("DataQuality SQL Results: {0} -> commit", result));
					transaction.Commit ();
					Logger.LogToConsole ("Done!");

				}
			}
		}

		public static void Purge(CoreData coreData)
		{

			using (var businessContext = new BusinessContext (coreData, false))
			{
				Logger.LogToConsole ("RoleCache -> Purge");

				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;

				var sqlCommand = "UPDATE mud_lva73 " +
								 "SET mud_lva73.u_lvoo33 = '' " +
								 "WHERE mud_lva73.u_lvop33 = 0";


				var sqlBuilder = dbAbstraction.SqlBuilder;


				using (IDbTransaction transaction = dbAbstraction.BeginReadWriteTransaction ())
				{
					var command = sqlBuilder.CreateCommand (transaction, sqlCommand);
					command.Transaction = transaction;

					int result;
					sqlEngine.Execute (command, DbCommandType.NonQuery, 1, out result);

					Logger.LogToConsole (string.Format ("DataQuality SQL Results: {0} -> commit", result));
					transaction.Commit ();
					Logger.LogToConsole ("Purged!");
					
				}


			}
		}


		public static void Build(CoreData coreData)
		{
			Logger.LogToConsole ("START BUILD ROLE CACHE");

			AiderEnumerator.ParticipationRoleCache (coreData, RoleCache.BuildBatch);

			Logger.LogToConsole ("DONE BUILD ROLE CACHE ");
		}


		private static void BuildBatch
		(
			BusinessContext businessContext,
			IEnumerable<AiderGroupParticipantEntity> participations)
		{
			Logger.LogToConsole ("START BATCH");

			foreach (var participation in participations)
			{
				if (participation.Contact.IsNotNull ())
				{
					var role		= AiderParticipationsHelpers.BuildRoleFromParticipation (participation)
																.GetRole (participation);

					var rolePath	= AiderParticipationsHelpers.GetRolePath (participation);

					participation.RoleCache		= role;
					participation.RolePathCache = rolePath;

					Logger.LogToConsole (string.Format ("{0} is {1} ({2})", participation.Contact.DisplayName, role, rolePath));
				}
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			Logger.LogToConsole ("DONE BATCH");
		}


	}


}
