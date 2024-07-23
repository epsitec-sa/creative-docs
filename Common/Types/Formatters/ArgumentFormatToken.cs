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

namespace Epsitec.Common.Types.Formatters
{
    /// <summary>
    /// The <c>ArgumentFormatToken</c> class implements handling of formatting tokens
    /// which take one or more arguments, simply enclosed by a pair of parentheses.
    /// </summary>
    public sealed class ArgumentFormatToken : FormatToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentFormatToken"/> class.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="func">The formatting function, which takes an additional argument.</param>
        public ArgumentFormatToken(string format, System.Func<FormatterHelper, string, string> func)
            : base(format)
        {
            this.func = func;
        }

        /// <summary>
        /// Checks whether this format token matches the specified format string.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="format">The format string.</param>
        /// <param name="pos">The position in the format string.</param>
        /// <returns>
        ///   <c>true</c> if the format token matches; otherwise, <c>false</c>.
        /// </returns>
        public override bool Matches(FormatterHelper formatter, string format, int pos)
        {
            //	Since we need the format and position for the formatting part of this task,
            //	we store them both into the formatter's FormatContext object...

            if (format.ContainsAtPosition(this.format, pos))
            {
                int n = this.format.Length;

                int start = format.IndexOf('(', pos) + 1;
                int length = format.IndexOf(')', start) - start;

                if ((start - 1 == pos + n) && (length >= 0))
                {
                    //	Argument specified within a pair of parentheses, such as "#foo(abc)";
                    //	the format string contains more text after the argument.

                    string arguments = format.Substring(start, length);
                    formatter.FormattingContext.DefineArgs("(" + arguments);
                    return true;
                }

                if (format.ContainsAtPosition(" ", pos + n))
                {
                    //	Argument specified with a space as the separator, such as "#foo abc";
                    //	the format string does not contain any other text after the argument.

                    string arguments = format.Substring(pos + n + 1);
                    formatter.FormattingContext.DefineArgs(" " + arguments);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Outputs the formatted data, as requested by the format string submitted to
        /// the <see cref="Matches"/> method.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="buffer">The buffer where to output the result.</param>
        /// <returns>
        /// The length to skip in the format string.
        /// </returns>
        public override int Format(FormatterHelper formatter, System.Text.StringBuilder buffer)
        {
            string args = formatter.FormattingContext.Args;
            char type = args[0];

            int skipLength;

            switch (type)
            {
                case ' ':
                    skipLength = this.format.Length + args.Length; //	skip "#foo abc"
                    break;

                case '(':
                    skipLength = this.format.Length + args.Length + 1; //	skip "#foo(abc)"
                    break;

                default:
                    throw new System.InvalidOperationException();
            }

            buffer.Append(this.func(formatter, args.Substring(1)));

            return skipLength;
        }

        private readonly System.Func<FormatterHelper, string, string> func;
    }
}
