//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;
using System.Linq;



namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// A tile column is an entity column which is composed by a list of tiles that must be
	/// displayed.
	/// </summary>
	internal class TileColumn : EntityColumn
	{
		public IList<AbstractTile>				Tiles
		{
			get;
			set;
		}


		public override string GetColumnType()
		{
			return "tile";
		}

		public override Dictionary<string, object> ToDictionary(Caches caches)
		{
			var column = base.ToDictionary (caches);

			column["tiles"] = this.Tiles.Select (t => t.ToDictionary ()).ToList ();

			return column;
		}
	}
}
