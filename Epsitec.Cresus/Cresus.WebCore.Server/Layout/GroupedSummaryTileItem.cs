using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This class represents an item in a grouped summary tile, i.e. the summary of an entity that
	/// is contained within the list.
	/// </summary>
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
