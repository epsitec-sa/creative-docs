using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Database;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Aider.Data.Job
{
	internal class SqlHelpers
	{
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
	}
}
