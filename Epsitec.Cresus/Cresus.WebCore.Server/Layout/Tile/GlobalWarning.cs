using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal class GlobalWarning : AbstractEditionTilePart
	{


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["type"] = "globalWarning";

			return brick;
		}


	}


}
