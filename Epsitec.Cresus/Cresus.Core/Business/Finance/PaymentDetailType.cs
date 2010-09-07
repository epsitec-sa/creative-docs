//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	[DesignerVisible]
	public enum PaymentDetailType
	{
		None			= 0,

		AmountDue		= 1,					//	montant dû
		AmountPaid		= 2,					//	montant payé ou encaissé
	}
}
