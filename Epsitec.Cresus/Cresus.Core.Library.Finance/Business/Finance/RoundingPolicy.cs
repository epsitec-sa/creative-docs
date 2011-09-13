//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	[DesignerVisible]
	public enum RoundingPolicy
	{
		None					= 0,
		All						= 1,

		OnUnitPriceBeforeTax	= 2,
		OnUnitPriceAfterTax		= 3,

		OnLinePriceBeforeTax	= 4,
		OnLinePriceAfterTax		= 5,
	}
}