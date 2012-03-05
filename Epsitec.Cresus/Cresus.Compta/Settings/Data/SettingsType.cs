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

		EcritureMontantZéro,
		EcriturePièces,
		EcritureAutoPièces,
		EcriturePlusieursPièces,
		EcritureForcePièces,
		EcritureMultiEditionLineCount,
		EcritureTVA,
		EcritureMontreCompteTVA,
		EcritureEditeMontantTVA,

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
