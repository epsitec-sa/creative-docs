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


namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>TypedObject</c> structure is used to make any type for which
    /// there is a converter, compatible with the <c>DependencyProperty</c>
    /// serialization.
    /// </summary>
    [System.Serializable]
    [System.ComponentModel.TypeConverter(typeof(TypedObject.Converter))]
    public struct TypedObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedObject"/> structure.
        /// </summary>
        /// <param name="value">The value.</param>
        public TypedObject(object value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value of this typed object.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Converts the value to a string representation.
        /// </summary>
        /// <returns>
        /// A string representation of the value.
        /// </returns>
        public override string ToString()
        {
            if (this.value == null)
            {
                return "<null>";
            }
            else
            {
                System.Type type = this.value.GetType();
                ISerializationConverter converter = InvariantConverter.GetSerializationConverter(
                    type
                );
                string assemblyTypeName = string.Join(
                    ", ",
                    type.AssemblyQualifiedName.Split(
                        new string[] { ", " },
                        System.StringSplitOptions.None
                    ),
                    0,
                    2
                );
                return string.Concat(
                    assemblyTypeName,
                    "/",
                    converter.ConvertToString(this.value, null)
                );
            }
        }

        /// <summary>
        /// Parses the specified value and returns a typed object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The typed object.</returns>
        public static TypedObject Parse(string value)
        {
            if (value == "<null>")
            {
                return new TypedObject(null);
            }
            else
            {
                int pos = value.IndexOf('/');
                string typeName = value.Substring(0, pos);
                string strValue = value.Substring(pos + 1);

                System.Type type = TypeRosetta.GetSystemType(typeName);

                ISerializationConverter converter = InvariantConverter.GetSerializationConverter(
                    type
                );

                if (converter == null)
                {
                    System.Diagnostics.Debug.WriteLine("Could not resolve type " + typeName);
                    return new TypedObject(null);
                }

                return new TypedObject(converter.ConvertFromString(strValue, null));
            }
        }

        #region Converter Class

        public class Converter : AbstractStringConverter
        {
            public override object ParseString(
                string value,
                System.Globalization.CultureInfo culture
            )
            {
                return TypedObject.Parse(value);
            }

            public override string ToString(object value, System.Globalization.CultureInfo culture)
            {
                return ((TypedObject)value).ToString();
            }
        }

        #endregion

        private object value;
    }
}
