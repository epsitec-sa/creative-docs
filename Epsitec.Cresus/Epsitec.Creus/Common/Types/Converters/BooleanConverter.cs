//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>BooleanConverter</c> class does a transparent conversion;
	/// it is required so that the <see cref="Marshaler"/> can work with
	/// <c>bool</c> values.
	/// </summary>
	public class BooleanConverter : GenericConverter<bool, BooleanConverter>
	{
		public BooleanConverter()
			: this (null)
		{
			//	Keep the constructor with no argument -- it is required by the conversion
			//	framework. We cannot collapse both constructors to a single one with a
			//	default culture set to null, since this won't produce the parameterless
			//	constructor.
		}

		public BooleanConverter(System.Globalization.CultureInfo culture)
			: base (culture)
		{
		}

		public override string ConvertToString(bool value)
		{
			return value.ToString (this.GetCurrentCulture ());
		}

		public override ConversionResult<bool> ConvertFromString(string text)
		{
			if (text.IsNullOrWhiteSpace ())
			{
				return new ConversionResult<bool>
				{
					IsNull = true
				};
			}

			bool result;

			if (bool.TryParse (text, out result))
			{
				return new ConversionResult<bool>
				{
					IsNull = false,
					Value = result,
				};
			}
			else
			{
				return new ConversionResult<bool>
				{
					IsInvalid = true,
				};
			}
		}

		public override bool CanConvertFromString(string text)
		{
			bool result;

			if (bool.TryParse (text, out result))
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
