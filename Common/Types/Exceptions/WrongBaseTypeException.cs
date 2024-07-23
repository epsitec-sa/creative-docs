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


namespace Epsitec.Common.Types.Exceptions
{
    /// <summary>
    /// The <c>WrongBaseTypeException</c> is thrown when a class does not derive
    /// from <c>DependencyObject</c> and tries to register a <c>DependencyProperty</c>.
    /// </summary>
    public class WrongBaseTypeException
        : System.ApplicationException,
            System.Runtime.Serialization.ISerializable
    {
        public WrongBaseTypeException() { }

        public WrongBaseTypeException(string message)
            : base(message) { }

        public WrongBaseTypeException(System.Type type)
            : this(string.Format("Wrong base type for type {0}", type.FullName)) { }

        public WrongBaseTypeException(string message, System.Exception innerException)
            : base(message, innerException) { }
    }
}
