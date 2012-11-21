using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;


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
				Id = this.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				TypeName = layoutBuilder.GetTypeName (entityReferencePropertyAccessor.Type),
				Value = EntityValue.Create (layoutBuilder, target),
				AllowBlank = this.AllowBlank,
			};
		}


	}


}