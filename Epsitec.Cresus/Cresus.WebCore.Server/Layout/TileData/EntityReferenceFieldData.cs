using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using Epsitec.Cresus.WebCore.Server.NancyModules;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class EntityReferenceFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var entityReferencePropertyAccessor = (EntityReferencePropertyAccessor) this.PropertyAccessor;

			var target = entityReferencePropertyAccessor.GetEntity (entity);

			var entityField = new EntityReferenceField ()
			{
				PropertyAccessorId = InvariantConverter.ToString (entityReferencePropertyAccessor.Id),
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = layoutBuilder.GetEntityId (target) ?? "null",
			};

			var possibleValues = this.GetPossibleValues (layoutBuilder);

			entityField.PossibleValues.AddRange (possibleValues);

			return entityField;
		}


		private IEnumerable<Tuple<string, string>> GetPossibleValues(LayoutBuilder layoutBuilder)
		{
			if (this.PropertyAccessor.Property.IsNullable)
			{
				yield return Tuple.Create (Constants.KeyForNullValue, Constants.TextForNullValue);
			}

			foreach (var entity in layoutBuilder.GetEntities (this.PropertyAccessor.Type))
			{
				var entityId = layoutBuilder.GetEntityId (entity);
				var entitySummary = entity.GetCompactSummary ().ToString ();

				yield return Tuple.Create (entityId, entitySummary);
			}
		}


	}


}
