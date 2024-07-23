/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.Types.Converters
{
    /// <summary>
    /// The <c>DateConverter</c> is used to convert <see cref="Date"/> to/from
    /// <c>string</c> using the current culture.
    /// </summary>
    public class DateConverter : GenericConverter<Date, DateConverter>
    {
        public DateConverter()
            : this(null)
        {
            //	Keep the constructor with no argument -- it is required by the conversion
            //	framework. We cannot collapse both constructors to a single one with a
            //	default culture set to null, since this won't produce the parameterless
            //	constructor.
        }

        public DateConverter(System.Globalization.CultureInfo culture)
            : base(culture) { }

        public override string ConvertToString(Date date)
        {
            if (date == Date.Null)
            {
                return null;
            }
            else
            {
                return date.ToDateTime().ToString("d", this.GetCurrentCulture());
            }
        }

        public override ConversionResult<Date> ConvertFromString(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return new ConversionResult<Date> { IsNull = true };
            }

            System.DateTime result;

            if (
                System.DateTime.TryParse(
                    text,
                    this.GetCurrentCulture(),
                    System.Globalization.DateTimeStyles.AssumeLocal
                        | System.Globalization.DateTimeStyles.AllowWhiteSpaces,
                    out result
                )
            )
            {
                return new ConversionResult<Date> { IsNull = false, Value = new Date(result) };
            }
            else
            {
                return new ConversionResult<Date> { IsInvalid = true, };
            }
        }

        public override bool CanConvertFromString(string text)
        {
            System.DateTime result;

            if (
                System.DateTime.TryParse(
                    text,
                    this.GetCurrentCulture(),
                    System.Globalization.DateTimeStyles.AssumeLocal
                        | System.Globalization.DateTimeStyles.AllowWhiteSpaces,
                    out result
                )
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static readonly DateConverter Invariant = new DateConverter(
            System.Globalization.CultureInfo.InvariantCulture
        );
    }
}
