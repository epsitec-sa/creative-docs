//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>Marshaler</c> class can get/set values represented by strings,
	/// automatically converting between the underlying type and <c>string</c>.
	/// </summary>
	public abstract class Marshaler
	{
		/// <summary>
		/// Creates a marshaler compatible with the specified getter and setter.
		/// </summary>
		/// <typeparam name="T">The underlying type.</typeparam>
		/// <param name="getter">The getter.</param>
		/// <param name="setter">The setter.</param>
		/// <returns>The <see cref="Marshaler"/> for the underlying type.</returns>
		public static Marshaler Create<T>(System.Func<T> getter, System.Action<T> setter)
			where T : struct
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
		public static Marshaler Create<T>(System.Func<T?> getter, System.Action<T?> setter)
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
	}
}