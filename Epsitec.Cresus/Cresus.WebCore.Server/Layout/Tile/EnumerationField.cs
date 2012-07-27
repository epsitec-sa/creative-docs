using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
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

			fieldDictionary["xtype"] = "epsitec.enumerationfield";
			fieldDictionary["value"] = this.Value;
			fieldDictionary["enumerationName"] = this.TypeName;

			return fieldDictionary;
		}


	}


}

