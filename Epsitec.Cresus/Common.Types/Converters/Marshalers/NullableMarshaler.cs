//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters.Marshalers
{
	public class NullableMarshaler<T> : GenericMarshaler<T?, T>
			where T : struct
	{
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
			if (string.IsNullOrWhiteSpace (text))
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
			return string.IsNullOrWhiteSpace (text) || this.Converter.CanConvertFromString (text);
		}
	}
}
