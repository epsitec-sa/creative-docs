using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This is the base class of all the parts that can be contained in an edition tile, like
	/// fields, separators, etc.
	/// </summary>
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
