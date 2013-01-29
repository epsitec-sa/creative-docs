using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout
{
	
	
	internal sealed class GroupedSummaryTile : AbstractTile
	{


		public bool IsRoot
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


		public string PropertyAccessorId
		{
			get;
			set;
		}


		public bool HideRemoveButton
		{
			get;
			set;
		}


		public bool HideAddButton
		{
			get;
			set;
		}


		public List<GroupedSummaryTileItem> Items
		{
			get;
			set;
		}


		public IList<ActionItem> Actions
		{
			get;
			set;
		}


		protected override string GetTileType()
		{
			return "groupedSummary";
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = base.ToDictionary ();

			tile["isRoot"] = this.IsRoot;
			tile["subViewMode"] = this.SubViewMode;
			tile["subViewId"] = this.SubViewId;
			tile["hideRemoveButton"] = this.HideRemoveButton;
			tile["hideAddButton"] = this.HideAddButton;
			tile["propertyAccessorId"] = this.PropertyAccessorId;
			tile["items"] = this.Items.Select (i => i.ToDictionary ()).ToList ();
			tile["actions"] = this.Actions.Select (a => a.ToDictionary ()).ToList ();

			return tile;
		}


	}


}
