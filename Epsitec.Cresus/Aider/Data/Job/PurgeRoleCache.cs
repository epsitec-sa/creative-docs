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
	internal static class PurgeRoleCache
	{
		public static void Purge(CoreData coreData)
		{
			
			using (var businessContext = new BusinessContext (coreData, false))
			{
				PurgeRoleCache.LogToConsole ("RoleCache -> Purge");

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
					sqlEngine.Execute (command, DbCommandType.NonQuery , 1, out result);

					transaction.Commit ();
					PurgeRoleCache.LogToConsole ("DataQuality SQL Results: {0}", result);
				}

				
			}
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
	}
}
