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


		protected override string GetEditionTilePartType()
		{
			return "textField";
		}


		protected override object GetValue()
		{
			return this.Value;
		}


	}


}

