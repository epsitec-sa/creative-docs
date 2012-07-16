using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class Panel
	{


		public string EntityId
		{
			get;
			set;
		}


		public string ViewControllerMode
		{
			get;
			set;
		}


		public string ControllerSubTypeId
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

			panel["parentEntity"] = this.EntityId;
			panel["controllerMode"] = this.ViewControllerMode;
			panel["controllerSubTypeId"] = this.ControllerSubTypeId;
			panel["items"] = this.Tiles.Select (t => t.ToDictionary ()).ToList ();

			return panel;
		}


	}


}
