using System.Collections.Generic;

using System.Linq;
using System.Collections;


namespace Epsitec.Cresus.Database
{


	// TODO Comment this class.
	// Marc


	internal static class DbSchemaChecker
	{


		public static bool CheckSchema(DbInfrastructure dbInfrastructure, List<DbTable> schema)
		{
			bool ok = true;

			for (int i = 0; ok && i < schema.Count; i++)
			{
				DbTable expected = schema[i];
				DbTable actual = dbInfrastructure.ResolveDbTable (expected.Name);

				ok = DbSchemaChecker.CheckTables (expected, actual);
			}

			return ok;
		}


		public static bool CheckTables(DbTable a, DbTable b)
		{
			bool same;

			if (a == null && b == null)
			{
				same = true;
			}
			else
			{
				same = a != null
					&& b != null
					&& DbSchemaChecker.AreDbTableEqual (a, b);
			}

			return same;
		}


		private static bool AreDbTableEqual(DbTable a, DbTable b)
		{
			return DbSchemaChecker.AreDbTableValuesEqual (a, b)
				&& DbSchemaChecker.AreDbTableLocalizationsEqual (a, b)
				&& DbSchemaChecker.AreDbTablePrimaryKeysEqual (a, b)
				&& DbSchemaChecker.AreDbTableForeignKeysEqual (a, b)
				&& DbSchemaChecker.AreDbTableIndexesEqual (a, b)
				&& DbSchemaChecker.AreDbTableColumnsEqual (a, b);
		}


		private static bool AreDbTableValuesEqual(DbTable a, DbTable b)
		{
			bool same = a.CaptionId == b.CaptionId
				&& string.Equals (a.Name, b.Name)
				&& string.Equals (a.Comment, b.Comment)
				&& a.Category == b.Category
				&& a.RevisionMode == b.RevisionMode;

			if (same && a.Category == DbElementCat.Relation)
			{
				same = string.Equals (a.RelationSourceTableName, b.RelationSourceTableName)
					&& string.Equals (a.RelationTargetTableName, b.RelationTargetTableName);
			}

			return same;
		}


		private static bool AreDbTablePrimaryKeysEqual(DbTable a, DbTable b)
		{
			return a.HasPrimaryKeys == b.HasPrimaryKeys
				&& DbSchemaChecker.CompareUnOrderedLists (a.PrimaryKeys.ToList (),b.PrimaryKeys.ToList (),DbSchemaChecker.AreDbColumnEqual);
		}


		private static bool AreDbTableForeignKeysEqual(DbTable a, DbTable b)
		{
			return DbSchemaChecker.CompareUnOrderedLists (a.ForeignKeys.ToList(),b.ForeignKeys.ToList(),DbSchemaChecker.AreDbForeignKeyEqual);
		}


		private static bool AreDbTableIndexesEqual(DbTable a, DbTable b)
		{
			return a.HasIndexes == b.HasIndexes
				&& DbSchemaChecker.CompareUnOrderedLists (a.Indexes.ToList (), b.Indexes.ToList (), DbSchemaChecker.AreDbIndexEqual);
		}


		private static bool AreDbTableLocalizationsEqual(DbTable a, DbTable b)
		{
			return a.LocalizationCount == b.LocalizationCount
				&& DbSchemaChecker.CompareUnOrderedLists (a.Localizations.ToList (), b.Localizations.ToList (), string.Equals);
		}


		private static bool AreDbTableColumnsEqual(DbTable a, DbTable b)
		{
			return DbSchemaChecker.CompareUnOrderedLists (a.Columns.ToList (), b.Columns.ToList (), DbSchemaChecker.AreDbColumnEqual);
		}


		private static bool AreDbForeignKeyEqual(DbForeignKey a, DbForeignKey b)
		{
			return DbSchemaChecker.CompareOrderedArrays (a.Columns, b.Columns, DbSchemaChecker.AreDbColumnEqual);
		}


		private static bool AreDbIndexEqual(DbIndex a, DbIndex b)
		{
			return a.SortOrder == b.SortOrder
				&& DbSchemaChecker.CompareOrderedArrays (a.Columns,b.Columns,DbSchemaChecker.AreDbColumnEqual);
		}


		private static bool AreDbColumnEqual(DbColumn a, DbColumn b)
		{
			return a.CaptionId == b.CaptionId
				&& string.Equals (a.Name, b.Name)
				&& string.Equals (a.Comment, b.Comment)
				&& ((a.Table == null && b.Table == null) || (a.Table.CaptionId == b.Table.CaptionId))
				&& string.Equals (a.TargetTableName, b.TargetTableName)
				&& string.Equals (a.TargetColumnName, b.TargetColumnName)
				&& a.Category == b.Category
				&& a.ColumnClass == b.ColumnClass
				&& a.Cardinality == b.Cardinality
				&& a.Localization == b.Localization
				&& a.IsPrimaryKey == b.IsPrimaryKey
				&& a.IsForeignKey == b.IsForeignKey
				&& a.IsAutoIncremented == b.IsAutoIncremented
				&& a.RevisionMode == b.RevisionMode
				&& a.Type == b.Type;
		}


		private static bool CompareOrderedArrays<T>(T[] arrayA, T[] arrayB, System.Func<T, T, bool> comparer)
		{
			bool same = arrayA.Length == arrayB.Length;
			
			for (int i = 0; same && i < arrayA.Length; i++)
			{
				same = comparer (arrayA[i], arrayB[i]);
			}

			return same;
		}


		private static bool CompareUnOrderedLists<T>(List<T> listA, List<T> listB, System.Func<T, T, bool> comparer)
		{
			bool same = listA.Count == listB.Count;

			if (same)
			{
				for (int i = 0; same && i < listA.Count; i++)
				{
					T a = listA[i];

					int index = listB.FindIndex (b => comparer (a, b));

					if (index == -1)
					{
						same = false;
					}
					else
					{
						listB.RemoveAt (index);
					}
				}
			}

			return same;
		}


	}


}
