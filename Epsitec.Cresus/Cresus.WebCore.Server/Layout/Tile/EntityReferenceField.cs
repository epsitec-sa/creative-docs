using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EntityReferenceField : AbstractField
	{


		public string DisplayedValue
		{
			get;
			set;
		}


		public string InputValue
		{
			get;
			set;
		}


		public string TypeName
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();

			fieldDictionary["value"] = new Dictionary<string, string> ()
			{
				{ "displayed", this.DisplayedValue },
				{ "submitted", this.InputValue },
			};
			fieldDictionary["xtype"] = "epsitec.entityreferencefield";
			fieldDictionary["entityName"] = this.TypeName;

			return fieldDictionary;
		}


	}


}