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
	public abstract class GenericConverter : ISerializationConverter
	{
		/// <summary>
		/// Gets the converter for the specified type.
		/// </summary>
		/// <typeparam name="T">The type for which a converter is required.</typeparam>
		/// <returns>The matching converter.</returns>
		/// <exception cref="System.InvalidOperationException">Throws an invalid operation exception if there is not converter for the specified type.</exception>
		public static GenericConverter<T> GetConverter<T>()
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
		{
			var assembly = System.Reflection.Assembly.GetAssembly (typeof (GenericConverter));

			var types = from type in assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract && type.IsSubclassOf (typeof (GenericConverter)) && type.BaseType.BaseType == typeof (GenericConverter<T>)
						select type;

			return types.FirstOrDefault ();
		}

		#region ISerializationConverter Members

		string ISerializationConverter.ConvertToString(object value, IContextResolver context)
		{
			return this.ConvertObjectToString (value);
		}

		object ISerializationConverter.ConvertFromString(string value, IContextResolver context)
		{
			return this.ConvertObjectFromString (value);
		}

		#endregion
		
		protected abstract string ConvertObjectToString(object value);

		protected abstract object ConvertObjectFromString(string value);
	}
}
