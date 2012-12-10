using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


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
