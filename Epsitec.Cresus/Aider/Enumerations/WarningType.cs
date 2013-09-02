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
		None = 0,

		Generic = 1,

		Mismatch = 1000,
		ParishMismatch = 1001,
		ParishArrival = 1002,
		ParishDeparture = 1003,
		
		EChPersonNew = 2000,
		EChPersonMissing = 2001,
		EChHouseholdMissing = 2002,
		EChPersonDataChanged = 2003,
		EChHouseholdChanged = 2004,
		EChHouseholdAdded = 2005,
		
		EChAddressChanged = 3000,
		
		EChProcessDeparture = 3001,
		EChProcessArrival = 3002,

		EChPersonDuplicated = 4000
	}
}
