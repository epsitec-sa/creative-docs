using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal abstract class AbstractEditionTilePart
	{


		public virtual IEnumerable<Dictionary<string, object>> ToDictionaries()
		{
			return new Dictionary<string, object>[] { this.ToDictionary () };
		}


		public virtual Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object> ();
		}

	}


}
