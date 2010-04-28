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
			Druid entityId = this.DataContext.EntityContext.CreateEmptyEntity<EntityType> ().GetEntityStructuredTypeId ();
			
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

			this.AddSubtypingJoins (reader, entityId);
			this.AddQueryFields (reader, entityId);
			this.AddCondition (reader, entityId, example);

			return reader;
		}


		private void AddSubtypingJoins(DbReader reader, Druid entityId)
		{
			Druid subEntityId = entityId;
			Druid superEntityId = (this.DataContext.EntityContext.GetStructuredType (entityId) as StructuredType).BaseTypeId;

			while (superEntityId.IsValid)
			{
				DbTableColumn subEntityColumn = this.GetTableColumn (subEntityId, DataBrowser.idFieldPath);
				DbTableColumn superEntityColumn = this.GetTableColumn (superEntityId, DataBrowser.idFieldPath);
				SqlJoinCode type = SqlJoinCode.Inner;

				reader.AddJoin (subEntityColumn, superEntityColumn, type);

				subEntityId = superEntityId;
				superEntityId = (this.DataContext.EntityContext.GetStructuredType (superEntityId) as StructuredType).BaseTypeId;
			}
		}


		private void AddQueryFields(DbReader reader, Druid entityId)
		{
			Druid currentId = entityId;

			while (currentId.IsValid)
			{
				foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityFieldDefinitions (currentId))
				{
					if (field.Relation == FieldRelation.None && field.Membership == FieldMembership.Local && field.Expression == null)
					{
						DbTableColumn tableColumn = this.GetTableColumn (currentId, EntityFieldPath.CreateRelativePath (field.Id));
						reader.AddQueryField (tableColumn);

						// TODO Find a way to give sorting informations. Below is the way it was done
						// before.

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
				}

				Druid nextId = (this.DataContext.EntityContext.GetStructuredType (currentId) as StructuredType).BaseTypeId;

				if (!nextId.IsValid)
				{
					DbTableColumn tableColumn = this.GetTableColumn (currentId, DataBrowser.typeFieldPath);
					reader.AddQueryField (tableColumn);			
				}

				currentId = nextId;
			}
		}


		private void AddCondition(DbReader reader, Druid entityId, AbstractEntity example)
		{
			// TODO Refractor this method to add the condition on the relations, recursively.
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
					EntityFieldPath   fieldPath   = EntityFieldPath.CreateRelativePath (id);
					DbTableColumn     tableColumn = this.GetTableColumn (entityId, fieldPath);

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


		private DbTableColumn GetTableColumn(Druid entityId, EntityFieldPath relativeFieldPath)
		{
			DbTable dbTable = this.SchemaEngine.FindTableDefinition (entityId);
			DbColumn dbColumn = dbTable.Columns[this.SchemaEngine.GetDataColumnName (relativeFieldPath.ToString ())];

			return new DbTableColumn (dbColumn)
			{
				TableAlias = entityId.ToString (),
				ColumnAlias = relativeFieldPath.ToString (),
			};
		}

		private static EntityFieldPath idFieldPath = EntityFieldPath.CreateRelativePath ("[" + Tags.ColumnId + "]");

		private static EntityFieldPath typeFieldPath = EntityFieldPath.CreateRelativePath ("[" + Tags.ColumnInstanceType + "]");

	}

}
