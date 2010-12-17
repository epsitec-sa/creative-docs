//	Copyright � 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{
	
	
	/// <summary>
	/// The <c>SchemaBuilder</c> class is used internally to build <see cref="DbTable"/> and 
	/// <see cref="DbTypeDef"/> and then register them to the database.
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


		/// <summary>
		/// Builds and registers the schema of the <see cref="AbstractEntity"/> defined by the given
		/// <see cref="Druid"/> in the database. This method will also build and register all the
		/// required <see cref="AbstractEntity"/> that are not yet defined in the database.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> defining the <see cref="AbstractEntity"/> whose schema to register.</param>
		public void RegisterSchema(Druid entityId)
		{
			entityId.ThrowIf (id => !id.IsValid, "entityId must be valid");
			
			var schema = this.BuildSchema (entityId, true);

			IList<DbTable> schemaDbTables = schema.Item1;
			IList<DbTypeDef> schemaDbTypesDefs = schema.Item2;

			this.RegisterDbTypeDefs (schemaDbTypesDefs);
			this.RegisterDbTables (schemaDbTables);
		}


		/// <summary>
		/// Checks that the schema of the <see cref="AbstractEntity"/> defined by the given
		/// <see cref="Druid"/> is correctly defined in the database. All the referenced
		/// <see cref="AbstractEntity"/> are also checked.
		/// </summary>
		/// <param name="entityId"></param>
		/// <returns></returns>
		public bool CheckSchema(Druid entityId)
		{
			entityId.ThrowIf (id => !id.IsValid, "entityId must be valid");

			IList<DbTable> schemaDbTables = this.BuildSchema (entityId, false).Item1;

			return this.CheckSchema (schemaDbTables);
		}


		/// <summary>
		/// Builds the schema of the <see cref="AbstractEntity"/> defined by the given
		/// <see cref="Druid"/>. The new schema may or may not reference existing stuff in the
		/// database, depending on the value of <paramref name="useDatabase"/>.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> defining the schema to build.</param>
		/// <param name="useDatabase">If set to <c>true</c>, the created schema will reference existing stuff in the database, if set to <c>false</c>, the created schema will create everything, event if it already exists in the database.</param>
		/// <returns>A tuple containing the sequence of created <see cref="DbTable"/> and the sequence of created <see cref="DbTypeDef"/>.</returns>
		private System.Tuple<IList<DbTable>, IList<DbTypeDef>> BuildSchema(Druid entityId, bool useDatabase)
		{
			// We follow the graph of AbstractEntity starting by the one defined by entityId in order
			// to get all the StructuredTypes that are connected directly or indirectly to the one
			// defined by entityId, including the one defined by entityId.
			var structuredTypes = this.GetStructuredTypesUsedInSchema (entityId);

			// We process these StructuredTypes to get on one side the list of StructuredTypes that
			// are not yet defined in the database, and on the other side the list of DbTables that
			// correspond to a StructuredType that is defined in the database.
			var splitedTables = this.SplitTables (structuredTypes, useDatabase);

			var unregisteredTables = splitedTables.Item1;
			var registeredDbTables = splitedTables.Item2;

			// We get all the INamedTypes that are used in the StructuredTypes that are not yet
			// defined in the database.
			var namedTypes = this.GetNamedTypesUsedInStructuredTypes (unregisteredTables);

			// We process them to get on one side the list of DbTypeDefs that correspond to the
			// INamedType that are not yet defined in the database, and on the other side, the list
			// of DbTypeDefs that correspond to an sINamedType that is defined in the database.
			var splitedDbTypeDefs = this.SplitAndBuildDbTypeDefs (namedTypes, useDatabase);

			var unregisteredDbTypeDefs = splitedDbTypeDefs.Item1;
			var registeredDbTypeDefs = splitedDbTypeDefs.Item2;

			var dbTypeDefs = unregisteredDbTypeDefs.Concat (registeredDbTypeDefs).ToList ();
			
			// We build the DbTable objects that correspond to the StructuredTypes that must be
			// created.
			var unexistingDbTables = this.BuildDbTables (unregisteredTables, registeredDbTables, dbTypeDefs);

			// We return the created schema, that is the newly created DbTables and the newly created
			// DbTypeDefs.
			return System.Tuple.Create (unexistingDbTables, unregisteredDbTypeDefs);
		}


		/// <summary>
		/// Gets the sequence of <see cref="StructuredType"/> that are referenced somewhere in the
		/// graph of <see cref="AbstractEntity"/> containing the <see cref="AbstractEntity"/> defined
		/// by the given <see cref="Druid"/>.
		/// </summary>
		/// <param name="typeId">The <see cref="Druid"/> defined the type of the <see cref="AbstractEntity"/>.</param>
		/// <returns>The sequence of <see cref="StructuredType"/>.</returns>
		private IList<StructuredType> GetStructuredTypesUsedInSchema(Druid typeId)
		{
			IDictionary<Druid, StructuredType> types = new Dictionary<Druid, StructuredType> ();

			this.GetStructuredTypesUsedInSchema (types, typeId);

			return types.Values.ToList ();
		}


		/// <summary>
		/// Adds the given <see cref="Druid"/> and the corresponding <see cref="StructuredType"/> if
		/// it is not already in the <see cref="IDictionary"/> and recursively adds the same data
		/// for its base type and its fields.
		/// </summary>
		/// <param name="types">The <see cref="IDictionary"/> that contains the <see cref="Druid"/> and the <see cref="StructuredType"/>.</param>
		/// <param name="typeId">The <see cref="Druid"/> of the <see cref="StructuredType"/> to add.</param>
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


		/// <summary>
		/// Gets the sequence of <see cref="INamedType"/> used in the given sequence of
		/// <see cref="StructuredType"/>.
		/// </summary>
		/// <param name="structuredTypes">The sequence of <see cref="StructuredType"/> whose <see cref="INamedType"/> to retrieve.</param>
		/// <returns>The sequence of <see cref="INamedType"/> referenced in the sequence of <see cref="StructuredType"/>.</returns>
		private IList<INamedType> GetNamedTypesUsedInStructuredTypes(IList<StructuredType> structuredTypes)
		{
			var namedTypes = new Dictionary<Druid, INamedType> ();

			foreach (StructuredType structuredType in structuredTypes)
			{
				foreach (StructuredTypeField field in this.GetValueFields (structuredType))
				{
					INamedType namedType = field.Type;
					Druid namedTypeId = namedType.CaptionId;

					if (namedTypes.ContainsKey (namedTypeId) == false)
					{
						namedTypes[namedTypeId] = namedType;
					}
				}
			}

			return namedTypes.Values.ToList ();
		}


		/// <summary>
		/// Gets the <see cref="StructuredType"/> that corresponds to the given <see cref="Druid"/>.
		/// </summary>
		/// <param name="typeId">The <see cref="Druid"/> defining the <see cref="StructuredType"/> to get.</param>
		/// <returns>The <see cref="StructuredType"/>.</returns>
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


		/// <summary>
		/// Gets the sequence of <see cref="StructuredTypeField"/> of the given <see cref="StructuredType"/>
		/// that are value fields.
		/// </summary>
		/// <param name="type">The <see cref="StructuredType"/> whose value fields to get.</param>
		/// <returns>The value fields of the given <see cref="StructuredType"/>.</returns>
		private IEnumerable<StructuredTypeField> GetValueFields(StructuredType type)
		{
			return from field in this.GetLocalFields (type)
				   where field.Relation == FieldRelation.None
				   select field;
		}


		/// <summary>
		/// Gets the sequence of <see cref="StructuredTypeField"/> of the given <see cref="StructuredType"/>
		/// that are relation fields.
		/// </summary>
		/// <param name="type">The <see cref="StructuredType"/> whose relation fields to get.</param>
		/// <returns>The relation fields of the given <see cref="StructuredType"/>.</returns>
		private IEnumerable<StructuredTypeField> GetRelationFields(StructuredType type)
		{
			return from field in this.GetLocalFields (type)
				   where field.Relation == FieldRelation.Reference || field.Relation == FieldRelation.Collection
				   select field;
		}


		/// <summary>
		/// Gets the sequence of <see cref="StructuredTypeField"/> of the given <see cref="StructuredType"/>
		/// that are local fields.
		/// </summary>
		/// <param name="type">The <see cref="StructuredType"/> whose local fields to get.</param>
		/// <returns>The local fields of the given <see cref="StructuredType"/>.</returns>
		private IEnumerable<StructuredTypeField> GetLocalFields(StructuredType type)
		{
			return from field in type.Fields.Values
				   where field.Membership == FieldMembership.Local || field.Membership == FieldMembership.LocalOverride
				   where field.Source == FieldSource.Value
				   select field;
		}


		/// <summary>
		/// Splits the given sequence of <see cref="StructuredType"/> in a sequence which contains
		/// the ones that are not defined in the database, and in a sequence which contains the
		/// <see cref="DbTable"/> corresponding to the ones that are defined in the database.
		/// </summary>
		/// <param name="types">The sequence of <see cref="StructuredType"/> to split.</param>
		/// <param name="useDatabase">If set to <c>true</c>, this method will really look in the database, if set to <c>false</c>, every <see cref="StructuredType"/> will be considered as not defined in the database.</param>
		/// <returns>The pair of resulting sequences.</returns>
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


		/// <summary>
		/// Splits the given sequence of <see cref="INamedType"/> in a sequence which contains the
		/// corresponding <see cref="DbTypeDef"/> of the ones that are not defined in the database
		/// and in a sequence which contains the corresponding <see cref="DbTypeDef"/> of the ones
		/// that are defined in the database.
		/// </summary>
		/// <param name="namedTypes">The sequence of <see cref="INamedType"/>.</param>
		/// <param name="useDatabase">If set to <c>true</c>, this method will really look in the database, if set to <c>false</c>, every <see cref="INamedType"/> will be considered as not defined in the database.</param>
		/// <returns>The pair of resulting sequences.</returns>
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


		/// <summary>
		/// Builds the sequence of <see cref="DbTable"/> that corresponds to the given sequence of
		/// <see cref="StructuredType"/>. In order to build them, this method takes a sequence of
		/// the <see cref="DbTable"/> that already exists and the sequence of <see cref="DbTypeDef"/>
		/// that will be used to build the <see cref="DbTable"/>.
		/// </summary>
		/// <param name="unregisteredTables">The sequence of <see cref="StructuredType"/> whose corresponding <see cref="DbTable"/> to build.</param>
		/// <param name="registeredDbTables">The sequence of <see cref="DbTable"/> that already exist.</param>
		/// <param name="dbTypeDefs">The sequence of <see cref="DbTypeDef"/> that will be used to build the sequence of <see cref="DbTable"/>.</param>
		/// <returns>The sequence of newly created <see cref="DbTable"/>.</returns>
		private IList<DbTable> BuildDbTables(IEnumerable<StructuredType> unregisteredTables, IEnumerable<DbTable> registeredDbTables, IEnumerable<DbTypeDef> dbTypeDefs)
		{
			Dictionary<Druid, DbTable> dbTables = registeredDbTables.ToDictionary (t => t.CaptionId, t => t);
			Dictionary<Druid, DbTypeDef> dbTypeDefsDict = dbTypeDefs.ToDictionary (t => t.TypeId, t => t);
			Dictionary<Druid, StructuredType> unregisteredTablesDict = unregisteredTables.ToDictionary (t => t.CaptionId, t => t);

			List<DbTable> unregisteredDbTables = new List<DbTable> ();

			// We build the sequence of DbTables in two passes. In the first one, we create a basic
			// DbTable that contains only the metadata columns. Then we create the data columns in
			// the second pass. The reason for these two passes is that we must build all DbTables
			// before we can reference them, and there might be cycles in their graph.

			foreach (StructuredType unregisteredTable in unregisteredTablesDict.Values)
			{
				DbTable unregisteredDbTable = this.BuildBasicTable (unregisteredTable);

				dbTables[unregisteredTable.CaptionId] = unregisteredDbTable;
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


		/// <summary>
		/// Builds a basic <see cref="DbTable"/> for the given <see cref="StructuredType"/>. The
		/// created <see cref="DbTable"/> will only contains a name, comment and the metadata columns.
		/// </summary>
		/// <param name="tableType">The <see cref="StructuredType"/> corresponding to the <see cref="DbTable"/> to create.</param>
		/// <returns>The newly created <see cref="DbTable"/>.</returns>
		private DbTable BuildBasicTable(StructuredType tableType)
		{
			DbTable table = this.DbInfrastructure.CreateDbTable (tableType.CaptionId, DbElementCat.ManagedUserData, DbRevisionMode.TrackChanges, (tableType.BaseType == null));

			table.Comment = table.DisplayName;

			if (tableType.BaseType == null)
			{
				// If this entity has no parent in the class hierarchy, then we need to add a
				// special identification column, which can be used to map a row to its proper
				// derived entity class. We also add a column which in order to log info about
				// who made the last change to the entity.
	
				DbTypeDef keyTypeDef = this.DbInfrastructure.ResolveDbType (Tags.TypeKeyId);
				
				DbColumn typeColumn = new DbColumn (Tags.ColumnInstanceType, keyTypeDef, DbColumnClass.Data, DbElementCat.Internal, DbRevisionMode.Immutable);
				DbColumn logColumn = new DbColumn (Tags.ColumnRefLog, keyTypeDef, DbColumnClass.RefInternal, DbElementCat.Internal, DbRevisionMode.IgnoreChanges);

				table.Columns.Add (typeColumn);
				table.Columns.Add (logColumn);
			}

			return table;
		}


		/// <summary>
		/// Builds the <see cref="DbColumn"/> that corresponds to the given value
		/// <see cref="StructuredTypeField"/>.
		/// </summary>
		/// <param name="dbTypeDefs">The mapping of <see cref="Druid"/>  to<see cref="DbTypeDef"/> that can be used for the types.</param>
		/// <param name="field">The <see cref="StructuredTypeField"/> whose corresponding <see cref="DbColumn"/> to create.</param>
		/// <returns>The newly created <see cref="DbColumn"/>.</returns>
		private DbColumn BuildValueColumn(IDictionary<Druid, DbTypeDef> dbTypeDefs, StructuredTypeField field)
		{
			DbTypeDef columnType = dbTypeDefs[field.Type.CaptionId];
			DbColumn column = new DbColumn (field.CaptionId, columnType, DbColumnClass.Data, DbElementCat.ManagedUserData, DbRevisionMode.TrackChanges);

			column.Comment = column.DisplayName;
			column.IsNullable = field.IsNullable;

			return column;
		}


		/// <summary>
		/// Builds the <see cref="DbColumn"/> that corresponds to the given relation
		/// <see cref="StructuredTypeField"/>.
		/// </summary>
		/// <param name="newTables">The mapping of <see cref="Druid"/>  to<see cref="DbTable"/> that can be referenced by the <see cref="DbColumn"/>.</param>
		/// <param name="field">The <see cref="StructuredTypeField"/> whose corresponding <see cref="DbColumn"/> to create.</param>
		/// <returns>The newly created <see cref="DbColumn"/>.</returns>
		private DbColumn BuildRelationColumn(Dictionary<Druid, DbTable> newTables, StructuredTypeField field)
		{
			Druid targetEntityId = field.TypeId;

			DbTable targetTable = newTables[targetEntityId];

			DbCardinality cardinality = this.FieldRelationToDbCardinality (field.Relation);

			DbColumn column = DbTable.CreateRelationColumn (field.CaptionId, targetTable, DbRevisionMode.TrackChanges, cardinality);

			return column;
		}


		/// <summary>
		/// Registers all the given <see cref="DbTypeDef"/> to the database.
		/// </summary>
		/// <param name="dbTypeDefs">The sequence of <see cref="DbTypeDef"/> to register.</param>
		private void RegisterDbTypeDefs(IList<DbTypeDef> dbTypeDefs)
		{
			foreach (DbTypeDef dbTypeDef in dbTypeDefs)
			{
				this.DbInfrastructure.RegisterNewDbType (dbTypeDef);
			}
		}


		/// <summary>
		/// Registers all the given <see cref="DbTable"/> to the database.
		/// </summary>
		/// <param name="dbTables">The sequence of <see cref="DbTable"/> to register in the database.</param>
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


		/// <summary>
		/// Checks that all the given <see cref="DbTable"/> are correctly defined in the database.
		/// </summary>
		/// <param name="schema">The sequence of <see cref="DbTable"/> to check.</param>
		/// <returns><c>true</c> if all the <see cref="DbTable"/> ar </returns>
		private bool CheckSchema(IList<DbTable> schema)
		{
			return DbSchemaChecker.CheckSchema (this.DbInfrastructure, schema);
		}


		/// <summary>
		/// Converts a given <see cref="FieldRelation"/> to the equivalent <see cref="DbCardinality"/>.
		/// </summary>
		/// <param name="fieldRelation">The <see cref="FieldRelation"/> object to convert.</param>
		/// <returns>The result of the conversion.</returns>
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
