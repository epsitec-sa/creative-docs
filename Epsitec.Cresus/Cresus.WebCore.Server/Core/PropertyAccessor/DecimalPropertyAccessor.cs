using Epsitec.Cresus.WebCore.Server.Layout.TileData;

using System.Linq.Expressions;
using Epsitec.Common.Types;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class DecimalPropertyAccessor : AbstractStringPropertyAccessor
	{
		
		
		public DecimalPropertyAccessor(LambdaExpression lambda, int id)
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
