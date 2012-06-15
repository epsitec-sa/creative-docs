using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
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


		public string SubViewControllerMode
		{
			get;
			set;
		}


		public string SubViewControllerSubTypeId
		{
			get;
			set;
		}


		public string AutoCreatorId
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

			panel["entityId"] = this.EntityId;
			panel["subViewControllerMode"] = this.SubViewControllerMode;
			panel["subViewControllerSubTypeId"] = this.SubViewControllerSubTypeId;

			panel["autoCreatorId"] = this.AutoCreatorId;

			return panel;
		}


	}


}

