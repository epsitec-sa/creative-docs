using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This class represents the summary tile that is used to display an item of a template (i.e.
	/// an entity list) with the auto grouping disabled.
	/// </summary>
	internal class CollectionSummaryTile : SummaryTile
	{


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

			return tile;
		}


	}


}
