using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;

using System.Linq;



namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class TileColumn : EntityColumn
	{


		public IList<AbstractTile> Tiles
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
