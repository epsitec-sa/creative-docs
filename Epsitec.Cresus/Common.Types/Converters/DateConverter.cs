//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>DateConverter</c> is used to convert <see cref="Date"/> to/from
	/// <c>string</c>.
	/// </summary>
	public class DateConverter : GenericConverter<Date>
	{
		public override string ConvertToString(Date date)
		{
			return date == Date.Null ? null : date.ToDateTime ().ToShortDateString ();
		}

		public override ConversionResult<Date> ConvertFromString(string text)
		{
			if (text.IsNullOrWhiteSpace ())
            {
				return new ConversionResult<Date>
				{
					IsNull = true
				};
            }
			
			System.DateTime result;

			if (System.DateTime.TryParse (text, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result))
			{
				return new ConversionResult<Date>
				{
					IsNull = false,
					Value = new Date (result)
				};
			}
			else
			{
				return new ConversionResult<Date>
				{
					IsInvalid = true,
				};
			}
		}

		public override bool CanConvertFromString(string text)
		{
			System.DateTime result;

			if (System.DateTime.TryParse (text, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result))
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
