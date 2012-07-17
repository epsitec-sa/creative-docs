using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class DateField : AbstractField
	{


		public string Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();

			fieldDictionary["xtype"] = "datefield";
			fieldDictionary["format"] = "d.m.Y";
			fieldDictionary["value"] = this.Value;

			return fieldDictionary;
		}


	}


}

