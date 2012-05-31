using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Schema;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class FieldChecker
	{


		public FieldChecker(HashSet<AbstractEntity> entities, EntityTypeEngine typeEngine)
		{
			this.entities = entities;
			this.typeEngine = typeEngine;
		}


		public void CheckValueField(AbstractEntity entity, Druid fieldId)
		{
			this.CheckEntity (entity);

			if (!this.IsValueField (entity, fieldId))
			{
				throw new ArgumentException ("Invalid value field id");
			}
		}


		public void CheckReferenceField(AbstractEntity entity, Druid fieldId)
		{
			this.CheckEntity (entity);

			if (!this.IsReferenceField (entity, fieldId))
			{
				throw new ArgumentException ("Invalid reference field id");
			}
		}


		public void CheckCollectionField(AbstractEntity entity, Druid fieldId, string name)
		{
			this.CheckEntity (entity);
			
			if (!this.IsCollectionField (entity, fieldId))
			{
				throw new ArgumentException ("Invalid collection field id");
			}

			if (!this.IsCollectionFieldName (name))
			{
				throw new ArgumentException ("Invalid collection field column name");
			}
		}


		public void CheckInternalEntityField(AbstractEntity entity, string name)
		{
			this.CheckEntity (entity);

			if (!this.IsInternalEntityField (name))
			{
				throw new ArgumentException ("Invalid internal field name");
			}
		}


		private bool IsValueField(AbstractEntity entity, Druid fieldId)
		{
			return this.IsField (entity, fieldId, FieldRelation.None);
		}


		private bool IsReferenceField(AbstractEntity entity, Druid fieldId)
		{
			return this.IsField (entity, fieldId, FieldRelation.Reference);
		}


		private bool IsCollectionField(AbstractEntity entity, Druid fieldId)
		{
			return this.IsField (entity, fieldId, FieldRelation.Collection);
		}


		private bool IsField(AbstractEntity entity, Druid fieldId, FieldRelation relation)
		{
			var leafEntityId = entity.GetEntityStructuredTypeId ();
			var field = this.typeEngine.GetField (leafEntityId, fieldId);

			return field != null && field.Relation == relation;
		}


		private bool IsInternalEntityField(string name)
		{
			return name == EntitySchemaBuilder.EntityTableColumnIdName
				|| name == EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName;
		}


		private bool IsCollectionFieldName(string name)
		{
			return name == EntitySchemaBuilder.EntityFieldTableColumnRankName;
		}


		private void CheckEntity(AbstractEntity entity)
		{
			if (!this.entities.Contains (entity))
			{
				var message = "Entity in condition or sort clause is not in reachable graph.";

				throw new ArgumentException (message);
			}
		}


		private readonly HashSet<AbstractEntity> entities;


		private readonly EntityTypeEngine typeEngine;


	}


}
