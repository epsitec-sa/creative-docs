using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EnumerationField : AbstractField
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


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["type"] = "enumerationField";
			brick["value"] = this.Value;
			brick["enumerationName"] = this.TypeName;

			return brick;
		}


	}


}

