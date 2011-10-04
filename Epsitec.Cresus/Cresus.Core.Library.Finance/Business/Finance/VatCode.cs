//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
#if false
	[DesignerVisible]
	public enum VatCode
	{
		None = 0,
		
		StandardInputTaxOnMaterialOrServiceExpenses = 21,		//	IPM
		ReducedInputTaxOnMaterialOrServiceExpenses = 22,		//	IPMRED
		SpecialInputTaxOnMaterialOrServiceExpenses = 23,		//	IPMHEB (n'existe pas)

		StandardInputTaxOnInvestementOrOperatingExpenses = 31,	//	IPI
		ReducedInputTaxOnInvestementOrOperatingExpenses = 32,	//	IPIRED
		SpecialInputTaxOnInvestementOrOperatingExpenses = 33,	//	IP(I)HEB

		StandardTaxOnTurnover = 41,								//	TVA
		ReducedTaxOnTurnover = 42,								//	TVARED
		SpecialTaxOnTurnover = 43,								//	TVAHEB

		Excluded    = 100,										//	EXCLU, exclu de l'impôt LTVA art. 21 (http://www.admin.ch/ch/f/rs/641_20/index.html) et (http://www.epsitec.ch/support/faq/cc-tva-06)
		ZeroRated   = 110,										//	EXONERE et EXPORT, exonéré LTVA art. 23
	}
#endif
}