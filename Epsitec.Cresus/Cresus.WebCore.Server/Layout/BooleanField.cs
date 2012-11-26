using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

namespace Epsitec.Cresus.WebCore.Server.Layout
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
			return ValueConverter.ConvertFieldToClientForBool (this.Value);
		}

		
	}


}