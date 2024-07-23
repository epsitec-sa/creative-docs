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
    public class LongConverter : GenericConverter<long, LongConverter>
    {
        public LongConverter()
            : this(null)
        {
            //	Keep the constructor with no argument -- it is required by the conversion
            //	framework. We cannot collapse both constructors to a single one with a
            //	default culture set to null, since this won't produce the parameterless
            //	constructor.
        }

        public LongConverter(System.Globalization.CultureInfo culture)
            : base(culture) { }

        public override string ConvertToString(long value)
        {
            return value.ToString(this.GetCurrentCulture());
        }

        public override ConversionResult<long> ConvertFromString(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return new ConversionResult<long> { IsNull = true };
            }

            long result;

            if (
                long.TryParse(
                    text,
                    System.Globalization.NumberStyles.Number,
                    this.GetCurrentCulture(),
                    out result
                )
            )
            {
                return new ConversionResult<long> { IsNull = false, Value = result, };
            }
            else
            {
                return new ConversionResult<long> { IsInvalid = true, };
            }
        }

        public override bool CanConvertFromString(string text)
        {
            long result;

            if (
                long.TryParse(
                    text,
                    System.Globalization.NumberStyles.Number,
                    this.GetCurrentCulture(),
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
    }
}
