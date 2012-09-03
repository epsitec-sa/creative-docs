using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal class HorizontalGroup : AbstractEditionTilePart
	{


		public string Title
		{
			get;
			set;
		}


		public IList<AbstractField> Fields
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = new Dictionary<string, object> ();

			brick["type"] = "horizontalGroup";
			brick["title"] = this.Title;
			brick["bricks"] = this.Fields.Select (f => f.ToDictionary ()).ToList ();

			return brick;
		}


	}


}
