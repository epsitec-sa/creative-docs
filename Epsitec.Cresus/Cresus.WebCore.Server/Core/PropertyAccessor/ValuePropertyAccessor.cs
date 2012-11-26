using Epsitec.Common.Types;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class ValuePropertyAccessor : AbstractPropertyAccessor
	{


		public ValuePropertyAccessor(LambdaExpression lambda, FieldType fieldType, string id)
			: base (lambda, fieldType, id)
		{
		}


		public override bool CheckValue(object value)
		{
			if (value == null)
			{
				return this.Property.IsNullable;
			}
			
			var highLevelType = (AbstractType) this.Property.Type;

			return highLevelType.IsValidValue (value);
		}


	}


}
