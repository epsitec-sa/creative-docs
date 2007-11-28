//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public class DataBrowser
	{
		public DataBrowser(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.schemaEngine = SchemaEngine.GetSchemaEngine (this.infrastructure) ?? new SchemaEngine (this.infrastructure);
		}


		public void QueryByExample<T>(T example, IEnumerable<EntityFieldPath> outputFields) where T : AbstractEntity, new ()
		{
			this.QueryByExample ((AbstractEntity) example, outputFields);
		}

		public void QueryByExample(AbstractEntity example, IEnumerable<EntityFieldPath> outputFields)
		{
			Druid rootEntityId = example.GetEntityStructuredTypeId ();

			DataQuery query = new DataQuery ();

			foreach (EntityFieldPath fieldPath in outputFields)
			{
				System.Diagnostics.Debug.Assert (fieldPath.IsRelative);
				System.Diagnostics.Debug.Assert (fieldPath.ContainsIndex == false);

				query.OutputFields.Add (EntityFieldPath.CreateAbsolutePath (rootEntityId, fieldPath));
			}
		}


		public void ExecuteQuery(DataQuery query)
		{
			List<DbTableColumn> tableColumns = new List<DbTableColumn> ();
			
			foreach (EntityFieldPath fieldPath in query.OutputFields)
			{
				System.Diagnostics.Debug.Assert (fieldPath.IsAbsolute);
				System.Diagnostics.Debug.Assert (fieldPath.ContainsIndex == false);
				
				Druid  dataEntityId;
				string dataFieldId;

				if (fieldPath.Navigate (out dataEntityId, out dataFieldId) == false)
				{
					throw new System.ArgumentException ("Cannot resolve field " + fieldPath.ToString ());
				}

				DbTable  tableDef   = this.schemaEngine.FindTableDefinition (dataEntityId);
				string   columnName = this.schemaEngine.GetDataColumnName (dataFieldId);
				DbColumn columnDef  = tableDef == null ? null : tableDef.Columns[columnName];

				System.Diagnostics.Debug.Assert (tableDef != null);
				System.Diagnostics.Debug.Assert (columnDef != null);
				System.Diagnostics.Debug.Assert (tableDef == columnDef.Table);
				
				DbTableColumn tableColumn = new DbTableColumn (columnDef);

				tableColumn.TableAlias  = fieldPath.GetParentPath ().ToString ();
				tableColumn.ColumnAlias = fieldPath.ToString ();

				tableColumns.Add (tableColumn);
			}

			
		}


		
		
		readonly DbInfrastructure infrastructure;
		readonly SchemaEngine schemaEngine;
	}
}
