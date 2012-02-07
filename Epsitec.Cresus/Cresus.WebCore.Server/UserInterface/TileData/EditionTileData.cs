using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class EditionTileData : AbstractEditionData, ITileData
	{


		// TODO Add Icon, Title, CompactTitle, Text, CompactText ?


		public IList<AbstractEditionData> Items
		{
			get
			{
				return this.items;
			}
		}


		#region ITileData Members


		public IEnumerable<AbstractTile> ToTiles(AbstractEntity entity, Func<AbstractEntity, string> entityKeyGetter, Func<string, string> iconClassGetter, Func<LambdaExpression, string> lambdaIdGetter, Func<Type, string> typeGetter)
		{
			throw new NotImplementedException ();
		}


		#endregion


		private readonly IList<AbstractEditionData> items = new List<AbstractEditionData> ();


	}


}

