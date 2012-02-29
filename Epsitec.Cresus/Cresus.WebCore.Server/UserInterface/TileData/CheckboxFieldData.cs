using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class CheckboxFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			var textPropertyAccessor = (TextPropertyAccessor) this.PropertyAccessor;

			return new CheckBoxField ()
			{
				PropertyAccessorId = textPropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = bool.Parse (textPropertyAccessor.GetString (entity))
			};
		}

	}


}