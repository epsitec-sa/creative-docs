using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This is the base class for all tiles that might have some actions that can be triggered.
	/// </summary>
	internal abstract class AbstractActionTile : AbstractEntityTile
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
