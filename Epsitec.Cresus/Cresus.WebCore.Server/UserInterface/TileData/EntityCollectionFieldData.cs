using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{

	
	internal sealed class EntityCollectionFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			var entityCollectionPropertyAccessor = (EntityCollectionPropertyAccessor) this.PropertyAccessor;

			var collectionField = new EntityCollectionField ()
			{
				PropertyAccessorId = InvariantConverter.ToString (entityCollectionPropertyAccessor.Id),
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
			};

			var possibleValues = panelBuilder.GetEntities (entityCollectionPropertyAccessor.CollectionType);
			var checkedValues = entityCollectionPropertyAccessor.GetEntityCollection (entity);

			var checkBoxFields = possibleValues.Select ((v, i) => new EntityCollectionCheckBoxField ()
			{
				Value = checkedValues.Contains (v),
				InputValue = panelBuilder.GetEntityId (v),
				Label = v.GetSummary ().ToString (),
				Index = i
			});

			collectionField.CheckBoxFields.AddRange (checkBoxFields);

			return collectionField;
		}


	}


}
