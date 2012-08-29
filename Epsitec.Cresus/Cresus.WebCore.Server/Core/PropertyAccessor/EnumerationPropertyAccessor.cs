using Epsitec.Cresus.WebCore.Server.Layout.TileData;
using Epsitec.Cresus.WebCore.Server.NancyModules;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{
	
	
	internal sealed class EnumerationPropertyAccessor : AbstractStringPropertyAccessor
	{
		
		
		public EnumerationPropertyAccessor(LambdaExpression lambda, int id)
			: base (lambda, id)
		{			
		}


		public override PropertyAccessorType PropertyAccessorType
		{
			get
			{
				return PropertyAccessorType.Enumeration;
			}
		}


		protected override string ConvertValueToText(object value)
		{
			var val = (string) value;

			if (string.IsNullOrEmpty (val) || val == Constants.KeyForNullValue)
			{
				return null;
			}

			return val;
		}


		protected override object ConvertTextToValue(string text)
		{
			return text ?? Constants.KeyForNullValue;
		}


	}


}
