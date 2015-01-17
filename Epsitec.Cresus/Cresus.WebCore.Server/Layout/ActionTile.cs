//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents the action tiles, i.e. the layout that must be presented to the user
	/// when he decides to perform an action.
	/// </summary>
	internal sealed class ActionTile : AbstractEntityTile
	{
		public string							Text
		{
			get;
			set;
		}

		public IList<AbstractField>				Fields
		{
			get;
			set;
		}


		protected override string GetTileType()
		{
			return "action";
		}

		protected override void FillDictionary(Dictionary<string, object> tile)
		{
			base.FillDictionary (tile);
			
			tile["text"]   = this.Text;
			tile["fields"] = this.Fields.Select (f => f.ToDictionary ()).ToList ();
		}
	}
}
