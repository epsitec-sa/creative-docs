//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	[DesignerVisible]
	[System.Flags]
	public enum PriceDisplayModes
	{
		None			= 0,

		Discount		= 0x0001,
		PrimaryTotal	= 0x0002,
		FixedPrice		= 0x0004,
		Tax				= 0x0008,
		ResultingTotal	= 0x0010,

		WithTax			= 0x1000,
	}
}
