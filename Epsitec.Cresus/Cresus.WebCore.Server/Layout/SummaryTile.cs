//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represent the tiles that are regular summary tiles, i.e. the tiles that display
	/// some information about an entity.
	/// </summary>
	internal class SummaryTile : AbstractActionTile
	{
		public string							Text
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

		public string							AutoCreatorId
		{
			get;
			set;
		}

		public bool								IsRoot
		{
			get;
			set;
		}


		protected override string GetTileType()
		{
			return "summary";
		}

		protected override void FillDictionary(Dictionary<string, object> tile)
		{
			base.FillDictionary (tile);

			tile["text"]          = this.Text;
			tile["isRoot"]        = this.IsRoot;
			tile["subViewMode"]   = this.SubViewMode;
			tile["subViewId"]     = this.SubViewId;
			tile["autoCreatorId"] = this.AutoCreatorId;
		}
	}
}
