//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{
	
	
	/// <summary>
	/// The <c>SchemaBuilder</c> class is used internally to build <see cref="DbTable"/> and register
	/// them to the DbInfrastructure.
	/// </summary>
	internal sealed class SchemaBuilder
	{
		
		
		/// <summary>
		/// Builds a new <c>SchemaBuilder.</c>
		/// </summary>
		public SchemaBuilder(DbInfrastructure dbInfrastructure)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			
			this.DbInfrastructure = dbInfrastructure;
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"/> associated with this instance.
		/// </summary>
		private DbInfrastructure DbInfrastructure
		{
			get;
			set;
		}


		public void RegisterSchema(Druid entityId)
		{
			entityId.ThrowIf (id => !id.IsValid, "entityId must be valid");
			
			var schema = this.BuildSchema (entityId, true);

			IList<DbTable> schemaDbTables = schema.Item1;
			IList<DbTypeDef> schemaDbTypesDefs = schema.Item2;

			this.RegisterDbTypeDefs (schemaDbTypesDefs);
			this.RegisterDbTables (schemaDbTables);
		}


		public bool CheckSchema(Druid entityId)
		{
			entityId.ThrowIf (id => !id.IsValid, "entityId must be valid");

			IList<DbTable> schemaDbTables = this.BuildSchema (entityId, false).Item1;

			return this.CheckSchema (schemaDbTables);
		}


		private StructuredType GetStructuredType(Druid typeId)
		{
			ResourceManager manager = this.DbInfrastructure.DefaultContext.ResourceManager;

			StructuredType structuredType = TypeRosetta.CreateTypeObject (manager, typeId) as StructuredType;

			if (structuredType == null)
			{
				throw new System.ArgumentException ("typeId does not defined a structured type.");
			}

			return structuredType;
		}


		private DbTable BuildBasicTable(Druid entityId, bool isRootTable)
		{
			DbTable table = this.DbInfrastructure.CreateDbTable (entityId, DbElementCat.ManagedUserData, DbRevisionMode.TrackChanges, isRootTable);

			table.Comment = table.DisplayName;

			if (isRootTable)
			{
				//	If this entity has no parent in the class hierarchy, then we
				//	need to add a special identification column, which can be used
				//	to map a row to its proper derived entity class.

				DbTypeDef typeDef = this.DbInfrastructure.ResolveDbType (Tags.TypeKeyId);
				DbColumn column = new DbColumn (Tags.ColumnInstanceType, typeDef, DbColumnClass.Data, DbElementCat.Internal, DbRevisionMode.Immutable);

				table.Columns.Add (column);
			}

			return table;
		}


		private System.Tuple<IList<DbTable>, IList<DbTypeDef>> BuildSchema(Druid entityId, bool useDatabase)
		{
			var structuredTypes = this.GetStructuredTypesUsedInSchema (entityId);

			var splitedTables = this.SplitTables (structuredTypes, useDatabase);

			var unregisteredTables = splitedTables.Item1;
			var registeredDbTables = splitedTables.Item2;

			var namedTypes = this.GetNamedTypesUsedInStructuredTypes (unregisteredTables);

			var splitedDbTypeDefs = this.SplitAndBuildDbTypeDefs (namedTypes, useDatabase);

			var unregisteredDbTypeDefs = splitedDbTypeDefs.Item1;
			var registeredDbTypeDefs = splitedDbTypeDefs.Item2;

			var dbTypeDefs = unregisteredDbTypeDefs.Concat (registeredDbTypeDefs).ToList ();
			
			var unexistingDbTables = this.BuildDbTables (unregisteredTables, registeredDbTables, dbTypeDefs);

			return System.Tuple.Create (unexistingDbTables, unregisteredDbTypeDefs);
		}


		private IList<StructuredType> GetStructuredTypesUsedInSchema(Druid typeId)
		{
			IDictionary<Druid, StructuredType> types = new Dictionary<Druid, StructuredType> ();

			this.GetStructuredTypesUsedInSchema (types, typeId);

			return types.Values.ToList ();
		}


		private void GetStructuredTypesUsedInSchema(IDictionary<Druid, StructuredType> types, Druid typeId)
		{
			if (!types.ContainsKey (typeId))
			{
				StructuredType type = this.GetStructuredType (typeId);

				types[type.CaptionId] = type;

				StructuredType baseType = type.BaseType;

				if (baseType != null)
				{
					this.GetStructuredTypesUsedInSchema (types, type.BaseType.CaptionId);
				}

				foreach (StructuredTypeField field in this.GetRelationFields (type))
				{
					this.GetStructuredTypesUsedInSchema (types, field.Type.CaptionId);
				}
			}
		}


		private IList<INamedType> GetNamedTypesUsedInStructuredTypes(IList<StructuredType> structuredTypes)
		{
			IDictionary<Druid, INamedType> namedTypes = new Dictionary<Druid, INamedType> ();

			foreach (StructuredType structuredType in structuredTypes)
			{
				foreach (StructuredTypeField field in this.GetValueFields (structuredType))
				{
					INamedType namedType = field.Type;
					Druid namedTypeId = namedType.CaptionId;

					namedTypes[namedTypeId] = namedType;
				}
			}

			return namedTypes.Values.ToList ();
		}


		private IEnumerable<StructuredTypeField> GetValueFields(StructuredType type)
		{
			return from field in this.GetLocalFields (type)
				   where field.Relation == FieldRelation.None
				   select field;
		}


		private IEnumerable<StructuredTypeField> GetRelationFields(StructuredType type)
		{
			return from field in this.GetLocalFields (type)
				   where field.Relation == FieldRelation.Reference || field.Relation == FieldRelation.Collection
				   select field;
		}


		private IEnumerable<StructuredTypeField> GetLocalFields(StructuredType type)
		{
			return from field in type.Fields.Values
				   where field.Membership == FieldMembership.Local || field.Membership == FieldMembership.LocalOverride
				   where field.Source == FieldSource.Value
				   select field;
		}


		private System.Tuple<IList<StructuredType>, IList<DbTable>> SplitTables(IEnumerable<StructuredType> types, bool useDatabase)
		{
			List<StructuredType> unregisteredTables = new List<StructuredType> ();
			List<DbTable> registeredTables = new List<DbTable> ();

			foreach (StructuredType type in types)
			{
				Druid typeId = type.CaptionId;

				DbTable registeredDbTable = null;

				if (useDatabase)
				{
					registeredDbTable = this.DbInfrastructure.ResolveDbTable (typeId);
				}

				if (registeredDbTable != null)
				{
					registeredTables.Add (registeredDbTable);
				}
				else
				{
					unregisteredTables.Add (type);
				}
			}

			return System.Tuple.Create ((IList<StructuredType>) unregisteredTables, (IList<DbTable>) registeredTables);
		}


		private System.Tuple<IList<DbTypeDef>, IList<DbTypeDef>> SplitAndBuildDbTypeDefs(IEnumerable<INamedType> namedTypes, bool useDatabase)
		{
			List<DbTypeDef> unregisteredDbTypeDefs = new List<DbTypeDef> ();
			List<DbTypeDef> registeredDbTypeDefs = new List<DbTypeDef> ();

			foreach (INamedType namedType in namedTypes)
			{
				DbTypeDef registeredDbTypeDef = null;

				if (useDatabase)
				{
					registeredDbTypeDef = this.DbInfrastructure.ResolveDbType (namedType);
				}

				if (registeredDbTypeDef != null)
				{
					registeredDbTypeDefs.Add (registeredDbTypeDef);
				}
				else
				{
					DbTypeDef unregisteredDbTypeDef = new DbTypeDef (namedType);

					unregisteredDbTypeDefs.Add (unregisteredDbTypeDef);
				}
			}

			return System.Tuple.Create ((IList<DbTypeDef>) unregisteredDbTypeDefs, (IList<DbTypeDef>) registeredDbTypeDefs);
		}


		private IList<DbTable> BuildDbTables(IEnumerable<StructuredType> unregisteredTables, IEnumerable<DbTable> registeredDbTables, IEnumerable<DbTypeDef> dbTypeDefs)
		{
			Dictionary<Druid, DbTable> dbTables = registeredDbTables.ToDictionary (t => t.CaptionId, t => t);
			Dictionary<Druid, DbTypeDef> dbTypeDefsDict = dbTypeDefs.ToDictionary (t => t.TypeId, t => t);
			Dictionary<Druid, StructuredType> unregisteredTablesDict = unregisteredTables.ToDictionary (t => t.CaptionId, t => t);

			List<DbTable> unregisteredDbTables = new List<DbTable> ();

			foreach (StructuredType unregisteredTable in unregisteredTablesDict.Values)
			{
				Druid unregisteredTableId = unregisteredTable.CaptionId;
				
				bool isRootTable = (unregisteredTable.BaseType == null);

				DbTable unregisteredDbTable = this.BuildBasicTable (unregisteredTableId, isRootTable);

				dbTables[unregisteredTableId] = unregisteredDbTable;
				unregisteredDbTables.Add (unregisteredDbTable);
			}

			foreach (DbTable unregisteredDbTable in unregisteredDbTables)
			{
				StructuredType type = unregisteredTablesDict[unregisteredDbTable.CaptionId];

				foreach (var field in this.GetRelationFields (type))
				{
					DbColumn newColumn = this.BuildRelationColumn (dbTables, field);

					unregisteredDbTable.Columns.Add (newColumn);
				}

				foreach (var field in this.GetValueFields (type))
				{
					DbColumn newColumn = this.BuildValueColumn (dbTypeDefsDict, field);

					unregisteredDbTable.Columns.Add (newColumn);
				}
			}

			return unregisteredDbTables;
		}


		private DbColumn BuildValueColumn(IDictionary<Druid, DbTypeDef> dbTypeDefs, StructuredTypeField field)
		{
			DbTypeDef columnType = dbTypeDefs[field.Type.CaptionId];
			DbColumn column = new DbColumn (field.CaptionId, columnType, DbColumnClass.Data, DbElementCat.ManagedUserData, DbRevisionMode.TrackChanges);

			column.Comment = column.DisplayName;
			column.IsNullable = field.IsNullable;

			return column;
		}


		private DbColumn BuildRelationColumn(Dictionary<Druid, DbTable> newTables, StructuredTypeField field)
		{
			Druid targetEntityId = field.TypeId;

			DbTable targetTabe = newTables[targetEntityId];

			DbCardinality cardinality = this.FieldRelationToDbCardinality (field.Relation);

			DbColumn column = DbTable.CreateRelationColumn (field.CaptionId, targetTabe, DbRevisionMode.TrackChanges, cardinality);

			return column;
		}


		private void RegisterDbTypeDefs(IList<DbTypeDef> dbTypeDefs)
		{
			foreach (DbTypeDef dbTypeDef in dbTypeDefs)
			{
				this.DbInfrastructure.RegisterNewDbType (dbTypeDef);
			}
		}


		private void RegisterDbTables(IList<DbTable> dbTables)
		{
			foreach (DbTable dbTable in dbTables)
			{
				this.DbInfrastructure.RegisterNewDbTable (dbTable);
			}

			foreach (DbTable dbTable in dbTables)
			{
				this.DbInfrastructure.RegisterColumnRelations (dbTable);
			}
		}


		private bool CheckSchema(IList<DbTable> schema)
		{
			return DbSchemaChecker.CheckSchema (this.DbInfrastructure, schema);
		}


		private DbCardinality FieldRelationToDbCardinality(FieldRelation fieldRelation)
		{
			switch (fieldRelation)
			{
				case FieldRelation.None:
					return DbCardinality.None;

				case FieldRelation.Reference:
					return DbCardinality.Reference;

				case FieldRelation.Collection:
					return DbCardinality.Collection;

				default:
					throw new System.NotImplementedException ();
			}
		}


	}


}
