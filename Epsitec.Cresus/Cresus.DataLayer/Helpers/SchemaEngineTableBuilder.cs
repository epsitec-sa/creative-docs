//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Helpers
{
	internal class SchemaEngineTableBuilder
	{
		public SchemaEngineTableBuilder(SchemaEngine engine)
		{
			this.engine = engine;
			this.tables = new List<DbTable> ();
		}

		public System.IDisposable BeginTransaction()
		{
			if ((this.transaction != null) &&
				(this.transaction.IsActive))
			{
				throw new System.InvalidOperationException ("SchemaEngineTableBuilder already has an active transaction");
			}

			this.transaction = this.engine.Infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite);

			return this.transaction;
		}

		public void CommitTransaction()
		{
			this.transaction.Commit ();
			this.transaction = null;
		}


		public void Add(Druid entityId)
		{
			StructuredType entityType = this.engine.GetEntityType (entityId);
			string         tableName  = this.engine.GetUserFriendlyTableName (entityId);

			if (entityType == null)
			{
				throw new System.ArgumentException ("Invalid entity ID", "entityId");
			}
			
			this.AssertTransaction ();

			DbInfrastructure infrastructure = this.engine.Infrastructure;
			DbTable table = infrastructure.CreateDbTable (tableName, DbElementCat.ManagedUserData, DbRevisionMode.TrackChanges);

			table.DefineCaptionId (entityId);
			
			this.tables.Add (table);

			if (entityType.BaseTypeId.IsEmpty)
			{
				//	If this entity has no parent in the class hierarchy, then we
				//	need to add a special identification column, which can be used
				//	to map a row to its proper derived entity class.

				DbTypeDef typeDef = infrastructure.ResolveDbType (this.transaction, Tags.TypeKeyId);
				DbColumn column = new DbColumn (Tags.ColumnInstanceType, typeDef, DbColumnClass.Data, DbElementCat.Internal, DbRevisionMode.Immutable);

				table.Columns.Add (column);
			}

			foreach (StructuredTypeField field in entityType.Fields.Values)
			{
				if ((field.Membership == FieldMembership.Local) ||
					(field.Membership == FieldMembership.LocalOverride))
				{
					//	Add a column for the specified field.

					this.AddColumn (table, field);
				}
			}

			infrastructure.RegisterNewDbTable (this.transaction, table);
		}

		private void AddColumn(DbTable table, StructuredTypeField field)
		{
			switch (field.Relation)
			{
				case FieldRelation.None:
					this.AddDataColumn (table, field);
					break;
				
				case FieldRelation.Reference:
					break;
				
				case FieldRelation.Collection:
					break;

				default:
					throw new System.NotImplementedException (string.Format ("Missing support for Relation.{0}", field.Relation));
			}
		}

		private void AddDataColumn(DbTable table, StructuredTypeField field)
		{
			string    typeName = this.engine.GetTypeName (field.TypeId);
			DbTypeDef typeDef  = this.GetTypeDef (typeName, field.Type);
			DbColumn  column   = new DbColumn (field.CaptionId, typeDef, DbColumnClass.Data, DbElementCat.ManagedUserData, DbRevisionMode.TrackChanges);

			table.Columns.Add (column);
		}

		private DbTypeDef GetTypeDef(string typeName, INamedType type)
		{
			this.AssertTransaction ();

			if (type == null)
			{
				throw new System.ArgumentNullException ("type");
			}
			if (type is IStructuredType)
			{
				throw new System.InvalidOperationException ("Cannot create type definition for structure");
			}

			DbInfrastructure infrastructure = this.engine.Infrastructure;

			DbTypeDef typeDef;

			typeDef = infrastructure.ResolveDbType (this.transaction, typeName);

			if (typeDef == null)
			{
				typeDef = new DbTypeDef (type, typeName);

				infrastructure.RegisterNewDbType (this.transaction, typeDef);
			}

			System.Diagnostics.Debug.Assert (typeDef != null);
			System.Diagnostics.Debug.Assert (!typeDef.Key.IsEmpty);

			return typeDef;
			
		}

		public DbTable GetFirstTable()
		{
			if (this.tables.Count > 0)
			{
				return this.tables[0];
			}
			else
			{
				return null;
			}
		}


		private void AssertTransaction()
		{
			if ((this.transaction != null) &&
				(this.transaction.IsActive))
			{
				return;
			}
			else
			{
				throw new System.InvalidOperationException ("SchemaEngineTableBuilder has no active transaction");
			}
		}

		SchemaEngine engine;
		List<DbTable> tables;
		DbTransaction transaction;
	}
}
