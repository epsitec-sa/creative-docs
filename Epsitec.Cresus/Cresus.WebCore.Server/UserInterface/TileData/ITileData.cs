using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal interface ITileData
	{


		IEnumerable<AbstractTile> ToTiles(AbstractEntity entity, Func<AbstractEntity, string> entityKeyGetter, Func<string, string> iconClassGetter, Func<LambdaExpression, string> lambdaIdGetter, Func<Type, string> typeGetter);


	}


}

