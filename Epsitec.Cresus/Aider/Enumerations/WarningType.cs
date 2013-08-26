//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		Duplicated = 2000,
		MissingECh = 2001,
		NoHouseholdECh = 2002,
		DataChangedECh = 2003,
		AddressChange = 3000,
        DepartureProcessNeeded = 3001,
        ArrivalProcessNeeded = 3002
	}
}
