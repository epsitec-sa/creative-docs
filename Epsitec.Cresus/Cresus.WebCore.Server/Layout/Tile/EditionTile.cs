using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EditionTile : AbstractTile
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


		public IList<AbstractEditionTilePart> Bricks
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = new Dictionary<string, object> ();

			tile["type"] = "edition";

			tile["title"] = this.Title;
			tile["icon"] = this.IconClass;
			tile["bricks"] = this.Bricks.SelectMany (i => i.ToDictionaries ()).ToList ();

			tile["entityId"] = this.EntityId;

			return tile;
		}


	}


}
