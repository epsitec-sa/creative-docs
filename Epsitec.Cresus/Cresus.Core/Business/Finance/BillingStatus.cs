//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	[DesignerVisible]
	public enum BillingStatus
	{
		None				= 0,

		NotAnInvoice		= 1,				//	ceci n'est pas (encore) une facture

		DebtorBillOpen		= 2,				//	facture débiteur ouverte
		DebtorBillClosed	= 3,				//	facture débiteur fermée

		CreditorBillOpen	= 8,				//	facture créancier/fournisseur ouverte
		CreditorBillClosed	= 9,				//	facture créancier/fournisseur fermée
	}
}
