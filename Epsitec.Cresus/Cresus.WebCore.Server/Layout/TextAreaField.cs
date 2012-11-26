using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;


namespace Epsitec.Cresus.WebCore.Server.Layout
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
			return ValueConverter.ConvertFieldToClientForText (this.Value);
		}


	}


}