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
using System.Collections.Generic;

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>TextFormatterConverterResolver</c> class is used to find a pretty printer
    /// (<see cref=" ITextFormatterConverter"/>) for a given data type.
    /// </summary>
    public sealed class TextFormatterConverterResolver
    {
        /// <summary>
        /// Resolves the pretty printer for the specified data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>A pretty printer or <c>null</c>.</returns>
        public static ITextFormatterConverter Resolve(System.Type dataType)
        {
            if (TextFormatterConverterResolver.converters == null)
            {
                TextFormatterConverterResolver.Setup();
            }

            ITextFormatterConverter prettyPrinter;

            if (TextFormatterConverterResolver.converters.TryGetValue(dataType, out prettyPrinter))
            {
                return prettyPrinter;
            }
            else
            {
                return null;
            }
        }

        private static void Setup()
        {
            TextFormatterConverterResolver.converters =
                new Dictionary<System.Type, ITextFormatterConverter>();

            foreach (
                var item in InterfaceImplementationResolver<ITextFormatterConverter>.CreateInstances()
            )
            {
                foreach (var convertibleType in item.GetConvertibleTypes())
                {
                    TextFormatterConverterResolver.converters[convertibleType] = item;
                }
            }
        }

        [System.ThreadStatic]
        private static Dictionary<System.Type, ITextFormatterConverter> converters;
    }
}
