using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class DecimalField : AbstractField
	{


		public decimal? Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();

			fieldDictionary["xtype"] = "numberfield";
			fieldDictionary["value"] = this.Value;

			return fieldDictionary;
		}


	}


}
