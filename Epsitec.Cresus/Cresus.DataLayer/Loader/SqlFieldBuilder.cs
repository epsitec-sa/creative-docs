using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Schema;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class SqlFieldBuilder
	{


		public SqlFieldBuilder(DataContext dataContext)
		{
			this.dataContext = dataContext;

			this.aliasManager = new AliasManager ();
		}


		public AliasManager AliasManager
		{
			get
			{
				return this.aliasManager;
			}
		}


		private DataConverter DataConverter
		{
			get
			{
				return this.dataContext.DataConverter;
			}
		}


		private EntityTypeEngine TypeEngine
		{
			get
			{
				return this.dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine;
			}
		}


		private EntitySchemaEngine SchemaEngine
		{
			get
			{
				return this.dataContext.DataInfrastructure.EntityEngine.EntitySchemaEngine;
			}
		}


		public SqlField BuildConstantForField(AbstractEntity entity, Druid fieldId)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var localEntityTypeId = this.TypeEngine.GetLocalType (leafEntityTypeId, fieldId).CaptionId;

			var dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityTypeId, fieldId);
			var dbTypeDef = dbColumn.Type;
			var dbRawType = dbTypeDef.RawType;
			var dbSimpleType = dbTypeDef.SimpleType;
			var dbNumDef = dbTypeDef.NumDef;

			var fieldName = fieldId.ToResourceId ();
			var value = entity.InternalGetValue (fieldName);

			return this.BuildConstantForField (dbRawType, dbSimpleType, dbNumDef, value);
		}


		public SqlField BuildConstantForField(DbRawType dbRawType, DbSimpleType dbSimpleType, DbNumDef dbNumDef, object value)
		{
			var convertedValue = this.DataConverter.FromCresusToDatabaseValue (dbRawType, dbSimpleType, dbNumDef, value);
			var convertedType = this.DataConverter.FromDotNetToDatabaseType (dbRawType);

			return this.BuildConstant (convertedValue, convertedType);
		}


		public SqlField BuildConstantForKey(AbstractEntity entity)
		{
			var entityKey = this.dataContext.GetNormalizedEntityKey (entity);
			
			var value = entityKey.Value.RowKey.Id.Value;
			var dbRawType = DbRawType.Int64;

			return this.BuildConstant (value, dbRawType);
		}


		public SqlField BuildEntityTable(AbstractEntity entity, Druid localEntityTypeId)
		{
			var tableAlias = this.aliasManager.GetAlias (entity, localEntityTypeId);
			var dbTable = this.SchemaEngine.GetEntityTable (localEntityTypeId);

			return this.BuildTable (tableAlias, dbTable);
		}


		public SqlField BuildRelationTable(AbstractEntity source, Druid fieldId, AbstractEntity target)
		{
			var leafSourceTypeId = source.GetEntityStructuredTypeId ();
			var localSourceTypeId = this.TypeEngine.GetLocalType (leafSourceTypeId, fieldId).CaptionId;

			var tableAlias = this.aliasManager.GetAlias (source, fieldId, target);
			var dbTable = this.SchemaEngine.GetEntityFieldTable (localSourceTypeId, fieldId);

			return this.BuildTable (tableAlias, dbTable);
		}


		public SqlField BuildEntityField(AbstractEntity entity, Druid fieldId)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var localEntityTypeId = this.TypeEngine.GetLocalType (leafEntityTypeId, fieldId).CaptionId;

			var columnName = EntitySchemaBuilder.GetEntityFieldColumnName (fieldId);

			return this.BuildEntityColumn (entity, localEntityTypeId, columnName);
		}


		public SqlField BuildRootId(AbstractEntity entity)
		{
			var name = EntitySchemaBuilder.EntityTableColumnIdName;

			return this.BuildRootColumn (entity, name);
		}


		public SqlField BuildRootLogId(AbstractEntity entity)
		{
			var name = EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName;

			return this.BuildRootColumn (entity, name);
		}


		public SqlField BuildRootTypeId(AbstractEntity entity)
		{
			var name = EntitySchemaBuilder.EntityTableColumnEntityTypeIdName;

			return this.BuildRootColumn (entity, name);
		}


		public SqlField BuildRootColumn(AbstractEntity entity, string name)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var rootEntityTypeId = this.TypeEngine.GetRootType (leafEntityTypeId).CaptionId;

			return this.BuildEntityColumn (entity, rootEntityTypeId, name);
		}


		public SqlField BuildEntityId(AbstractEntity entity, Druid localEntityTypeId)
		{
			var name = EntitySchemaBuilder.EntityTableColumnIdName;

			return this.BuildEntityColumn (entity, localEntityTypeId, name);
		}


		public SqlField BuildEntityColumn(AbstractEntity entity, Druid localEntityTypeId, string name)
		{
			var tableAlias = this.aliasManager.GetAlias (entity, localEntityTypeId);

			var dbTable = this.SchemaEngine.GetEntityTable (localEntityTypeId);
			var dbColumn = dbTable.Columns[name];

			return this.BuildColumn (tableAlias, dbColumn);
		}


		public SqlField BuildRelationSourceId(string tableAlias, Druid localEntityTypeId, Druid fieldId)
		{
			var name = EntitySchemaBuilder.EntityFieldTableColumnSourceIdName;

			return this.BuildRelationColumn (tableAlias, localEntityTypeId, fieldId, name);
		}


		public SqlField BuildRelationTargetId(string tableAlias, Druid localEntityTypeId, Druid fieldId)
		{
			var name = EntitySchemaBuilder.EntityFieldTableColumnTargetIdName;

			return this.BuildRelationColumn (tableAlias, localEntityTypeId, fieldId, name);
		}


		public SqlField BuildRelationRank(string tableAlias, Druid localEntityTypeId, Druid fieldId)
		{
			var name = EntitySchemaBuilder.EntityFieldTableColumnRankName;

			return this.BuildRelationColumn (tableAlias, localEntityTypeId, fieldId, name);
		}


		public SqlField BuildRelationColumn(string tableAlias, Druid localEntityTypeId, Druid fieldId, string columnName)
		{
			var dbTable = this.SchemaEngine.GetEntityFieldTable (localEntityTypeId, fieldId);
			var dbColumn = dbTable.Columns[columnName];

			return this.BuildColumn (tableAlias, dbColumn);
		}


		public SqlField BuildConstant(object value, DbRawType dbRawType)
		{
			return SqlField.CreateConstant (value, dbRawType);
		}


		public SqlField BuildTable(string tableAlias, DbTable dbTable)
		{
			var tableName = dbTable.GetSqlName ();

			return SqlField.CreateAliasedName (tableName, tableAlias);
		}


		public SqlField BuildColumn(string tableAlias, DbColumn dbColumn)
		{
			var columnName = dbColumn.GetSqlName ();
			var columnAlias = dbColumn.Name;

			return SqlField.CreateAliasedName (tableAlias, columnName, columnAlias);
		}


		private readonly AliasManager aliasManager;


		private readonly DataContext dataContext;


	}


}
