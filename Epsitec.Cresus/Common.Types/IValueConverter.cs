//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public interface IValueConverter
	{
		object Convert(object value, System.Type expectedType, object parameter, System.Globalization.CultureInfo culture);
		object ConvertBack(object value, System.Type expectedType, object parameter, System.Globalization.CultureInfo culture);
	}
}
