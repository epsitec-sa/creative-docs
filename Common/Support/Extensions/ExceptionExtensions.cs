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


using System.Collections.Generic;

namespace Epsitec.Common.Support.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Gets the full text for the exception (and possibly for the inner exceptions too).
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The full text.</returns>
        public static string GetFullText(this System.Exception exception)
        {
            var set = new HashSet<System.Exception>();
            var buffer = new System.Text.StringBuilder();
            var title = "Exception:";

            for (var e = exception; e != null; e = e.InnerException)
            {
                if (set.Add(e))
                {
                    ExceptionExtensions.DumpException(buffer, "", title, e, set);
                }
                title = "Inner exception:";
            }

            return buffer.ToString();
        }

        private static void DumpException(
            System.Text.StringBuilder buffer,
            string prefix,
            string title,
            System.Exception exception,
            HashSet<System.Exception> set
        )
        {
            var format =
                "{0}\n"
                + "  Type: {1}\n"
                + "  Message: {2}\n"
                + "  Source: {3}\n"
                + "  Stack trace:\n"
                + " {4}";

            var type = exception.GetType();
            var message = exception.Message ?? "<none>";
            var source = exception.Source ?? "<none>";
            var trace =
                exception.StackTrace == null
                    ? "   <none>"
                    : exception.StackTrace.Replace("\n", "\n ");

            var exceptionText =
                prefix
                + string.Format(format, title, type, message, source, trace)
                    .Replace("\n", "\n" + prefix);

            buffer.AppendLine(exceptionText);

            var aggregate = exception as System.AggregateException;

            if (aggregate != null)
            {
                foreach (var ex in aggregate.InnerExceptions)
                {
                    if (set.Add(ex))
                    {
                        ExceptionExtensions.DumpException(
                            buffer,
                            prefix + "  ",
                            "Aggregate inner exception:",
                            ex,
                            set
                        );
                    }
                }
            }
        }
    }
}
