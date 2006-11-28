//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>Adapter</c> class is used to merge the gap between the database
	/// layers and the data binding aware code.
	/// </summary>
	public static class Adapter
	{
		/// <summary>
		/// Creates and registers a table definition based on a structured type.
		/// </summary>
		/// <param name="transaction">The database transaction.</param>
		/// <param name="infrastructure">The database infrastructure.</param>
		/// <param name="type">The structured type used as model.</param>
		/// <returns>The table definition.</returns>
		public static DbTable CreateTableDefinition(DbTransaction transaction, DbInfrastructure infrastructure, StructuredType type)
		{
			List<DbTable> tables = new List<DbTable> ();

			Adapter.CreateTableDefinition (transaction, infrastructure, type, tables);

			if (tables.Count > 1)
			{
				foreach (DbTable table in tables)
				{
					infrastructure.RegisterColumnRelations (transaction, table);
				}
			}
			
			return tables[0];
		}

		/// <summary>
		/// Creates a type definition based on a named type. The named type may
		/// not be a structured type (<c>IStructuredType</c>); for these, use the
		/// <see cref="CreateTableDefinition"/> method instead.
		/// </summary>
		/// <param name="transaction">The database transaction.</param>
		/// <param name="infrastructure">The database infrastructure.</param>
		/// <param name="type">The named type used as model.</param>
		/// <returns>The type definition.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the specified type is derived from <see cref="IStructuredType"/>.</exception>
		public static DbTypeDef CreateTypeDefinition(DbTransaction transaction, DbInfrastructure infrastructure, INamedType type)
		{
			if (type == null)
			{
				return null;
			}

			if (type is IStructuredType)
			{
				throw new System.InvalidOperationException ("Cannot create type definition for structure");
			}

			DbTypeDef typeDef = new DbTypeDef (type);

			infrastructure.RegisterNewDbType (transaction, typeDef);

			System.Diagnostics.Debug.Assert (! typeDef.Key.IsEmpty);

			return typeDef;
		}

		private static void CreateTableDefinition(DbTransaction transaction, DbInfrastructure infrastructure, StructuredType type, List<DbTable> tables)
		{
			DbContext context   = infrastructure.DefaultContext;
			string tableName = context.ResourceManager.GetCaption (type.CaptionId).Name;

			foreach (DbTable iter in tables)
			{
				if (iter.Name == tableName)
				{
					return;
				}
			}

			if (tables.Count > 0)
			{
				DbTable find = infrastructure.ResolveDbTable (transaction, tableName);

				if (find != null)
				{
					tables.Add (find);
					return;
				}
			}

			DbTable table = infrastructure.CreateDbTable (tableName, DbElementCat.ManagedUserData, DbRevisionMode.Enabled);

			tables.Add (table);

			foreach (string id in type.GetFieldIds ())
			{
				StructuredTypeField field = type.GetField (id);

				DbColumn column;
				string typeName = field.Type.Name;

				if (field.Type is IStructuredType)
				{
					StructuredType structuredType = field.Type as StructuredType;
					INullableType nullableType   = field.Type as INullableType;

					string columnName = context.ResourceManager.GetCaption (field.CaptionId).Name;
					string targetName = context.ResourceManager.GetCaption (field.Type.CaptionId).Name;

					if (structuredType == null)
					{
						throw new System.ArgumentException (string.Format ("Field {0} (type {1}) of structure {2} is invalid", columnName, typeName, tableName));
					}

					bool isNullable = nullableType == null ? false : nullableType.IsNullable;

					column = DbTable.CreateRefColumn (transaction, infrastructure, columnName, targetName, isNullable ? DbNullability.Yes : DbNullability.No);

					Adapter.CreateTableDefinition (transaction, infrastructure, structuredType, tables);
				}
				else
				{
					DbTypeDef columnType = infrastructure.ResolveDbType (transaction, typeName);
					string columnName = context.ResourceManager.GetCaption (field.CaptionId).Name;

					if (columnType == null)
					{
						columnType = Adapter.CreateTypeDefinition (transaction, infrastructure, field.Type);
					}

					column = DbTable.CreateUserDataColumn (columnName, columnType);
				}

				System.Diagnostics.Debug.Assert (column != null);
				System.Diagnostics.Debug.Assert (!column.Type.Key.IsEmpty);

				table.Columns.Add (column);
			}

			infrastructure.RegisterNewDbTable (transaction, table);
		}
	}
}
