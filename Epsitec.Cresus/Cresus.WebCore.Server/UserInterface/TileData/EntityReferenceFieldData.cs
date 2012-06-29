using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.NancyModules;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class EntityReferenceFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			var entityReferencePropertyAccessor = (EntityReferencePropertyAccessor) this.PropertyAccessor;

			var target = entityReferencePropertyAccessor.GetEntity (entity);

			var entityField = new EntityReferenceField ()
			{
				PropertyAccessorId = entityReferencePropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = panelBuilder.GetEntityId (target) ?? "null",
			};

			var possibleValues = this.GetPossibleValues (panelBuilder);

			entityField.PossibleValues.AddRange (possibleValues);

			return entityField;
		}


		private IEnumerable<Tuple<string, string>> GetPossibleValues(PanelBuilder panelBuilder)
		{
			if (this.PropertyAccessor.Property.IsNullable)
			{
				yield return Tuple.Create (Constants.KeyForNullValue, Constants.TextForNullValue);
			}

			foreach (var entity in panelBuilder.GetEntities (this.PropertyAccessor.Type))
			{
				var entityId = panelBuilder.GetEntityId (entity);
				var entitySummary = entity.GetCompactSummary ().ToString ();

				yield return Tuple.Create (entityId, entitySummary);
			}
		}


	}


}
