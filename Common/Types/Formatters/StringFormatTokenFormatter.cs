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


namespace Epsitec.Common.Types.Formatters
{
    /// <summary>
    /// The <c>StringFormatTokenFormatter</c> class implements the <c>#string(format)</c>
    /// formatting command, which has the same syntax as the <see cref="string.Format"/>
    /// format string.
    /// </summary>
    public class StringFormatTokenFormatter : IFormatTokenFormatter
    {
        #region IFormatTokenFormatter Members

        /// <summary>
        /// Gets the format token of this formatter.
        /// </summary>
        /// <returns>
        /// The <see cref="FormatToken"/> of this formatter.
        /// </returns>
        public FormatToken GetFormatToken()
        {
            return new ArgumentFormatToken("#string", this.Format);
        }

        #endregion

        private string Format(FormatterHelper helper, string argument)
        {
            var value = helper.FormattingContext.Data;

            if (value == null)
            {
                return null;
            }
            else
            {
                return string.Format(argument, value);
            }
        }
    }
}
