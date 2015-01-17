//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents the edition tiles, i.e. the tiles that have edition fields that the
	/// user can fill and save, in order to update the data of an entity.
	/// </summary>
	internal sealed class EditionTile : AbstractActionTile
	{
		public IList<AbstractEditionTilePart>	Bricks
		{
			get;
			set;
		}


		protected override string GetTileType()
		{
			return "edition";
		}

		protected override void FillDictionary(Dictionary<string, object> tile)
		{
			base.FillDictionary (tile);
			
			tile["bricks"] = this.Bricks.Select (i => i.ToDictionary ()).ToList ();
		}
	}
}
