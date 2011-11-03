//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	[System.Flags]
	public enum EntityStatus
	{
		None = 0,

		Empty = 0x01,
		Valid = 0x02,

		PartiallyCreated = 0x04,
	}
}
