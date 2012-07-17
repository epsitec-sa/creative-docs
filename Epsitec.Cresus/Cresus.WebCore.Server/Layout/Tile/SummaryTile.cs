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
			var panel = new Dictionary<string, object> ();

			panel["xtype"] = "summary";

			if (this.IconClass != null)
			{
				panel["iconCls"] = this.IconClass;
			}

			panel["title"] = this.Title;
			panel["html"] = this.Text;

			panel["isRoot"] = this.IsRoot;
			panel["entityId"] = this.EntityId;
			panel["subViewMode"] = this.SubViewMode;
			panel["subViewId"] = this.SubViewId;

			panel["autoCreatorId"] = this.AutoCreatorId;

			return panel;
		}


	}


}

