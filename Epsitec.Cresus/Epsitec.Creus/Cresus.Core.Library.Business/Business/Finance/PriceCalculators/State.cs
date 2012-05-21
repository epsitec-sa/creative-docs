//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	/// <summary>
	/// The <c>State</c> is used inside the price calculators to remember which
	/// type of lines are currently being processed.
	/// </summary>
	internal enum State
	{
		None,
		Article,
		SubTotal,
	}
}
