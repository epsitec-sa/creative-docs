//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum WarningType
	{
		[Hidden]
		None = 0,

		[Hidden]
		Generic = 1,

		[Hidden]
		HouseholdMissing	 = 1000,

		[Hidden]
		[Rank (5)]
		ParishMismatch		 = 1001,

		[Rank (3)]
		ParishArrival		 = 1002,

		[Rank (4)]
		ParishDeparture		 = 1003,
		
		[Rank (10)]
		SubscriptionMissing  = 1004,
		
		[Hidden]
		EChPersonNew         = 2000,
		
		[Hidden]
		EChPersonMissing     = 2001,
		
		[Hidden]
		[Rank (6)]
		EChHouseholdMissing  = 2002,
		
		[Rank (9)]
		EChPersonDataChanged = 2003,
		
		[Hidden]
		EChHouseholdChanged  = 2004,

		[Hidden]
		[Rank (7)]
		EChHouseholdAdded    = 2005,
		
		[Rank (8)]
		EChAddressChanged    = 3000,
		
		[Rank (2)]
		EChProcessDeparture  = 3001,
		
		[Rank (1)]
		EChProcessArrival    = 3002,

		[Hidden]
		EChPersonDuplicated  = 4000,
		
		//	Not yet fully implemented...
		[Hidden]
		PersonProbablyDuplicated  = 4001
	}
}