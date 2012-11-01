using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using Epsitec.Cresus.WebCore.Server.NancyModules;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class EntityReferenceFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var entityReferencePropertyAccessor = (EntityReferencePropertyAccessor) this.PropertyAccessor;
			var target = entityReferencePropertyAccessor.GetEntity (entity);

			return new EntityReferenceField ()
			{
				PropertyAccessorId = entityReferencePropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				TypeName = layoutBuilder.GetTypeName (entityReferencePropertyAccessor.Type),
				Value = EntityValue.Create (layoutBuilder, target),
			};
		}


	}


}