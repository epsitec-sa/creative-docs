using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class EntityReferenceFieldData : AbstractFieldData
	{


		protected override AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, AbstractPropertyAccessor propertyAccessor)
		{
			var entityReferencePropertyAccessor = (EntityReferencePropertyAccessor) propertyAccessor;

			var target = entityReferencePropertyAccessor.GetEntity (entity);

			var entityField = new EntityReferenceField ()
			{
				PropertyAccessorId = entityReferencePropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = entityIdGetter (target),
			};

			var possibleValues = entitiesGetter (entityReferencePropertyAccessor.Type)
				.Select (e => Tuple.Create (entityIdGetter (e), e.GetCompactSummary ().ToString ()));

			entityField.PossibleValues.AddRange (possibleValues);

			return entityField;
		}


	}


}
