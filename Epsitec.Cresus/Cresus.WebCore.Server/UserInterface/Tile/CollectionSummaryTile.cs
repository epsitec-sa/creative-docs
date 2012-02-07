using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class CollectionSummaryTile : SummaryTile
	{


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


		public string HideRemoveButton
		{
			get;
			set;
		}


		public string HideAddButton
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var panel = base.ToDictionary ();

			panel["hideRemoveButton"] = this.HideRemoveButton;
			panel["hideAddButton"] = this.HideAddButton;
			panel["lambdaId"] = this.LambdaId;
			panel["entityType"] = this.EntityType;

			return panel;
		}


	}


}

