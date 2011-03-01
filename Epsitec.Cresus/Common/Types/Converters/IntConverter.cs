//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>IntConverter</c> class does a transparent conversion;
	/// it is required so that the <see cref="Marshaler"/> can work with
	/// <c>int</c> values.
	/// </summary>
	public class IntConverter : GenericConverter<int>
	{
		public IntConverter(System.Globalization.CultureInfo culture = null)
			: base (culture)
		{
		}

		public override string ConvertToString(int value)
		{
			return value.ToString (this.GetCurrentCulture ());
		}

		public override ConversionResult<int> ConvertFromString(string text)
		{
			if (text.IsNullOrWhiteSpace ())
			{
				return new ConversionResult<int>
				{
					IsNull = true
				};
			}

			int result;

			if (int.TryParse (text, System.Globalization.NumberStyles.Integer, this.GetCurrentCulture (), out result))
			{
				return new ConversionResult<int>
				{
					IsNull = false,
					Value = result,
				};
			}
			else
			{
				return new ConversionResult<int>
				{
					IsInvalid = true,
				};
			}
		}

		public override bool CanConvertFromString(string text)
		{
			int result;

			if (int.TryParse (text, System.Globalization.NumberStyles.Integer, this.GetCurrentCulture (), out result))
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
