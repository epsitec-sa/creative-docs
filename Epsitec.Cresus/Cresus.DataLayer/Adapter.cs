//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public class Adapter
	{
		public static DbTable CreateTableDefinition(DbTransaction transaction, DbInfrastructure infrastructure, StructuredType type)
		{
			DbContext context   = infrastructure.DefaultContext;
			string    tableName = type.Name;
			DbTable   table     = infrastructure.CreateDbTable (tableName, DbElementCat.ManagedUserData, DbRevisionMode.Enabled);
			
			foreach (string id in type.GetFieldIds ())
			{
				StructuredTypeField field = type.GetField (id);

				string typeName = field.Type.Name;

				if (field.Type is IStructuredType)
				{
					//	TODO: handle foreign keys
				}
				else
				{
					DbTypeDef columnType = infrastructure.ResolveDbType (transaction, typeName);
					string    columnName = context.ResourceManager.GetCaption (field.CaptionId).Name;

					if (columnType == null)
					{
						columnType = Adapter.CreateTypeDefinition (transaction, infrastructure, field.Type);
					}

					DbColumn column = new DbColumn (field.CaptionId, columnType);

					System.Diagnostics.Debug.Assert (! column.Type.Key.IsEmpty);
					
					table.Columns.Add (column);
				}
			}

			infrastructure.RegisterNewDbTable (transaction, table);

			return table;
		}

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
	}
}
