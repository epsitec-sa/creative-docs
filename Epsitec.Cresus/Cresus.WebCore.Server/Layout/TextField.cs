using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class TextField : AbstractField
	{


		public bool IsPassword
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "textField";
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["isPassword"] = this.IsPassword;

			return brick;
		}


	}


}

