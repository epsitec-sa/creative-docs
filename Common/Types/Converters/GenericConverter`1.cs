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


namespace Epsitec.Common.Types.Converters
{
    /// <summary>
    /// The <c>GenericConverter</c> class is the base class for specific type converters.
    /// </summary>
    /// <typeparam name="T">The type on which the converter operates.</typeparam>
    public abstract class GenericConverter<T> : GenericConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericConverter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="culture">The culture which must be used; if <c>null</c>, uses the
        /// current thread culture whenever a conversion takes place.</param>
        protected GenericConverter(System.Globalization.CultureInfo culture)
        {
            this.culture = culture;
        }

        public abstract string ConvertToString(T value);

        public abstract ConversionResult<T> ConvertFromString(string text);

        public abstract bool CanConvertFromString(string text);

        public static readonly GenericConverter<T> Instance;

        protected override object ConvertObjectFromString(string value)
        {
            var result = this.ConvertFromString(value);

            if (result.IsInvalid)
            {
                throw new System.FormatException();
            }

            if (result.IsNull)
            {
                return null;
            }
            else
            {
                return result.Value;
            }
        }

        protected override string ConvertObjectToString(object value)
        {
            return this.ConvertToString((T)value);
        }

        protected System.Globalization.CultureInfo GetCurrentCulture()
        {
            return this.culture ?? System.Globalization.CultureInfo.CurrentCulture;
        }

        static GenericConverter()
        {
            var converterType = GenericConverter.FindConverterType<T>();

            if (converterType != null)
            {
                //	We know for sure that converters implement a default constructor. See the
                //	constraint of TSelf to new() on GenericConverter<T, TSelf>.

                GenericConverter<T>.Instance =
                    System.Activator.CreateInstance(converterType) as GenericConverter<T>;
            }
            else if (typeof(T).IsEnum)
            {
                GenericConverter<T>.Instance = new EnumConverter<T>(typeof(T));
            }
        }

        private System.Globalization.CultureInfo culture;
    }
}
