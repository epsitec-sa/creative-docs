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

		/// <summary>
		/// Finds the type of the matching converter, if any.
		/// </summary>
		/// <typeparam name="T">The type for which a converter is required.</typeparam>
		/// <returns>The type of the matching converter if it exists in the current assembly; otherwise, <c>null</c>.</returns>
		protected static System.Type FindConverterType<T>()
			where T : struct
		{
			var assembly = System.Reflection.Assembly.GetAssembly (typeof (GenericConverter));

			var types = from type in assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract && type.IsSubclassOf (typeof (GenericConverter)) && type.BaseType == typeof (GenericConverter<T>)
						select type;

			return types.FirstOrDefault ();
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

		public ConversionResult<T> GetConversionResult(string text)
		{
			bool canConvert = this.CanConvertFromString (text);
			var  result     = this.ConvertFromString (text);

			return new ConversionResult<T>
			{
				IsInvalid = !canConvert,
				IsNull    = !result.HasValue,
				Value     = result.HasValue ? result.Value : default (T)
			};
		}

		public static readonly GenericConverter<T> Instance;

		static GenericConverter()
		{
			var converterType = GenericConverter.FindConverterType<T> ();

			if (converterType != null)
			{
				GenericConverter<T>.Instance = System.Activator.CreateInstance (converterType) as GenericConverter<T>;
			}
		}
	}
}
