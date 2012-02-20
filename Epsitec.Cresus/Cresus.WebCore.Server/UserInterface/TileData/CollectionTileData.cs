using Epsitec.Common.Support.EntityEngine;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class CollectionTileData : AbstractSummaryTileData
	{


		public LambdaExpression Lambda
		{
			get;
			set;
		}


		public Func<AbstractEntity, IEnumerable<AbstractEntity>> CollectionGetter
		{
			get;
			set;
		}


	}


}

