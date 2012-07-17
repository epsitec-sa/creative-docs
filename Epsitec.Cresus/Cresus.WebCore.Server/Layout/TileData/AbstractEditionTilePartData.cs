using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal abstract class AbstractEditionTilePartData
	{


		public abstract AbstractEditionTilePart ToAbstractEditionTilePart(LayoutBuilder layoutBuilder, AbstractEntity entity);


	}


}

