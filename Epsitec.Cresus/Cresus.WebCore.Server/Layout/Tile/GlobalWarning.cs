using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal class GlobalWarning : AbstractEditionTilePart
	{


		public override Dictionary<string, object> ToDictionary()
		{
			var item = base.ToDictionary ();

			item["xtype"] = "displayfield";
			item["value"] = "<i><b>ATTENTION:</b> Les modifications effectuées ici seront répercutées dans tous les enregistrements.</i>";
			item["cls"] = "global-warning";

			return item;
		}


	}


}
