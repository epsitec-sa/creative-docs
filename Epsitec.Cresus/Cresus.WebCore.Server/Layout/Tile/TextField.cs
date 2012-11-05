using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class TextField : AbstractField
	{


		public string Value
		{
			get;
			set;
		}


		public bool IsPassword
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "textField";
		}


		protected override object GetValue()
		{
			return this.Value;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["isPassword"] = this.IsPassword;

			return brick;
		}


	}


}

