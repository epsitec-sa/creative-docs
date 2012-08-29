using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class BooleanFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var textPropertyAccessor = (TextPropertyAccessor) this.PropertyAccessor;

			return new BooleanField ()
			{
				PropertyAccessorId = InvariantConverter.ToString (textPropertyAccessor.Id),
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = bool.Parse (textPropertyAccessor.GetString (entity))
			};
		}

	}


}