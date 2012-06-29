using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class CheckboxFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(PanelBuilder panelBuilder, AbstractEntity entity)
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