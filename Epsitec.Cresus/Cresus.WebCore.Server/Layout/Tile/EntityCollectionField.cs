using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EntityCollectionField : AbstractField
	{


		public string TypeName
		{
			get;
			set;
		}


		public IList<EntityValue> Values
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["type"] = "entityCollectionField";
			brick["values"] = this.Values.Select (v => v.ToDictionary ()).ToList ();
			brick["entityName"] = this.TypeName;

			return brick;
		}


	}


}

