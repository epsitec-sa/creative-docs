using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class IntegerField : AbstractField
	{


		public long? Value
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "integerField";
		}


		protected override object GetValue()
		{
			return ValueConverter.ConvertFieldToClientForInteger (this.Value);
		}


	}


}
