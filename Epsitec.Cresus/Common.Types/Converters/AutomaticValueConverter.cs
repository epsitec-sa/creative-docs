//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>AutomaticValueConverter</c> converts between any convertible
	/// types. If the conversion is not possible, the conversion methods
	/// return an <see cref="InvalidValue"/>.
	/// </summary>
	public sealed class AutomaticValueConverter : IValueConverter
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
		/// <returns>The converted value or <see cref="InvalidValue.Value"/>
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
					return InvalidValue.Value;
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
						return InvalidValue.Value;
					}
				}

				if ((sourceTypeCode != System.TypeCode.Object) &&
					(expectedTypeCode != System.TypeCode.Object))
				{
					return System.Convert.ChangeType (value, expectedType, culture);
				}

				//	If the caller provided an empty string as value, it cannot be
				//	converted to any other simple type; don't even try, this saves
				//	us an exception...
				
				if (sourceTypeCode == System.TypeCode.String)
				{
					string text = value as string;

					if (expectedType == typeof (FormattedText))
					{
						return new FormattedText (text);
					}
					
					if (text.Length == 0)
					{
						return InvalidValue.Value;
					}
				}

				return AutomaticValueConverter.ConvertUsingTypeConverter (value, sourceType, expectedType, culture);
			}
			catch (System.InvalidCastException)
			{
				return InvalidValue.Value;
			}
			catch (System.FormatException)
			{
				return InvalidValue.Value;
			}
			catch (System.Exception ex)
			{
				if ((ex.InnerException != null) &&
					(ex.InnerException is System.FormatException))
				{
					return InvalidValue.Value;
				}

				throw;
			}
		}

		/// <summary>
		/// Converts the specified value back to the expected type.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="expectedType">The expected type.</param>
		/// <param name="parameter">The optional parameter (not used).</param>
		/// <param name="culture">The culture used for the conversion.</param>
		/// <returns>The converted value or <see cref="InvalidValue.Value"/>
		/// if the conversion was not possible.</returns>
		public object ConvertBack(object value, System.Type expectedType, object parameter, System.Globalization.CultureInfo culture)
		{
			return this.Convert (value, expectedType, parameter, culture);
		}

		#endregion

		/// <summary>
		/// Converts the value using using a type converter provided by the CLR.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="sourceType">The source type.</param>
		/// <param name="expectedType">The expected type.</param>
		/// <param name="culture">The culture.</param>
		/// <returns>The converted value or <see cref="InvalidValue.Value"/>
		/// if there is no available converter.</returns>
		private static object ConvertUsingTypeConverter(object value, System.Type sourceType, System.Type expectedType, System.Globalization.CultureInfo culture)
		{
			System.ComponentModel.TypeConverter converter;

			//	Try converting using the source type converter first :
			
			converter = System.ComponentModel.TypeDescriptor.GetConverter (sourceType);

			if ((converter != null) &&
				(converter.CanConvertTo (expectedType)))
			{
				return converter.ConvertTo (null, culture, value, expectedType);
			}
		
			//	There was no converter between source type and expected type.
			//	But maybe there is a reverse converter between the expected type
			//	and the source type :
			
			converter = System.ComponentModel.TypeDescriptor.GetConverter (expectedType);

			if ((converter != null) &&
				(converter.CanConvertFrom (sourceType)))
			{
				return converter.ConvertFrom (null, culture, value);
			}

			//	No converter found -- give up

			return InvalidValue.Value;
		}

		public static readonly AutomaticValueConverter Instance = new AutomaticValueConverter ();
	}
}
