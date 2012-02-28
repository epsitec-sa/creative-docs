using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class GlobalWarningData : AbstractEditionTilePartData
	{


		public override AbstractEditionTilePart ToAbstractEditionTilePart(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, AbstractPropertyAccessor> propertyAccessorGetter)
		{
			return new GlobalWarning ();
		}


	}


}

