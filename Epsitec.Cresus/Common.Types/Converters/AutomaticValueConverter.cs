//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>AutomaticValueConverter</c> converts between the CLR base types.
	/// If the conversion is not possible, returns an <see cref="T:InvalidValue"/>.
	/// </summary>
	public class AutomaticValueConverter : IValueConverter
	{
		private AutomaticValueConverter()
		{
		}
		
		#region IValueConverter Members

		/// <summary>
		/// Converts the specified value to the expected type.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="expectedType">The expected type.</param>
		/// <param name="parameter">The optional parameter (not used).</param>
		/// <param name="culture">The culture used for the conversion.</param>
		/// <returns>The converted value or <see cref="InvalidValue.Instance"/>
		/// if the conversion was not possible.</returns>
		public object Convert(object value, System.Type expectedType, object parameter, System.Globalization.CultureInfo culture)
		{
			System.Type sourceType = value == null ? null : value.GetType ();

			if (expectedType == null)
			{
				throw new System.ArgumentNullException ("expectedType", "Expected type is null");
			}
			if (culture == null)
			{
				throw new System.ArgumentNullException ("culture", "No culture was specified");
			}

			//	If the caller wants the value converted to object, there is nothing
			//	to do (it is already boxed as an object), or if there is no change
			//	between the source and type and expected type, there is nothing to
			//	do, either.
			
			if ((expectedType == typeof (object)) ||
				(expectedType == sourceType))
			{
				return value;
			}

			//	If the caller wants to convert a null value to a reference type,
			//	then nothing needs to be done: null is always null. But converting
			//	null to a value type is not possible.
			
			if (value == null)
			{
				if (expectedType.IsValueType)
				{
					return InvalidValue.Instance;
				}
				else
				{
					return null;
				}
			}

			System.Diagnostics.Debug.Assert (sourceType != null);

			System.TypeCode sourceTypeCode   = System.Type.GetTypeCode (sourceType);
			System.TypeCode expectedTypeCode = System.Type.GetTypeCode (expectedType);

			try
			{
				//	We handle DateTime in a special way, since we do not want the
				//	default .NET converter to be used here :

				if ((sourceTypeCode == System.TypeCode.String) &&
					(expectedTypeCode == System.TypeCode.DateTime) &&
					(culture == System.Globalization.CultureInfo.InvariantCulture))
				{
					return InvariantConverter.ToDateTime (value as string);
				}
				
				if ((sourceTypeCode == System.TypeCode.DateTime) &&
					(expectedTypeCode == System.TypeCode.String) &&
					(culture == System.Globalization.CultureInfo.InvariantCulture))
				{
					return InvariantConverter.ToString (value);
				}

				//	Handle enumerations by first converting the value to a string,
				//	then converting the string to the enumeration :
				
				if (expectedType.IsEnum)
				{
					value = this.Convert (value, typeof (string), null, culture);
					
					if (InvalidValue.IsInvalidValue (value))
					{
						return value;
					}

					System.Enum enumValue;

					if (InvariantConverter.Convert (value, expectedType, out enumValue))
					{
						return enumValue;
					}
					else
					{
						return InvalidValue.Instance;
					}
				}

				if ((sourceTypeCode != System.TypeCode.Object) &&
					(expectedTypeCode != System.TypeCode.Object))
				{
					return System.Convert.ChangeType (value, expectedType, culture);
				}

				return AutomaticValueConverter.ConvertUsingTypeConverter (value, sourceType, expectedType, culture);
			}
			catch (System.InvalidCastException)
			{
				return InvalidValue.Instance;
			}
			catch (System.FormatException)
			{
				return InvalidValue.Instance;
			}
			catch (System.Exception ex)
			{
				if ((ex.InnerException != null) &&
					(ex.InnerException is System.FormatException))
				{
					return InvalidValue.Instance;
				}

				throw;
			}
		}

		private static object ConvertUsingTypeConverter(object value, System.Type sourceType, System.Type expectedType, System.Globalization.CultureInfo culture)
		{
			System.ComponentModel.TypeConverter converter;
			
			converter = System.ComponentModel.TypeDescriptor.GetConverter (sourceType);

			if (converter != null)
			{
				if (converter.CanConvertTo (expectedType))
				{
					return converter.ConvertTo (null, culture, value, expectedType);
				}
			}

			converter = System.ComponentModel.TypeDescriptor.GetConverter (expectedType);

			if (converter != null)
			{
				if (converter.CanConvertFrom (sourceType))
				{
					return converter.ConvertFrom (null, culture, value);
				}
			}

			return InvalidValue.Instance;
		}

		/// <summary>
		/// Converts the specified value back to the expected type.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="expectedType">The expected type.</param>
		/// <param name="parameter">The optional parameter (not used).</param>
		/// <param name="culture">The culture used for the conversion.</param>
		/// <returns>The converted value or <see cref="InvalidValue.Instance"/>
		/// if the conversion was not possible.</returns>
		public object ConvertBack(object value, System.Type expectedType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (expectedType == null)
			{
				throw new System.ArgumentNullException ("expectedType", "Expected type is null");
			}

			System.Type sourceType = value == null ? null : value.GetType ();

			try
			{
				if (sourceType != expectedType)
				{
					if (expectedType.IsEnum)
					{
						System.Diagnostics.Debug.WriteLine ("ConvertBack value " + value.ToString () + " to enum type " + expectedType.Name);

						value = this.ConvertBack (value, typeof (string), null, System.Globalization.CultureInfo.InvariantCulture);

						if (InvalidValue.IsInvalidValue (value))
						{
							return value;
						}

						System.Enum enumValue;

						if (InvariantConverter.Convert (value, expectedType, out enumValue))
						{
							System.Diagnostics.Debug.WriteLine ("ConvertBack succeeded on enum value");
							return enumValue;
						}
					}
				}
				else
				{
					return value;
				}
				
				if (expectedType == typeof (object))
				{
					return value;
				}

				string text = value as string;

				if ((text != null) &&
					(text.Length == 0))
				{
					if ((expectedType == typeof (int)) ||
						(expectedType == typeof (long)) ||
						(expectedType == typeof (short)) ||
						(expectedType == typeof (decimal)) ||
						(expectedType == typeof (double)))
					{
						return InvalidValue.Instance;
					}
				}
				
				if ((text != null) &&
					(expectedType == typeof (System.DateTime)) &&
					(culture == System.Globalization.CultureInfo.InvariantCulture))
				{
					return InvariantConverter.ToDateTime (text);
				}

				if (true)
				{
					return AutomaticValueConverter.ConvertBackClassType (value, expectedType, culture, sourceType);
				}
				else
				{
					return System.Convert.ChangeType (value, expectedType, culture);
				}
			}
			catch (System.InvalidCastException)
			{
				return ConvertBackClassType (value, expectedType, culture, sourceType);
			}
			catch (System.FormatException)
			{
				return InvalidValue.Instance;
			}
		}

		private static object ConvertBackClassType(object value, System.Type expectedType, System.Globalization.CultureInfo culture, System.Type sourceType)
		{
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (expectedType);

			if (converter != null)
			{
				if (converter.CanConvertFrom (sourceType))
				{
					return converter.ConvertFrom (null, culture, value);
				}
			}

			converter = System.ComponentModel.TypeDescriptor.GetConverter (sourceType);

			if (converter != null)
			{
				if (converter.CanConvertTo (expectedType))
				{
					return converter.ConvertTo (null, culture, value, expectedType);
				}
			}

			return InvalidValue.Instance;
		}

		#endregion

		public static readonly AutomaticValueConverter Instance = new AutomaticValueConverter ();
	}
}
