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

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>InterfaceImplementationTester</c> class provides a single <c>Check</c> method
    /// which can be used to verify very efficiently on run-time if a class implements a
    /// given interface.
    /// </summary>
    /// <typeparam name="T">The type of the class to test.</typeparam>
    /// <typeparam name="TInterface">The type of the interface that <c>T</c> should implement.</typeparam>
    public static class InterfaceImplementationTester<T, TInterface>
    {
        /// <summary>
        /// Checks whether the type <c>T</c> implements interface <c>TInterface</c>.
        /// </summary>
        /// <returns><c>true</c> if type <c>T</c> implements interface <c>TInterface</c>; otherwise, <c>false</c>.</returns>
        public static bool Check()
        {
            if (InterfaceImplementationTester<T, TInterface>.result.HasValue == false)
            {
                InterfaceImplementationTester<T, TInterface>.result =
                    typeof(T).ContainsInterface<TInterface>();
            }

            return InterfaceImplementationTester<T, TInterface>.result.Value;
        }

        [System.ThreadStatic]
        private static bool? result;
    }
}
