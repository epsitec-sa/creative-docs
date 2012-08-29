using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class BooleanField : AbstractField
	{


		public bool Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();

			fieldDictionary["xtype"] = "checkboxfield";
			fieldDictionary["checked"] = this.Value;
			fieldDictionary["inputValue"] = true;
			fieldDictionary["uncheckedValue"] = false;

			return fieldDictionary;
		}

		
	}


}