//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	[DesignerVisible]
	public enum VatCode
	{
		None = 0,

		Excluded    = 10,										//	EXCLU, exclu de l'impôt LTVA art. 21 (http://www.admin.ch/ch/f/rs/641_20/index.html) et (http://www.epsitec.ch/support/faq/cc-tva-06)
		ZeroRated   = 11,										//	EXONERE et EXPORT, exonéré LTVA art. 23
		
		StandardTax = 12,										//	taux normal (IPM/IPI/TVA selon le contexte)
		ReducedTax  = 13,										//	taux réduit (IPMRED/IPIRED/TVARED selon le contexte)
		SpecialTax  = 14,										//	taux spécial (IPMHEB/IPIHEB/TVAHEB selon le contexte)


		StandardInputTaxOnMaterialOrServiceExpenses = 20,		//	IPM
		ReducedInputTaxOnMaterialOrServiceExpenses = 21,		//	IPMRED
		SpecialInputTaxOnMaterialOrServiceExpenses = 22,		//	IPMHEB (n'existe pas)

		StandardInputTaxOnInvestementOrOperatingExpenses = 30,	//	IPI
		ReducedInputTaxOnInvestementOrOperatingExpenses = 31,	//	IPIRED
		SpecialInputTaxOnInvestementOrOperatingExpenses = 32,	//	IP(I)HEB

		StandardTaxOnTurnover = 40,								//	TVA
		ReducedTaxOnTurnover = 41,								//	TVARED
		SpecialTaxOnTurnover = 42,								//	TVAHEB
	}
}