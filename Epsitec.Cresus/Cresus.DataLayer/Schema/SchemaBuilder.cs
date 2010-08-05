//	Copyright � 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
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
	internal sealed class SchemaBuilder
	{
		
		
		/// <summary>
		/// Builds a new <c>SchemaBuilder.</c>
		/// </summary>
		public SchemaBuilder(DbInfrastructure dbInfrastructure)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			
			this.DbInfrastructure = dbInfrastructure;

			this.tableCache = new Dictionary<Druid, DbTable> ();
			this.typeCache = new Dictionary<Druid, DbTypeDef> ();
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
		/// Creates the schema corresponding to the given <see cref="Druid"/> and register it to the
		/// database. This method will recursively create and register everything that is required
		/// to build the schema.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="entityId">The <see cref="Druid"/> of the schema to create.</param>
		/// <returns>The <see cref="DbTable"/> that have been created.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="transaction"/> is null.</exception>
		public IEnumerable<DbTable> CreateSchema(DbTransaction transaction, Druid entityId)
		{
			transaction.ThrowIfNull ("transaction");

			List<DbTable> newTables = new List<DbTable> ();

			this.GetOrCreateTable (transaction, newTables, entityId);

			foreach (DbTable table in newTables)
			{
				this.DbInfrastructure.RegisterNewDbTable (transaction, table);
			}

			foreach (DbTable table in newTables)
			{
				this.DbInfrastructure.RegisterColumnRelations (transaction, table);
			}

			return newTables;
		}


		/// <summary>
		/// Gets the <see cref="DbTable"/> corresponding to a <see cref="Druid"/> or creates it with
		/// all its dependencies if it does not exist.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="newTables">The list of the created <see cref="DbTable"/>.</param>
		/// <param name="entityId">The <see cref="Druid"/> of the <see cref="DbTable"/> to create.</param>
		/// <returns>The root <see cref="DbTable"/>.</returns>
		private DbTable GetOrCreateTable(DbTransaction transaction, List<DbTable> newTables, Druid entityId)
		{
			// Ok, this looks like kind of simple to me, to simple too be alright, so I'll explain.
			// The ?? operator ensures that first we look in the cache. If there is something in the
			// cache, we stop here, but if we have nothing in the cache, we get a null and thus, we
			// look in the database. Again, if we have nothing in the database, we go on and create
			// the DbTable.
			// Marc
			
			return this.LookForTableInCache (entityId)
				?? this.LookForTableInDatabase (transaction, entityId)
				?? this.BuildTable (transaction, newTables, entityId);
		}


		/// <summary>
		/// Looks for the <see cref="DbTable"/> corresponding to a <see cref="Druid"/> in the local
		/// cache.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> of the <see cref="DbTable"/> to look for.</param>
		/// <returns>The <see cref="DbTable"/> if it is in the cache, or null.</returns>
		private DbTable LookForTableInCache(Druid entityId)
		{
			DbTable table;

			this.tableCache.TryGetValue (entityId, out table);
			
			return table;
		}


		/// <summary>
		/// Looks the <see cref="DbTable"/> corresponding to a <see cref="Druid"/> in the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="entityId">The <see cref="Druid"/> of the <see cref="DbTable"/> to look for.</param>
		/// <returns>The <see cref="DbTable"/> if it is in the database, or null.</returns>
		private DbTable LookForTableInDatabase(DbTransaction transaction, Druid entityId)
		{
			DbTable table = this.DbInfrastructure.ResolveDbTable (transaction, entityId);

			if (table != null)
			{
				this.tableCache[entityId] = table;
			}

			return table;
		}


		/// <summary>
		/// Creates the <see cref="DbTable"/> corresponding to a <see cref="Druid"/> and everything
		/// which is required such as the parent and neighbor <see cref="DbTable"/> without registering
		/// them to the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="newTables">The list of the created <see cref="DbTable"/>.</param>
		/// <param name="entityId">The <see cref="Druid"/> of the <see cref="DbTable"/> to create.</param>
		/// <returns>The new <see cref="DbTable"/>.</returns>
		private DbTable BuildTable(DbTransaction transaction, List<DbTable> newTables, Druid entityId)
		{
			ResourceManager manager = this.DbInfrastructure.DefaultContext.ResourceManager;
			StructuredType entityType = TypeRosetta.CreateTypeObject (manager, entityId) as StructuredType;

			if (entityType == null)
			{
				throw new System.ArgumentException ("Invalid entity ID", "entityId");
			}

			DbTable table = this.DbInfrastructure.CreateDbTable (entityId, DbElementCat.ManagedUserData, DbRevisionMode.TrackChanges);
			table.Comment = table.DisplayName;

			newTables.Add (table);
			this.tableCache[entityId] = table;

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
				this.GetOrCreateTable (transaction, newTables, entityType.BaseTypeId);
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
						this.CreateColumn (transaction, newTables, table, field);
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
		/// <param name="newTables">The list of the created <see cref="DbTable"/>.</param>
		/// <param name="table">The <see cref="DbTable"/> to which to add the <see cref="DbColumn"/>.</param>
		/// <param name="field">The field to be represented by the <see cref="DbColumn"/>.</param>
		private void CreateColumn(DbTransaction transaction, List<DbTable> newTables, DbTable table, StructuredTypeField field)
		{
			switch (field.Relation)
			{
				case FieldRelation.None:
					this.CreateDataColumn (transaction, table, field);
					break;
				
				case FieldRelation.Reference:
					this.CreateRelationColumn (transaction, newTables, table, field, DbCardinality.Reference);
					break;
				
				case FieldRelation.Collection:
					this.CreateRelationColumn (transaction, newTables, table, field, DbCardinality.Collection);
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
		/// <param name="newTables">The list of the created <see cref="DbTable"/>.</param>
		/// <param name="table">The <see cref="DbTable"/> to which to add the <see cref="DbColumn"/>.</param>
		/// <param name="field">The field to be represented by the <see cref="DbColumn"/>.</param>
		/// <param name="cardinality">The cardinality of the relation.</param>
		private void CreateRelationColumn(DbTransaction transaction, List<DbTable> newTables, DbTable table, StructuredTypeField field, DbCardinality cardinality)
		{
			cardinality.ThrowIf (c => c == DbCardinality.None, "cardinality cannot be 'none'");
			field.ThrowIf (f => !f.CaptionId.IsValid, "field caption must be valid");
			field.ThrowIf (f => !(f.Type is StructuredType), "field must be of type StructuredType");
			
			DbTable target = this.GetOrCreateTable (transaction, newTables, field.TypeId);
			DbColumn column = DbTable.CreateRelationColumn (transaction, this.DbInfrastructure, field.CaptionId, target, DbRevisionMode.TrackChanges, cardinality);

			table.Columns.Add (column);
		}


		/// <summary>
		/// Gets the <see cref="DbTypeDef"/> for the given type or creates it  and register it to the
		/// database if it does not exists.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="type">The type whose <see cref="DbTypeDef"/> to get or create.</param>
		/// <param name="options">The <see cref="FieldOptions"/> of the type.</param>
		/// <returns>The <see cref="DbTypeDef"/>.</returns>
		private DbTypeDef GetOrCreateTypeDef(DbTransaction transaction, INamedType type, FieldOptions options)
		{
			type.ThrowIfNull ("type");
			type.ThrowIf (t => t is IStructuredType, "Cannot create type definition for structure");

			// Here I used the same trick as above. If it's not clear, look at the comments in the
			// GetOrCreateTable method.
			// Marc

			DbTypeDef typeDef = this.LookForTypeDefInCache (type, options)
				?? this.LookForTypeDefInDatabase (transaction, type, options)
				?? this.BuildTypeDef (transaction, type, options);

			System.Diagnostics.Debug.Assert (typeDef != null);
			System.Diagnostics.Debug.Assert (!typeDef.Key.IsEmpty);

			return typeDef;
		}


		/// <summary>
		/// Looks for the <see cref="DbTypeDef"/> corresponding to a <see cref="INamedType"/> in the
		/// local cache.
		/// </summary>
		/// <param name="type">The <see cref="INamedType"/> to look for.</param>
		/// <param name="options">The <see cref="FieldOptions"/> of the type.</param>
		/// <returns>The <see cref="DbTypeDef"/> if it is in the cache, or null.</returns>
		private DbTypeDef LookForTypeDefInCache(INamedType type, FieldOptions options)
		{
			DbTypeDef typeDef;

			this.typeCache.TryGetValue (type.CaptionId, out typeDef);

			return this.CreateNewTypeDefIfNecessary (type, options, typeDef);
		}


		/// <summary>
		/// Looks for the <see cref="DbTypeDef"/> corresponding to a <see cref="INamedType"/> in the
		/// database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="type">The <see cref="INamedType"/> to look for.</param>
		/// <param name="options">The <see cref="FieldOptions"/> of the type.</param>
		/// <returns>The <see cref="DbTypeDef"/> if it is in the cache, or null.</returns>
		private DbTypeDef LookForTypeDefInDatabase(DbTransaction transaction, INamedType type, FieldOptions options)
		{
			DbTypeDef typeDef = this.DbInfrastructure.ResolveDbType (transaction, type);

			if (typeDef != null)
			{
				this.typeCache[type.CaptionId] = typeDef;
			}

			return this.CreateNewTypeDefIfNecessary (type, options, typeDef);
		}


		/// <summary>
		/// Create the <see cref="DbTypeDef"/> corresponding to a <see cref="INamedType"/> and
		/// registers it to the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use.</param>
		/// <param name="type">The <see cref="INamedType"/> to create.</param>
		/// <param name="options">The <see cref="FieldOptions"/> to use.</param>
		/// <returns>The new <see cref="DbTypeDef"/>.</returns>
		private DbTypeDef BuildTypeDef(DbTransaction transaction, INamedType type, FieldOptions options)
		{
			DbTypeDef typeDef = new DbTypeDef (type, options == FieldOptions.Nullable);

			this.DbInfrastructure.RegisterNewDbType (transaction, typeDef);

			this.typeCache[type.CaptionId] = typeDef;

			return typeDef;
		}



		/// <summary>
		/// Checks that the given <see cref="DbTypeDef"/> has the proper value for its property
		/// <see cref="DbTypeDef.IsNullable"/> and creates a new one with the proper value if this
		/// is not the case.
		/// </summary>
		/// <param name="type">The <see cref="INamedType"/> of the <see cref="DbTypeDef"/>.</param>
		/// <param name="options">The <see cref="FieldOptions"/> that the <see cref="DbTypeDef"/> should match.</param>
		/// <param name="typeDef">The <see cref="DbTypeDef"/> to check.</param>
		/// <returns>The given <see cref="DbTypeDef"/> or a new one.</returns>
		private DbTypeDef CreateNewTypeDefIfNecessary(INamedType type, FieldOptions options, DbTypeDef typeDef)
		{
			DbTypeDef newTypeDef = typeDef;

			if (typeDef != null)
			{
				if (options == FieldOptions.Nullable && !typeDef.IsNullable)
				{
					newTypeDef = new DbTypeDef (type, typeDef.Key, true);
				}
				else if (options == FieldOptions.None && typeDef.IsNullable)
				{
					newTypeDef = new DbTypeDef (type, typeDef.Key, false);
				}
			}

			return newTypeDef;
		}


		/// <summary>
		/// Stores the mapping between all the <see cref="Druid"/> and the <see cref="DbTable"/> that
		/// have been seen by this instance.
		/// </summary>
		private Dictionary<Druid, DbTable> tableCache;


		/// <summary>
		/// Stores the mapping between the <see cref="Druid"/> and the <see cref="DbTypeDef"/> that
		/// have been seen by this instance.
		/// </summary>
		private Dictionary<Druid, DbTypeDef> typeCache;


	}


}
