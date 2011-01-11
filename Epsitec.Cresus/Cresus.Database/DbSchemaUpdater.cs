﻿using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database
{
	
	
	public static class DbSchemaUpdater
	{


		public static void UpdateSchema(DbInfrastructure dbInfrastructure, IEnumerable<DbTable> newSchema)
		{
			using (DbTransaction dbTransaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				var dbTablesIn = DbSchemaUpdater.GetDbTablesIn (dbInfrastructure, dbTransaction).ToList ();
				var dbTablesOut = DbSchemaUpdater.GetDbTablesOut (dbInfrastructure, newSchema).ToList ();

				var dbTypeDefsIn = DbSchemaUpdater.GetDbTypeDefsOfSchema (dbInfrastructure, dbTablesIn).ToList ();
				var dbTypeDefsOut = DbSchemaUpdater.GetDbTypeDefsOfSchema (dbInfrastructure, dbTablesOut).ToList ();

				var dbTypeDefsToAdd = DbSchemaUpdater.GetDbTypeDefsToAdd (dbInfrastructure, dbTypeDefsIn, dbTypeDefsOut).ToList ();
				var dbTypeDefsToAlter = DbSchemaUpdater.GetDbTypeDefsToAlter (dbTypeDefsIn, dbTypeDefsOut).ToList ();
				var dbTypeDefsToRemove = DbSchemaUpdater.GetDbTypeDefsToRemove (dbInfrastructure, dbTypeDefsIn, dbTypeDefsOut).ToList ();

				var dbTablesToAdd = DbSchemaUpdater.GetDbTablesToAdd (dbTablesIn, dbTablesOut).ToList ();
				var dbTablesWithDifference = DbSchemaUpdater.GetDbTablesWithDifference (dbTablesIn, dbTablesOut).ToList ();
				var dbTablesWithDefititionDifference = DbSchemaUpdater.GetDbTablesWithDefinitionDifference (dbTablesWithDifference).ToList ();
				var dbTablesWithColumnDifference = DbSchemaUpdater.GetDbTablesWithColumnDifference (dbTablesWithDifference).ToList ();
				var dbTablesToRemove = DbSchemaUpdater.GetDbTablesToRemove (dbTablesIn, dbTablesOut).ToList ();

				var dbColumnsToAdd = DbSchemaUpdater.GetDbColumnsToAdd (dbTablesWithColumnDifference).ToList ();
				var dbColumnsToAlter = DbSchemaUpdater.GetDbColumnsToAlter (dbTablesWithColumnDifference).ToList ();
				var dbColumnsToRemove = DbSchemaUpdater.GetDbColumnsToRemove (dbTablesWithColumnDifference).ToList ();

				if (dbTypeDefsToAlter.Any ())
				{
					throw new System.InvalidOperationException ("Cannot alter a type definition");
				}

				if (dbTablesWithDefititionDifference.Any ())
				{
					throw new System.InvalidOperationException ("Cannot alter a table definition");
				}

				if (dbColumnsToAlter.Any ())
				{
					throw new System.InvalidOperationException ("Cannot alter a column definition");
				}

				DbSchemaUpdater.AddNewDbTypes (dbInfrastructure, dbTransaction, dbTypeDefsToAdd);

				DbSchemaUpdater.AddNewDbTables (dbInfrastructure, dbTransaction, dbTablesToAdd);

				DbSchemaUpdater.AddNewTableColumn (dbInfrastructure, dbTransaction, dbColumnsToAdd);

				DbSchemaUpdater.RemoveOldTableColumn (dbInfrastructure, dbTransaction, dbColumnsToRemove);

				DbSchemaUpdater.RemoveOldDbTable (dbInfrastructure, dbTransaction, dbTablesToRemove);

				DbSchemaUpdater.RemoveOldDbTypes (dbInfrastructure, dbTransaction, dbTypeDefsToRemove);

				dbTransaction.Commit ();
			}
		}

		
		private static IEnumerable<DbTable> GetDbTablesIn(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction)
		{
			return dbInfrastructure.FindDbTables (dbTransaction, DbElementCat.Any)
				.Where (t => !DbSchemaUpdater.IsBuiltInOrRelation (dbInfrastructure, t));
		}


		private static IEnumerable<DbTable> GetDbTablesOut(DbInfrastructure dbInfrastructure, IEnumerable<DbTable> newSchema)
		{
			return newSchema.Where (t => !DbSchemaUpdater.IsBuiltInOrRelation (dbInfrastructure, t));
		}


		private static bool IsBuiltInOrRelation(DbInfrastructure dbInfrastructure, DbTable dbTable)
		{
			// HACK This method is an ugly hack and should be cleaned when the categories for the
			// tables are implemented.
			// Marc

			return dbTable.Category == DbElementCat.Relation
				|| dbInfrastructure.FindBuiltInDbTables ()
				.Select (t => t.Name)
				.Append (Tags.TableConnection, Tags.TableInfo, Tags.TableLog, Tags.TableLock, Tags.TableUid)
				.Any (n => string.Equals (n, dbTable.Name));
		}


		private static IEnumerable<DbTypeDef> GetDbTypeDefsOfSchema(DbInfrastructure dbInfrastructure, IEnumerable<DbTable> dbTables)
		{
			var types = dbTables
				.SelectMany (t => t.Columns)
				.Select (c => c.Type)
				.Where (t => t != null)
				.Distinct<DbTypeDef> (new INameComparer ());

			var internalTypes = dbInfrastructure.FindBuiltInDbTypes ().ToList ();

			return DbSchemaUpdater.ExceptOnName (types, internalTypes);
		}


		private static IEnumerable<DbTypeDef> GetDbTypeDefsToAdd(DbInfrastructure dbInfrastructure, IList<DbTypeDef> dbTypeDefsIn, IList<DbTypeDef> dbTypeDefsOut)
		{
			return DbSchemaUpdater.ExceptOnName (dbTypeDefsOut, dbTypeDefsIn)
				.Where (t => !DbSchemaUpdater.IsUsedOrBuiltIn (dbInfrastructure, t));
		}


		private static IEnumerable<System.Tuple<DbTypeDef, DbTypeDef>> GetDbTypeDefsToAlter(IList<DbTypeDef> dbTypeDefsIn, IList<DbTypeDef> dbTypeDefsOut)
		{
			return DbSchemaUpdater.JoinOnName (dbTypeDefsIn, dbTypeDefsOut)
				.Where (t => !DbSchemaChecker.AreDbTypeDefEqual (t.Item1, t.Item2));
		}


		private static IEnumerable<DbTypeDef> GetDbTypeDefsToRemove(DbInfrastructure dbInfrastructure, IList<DbTypeDef> dbTypeDefsIn, IList<DbTypeDef> dbTypeDefsOut)
		{
			return DbSchemaUpdater.ExceptOnName (dbTypeDefsIn, dbTypeDefsOut)
				.Where (t => !DbSchemaUpdater.IsUsedOrBuiltIn (dbInfrastructure, t));
		}


		private static bool IsUsedOrBuiltIn(DbInfrastructure dbInfrastructure, DbTypeDef dbTypeDef)
		{
			// HACK This method is an ugly hack and should be cleaned when the categories for the
			// tables are implemented.
			// Marc

			return dbInfrastructure.FindBuiltInDbTypes ()
				.Select (t => t.Name)
				.Any (n => string.Equals (n, dbTypeDef.Name));
		}


		private static IEnumerable<DbTable> GetDbTablesToAdd(IList<DbTable> dbTablesIn, IList<DbTable> dbTablesOut)
		{
			return DbSchemaUpdater.ExceptOnName (dbTablesOut, dbTablesIn);
		}


		private static IEnumerable<System.Tuple<DbTable, DbTable>> GetDbTablesWithDifference(IList<DbTable> dbTablesIn, IList<DbTable> dbTablesOut)
		{
			return DbSchemaUpdater.JoinOnName (dbTablesIn, dbTablesOut)
				.Where (t => !DbSchemaChecker.CheckTables (t.Item1, t.Item2));
		}


		private static IEnumerable<System.Tuple<DbTable, DbTable>> GetDbTablesWithDefinitionDifference(IEnumerable<System.Tuple<DbTable, DbTable>> dbTablesToAlter)
		{
			return dbTablesToAlter
				.Where (t => !DbSchemaChecker.AreDbTableValuesEqual (t.Item1, t.Item2)
						  || !DbSchemaChecker.AreDbTableLocalizationsEqual (t.Item1, t.Item2)
						  || !DbSchemaChecker.AreDbTablePrimaryKeysEqual (t.Item1, t.Item2)
						  || !DbSchemaChecker.AreDbTableForeignKeysEqual (t.Item1, t.Item2)
						  || !DbSchemaChecker.AreDbTableIndexesEqual (t.Item1, t.Item2)
				);
		}


		private static IEnumerable<System.Tuple<DbTable, DbTable>> GetDbTablesWithColumnDifference(IEnumerable<System.Tuple<DbTable, DbTable>> dbTablesToAlter)
		{
			return dbTablesToAlter
				.Where (t => !DbSchemaChecker.AreDbTableColumnsEqual (t.Item1, t.Item2));
		}


		private static IEnumerable<DbTable> GetDbTablesToRemove(IList<DbTable> dbTablesIn, IList<DbTable> dbTablesOut)
		{
			return DbSchemaUpdater.ExceptOnName (dbTablesIn, dbTablesOut);
		}


		private static IEnumerable<DbColumn> GetDbColumnsToAdd(IList<System.Tuple<DbTable, DbTable>> tables)
		{
			return tables.SelectMany (t => DbSchemaUpdater.ExceptOnName (t.Item2.Columns, t.Item1.Columns));
		}


		private static IEnumerable<System.Tuple<DbColumn, DbColumn>> GetDbColumnsToAlter(IList<System.Tuple<DbTable, DbTable>> tables)
		{
			return tables.SelectMany (t => DbSchemaUpdater.JoinOnName (t.Item1.Columns, t.Item2.Columns))
				.Where (t => !DbSchemaChecker.AreDbColumnEqual (t.Item1, t.Item2));
		}


		private static IEnumerable<DbColumn> GetDbColumnsToRemove(IList<System.Tuple<DbTable, DbTable>> tables)
		{
			return tables.SelectMany (t => DbSchemaUpdater.ExceptOnName (t.Item1.Columns, t.Item2.Columns));
		}


		private static void AddNewDbTypes(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbTypeDef> dbTypeDefsToAdd)
		{
			var dbTypeDefsToRegister = DbSchemaUpdater.ExceptOnName (dbTypeDefsToAdd, dbInfrastructure.FindBuiltInDbTypes ());

			foreach (DbTypeDef dbTypeDef in dbTypeDefsToRegister)
			{
				dbInfrastructure.AddType (dbTransaction, dbTypeDef);
			}
		}


		private static void RemoveOldDbTypes(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbTypeDef> dbTypeDefsToRemove)
		{
			var dbTypeDefsToUnRegister = DbSchemaUpdater.ExceptOnName (dbTypeDefsToRemove, dbInfrastructure.FindBuiltInDbTypes ());

			foreach (DbTypeDef dbTypeDef in dbTypeDefsToUnRegister)
			{
				dbInfrastructure.RemoveType (dbTransaction, dbTypeDef);
			}
		}


		private static void AddNewDbTables(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbTable> dbTablesToAdd)
		{
			var dbTablesToRegister = DbSchemaUpdater.ExceptOnName (dbTablesToAdd, dbInfrastructure.FindBuiltInDbTables ());

			dbInfrastructure.AddTables (dbTransaction, dbTablesToRegister);
		}


		private static void RemoveOldDbTable(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbTable> dbTablesToRemove)
		{
			var dbTablesToUnregister = DbSchemaUpdater.ExceptOnName (dbTablesToRemove, dbInfrastructure.FindBuiltInDbTables ());

			foreach (DbTable dbTable in dbTablesToUnregister)
			{
				dbInfrastructure.RemoveTable (dbTransaction, dbTable);
			}
		}


		private static void AddNewTableColumn(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbColumn> dbColumnsToAdd)
		{
			foreach (DbColumn dbColumn in dbColumnsToAdd)
			{
				string dbTableName = dbColumn.Table.Name;
				DbTable dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);

				dbInfrastructure.AddColumnToTable (dbTransaction, dbTable, dbColumn);
			}
		}


		private static void RemoveOldTableColumn(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbColumn> dbColumnsToRemove)
		{
			foreach (DbColumn dbColumn in dbColumnsToRemove)
			{
				string dbTableName = dbColumn.Table.Name;
				DbTable dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);

				dbInfrastructure.RemoveColumnFromTable (dbTransaction, dbTable, dbColumn);
			}
		}


		private static IEnumerable<T> ExceptOnName<T>(IEnumerable<T> a, IEnumerable<T> b) where T : IName
		{
			// All that stuff with the cast of the INameComparer and the check about whether it is null
			// is here just to make the compiler happy, because the program won't compile if we use
			// an instance of INameComparer directly.
			// Marc

			IEqualityComparer<T> comparer = new INameComparer () as IEqualityComparer<T>;

			if (comparer == null)
			{
				throw new System.Exception ();
			}

			return Enumerable.Except<T> (a, b, comparer);
		}


		private static IEnumerable<System.Tuple<T, T>> JoinOnName<T>(IEnumerable<T> a, IEnumerable<T> b) where T : IName
		{
			return Enumerable.Join
			(
				a,
				b,
				e => e,
				e => e,
				(e1, e2) => System.Tuple.Create (e1, e2),
				new INameComparer ()
			);
		}
		

	}


}
