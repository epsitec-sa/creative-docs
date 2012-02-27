using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class EntityFieldData : AbstractFieldData
	{


		protected override AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, AbstractPanelFieldAccessor panelFieldAccessor)
		{
			var entityPanelFieldAccessor = (EntityPanelFieldAccessor) panelFieldAccessor;

			var target = entityPanelFieldAccessor.GetEntity (entity);

			var entityField = new EntityField ()
			{
				PanelFieldAccessorId = entityPanelFieldAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = entityIdGetter (target),
			};

			var possibleValues = entitiesGetter (entityPanelFieldAccessor.Type)
				.Select (e => Tuple.Create (entityIdGetter (e), e.GetCompactSummary ().ToString ()));

			entityField.PossibleValues.AddRange (possibleValues);

			return entityField;
		}


	}


}
