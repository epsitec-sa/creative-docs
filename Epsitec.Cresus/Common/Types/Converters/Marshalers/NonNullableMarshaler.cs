//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters.Marshalers
{
	public sealed class NonNullableMarshaler<T> : GenericMarshaler<T, T>
	{
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
