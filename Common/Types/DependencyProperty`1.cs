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

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>DependencyProperty{T}</c> class is used to register <see cref="DependencyProperty"/> instances
    /// with a <see cref="DependencyObject"/>.
    /// </summary>
    /// <typeparam name="T">The owner type (i.e. the type of the <see cref="DependencyObject"/> on which this property is defined.</typeparam>
    public static class DependencyProperty<T>
        where T : DependencyObject
    {
        /// <summary>
        /// Registers a read/write property.
        /// </summary>
        /// <typeparam name="TResult">The type of the property.</typeparam>
        /// <param name="expression">The expression used to access the property (for instance <c>x =&gt; x.Foo</c> for a property named <c>Foo</c>).</param>
        /// <param name="metadata">The optional metadata.</param>
        /// <returns>The dependency property.</returns>
        public static DependencyProperty Register<TResult>(
            Expression<System.Func<T, TResult>> expression,
            DependencyPropertyMetadata metadata = null
        )
        {
            string expressionName = ExpressionAnalyzer.GetLambdaPropertyInfo(expression).Name;

            System.Type propertyType = typeof(TResult);
            System.Type ownerType = typeof(T);

            return DependencyProperty.Register(
                expressionName,
                propertyType,
                ownerType,
                metadata ?? new DependencyPropertyMetadata(default(TResult))
            );
        }

        public static DependencyProperty Register<TResult>(
            Expression<System.Func<T, TResult>> expression,
            System.Func<T, TResult> getValueOverrideCallback
        )
        {
            return DependencyProperty<T>.Register<TResult>(
                expression,
                new DependencyPropertyMetadata(
                    default(TResult),
                    x => getValueOverrideCallback(x as T)
                )
            );
        }

        public static DependencyProperty Register<TResult>(
            Expression<System.Func<T, TResult>> expression,
            System.Func<T, TResult> getValueOverrideCallback,
            PropertyInvalidatedCallback<T, TResult> propertyInvalidatedCallback
        )
        {
            System.Action<DependencyObject, object, object> genericPropertyInvalidatedCallback = (
                s,
                a,
                b
            ) => propertyInvalidatedCallback((T)s, (TResult)a, (TResult)b);

            return DependencyProperty<T>.Register<TResult>(
                expression,
                new DependencyPropertyMetadata(
                    default(TResult),
                    x => getValueOverrideCallback(x as T),
                    new PropertyInvalidatedCallback(genericPropertyInvalidatedCallback)
                )
            );
        }

        /// <summary>
        /// Registers a read/write property.
        /// </summary>
        /// <typeparam name="TResult">The type of the property.</typeparam>
        /// <param name="expression">The expression used to access the property (for instance <c>x =&gt; x.Foo</c> for a property named <c>Foo</c>).</param>
        /// <param name="propertyType">Type of the property (if different from <c>TResult</c>).</param>
        /// <param name="metadata">The optional metadata.</param>
        /// <returns>The dependency property.</returns>
        public static DependencyProperty Register<TResult>(
            Expression<System.Func<T, TResult>> expression,
            System.Type propertyType,
            DependencyPropertyMetadata metadata = null
        )
        {
            string expressionName = ExpressionAnalyzer.GetLambdaPropertyInfo(expression).Name;
            System.Type ownerType = typeof(T);

            return DependencyProperty.Register(
                expressionName,
                propertyType,
                ownerType,
                metadata ?? new DependencyPropertyMetadata(default(TResult))
            );
        }

        /// <summary>
        /// Registers a read-only property.
        /// </summary>
        /// <typeparam name="TResult">The type of the property.</typeparam>
        /// <param name="expression">The expression used to access the property (for instance <c>x =&gt; x.Foo</c> for a property named <c>Foo</c>).</param>
        /// <param name="metadata">The optional metadata.</param>
        /// <returns>The dependency property.</returns>
        public static DependencyProperty RegisterReadOnly<TResult>(
            Expression<System.Func<T, TResult>> expression,
            DependencyPropertyMetadata metadata = null
        )
        {
            string expressionName = ExpressionAnalyzer.GetLambdaPropertyInfo(expression).Name;

            System.Type propertyType = typeof(TResult);
            System.Type ownerType = typeof(T);

            return DependencyProperty.RegisterReadOnly(
                expressionName,
                propertyType,
                ownerType,
                metadata ?? new DependencyPropertyMetadata(default(TResult))
            );
        }

        public static DependencyProperty RegisterReadOnly<TResult>(
            Expression<System.Func<T, TResult>> expression,
            System.Func<T, TResult> getValueOverrideCallback
        )
        {
            return DependencyProperty<T>.RegisterReadOnly<TResult>(
                expression,
                new DependencyPropertyMetadata(
                    default(TResult),
                    x => getValueOverrideCallback(x as T)
                )
            );
        }

        /// <summary>
        /// Registers an attached property.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="metadata">The optional metadata.</param>
        /// <returns>
        /// The dependency property.
        /// </returns>
        public static DependencyProperty RegisterAttached<TResult>(
            string name,
            DependencyPropertyMetadata metadata = null
        )
        {
            System.Type ownerType = typeof(T);
            System.Type propertyType = typeof(TResult);

            return DependencyProperty.RegisterAttached(
                name,
                propertyType,
                ownerType,
                metadata ?? new DependencyPropertyMetadata(default(TResult))
            );
        }
    }
}
