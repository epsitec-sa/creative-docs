using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Database;
using Epsitec.Common.Support.Extensions;
using Epsitec.Aider.Properties;

namespace Epsitec.Aider.Data.Job
{
	internal class SqlHelpers
	{
		public static bool ViewExist(BusinessContext businessContext, string viewName)
		{   
			var command = 
				"SELECT DISTINCT v.RDB$VIEW_NAME as name " +
				"FROM RDB$VIEW_RELATIONS v " +
				"WHERE v.RDB$VIEW_NAME = '" + viewName + "';";
			var exist   = false;
			SqlHelpers.Select (businessContext, command, (result) => exist = result.Any ());
			return exist;
		}

		public static void SelectDbIds(BusinessContext businessContext, string from, string where, System.Action<List<DbId>> action)
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

			if (ids.Any ())
			{
				action (ids);
			}
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

			if (values.Any ())
			{
				action (values);
			}
		}

		public static void Select(BusinessContext businessContext, string sqlCommand, System.Action<List<string>> action)
		{
			var db = businessContext.DataContext.DbInfrastructure;
			var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
			var sqlEngine = dbAbstraction.SqlEngine;
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

			if (values.Any ())
			{
				action (values);
			}
		}

		public static int CommitTransaction (BusinessContext businessContext, string sqlCommand)
		{
			var db = businessContext.DataContext.DbInfrastructure;
			var dbAbstraction = DbFactory.CreateDatabaseAbstraction (db.Access);
			var sqlEngine = dbAbstraction.SqlEngine;
			var sqlBuilder = dbAbstraction.SqlBuilder;

			using (IDbTransaction transaction = dbAbstraction.BeginReadWriteTransaction ())
			{
				var command = sqlBuilder.CreateCommand (transaction, sqlCommand);
				command.Transaction = transaction;

				int result;
				sqlEngine.Execute (command, DbCommandType.NonQuery, 1, out result);
				transaction.Commit ();
				return result;
			}			
		}
	}
}
