using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database.Collections;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database
{
	
	
	public static class DbSchemaUpdater
	{

		
		public static void UpdateSchema(DbInfrastructure dbInfrastructure, IEnumerable<DbTable> newSchema)
		{
			// TODO We could have problems here if in the tables, there are two types that share the
			// same name but have different values. Only one will be used. Should we check for that
			// and throw an exception in such cases?
			// Marc

			List<DbTable> dbTablesIn;
			List<DbTable> dbTablesOut;

			List<DbTypeDef> dbTypeDefsIn;
			List<DbTypeDef> dbTypeDefsOut;

			List<DbTypeDef> dbTypeDefsToAdd;
			List<Tuple<DbTypeDef, DbTypeDef>> dbTypeDefsToAlter;
			List<DbTypeDef> dbTypeDefsToRemove;

			List<DbTable> dbTablesToAdd;
			List<Tuple<DbTable, DbTable>> dbTablesWithDifference ;
			List<Tuple<DbTable, DbTable>> dbTablesWithDefititionDifference;
			List<Tuple<DbTable, DbTable>> dbTablesWithIndexDifference;
			List<Tuple<DbTable, DbTable>> dbTablesWithColumnDifference;
			List<DbTable> dbTablesToRemove;

			List<Tuple<DbTable, DbIndex>> dbIndexesToAdd;
			List<Tuple<DbTable, DbIndex>> dbIndexesToAlter;
			List<Tuple<DbTable, DbIndex>> dbIndexesToRemove;

			List<DbColumn> dbColumnsToAdd;
			List<Tuple<DbColumn, DbColumn>> dbColumnsToAlter;
			List<DbColumn> dbColumnsToRemove;

			List<Tuple<DbTable, DbTable>> dbTablesWithInvalidDefinitionDifference;
			List<Tuple<DbColumn, DbColumn>> dbColumnsWithInvalidAlterations;


			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				dbTablesIn = DbSchemaUpdater.GetDbTablesIn (dbInfrastructure, dbTransaction).ToList ();
				dbTablesOut = DbSchemaUpdater.GetDbTablesOut (dbInfrastructure, newSchema).ToList ();

				dbTypeDefsIn = DbSchemaUpdater.GetDbTypeDefsOfSchema (dbInfrastructure, dbTablesIn).ToList ();
				dbTypeDefsOut = DbSchemaUpdater.GetDbTypeDefsOfSchema (dbInfrastructure, dbTablesOut).ToList ();

				dbTypeDefsToAdd = DbSchemaUpdater.GetDbTypeDefsToAdd (dbInfrastructure, dbTypeDefsIn, dbTypeDefsOut).ToList ();
				dbTypeDefsToAlter = DbSchemaUpdater.GetDbTypeDefsToAlter (dbTypeDefsIn, dbTypeDefsOut).ToList ();
				dbTypeDefsToRemove = DbSchemaUpdater.GetDbTypeDefsToRemove (dbInfrastructure, dbTypeDefsIn, dbTypeDefsOut).ToList ();

				dbTablesToAdd = DbSchemaUpdater.GetDbTablesToAdd (dbTablesIn, dbTablesOut).ToList ();
				dbTablesWithDifference = DbSchemaUpdater.GetDbTablesWithDifference (dbTablesIn, dbTablesOut).ToList ();
				dbTablesWithDefititionDifference = DbSchemaUpdater.GetDbTablesWithDefinitionDifference (dbTablesWithDifference).ToList ();
				dbTablesWithIndexDifference = DbSchemaUpdater.GetDbTablesWithIndexDifference (dbTablesWithDifference).ToList ();
				dbTablesWithColumnDifference = DbSchemaUpdater.GetDbTablesWithColumnDifference (dbTablesWithDifference).ToList ();
				dbTablesToRemove = DbSchemaUpdater.GetDbTablesToRemove (dbTablesIn, dbTablesOut).ToList ();

				dbIndexesToAdd = DbSchemaUpdater.GetDbIndexesToAdd (dbTablesWithIndexDifference).ToList ();
				dbIndexesToAlter = DbSchemaUpdater.GetDbIndexesToAlter (dbTablesWithIndexDifference).ToList ();
				dbIndexesToRemove = DbSchemaUpdater.GetDbIndexesToRemove (dbTablesWithIndexDifference).ToList ();

				dbColumnsToAdd = DbSchemaUpdater.GetDbColumnsToAdd (dbTablesWithColumnDifference).ToList ();
				dbColumnsToAlter = DbSchemaUpdater.GetDbColumnsToAlter (dbTablesWithColumnDifference).ToList ();
				dbColumnsToRemove = DbSchemaUpdater.GetDbColumnsToRemove (dbTablesWithColumnDifference).ToList ();

				dbTablesWithInvalidDefinitionDifference = DbSchemaUpdater.GetDbTablesWithInvalidDefinitionDifference (dbTablesWithDefititionDifference).ToList ();
				dbColumnsWithInvalidAlterations = DbSchemaUpdater.GetDbColumnsWithInvalidDifference (dbColumnsToAlter).ToList ();
			}

			if (dbTypeDefsToAlter.Any ())
			{
				throw new InvalidOperationException ("Cannot alter a type definition");
			}

			if (dbTablesWithInvalidDefinitionDifference.Any ())
			{
				throw new InvalidOperationException ("Cannot alter a table definition");
			}

			if (dbColumnsWithInvalidAlterations.Any ())
			{
				throw new InvalidOperationException ("Cannot alter a column with invalid column alterations.");
			}

			DbSchemaUpdater.AddNewDbTypes (dbInfrastructure, dbTypeDefsToAdd);
			DbSchemaUpdater.UpdateDbTypesKey (dbInfrastructure, newSchema);
			DbSchemaUpdater.AddNewDbTables (dbInfrastructure, dbTablesToAdd);
			DbSchemaUpdater.AddNewTableColumns (dbInfrastructure, dbColumnsToAdd);
			DbSchemaUpdater.RemoveOldIndexes (dbInfrastructure, dbIndexesToRemove);
			DbSchemaUpdater.AlterIndexes (dbInfrastructure, dbIndexesToAlter);
			DbSchemaUpdater.AlterTableDefinitions (dbInfrastructure, dbTablesWithDefititionDifference);
			DbSchemaUpdater.AddNewIndexes (dbInfrastructure, dbIndexesToAdd);
			DbSchemaUpdater.RemoveOldTableColumns (dbInfrastructure, dbColumnsToRemove);
			DbSchemaUpdater.RemoveOldDbTables (dbInfrastructure, dbTablesToRemove);
			DbSchemaUpdater.RemoveOldDbTypes (dbInfrastructure, dbTypeDefsToRemove);
			DbSchemaUpdater.AlterTableColumns (dbInfrastructure, dbColumnsToAlter);
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
			return dbTable.Category == DbElementCat.Relation
				|| dbInfrastructure.FindBuiltInDbTables ().Contains (dbTable, INameComparer.Instance);
		}



		private static IEnumerable<DbTypeDef> GetDbTypeDefsInTables(IEnumerable<DbTable> dbTables)
		{
			return dbTables
				.SelectMany (t => t.Columns)
				.Select (c => c.Type)
				.Where (t => t != null)
				.Distinct<DbTypeDef> (INameComparer.Instance);
		}

		private static IEnumerable<DbTypeDef> GetDbTypeDefsOfSchema(DbInfrastructure dbInfrastructure, IEnumerable<DbTable> dbTables)
		{
			var types = GetDbTypeDefsInTables (dbTables);
			var internalTypes = dbInfrastructure.FindBuiltInDbTypes ().ToList ();

			return DbSchemaUpdater.ExceptOnName (types, internalTypes);
		}


		private static IEnumerable<DbTypeDef> GetDbTypeDefsToAdd(DbInfrastructure dbInfrastructure, IList<DbTypeDef> dbTypeDefsIn, IList<DbTypeDef> dbTypeDefsOut)
		{
			return DbSchemaUpdater.ExceptOnName (dbTypeDefsOut, dbTypeDefsIn)
				.Where (t => !DbSchemaUpdater.IsUsedOrBuiltIn (dbInfrastructure, t));
		}


		private static IEnumerable<Tuple<DbTypeDef, DbTypeDef>> GetDbTypeDefsToAlter(IList<DbTypeDef> dbTypeDefsIn, IList<DbTypeDef> dbTypeDefsOut)
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
			return dbInfrastructure.FindBuiltInDbTypes ()
				.Select (t => t.Name)
				.Any (n => string.Equals (n, dbTypeDef.Name));
		}


		private static IEnumerable<DbTable> GetDbTablesToAdd(IList<DbTable> dbTablesIn, IList<DbTable> dbTablesOut)
		{
			return DbSchemaUpdater.ExceptOnName (dbTablesOut, dbTablesIn);
		}


		private static IEnumerable<Tuple<DbTable, DbTable>> GetDbTablesWithDifference(IList<DbTable> dbTablesIn, IList<DbTable> dbTablesOut)
		{
			return DbSchemaUpdater.JoinOnName (dbTablesIn, dbTablesOut)
				.Where (t => !DbSchemaChecker.AreDbTablesEqual (t.Item1, t.Item2));
		}


		private static IEnumerable<Tuple<DbTable, DbTable>> GetDbTablesWithDefinitionDifference(IEnumerable<Tuple<DbTable, DbTable>> dbTablesToAlter)
		{
			return dbTablesToAlter
				.Where (t => !DbSchemaChecker.AreDbTableValuesEqual (t.Item1, t.Item2)
						  || !DbSchemaChecker.AreDbTablePrimaryKeysEqual (t.Item1, t.Item2)
						  || !DbSchemaChecker.AreDbTableForeignKeysEqual (t.Item1, t.Item2)
				);
		}


		private static IEnumerable<Tuple<DbTable, DbTable>> GetDbTablesWithIndexDifference(IEnumerable<Tuple<DbTable, DbTable>> dbTablesToAlter)
		{
			return dbTablesToAlter
				.Where (t => !DbSchemaChecker.AreDbTableIndexesEqual (t.Item1, t.Item2));
		}


		private static IEnumerable<Tuple<DbTable, DbTable>> GetDbTablesWithColumnDifference(IEnumerable<Tuple<DbTable, DbTable>> dbTablesToAlter)
		{
			return dbTablesToAlter
				.Where (t => !DbSchemaChecker.AreDbTableColumnsEqual (t.Item1, t.Item2));
		}


		private static IEnumerable<DbTable> GetDbTablesToRemove(IList<DbTable> dbTablesIn, IList<DbTable> dbTablesOut)
		{
			return DbSchemaUpdater.ExceptOnName (dbTablesIn, dbTablesOut);
		}


		private static IEnumerable<Tuple<DbTable, DbIndex>> GetDbIndexesToAdd(IList<Tuple<DbTable, DbTable>> tables)
		{
			foreach (var t in tables)
			{
				foreach (DbIndex index in DbSchemaUpdater.ExceptOnName (t.Item2.Indexes, t.Item1.Indexes))
				{
					yield return Tuple.Create (t.Item2, index);
				}
			}
		}


		private static IEnumerable<Tuple<DbTable, DbIndex>> GetDbIndexesToAlter(IList<Tuple<DbTable, DbTable>> tables)
		{
			foreach (var t in tables)
			{
				foreach (var i in DbSchemaUpdater.JoinOnName (t.Item1.Indexes, t.Item2.Indexes))
				{
					if (!DbSchemaChecker.AreDbIndexEqual (i.Item1, i.Item2))
					{
						yield return Tuple.Create (t.Item2, i.Item2);
					}
				}
			}
		}


		private static IEnumerable<Tuple<DbTable, DbIndex>> GetDbIndexesToRemove(IList<Tuple<DbTable, DbTable>> tables)
		{
			foreach (var t in tables)
			{
				foreach (DbIndex index in DbSchemaUpdater.ExceptOnName (t.Item1.Indexes, t.Item2.Indexes))
				{
					yield return Tuple.Create (t.Item1, index);
				}
			}
		}


		private static IEnumerable<DbColumn> GetDbColumnsToAdd(IList<Tuple<DbTable, DbTable>> tables)
		{
			return tables.SelectMany (t => DbSchemaUpdater.ExceptOnName (t.Item2.Columns, t.Item1.Columns));
		}


		private static IEnumerable<Tuple<DbColumn, DbColumn>> GetDbColumnsToAlter(IList<Tuple<DbTable, DbTable>> tables)
		{
			return tables.SelectMany (t => DbSchemaUpdater.JoinOnName (t.Item1.Columns, t.Item2.Columns))
				.Where (t => !DbSchemaChecker.AreDbColumnEqual (t.Item1, t.Item2));
		}


		private static IEnumerable<DbColumn> GetDbColumnsToRemove(IList<Tuple<DbTable, DbTable>> tables)
		{
			return tables.SelectMany (t => DbSchemaUpdater.ExceptOnName (t.Item1.Columns, t.Item2.Columns));
		}


		private static IEnumerable<Tuple<DbTable, DbTable>> GetDbTablesWithInvalidDefinitionDifference(IList<Tuple<DbTable, DbTable>> tables)
		{
			return from item in tables
				   let a = item.Item1
				   let b = item.Item2
				   where DbSchemaUpdater.IsDbTableDefinitionDifferenceInvalid (a, b)
				   select item;
		}


		private static bool IsDbTableDefinitionDifferenceInvalid(DbTable a, DbTable b)
		{
			bool valid = a.CaptionId == b.CaptionId
				&& string.Equals (a.Name, b.Name)
				&& a.Category == b.Category;

			if (valid && a.Category == DbElementCat.Relation)
			{
				valid = string.Equals (a.RelationSourceTableName, b.RelationSourceTableName)
						&& string.Equals (a.RelationTargetTableName, b.RelationTargetTableName);
			}

			return !valid;
		}


		private static IEnumerable<Tuple<DbColumn, DbColumn>> GetDbColumnsWithInvalidDifference(IList<Tuple<DbColumn, DbColumn>> columns)
		{
			return from item in columns
				   let a = item.Item1
				   let b = item.Item2
				   where a.CaptionId != b.CaptionId
					|| !string.Equals (a.Name, b.Name)
					|| !((a.Table == null && b.Table == null) || (string.Equals (a.Table.Name, b.Table.Name) && a.Table.CaptionId == b.Table.CaptionId))
					|| !string.Equals (a.TargetTableName, b.TargetTableName)
					|| !string.Equals (a.TargetColumnName, b.TargetColumnName)
					|| a.Category != b.Category
					|| a.ColumnClass != b.ColumnClass
					|| a.Cardinality != b.Cardinality
					|| a.IsPrimaryKey != b.IsPrimaryKey
					|| a.IsForeignKey != b.IsForeignKey
					|| a.IsAutoIncremented != b.IsAutoIncremented
					|| a.IsAutoIncremented
					|| b.IsAutoIncremented
					|| a.IsAutoTimeStampOnInsert != b.IsAutoTimeStampOnInsert
					|| a.IsAutoTimeStampOnInsert
					|| b.IsAutoTimeStampOnInsert
					|| a.IsAutoTimeStampOnUpdate != b.IsAutoTimeStampOnUpdate
					|| a.IsAutoTimeStampOnUpdate
					|| b.IsAutoTimeStampOnUpdate
					|| !DbSchemaUpdater.AreDbTypeDefsValueCompatibles (a.Type, b.Type)
				   select item;
		}


		private static bool AreDbTypeDefsValueCompatibles(DbTypeDef a, DbTypeDef b)
		{
			if (DbSchemaUpdater.AreDbRawTypesValueCompatible (a.RawType, b.RawType)
				&& DbSchemaUpdater.AreDbSimpleTypesValueCompatible (a.SimpleType, b.SimpleType)
				&& DbSchemaUpdater.AreDbNumDefValueCompatible (a.NumDef, b.NumDef))
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		private static bool AreDbSimpleTypesValueCompatible(DbSimpleType a, DbSimpleType b)
		{
			switch (b)
			{
				case DbSimpleType.ByteArray:
				case DbSimpleType.Date:
				case DbSimpleType.Time:
				case DbSimpleType.Guid:
				case DbSimpleType.Decimal:
					return a == b;
				
				case DbSimpleType.DateTime:
					return a == DbSimpleType.Date
						|| a == DbSimpleType.DateTime;

				case DbSimpleType.String:
					return true;
				
				default:
					throw new NotImplementedException ();
			}
		}


		private static bool AreDbRawTypesValueCompatible(DbRawType a, DbRawType b)
		{
			switch (b)
			{
				case DbRawType.Boolean:
				case DbRawType.ByteArray:
				case DbRawType.Guid:
				case DbRawType.Date:
				case DbRawType.Time:
				case DbRawType.Int16:
					return a == b;
	
				case DbRawType.String:
					return true;

				case DbRawType.DateTime:
					return a == DbRawType.Date
						|| a == DbRawType.DateTime;

				case DbRawType.Int32:
					return a == DbRawType.Int16
						|| a == DbRawType.Int32;

				case DbRawType.Int64:
					return a == DbRawType.Int16
						|| a == DbRawType.Int32
						|| a == DbRawType.Int64;

				case DbRawType.SmallDecimal:
					return a == DbRawType.Int16
						|| a == DbRawType.Int32
						|| a == DbRawType.SmallDecimal;

				case DbRawType.LargeDecimal:
					return a == DbRawType.Int16
						|| a == DbRawType.Int32
						|| a == DbRawType.SmallDecimal
						|| a == DbRawType.LargeDecimal;

				default:
					throw new NotImplementedException ();
			}
		}


		private static bool AreDbNumDefValueCompatible(DbNumDef a, DbNumDef b)
		{
			// HACK Remove this line and uncomment the body of the method to correct it.
			
			return true;

			//return (b == null) || 
			//    (
			//           a != null && b != null
			//        && a.MinValue >= b.MinValue
			//        && a.MaxValue <= b.MaxValue
			//        && a.DigitShift <= b.DigitShift
			//        && a.DigitPrecision - a.DigitShift <= b.DigitPrecision - b.DigitShift
			//    );
		}


		private static void AddNewDbTypes(DbInfrastructure dbInfrastructure, IEnumerable<DbTypeDef> dbTypeDefsToAdd)
		{
			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (DbTypeDef dbTypeDef in dbTypeDefsToAdd)
				{
					dbInfrastructure.AddType (dbTransaction, dbTypeDef);
				}

				dbTransaction.Commit ();
			}
		}


		private static void UpdateDbTypesKey(DbInfrastructure dbInfrastructure, IEnumerable<DbTable> newTables)
		{
			// NOTE This method might become incorrect if we allow the alteration of types and must
			// be changed if that ever happens.
			// Marc

			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (DbTypeDef dbTypeDef in DbSchemaUpdater.GetDbTypeDefsInTables (newTables))
				{
					if (dbTypeDef.Key.IsEmpty)
					{
						DbTypeDef type = dbInfrastructure.ResolveDbType (dbTransaction, dbTypeDef.Name);

						if (type != null)
						{
							dbTypeDef.DefineKey (type.Key);
						}
					}
				}

				dbTransaction.Commit ();
			}
		}


		private static void RemoveOldDbTypes(DbInfrastructure dbInfrastructure, IEnumerable<DbTypeDef> dbTypeDefsToRemove)
		{
			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (DbTypeDef dbTypeDef in dbTypeDefsToRemove)
				{
					dbInfrastructure.RemoveType (dbTransaction, dbTypeDef);
				}

				dbTransaction.Commit ();
			}
		}


		private static void AddNewDbTables(DbInfrastructure dbInfrastructure, IEnumerable<DbTable> dbTablesToAdd)
		{
			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				dbInfrastructure.AddTables (dbTransaction, dbTablesToAdd);

				dbTransaction.Commit ();
			}
		}


		private static void RemoveOldDbTables(DbInfrastructure dbInfrastructure, IEnumerable<DbTable> dbTablesToRemove)
		{
			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (DbTable dbTable in dbTablesToRemove)
				{
					dbInfrastructure.RemoveTable (dbTransaction, dbTable);
				}

				dbTransaction.Commit ();
			}
		}


		private static void AddNewTableColumns(DbInfrastructure dbInfrastructure, IEnumerable<DbColumn> dbColumnsToAdd)
		{
			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (DbColumn dbColumn in dbColumnsToAdd)
				{
					string dbTableName = dbColumn.Table.Name;
					DbTable dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);

					dbInfrastructure.AddColumnToTable (dbTransaction, dbTable, dbColumn);
				}

				dbTransaction.Commit ();
			}
		}


		private static void RemoveOldTableColumns(DbInfrastructure dbInfrastructure, IEnumerable<DbColumn> dbColumnsToRemove)
		{
			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (DbColumn dbColumn in dbColumnsToRemove)
				{
					string dbTableName = dbColumn.Table.Name;
					DbTable dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);

					dbInfrastructure.RemoveColumnFromTable (dbTransaction, dbTable, dbColumn);
				}

				dbTransaction.Commit ();
			}
		}


		private static void AddNewIndexes(DbInfrastructure dbInfrastructure, IEnumerable<Tuple<DbTable, DbIndex>> dbIndexesToAdd)
		{
			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (var item in dbIndexesToAdd)
				{
					DbTable table = item.Item1;
					DbIndex index = item.Item2;

					dbInfrastructure.AddIndexToTable (dbTransaction, table, index);
				}

				dbTransaction.Commit ();
			}
		}


		private static void AlterIndexes(DbInfrastructure dbInfrastructure, List<Tuple<DbTable, DbIndex>> dbIndexesToAlter)
		{
			DbSchemaUpdater.RemoveOldIndexes (dbInfrastructure, dbIndexesToAlter);
			DbSchemaUpdater.AddNewIndexes (dbInfrastructure, dbIndexesToAlter);
		}


		private static void RemoveOldIndexes(DbInfrastructure dbInfrastructure, IEnumerable<Tuple<DbTable, DbIndex>> dbIndexesToRemove)
		{
			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (var item in dbIndexesToRemove)
				{
					DbTable table = item.Item1;
					DbIndex index = item.Item2;

					dbInfrastructure.RemoveIndexFromTable (dbTransaction, table, index);
				}

				dbTransaction.Commit ();
			}
		}


		private static void AlterTableDefinitions(DbInfrastructure dbInfrastructure, List<Tuple<DbTable, DbTable>> dbTablesWithDefititionDifference)
		{
			using (var dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (var item in dbTablesWithDefititionDifference)
				{
					DbTable table = item.Item2;
					string comment = table.Comment;

					dbInfrastructure.SetTableComment (dbTransaction, table, comment);
				}

				dbTransaction.Commit ();
			}
		}


		private static void AlterTableColumns(DbInfrastructure dbInfrastructure, List<Tuple<DbColumn, DbColumn>> dbColumnsToAlter)
		{
			var indexesToDisable = DbSchemaUpdater.GetIndexesToDisable (dbColumnsToAlter);

			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				DbSchemaUpdater.DisableIndexes (dbInfrastructure, dbTransaction, indexesToDisable);

				dbTransaction.Commit ();
			}

			foreach (var item in dbColumnsToAlter)
			{
				// NOTE For now, all we can do is the following :
				// - alter the nullability of the column. Note that I said the nullability of the
				//  column, I don't speak of the nullability of the type of the data in the column.
				// - alter the type of the column. Only the type alterations that doesn't require a
				//   conversion of the values are allowed. For instance int to long is allowed as
				//   well as anything to string, but long to int is not allowed.
				// - alter the charset or the collation of a text column.
				// All other alterations are not treated and should not happen as we make sure that
				// there aren't such alterations before in this method.
				// Marc

				DbSchemaUpdater.AlterTableColumn (dbInfrastructure, item);
			}
			
			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				DbSchemaUpdater.EnableIndexes (dbInfrastructure, dbTransaction, indexesToDisable);

				dbTransaction.Commit ();
			}
		}


		private static void AlterTableColumn(DbInfrastructure dbInfrastructure, Tuple<DbColumn, DbColumn> item)
		{
			// NOTE Here we need to make 3 transactions, because we can't alter tables and columns
			// and alter data within the same tables and columns. Therefore we need one transaction
			// to rename the old column and create the new one, one transaction to copy the data
			// from the old column to the now one and one transaction to remove the old one.
			// Marc
		
			var oldColumn = item.Item1;
			var newColumn = item.Item2;

			var dbTableName = oldColumn.Table.Name;
			var dbTable = dbInfrastructure.ResolveDbTable (dbTableName);

			var tmpColumnName = oldColumn.Name + "TMP";

			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				dbInfrastructure.RenameTableColumn (dbTable, oldColumn, tmpColumnName);
				dbInfrastructure.AddColumnToTable (dbTable, newColumn);

				dbTransaction.Commit ();
			}

			dbTable = dbInfrastructure.ResolveDbTable (dbTableName);
			var tmpColumn = dbTable.Columns[tmpColumnName];

			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				if (oldColumn.IsNullable && !newColumn.IsNullable)
				{
					DbSchemaUpdater.SetDefaultValueInNullCells (dbInfrastructure, dbTransaction, dbTable, tmpColumn);
				}

				DbSchemaUpdater.CopyValues (dbInfrastructure, dbTransaction, dbTable, tmpColumn, newColumn);

				dbTransaction.Commit ();
			}

			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				dbInfrastructure.RemoveColumnFromTable (dbTable, tmpColumn);

				dbTransaction.Commit ();
			}	
		}


		private static Dictionary<DbTable, List<DbIndex>> GetIndexesToDisable(IEnumerable<Tuple<DbColumn, DbColumn>> dbColumsToAlter)
		{
			return dbColumsToAlter
				.Select (c => c.Item2)
				.GroupBy (c => c.Table)
				.Select (g => Tuple.Create (g.Key, g.ToSet ()))
				.ToDictionary
				(
					t => t.Item1,
					t => t.Item1.Indexes
						.Where (i => i.Columns.Any (c => t.Item2.Contains (c)))
						.ToList ()
				);
		}


		private static void DisableIndexes(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, Dictionary<DbTable, List<DbIndex>> dbIndexes)
		{
			foreach (var item in dbIndexes)
			{
				var dbTable = item.Key;

				foreach (var dbIndex in item.Value)
				{
					dbInfrastructure.RemoveIndexFromTable (dbTransaction, dbTable, dbIndex);
				}
			}
		}


		private static void EnableIndexes(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, Dictionary<DbTable, List<DbIndex>> dbIndexes)
		{
			foreach (var item in dbIndexes)
			{
				var dbTable = item.Key;

				foreach (var dbIndex in item.Value)
				{
					dbInfrastructure.AddIndexToTable (dbTransaction, dbTable, dbIndex);
				}
			}
		}


		private static void SetDefaultValueInNullCells(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, DbTable dbTable, DbColumn dbColumn)
		{
			var sqlTableName = dbTable.GetSqlName ();
			var sqlColumnName = dbColumn.GetSqlName ();
			
			var sqlFields = new SqlFieldList ();
			DbTypeDef type = dbColumn.Type;
			SqlField sqlField = DbSchemaUpdater.GetConstantWithDefaultValue (type);
			sqlField.Alias = sqlColumnName;
			sqlFields.Add (sqlField);

			var sqlConditions = new SqlFieldList ()
			{
				SqlField.CreateFunction
				(
					new SqlFunction
					(
						SqlFunctionCode.CompareIsNull,
						SqlField.CreateName(sqlTableName, sqlColumnName)
					)
				)
			};

			dbTransaction.SqlBuilder.UpdateData (sqlTableName, sqlFields, sqlConditions);
			dbInfrastructure.ExecuteSilent (dbTransaction);
		}


		private static SqlField GetConstantWithDefaultValue(DbTypeDef type)
		{
			var typeRawType = type.RawType;
			var value = TypeConverter.GetDefaultValueForDbRawType(typeRawType);

			return SqlField.CreateConstant (value, type.RawType);
		}


		private static void CopyValues(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, DbTable dbTable, DbColumn dbColumnFrom, DbColumn dbColumnTo)
		{
			var sqlTableName = dbTable.GetSqlName ();
			var sqlColumnFromName = dbColumnFrom.GetSqlName ();
			var sqlColumnToName = dbColumnTo.GetSqlName ();

			var sqlFields = new SqlFieldList ()
			{
				SqlField.CreateAliasedName (sqlColumnFromName, sqlColumnToName)
			};
			
			var sqlConditions = new SqlFieldList ();

			dbTransaction.SqlBuilder.UpdateData (sqlTableName, sqlFields, sqlConditions);
			dbInfrastructure.ExecuteSilent (dbTransaction);
		}


		private static IEnumerable<T> ExceptOnName<T>(IEnumerable<T> a, IEnumerable<T> b) where T : IName
		{
			// All that stuff with the cast of the INameComparer and the check about whether it is null
			// is here just to make the compiler happy, because the program won't compile if we use
			// an instance of INameComparer directly.
			// Marc

			IEqualityComparer<T> comparer = INameComparer.Instance as IEqualityComparer<T>;

			if (comparer == null)
			{
				throw new Exception ();
			}

			return Enumerable.Except<T> (a, b, comparer);
		}


		private static IEnumerable<Tuple<T, T>> JoinOnName<T>(IEnumerable<T> a, IEnumerable<T> b) where T : IName
		{
			return Enumerable.Join
			(
				a,
				b,
				e => e,
				e => e,
				(e1, e2) => Tuple.Create (e1, e2),
				INameComparer.Instance
			);
		}


	}


}
