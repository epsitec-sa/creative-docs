using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout
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


		protected override string GetEditionTilePartType()
		{
			return "horizontalGroup";
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["title"] = this.Title;
			brick["bricks"] = this.Fields.Select (f => f.ToDictionary ()).ToList ();

			return brick;
		}


	}


}
