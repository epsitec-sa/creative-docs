namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class DateField : AbstractField
	{


		public string Value
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "dateField";
		}


		protected override object GetValue()
		{
			return this.Value;
		}


	}


}

