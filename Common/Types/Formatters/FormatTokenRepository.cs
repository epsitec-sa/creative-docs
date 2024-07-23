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
using System.Linq;

namespace Epsitec.Common.Types.Formatters
{
    public static class FormatTokenRepository
    {
        /// <summary>
        /// Gets all known <see cref="FormatToken"/> instances. This is thread-safe.
        /// </summary>
        public static IEnumerable<FormatToken> Items
        {
            get { return FormatTokenRepository.items; }
        }

        /// <summary>
        /// Initializes the <see cref="FormatTokenRepository"/> class and fills the collection
        /// of the sorted <see cref="FormatToken"/> instances.
        /// </summary>
        static FormatTokenRepository()
        {
            //	Tokens will be sorted from longest to shortest ("yyyy", then "yy") so that
            //	they can be evaluated one after the other in order to find the proper match:

            var sortedTokens = FormatterHelper
                .GetTokens()
                .OrderByDescending(x => x.FormatString.Length)
                .ThenBy(x => x.FormatString);

            FormatTokenRepository.items = new List<FormatToken>(sortedTokens);
        }

        private static readonly List<FormatToken> items;
    }
}
