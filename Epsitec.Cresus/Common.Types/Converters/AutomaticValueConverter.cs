//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Converters
{
	public class AutomaticValueConverter : IValueConverter
	{
		private AutomaticValueConverter()
		{
		}
		
		#region IValueConverter Members

		public object Convert(object value, System.Type expectedType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				return System.Convert.ChangeType (value, expectedType, culture);
			}
			catch (System.InvalidCastException)
			{
				return Binding.DoNothing;
			}
			catch (System.FormatException)
			{
				return Binding.DoNothing;
			}
		}

		public object ConvertBack(object value, System.Type expectedType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				return System.Convert.ChangeType (value, expectedType, culture);
			}
			catch (System.InvalidCastException)
			{
				return Binding.DoNothing;
			}
			catch (System.FormatException)
			{
				return Binding.DoNothing;
			}
		}

		#endregion

		public static AutomaticValueConverter Instance
		{
			get
			{
				return AutomaticValueConverter.instance;
			}
		}

		private static AutomaticValueConverter instance = new AutomaticValueConverter ();
	}
}
