using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Layout.TileData;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class BooleanPropertyAccessor : AbstractStringPropertyAccessor
	{


		public BooleanPropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, id)
		{		
		}


		public override PropertyAccessorType PropertyAccessorType
		{
			get
			{
				return PropertyAccessorType.Boolean;
			}
		}


		protected override string ConvertValueToText(object value)
		{
			if (value == null)
			{
				return null;
			}

			return InvariantConverter.ToString ((bool) value);
		}


		protected override object ConvertTextToValue(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			return bool.Parse (text);
		}


	}


}
