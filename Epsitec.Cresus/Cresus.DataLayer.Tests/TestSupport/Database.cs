//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

namespace Epsitec.Cresus.DataLayer.TestSupport
{
	static class Database
	{
		public static DbInfrastructure Infrastructure
		{
			get
			{
				return Database.infrastructure;
			}
		}

		public static void DumpDataSet(System.Data.DataSet dataSet)
		{
			foreach (System.Data.DataTable table in dataSet.Tables)
			{
				System.Console.Out.WriteLine ("--------------------------------------");
				System.Console.Out.WriteLine ("Table {0}, {1} columns, {2} rows", table.TableName, table.Columns.Count, table.Rows.Count);

				foreach (System.Data.DataColumn column in table.Columns)
				{
					System.Console.Out.WriteLine ("- Column {0} : {1}{2}", column.ColumnName, column.DataType.Name, column.Unique ? ", unique" : "");
				}
				System.Console.Out.WriteLine ("--------------------------------------");
				foreach (System.Data.DataRow row in table.Rows)
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					foreach (object o in row.ItemArray)
					{
						if (buffer.Length > 0)
						{
							buffer.Append (", ");
						}
						buffer.Append (o == null ? "<null>" : o.ToString ());
					}
					System.Console.Out.WriteLine (buffer);
				}
				System.Console.Out.WriteLine ();
			}
		}
		
		public static DbInfrastructure NewInfrastructure()
		{
			if (Database.infrastructure != null)
			{
				Database.infrastructure.Dispose ();
				Database.infrastructure = null;
				System.Threading.Thread.Sleep (100);
			}

			for (int i = 0; i < 10; i++)
			{
				try
				{
					System.IO.File.Delete (Database.databasePath);
					break;
				}
				catch (System.IO.IOException ex)
				{
					System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting...", ex.ToString ());
					System.Threading.Thread.Sleep (1000);
				}
			}

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (DbInfrastructure.CreateDatabaseAccess ("FICHE"));
			}

			Database.infrastructure = new DbInfrastructure ();
			Database.infrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess ("FICHE"));

			return Database.infrastructure;
		}
		
		private static DbInfrastructure infrastructure;
		private const string databasePath = @"C:\Program Files\firebird\Data\Epsitec\FICHE.FIREBIRD";
	}
}
