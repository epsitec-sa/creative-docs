using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class EnumField : AbstractField
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


		protected override Dictionary<string, object> GetFieldDictionary()
		{
			var fieldDictionary = base.GetFieldDictionary ();

			fieldDictionary["xtype"] = "epsitec.enum";
			fieldDictionary["value"] = this.Value;
			fieldDictionary["storeClass"] = this.TypeName;

			return fieldDictionary;
		}


	}


}

