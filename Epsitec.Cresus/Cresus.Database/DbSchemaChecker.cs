using Epsitec.Common.Support.Extensions;


using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>DbSchemaChecker</c> class provides some functions used to compare schemas and
	/// <see cref="DbTable"/>.
	/// </summary>
	internal static class DbSchemaChecker
	{


		/// <summary>
		/// Checks that each given <see cref="DbTable"/> is defined exactly "as is" in the database.
		/// The equality between two <see cref="DbTable"/> is defined by the
		/// <see cref="DbSchemaChecker.CheckTables"/> method.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <param name="schema">The <see cref="DbTable"/> to check.</param>
		/// <returns><c>true</c> if the sequence of <see cref="DbTable"/> si defined in the database, <c>false</c> if it is not.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="schema"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="schema"/> is contains <c>null</c> items.</exception>
		public static bool CheckSchema(DbInfrastructure dbInfrastructure, List<DbTable> schema)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			schema.ThrowIfNull ("schema");
			schema.ThrowIf (s => s.Contains (null), "schema cannot contain null items");

			bool ok = true;

			for (int i = 0; ok && i < schema.Count; i++)
			{
				DbTable expected = schema[i];
				DbTable actual = dbInfrastructure.ResolveDbTable (expected.Name);

				ok = DbSchemaChecker.CheckTables (expected, actual);
			}

			return ok;
		}


		/// <summary>
		/// Checks that both <see cref="DbTable"/> are equal. The equality is checked by value for the
		/// values of the table definition, the keys, the indexes and the columns.
		/// </summary>
		/// <remarks>
		/// This method is designed to be called with valid <see cref="DbTable"/> objects. If you call
		/// it with strange arguments, like a <see cref="DbTable"/> whose primary keys are columns of
		/// another <see cref="DbTable"/> or a <see cref="DbTable"/> whose column's table is another
		/// <see cref="DbTable"/>, it might give incorrect results. However, these kind of <see cref="DbTable"/>
		/// clearly do not make any sense, so if your <see cref="DbTable"/> make sense, they will be
		/// processed properly by this method.
		/// </remarks>
		/// <param name="a">The first <see cref="DbTable"/> to compare.</param>
		/// <param name="b">The second <see cref="DbTable"/> to compare.</param>
		/// <returns><c>true</c> if both <see cref="DbTable"/> are equal, <c>false</c> if they are not.</returns>
		public static bool CheckTables(DbTable a, DbTable b)
		{
			return (a == null && b == null)
				|| (a != null && b != null && DbSchemaChecker.AreDbTableEqual (a, b));
		}


		/// <summary>
		/// Checks that both <see cref="DbTable"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="DbTable"/> to compare.</param>
		/// <param name="b">The second <see cref="DbTable"/> to compare.</param>
		/// <returns><c>true</c> if both <see cref="DbTable"/> are equal, <c>false</c> if they are not.</returns>
		private static bool AreDbTableEqual(DbTable a, DbTable b)
		{
			return DbSchemaChecker.AreDbTableValuesEqual (a, b)
				&& DbSchemaChecker.AreDbTableLocalizationsEqual (a, b)
				&& DbSchemaChecker.AreDbTablePrimaryKeysEqual (a, b)
				&& DbSchemaChecker.AreDbTableForeignKeysEqual (a, b)
				&& DbSchemaChecker.AreDbTableIndexesEqual (a, b)
				&& DbSchemaChecker.AreDbTableColumnsEqual (a, b);
		}


		/// <summary>
		/// Checks that the values at the level of the <see cref="DbTable"/> object of both
		/// <see cref="DbTable"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="DbTable"/> whose values to compare.</param>
		/// <param name="b">The second <see cref="DbTable"/> whose values to compare.</param>
		/// <returns><c>true</c> if the values of both <see cref="DbTable"/> are equal, <c>false</c> if they are not.</returns>
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


		/// <summary>
		/// Checks that the primary keys of both <see cref="DbTable"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="DbTable"/> whose primary keys to compare.</param>
		/// <param name="b">The second <see cref="DbTable"/> whose primary keys to compare.</param>
		/// <returns><c>true</c> if the primary keys of both <see cref="DbTable"/> are equal, <c>false</c> if they are not.</returns>
		private static bool AreDbTablePrimaryKeysEqual(DbTable a, DbTable b)
		{
			return a.HasPrimaryKeys == b.HasPrimaryKeys
				&& DbSchemaChecker.CompareUnOrderedLists (a.PrimaryKeys.ToList (),b.PrimaryKeys.ToList (),DbSchemaChecker.AreDbColumnEqual);
		}


		/// <summary>
		/// Checks that the foreign keys of both <see cref="DbTable"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="DbTable"/> whose foreign keys to compare.</param>
		/// <param name="b">The second <see cref="DbTable"/> whose foreign keys to compare.</param>
		/// <returns><c>true</c> if the foreign keys of both <see cref="DbTable"/> are equal, <c>false</c> if they are not.</returns>
		private static bool AreDbTableForeignKeysEqual(DbTable a, DbTable b)
		{
			return DbSchemaChecker.CompareUnOrderedLists (a.ForeignKeys.ToList(),b.ForeignKeys.ToList(),DbSchemaChecker.AreDbForeignKeyEqual);
		}


		/// <summary>
		/// Checks that the indexes of both <see cref="DbTable"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="DbTable"/> whose indexes to compare.</param>
		/// <param name="b">The second <see cref="DbTable"/> whose indexes to compare.</param>
		/// <returns><c>true</c> if the indexes of both <see cref="DbTable"/> are equal, <c>false</c> if they are not.</returns>
		private static bool AreDbTableIndexesEqual(DbTable a, DbTable b)
		{
			return a.HasIndexes == b.HasIndexes
				&& DbSchemaChecker.CompareUnOrderedLists (a.Indexes.ToList (), b.Indexes.ToList (), DbSchemaChecker.AreDbIndexEqual);
		}


		/// <summary>
		/// Checks that the localization of both <see cref="DbTable"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="DbTable"/> whose localization to compare.</param>
		/// <param name="b">The second <see cref="DbTable"/> whose localization to compare.</param>
		/// <returns><c>true</c> if the localization of both <see cref="DbTable"/> are equal, <c>false</c> if they are not.</returns>
		private static bool AreDbTableLocalizationsEqual(DbTable a, DbTable b)
		{
			return a.LocalizationCount == b.LocalizationCount
				&& DbSchemaChecker.CompareUnOrderedLists (a.Localizations.ToList (), b.Localizations.ToList (), string.Equals);
		}


		/// <summary>
		/// Checks that the <see cref="DbColumn"/> of both <see cref="DbTable"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="DbTable"/> whose <see cref="DbColumn"/> to compare.</param>
		/// <param name="b">The second <see cref="DbTable"/> whose <see cref="DbColumn"/> to compare.</param>
		/// <returns><c>true</c> if the <see cref="DbColumn"/> of both <see cref="DbTable"/> are equal, <c>false</c> if they are not.</returns>
		private static bool AreDbTableColumnsEqual(DbTable a, DbTable b)
		{
			return DbSchemaChecker.CompareUnOrderedLists (a.Columns.ToList (), b.Columns.ToList (), DbSchemaChecker.AreDbColumnEqual);
		}


		/// <summary>
		/// Checks that both <see cref="DbForeignKey"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="DbForeignKey"/> to compare.</param>
		/// <param name="b">The second <see cref="DbForeignKey"/> to compare.</param>
		/// <returns><c>true</c> if both <see cref="DbForeignKey"/> are equal, <c>false</c> if they are not.</returns>
		private static bool AreDbForeignKeyEqual(DbForeignKey a, DbForeignKey b)
		{
			return DbSchemaChecker.CompareOrderedArrays (a.Columns, b.Columns, DbSchemaChecker.AreDbColumnEqual);
		}


		/// <summary>
		/// Checks that both <see cref="DbIndex"/> are equal.
		/// </summary>
		/// <param name="a">The first <see cref="DbIndex"/> to compare.</param>
		/// <param name="b">The second <see cref="DbIndex"/> to compare.</param>
		/// <returns><c>true</c> if both <see cref="DbIndex"/> are equal, <c>false</c> if they are not.</returns>
		private static bool AreDbIndexEqual(DbIndex a, DbIndex b)
		{
			return a.SortOrder == b.SortOrder
				&& DbSchemaChecker.CompareOrderedArrays (a.Columns,b.Columns,DbSchemaChecker.AreDbColumnEqual);
		}


		/// <summary>
		/// Checks that both <see cref="DbColumn"/> are equal. The comparison is made by value for the
		/// values of the <see cref="DbColumn"/> object, but their table are compared by name and caption
		/// id.
		/// </summary>
		/// <param name="a">The first <see cref="DbColumn"/> to compare.</param>
		/// <param name="b">The second <see cref="DbColumn"/> to compare.</param>
		/// <returns><c>true</c> if both <see cref="DbColumn"/> are equal, <c>false</c> if they are not.</returns>
		private static bool AreDbColumnEqual(DbColumn a, DbColumn b)
		{
			return a.CaptionId == b.CaptionId
				&& string.Equals (a.Name, b.Name)
				&& string.Equals (a.Comment, b.Comment)
				&& ((a.Table == null && b.Table == null) || (a.Table.Name == b.Table.Name && a.Table.CaptionId == b.Table.CaptionId))
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


		/// <summary>
		/// Checks that two arrays are equal, given a method to compare their element. The order is
		/// taken in account for the comparison.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the array.</typeparam>
		/// <param name="arrayA">The first array to compare.</param>
		/// <param name="arrayB">The second array to compare.</param>
		/// <param name="comparer">A method that can compare two elements of the arrays.</param>
		/// <returns><c>true</c> if both arrays contains the same elements, <c>false</c> if they do not.</returns>
		private static bool CompareOrderedArrays<T>(T[] arrayA, T[] arrayB, System.Func<T, T, bool> comparer)
		{
			bool same = arrayA.Length == arrayB.Length;
			
			for (int i = 0; same && i < arrayA.Length; i++)
			{
				same = comparer (arrayA[i], arrayB[i]);
			}

			return same;
		}


		/// <summary>
		/// Checks that two set of elements are equal, given a method to compare their element. The
		/// order is not taken in account for the comparison, which means that two lists with the same
		/// elements in different orders will be considered are equal.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the sets.</typeparam>
		/// <param name="arrayA">The first set to compare.</param>
		/// <param name="arrayB">The second set to compare.</param>
		/// <param name="comparer">A method that can compare two elements of the sets.</param>
		/// <returns><c>true</c> if both sets contains the same elements, <c>false</c> if they do not.</returns>
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
