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


		protected override Dictionary<string, object> GetFieldDictionary()
		{
			var fieldDictionary = base.GetFieldDictionary ();

			fieldDictionary["xtype"] = "textfield";
			fieldDictionary["value"] = this.Value;

			return fieldDictionary;
		}


	}


}

