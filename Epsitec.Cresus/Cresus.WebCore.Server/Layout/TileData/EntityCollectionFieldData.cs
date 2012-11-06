using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class EntityCollectionFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var entityCollectionPropertyAccessor = (EntityCollectionPropertyAccessor) this.PropertyAccessor;
			var targets = entityCollectionPropertyAccessor.GetEntityCollection (entity);

			return new EntityCollectionField ()
			{
				PropertyAccessorId = entityCollectionPropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				TypeName = this.GetTypeName (layoutBuilder, entityCollectionPropertyAccessor.Type),
				Values = targets.Select (t => EntityValue.Create (layoutBuilder, t)).ToList ()
			};
		}


		private string GetTypeName(LayoutBuilder layoutBuilder, Type collectionType)
		{
			var entityType = collectionType.GetGenericArguments ()[0];
			
			return layoutBuilder.GetTypeName (entityType);
		}


	}


}
