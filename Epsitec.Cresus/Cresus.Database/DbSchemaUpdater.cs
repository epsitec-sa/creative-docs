using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Linq;
using System.Collections;


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
			List<System.Tuple<DbTypeDef, DbTypeDef>> dbTypeDefsToAlter;
			List<DbTypeDef> dbTypeDefsToRemove;

			List<DbTable> dbTablesToAdd;
			List<System.Tuple<DbTable, DbTable>> dbTablesWithDifference ;
			List<System.Tuple<DbTable, DbTable>> dbTablesWithDefititionDifference;
			List<System.Tuple<DbTable, DbTable>> dbTablesWithIndexDifference;
			List<System.Tuple<DbTable, DbTable>> dbTablesWithColumnDifference;
			List<DbTable> dbTablesToRemove;

			List<System.Tuple<DbTable, DbIndex>> dbIndexesToAdd;
			List<System.Tuple<DbTable, DbIndex>> dbIndexesToAlter;
			List<System.Tuple<DbTable, DbIndex>> dbIndexesToRemove;

			List<DbColumn> dbColumnsToAdd;
			List<System.Tuple<DbColumn, DbColumn>> dbColumnsToAlter;
			List<DbColumn> dbColumnsToRemove;

			List<System.Tuple<DbColumn, DbColumn>> dbColumnsWithInvalidAlterations;


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

				dbColumnsWithInvalidAlterations = DbSchemaUpdater.GetDbColumnsWithInvalidDifference (dbColumnsToAlter).ToList ();
			}

			if (dbTypeDefsToAlter.Any ())
			{
				throw new System.InvalidOperationException ("Cannot alter a type definition");
			}

			if (dbTablesWithDefititionDifference.Any ())
			{
				throw new System.InvalidOperationException ("Cannot alter a table definition");
			}

			if (dbColumnsWithInvalidAlterations.Any ())
			{
				throw new System.InvalidOperationException ("Cannot alter a column with invalid column alterations.");
			}

			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				DbSchemaUpdater.AddNewDbTypes (dbInfrastructure, dbTransaction, dbTypeDefsToAdd);

				DbSchemaUpdater.UpdateDbTypesKey (dbInfrastructure, dbTransaction, newSchema);

				DbSchemaUpdater.AddNewDbTables (dbInfrastructure, dbTransaction, dbTablesToAdd);

				DbSchemaUpdater.AddNewTableColumns (dbInfrastructure, dbTransaction, dbColumnsToAdd);

				DbSchemaUpdater.RemoveOldIndexes (dbInfrastructure, dbTransaction, dbIndexesToRemove);

				DbSchemaUpdater.AlterIndexes (dbInfrastructure, dbTransaction, dbIndexesToAlter);

				DbSchemaUpdater.AddNewIndexes (dbInfrastructure, dbTransaction, dbIndexesToAdd);

				DbSchemaUpdater.RemoveOldTableColumns (dbInfrastructure, dbTransaction, dbColumnsToRemove);

				DbSchemaUpdater.RemoveOldDbTables (dbInfrastructure, dbTransaction, dbTablesToRemove);

				DbSchemaUpdater.RemoveOldDbTypes (dbInfrastructure, dbTransaction, dbTypeDefsToRemove);

				dbTransaction.Commit ();
			}

			// NOTE Here we need to make 3 transactions, because we can't alter tables and columns
			// and alter data within the same tables and columns. Therefore we need one transaction
			// to rename the old column and create the new one, one transaction to copy the data
			// from the old column to the now one and one transaction to remove the old one.
			// Marc

			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				DbSchemaUpdater.ExecutePart1OfColumnAlteration (dbInfrastructure, dbTransaction, dbColumnsToAlter);

				dbTransaction.Commit ();
			}

			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				// NOTE For now, all we can do is the following :
				// - alter the nullability of the column. Note that I said the nullability of the
				//  column, I don't speak of the nullability of the type of the data in the column.
				// - alter the type of the column. Only the type alterations that doesn't require a
				//   conversion of the values are allowed. For instance int to long is allowed as
				//   well as anything to string, but long to int is not allowed.
				// All other alterations are not treated and should not happen as we make sure that
				// there aren't such alterations before in this method.
				// Marc

				DbSchemaUpdater.ExecutePart2OfColumnAlteration (dbInfrastructure, dbTransaction, dbColumnsToAlter);

				dbTransaction.Commit ();
			}

			using (DbTransaction dbTransaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				DbSchemaUpdater.ExecutePart3OfColumnAlteration (dbInfrastructure, dbTransaction, dbColumnsToAlter);

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
				.Where (t => !DbSchemaChecker.AreDbTablesEqual (t.Item1, t.Item2));
		}


		private static IEnumerable<System.Tuple<DbTable, DbTable>> GetDbTablesWithDefinitionDifference(IEnumerable<System.Tuple<DbTable, DbTable>> dbTablesToAlter)
		{
			return dbTablesToAlter
				.Where (t => !DbSchemaChecker.AreDbTableValuesEqual (t.Item1, t.Item2)
						  || !DbSchemaChecker.AreDbTablePrimaryKeysEqual (t.Item1, t.Item2)
						  || !DbSchemaChecker.AreDbTableForeignKeysEqual (t.Item1, t.Item2)
				);
		}


		private static IEnumerable<System.Tuple<DbTable, DbTable>> GetDbTablesWithIndexDifference(IEnumerable<System.Tuple<DbTable, DbTable>> dbTablesToAlter)
		{
			return dbTablesToAlter
				.Where (t => !DbSchemaChecker.AreDbTableIndexesEqual (t.Item1, t.Item2));
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


		private static IEnumerable<System.Tuple<DbTable, DbIndex>> GetDbIndexesToAdd(IList<System.Tuple<DbTable, DbTable>> tables)
		{
			foreach (var t in tables)
			{
				foreach (DbIndex index in DbSchemaUpdater.ExceptOnName (t.Item2.Indexes, t.Item1.Indexes))
				{
					yield return System.Tuple.Create (t.Item2, index);
				}
			}
		}


		private static IEnumerable<System.Tuple<DbTable, DbIndex>> GetDbIndexesToAlter(IList<System.Tuple<DbTable, DbTable>> tables)
		{
			foreach (var t in tables)
			{
				foreach (var i in DbSchemaUpdater.JoinOnName (t.Item1.Indexes, t.Item2.Indexes))
				{
					yield return System.Tuple.Create (t.Item2, i.Item2);
				}
			}
		}


		private static IEnumerable<System.Tuple<DbTable, DbIndex>> GetDbIndexesToRemove(IList<System.Tuple<DbTable, DbTable>> tables)
		{
			foreach (var t in tables)
			{
				foreach (DbIndex index in DbSchemaUpdater.ExceptOnName (t.Item1.Indexes, t.Item2.Indexes))
				{
					yield return System.Tuple.Create (t.Item1, index);
				}
			}
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


		private static IEnumerable<System.Tuple<DbColumn, DbColumn>> GetDbColumnsWithInvalidDifference(IList<System.Tuple<DbColumn, DbColumn>> columns)
		{
			return from item in columns
				   let a = item.Item1
				   let b = item.Item2
				   where a.CaptionId != b.CaptionId
					|| !string.Equals (a.Name, b.Name)
					|| !string.Equals (a.Comment, b.Comment)
					|| !((a.Table == null && b.Table == null) || (string.Equals (a.Table.Name, b.Table.Name) && a.Table.CaptionId == b.Table.CaptionId))
					|| !string.Equals (a.TargetTableName, b.TargetTableName)
					|| !string.Equals (a.TargetColumnName, b.TargetColumnName)
					|| a.Category != b.Category
					|| a.ColumnClass != b.ColumnClass
					|| a.Cardinality != b.Cardinality
					|| a.IsPrimaryKey != b.IsPrimaryKey
					|| a.IsForeignKey != b.IsForeignKey
					|| a.IsAutoIncremented != b.IsAutoIncremented
					|| a.IsAutoTimeStampOnInsert != b.IsAutoTimeStampOnInsert
					|| a.IsAutoTimeStampOnUpdate != b.IsAutoTimeStampOnUpdate
					|| !DbSchemaUpdater.AreDbTypeDefsValueCompatibles (a.Type, b.Type)
				   select item;
		}


		private static bool AreDbTypeDefsValueCompatibles(DbTypeDef a, DbTypeDef b)
		{
			return DbSchemaUpdater.AreDbRawTypesValueCompatible (a.RawType, b.RawType)
				&& DbSchemaUpdater.AreDbSimpleTypesValueCompatible (a.SimpleType, b.SimpleType)
				&& DbSchemaUpdater.AreDbNumDefValueCompatible (a.NumDef, b.NumDef);
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
					throw new System.NotImplementedException ();
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
						|| a == DbRawType.Int32;

				case DbRawType.LargeDecimal:
					return a == DbRawType.Int16
						|| a == DbRawType.Int32
						|| a == DbRawType.SmallDecimal;

				default:
					throw new System.NotImplementedException ();
			}
		}


		private static bool AreDbNumDefValueCompatible(DbNumDef a, DbNumDef b)
		{
			return (b == null) || 
				(
					   a != null && b != null
					&& a.MinValue >= b.MinValue
					&& a.MaxValue <= b.MaxValue
					&& a.DigitShift <= b.DigitShift
					&& a.DigitPrecision - a.DigitShift <= b.DigitPrecision - b.DigitShift
				);
		}


		private static void AddNewDbTypes(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbTypeDef> dbTypeDefsToAdd)
		{
			foreach (DbTypeDef dbTypeDef in dbTypeDefsToAdd)
			{
				dbInfrastructure.AddType (dbTransaction, dbTypeDef);
			}
		}


		private static void UpdateDbTypesKey(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbTable> newTables)
		{
			// NOTE This method might become incorrect if we allow the alteration of types and must
			// be changed if that ever happens.
			// Marc

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
		}


		private static void RemoveOldDbTypes(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbTypeDef> dbTypeDefsToRemove)
		{
			foreach (DbTypeDef dbTypeDef in dbTypeDefsToRemove)
			{
				dbInfrastructure.RemoveType (dbTransaction, dbTypeDef);
			}
		}


		private static void AddNewDbTables(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbTable> dbTablesToAdd)
		{
			dbInfrastructure.AddTables (dbTransaction, dbTablesToAdd);
		}


		private static void RemoveOldDbTables(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbTable> dbTablesToRemove)
		{
			foreach (DbTable dbTable in dbTablesToRemove)
			{
				dbInfrastructure.RemoveTable (dbTransaction, dbTable);
			}
		}


		private static void AddNewTableColumns(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbColumn> dbColumnsToAdd)
		{
			foreach (DbColumn dbColumn in dbColumnsToAdd)
			{
				string dbTableName = dbColumn.Table.Name;
				DbTable dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);

				dbInfrastructure.AddColumnToTable (dbTransaction, dbTable, dbColumn);
			}
		}


		private static void RemoveOldTableColumns(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<DbColumn> dbColumnsToRemove)
		{
			foreach (DbColumn dbColumn in dbColumnsToRemove)
			{
				string dbTableName = dbColumn.Table.Name;
				DbTable dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);

				dbInfrastructure.RemoveColumnFromTable (dbTransaction, dbTable, dbColumn);
			}
		}


		private static void AddNewIndexes(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<System.Tuple<DbTable, DbIndex>> dbIndexesToAdd)
		{
			foreach (var item in dbIndexesToAdd)
			{
				DbTable table = item.Item1;
				DbIndex index = item.Item2;

				dbInfrastructure.AddIndexToTable (dbTransaction, table, index);
			}
		}


		private static void AlterIndexes(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, List<System.Tuple<DbTable, DbIndex>> dbIndexesToAlter)
		{
			DbSchemaUpdater.RemoveOldIndexes (dbInfrastructure, dbTransaction, dbIndexesToAlter);
			DbSchemaUpdater.AddNewIndexes (dbInfrastructure, dbTransaction, dbIndexesToAlter);
		}


		private static void RemoveOldIndexes(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<System.Tuple<DbTable, DbIndex>> dbIndexesToRemove)
		{
			foreach (var item in dbIndexesToRemove)
			{
				DbTable table = item.Item1;
				DbIndex index = item.Item2;

				dbInfrastructure.RemoveIndexFromTable (dbTransaction, table, index);
			}
		}


		private static void ExecutePart1OfColumnAlteration(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<System.Tuple<DbColumn, DbColumn>> dbColumsToAlter)
		{
			foreach (var item in dbColumsToAlter)
			{
				var oldColumn = item.Item1;
				var newColumn = item.Item2;

				var dbTableName = oldColumn.Table.Name;
				var dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);

				string tmpColumnName = oldColumn.Name + "TMP";
				dbInfrastructure.RenameTableColumn (dbTable, oldColumn, tmpColumnName);

				dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);
				var tmpColumn = dbTable.Columns[tmpColumnName];

				dbInfrastructure.AddColumnToTable (dbTable, newColumn);
			}
		}


		private static void ExecutePart2OfColumnAlteration(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<System.Tuple<DbColumn, DbColumn>> dbColumsToAlter)
		{
			foreach (var item in dbColumsToAlter)
			{
				var oldColumn = item.Item1;
				var newColumn = item.Item2;

				var dbTableName = oldColumn.Table.Name;
				var dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);

				var tmpColumnName = oldColumn.Name + "TMP";
				var tmpColumn = dbTable.Columns[tmpColumnName];

				var sqlTableName = dbTable.GetSqlName ();
				var sqlTmpColumnName = tmpColumn.GetSqlName ();
				var sqlNewColumnName = newColumn.GetSqlName ();
				
				if (oldColumn.IsNullable && !newColumn.IsNullable)
				{
					DbSchemaUpdater.SetDefaultValueInNullCells (dbInfrastructure, dbTransaction, sqlTableName, sqlTmpColumnName, newColumn.Type);
				}

				DbSchemaUpdater.CopyValues (dbInfrastructure, dbTransaction, sqlTableName, sqlTmpColumnName, sqlNewColumnName);
			}
		}


		private static void SetDefaultValueInNullCells(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, string sqlTableName, string sqlTmpColumnName, DbTypeDef type)
		{
			var sqlFields = new SqlFieldList (){};

			SqlField sqlField = DbSchemaUpdater.GetConstantWithDefaultValue (type);

			sqlField.Alias = sqlTmpColumnName;

			sqlFields.Add (sqlField);

			var sqlConditions = new SqlFieldList ()
            {
                SqlField.CreateFunction
                (
                    new SqlFunction
                    (
                    	SqlFunctionCode.CompareIsNull,
                    	SqlField.CreateAliasedName(sqlTmpColumnName, sqlTmpColumnName)
                    )
                )
            };

			dbTransaction.SqlBuilder.UpdateData (sqlTableName, sqlFields, sqlConditions);
			dbInfrastructure.ExecuteSilent (dbTransaction);
		}


		private static SqlField GetConstantWithDefaultValue(DbTypeDef type)
		{
			switch (type.RawType)
			{
				case DbRawType.Boolean:
					return SqlField.CreateConstant (false, DbRawType.Boolean);

				case DbRawType.ByteArray:
					return SqlField.CreateConstant (new byte[0], DbRawType.ByteArray);

				case DbRawType.Date:
					return SqlField.CreateConstant (new Date (), DbRawType.Date);

				case DbRawType.DateTime:
					return SqlField.CreateConstant (new System.DateTime (), DbRawType.DateTime);

				case DbRawType.Guid:
					return SqlField.CreateConstant (new System.Guid (), DbRawType.Guid);

				case DbRawType.Int16:
					return SqlField.CreateConstant ((short) 0, DbRawType.Int16);

				case DbRawType.Int32:
					return SqlField.CreateConstant ((short) 0, DbRawType.Int32);

				case DbRawType.Int64:
					return SqlField.CreateConstant ((short) 0, DbRawType.Int64);

				case DbRawType.LargeDecimal:
					return SqlField.CreateConstant ((decimal) 0, DbRawType.LargeDecimal);

				case DbRawType.SmallDecimal:
					return SqlField.CreateConstant ((decimal) 0, DbRawType.SmallDecimal);

				case DbRawType.String:
					return SqlField.CreateConstant ("", DbRawType.String);

				case DbRawType.Time:
					return SqlField.CreateConstant (new Time(), DbRawType.Time);

				default:
					throw new System.NotImplementedException ();
			}
		}


		private static void CopyValues(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, string sqlTableName, string sqlTmpColumnName, string sqlNewColumnName)
		{
			var sqlFields = new SqlFieldList ()
            {
                SqlField.CreateAliasedName (sqlTmpColumnName, sqlNewColumnName)
            };
			var sqlConditions = new SqlFieldList ();

			dbTransaction.SqlBuilder.UpdateData (sqlTableName, sqlFields, sqlConditions);
			dbInfrastructure.ExecuteSilent (dbTransaction);
		}


		private static void ExecutePart3OfColumnAlteration(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, IEnumerable<System.Tuple<DbColumn, DbColumn>> dbColumsToAlter)
		{
			foreach (var item in dbColumsToAlter)
			{
				var oldColumn = item.Item1;
				var newColumn = item.Item2;

				var dbTableName = oldColumn.Table.Name;
				var dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, dbTableName);

				string tmpColumnName = oldColumn.Name + "TMP";
				var tmpColumn = dbTable.Columns[tmpColumnName];

				dbInfrastructure.RemoveColumnFromTable (dbTable, tmpColumn);
			}
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
				INameComparer.Instance
			);
		}
		

	}


}
