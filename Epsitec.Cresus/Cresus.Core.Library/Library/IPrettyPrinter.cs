//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public interface IPrettyPrinter
	{
		bool CanConvertToFormattedText(System.Type type);
		FormattedText ConvertToFormattedText(object value, System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel);
	}
}
