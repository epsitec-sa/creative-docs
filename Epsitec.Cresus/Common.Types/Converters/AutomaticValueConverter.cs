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
