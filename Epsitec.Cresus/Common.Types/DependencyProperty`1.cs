//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
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
		public static DependencyProperty Register<TResult>(Expression<System.Func<T, TResult>> expression, DependencyPropertyMetadata metadata = null)
		{
			string expressionName = ExpressionAnalyzer.GetLambdaPropertyInfo (expression).Name;

			System.Type propertyType = typeof (TResult);
			System.Type ownerType    = typeof (T);

			return DependencyProperty.Register (expressionName, propertyType, ownerType, metadata ?? new DependencyPropertyMetadata ());
		}

		/// <summary>
		/// Registers a read/write property.
		/// </summary>
		/// <typeparam name="TResult">The type of the property.</typeparam>
		/// <param name="expression">The expression used to access the property (for instance <c>x =&gt; x.Foo</c> for a property named <c>Foo</c>).</param>
		/// <param name="propertyType">Type of the property (if different from <c>TResult</c>).</param>
		/// <param name="metadata">The optional metadata.</param>
		/// <returns>The dependency property.</returns>
		public static DependencyProperty Register<TResult>(Expression<System.Func<T, TResult>> expression, System.Type propertyType, DependencyPropertyMetadata metadata = null)
		{
			string expressionName = ExpressionAnalyzer.GetLambdaPropertyInfo (expression).Name;
			System.Type ownerType = typeof (T);

			return DependencyProperty.Register (expressionName, propertyType, ownerType, metadata ?? new DependencyPropertyMetadata ());
		}

		/// <summary>
		/// Registers a read-only property.
		/// </summary>
		/// <typeparam name="TResult">The type of the property.</typeparam>
		/// <param name="expression">The expression used to access the property (for instance <c>x =&gt; x.Foo</c> for a property named <c>Foo</c>).</param>
		/// <param name="metadata">The optional metadata.</param>
		/// <returns>The dependency property.</returns>
		public static DependencyProperty RegisterReadOnly<TResult>(Expression<System.Func<T, TResult>> expression, DependencyPropertyMetadata metadata = null)
		{
			string expressionName = ExpressionAnalyzer.GetLambdaPropertyInfo (expression).Name;

			System.Type propertyType = typeof (TResult);
			System.Type ownerType    = typeof (T);

			return DependencyProperty.RegisterReadOnly (expressionName, propertyType, ownerType, metadata ?? new DependencyPropertyMetadata ());
		}

		/// <summary>
		/// Registers an attached property.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="propertyType">The type of the property.</param>
		/// <param name="metadata">The optional metadata.</param>
		/// <returns>The dependency property.</returns>
		public static DependencyProperty RegisterAttached(string name, System.Type propertyType, DependencyPropertyMetadata metadata = null)
		{
			System.Type ownerType    = typeof (T);

			return DependencyProperty.RegisterAttached (name, propertyType, ownerType, metadata ?? new DependencyPropertyMetadata ());
		}
	}
}