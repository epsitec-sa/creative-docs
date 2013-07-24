using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This is the base class for all tiles that are bound to an entity.
	/// </summary>
	internal abstract class AbstractEntityTile : AbstractTile
	{


		public string EntityId
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = base.ToDictionary ();

			tile["entityId"] = this.EntityId;

			return tile;
		}


	}


}
