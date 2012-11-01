using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using Epsitec.Cresus.WebCore.Server.NancyModules;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class EnumerationFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var enumerationPropertyAccessor = (EnumerationPropertyAccessor) this.PropertyAccessor;

			return new EnumerationField ()
			{
				PropertyAccessorId = enumerationPropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = (string) enumerationPropertyAccessor.GetValue (entity),
				TypeName = layoutBuilder.GetTypeName (enumerationPropertyAccessor.Type),
			};
		}


	}


}
