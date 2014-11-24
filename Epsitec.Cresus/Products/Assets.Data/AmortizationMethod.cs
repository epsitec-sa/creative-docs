//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public enum AmortizationMethod
	{
		Unknown,

		None,				// pas d'amortissement
		RateLinear,			// amortissement classique selon un taux linéaire
		RateDegressive,		// amortissement classique selon un taux dégressif
		YearsLinear,		// amortissement selon une durée en années linéaire
		YearsDegressive,	// amortissement selon une durée en années dégressif
		Custom,				// amortissement selon une expression C# personnalisée
	}
}