using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class DateField : AbstractField
	{


		public string Value
		{
			get;
			set;
		}


		protected override Dictionary<string, object> GetFieldDictionary()
		{
			var fieldDictionary = base.GetFieldDictionary ();

			fieldDictionary["xtype"] = "datefield";
			fieldDictionary["format"] = "d.m.Y";
			fieldDictionary["value"] = this.Value;

			return fieldDictionary;
		}


	}


}

