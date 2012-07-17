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
			var panel = new Dictionary<string, object> ();

			panel["xtype"] = "emptysummarytile";
			panel["propertyAccessorId"] = this.PropertyAccessorId;
			panel["entityType"] = this.EntityType;

			return panel;
		}

	}


}

