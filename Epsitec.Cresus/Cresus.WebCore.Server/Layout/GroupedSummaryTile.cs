//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents a tile for a template (i.e. multiple elements from a list), with the
	/// auto grouping enabled.
	/// </summary>
	internal sealed class GroupedSummaryTile : AbstractTile
	{
		public bool								IsRoot
		{
			get;
			set;
		}

		public string							SubViewMode
		{
			get;
			set;
		}

		public string							SubViewId
		{
			get;
			set;
		}

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

		public List<GroupedSummaryTileItem>		Items
		{
			get;
			set;
		}

		public IList<ActionItem>				Actions
		{
			get;
			set;
		}


		protected override void FillDictionary(Dictionary<string, object> tile)
		{
			base.FillDictionary (tile);
			
			tile["isRoot"]             = this.IsRoot;
			tile["subViewMode"]        = this.SubViewMode;
			tile["subViewId"]          = this.SubViewId;
			tile["hideRemoveButton"]   = this.HideRemoveButton;
			tile["hideAddButton"]      = this.HideAddButton;
			tile["propertyAccessorId"] = this.PropertyAccessorId;
			tile["items"]              = this.Items.Select (i => i.ToDictionary ()).ToList ();
			tile["actions"]            = this.Actions.Select (a => a.ToDictionary ()).ToList ();
		}
		
		protected override string GetTileType()
		{
			return "groupedSummary";
		}
	}
}
