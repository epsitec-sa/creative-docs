//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Common.Types.Converters.Marshalers
{
	public sealed class NullableMarshaler<T> : GenericMarshaler<T?, T>
			where T : struct
	{
		public NullableMarshaler()
		{
		}

		public NullableMarshaler(System.Func<T?> getter, System.Action<T?> setter, Expression expression)
		{
			this.ValueGetter = getter;
			this.ValueSetter = setter;
			this.InitialValue = getter ();
			this.ValueGetterExpression = expression;
		}

		public override string GetStringValue()
		{
			var value = this.GetValue ();
			
			if (value.HasValue)
			{
				return this.Converter.ConvertToString (value.Value);
			}
			else
			{
				return null;
			}
		}

		public override void SetStringValue(string text)
		{
			if (text.IsNullOrWhiteSpace ())
			{
				this.SetValue (null);
				return;
			}

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
			return text.IsNullOrWhiteSpace () || this.Converter.CanConvertFromString (text);
		}
	}
}
