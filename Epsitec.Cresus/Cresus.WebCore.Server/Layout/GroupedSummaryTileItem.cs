using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class GroupedSummaryTileItem
	{


		public string EntityId
		{
			get;
			set;
		}


		public string Text
		{
			get;
			set;
		}


		public Dictionary<string, object> ToDictionary()
		{
			var item = new Dictionary<string, object> ();

			item["entityId"] = this.EntityId;
			item["text"] = this.Text;

			return item;
		}


	}


}
