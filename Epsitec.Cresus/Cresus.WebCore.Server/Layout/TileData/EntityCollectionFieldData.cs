using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{

	
	internal sealed class EntityCollectionFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var entityCollectionPropertyAccessor = (EntityCollectionPropertyAccessor) this.PropertyAccessor;

			var collectionField = new EntityCollectionField ()
			{
				PropertyAccessorId = InvariantConverter.ToString (entityCollectionPropertyAccessor.Id),
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
			};

			var possibleValues = layoutBuilder.GetEntities (entityCollectionPropertyAccessor.CollectionType);
			var checkedValues = entityCollectionPropertyAccessor.GetEntityCollection (entity);

			var checkBoxFields = possibleValues.Select ((v, i) => new EntityCollectionCheckBoxField ()
			{
				Value = checkedValues.Contains (v),
				InputValue = layoutBuilder.GetEntityId (v),
				Label = v.GetSummary ().ToString (),
				Index = i
			});

			collectionField.CheckBoxFields.AddRange (checkBoxFields);

			return collectionField;
		}


	}


}
