using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal class GlobalWarning : AbstractEditionTilePart
	{


		public override IEnumerable<Dictionary<string, object>> ToDictionary()
		{
			var item = new Dictionary<string, object> ();

			item["xtype"] = "displayfield";
			item["value"] = "<i><b>ATTENTION:</b> Les modifications effectuées ici seront répercutées dans tous les enregistrements.</i>";
			item["cls"] = "global-warning";

			yield return item;
		}


	}


}
