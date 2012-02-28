using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{

	
	internal sealed class EntityCollectionFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			var entityCollectionPropertyAccessor = (EntityCollectionPropertyAccessor) this.PropertyAccessor;

			var collectionField = new EntityCollectionField ()
			{
				PropertyAccessorId = entityCollectionPropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
			};

			var possibleValues = entitiesGetter (entityCollectionPropertyAccessor.CollectionType);
			var checkedValues = entityCollectionPropertyAccessor.GetEntityCollection (entity);

			var checkBoxFields = possibleValues.Select ((v, i) => new CheckBoxField ()
			{
				Checked = checkedValues.Contains (v),
				InputValue = entityIdGetter (v),
				Label = v.GetSummary ().ToString (),
				Index = i
			});

			collectionField.CheckBoxFields.AddRange (checkBoxFields);

			return collectionField;
		}


	}


}
