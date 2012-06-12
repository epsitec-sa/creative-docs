//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Formatters
{
	/// <summary>
	/// The <c>IFormatTokenFormatter</c> interface is used to format a piece of
	/// information. See also the class <c>FormattedIdGenerator</c> and the
	/// <see cref="FormatTokenFormatterResolver"/>.
	/// </summary>
	public interface IFormatTokenFormatter
	{
		/// <summary>
		/// Gets the format token of this formatter.
		/// </summary>
		/// <returns>The <see cref="FormatToken"/> of this formatter.</returns>
		FormatToken GetFormatToken();
	}
}
