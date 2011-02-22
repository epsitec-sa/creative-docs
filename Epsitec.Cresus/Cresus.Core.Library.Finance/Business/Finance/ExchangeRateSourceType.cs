//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business.Finance
{
	/// <summary>
	/// The <c>ExchangeRateType</c> defines who specifies the exchange rate.
	/// See §1.1.3.1 of the document "Info TVA 7".
	/// </summary>
	[DesignerVisible]
	public enum ExchangeRateSourceType
	{
		Unknown,

		FederalMonthlyRate,					//	PDF http://www.estv.admin.ch/mwst/dienstleistungen/00304/00308/index.html?lang=fr
											//	HTML http://www.estv.admin.ch/mwst/dienstleistungen/00304/00308/00692/index.html?lang=fr
		FederalDailyRate,					//	XML http://www.afd.admin.ch/publicdb/newdb/mwst_kurse/wechselkurse.php

		GroupSpecific,
		BankSpecific,
	}
}
