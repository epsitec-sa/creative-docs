//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This is the base class for all kind of tiles.
	/// </summary>
	internal abstract class AbstractTile
	{
		public string							Title
		{
			get;
			set;
		}

		public string							IconClass
		{
			get;
			set;
		}


		public Dictionary<string, object> ToDictionary()
		{
			var tile = new Dictionary<string, object> ();

			this.FillDictionary (tile);

			return tile;
		}

		
		protected abstract string GetTileType();

		protected virtual void FillDictionary(Dictionary<string, object> tile)
		{
			tile["type"] = this.GetTileType ();
			tile["title"] = this.Title;
			tile["icon"] = this.IconClass;
		}
	}
}
