namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This class represents the tile that is diplayed for a template (i.e. an entity list) which
	/// is empty.
	/// </summary>
	internal sealed class EmptySummaryTile : CollectionSummaryTile
	{


		protected override string GetTileType()
		{
			return "emptySummary";
		}

	}


}
