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
    /// The <c>FormatToken</c> class is the base class for <see cref="SimpleFormatToken"/> and
    /// <see cref="ArgumentFormatToken"/>, which implement the formatting details.
    /// </summary>
    public abstract class FormatToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatToken"/> class for
        /// the specified format string.
        /// </summary>
        /// <param name="format">The format.</param>
        protected FormatToken(string format)
        {
            this.format = format;
        }

        /// <summary>
        /// Gets the format string used by this instance.
        /// </summary>
        public string FormatString
        {
            get { return this.format; }
        }

        /// <summary>
        /// Checks whether this format token matches the specified format string.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="format">The format string.</param>
        /// <param name="pos">The position in the format string.</param>
        /// <returns><c>true</c> if the format token matches; otherwise, <c>false</c>.</returns>
        public abstract bool Matches(FormatterHelper formatter, string format, int pos);

        /// <summary>
        /// Outputs the formatted data, as requested by the format string submitted to the
        /// <see cref="Matches"/> method.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="buffer">The buffer where to output the result.</param>
        /// <returns>The length to skip in the format string.</returns>
        public abstract int Format(FormatterHelper formatter, System.Text.StringBuilder buffer);

        protected readonly string format;
    }
}
