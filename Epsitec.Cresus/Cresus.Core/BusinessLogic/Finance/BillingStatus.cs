//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Finance
{
	[DesignerVisible]
	public enum BillingStatus
	{
		None				= 0,

		DebtorBillOpen		= 2,				//	facture d�biteur ouverte
		DebtorBillClosed	= 3,				//	facture d�biteur ferm�e

		CreditorBillOpen	= 8,				//	facture cr�ancier/fournisseur ouverte
		CreditorBillClosed	= 9,				//	facture cr�ancier/fournisseur ferm�e
	}
}
