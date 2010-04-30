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
			this.SchemaEngine = SchemaEngine.GetSchemaEngine (this.DbInfrastructure) ?? new SchemaEngine (this.DbInfrastructure);
			this.DataContext = dataContext;

			this.rootTypeTableAlias = new Stack<string> ();
			this.subTypeTableAlias = new Stack<string> ();
		}


		public DbInfrastructure DbInfrastructure
		{
			get;
			private set;
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


		public IEnumerable<EntityType> QueryByExample<EntityType>(EntityType example) where EntityType : AbstractEntity, new ()
		{
			Druid entityId = example.GetEntityContext ().CreateEmptyEntity<EntityType> ().GetEntityStructuredTypeId ();
			
			DataTable dataTable = this.BuildDataTable (entityId);

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				using (DbReader reader = this.CreateReader (entityId, example))
				{
					reader.CreateDataReader (transaction);
					
					foreach (var row in reader.Rows)
					{
						DbKey rowKey = this.ExtractDbKey (row);
						Druid realEntityId = this.ExtractRealEntityId (row);
						DataRow dataRow = this.ExtractDataRow (dataTable, row);

						yield return this.DataContext.ResolveEntity (realEntityId, entityId, rowKey, dataRow) as EntityType;
					}
				}

				transaction.Commit ();
			}

			dataTable.Dispose ();
		}


		private DataTable BuildDataTable(Druid entityId)
		{
			DataTable dataTable = new DataTable ();

			foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityFieldDefinitions (entityId))
			{
				if (field.Relation == FieldRelation.None && field.Expression == null)
				{
					dataTable.Columns.Add (new DataColumn (this.DataContext.SchemaEngine.GetDataColumnName (field.Id)));
				}
			}

			return dataTable;
		}


		private DataRow ExtractDataRow(DataTable dataTable, DbReader.RowData row)
		{
			DataRow dataRow = dataTable.NewRow ();

			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				string name = dataTable.Columns[i].ColumnName;
				object value = row.Values[i];

				dataRow[name] = value;
			}

			return dataRow;
		}


		private Druid ExtractRealEntityId(DbReader.RowData row)
		{
			long realEntityIdAsLong = (long) row.Values.Last ();

			return Druid.FromLong (realEntityIdAsLong);
		}


		private DbKey ExtractDbKey(DbReader.RowData row)
		{
			return row.Keys[0];
		}


		private DbReader CreateReader(Druid entityId, AbstractEntity example)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = true,
			};

			this.resetTableAliases ();

			this.PushRootTypeTableAlias ();
			this.AddQueryDataForEntity (reader, entityId, example, true);
			this.PopRootTypeTableAlias ();

			return reader;
		}


		private void AddQueryDataForEntity(DbReader reader, Druid entityId, AbstractEntity example, bool getFields)
		{
			List<string> definedFieldIds = example.GetEntityContext ().GetDefinedFieldIds (example).ToList ();
			List<Druid> heritedEntityIds = example.GetEntityContext ().GetHeritedEntityIds (entityId).ToList();

			Druid rootEntityId =  heritedEntityIds.Last ();

			foreach (Druid currentEntityId in heritedEntityIds)
			{
				bool isRootType = currentEntityId == rootEntityId;
				this.PushSubTypeTableAlias (isRootType);
				
				List<StructuredTypeField> localFields = new List<StructuredTypeField> (

					from StructuredTypeField field in example.GetEntityContext ().GetEntityFieldDefinitions (currentEntityId)
					where (field.Membership == FieldMembership.Local) && (field.Expression == null)
					select field

				);

				List<StructuredTypeField> localValueFields = new List<StructuredTypeField> (

					from StructuredTypeField field in localFields
					where field.Relation == FieldRelation.None
					select field

				);

				List<StructuredTypeField> definedLocalFields = new List<StructuredTypeField> (

					from StructuredTypeField field in localFields
					where definedFieldIds.Contains (field.Id)
					select field

				);

				List<StructuredTypeField> definedLocalValueFields = new List<StructuredTypeField>(

					from StructuredTypeField field in definedLocalFields
					where field.Relation == FieldRelation.None
					select field

				);

				bool localQueryFieldExists = getFields && localValueFields.Count > 0;
				bool definedLocalValueFieldExists = definedLocalValueFields.Count () > 0; 
				bool definedFieldExists = definedLocalFields.Count > 0;

				if (!isRootType && (localQueryFieldExists || definedLocalValueFieldExists))
				{
					this.AddSubTypingJoin (reader, currentEntityId, heritedEntityIds.Last ());
				}

				if (definedFieldExists)
				{
					foreach (StructuredTypeField field in definedLocalFields)
					{
						this.AddQueryDataForField (reader, currentEntityId, example, field);
					}
				}

				if (localQueryFieldExists)
				{
					foreach (StructuredTypeField field in localValueFields)
					{
						this.AddQueryField (reader, currentEntityId, field.Id);
					}
				}

				if (getFields && isRootType)
				{
					this.AddQueryField (reader, currentEntityId, DataBrowser.typeColumn);
				}

				this.PopSubTypeTableAlias ();
			}

		}


		private void AddQueryDataForField(DbReader reader, Druid currentEntityId, AbstractEntity example, StructuredTypeField field)
		{
			// TODO Delete conditions
			if (field.Membership == FieldMembership.Local && field.Expression == null)
			{
				switch (field.Relation)
				{
					case FieldRelation.None:
						this.AddQueryDataForValue (reader, currentEntityId, example, field);
						break;

					case FieldRelation.Reference:
						this.AddQueryDataForReference (reader, currentEntityId, example, field);
						break;

					case FieldRelation.Collection:
						this.AddQueryDataForCollection (reader, currentEntityId, example, field);
						break;

					default:
						throw new System.NotSupportedException ();
				}
			}
		}


		private void AddQueryDataForValue(DbReader reader, Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			//IFieldPropertyStore fieldProperties = example as IFieldPropertyStore;

			//string id = field.Id;
			//object fieldValue = example.InternalGetValue (id);
			//AbstractType fieldType = field.Type as AbstractType;

			//System.Diagnostics.Debug.Assert (fieldType != null);
			//System.Diagnostics.Debug.WriteLine (string.Format ("Field {0} contains {1} (type {2})", id, fieldValue, fieldType.SystemType == null ? "<null>" : fieldType.SystemType.Name));

			//if (fieldType.TypeCode == TypeCode.String)
			//{
			//    IStringType fieldStringType = fieldType as IStringType;
			//    string textValue = fieldValue as string;

			//    if (string.IsNullOrEmpty (textValue))
			//    {
			//        return;
			//    }

			//    DbSelectCondition condition = new DbSelectCondition (this.DbInfrastructure.Converter);
			//    EntityFieldPath fieldPath  = EntityFieldPath.CreateRelativePath (id);
			//    DbTableColumn tableColumn = this.GetTableColumn (entityId, fieldPath);

			//    if (tableColumn == null)
			//    {
			//        System.Diagnostics.Debug.WriteLine (string.Format ("Error: field {0} does not map to a column", id));
			//        return;
			//    }

			//    StringSearchBehavior searchBehavior = fieldStringType.DefaultSearchBehavior;
			//    StringComparisonBehavior comparisonBehavior = fieldStringType.DefaultComparisonBehavior;

			//    //	If the provided example implements IFieldPropertyStore, check
			//    //	if special properties are attached to the current field :

			//    if (fieldProperties != null)
			//    {
			//        if (fieldProperties.ContainsValue (id, StringType.DefaultSearchBehaviorProperty))
			//        {
			//            searchBehavior = (StringSearchBehavior) fieldProperties.GetValue (id, StringType.DefaultSearchBehaviorProperty);
			//        }
			//        if (fieldProperties.ContainsValue (id, StringType.DefaultComparisonBehaviorProperty))
			//        {
			//            comparisonBehavior = (StringComparisonBehavior) fieldProperties.GetValue (id, StringType.DefaultComparisonBehaviorProperty);
			//        }
			//    }

			//    string pattern = this.CreateSearchPattern (textValue, searchBehavior);

			//    if (pattern.Contains (DbSqlStandard.CompareLikeEscape))
			//    {
			//        condition.AddCondition (tableColumn, DbCompare.LikeEscape, pattern);
			//    }
			//    else
			//    {
			//        condition.AddCondition (tableColumn, DbCompare.Like, pattern);
			//    }

			//    reader.AddCondition (condition);

			//    System.Diagnostics.Debug.WriteLine ("Condition : " + pattern);
			//}
			//else
			//{
			//    // TODO Add condition when field is not of type string
			//}
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


		private void AddQueryDataForReference(DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField)
		{
			AbstractEntity targetEntity = sourceExample.InternalGetValue (sourceField.Id) as AbstractEntity;

			this.AddQueryDataForRelation (reader, sourceEntityId, Druid.Parse (sourceField.Id), targetEntity);
		}


		private void AddQueryDataForCollection(DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField)
		{
			foreach (object referencedObject in sourceExample.InternalGetFieldCollection (sourceField.Id))
			{
				AbstractEntity targetEntity = referencedObject as AbstractEntity;

				this.AddQueryDataForRelation (reader, sourceEntityId, Druid.Parse(sourceField.Id), targetEntity);
			}
		}


		private void AddQueryDataForRelation(DbReader reader, Druid sourceEntityId, Druid sourceFieldId, AbstractEntity targetEntity)
		{
			Druid targetEntityId = targetEntity.GetEntityStructuredTypeId ();

			this.AddRelationJoin (reader, sourceEntityId, sourceFieldId, targetEntityId);

			this.AddQueryDataForEntity (reader, targetEntityId, targetEntity, false);

			this.PopRootTypeTableAlias ();
			this.PopRootTypeTableAlias ();
		}


		private void AddSubTypingJoin(DbReader reader, Druid subTypeEntityId, Druid rootTypeEntityId)
		{
			string subTypeTableAlias = this.PeekSubTypeTableAlias ();
			string rootTypeTableAlias = this.PeekRootTypeTableAlias ();

			DbTableColumn subEntityColumn = this.GetEntityTableColumn (subTypeEntityId, subTypeTableAlias, DataBrowser.idColumn);
			DbTableColumn superEntityColumn = this.GetEntityTableColumn (rootTypeEntityId, rootTypeTableAlias, DataBrowser.idColumn);
			
			SqlJoinCode type = SqlJoinCode.Inner;

			reader.AddJoin (subEntityColumn, superEntityColumn, type);
		}


		private void AddRelationJoin(DbReader reader, Druid sourceEntityId, Druid sourcefieldId, Druid targetEntityId)
		{
			Druid rootSourceEntityId = this.DataContext.EntityContext.GetBaseEntityId (sourceEntityId);
			Druid rootTargetEntityId = this.DataContext.EntityContext.GetBaseEntityId (targetEntityId);

			string sourceTableAlias = this.PeekRootTypeTableAlias ();
			string relationTableAlias = this.PushRootTypeTableAlias ();
			string targetTableAlias = this.PushRootTypeTableAlias ();

			DbTableColumn sourceColumnId = this.GetEntityTableColumn (rootSourceEntityId, sourceTableAlias, DataBrowser.idColumn);
			DbTableColumn relationSourceColumnId = this.GetRelationTableColumn (sourceEntityId, sourcefieldId, relationTableAlias, DataBrowser.relationSourceColumn);

			DbTableColumn relationTargetColumnId = this.GetRelationTableColumn (sourceEntityId, sourcefieldId, relationTableAlias, DataBrowser.relationTargetColumn);
			DbTableColumn targetColumnId = this.GetEntityTableColumn (rootTargetEntityId, targetTableAlias, DataBrowser.idColumn);

			SqlJoinCode type = SqlJoinCode.Inner;

			reader.AddJoin (sourceColumnId, relationSourceColumnId, type);
			reader.AddJoin (relationTargetColumnId, targetColumnId, type);
		}


		private void AddQueryField(DbReader reader, Druid entityId, string columnName)
		{
			string tableAlias = this.PeekSubTypeTableAlias ();
			DbTableColumn tableColumn = this.GetEntityTableColumn (entityId, tableAlias, columnName);

			reader.AddQueryField (tableColumn);

			// TODO Find a way to give sorting informations. Below is the way it was done
			// before. I probably should use that SearchEntity stuff.

			//switch (queryColumn.SortOrder)
			//{
			//    case DataQuerySortOrder.None:
			//        break;
			//    case DataQuerySortOrder.Ascending:
			//        reader.AddSortOrder (tableColumn, SqlSortOrder.Ascending);
			//        break;
			//    case DataQuerySortOrder.Descending:
			//        reader.AddSortOrder (tableColumn, SqlSortOrder.Descending);
			//        break;
			//    default:
			//        throw new System.NotSupportedException ("Unsupported sort order");
			//}
		}


		private DbTableColumn GetEntityTableColumn(Druid entityId, string entityAlias, string columName)
		{
			DbTable dbTable = this.SchemaEngine.FindTableDefinition (entityId);
			DbColumn dbColumn = dbTable.Columns[this.SchemaEngine.GetDataColumnName (columName)];

			return new DbTableColumn (dbColumn)
			{
				TableAlias = entityAlias,
				ColumnAlias = columName,
			};
		}


		private DbTableColumn GetRelationTableColumn(Druid sourceEntityId, Druid sourceFieldId, string relationAlias, string columName)
		{
			string sourceTableName = this.SchemaEngine.GetDataTableName (sourceEntityId);
			string sourceColumnName = this.SchemaEngine.GetDataColumnName (sourceFieldId.ToString ());
			string relationTableName = DbTable.GetRelationTableName (sourceTableName, sourceColumnName);

			DbTable dbRelationTable = this.DbInfrastructure.ResolveDbTable (relationTableName);
			DbColumn dbColumn = dbRelationTable.Columns[this.SchemaEngine.GetDataColumnName (columName)];

			return new DbTableColumn (dbColumn)
			{
				TableAlias = relationAlias,
				ColumnAlias = columName,
			};
		}

		private void resetTableAliases()
		{
			this.tableAliasNumber = 0;

			this.rootTypeTableAlias.Clear ();
			this.subTypeTableAlias.Clear ();
		}

		private string PushRootTypeTableAlias()
		{
			string newTableAlias = this.GetNewTableAlias ();
			
			this.rootTypeTableAlias.Push (newTableAlias);

			return newTableAlias;
		}

		private string PeekRootTypeTableAlias()
		{
			return this.rootTypeTableAlias.Peek ();
		}

		private void PopRootTypeTableAlias()
		{
			this.rootTypeTableAlias.Pop ();
		}

		private string PushSubTypeTableAlias(bool rootType)
		{
			string newTableAlias = (rootType) ? this.PeekRootTypeTableAlias () : this.GetNewTableAlias ();

			subTypeTableAlias.Push (newTableAlias);

			return newTableAlias;
		}

		private string PeekSubTypeTableAlias()
		{
			return this.subTypeTableAlias.Peek ();
		}

		private void PopSubTypeTableAlias()
		{
			this.subTypeTableAlias.Pop ();
		}

		private string GetNewTableAlias()
		{
			string tableAlias = "table" + this.tableAliasNumber;

			this.tableAliasNumber++;

			return tableAlias;
		}

		private int tableAliasNumber;

		private Stack<string> rootTypeTableAlias;

		private Stack<string> subTypeTableAlias;

		private static string relationSourceColumn = "[" + Tags.ColumnRefSourceId + "]";

		private static string relationTargetColumn = "[" + Tags.ColumnRefTargetId + "]";

		private static string idColumn = "[" + Tags.ColumnId + "]";

		private static string typeColumn = "[" + Tags.ColumnInstanceType + "]";

	}

}
