using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class DateFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			var textPropertyAccessor = (TextPropertyAccessor) this.PropertyAccessor;

			return new DateField ()
			{
				PropertyAccessorId = textPropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = textPropertyAccessor.GetString (entity),
			};
		}


	}


}
