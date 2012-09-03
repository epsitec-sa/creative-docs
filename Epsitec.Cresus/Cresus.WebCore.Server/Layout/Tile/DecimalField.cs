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
			var brick = base.ToDictionary ();

			brick["type"] = "decimalField";
			brick["value"] = this.Value;

			return brick;
		}


	}


}
