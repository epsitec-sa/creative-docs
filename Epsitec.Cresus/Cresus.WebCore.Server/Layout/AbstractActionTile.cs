using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal abstract class AbstractActionTile : AbstractTile
	{


		public IList<ActionItem> Actions
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = base.ToDictionary ();

			tile["actions"] = this.Actions.Select (a => a.ToDictionary ()).ToList ();

			return tile;
		}


	}


}
