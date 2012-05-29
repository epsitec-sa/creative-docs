using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Schema;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal static class EntityHelper
	{


		internal static IEnumerable<AbstractEntity> GetChildren(EntityTypeEngine typeEngine, AbstractEntity entity)
		{
			return EntityHelper
				.GetFieldsWithChildren (typeEngine, entity)
				.Select (x => x.Item2)
				.Distinct ();
		}


		internal static IEnumerable<Tuple<Druid, AbstractEntity>> GetFieldsWithChildren(EntityTypeEngine typeEngine, AbstractEntity entity)
		{
			var result = new HashSet<Tuple<Druid, AbstractEntity>> ();
			
			var entityTypeId = entity.GetEntityStructuredTypeId ();

			foreach(var field in typeEngine.GetReferenceFields (entityTypeId))
			{
				var fieldId = field.CaptionId;
				var fieldName = fieldId.ToResourceId ();

				if (entity.IsFieldDefined (fieldName))
				{
					var target = entity.GetField<AbstractEntity> (fieldName);

					result.Add (Tuple.Create (fieldId, target));
				}
			}

			foreach (var field in typeEngine.GetCollectionFields (entityTypeId))
			{
				var fieldId = field.CaptionId;
				var fieldName = fieldId.ToResourceId ();

				if (entity.IsFieldNotEmpty (fieldName))
				{
					var targets = entity.GetFieldCollection<AbstractEntity> (fieldName);

					foreach(var target in targets)
					{
						result.Add (Tuple.Create (fieldId, target));
					}
				}
			}

			return result;
		}


		public static bool HasRelationToPersistentTarget(EntityTypeEngine typeEngine, DataContext dataContext, AbstractEntity entity)
		{
			var children = EntityHelper.GetChildren (typeEngine, entity);

			return children.Any (dataContext.IsPersistent);
		}


		public static bool HasValueFieldDefined(EntityTypeEngine typeEngine, AbstractEntity entity)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var valueFields = typeEngine.GetValueFields (leafEntityTypeId);
			
			return valueFields.Any (f => entity.IsFieldDefined (f.Id));
		}


		public static bool HasCollectionFieldDefined(EntityTypeEngine typeEngine, AbstractEntity entity)
		{
			var leafEntityId = entity.GetEntityStructuredTypeId ();
			var collectionFields = typeEngine.GetCollectionFields (leafEntityId);

			return collectionFields.Any (f => entity.IsFieldNotEmpty (f.Id));
		}


	}


}
