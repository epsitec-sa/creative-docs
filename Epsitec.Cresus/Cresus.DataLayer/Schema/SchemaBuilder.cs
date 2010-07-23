//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Schema
{
	
	
	/// <summary>
	/// The <c>SchemaBuilder</c> class is used internally to build <see cref="DbTable"/> and register
	/// them to the DbInfrastructure.
	/// </summary>
	internal class SchemaBuilder
	{
		
		
		/// <summary>
		/// Builds a new <c>SchemaBuilder.</c>
		/// </summary>
		/// <param name="schemaEngine">The <see cref="SchemaEngine"/> used as a cache for this instance.</param>
		public SchemaBuilder(SchemaEngine schemaEngine)
		{
			this.SchemaEngine = schemaEngine;
		}


		/// <summary>
		/// The <see cref="SchemaEngine"/> associated with this instance.
		/// </summary>
		private SchemaEngine SchemaEngine
		{
			get;
			set;
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"/> associated with this instance.
		/// </summary>
		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.SchemaEngine.DbInfrastructure;
			}
		}


		/// <summary>
		/// Creates the schema of an <see cref="AbstractEntity"/> and register it to the database. This
		/// method will recursively create and register everything that is required to build the schema.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="entityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> whose schema to build</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="transaction"/> is null.</exception>
		public void CreateSchema(DbTransaction transaction, Druid entityId)
		{
			transaction.ThrowIfNull ("transaction");

			this.newTablesDictionary = new Dictionary<Druid, DbTable> ();
			this.newTypesDictionary = new Dictionary<Druid, DbTypeDef> ();

			this.CreateTable (transaction, entityId);

			foreach (DbTable table in this.newTablesDictionary.Values)
			{
				this.DbInfrastructure.RegisterNewDbTable (transaction, table);
			}

			foreach (DbTable table in this.newTablesDictionary.Values)
			{
				this.DbInfrastructure.RegisterColumnRelations (transaction, table);
			}
		}


		/// <summary>
		/// Gets the mapping of the <see cref="Druid"/> and the <see cref="DbTable"/> that where created
		/// during the last call of the method <see cref="CreateSchema"/>.
		/// </summary>
		/// <returns>The mapping between the <see cref="Druid"/> and the <see cref="DbTable"/>.</returns>
		public Dictionary<Druid, DbTable> GetNewTableDefinitions()
		{
			return this.newTablesDictionary;
		}


		/// <summary>
		/// Gets the mapping of the <see cref="Druid"/> and the <see cref="DbTypeDef"/> that where created
		/// during the last call of the method <see cref="CreateSchema"/>.
		/// </summary>
		/// <returns>The mapping between the <see cref="Druid"/> and the <see cref="DbTypeDef"/>.</returns>
		public Dictionary<Druid, DbTypeDef> GetNewTypeDefinitions()
		{
			return this.newTypesDictionary;
		}


		/// <summary>
		/// Creates the <see cref="DbTable"/> of an <see cref="AbstractEntity"/> and everything which
		/// is required such as the parent and neighbor <see cref="DbTable"/> without registering
		/// them to the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="entityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> whose schema to create.</param>
		/// <returns>The root <see cref="DbTable"/> of the <see cref="AbstractEntity"/>.</returns>
		private DbTable CreateTable(DbTransaction transaction, Druid entityId)
		{
			DbTable table = null;

			//	If we have already generated a table definition for this entity,
			//	just re-use it. This check makes circular references possible.
			
			if (table == null)
			{
				this.newTablesDictionary.TryGetValue (entityId, out table);
			}

			if (table == null)
			{
				table = this.SchemaEngine.GetEntityTableDefinition (entityId);
			}

			if (table == null)
			{
				table = this.CreateTableHelper (transaction, entityId);
			}

			return table;
		}


		/// <summary>
		/// Helper method for <see cref="CreateTable"/>, which will do the real job of creating a
		/// <see cref="DbTable"/> that does not exists and all its dependencies.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="entityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> whose schema to create.</param>
		/// <returns>The root <see cref="DbTable"/> of the <see cref="AbstractEntity"/>.</returns>
		private DbTable CreateTableHelper(DbTransaction transaction, Druid entityId)
		{
			ResourceManager manager = this.DbInfrastructure.DefaultContext.ResourceManager;
			StructuredType entityType = TypeRosetta.CreateTypeObject (manager, entityId) as StructuredType;

			if (entityType == null)
			{
				throw new System.ArgumentException ("Invalid entity ID", "entityId");
			}

			DbTable table = this.DbInfrastructure.CreateDbTable (entityId, DbElementCat.ManagedUserData, DbRevisionMode.TrackChanges);
			table.Comment = table.DisplayName;

			this.newTablesDictionary[entityId] = table;

			if (entityType.BaseTypeId.IsEmpty)
			{
				//	If this entity has no parent in the class hierarchy, then we
				//	need to add a special identification column, which can be used
				//	to map a row to its proper derived entity class.

				DbTypeDef typeDef = this.DbInfrastructure.ResolveDbType (transaction, Tags.TypeKeyId);
				DbColumn column = new DbColumn (Tags.ColumnInstanceType, typeDef, DbColumnClass.Data, DbElementCat.Internal, DbRevisionMode.Immutable);

				table.Columns.Add (column);
			}
			else
			{
				this.CreateTable (transaction, entityType.BaseTypeId);
			}

			//	For every locally defined field (this includes field inserted
			//	through an interface, possibly locally overridden), create a
			//	column in the table.

			foreach (StructuredTypeField field in entityType.Fields.Values)
			{
				if (field.Membership == FieldMembership.Local || field.Membership == FieldMembership.LocalOverride)
				{
					if (field.Source == FieldSource.Value)
					{
						this.CreateColumn (transaction, table, field);
					}
				}
			}

			return table;
		}


		/// <summary>
		/// Creates a <see cref="DbColumn"/> to represent the given field in the given
		/// <see cref="DbTable"/>.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="table">The <see cref="DbTable"/> to which to add the <see cref="DbColumn"/>.</param>
		/// <param name="field">The field to be represented by the <see cref="DbColumn"/>.</param>
		private void CreateColumn(DbTransaction transaction, DbTable table, StructuredTypeField field)
		{
			switch (field.Relation)
			{
				case FieldRelation.None:
					this.CreateDataColumn (transaction, table, field);
					break;
				
				case FieldRelation.Reference:
					this.CreateRelationColumn (transaction, table, field, DbCardinality.Reference);
					break;
				
				case FieldRelation.Collection:
					this.CreateRelationColumn (transaction, table, field, DbCardinality.Collection);
					break;

				default:
					throw new System.NotImplementedException (string.Format ("Missing support for Relation.{0}", field.Relation));
			}
		}


		/// <summary>
		/// Creates a <see cref="DbColumn"/> for the given data field.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="table">The <see cref="DbTable"/> to which to add the <see cref="DbColumn"/>.</param>
		/// <param name="field">The field to be represented by the <see cref="DbColumn"/>.</param>
		private void CreateDataColumn(DbTransaction transaction, DbTable table, StructuredTypeField field)
		{
			DbTypeDef typeDef = this.GetOrCreateTypeDef (transaction, field.Type, field.Options);
			DbColumn column = new DbColumn (field.CaptionId, typeDef, DbColumnClass.Data, DbElementCat.ManagedUserData, DbRevisionMode.TrackChanges);

			column.Comment = column.DisplayName;

			table.Columns.Add (column);
		}


		/// <summary>
		/// Creates a <see cref="DbColumn"/> for the given relation field.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="table">The <see cref="DbTable"/> to which to add the <see cref="DbColumn"/>.</param>
		/// <param name="field">The field to be represented by the <see cref="DbColumn"/>.</param>
		/// <param name="cardinality">The cardinality of the relation.</param>
		private void CreateRelationColumn(DbTransaction transaction, DbTable table, StructuredTypeField field, DbCardinality cardinality)
		{
			System.Diagnostics.Debug.Assert (cardinality != DbCardinality.None);
			System.Diagnostics.Debug.Assert (field.CaptionId.IsValid);
			System.Diagnostics.Debug.Assert (field.Type is StructuredType);

			DbTable target = this.CreateTable (transaction, field.TypeId);
			DbColumn column = DbTable.CreateRelationColumn (transaction, this.DbInfrastructure, field.CaptionId, target, DbRevisionMode.TrackChanges, cardinality);

			table.Columns.Add (column);
		}


		/// <summary>
		/// Gets the <see cref="DbTypeDef"/> for the given type. If it is not yet known, it is
		/// created and registered to the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="type">The type whose <see cref="DbTypeDef"/> to get.</param>
		/// <param name="options">The <see cref="FieldOptions"/> of the type.</param>
		/// <returns>The <see cref="DbTypeDef"/>.</returns>
		private DbTypeDef GetOrCreateTypeDef(DbTransaction transaction, INamedType type, FieldOptions options)
		{
			type.ThrowIfNull ("type");
			type.ThrowIf (t => t is IStructuredType, "Cannot create type definition for structure");

			DbTypeDef typeDef = null;
			
			System.Diagnostics.Debug.Assert (type.CaptionId.IsValid);

			if (typeDef == null)
			{
				this.newTypesDictionary.TryGetValue (type.CaptionId, out typeDef);
			}

			if (typeDef == null)
			{
				typeDef = this.SchemaEngine.GetTypeDefinition (type.CaptionId);
			}
			
			if (typeDef == null)
			{
				typeDef = new DbTypeDef (type, options == FieldOptions.Nullable);

				this.DbInfrastructure.RegisterNewDbType (transaction, typeDef);

				this.newTypesDictionary[type.CaptionId] = typeDef;
			}

			System.Diagnostics.Debug.Assert (typeDef != null);
			System.Diagnostics.Debug.Assert (!typeDef.Key.IsEmpty);

			return typeDef;
		}


		/// <summary>
		/// Stores the mapping between the <see cref="Druid"/> and the <see cref="DbTable"/> that
		/// where created during the last call of <see cref="CreateSchema"/>.
		/// </summary>
		private Dictionary<Druid, DbTable> newTablesDictionary;


		/// <summary>
		/// Stores the mapping between the <see cref="Druid"/> and the <see cref="DbTypeDef"/> that
		/// where created during the last call of <see cref="CreateSchema"/>.
		/// </summary>
		private Dictionary<Druid, DbTypeDef> newTypesDictionary;


	}


}
