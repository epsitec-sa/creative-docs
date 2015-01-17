//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This is the base class for all tiles that might have some actions that can be triggered.
	/// </summary>
	internal abstract class AbstractActionTile : AbstractEntityTile
	{
		public IList<ActionItem>				Actions
		{
			get;
			set;
		}


		protected override void FillDictionary(Dictionary<string, object> tile)
		{
			base.FillDictionary (tile);

			tile["actions"] = this.Actions.Select (a => a.ToDictionary ()).ToList ();
		}
	}
}
