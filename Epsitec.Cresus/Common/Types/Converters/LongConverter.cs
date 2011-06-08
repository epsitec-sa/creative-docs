//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	public class LongConverter : GenericConverter<long, LongConverter>
	{
		public LongConverter()
			: this (null)
		{
			//	Keep the constructor with no argument -- it is required by the conversion
			//	framework. We cannot collapse both constructors to a single one with a
			//	default culture set to null, since this won't produce the parameterless
			//	constructor.
		}

		public LongConverter(System.Globalization.CultureInfo culture)
			: base (culture)
		{
		}

		public override string ConvertToString(long value)
		{
			return value.ToString (this.GetCurrentCulture ());
		}

		public override ConversionResult<long> ConvertFromString(string text)
		{
			if (text.IsNullOrWhiteSpace ())
			{
				return new ConversionResult<long>
				{
					IsNull = true
				};
			}

			long result;

			if (long.TryParse (text, System.Globalization.NumberStyles.Number, this.GetCurrentCulture (), out result))
			{
				return new ConversionResult<long>
				{
					IsNull = false,
					Value = result,
				};
			}
			else
			{
				return new ConversionResult<long>
				{
					IsInvalid = true,
				};
			}
		}

		public override bool CanConvertFromString(string text)
		{
			long result;

			if (long.TryParse (text, System.Globalization.NumberStyles.Number, this.GetCurrentCulture (), out result))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
