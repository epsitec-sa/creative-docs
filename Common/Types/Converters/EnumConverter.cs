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
    public class EnumConverter<T> : GenericConverter<T>
    {
        public EnumConverter(System.Type systemType)
            : base(System.Globalization.CultureInfo.InvariantCulture)
        {
            System.Diagnostics.Debug.Assert(systemType.IsEnum);

            this.enumType = EnumType.GetDefault(systemType);
        }

        public override string ConvertToString(T value)
        {
            return EnumConverter<T>.ConvertToNumericString(value);
        }

        public override ConversionResult<T> ConvertFromString(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return new ConversionResult<T>() { IsNull = true, };
            }
            return new ConversionResult<T>() { Value = InvariantConverter.ToEnum<T>(text) };
        }

        public override bool CanConvertFromString(string text)
        {
            return this.enumType.IsValidValue(text);
        }

        public static string ConvertToNumericString(T value)
        {
            object boxedValue = value;
            return InvariantConverter.ToString(EnumType.ConvertToInt((System.Enum)boxedValue));
        }

        private readonly EnumType enumType;
    }
}
