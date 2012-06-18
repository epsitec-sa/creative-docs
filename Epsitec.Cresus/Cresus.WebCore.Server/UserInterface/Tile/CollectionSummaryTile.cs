using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class CollectionSummaryTile : SummaryTile
	{


		public string PropertyAccessorId
		{
			get;
			set;
		}


		public string EntityType
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


		public override Dictionary<string, object> ToDictionary()
		{
			var panel = base.ToDictionary ();

			panel["xtype"] = "collectionsummary";

			panel["hideRemoveButton"] = this.HideRemoveButton;
			panel["hideAddButton"] = this.HideAddButton;
			panel["propertyAccessorId"] = this.PropertyAccessorId;
			panel["entityType"] = this.EntityType;

			return panel;
		}


	}


}

