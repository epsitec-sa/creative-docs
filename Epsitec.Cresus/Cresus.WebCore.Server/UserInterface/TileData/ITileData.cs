using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal interface ITileData
	{


		IEnumerable<AbstractTile> ToTiles(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, string, string> iconClassGetter, Func<LambdaExpression, string> lambdaIdGetter, Func<Type, string> typeGetter, Func<AbstractEntity, IEnumerable<AbstractTile>> editionTileBuilder, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, PanelFieldAccessor> panelFieldAccessorGetter);


	}


}

