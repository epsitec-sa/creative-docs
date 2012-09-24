//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Common.Types.Converters.Marshalers
{
	public sealed class NonNullableMarshaler<T> : GenericMarshaler<T, T>
	{
		public NonNullableMarshaler()
		{
		}

		public NonNullableMarshaler(System.Func<T> getter, System.Action<T> setter, Expression expression)
		{
			this.ValueGetter = getter;
			this.ValueSetter = setter;
			this.InitialValue = getter ();
			this.ValueGetterExpression = expression;
		}

		public override string GetStringValue()
		{
			var value = this.GetValue ();
			return this.Converter.ConvertToString (value);
		}

		public override void SetStringValue(string text)
		{
			var value = this.Converter.ConvertFromString (text);

			if (value.HasValue)
			{
				this.SetValue (value.Value);
			}
			else
			{
				this.SetValue (this.InitialValue);
			}
		}

		public override bool CanConvert(string text)
		{
			return this.Converter.CanConvertFromString (text);
		}
	}
}
