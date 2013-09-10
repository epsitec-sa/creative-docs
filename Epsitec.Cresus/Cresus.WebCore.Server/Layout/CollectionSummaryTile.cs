//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents the summary tile that is used to display an item of a template (i.e.
	/// an entity list) with the auto grouping disabled.
	/// </summary>
	internal class CollectionSummaryTile : SummaryTile
	{
		public string							PropertyAccessorId
		{
			get;
			set;
		}

		public bool								HideRemoveButton
		{
			get;
			set;
		}

		public bool								HideAddButton
		{
			get;
			set;
		}


		protected override string GetTileType()
		{
			return "collectionSummary";
		}

		protected override void FillDictionary(Dictionary<string, object> tile)
		{
			base.FillDictionary (tile);
			
			tile["hideRemoveButton"]   = this.HideRemoveButton;
			tile["hideAddButton"]      = this.HideAddButton;
			tile["propertyAccessorId"] = this.PropertyAccessorId;
		}
	}
}
