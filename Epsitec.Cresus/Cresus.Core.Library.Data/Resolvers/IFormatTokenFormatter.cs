//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Helpers;
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>IFormatTokenFormatter</c> interface is used to format a piece of
	/// information. See also the <see cref="FormattedIdGenerator"/> and the
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
