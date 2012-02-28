using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class EnumerationFieldData : AbstractFieldData
	{


		protected override AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, AbstractPropertyAccessor propertyAccessor)
		{
			var textPropertyAccessor = (TextPropertyAccessor) propertyAccessor;

			return new EnumerationField ()
			{
				PropertyAccessorId = textPropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = textPropertyAccessor.GetString (entity),
				TypeName = textPropertyAccessor.Type.AssemblyQualifiedName,
			};
		}


	}


}
