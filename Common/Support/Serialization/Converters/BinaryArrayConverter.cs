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


using Epsitec.Common.Support.Serialization.Converters;
using Epsitec.Common.Types;

[assembly: DependencyConverter(typeof(byte[]), Converter = typeof(BinaryArrayConverter))]

namespace Epsitec.Common.Support.Serialization.Converters
{
    public class BinaryArrayConverter : ISerializationConverter
    {
        #region ISerializationConverter Members

        public string ConvertToString(object value, IContextResolver context)
        {
            if (value == null)
            {
                return "<null>";
            }

            return Epsitec.Common.IO.Ascii85.Encode((byte[])value);
        }

        public object ConvertFromString(string value, IContextResolver context)
        {
            if (value == "<null>")
            {
                return null;
            }

            return Epsitec.Common.IO.Ascii85.Decode(value);
        }

        #endregion
    }
}
