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
	internal static class UpdateLegalPerson
	{
		public static void AdminToMunicipality(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				Logger.LogToConsole ("Update AdminToMunicipality -> Fetch contacts to refresh");

				List<AiderContactEntity> contactsToRefresh = new List<AiderContactEntity> ();
				var where = "a.U_LVAV8 LIKE 'Administration%ommunale%' " +
							"AND a.U_LVAEF not in (SELECT b.U_LVAEF FROM MUD_LVAR6 b WHERE b.U_LVAV8 LIKE 'Municipalité%')";
				SqlHelpers.SelectDbIds (
					businessContext,
					"MUD_LVAR6 a",
					where,
					(crids) =>
					{
						foreach (var id in crids)
						{
							var entity = businessContext.DataContext.ResolveEntity<AiderLegalPersonEntity> (new DbKey (id));
							contactsToRefresh.AddRange (entity.Contacts);
						}
					}
				);

				Logger.LogToConsole ("Update AdminToMunicipality -> Apply update");
				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;


				var sqlCommand = 
								"UPDATE MUD_LVAR6 u " +
								"SET u.U_LVAV8 = 'Municipalité' " + 
								"where u.CR_ID IN " +  
								"(SELECT a.CR_ID " +
								"FROM " +
								"MUD_LVAR6 a " +
								"WHERE " + 
								"a.U_LVAV8 LIKE 'Administration%ommunale%' " +
								"AND a.U_LVAEF not in (SELECT b.U_LVAEF FROM MUD_LVAR6 b WHERE b.U_LVAV8 LIKE 'Municipalité%')); ";


				var sqlBuilder = dbAbstraction.SqlBuilder;


				using (IDbTransaction transaction = dbAbstraction.BeginReadWriteTransaction ())
				{
					var command = sqlBuilder.CreateCommand (transaction, sqlCommand);
					command.Transaction = transaction;

					int result;
					sqlEngine.Execute (command, DbCommandType.NonQuery, 1, out result);

					Logger.LogToConsole (string.Format ("Update SQL Results: {0} -> commit", result));
					transaction.Commit ();
				}

				Logger.LogToConsole ("Update AdminToMunicipality -> Refresh contacts");
				foreach(var contact in contactsToRefresh)
				{
					contact.RefreshCache ();
					contact.RefreshRoleCache (businessContext.DataContext);
					Logger.LogToConsole (contact.DisplayName);
				}

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.None);
				Logger.LogToConsole ("Update AdminToMunicipality -> Job Done!");
			}
		}

	}


}
