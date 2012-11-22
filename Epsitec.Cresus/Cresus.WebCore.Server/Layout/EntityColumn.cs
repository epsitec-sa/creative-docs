using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class EntityColumn
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
			var entityColumn = new Dictionary<string, object> ();

			entityColumn["entityId"] = this.EntityId;
			entityColumn["viewMode"] = this.ViewMode;
			entityColumn["viewId"] = this.ViewId;
			entityColumn["tiles"] = this.Tiles.Select (t => t.ToDictionary ()).ToList ();

			return entityColumn;
		}


	}


}
