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
    /// La classe AbstractStringConverter fournit les supports de base pour une
    /// conversion de/vers le type String.
    /// </summary>
    public abstract class AbstractStringConverter : System.ComponentModel.TypeConverter
    {
        protected AbstractStringConverter() { }

        public abstract object ParseString(string value, System.Globalization.CultureInfo culture);
        public abstract string ToString(object value, System.Globalization.CultureInfo culture);

        public override bool CanConvertFrom(
            System.ComponentModel.ITypeDescriptorContext context,
            System.Type sourceType
        )
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(
            System.ComponentModel.ITypeDescriptorContext context,
            System.Type destinationType
        )
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(
            System.ComponentModel.ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value
        )
        {
            string text = value as string;

            if (text != null)
            {
                return this.ParseString(text, culture);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
            System.ComponentModel.ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value,
            System.Type destinationType
        )
        {
            if (destinationType == typeof(string))
            {
                return this.ToString(value, culture);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
