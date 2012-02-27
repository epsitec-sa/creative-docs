using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{

	
	internal sealed class CollectionFieldData : AbstractFieldData
	{


		protected override AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, AbstractPanelFieldAccessor panelFieldAccessor)
		{
			var entityListPanelFieldAccessor = (EntityListPanelFieldAccessor) panelFieldAccessor;

			var collectionField = new CollectionField ()
			{
				PanelFieldAccessorId = entityListPanelFieldAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
			};

			var possibleValues = entitiesGetter (entityListPanelFieldAccessor.CollectionType);
			var checkedValues = entityListPanelFieldAccessor.GetCollection (entity).Cast<AbstractEntity> ();

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
