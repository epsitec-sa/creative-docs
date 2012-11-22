using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
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


		protected override string GetEditionTilePartType()
		{
			return "enumerationField";
		}


		protected override object GetValue()
		{
			return this.Value;
		}
		
		
		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["enumerationName"] = this.TypeName;

			return brick;
		}


	}


}

