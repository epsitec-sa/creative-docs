using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class TextPropertyAccessor : AbstractStringPropertyAccessor
	{
		
		
		public TextPropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, id)
		{		
		}


		public override PropertyAccessorType PropertyAccessorType
		{
			get
			{
				return PropertyAccessorType.Text;
			}
		}


		protected override string ConvertValueToText(object value)
		{
			return ((string) value) ?? "";
		}


		protected override object ConvertTextToValue(string text)
		{
			return text ?? "";
		}


	}


}
