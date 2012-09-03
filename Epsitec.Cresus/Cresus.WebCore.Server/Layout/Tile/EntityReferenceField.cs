using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EntityReferenceField : AbstractField
	{


		public string TypeName
		{
			get;
			set;
		}


		public EntityValue Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["type"] = "entityReferenceField";
			brick["value"] = this.Value.ToDictionary ();
			brick["entityName"] = this.TypeName;

			return brick;
		}


	}


}