namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class DecimalField : AbstractField
	{


		public decimal? Value
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "decimalField";
		}


		protected override object GetValue()
		{
			return this.Value;
		}


	}


}
