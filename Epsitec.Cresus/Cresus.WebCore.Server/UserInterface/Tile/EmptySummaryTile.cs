using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class EmptySummaryTile : AbstractTile
	{


		public string Type
		{
			get
			{
				return "emptysummary";
			}
		}


		public string LambdaId
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

			panel["xtype"] = this.Type;
			panel["lambdaId"] = this.LambdaId;
			panel["entityType"] = this.EntityType;

			return panel;
		}

	}


}

