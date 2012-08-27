using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class IntegerField : AbstractField
	{


		public long? Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();

			fieldDictionary["xtype"] = "numberfield";
			fieldDictionary["allowDecimals"] = false;
			fieldDictionary["value"] = this.Value;

			return fieldDictionary;
		}


	}


}
