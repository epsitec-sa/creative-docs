//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public enum EntryScenario
	{
		None,

		PreInput,			// financement préalable
		Purchase,			// achat
		Sale,				// vente
		AmortizationAuto,
		AmortizationExtra,
		Increase,
		Decrease,
		Adjust,
	}
}
