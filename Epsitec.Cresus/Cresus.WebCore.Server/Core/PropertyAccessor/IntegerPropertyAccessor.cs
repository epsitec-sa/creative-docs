using Epsitec.Common.Types;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{
	
	
	internal sealed class IntegerPropertyAccessor : AbstractStringPropertyAccessor
	{
		
		
		public IntegerPropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, id)
		{	
		}


		public override PropertyAccessorType PropertyAccessorType
		{
			get
			{
				return PropertyAccessorType.Integer;
			}
		}


		protected override string ConvertValueToText(object value)
		{
			if (value == null)
			{
				return null;
			}

			return InvariantConverter.ToString ((long) value);
		}


		protected override object ConvertTextToValue(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			return long.Parse (text);
		}


	}


}
