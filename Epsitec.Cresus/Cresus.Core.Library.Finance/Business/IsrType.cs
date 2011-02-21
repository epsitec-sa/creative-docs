//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business.Finance;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// </summary>
	/// <remarks>See http://www.postfinance.ch/medialib/pf/en/doc/consult/manual/dlserv/inpayslip_isr_man.Par.0001.File.pdf paragraph 3.5.5.3.</remarks>
	public enum IsrType
	{
		Invalid,

		Code01_Isr_Chf								=  1,
		Code03_CashOnDelivery_Chf					=  3,
		Code04_IsrPlus_Chf							=  4,
		Code11_IsrForCreditToOwnAccount_Chf			= 11,
		Code14_IsrPlusForCreditToOwnAccount_Chf		= 14,
		Code21_Isr_Eur								= 21,
		Code23_IsrForCreditToOwnAccount_Eur			= 23,
		Code31_IsrPlus_Eur							= 31,
		Code33_IsrPlusForCreditToOwnAccount_Eur		= 33,
	}
}
