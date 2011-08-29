//	Copyright � 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	/// <summary>
	/// The <c>PaymentDetailType</c> enumeration defines the three types of
	/// payments: amount due, amount paid and discount.
	/// </summary>
	[DesignerVisible]
	public enum PaymentDetailType
	{
		None			= 0,

		Due				= 1,					//	montant d�
		Paid			= 2,					//	montant pay� ou encaiss�
		Discount		= 3,					//	montant escompt�
	}
}
