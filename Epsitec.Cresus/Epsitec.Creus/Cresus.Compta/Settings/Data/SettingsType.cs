//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		EcritureMontantZ�ro,
		EcriturePi�ces,
		EcritureAutoPi�ces,
		EcriturePlusieursPi�ces,
		EcritureForcePi�ces,
		EcritureMultiEditionLineCount,
		EcritureTVA,
		EcritureEditeMontantTVA,
		EcritureEditeMontantHT,
		EcritureEditeCompteTVA,
		EcritureEditeCodeTVA,
		EcritureEditeTauxTVA,
		EcritureMontreType,
		EcritureMontreOrigineTVA,
		EcritureProposeVide,
		
		PriceDecimalSeparator,
		PriceGroupSeparator,
		PriceNullParts,
		PriceNegativeFormat,
		PriceSample,

		PercentFracFormat,
		PercentDecimalSeparator,
		PercentSample,

		DateSeparator,
		DateYear,
		DateOrder,
		DateSample,
	}
}
