using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal class SummaryTile : AbstractTile
	{


		public string Type
		{
			get
			{
				return "summary";
			}
		}


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


		public override Dictionary<string, object> ToDictionary()
		{
			var panel = new Dictionary<string, object> ();

			panel["xtype"] = this.Type;

			if (this.IconClass != null)
			{
				panel["iconCls"] = this.IconClass;
			}

			panel["title"] = this.Title;
			panel["html"] = this.Text;

			panel["entityId"] = this.EntityId;
			panel["subViewControllerMode"] = this.SubViewControllerMode;
			panel["subViewControllerSubTypeId"] = this.SubViewControllerSubTypeId;

			return panel;
		}


	}


}

