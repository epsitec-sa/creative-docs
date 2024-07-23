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


namespace Epsitec.Common.Types.Converters
{
    /// <summary>
    /// The <c>StringConverter</c> class does a transparent conversion;
    /// it is required so that the <see cref="Marshaler"/> can work with
    /// <c>string</c> values.
    /// </summary>
    public class StringConverter : GenericConverter<string, StringConverter>
    {
        public StringConverter()
            : base(System.Globalization.CultureInfo.InvariantCulture) { }

        public override string ConvertToString(string text)
        {
            return text;
        }

        public override ConversionResult<string> ConvertFromString(string text)
        {
            if (text == null)
            {
                return new ConversionResult<string> { IsNull = true, };
            }
            else
            {
                return new ConversionResult<string> { Value = text, };
            }
        }

        public override bool CanConvertFromString(string text)
        {
            return true;
        }
    }
}
