//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>Marshaler</c> class can get/set values represented by strings,
	/// automatically converting between the underlying type and <c>string</c>.
	/// </summary>
	public abstract class Marshaler
	{
		public static Marshaler<T2> Create<T1, T2>(T1 source, Expression<System.Func<T1, System.Collections.Generic.IList<T2>>> getter, int index)
		{
			var getterFunc = getter.Compile ();
			var marshaler  = Marshaler.Create (() => getterFunc (source)[index], null);

			marshaler.getterExpression = getter;
			marshaler.collectionIndex = index;

			return marshaler;
		}

		public static Marshaler<T2> Create<T1, T2>(T1 source, Expression<System.Func<T1, T2>> getter, System.Action<T1, T2> setter)
		{
			var getterFunc = getter.Compile ();
			var marshaler  = Marshaler.Create (() => getterFunc (source), x => setter (source, x));

			marshaler.getterExpression = getter;

			return marshaler;
		}
		
		public static Marshaler<T2?> Create<T1, T2>(T1 source, Expression<System.Func<T1, T2?>> getter, System.Action<T1, T2?> setter)
			where T2 : struct
		{
			var getterFunc = getter.Compile ();
			var marshaler  = Marshaler.Create (() => getterFunc (source), x => setter (source, x));

			marshaler.getterExpression = getter;
			
			return marshaler;
		}

		/// <summary>
		/// Creates a marshaler compatible with the specified getter and setter.
		/// </summary>
		/// <typeparam name="T">The underlying type.</typeparam>
		/// <param name="getter">The getter.</param>
		/// <param name="setter">The setter.</param>
		/// <returns>The <see cref="Marshaler"/> for the underlying type.</returns>
		public static Marshaler<T> Create<T>(System.Func<T> getter, System.Action<T> setter)
		{
			return new Marshalers.NonNullableMarshaler<T>
			{
				ValueGetter = getter,
				ValueSetter = setter,
				InitialValue = getter (),
			};
		}

		/// <summary>
		/// Creates a marshaler compatible with the specified getter and setter.
		/// </summary>
		/// <typeparam name="T">The underlying type.</typeparam>
		/// <param name="getter">The getter.</param>
		/// <param name="setter">The setter.</param>
		/// <returns>The <see cref="Marshaler"/> for the underlying type.</returns>
		public static Marshaler<T?> Create<T>(System.Func<T?> getter, System.Action<T?> setter)
			where T : struct
		{
			return new Marshalers.NullableMarshaler<T>
			{
				ValueGetter = getter,
				ValueSetter = setter,
				InitialValue = getter (),
			};
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
		
		public T GetValue<T>()
		{
			object value = this.GetObjectValue ();

			if (value is T)
			{
				return (T) value;
			}

			return default (T);
		}

		public Expression GetGetterExpression()
		{
			return this.getterExpression;
		}

		public int GetCollectionIndex()
		{
			return this.collectionIndex;
		}

		protected abstract object GetObjectValue();

		private Expression getterExpression;
		private int collectionIndex = -1;
	}

	public abstract class Marshaler<T> : Marshaler
	{
		public abstract void SetValue(T value);

		public abstract T GetValue();

		protected override object GetObjectValue()
		{
			return this.GetValue ();
		}
	}
}