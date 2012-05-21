using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class EnumerationField : AbstractField
	{


		public string Value
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

			fieldDictionary["xtype"] = "epsitec.enum";
			fieldDictionary["value"] = this.Value;
			fieldDictionary["storeClass"] = this.TypeName;

			return fieldDictionary;
		}


	}


}

