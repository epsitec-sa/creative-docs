using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class TextAreaField : AbstractField
	{

		public string Value
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "textAreaField";
		}


		protected override object GetValue()
		{
			return this.Value;
		}


	}


}