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


		protected override string GetEditionTilePartType()
		{
			return "booleanField";
		}


		protected override object GetValue()
		{
			return this.Value;
		}

		
	}


}