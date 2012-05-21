using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.NancyModules;
using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class EntityReferenceFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			var entityReferencePropertyAccessor = (EntityReferencePropertyAccessor) this.PropertyAccessor;

			var target = entityReferencePropertyAccessor.GetEntity (entity);

			var entityField = new EntityReferenceField ()
			{
				PropertyAccessorId = entityReferencePropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = entityIdGetter (target) ?? "null",
			};

			var possibleValues = this.GetPossibleValues (entityIdGetter, entitiesGetter);

			entityField.PossibleValues.AddRange (possibleValues);

			return entityField;
		}


		private IEnumerable<Tuple<string, string>> GetPossibleValues(Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			if (this.PropertyAccessor.Property.IsNullable)
			{
				yield return Tuple.Create (EntityModule.StringForNullValue, "");
			}

			foreach (var entity in entitiesGetter (this.PropertyAccessor.Type))
			{
				var entityId = entityIdGetter (entity);
				var entitySummary = entity.GetCompactSummary ().ToString ();

				yield return Tuple.Create (entityId, entitySummary);
			}
		}


	}


}
