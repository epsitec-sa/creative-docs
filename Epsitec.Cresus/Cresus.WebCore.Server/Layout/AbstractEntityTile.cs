//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This is the base class for all tiles that are bound to an entity.
	/// </summary>
	internal abstract class AbstractEntityTile : AbstractTile
	{
		public string							EntityId
		{
			get;
			set;
		}


		protected override void FillDictionary(Dictionary<string, object> tile)
		{
			base.FillDictionary (tile);
			
			tile["entityId"] = this.EntityId;
		}
	}
}
