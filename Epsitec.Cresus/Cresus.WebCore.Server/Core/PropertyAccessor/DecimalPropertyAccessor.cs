using Epsitec.Common.Types;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class DecimalPropertyAccessor : AbstractStringPropertyAccessor
	{
		
		
		public DecimalPropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, id)
		{		
		}


		public override PropertyAccessorType PropertyAccessorType
		{
			get
			{
				return PropertyAccessorType.Decimal;
			}
		}


		protected override string ConvertValueToText(object value)
		{
			if (value == null)
			{
				return null;
			}

			return InvariantConverter.ToString ((decimal) value);
		}


		protected override object ConvertTextToValue(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			return decimal.Parse (text);
		}


	}


}
