using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This is the base class for all kind of tiles.
	/// </summary>
	internal abstract class AbstractTile
	{


		public string Title
		{
			get;
			set;
		}


		public string IconClass
		{
			get;
			set;
		}


		protected abstract string GetTileType();


		public virtual Dictionary<string, object> ToDictionary()
		{
			var tile = new Dictionary<string, object> ();

			tile["type"] = this.GetTileType ();
			tile["title"] = this.Title;
			tile["icon"] = this.IconClass;

			return tile;
		}


	}


}
