using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal class CollectionSummaryTile : SummaryTile
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


		protected override string GetTileType()
		{
			return "collectionSummary";
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = base.ToDictionary ();

			tile["hideRemoveButton"] = this.HideRemoveButton;
			tile["hideAddButton"] = this.HideAddButton;
			tile["propertyAccessorId"] = this.PropertyAccessorId;
			tile["entityType"] = this.EntityType;

			return tile;
		}


	}


}

