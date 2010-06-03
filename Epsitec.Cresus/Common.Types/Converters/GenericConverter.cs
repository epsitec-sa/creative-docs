//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>GenericConverter</c> class is the base class for every type converter
	/// in the <c>Epsitec.Common.Types.Converters</c> namespace.
	/// </summary>
	public class GenericConverter
	{
		/// <summary>
		/// Gets the converter for the specified type.
		/// </summary>
		/// <typeparam name="T">The type for which a converter is required.</typeparam>
		/// <returns>The matching converter.</returns>
		/// <exception cref="System.InvalidOperationException">Throws an invalid operation exception if there is not converter for the specified type.</exception>
		public static GenericConverter<T> GetConverter<T>()
			where T : struct
		{
			var converter = GenericConverter<T>.Instance;

			if (converter == null)
			{
				throw new System.InvalidOperationException ("No converter found for type " + typeof (T).Name);
			}

			return converter;
		}
	}

	/// <summary>
	/// The <c>GenericConverter</c> class is the base class for specific type converters.
	/// </summary>
	/// <typeparam name="T">The type on which the converter operates.</typeparam>
	public abstract class GenericConverter<T> : GenericConverter
		where T : struct
	{
		public string ConvertToString(T? value)
		{
			return value.HasValue ? this.ConvertToString (value.Value) : null;
		}

		public abstract string ConvertToString(T value);

		public abstract T? ConvertFromString(string text);

		public abstract bool CanConvertFromString(string text);

		public static readonly GenericConverter<T> Instance;

		static GenericConverter()
		{
			var assembly = System.Reflection.Assembly.GetAssembly (typeof (GenericConverter));

			var types = from type in assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract && type.IsSubclassOf (typeof (GenericConverter))
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.GetGenericArguments ()[0] == typeof (T)
						select type;

			var converterType = types.FirstOrDefault ();

			if (converterType != null)
			{
				GenericConverter<T>.Instance = System.Activator.CreateInstance (converterType) as GenericConverter<T>;
			}
		}
	}
}
