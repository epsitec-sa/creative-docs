using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal abstract class AbstractEditionTilePartData
	{


		public abstract AbstractEditionTilePart ToAbstractEditionTilePart(PanelBuilder panelBuilder, AbstractEntity entity);


	}


}

