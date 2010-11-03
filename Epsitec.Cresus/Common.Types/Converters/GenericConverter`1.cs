//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>GenericConverter</c> class is the base class for specific type converters.
	/// </summary>
	/// <typeparam name="T">The type on which the converter operates.</typeparam>
	public abstract class GenericConverter<T> : GenericConverter
	{
		public abstract string ConvertToString(T value);

		public abstract ConversionResult<T> ConvertFromString(string text);

		public abstract bool CanConvertFromString(string text);

		public static readonly GenericConverter<T> Instance;

		protected override object ConvertObjectFromString(string value)
		{
			var result = this.ConvertFromString (value);

			if (result.IsInvalid)
			{
				throw new System.FormatException ();
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
			return this.ConvertToString ((T) value);
		}

		static GenericConverter()
		{
			var converterType = GenericConverter.FindConverterType<T> ();

			if (converterType != null)
			{
				GenericConverter<T>.Instance = System.Activator.CreateInstance (converterType) as GenericConverter<T>;
			}
			else if (typeof (T).IsEnum)
			{
				GenericConverter<T>.Instance = new EnumConverter<T> (typeof (T));
			}
		}
	}
}