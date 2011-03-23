//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public interface IPrettyPrinter
	{
		IEnumerable<System.Type> GetConvertibleTypes();
		FormattedText ToFormattedText(object value, System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel);
	}
}
