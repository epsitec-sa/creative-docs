
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;



namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{

	
	internal sealed class TextFieldData : AbstractFieldData
	{


		protected override AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, AbstractPanelFieldAccessor panelFieldAccessor)
		{
			var stringPanelFieldAccessor = (StringPanelFieldAccessor) panelFieldAccessor;

			return new TextField ()
			{
				PanelFieldAccessorId = stringPanelFieldAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = stringPanelFieldAccessor.GetString (entity),
			};
		}


	}


}
