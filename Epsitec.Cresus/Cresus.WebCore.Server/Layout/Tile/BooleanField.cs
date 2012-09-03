using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class BooleanField : AbstractField
	{


		public bool? Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["type"] = "booleanField";
			brick["value"] = this.Value;

			return brick;
		}

		
	}


}