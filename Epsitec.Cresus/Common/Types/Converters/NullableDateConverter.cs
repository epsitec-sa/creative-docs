//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>NullableDateConverter</c> is used to convert <see cref="Date"/> to/from
	/// <c>string</c> using the current culture.
	/// </summary>
	public class NullableDateConverter : GenericConverter<Date?>
	{
		public NullableDateConverter(System.Globalization.CultureInfo culture = null)
			: base (culture)
		{
		}

		public override string ConvertToString(Date? date)
		{
			if ((date.HasValue) &&
						(date.Value != Date.Null))
			{
				return date.Value.ToDateTime ().ToString ("d", this.GetCurrentCulture ());
			}
			else
			{
				return null;
			}
		}

		public override ConversionResult<Date?> ConvertFromString(string text)
		{
			if (text.IsNullOrWhiteSpace ())
			{
				return new ConversionResult<Date?>
				{
					Value = null,
					IsNull = true
				};
			}

			System.DateTime result;

			if (System.DateTime.TryParse (text, this.GetCurrentCulture (), System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result))
			{
				return new ConversionResult<Date?>
				{
					IsNull = false,
					Value = new Date (result)
				};
			}
			else
			{
				return new ConversionResult<Date?>
				{
					IsInvalid = true,
				};
			}
		}

		public override bool CanConvertFromString(string text)
		{
			System.DateTime result;

			if (string.IsNullOrWhiteSpace (text))
			{
				return true;
			}

			if (System.DateTime.TryParse (text, this.GetCurrentCulture (), System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result))
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
