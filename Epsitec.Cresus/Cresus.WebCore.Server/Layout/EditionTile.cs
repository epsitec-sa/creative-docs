﻿using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class EditionTile : AbstractActionTile
	{


		public IList<AbstractEditionTilePart> Bricks
		{
			get;
			set;
		}


		protected override string GetTileType()
		{
			return "edition";
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = base.ToDictionary ();

			tile["bricks"] = this.Bricks.Select (i => i.ToDictionary ()).ToList ();

			return tile;
		}


	}


}
