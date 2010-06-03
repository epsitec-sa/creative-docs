﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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

		public override Date? ConvertFromString(string text)
		{
			if (string.IsNullOrWhiteSpace (text))
            {
				return null;
            }
			
			System.DateTime result;

			if (System.DateTime.TryParse (text, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result))
			{
				return new Date (result);
			}
			
			return null;
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
