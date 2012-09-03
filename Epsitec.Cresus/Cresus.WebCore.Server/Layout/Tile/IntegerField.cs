using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class IntegerField : AbstractField
	{


		public long? Value
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["type"] = "integerField";
			brick["value"] = this.Value;

			return brick;
		}


	}


}
