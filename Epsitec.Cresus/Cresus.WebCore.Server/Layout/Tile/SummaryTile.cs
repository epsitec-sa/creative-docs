using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal class SummaryTile : AbstractTile
	{


		public string Text
		{
			get;
			set;
		}


		public string SubViewMode
		{
			get;
			set;
		}


		public string SubViewId
		{
			get;
			set;
		}


		public string AutoCreatorId
		{
			get;
			set;
		}


		public bool IsRoot
		{
			get;
			set;
		}


		protected override string GetTileType()
		{
			return "summary";
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = base.ToDictionary ();

			tile["text"] = this.Text;
			tile["isRoot"] = this.IsRoot;
			tile["subViewMode"] = this.SubViewMode;
			tile["subViewId"] = this.SubViewId;
			tile["autoCreatorId"] = this.AutoCreatorId;

			return tile;
		}


	}


}

