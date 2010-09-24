using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database
{


	// TODO Comment this class.
	// Marc


	internal static class DbTableComparer
	{


		public static bool AreEqual(DbTable a, DbTable b)
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
					&& DbTableComparer.AreDbTableEqual (a, b);
			}

			return same;
		}


		private static bool AreDbTableEqual(DbTable a, DbTable b)
		{
			return DbTableComparer.AreDbTableValuesEqual (a, b)
				&& DbTableComparer.AreDbTableLocalizationsEqual (a, b)
				&& DbTableComparer.AreDbTablePrimaryKeysEqual (a, b)
				&& DbTableComparer.AreDbTableForeignKeysEqual (a, b)
				&& DbTableComparer.AreDbTableIndexesEqual (a, b)
				&& DbTableComparer.AreDbTableColumnsEqual (a, b);
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
				&& DbTableComparer.CompareUnOrderedLists (a.PrimaryKeys.ToList (),b.PrimaryKeys.ToList (),DbTableComparer.AreDbColumnEqual);
		}


		private static bool AreDbTableForeignKeysEqual(DbTable a, DbTable b)
		{
			return DbTableComparer.CompareUnOrderedLists (a.ForeignKeys.ToList(),b.ForeignKeys.ToList(),DbTableComparer.AreDbForeignKeyEqual);
		}


		private static bool AreDbTableIndexesEqual(DbTable a, DbTable b)
		{
			return a.HasIndexes == b.HasIndexes
				&& DbTableComparer.CompareUnOrderedLists (a.Indexes.ToList (), b.Indexes.ToList (), DbTableComparer.AreDbIndexEqual);
		}


		private static bool AreDbTableLocalizationsEqual(DbTable a, DbTable b)
		{
			return a.LocalizationCount == b.LocalizationCount
				&& DbTableComparer.CompareUnOrderedLists (a.Localizations.ToList (), b.Localizations.ToList (), string.Equals);
		}


		private static bool AreDbTableColumnsEqual(DbTable a, DbTable b)
		{
			return DbTableComparer.CompareUnOrderedLists (a.Columns.ToList (), b.Columns.ToList (), DbTableComparer.AreDbColumnEqual);
		}


		private static bool AreDbForeignKeyEqual(DbForeignKey a, DbForeignKey b)
		{
			return DbTableComparer.CompareOrderedArrays (a.Columns, b.Columns, DbTableComparer.AreDbColumnEqual);
		}


		private static bool AreDbIndexEqual(DbIndex a, DbIndex b)
		{
			return a.SortOrder == b.SortOrder
				&& DbTableComparer.CompareOrderedArrays (a.Columns,b.Columns,DbTableComparer.AreDbColumnEqual);
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
