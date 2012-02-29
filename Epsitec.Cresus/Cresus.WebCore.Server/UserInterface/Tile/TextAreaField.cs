using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class TextAreaField : AbstractField
	{

		public string Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();

			fieldDictionary["xtype"] = "textareafield";
			fieldDictionary["value"] = this.Value;

			return fieldDictionary;
		}


	}


}