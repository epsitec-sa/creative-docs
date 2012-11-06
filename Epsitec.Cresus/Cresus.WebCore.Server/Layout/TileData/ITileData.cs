using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal interface ITileData
	{


		IEnumerable<AbstractTile> ToTiles(LayoutBuilder layoutBuilder, AbstractEntity entity);


	}


}

