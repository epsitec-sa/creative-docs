using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EmptySummaryTile : AbstractTile
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


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = new Dictionary<string, object> ();

			tile["type"] = "emptySummary";

			tile["propertyAccessorId"] = this.PropertyAccessorId;
			tile["entityType"] = this.EntityType;

			return tile;
		}

	}


}

