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


using Epsitec.Common.Types.Formatters;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>FormatTokenFormatterResolver</c> class is used to find all classes
    /// which implement <see cref="IFormatTokenFormatter"/> and get the collection
    /// of their associated format tokens (see <see cref="FormatToken"/>).
    /// </summary>
    public sealed class FormatTokenFormatterResolver
    {
        /// <summary>
        /// Gets the collection of all the dynamic format tokens.
        /// </summary>
        /// <returns>The collection of all the dynamic format tokens.</returns>
        public static IEnumerable<FormatToken> GetFormatTokens()
        {
            return FormatTokenFormatterResolver.Resolve().Select(x => x.GetFormatToken());
        }

        /// <summary>
        /// Gets the collection of all classes which implement <see cref="IFormatTokenFormatter"/>.
        /// </summary>
        /// <returns>The collection of all classes which implement <see cref="IFormatTokenFormatter"/>.</returns>
        private static IEnumerable<IFormatTokenFormatter> Resolve()
        {
            if (FormatTokenFormatterResolver.formatters == null)
            {
                FormatTokenFormatterResolver.formatters =
                    InterfaceImplementationResolver<IFormatTokenFormatter>
                        .CreateInstances()
                        .ToList();
            }

            return FormatTokenFormatterResolver.formatters;
        }

        [System.ThreadStatic]
        private static List<IFormatTokenFormatter> formatters;
    }
}
