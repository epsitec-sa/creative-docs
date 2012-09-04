using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal abstract class AbstractTile
	{


		public string EntityId
		{
			get;
			set;
		}


		public string Title
		{
			get;
			set;
		}


		public string IconClass
		{
			get;
			set;
		}


		protected abstract string GetTileType();


		public virtual Dictionary<string, object> ToDictionary()
		{
			var tile = new Dictionary<string, object> ();

			tile["type"] = this.GetTileType ();
			tile["title"] = this.Title;
			tile["icon"] = this.IconClass;
			tile["entityId"] = this.EntityId;

			return tile;
		}


	}


}

