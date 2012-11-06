using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class SeparatorData : AbstractEditionTilePartData
	{


		public override AbstractEditionTilePart ToAbstractEditionTilePart(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new Separator ();
		}


	}


}

