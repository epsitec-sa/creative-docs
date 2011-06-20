//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public static class Numeric
	{
		public static readonly decimal MaxMonetaryAmount = 1000000000;  // en francs, 1'000'000'000.-, soit 1 milliard
		
		public static readonly DecimalRange MonetaryRange = new DecimalRange (-Numeric.MaxMonetaryAmount+0.01M, Numeric.MaxMonetaryAmount-0.01M, 0.01M);
	}
}
