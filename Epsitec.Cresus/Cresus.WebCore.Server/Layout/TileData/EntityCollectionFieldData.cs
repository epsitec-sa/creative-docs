using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class EntityCollectionFieldData : AbstractFieldData
	{


		public Func<AbstractEntity, IEnumerable<AbstractEntity>> ValueGetter
		{
			get;
			set;
		}


		public Type CollectionType
		{
			get;
			set;
		}


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new EntityCollectionField ()
			{
				Id = this.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				TypeName = layoutBuilder.GetTypeName (this.CollectionType),
				Values = this.ValueGetter (entity).Select (t => EntityValue.Create (layoutBuilder, t)).ToList (),
				AllowBlank = this.AllowBlank,
			};
		}


	}


}
