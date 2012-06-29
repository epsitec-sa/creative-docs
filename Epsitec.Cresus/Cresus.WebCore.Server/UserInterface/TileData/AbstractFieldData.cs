using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal abstract class AbstractFieldData : AbstractEditionTilePartData
	{


		// TODO Add PickFromCollection, ReadOnly, Width, Height


		public FormattedText Title
		{
			get;
			set;
		}


		public AbstractPropertyAccessor PropertyAccessor
		{
			get;
			set;
		}


		public bool IsReadOnly
		{
			get;
			set;
		}


		public override sealed AbstractEditionTilePart ToAbstractEditionTilePart(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			return this.ToAbstractField (entity, entityIdGetter, entitiesGetter);
		}


		public abstract AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter);


	}


}

