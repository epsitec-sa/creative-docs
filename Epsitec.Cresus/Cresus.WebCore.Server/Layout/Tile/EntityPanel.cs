using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EntityPanel
	{


		public string EntityId
		{
			get;
			set;
		}


		public string ViewMode
		{
			get;
			set;
		}


		public string ViewId
		{
			get;
			set;
		}


		public IList<AbstractTile> Tiles
		{
			get;
			set;
		}


		public Dictionary<string, object> ToDictionary()
		{
			var panel = new Dictionary<string, object> ();

			panel["entityId"] = this.EntityId;
			panel["viewMode"] = this.ViewMode;
			panel["viewId"] = this.ViewId;
			panel["items"] = this.Tiles.Select (t => t.ToDictionary ()).ToList ();

			return panel;
		}


	}


}
