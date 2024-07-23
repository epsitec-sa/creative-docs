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
    /// The <c>BooleanConverter</c> class does a transparent conversion;
    /// it is required so that the <see cref="Marshaler"/> can work with
    /// <c>bool</c> values.
    /// </summary>
    public class BooleanConverter : GenericConverter<bool, BooleanConverter>
    {
        public BooleanConverter()
            : this(null)
        {
            //	Keep the constructor with no argument -- it is required by the conversion
            //	framework. We cannot collapse both constructors to a single one with a
            //	default culture set to null, since this won't produce the parameterless
            //	constructor.
        }

        public BooleanConverter(System.Globalization.CultureInfo culture)
            : base(culture) { }

        public override string ConvertToString(bool value)
        {
            return value.ToString(this.GetCurrentCulture());
        }

        public override ConversionResult<bool> ConvertFromString(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return new ConversionResult<bool> { IsNull = true };
            }

            bool result;

            if (bool.TryParse(text, out result))
            {
                return new ConversionResult<bool> { IsNull = false, Value = result, };
            }
            else
            {
                return new ConversionResult<bool> { IsInvalid = true, };
            }
        }

        public override bool CanConvertFromString(string text)
        {
            bool result;

            if (bool.TryParse(text, out result))
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
