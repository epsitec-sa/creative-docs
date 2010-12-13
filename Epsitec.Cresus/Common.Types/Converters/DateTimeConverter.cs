//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>DateTimeConverter</c> is used to convert <see cref="DateTimeType"/> to/from
	/// <c>string</c>.
	/// </summary>
	public class DateTimeConverter : GenericConverter<System.DateTime>
	{
		public override string ConvertToString(System.DateTime date)
		{
			return date == null ? null : date.ToString ();
		}

		public override ConversionResult<System.DateTime> ConvertFromString(string text)
		{
			if (text.IsNullOrWhiteSpace ())
            {
				return new ConversionResult<System.DateTime>
				{
					IsNull = true
				};
            }
			
			System.DateTime result;

			if (System.DateTime.TryParse (text, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result))
			{
				return new ConversionResult<System.DateTime>
				{
					IsNull = false,
					Value = result,
				};
			}
			else
			{
				return new ConversionResult<System.DateTime>
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
