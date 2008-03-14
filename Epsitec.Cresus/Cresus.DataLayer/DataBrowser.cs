//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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


#if false
		public IEnumerable<object[]> QueryByExample<T>(DbTransaction transaction, T example, DataQuery query) where T : AbstractEntity, new ()
		{
			return this.QueryByExample (transaction, (AbstractEntity) example, query);
		}
#endif

		public IEnumerable<object[]> QueryByExample(DbTransaction transaction, AbstractEntity example, DataQuery query)
		{
			Druid rootEntityId = example.GetEntityStructuredTypeId ();

			DataQuery copy = new DataQuery ();

			copy.Distinct = query.Distinct;

			foreach (DataQueryColumn queryColumn in query.Columns)
			{
				EntityFieldPath fieldPath = queryColumn.FieldPath;

				System.Diagnostics.Debug.Assert (fieldPath.IsRelative);
				System.Diagnostics.Debug.Assert (fieldPath.ContainsIndex == false);

				EntityFieldPath absPath = EntityFieldPath.CreateAbsolutePath (rootEntityId, fieldPath);

				copy.Columns.Add (new DataQueryColumn (absPath, queryColumn.SortOrder));
			}

			using (DbReader reader = this.CreateReader (copy))
			{
				if (example != null)
				{
					EntityContext context = example.GetEntityContext ();

					foreach (string id in context.GetDefinedFieldIds (example))
					{
						AbstractType fieldType  = context.GetFieldType (example, id) as AbstractType;
						object       fieldValue = example.InternalGetValue (id);

						System.Diagnostics.Debug.Assert (fieldType != null);
						System.Diagnostics.Debug.Assert (fieldType.SystemType != null);
						System.Diagnostics.Debug.WriteLine (string.Format ("Field {0} contains {1} (type {2})", id, fieldValue, fieldType.SystemType.Name));

						if (fieldType.TypeCode == TypeCode.String)
						{
							string textValue = fieldValue as string;

							if (string.IsNullOrEmpty (textValue))
							{
								continue;
							}

							DbSelectCondition condition   = new DbSelectCondition (this.infrastructure.Converter);
							EntityFieldPath   fieldPath   = EntityFieldPath.CreateAbsolutePath (rootEntityId, id);
							DbTableColumn     tableColumn = this.GetTableColumn (fieldPath, rootEntityId, id);

							string pattern = DbSqlStandard.ConvertToCompareLikeWildcards (this.infrastructure.DefaultSqlBuilder, textValue);

							condition.AddCondition (tableColumn, DbCompare.LikeEscape, pattern);
							reader.AddCondition (condition);

							System.Diagnostics.Debug.WriteLine ("Condition : " + pattern);
						}
					}
				}

				reader.CreateDataReader (transaction);

				foreach (object[] values in reader.Rows)
				{
					yield return values;
				}
			}
		}


		private DbReader CreateReader(DataQuery query)
		{
			DbReader reader = new DbReader (this.infrastructure);

			List<DbTableColumn> tableColumns = new List<DbTableColumn> ();
			
			foreach (DataQueryColumn queryColumn in query.Columns)
			{
				EntityFieldPath fieldPath = queryColumn.FieldPath;

				System.Diagnostics.Debug.Assert (fieldPath.IsAbsolute);
				System.Diagnostics.Debug.Assert (fieldPath.ContainsIndex == false);
				
				Druid  dataEntityId;
				string dataFieldId;

				if (fieldPath.Navigate (out dataEntityId, out dataFieldId) == false)
				{
					throw new System.ArgumentException ("Cannot resolve field " + fieldPath.ToString ());
				}

				DbTableColumn tableColumn = this.GetTableColumn (fieldPath, dataEntityId, dataFieldId);

				tableColumns.Add (tableColumn);
				reader.AddQueryField (tableColumn);

				switch (queryColumn.SortOrder)
				{
					case DataQuerySortOrder.None:
						break;

					case DataQuerySortOrder.Ascending:
						reader.AddSortOrder (tableColumn, SqlSortOrder.Ascending);
						break;

					case DataQuerySortOrder.Descending:
						reader.AddSortOrder (tableColumn, SqlSortOrder.Descending);
						break;

					default:
						throw new System.NotSupportedException ("Unsupported sort order");
				}
			}

			reader.SelectPredicate = query.Distinct ? SqlSelectPredicate.Distinct : SqlSelectPredicate.All;

			return reader;
		}

		private DbTableColumn GetTableColumn(EntityFieldPath fieldPath, Druid dataEntityId, string dataFieldId)
		{
			DbTableColumn tableColumn;
			DbTable  tableDef   = this.schemaEngine.FindTableDefinition (dataEntityId);
			string   columnName = this.schemaEngine.GetDataColumnName (dataFieldId);
			DbColumn columnDef  = tableDef == null ? null : tableDef.Columns[columnName];

			System.Diagnostics.Debug.Assert (tableDef != null);
			System.Diagnostics.Debug.Assert (columnDef != null);
			System.Diagnostics.Debug.Assert (tableDef == columnDef.Table);

			tableColumn = new DbTableColumn (columnDef);

			tableColumn.TableAlias  = fieldPath.GetParentPath ().ToString ();
			tableColumn.ColumnAlias = fieldPath.ToString ();
			return tableColumn;
		}


		
		
		readonly DbInfrastructure infrastructure;
		readonly SchemaEngine schemaEngine;
	}
}
