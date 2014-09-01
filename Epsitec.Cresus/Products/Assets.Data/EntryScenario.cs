//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.Data
{
	[DesignerVisible]
	public enum EntryScenario
	{
		None,

		Purchase,			// achat
		Sale,				// vente
		AmortizationAuto,
		AmortizationExtra,
		Revaluation,
	}
}
