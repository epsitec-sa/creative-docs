//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	public enum ExchangeRateType
	{
		Unknown,

		FederalMonthlyRate,					//	PDF http://www.estv.admin.ch/mwst/dienstleistungen/00304/00308/index.html?lang=fr
		FederalDailyRate,					//	XML http://www.afd.admin.ch/publicdb/newdb/mwst_kurse/wechselkurse.php

		GroupSpecific,
		BankSpecific,
	}
}
