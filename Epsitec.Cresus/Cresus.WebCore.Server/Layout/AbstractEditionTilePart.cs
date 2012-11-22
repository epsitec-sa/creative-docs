using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal abstract class AbstractEditionTilePart
	{


		protected abstract string GetEditionTilePartType();


		public virtual Dictionary<string, object> ToDictionary()
		{
			var brick = new Dictionary<string, object> ();

			brick["type"] = this.GetEditionTilePartType ();

			return brick;
		}

	}


}
