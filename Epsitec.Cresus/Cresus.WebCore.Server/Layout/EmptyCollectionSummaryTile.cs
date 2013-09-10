//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents the tile that is diplayed for a template (i.e. an entity list) which
	/// is empty.
	/// </summary>
	internal sealed class EmptyCollectionSummaryTile : CollectionSummaryTile
	{
		protected override string GetTileType()
		{
			return "emptyCollectionSummary";
		}
	}
}
