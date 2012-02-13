using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal class Separator : AbstractEditionTilePart
	{


		public override IEnumerable<Dictionary<string, object>> ToDictionary()
		{
			var item = new Dictionary<string, object> ();

			item["xtype"] = "box";
			item["margin"] = "10 0";
			item["autoEl"] = new Dictionary<string, object> () { { "tag", "hr" } };

			yield return item;
		}


	}


}
