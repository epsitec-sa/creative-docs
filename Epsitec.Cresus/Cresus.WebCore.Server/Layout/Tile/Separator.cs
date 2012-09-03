using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal class Separator : AbstractEditionTilePart
	{


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = new Dictionary<string, object> ();

			brick["type"] = "separator";

			return brick;
		}


	}


}
