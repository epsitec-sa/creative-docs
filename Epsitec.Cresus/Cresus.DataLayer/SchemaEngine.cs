//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>SchemaEngine</c> class manages the mapping between entities and
	/// database table definitions.
	/// </summary>
	public sealed class SchemaEngine : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SchemaEngine"/> class.
		/// </summary>
		/// <param name="infrastructure">The database engine.</param>
		public SchemaEngine(DbInfrastructure infrastructure)
		{
			System.Diagnostics.Debug.Assert (SchemaEngine.GetSchemaEngine (infrastructure) == null);

			this.infrastructure  = infrastructure;
			this.context         = this.infrastructure.DefaultContext;
			this.resourceManager = this.context.ResourceManager;

			this.tableDefinitionCache = new Dictionary<Druid, DbTable> ();
			this.typeDefinitionCache  = new Dictionary<Druid, DbTypeDef> ();

			SchemaEngine.SetSchemaEngine (infrastructure, this);
		}


		/// <summary>
		/// Gets the associated database engine.
		/// </summary>
		/// <value>The <see cref="DbInfrastructure"/> instance.</value>
		public DbInfrastructure Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}

		/// <summary>
		/// Gets a table definition for the specified entity id, creating it
		/// if it was not yet known to the engine.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <returns>The root <see cref="DbTable"/> for the specified entity.</returns>
		public DbTable CreateTableDefinition(Druid entityId)
		{
			SchemaEngineTableBuilder builder = new SchemaEngineTableBuilder (this);

			using (builder.BeginTransaction ())
			{
				builder.Add (entityId);
				builder.CommitTransaction ();
			}

			builder.UpdateCache ();

			return builder.GetRootTable ();
		}

		/// <summary>
		/// Finds a table definition for the specified entity id.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <returns>The root <see cref="DbTable"/> for the specified entity or
		/// <c>null</c> if the entity is not yet known to the engine.</returns>
		public DbTable FindTableDefinition(Druid entityId)
		{
			DbTable table;

			if (this.tableDefinitionCache.TryGetValue (entityId, out table))
			{
				return table;
			}
			else
			{
				using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					table = this.infrastructure.ResolveDbTable (transaction, entityId);
					transaction.Commit ();
				}

				if (table != null)
				{
					this.tableDefinitionCache[entityId] = table;
				}

				return table;
			}
		}

		/// <summary>
		/// Finds the type definition for the specified type object.
		/// </summary>
		/// <param name="type">The type object.</param>
		/// <returns>The <see cref="DbTypeDef"/> instance or <c>null</c> if
		/// the type is not yet known to the engine.</returns>
		public DbTypeDef FindTypeDefinition(INamedType type)
		{
			DbTypeDef typeDef;

			if (type == null)
			{
				return null;
			}
			else if (this.typeDefinitionCache.TryGetValue (type.CaptionId, out typeDef))
			{
				return typeDef;
			}
			else
			{
				using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					typeDef = this.infrastructure.ResolveDbType (transaction, type);
					transaction.Commit ();
				}

				if (typeDef != null)
				{
					this.typeDefinitionCache[type.CaptionId] = typeDef;
				}

				return typeDef;
			}
		}

		/// <summary>
		/// Gets the name of the main data table used by an entity.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <returns>The name of the data table or <c>null</c> if none
		/// can be found.</returns>
		public string GetDataTableName(Druid entityId)
		{
			DbTable tableDef = this.FindTableDefinition (entityId);
			return tableDef == null ? null : tableDef.Name;
		}

		/// <summary>
		/// Gets the name of the data column given an entity field id.
		/// </summary>
		/// <param name="fieldId">The field id.</param>
		/// <returns>The name of the data column.</returns>
		public string GetDataColumnName(string fieldId)
		{
			System.Diagnostics.Debug.Assert (fieldId.StartsWith ("["));
			System.Diagnostics.Debug.Assert (fieldId.EndsWith ("]"));

			return fieldId.Substring (1, fieldId.Length-2);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.infrastructure != null)
				{
					if (SchemaEngine.GetSchemaEngine (this.infrastructure) == this)
					{
						SchemaEngine.SetSchemaEngine (this.infrastructure, null);
					}
				}
			}
			
			base.Dispose (disposing);
		}


		#region Internal Support Methods

		internal StructuredType GetEntityType(Druid entityId)
		{
			return TypeRosetta.CreateTypeObject (this.resourceManager, entityId) as StructuredType;
		}

		internal void AddTableDefinitionToCache(Druid entityId, DbTable table)
		{
			if (this.tableDefinitionCache.ContainsKey (entityId))
			{
				//	Nothing to do.
			}
			else
			{
				this.tableDefinitionCache[entityId] = table;
			}
		}

		internal void AddTypeDefinitionToCache(Druid typeId, DbTypeDef type)
		{
			if (this.typeDefinitionCache.ContainsKey (typeId))
			{
				//	Nothing to do.
			}
			else
			{
				this.typeDefinitionCache[typeId] = type;
			}
		}

		#endregion


		public static void SetSchemaEngine(DependencyObject obj, SchemaEngine value)
		{
			if (value == null)
			{
				obj.ClearValue (SchemaEngine.SchemaEngineProperty);
			}
			else
			{
				obj.SetValue (SchemaEngine.SchemaEngineProperty, value);
			}
		}

		public static SchemaEngine GetSchemaEngine(DependencyObject obj)
		{
			return obj.GetValue (SchemaEngine.SchemaEngineProperty) as SchemaEngine;
		}

		public static readonly DependencyProperty SchemaEngineProperty = DependencyProperty.RegisterAttached ("SchemaEngine", typeof (SchemaEngine), typeof (SchemaEngine));

		readonly DbInfrastructure infrastructure;
		readonly DbContext context;
		readonly ResourceManager resourceManager;

		readonly Dictionary<Druid, DbTable> tableDefinitionCache;
		readonly Dictionary<Druid, DbTypeDef> typeDefinitionCache;
	}
}
