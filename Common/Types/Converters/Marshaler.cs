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


using System.Linq.Expressions;

namespace Epsitec.Common.Types.Converters
{
    /// <summary>
    /// The <c>Marshaler</c> class can get/set values represented by strings,
    /// automatically converting between the underlying type and <c>string</c>.
    /// </summary>
    public abstract class Marshaler : IReadOnly
    {
        /// <summary>
        /// Gets a value indicating whether this marshaler operates on a nullable
        /// type.
        /// </summary>
        /// <value><c>true</c> if this marshaler operates on a nullable type; otherwise, <c>false</c>.</value>
        public abstract bool UsesNullableType { get; }

        /// <summary>
        /// Gets the type of the marshaled data (basically, <code>typeof (T)</code>).
        /// </summary>
        /// <value>The type of the marshaled data.</value>
        public abstract System.Type MarshaledType { get; }

        /// <summary>
        /// Gets the lambda expression for the value getter (if any).
        /// </summary>
        /// <value>The value getter expression.</value>
        public Expression ValueGetterExpression { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the marshaled type is of type <see cref="FormattedText"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the marshaled type is of type <see cref="FormattedText"/>; otherwise, <c>false</c>.
        /// </value>
        public bool IsMarshaledTypeFormattedText
        {
            get { return this.MarshaledType == typeof(FormattedText); }
        }

        /// <summary>
        /// Gets the associated converter. This is implemented in <see cref="GenericMarshaler&lt;T1, T2&gt;"/>.
        /// </summary>
        /// <returns>The associated converter or <c>null</c>.</returns>
        public virtual GenericConverter GetConverter()
        {
            return null;
        }

        /// <summary>
        /// Customizes the converter: this will create a local copy of the converter, so
        /// that it may be customized by setting specific parameters. This is implemented
        /// in <see cref="GenericMarshaler&lt;T1, T2&gt;"/>.
        /// </summary>
        public virtual void CustomizeConverter() { }

        /// <summary>
        /// Creates a marshaler compatible with the specified getter and setter.
        /// </summary>
        /// <typeparam name="T1">The type of the data source.</typeparam>
        /// <typeparam name="T2">The type of the marshaled data.</typeparam>
        /// <param name="source">The data source.</param>
        /// <param name="getterExpression">The getter expression.</param>
        /// <param name="setter">The setter.</param>
        /// <returns>
        /// The <see cref="Marshaler"/> for the underlying type.
        /// </returns>
        public static Marshaler<T2> Create<T1, T2>(
            T1 source,
            Expression<System.Func<T1, T2>> getterExpression,
            System.Action<T1, T2> setter = null
        )
        {
            var getter = getterExpression.Compile();

            return Marshaler.CreateInternal(
                () => getter(source),
                x => setter(source, x),
                getterExpression
            );
        }

        /// <summary>
        /// Creates a marshaler compatible with the specified getter and setter.
        /// </summary>
        /// <typeparam name="T1">The type of the data source.</typeparam>
        /// <typeparam name="T2">The type of the marshaled data.</typeparam>
        /// <param name="source">The data source.</param>
        /// <param name="getterExpression">The getter expression.</param>
        /// <param name="setter">The setter.</param>
        /// <returns>
        /// The <see cref="Marshaler"/> for the underlying type.
        /// </returns>
        public static Marshaler<T2?> Create<T1, T2>(
            T1 source,
            Expression<System.Func<T1, T2?>> getterExpression,
            System.Action<T1, T2?> setter = null
        )
            where T2 : struct
        {
            var getter = getterExpression.Compile();

            return Marshaler.CreateInternal(
                () => getter(source),
                x => setter(source, x),
                getterExpression
            );
        }

        /// <summary>
        /// Creates a marshaler compatible with the specified getter and setter.
        /// </summary>
        /// <typeparam name="T">The underlying type.</typeparam>
        /// <param name="getterExpression">The getter expression.</param>
        /// <param name="setter">The setter.</param>
        /// <returns>
        /// The <see cref="Marshaler"/> for the underlying type.
        /// </returns>
        public static Marshaler<T> Create<T>(
            Expression<System.Func<T>> getterExpression,
            System.Action<T> setter = null
        )
        {
            var getter = getterExpression.Compile();

            return Marshaler.CreateInternal(getter, setter, getterExpression);
        }

        /// <summary>
        /// Creates a marshaler compatible with the specified getter and setter.
        /// </summary>
        /// <typeparam name="T">The underlying type.</typeparam>
        /// <param name="getterExpression">The getter expression.</param>
        /// <param name="setter">The setter.</param>
        /// <returns>
        /// The <see cref="Marshaler"/> for the underlying type.
        /// </returns>
        public static Marshaler<T?> Create<T>(
            Expression<System.Func<T?>> getterExpression,
            System.Action<T?> setter = null
        )
            where T : struct
        {
            var getter = getterExpression.Compile();

            return Marshaler.CreateInternal(getter, setter, getterExpression);
        }

        /// <summary>
        /// Gets the formatted text value. If the underlying type is a <see cref="FormattedText"/>,
        /// this returns the source text; otherwise, it produces an escaped version of the string
        /// value returned by method <see cref="GetStringValue"/>.
        /// </summary>
        /// <returns>The formatted text value.</returns>
        public FormattedText GetFormattedTextValue()
        {
            var str = this.GetStringValue();

            if (this.IsMarshaledTypeFormattedText)
            {
                return new FormattedText(str);
            }
            else
            {
                return FormattedText.FromSimpleText(str);
            }
        }

        /// <summary>
        /// Sets the formatted text value. If the underlying type is a <see cref="FormattedText"/>,
        /// this assigns the source text; otherwise, it produces an simplified version of the string
        /// value and calls method <see cref="SetStringValue"/>.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SetFormattedTextValue(FormattedText text)
        {
            if (this.IsMarshaledTypeFormattedText)
            {
                this.SetStringValue(text.ToString());
            }
            else
            {
                this.SetStringValue(text.ToSimpleText());
            }
        }

        /// <summary>
        /// Gets the string value. Invokes the getter and converts the result to <c>string</c>.
        /// </summary>
        /// <returns>The string value.</returns>
        public abstract string GetStringValue();

        /// <summary>
        /// Sets the string value. Converts the value from <c>string</c> and invokes the setter.
        /// </summary>
        /// <param name="text">The text.</param>
        public abstract void SetStringValue(string text);

        /// <summary>
        /// Determines whether the specified text can be converted to the underlying
        /// type.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text can be converted; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanConvert(string text);

        /// <summary>
        /// Gets the value using the internal getter.
        /// </summary>
        /// <typeparam name="T">The type of the marshaled data.</typeparam>
        /// <returns>The value.</returns>
        public T GetValue<T>()
        {
            object value = this.GetObjectValue();
            return (value is T) ? (T)value : default(T);
        }

        private static Marshaler<T> CreateInternal<T>(
            System.Func<T> getter,
            System.Action<T> setter,
            Expression expression
        )
        {
            return new Marshalers.NonNullableMarshaler<T>
            {
                ValueGetter = getter,
                ValueSetter = setter,
                InitialValue = getter(),
                ValueGetterExpression = expression,
            };
        }

        private static Marshaler<T?> CreateInternal<T>(
            System.Func<T?> getter,
            System.Action<T?> setter,
            Expression expression
        )
            where T : struct
        {
            return new Marshalers.NullableMarshaler<T>
            {
                ValueGetter = getter,
                ValueSetter = setter,
                InitialValue = getter(),
                ValueGetterExpression = expression,
            };
        }

        protected abstract object GetObjectValue();

        #region IReadOnly Members

        public abstract bool IsReadOnly { get; }

        #endregion
    }
}
