using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal class SummaryTile : AbstractTile
	{


		public string EntityId
		{
			get;
			set;
		}


		public string IconClass
		{
			get;
			set;
		}


		public string Title
		{
			get;
			set;
		}


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


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = new Dictionary<string, object> ();

			tile["type"] = "summary";

			tile["title"] = this.Title;
			tile["icon"] = this.IconClass;
			tile["text"] = this.Text;

			tile["isRoot"] = this.IsRoot;
			tile["entityId"] = this.EntityId;
			tile["subViewMode"] = this.SubViewMode;
			tile["subViewId"] = this.SubViewId;

			tile["autoCreatorId"] = this.AutoCreatorId;

			return tile;
		}


	}


}

