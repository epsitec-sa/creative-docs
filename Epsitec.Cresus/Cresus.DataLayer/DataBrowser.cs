//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace Epsitec.Cresus.DataLayer
{
	
	/// <summary>
	/// The <c>DataBrowser</c> class provides sequential read access to the
	/// database.
	/// </summary>
	public class DataBrowser
	{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DataBrowser"/> class.
		/// </summary>
		/// <param name="infrastructure">The database infrastructure.</param>
		public DataBrowser(DbInfrastructure infrastructure, DataContext dataContext)
		{
			this.DbInfrastructure = infrastructure;
			this.DataContext = dataContext;
			this.SchemaEngine   = SchemaEngine.GetSchemaEngine (this.DbInfrastructure) ?? new SchemaEngine (this.DbInfrastructure);
		}

		public SchemaEngine SchemaEngine
		{
			get;
			private set;
		}

		public DataContext DataContext
		{
			get;
			private set;
		}

		public DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		public IEnumerable<EntityType> QueryByExample<EntityType>(EntityType example) where EntityType : AbstractEntity, new ()
		{
			Druid EntityId = this.DataContext.EntityContext.CreateEmptyEntity<EntityType> ().GetEntityStructuredTypeId ();
			Druid baseEntityId =  this.DataContext.EntityContext.GetBaseEntityId (EntityId);

			DataQuery dataQuery = this.BuildQuery (EntityId);
			DataTable dataTable = this.BuildDataTable (EntityId);

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				EntityFieldPath typePath = EntityFieldPath.CreateAbsolutePath (baseEntityId, DataBrowser.FieldPathType);

				foreach (DataBrowserRow dataBrowserRow in this.QueryByExample (transaction, example, dataQuery))
				{
					DbKey rowKey = dataBrowserRow.Keys[0];
					Druid realEntityTypeId = Druid.FromLong ((long) dataBrowserRow[typePath]);
					DataRow dataRow = this.BuildDataRow (dataTable, dataBrowserRow, typePath);

					yield return this.DataContext.ResolveEntity (realEntityTypeId, EntityId, baseEntityId, rowKey, dataRow) as EntityType;
				}

				transaction.Commit ();
			}

			dataTable.Dispose ();
		}

		private DataQuery BuildQuery(Druid entityId)
		{
			DataQuery dataQuery = new DataQuery ()
			{
				Distinct = true,
			};

			foreach (DataQueryColumn dataQueryColumn in this.BuildQueryColumns (entityId))
			{
				dataQuery.Columns.Add (dataQueryColumn);
			}

			foreach (DataQueryJoin dataQueryJoin in this.BuildQueryJoins (entityId))
			{
				dataQuery.Joins.Add (dataQueryJoin);
			}

			return dataQuery;
		}

		private IEnumerable<DataQueryColumn> BuildQueryColumns(Druid entityId)
		{
			Druid currentId = entityId;

			while (currentId.IsValid)
			{
				foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityFieldDefinitions (currentId))
				{
					if (field.Relation == FieldRelation.None && field.Membership == FieldMembership.Local && field.Expression == null)
					{
						yield return new DataQueryColumn (EntityFieldPath.CreateAbsolutePath (currentId, field.Id));
					}
				}

				Druid nextId = (this.DataContext.EntityContext.GetStructuredType (currentId) as StructuredType).BaseTypeId;

				if (!nextId.IsValid)
				{
					yield return new DataQueryColumn (EntityFieldPath.CreateAbsolutePath (currentId, DataBrowser.FieldPathType));
				}

				currentId = nextId;
			}
		}

		private IEnumerable<DataQueryJoin> BuildQueryJoins(Druid entityId)
		{
			Druid leftId = entityId;
			Druid rightId = (this.DataContext.EntityContext.GetStructuredType (entityId) as StructuredType).BaseTypeId;

			while (rightId.IsValid)
			{
				DataQueryColumn left = new DataQueryColumn (EntityFieldPath.CreateAbsolutePath (leftId, DataBrowser.FieldPathId));
				DataQueryColumn right = new DataQueryColumn (EntityFieldPath.CreateAbsolutePath (rightId, DataBrowser.FieldPathId));

				SqlJoinCode type = SqlJoinCode.Inner;

				yield return new DataQueryJoin (left, right, type);

				leftId = rightId;
				rightId = (this.DataContext.EntityContext.GetStructuredType (rightId) as StructuredType).BaseTypeId;
			}
		}

		private DataTable BuildDataTable(Druid entityId)
		{
			DataTable dataTable = new DataTable ();

			foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityFieldDefinitions (entityId))
			{
				dataTable.Columns.Add (new DataColumn (this.DataContext.SchemaEngine.GetDataColumnName (field.Id)));
			}

			return dataTable;
		}

		private DataRow BuildDataRow(DataTable dataTable, DataBrowserRow dataBrowserRow, EntityFieldPath typePath)
		{
			DataRow dataRow = dataTable.NewRow ();

			foreach (DataQueryColumn column in dataBrowserRow.Query.Columns)
			{
				if (column.FieldPath != typePath)
				{
					object value = dataBrowserRow[column];

					dataRow[this.DataContext.SchemaEngine.GetDataColumnName (column.FieldPath.Fields.Last ())] = value;
				}
			}

			return dataRow;
		}


		/// <summary>
		/// Queries the database by example and returns a collection of data
		/// rows.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="example">The example entity.</param>
		/// <param name="query">The query specification.</param>
		/// <returns>The data rows for the query.</returns>
		public IEnumerable<DataBrowserRow> QueryByExample(DbTransaction transaction, AbstractEntity example, DataQuery query)
		{
			Druid rootEntityId = example.GetEntityStructuredTypeId ();
			
			DataQuery copy = query.CreateAbsoluteCopy(rootEntityId);
			
			using (DbReader reader = this.CreateReader (copy))
			{
				if (example != null)
				{
					EntityContext context = example.GetEntityContext ();
					
					IFieldPropertyStore fieldProperties = example as IFieldPropertyStore;

					foreach (string id in context.GetDefinedFieldIds (example))
					{
						AbstractType fieldType  = context.GetFieldType (example, id) as AbstractType;
						object       fieldValue = example.InternalGetValue (id);

						System.Diagnostics.Debug.Assert (fieldType != null);
						System.Diagnostics.Debug.WriteLine (string.Format ("Field {0} contains {1} (type {2})", id, fieldValue, fieldType.SystemType == null ? "<null>" : fieldType.SystemType.Name));

						if (fieldType.TypeCode == TypeCode.String)
						{
							IStringType fieldStringType = fieldType as IStringType;
							string      textValue       = fieldValue as string;

							if (string.IsNullOrEmpty (textValue))
							{
								continue;
							}

							DbSelectCondition condition   = new DbSelectCondition (this.DbInfrastructure.Converter);
							EntityFieldPath   fieldPath   = EntityFieldPath.CreateAbsolutePath (rootEntityId, id);
							DbTableColumn     tableColumn = this.GetTableColumn (fieldPath, rootEntityId, id);

							if (tableColumn == null)
							{
								System.Diagnostics.Debug.WriteLine (string.Format ("Error: field {0} does not map to a column", id));
								continue;
							}
							
							StringSearchBehavior     searchBehavior     = fieldStringType.DefaultSearchBehavior;
							StringComparisonBehavior comparisonBehavior = fieldStringType.DefaultComparisonBehavior;

							//	If the provided example implements IFieldPropertyStore, check
							//	if special properties are attached to the current field :
							
							if (fieldProperties != null)
							{
								if (fieldProperties.ContainsValue (id, StringType.DefaultSearchBehaviorProperty))
								{
									searchBehavior = (StringSearchBehavior) fieldProperties.GetValue (id, StringType.DefaultSearchBehaviorProperty);
								}
								if (fieldProperties.ContainsValue (id, StringType.DefaultComparisonBehaviorProperty))
								{
									comparisonBehavior = (StringComparisonBehavior) fieldProperties.GetValue (id, StringType.DefaultComparisonBehaviorProperty);
								}
							}

							string pattern = this.CreateSearchPattern (textValue, searchBehavior);

							if (pattern.Contains (DbSqlStandard.CompareLikeEscape))
							{
								condition.AddCondition (tableColumn, DbCompare.LikeEscape, pattern);
							}
							else
							{
								condition.AddCondition (tableColumn, DbCompare.Like, pattern);
							}
							
							reader.AddCondition (condition);

							System.Diagnostics.Debug.WriteLine ("Condition : " + pattern);
						}
					}
				}

				//	Execute the reader and gets back all (flattened) rows, including their
				//	table row keys.

				reader.CreateDataReader (transaction);

				DataQueryResult queryResult = new DataQueryResult (query, reader.Tables.Select (table => table.CaptionId));

				foreach (var row in reader.Rows)
				{
					yield return new DataBrowserRow (queryResult, row);
				}
			}
		}


		/// <summary>
		/// Creates the SQL LIKE compatible search pattern.
		/// </summary>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="searchBehavior">The search behavior.</param>
		/// <returns>The SQL LIKE compatible search pattern.</returns>
		private string CreateSearchPattern(string searchPattern, StringSearchBehavior searchBehavior)
		{
			string pattern;

			switch (searchBehavior)
			{
				case StringSearchBehavior.ExactMatch:
					pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					break;

				case StringSearchBehavior.WildcardMatch:
					pattern = DbSqlStandard.ConvertToCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					break;

				case StringSearchBehavior.MatchStart:
					pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					pattern = string.Concat (pattern, "%");
					break;

				case StringSearchBehavior.MatchEnd:
					pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					pattern = string.Concat ("%", pattern);
					break;

				case StringSearchBehavior.MatchAnywhere:
					pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					pattern = string.Concat ("%", pattern, "%");
					break;

				default:
					throw new System.ArgumentException (string.Format ("Unsupported search behavior {0}", searchBehavior));
			}
			
			return pattern;
		}

		/// <summary>
		/// Creates the database reader and sets it up based on the query
		/// definition.
		/// </summary>
		/// <param name="query">The query definition.</param>
		/// <returns>The database reader.</returns>
		private DbReader CreateReader(DataQuery query)
		{
			DbReader reader = new DbReader (this.DbInfrastructure);

			foreach (DataQueryColumn queryColumn in query.Columns)
			{
				DbTableColumn tableColumn = this.GetTableColumn (queryColumn);

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

			foreach (DataQueryJoin join in query.Joins)
			{
				DbTableColumn leftColumn = this.GetTableColumn (join.LeftColumn);
				DbTableColumn rightColumn = this.GetTableColumn (join.RightColumn);

				reader.AddJoin (leftColumn, rightColumn, join.Type);
			}

			reader.SelectPredicate = query.Distinct ? SqlSelectPredicate.Distinct : SqlSelectPredicate.All;
			reader.IncludeRowKeys  = true;

			return reader;
		}

		private DbTableColumn GetTableColumn(DataQueryColumn column)
		{
			EntityFieldPath fieldPath = column.FieldPath;

			System.Diagnostics.Debug.Assert (fieldPath.IsAbsolute);
			System.Diagnostics.Debug.Assert (fieldPath.ContainsIndex == false);

			Druid  dataEntityId;
			string dataFieldId;

			if (fieldPath.Navigate (out dataEntityId, out dataFieldId) == false)
			{
				throw new System.ArgumentException ("Cannot resolve field " + fieldPath.ToString ());
			}

			return this.GetTableColumn (fieldPath, dataEntityId, dataFieldId);
		}

		/// <summary>
		/// Gets the table/column tuple for the specified field.
		/// </summary>
		/// <param name="fieldPath">The field path (used to get the containing table alias).</param>
		/// <param name="dataEntityId">The data entity id (used to get the table definition).</param>
		/// <param name="dataFieldId">The data field id (used to get the column definition).</param>
		/// <returns>The table/column tuple.</returns>
		private DbTableColumn GetTableColumn(EntityFieldPath fieldPath, Druid dataEntityId, string dataFieldId)
		{
			Druid id = dataEntityId;
		again:
			DbTableColumn tableColumn;
			DbTable  tableDef   = this.SchemaEngine.FindTableDefinition (id);
			string   columnName = this.SchemaEngine.GetDataColumnName (dataFieldId);
			DbColumn columnDef  = tableDef == null ? null : tableDef.Columns[columnName];

			if (columnDef == null)
			{
				StructuredType entityType = EntityContext.Current.GetStructuredType (id) as StructuredType;
				Druid baseTypeId = entityType.BaseTypeId;

				if (baseTypeId.IsValid)
				{
					id = baseTypeId;
					goto again;
				}
				
				return null;
			}
			
			System.Diagnostics.Debug.Assert (tableDef != null);
			System.Diagnostics.Debug.Assert (columnDef != null);
			System.Diagnostics.Debug.Assert (tableDef == columnDef.Table);

			tableColumn = new DbTableColumn (columnDef);

			tableColumn.TableAlias  = fieldPath.GetParentPath ().ToString ();
			tableColumn.ColumnAlias = fieldPath.ToString ();

			if (tableColumn.TableAlias.Length == 0)
			{
				tableColumn.TableAlias = fieldPath.GetParentPath ().EntityId.ToString ();
			}
			
			return tableColumn;
		}

		private static EntityFieldPath FieldPathId = EntityFieldPath.CreateRelativePath ("[" + Tags.ColumnId + "]");

		private static EntityFieldPath FieldPathType = EntityFieldPath.CreateRelativePath ("[" + Tags.ColumnInstanceType + "]");

	}

}
