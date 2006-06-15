//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		/// <returns>The converted value or <see cref="T:InvalidValue.Instance"/>
		/// if the conversion was not possible.</returns>
		public object Convert(object value, System.Type expectedType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				if (expectedType == null)
				{
					throw new System.ArgumentNullException ("expectedType", "Expected type is null");
				}

				System.Type sourceType = value == null ? null : value.GetType ();

				if ((sourceType != null) &&
					(sourceType.IsEnum))
				{
					System.Diagnostics.Debug.WriteLine ("Convert enum value " + value.ToString () + " to type " + expectedType.Name);
				}
				
				return System.Convert.ChangeType (value, expectedType, culture);
			}
			catch (System.InvalidCastException)
			{
				return InvalidValue.Instance;
			}
			catch (System.FormatException)
			{
				return InvalidValue.Instance;
			}
		}

		/// <summary>
		/// Converts the specified value back to the expected type.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="expectedType">The expected type.</param>
		/// <param name="parameter">The optional parameter (not used).</param>
		/// <param name="culture">The culture used for the conversion.</param>
		/// <returns>The converted value or <see cref="T:InvalidValue.Instance"/>
		/// if the conversion was not possible.</returns>
		public object ConvertBack(object value, System.Type expectedType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				if (expectedType == null)
				{
					throw new System.ArgumentNullException ("expectedType", "Expected type is null");
				}

				System.Type sourceType = value == null ? null : value.GetType ();

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

				return System.Convert.ChangeType (value, expectedType, culture);
			}
			catch (System.InvalidCastException)
			{
				return InvalidValue.Instance;
			}
			catch (System.FormatException)
			{
				return InvalidValue.Instance;
			}
		}

		#endregion

		public static readonly AutomaticValueConverter Instance = new AutomaticValueConverter ();
	}
}
