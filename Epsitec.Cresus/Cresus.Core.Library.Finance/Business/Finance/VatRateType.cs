//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	[DesignerVisible]
	public enum VatRateType
	{
		None		= 0,
		
		StandardTax = 1,						//	taux normal (IPM/IPI/TVA selon le contexte)
		ReducedTax  = 2,						//	taux réduit (IPMRED/IPIRED/TVARED selon le contexte)
		SpecialTax  = 3,						//	taux spécial (IPMHEB/IPIHEB/TVAHEB selon le contexte)
	}
}
