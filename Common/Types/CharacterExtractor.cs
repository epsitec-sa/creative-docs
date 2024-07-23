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


namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>CharacterExtractor</c> class is used to extract one or more chunks of characters
    /// from a source string (e.g. text/number/text).
    /// </summary>
    public sealed class CharacterExtractor
    {
        public CharacterExtractor(string value)
        {
            this.value = value ?? "";
            this.buffer = new System.Text.StringBuilder(this.value.Length);
            this.index = 0;
        }

        public bool IsEmpty
        {
            get { return this.index >= this.value.Length; }
        }

        public string GetNextText()
        {
            return this.GetNextValue(c => char.IsDigit(c));
        }

        public int? GetNextDigits()
        {
            var result = this.GetNextValue(c => !char.IsDigit(c));

            if (result.Length == 0)
            {
                return null;
            }
            else
            {
                return InvariantConverter.ConvertFromString<int>(result);
            }
        }

        public string GetNextValue(System.Predicate<char> endPredicate)
        {
            int length = this.value.Length;

            while (this.index < length)
            {
                char c = this.value[this.index];

                if (endPredicate(c))
                {
                    break;
                }

                this.buffer.Append(c);
                this.index++;
            }

            string result = this.buffer.ToString();
            this.buffer.Clear();
            return result;
        }

        private readonly string value;
        private readonly System.Text.StringBuilder buffer;
        private int index;
    }
}
