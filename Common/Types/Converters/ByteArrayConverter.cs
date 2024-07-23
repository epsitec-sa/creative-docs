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
    internal class ByteArrayConverter : GenericConverter<byte[], ByteArrayConverter>
    {
        public ByteArrayConverter()
            : base(System.Globalization.CultureInfo.InvariantCulture) { }

        public override string ConvertToString(byte[] value)
        {
            if (value == null)
            {
                return "N";
            }
            if (value.Length == 0)
            {
                return "0";
            }

            if (ByteArrayConverter.ProbeForUtf8(value))
            {
                try
                {
                    var text = ByteArrayConverter.utf8Encoding.GetString(value);

                    return "U" + text;
                }
                catch
                {
                    //	Invalid UTF-8 stream -- handle the data as plain binary...
                }
            }

            return "B" + System.Convert.ToBase64String(value);
        }

        public override ConversionResult<byte[]> ConvertFromString(string text)
        {
            if (text.Length > 0)
            {
                char code = text[0];
                string value = text.Substring(1);

                switch (code)
                {
                    case 'N':
                        return new ConversionResult<byte[]>() { IsNull = true };

                    case '0':
                        return new ConversionResult<byte[]>() { Value = new byte[0] };

                    case 'B':
                        break;

                    case 'U':
                        return new ConversionResult<byte[]>()
                        {
                            Value = ByteArrayConverter.utf8Encoding.GetBytes(text.Substring(1))
                        };

                    default:
                        return new ConversionResult<byte[]>() { IsInvalid = true };
                }

                try
                {
                    return new ConversionResult<byte[]>()
                    {
                        Value = System.Convert.FromBase64String(value)
                    };
                }
                catch { }
            }

            return new ConversionResult<byte[]>() { IsInvalid = true };
        }

        public override bool CanConvertFromString(string text)
        {
            if (text.Length > 0)
            {
                switch (text[0])
                {
                    case '0':
                    case 'N':
                        return true;

                    case 'U':
                    case 'B':
                        return this.ConvertFromString(text).IsValid;

                    default:
                        break;
                }
            }

            return false;
        }

        private static bool ProbeForUtf8(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char c = (char)value[i];

                if ((c == '\t') || (c == '\n') || (c == '\r'))
                {
                    continue;
                }

                if (c < 0x20)
                {
                    return false;
                }
            }

            return true;
        }

        private static System.Text.UTF8Encoding utf8Encoding = new System.Text.UTF8Encoding(
            encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: true
        );
    }
}
