using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
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
			var tile = base.ToDictionary ();

			tile["type"] = "collectionSummary";

			tile["hideRemoveButton"] = this.HideRemoveButton;
			tile["hideAddButton"] = this.HideAddButton;
			tile["propertyAccessorId"] = this.PropertyAccessorId;
			tile["entityType"] = this.EntityType;

			return tile;
		}


	}


}

