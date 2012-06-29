using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class GlobalWarningData : AbstractEditionTilePartData
	{


		public override AbstractEditionTilePart ToAbstractEditionTilePart(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			return new GlobalWarning ();
		}


	}


}

