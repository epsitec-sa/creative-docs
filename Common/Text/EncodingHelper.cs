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

using Epsitec.Common.Support;
using System.Text;

namespace Epsitec.Common.Text
{
    public sealed class EncodingHelper
    {
        public EncodingHelper(Encoding encoding)
        {
            this.encoding = Encoding.GetEncoding(
                encoding.CodePage,
                new EncoderReplacementFallback(EncodingHelper.replacement),
                new DecoderReplacementFallback(EncodingHelper.replacement)
            );
        }

        public string ConvertToEncoding(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var builder = new StringBuilder(value.Length);

            foreach (var c in value)
            {
                var s = c.ToString();

                if (!this.IsWithinEncoding(s))
                {
                    s = StringUtils.RemoveDiacritics(s);
                }

                if (this.IsWithinEncoding(s))
                {
                    builder.Append(s);
                }
            }

            return builder.ToString();
        }

        public bool IsWithinEncoding(string value)
        {
            var convertedBytes = this.encoding.GetBytes(value);
            var convertedValue = this.encoding.GetString(convertedBytes);

            return value == convertedValue;
        }

        private readonly Encoding encoding;

        private static readonly string replacement = "";
    }
}
