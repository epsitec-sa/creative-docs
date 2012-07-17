using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class CollectionTileData : AbstractSummaryTileData
	{


		public EntityCollectionPropertyAccessor PropertyAccessor
		{
			get;
			set;
		}


		public Func<AbstractEntity, IEnumerable<AbstractEntity>> EntitiesGetter
		{
			get;
			set;
		}


	}


}

