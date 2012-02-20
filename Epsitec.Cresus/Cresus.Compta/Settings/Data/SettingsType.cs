//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	public enum SettingsType
	{
		GlobalTitre,
		GlobalDescription,
		GlobalRemoveConfirmation,

		EcriturePièces,
		EcritureAutoPièces,
		EcritureProchainePièce,
		EcritureIncrémentPièce,
		EcriturePlusieursPièces,
		EcritureForcePièces,

		PriceDecimalDigits,
		PriceDecimalSeparator,
		PriceGroupSeparator,
		PriceNullParts,
		PriceNegativeFormat,
		PriceSample,

		DateSeparator,
		DateYear,
		DateOrder,
		DateSample,
	}
}
