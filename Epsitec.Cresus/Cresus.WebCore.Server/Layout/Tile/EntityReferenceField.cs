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
			var fieldDictionary = base.ToDictionary ();

			fieldDictionary["value"] = this.Value.ToDictionary ();
			fieldDictionary["xtype"] = "epsitec.entityreferencefield";
			fieldDictionary["entityName"] = this.TypeName;

			return fieldDictionary;
		}


	}


}