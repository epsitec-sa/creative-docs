using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class TextField : AbstractField
	{


		public string Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();

			fieldDictionary["xtype"] = "textfield";
			fieldDictionary["value"] = this.Value;

			return fieldDictionary;
		}


	}


}

