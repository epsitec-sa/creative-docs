using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.NancyModules;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class EnumerationFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			var textPropertyAccessor = (TextPropertyAccessor) this.PropertyAccessor;

			return new EnumerationField ()
			{
				PropertyAccessorId = InvariantConverter.ToString (textPropertyAccessor.Id),
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = textPropertyAccessor.GetString (entity) ?? Constants.KeyForNullValue,
				TypeName = textPropertyAccessor.Type.AssemblyQualifiedName,
			};
		}


	}


}
