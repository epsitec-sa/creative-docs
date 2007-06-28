using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Types
{
	public static class Converter
	{
		public static string ConvertToString<T>(T value)
		{
			return (string) Converters.AutomaticValueConverter.Instance.Convert (value, typeof (string), null, System.Globalization.CultureInfo.InvariantCulture);
		}

		public static T ConvertFromString<T>(string value)
		{
			return (T) Converters.AutomaticValueConverter.Instance.ConvertBack (value, typeof (T), null, System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
