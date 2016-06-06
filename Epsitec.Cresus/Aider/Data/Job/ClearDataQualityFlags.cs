using Epsitec.Common.IO;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Database;
using System.Data;

namespace Epsitec.Aider.Data.Job
{

	internal static class ClearDataQualityFlags
	{
		public static void Run(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				Logger.LogToConsole ("Clearing flags...");

				var db = businessContext.DataContext.DbInfrastructure;
				var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
				var sqlEngine = dbAbstraction.SqlEngine;

				var sqlCommand = "UPDATE mud_lvard SET mud_lvard.u_lvoh93 = ''";

				var sqlBuilder = dbAbstraction.SqlBuilder;

				using (IDbTransaction transaction = dbAbstraction.BeginReadWriteTransaction ())
				{
					var command = sqlBuilder.CreateCommand (transaction, sqlCommand);
					command.Transaction = transaction;

					int result;
					sqlEngine.Execute (command, DbCommandType.NonQuery, 1, out result);

					Logger.LogToConsole (string.Format ("DataQuality SQL Results: {0} -> commit", result));
					transaction.Commit ();
					Logger.LogToConsole ("Cleared!");

				}
			}
		}
	}

}
