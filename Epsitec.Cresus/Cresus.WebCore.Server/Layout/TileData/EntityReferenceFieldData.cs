using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class EntityReferenceFieldData : AbstractFieldData
	{


		public Func<AbstractEntity, AbstractEntity> ValueGetter
		{
			get;
			set;
		}


		public Type ReferenceType
		{
			get;
			set;
		}


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new EntityReferenceField ()
			{
				Id = this.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				TypeName = layoutBuilder.GetTypeName (this.ReferenceType),
				Value = EntityValue.Create (layoutBuilder, this.ValueGetter (entity)),
				AllowBlank = this.AllowBlank,
			};
		}


	}


}