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


namespace Epsitec.Common.Types.Converters.Marshalers
{
    /// <summary>
    /// The <c>GenericMarshaler{T1, T2}</c> class is used as a base class by
    /// the <see cref="NonNullableMarshaler{T}"/> and <see cref="NullableMarshaler{T}"/>.
    /// </summary>
    /// <typeparam name="T1">The real type (either <c>T2</c> or <c>T2?</c>).</typeparam>
    /// <typeparam name="T2">The non-nullable type.</typeparam>
    public abstract class GenericMarshaler<T1, T2> : Marshaler<T1>
    {
        /// <summary>
        /// Gets a value indicating whether this marshaler uses a nullable type <c>T1</c>.
        /// </summary>
        /// <value><c>true</c> if type <c>T1</c> is a nullable type; otherwise, <c>false</c>.</value>
        public override bool UsesNullableType
        {
            get { return GenericMarshaler<T1, T2>.usesNullableType; }
        }

        /// <summary>
        /// Gets the converter which can be used to convert between type <c>T2</c>
        /// and <c>string</c> (in both directions).
        /// </summary>
        /// <value>The converter.</value>
        public GenericConverter<T2> Converter
        {
            get { return this.customConverter ?? GenericMarshaler<T1, T2>.converter; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public override bool IsReadOnly
        {
            get { return this.ValueSetter == null; }
        }

        /// <summary>
        /// Gets the value using the value getter.
        /// </summary>
        /// <returns>The value.</returns>
        public override T1 GetValue()
        {
            if (this.ValueGetter == null)
            {
                return default(T1);
            }
            else
            {
                return this.ValueGetter();
            }
        }

        /// <summary>
        /// Sets the value using the value setter.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetValue(T1 value)
        {
            if (this.ValueSetter == null)
            {
                throw new System.InvalidOperationException("Cannot set value without a setter");
            }
            else
            {
                this.ValueSetter(value);
            }
        }

        /// <summary>
        /// Gets the associated converter.
        /// </summary>
        /// <returns>
        /// The associated converter.
        /// </returns>
        public override GenericConverter GetConverter()
        {
            return this.Converter;
        }

        /// <summary>
        /// Customizes the converter: this will create a local copy of the converter, so
        /// that it may be customized by setting specific parameters.
        /// </summary>
        public override void CustomizeConverter()
        {
            if (this.customConverter == null)
            {
                this.customConverter =
                    System.Activator.CreateInstance(this.Converter.GetType())
                    as GenericConverter<T2>;
            }
        }

        private static readonly bool usesNullableType =
            typeof(T1).IsGenericType && typeof(T1).FullName.StartsWith("System.Nullable`1");
        private static readonly GenericConverter<T2> converter =
            GenericConverter.GetConverter<T2>();

        private GenericConverter<T2> customConverter;
    }
}
