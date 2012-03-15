//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class TVA
	{
		public static decimal CalculeTTC(decimal ht, decimal taux)
		{
			return Converters.RoundMontant (ht + ht*taux);
		}

		public static decimal CalculeHT(decimal ttc, decimal taux)
		{
			return Converters.RoundMontant (ttc / (1+taux));
		}

		public static decimal CalculeTVA(decimal ht, decimal taux)
		{
			return Converters.RoundMontant (ht*taux);
		}
	}
}
